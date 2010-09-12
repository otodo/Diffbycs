using System;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage; // �u���X�g���[�W
using System.Runtime.Serialization.Formatters.Binary; // �V���A���C�Y

namespace adelieworks
{
	/// <summary>
	/// RecentFileList �̊T�v�̐����ł��B
	/// </summary>
	public delegate void MRUEventHandler(object sender, MRUEventArgs e);

	public class RecentFileList
	{
		protected ArrayList filelist = null;
		protected int m_nMaxFiles;
		protected int m_nMruBegin = -1;
		protected int m_nMruEnd   = -1;
		protected bool   m_bAutoReadWrite = true;				// �����I�Ɋu���X�g���[�W�ɕۑ��E��������
		protected string m_sStoreName     = "recentfilelist";	// �ۑ���̖��O

		protected StatusBar m_statusBar = null;					// ���j���[�I�����̃��b�Z�[�W�\�����s���X�e�[�^�X�o�[
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
		/// MRU���X�g�ɒǉ�
		/// </summary>
		/// <param name="filename">�ǉ����鍀�ڂ̖��O</param>
		public virtual void Add(string filename)
		{
			if( m_bAutoReadWrite ) ReadList();

			// �o�^�ς݂łȂ����T��
			int nFindIndex = -1;
			for( int i=0; i<filelist.Count; i++ ){
				if( string.Compare( filename, (string)filelist[i], true ) == 0 ){
					nFindIndex = i;
					break;
				}
			}

			// ���o�^�݂Ȃ�A���̗v�f��1�Ԗڂɒǉ�����
			if( nFindIndex == -1 ){
				filelist.Insert( 0, filename );
				if( filelist.Count > m_nMaxFiles ) filelist.RemoveAt(filelist.Count-1);
			}

			// �o�^�ς݂Ȃ�A���̗v�f��1�ԖڂɈړ�����
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
		/// MRU���j���[���\�z����
		/// </summary>
		/// <param name="fileMenu">MRU���ڂ�ǉ�����|�b�v�A�b�v���j���[�A�C�e��(��ʓI�ɂ�[�t�@�C��(F)]���j���[)</param>
		/// <param name="insertAt">MRU���ڂ̒ǉ��ʒu���������j���[�A�C�e��(�������̃|�b�v�A�b�v���j���[�Ɋ܂܂�Ă��邱��)</param>
		public virtual void BuildMenu(MenuItem fileMenu,MenuItem insertAt)
		{
			if( m_bAutoReadWrite ) ReadList();
			if( filelist.Count == 0 ) return;

			// ������MRU�����j���[����폜
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

			// MRU���X�g���쐬
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
				m_statusBar.Text = "�ŋߎg�����t�@�C�����J���܂��B";
			}
		}
		protected void ownerForm_MenuStart(object sender, System.EventArgs e)
		{
			BuildMenu( m_ownerMenu, m_insertAt );
		}

		/// <summary>
		/// �u���X�g���[�W���t�@�C�����X�g�𕜌�
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
		/// �^����ꂽ�X�g���[������t�@�C�����X�g�𕜌�
		/// </summary>
		public virtual void ReadList(Stream stream)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			filelist = (ArrayList)formatter.Deserialize(stream);
			while( filelist.Count > m_nMaxFiles ) filelist.RemoveAt(filelist.Count-1);
		}

		/// <summary>
		/// �u���X�g���[�W�Ƀt�@�C�����X�g��ۑ�
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
		/// �^����ꂽ�X�g���[���Ƀt�@�C�����X�g��ۑ�
		/// </summary>
		public virtual void WriteList(Stream stream)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize( stream, filelist );
		}

		/// <summary>
		/// MRU���X�g�������I�Ɋu���X�g���[�W�ɕۑ�/��������
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
		/// MRU���X�g�̕ۑ��於
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
