using System;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage; // 隔離ストレージ
using System.Runtime.Serialization.Formatters.Binary; // シリアライズ

namespace adelieworks
{
	/// <summary>
	/// RecentFileList の概要の説明です。
	/// </summary>
	public delegate void MRUEventHandler(object sender, MRUEventArgs e);

	public class RecentFileList
	{
		protected ArrayList filelist = null;
		protected int m_nMaxFiles;
		protected int m_nMruBegin = -1;
		protected int m_nMruEnd   = -1;
		protected bool   m_bAutoReadWrite = true;				// 自動的に隔離ストレージに保存・復元する
		protected string m_sStoreName     = "recentfilelist";	// 保存先の名前

		protected StatusBar m_statusBar = null;					// メニュー選択時のメッセージ表示を行うステータスバー
		protected MenuItem  m_insertAt  = null;
		protected MenuItem  m_ownerMenu = null;

		public event MRUEventHandler MenuClick;


		public RecentFileList(int nMaxFiles)
		{
			m_nMaxFiles = nMaxFiles;
			filelist = new ArrayList();
		}

		public void Install(Form ownerForm,MenuItem insertAt,StatusBar statusBar)
		{
			m_insertAt  = insertAt;
			m_ownerMenu = (MenuItem)insertAt.Parent;
			ownerForm.MenuStart += new EventHandler(ownerForm_MenuStart);
			
			m_statusBar = statusBar;
		}

		/// <summary>
		/// MRUリストに追加
		/// </summary>
		/// <param name="filename">追加する項目の名前</param>
		public virtual void Add(string filename)
		{
			if( m_bAutoReadWrite ) ReadList();

			// 登録済みでないか探す
			int nFindIndex = -1;
			for( int i=0; i<filelist.Count; i++ ){
				if( string.Compare( filename, (string)filelist[i], true ) == 0 ){
					nFindIndex = i;
					break;
				}
			}

			// 未登録みなら、その要素を1番目に追加する
			if( nFindIndex == -1 ){
				filelist.Insert( 0, filename );
				if( filelist.Count > m_nMaxFiles ) filelist.RemoveAt(filelist.Count-1);
			}

			// 登録済みなら、その要素を1番目に移動する
			else if( nFindIndex != 0 ){
				filelist.RemoveAt(nFindIndex);
				filelist.Insert(0,filename);
			}

			if( m_bAutoReadWrite ) WriteList();
		}

		public virtual string GetDisplayName(int i)
		{
			return (string)filelist[i];
		}

		/// <summary>
		/// MRUメニューを構築する
		/// </summary>
		/// <param name="fileMenu">MRU項目を追加するポップアップメニューアイテム(一般的には[ファイル(F)]メニュー)</param>
		/// <param name="insertAt">MRU項目の追加位置を示すメニューアイテム(第一引数のポップアップメニューに含まれていること)</param>
		public virtual void BuildMenu(MenuItem fileMenu,MenuItem insertAt)
		{
			if( m_bAutoReadWrite ) ReadList();
			if( filelist.Count == 0 ) return;

			// 既存のMRUをメニューから削除
			int nInsertAt;
			if( m_nMruBegin != -1 ){
				for(int i=0; i<=m_nMruEnd-m_nMruBegin; i++ ){
					fileMenu.MenuItems.RemoveAt(m_nMruBegin);
				}
				nInsertAt = m_nMruBegin;
			}
			else{
				nInsertAt = insertAt.Index;
				fileMenu.MenuItems.RemoveAt(insertAt.Index);
			}

			// MRUリストを作成
			for( int i=0; i<filelist.Count; i++ ){
				string fname = GetDisplayName(i);
				MRUMenuItem item = new MRUMenuItem( fname, new EventHandler(mruItem_Click) );
				item.Select += new EventHandler(mruItem_Select);
				item.MRUIndex = i;
				if( i == 0 )                m_nMruBegin = nInsertAt+i;
				if( i == filelist.Count-1 ) m_nMruEnd   = nInsertAt+i;
				fileMenu.MenuItems.Add(nInsertAt+i,item);
			}
		}

		protected void mruItem_Click(object sender, System.EventArgs e)
		{
			MRUMenuItem item = (MRUMenuItem)sender;
			MRUEventArgs e2 = new MRUEventArgs();
			e2.mruIndex = item.MRUIndex;
			e2.filename = (string)filelist[e2.mruIndex];
			MenuClick( this, e2 );
		}

		protected void mruItem_Select(object sender, System.EventArgs e)
		{
			if( m_statusBar != null ){
				m_statusBar.Text = "最近使ったファイルを開きます。";
			}
		}
		protected void ownerForm_MenuStart(object sender, System.EventArgs e)
		{
			BuildMenu( m_ownerMenu, m_insertAt );
		}

		/// <summary>
		/// 隔離ストレージよりファイルリストを復元
		/// </summary>
		public virtual void ReadList()
		{
			IsolatedStorageFile isoStore = 
				IsolatedStorageFile.GetStore(IsolatedStorageScope.Assembly|IsolatedStorageScope.User|IsolatedStorageScope.Roaming,null,null);
			try{
				Stream stream = new IsolatedStorageFileStream(StoreName,FileMode.Open,isoStore);
				try{
					ReadList(stream);
				}
				finally{
					stream.Close();
				}
			}
			catch( FileNotFoundException ){}
		}
		/// <summary>
		/// 与えられたストリームからファイルリストを復元
		/// </summary>
		public virtual void ReadList(Stream stream)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			filelist = (ArrayList)formatter.Deserialize(stream);
			while( filelist.Count > m_nMaxFiles ) filelist.RemoveAt(filelist.Count-1);
		}

		/// <summary>
		/// 隔離ストレージにファイルリストを保存
		/// </summary>
		public virtual void WriteList()
		{
			IsolatedStorageFile isoStore = 
				IsolatedStorageFile.GetStore(IsolatedStorageScope.Assembly|IsolatedStorageScope.User|IsolatedStorageScope.Roaming,null,null);
			Stream stream = new IsolatedStorageFileStream(StoreName,FileMode.Create,isoStore);
			try{
				WriteList(stream);
			}
			finally{
				stream.Close();
			}
		}
		/// <summary>
		/// 与えられたストリームにファイルリストを保存
		/// </summary>
		public virtual void WriteList(Stream stream)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize( stream, filelist );
		}

		/// <summary>
		/// MRUリストを自動的に隔離ストレージに保存/復元する
		/// </summary>
		public bool AutoReadWrite 
		{
			get {
				return m_bAutoReadWrite;
			}
			set {
				m_bAutoReadWrite = value;
			}
		}
		/// <summary>
		/// MRUリストの保存先名
		/// </summary>
		public string StoreName
		{
			get {
				return m_sStoreName;
			}
			set {
				m_sStoreName = value;
			}
		}

	}

	public class MRUMenuItem : MenuItem
	{
		public int MRUIndex = -1;
		public MRUMenuItem(string txt,EventHandler evh) : base(txt,evh)
		{
		}
	}

	public class MRUEventArgs : EventArgs
	{
		public int mruIndex;
		public string filename;
	}
}
