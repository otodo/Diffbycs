using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using System.IO;

using adelieworks;

namespace csdiff
{
	/// <summary>
	/// Form1 �̊T�v�̐����ł��B
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private Document document = null;
		private Option option = null;
		private View view1 = null;
		private FnameBar fnameBar = null;
		private RecentFileList mru = null;

		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem menuItem8;

		private System.Windows.Forms.HelpProvider helpProvider1;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItem9;
		private System.Windows.Forms.MenuItem menuItem11;
		private System.Windows.Forms.MenuItem miFileMRU;
		private System.Windows.Forms.MenuItem miFile;
		private System.Windows.Forms.MenuItem miEdit;
		private System.Windows.Forms.MenuItem miEditCopy;
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.MenuItem miContextCopy;
		private System.Windows.Forms.ToolBarButton tbbtnCopy;
		private System.Windows.Forms.ToolBarButton toolBarButton2;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem miEditPrevDiff;
		private System.Windows.Forms.MenuItem miEditNextDiff;
		private System.Windows.Forms.ToolBarButton tbbtnPrevDiff;
		private System.Windows.Forms.ToolBarButton tbbtnNextDiff;
		private System.Windows.Forms.ToolBarButton tbbtnFind;
		private System.Windows.Forms.MenuItem menuItem10;

		public Form1()
		{
			document = new Document();
			option  = new Option();

			view1 = new View(document,option);
			view1.Parent = this;
			view1.Dock = DockStyle.Fill;
			view1.Show();

			fnameBar = new FnameBar(document,option);
			fnameBar.Parent = this;
			fnameBar.Dock = DockStyle.Top;
			fnameBar.Show();

			InitializeComponent();	// Windows �t�H�[�� �f�U�C�i �T�|�[�g�ɕK�v�ł��B

			// MRU���X�g��ǉ�
			mru = new csdiff.RecentFileList();
			mru.Install( this, miFileMRU, statusBar1 );
			mru.MenuClick += new MRUEventHandler(this.OnMRUClick);

			// �c�[���o�[�̗L����Ԃ��X�V���邽�߂ɃA�C�h���C�x���g���n���h��
			Application.Idle += new EventHandler(application_Idle);

			// �w���v�t�@�C���̃t���p�X������(�A�v���P�[�V�����̂���t�H���_)
			helpProvider1.HelpNamespace = Path.Combine(Application.StartupPath,helpProvider1.HelpNamespace);
		}

		/// <summary>
		/// �g�p����Ă��郊�\�[�X�Ɍ㏈�������s���܂��B
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// �f�U�C�i �T�|�[�g�ɕK�v�ȃ��\�b�h�ł��B���̃��\�b�h�̓��e��
		/// �R�[�h �G�f�B�^�ŕύX���Ȃ��ł��������B
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Form1));
			this.statusBar1 = new System.Windows.Forms.StatusBar();
			this.toolBar1 = new System.Windows.Forms.ToolBar();
			this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
			this.tbbtnCopy = new System.Windows.Forms.ToolBarButton();
			this.tbbtnFind = new System.Windows.Forms.ToolBarButton();
			this.tbbtnPrevDiff = new System.Windows.Forms.ToolBarButton();
			this.tbbtnNextDiff = new System.Windows.Forms.ToolBarButton();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.miFile = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.miFileMRU = new System.Windows.Forms.MenuItem();
			this.menuItem11 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.miEdit = new System.Windows.Forms.MenuItem();
			this.miEditCopy = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.miEditPrevDiff = new System.Windows.Forms.MenuItem();
			this.miEditNextDiff = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.menuItem9 = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.menuItem10 = new System.Windows.Forms.MenuItem();
			this.menuItem8 = new System.Windows.Forms.MenuItem();
			this.helpProvider1 = new System.Windows.Forms.HelpProvider();
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			this.miContextCopy = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// statusBar1
			// 
			this.statusBar1.Location = new System.Drawing.Point(0, 246);
			this.statusBar1.Name = "statusBar1";
			this.statusBar1.Size = new System.Drawing.Size(416, 22);
			this.statusBar1.TabIndex = 0;
			// 
			// toolBar1
			// 
			this.toolBar1.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
			this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																						this.toolBarButton1,
																						this.toolBarButton2,
																						this.tbbtnCopy,
																						this.tbbtnFind,
																						this.tbbtnPrevDiff,
																						this.tbbtnNextDiff});
			this.toolBar1.DropDownArrows = true;
			this.toolBar1.ImageList = this.imageList1;
			this.toolBar1.Name = "toolBar1";
			this.toolBar1.ShowToolTips = true;
			this.toolBar1.Size = new System.Drawing.Size(416, 24);
			this.toolBar1.TabIndex = 1;
			this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
			// 
			// toolBarButton1
			// 
			this.toolBarButton1.ImageIndex = 0;
			this.toolBarButton1.ToolTipText = "��r����t�@�C�����J��";
			// 
			// toolBarButton2
			// 
			this.toolBarButton2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// tbbtnCopy
			// 
			this.tbbtnCopy.ImageIndex = 1;
			this.tbbtnCopy.ToolTipText = "�I���s���R�s�[";
			// 
			// tbbtnFind
			// 
			this.tbbtnFind.ImageIndex = 4;
			this.tbbtnFind.ToolTipText = "�����������";
			this.tbbtnFind.Visible = false;
			// 
			// tbbtnPrevDiff
			// 
			this.tbbtnPrevDiff.ImageIndex = 2;
			this.tbbtnPrevDiff.ToolTipText = "�O�̑���_";
			// 
			// tbbtnNextDiff
			// 
			this.tbbtnNextDiff.ImageIndex = 3;
			this.tbbtnNextDiff.ToolTipText = "���̑���_";
			// 
			// imageList1
			// 
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(16, 15);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Silver;
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.miFile,
																					  this.miEdit,
																					  this.menuItem6,
																					  this.menuItem9});
			// 
			// miFile
			// 
			this.miFile.Index = 0;
			this.miFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				   this.menuItem2,
																				   this.menuItem3,
																				   this.miFileMRU,
																				   this.menuItem11,
																				   this.menuItem4});
			this.miFile.Text = "�t�@�C��(&F)";
			this.miFile.Select += new System.EventHandler(this.Form1_MenuComplete);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 0;
			this.menuItem2.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
			this.menuItem2.Text = "�J��(&O)...";
			this.menuItem2.Click += new System.EventHandler(this.OnFileOpen);
			this.menuItem2.Select += new System.EventHandler(this.menuItem2_Select);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 1;
			this.menuItem3.Text = "-";
			// 
			// miFileMRU
			// 
			this.miFileMRU.Enabled = false;
			this.miFileMRU.Index = 2;
			this.miFileMRU.Text = "�i�ŋߎg�����t�@�C���j";
			// 
			// menuItem11
			// 
			this.menuItem11.Index = 3;
			this.menuItem11.Text = "-";
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 4;
			this.menuItem4.Text = "�I��(&X)";
			this.menuItem4.Click += new System.EventHandler(this.OnFileExit);
			this.menuItem4.Select += new System.EventHandler(this.menuItem4_Select);
			// 
			// miEdit
			// 
			this.miEdit.Index = 1;
			this.miEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				   this.miEditCopy,
																				   this.menuItem1,
																				   this.miEditPrevDiff,
																				   this.miEditNextDiff});
			this.miEdit.Text = "�ҏW(&E)";
			this.miEdit.Popup += new System.EventHandler(this.miEdit_Popup);
			// 
			// miEditCopy
			// 
			this.miEditCopy.Index = 0;
			this.miEditCopy.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
			this.miEditCopy.Text = "�R�s�[";
			this.miEditCopy.Click += new System.EventHandler(this.miEditCopy_Click);
			this.miEditCopy.Select += new System.EventHandler(this.miEditCopy_Select);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 1;
			this.menuItem1.Text = "-";
			// 
			// miEditPrevDiff
			// 
			this.miEditPrevDiff.Index = 2;
			this.miEditPrevDiff.Shortcut = System.Windows.Forms.Shortcut.ShiftF7;
			this.miEditPrevDiff.Text = "�O�̑���_(&P)";
			this.miEditPrevDiff.Click += new System.EventHandler(this.miEditPrevDiff_Click);
			this.miEditPrevDiff.Select += new System.EventHandler(this.miEditPrevDiff_Select);
			// 
			// miEditNextDiff
			// 
			this.miEditNextDiff.Index = 3;
			this.miEditNextDiff.Shortcut = System.Windows.Forms.Shortcut.F7;
			this.miEditNextDiff.Text = "���̑���_(&N)";
			this.miEditNextDiff.Click += new System.EventHandler(this.miEditNextDiff_Click);
			this.miEditNextDiff.Select += new System.EventHandler(this.miEditNextDiff_Select);
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 2;
			this.menuItem6.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem7});
			this.menuItem6.Text = "���̑�(&O)";
			this.menuItem6.Select += new System.EventHandler(this.Form1_MenuComplete);
			// 
			// menuItem7
			// 
			this.menuItem7.Index = 0;
			this.menuItem7.Text = "�ݒ�(&O)...";
			this.menuItem7.Click += new System.EventHandler(this.menuItem7_Click);
			this.menuItem7.Select += new System.EventHandler(this.menuItem7_Select);
			// 
			// menuItem9
			// 
			this.menuItem9.Index = 3;
			this.menuItem9.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem5,
																					  this.menuItem10,
																					  this.menuItem8});
			this.menuItem9.Text = "�w���v(&H)";
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 0;
			this.menuItem5.Text = "�ڎ�(&C)";
			this.menuItem5.Click += new System.EventHandler(this.menuItem5_Click);
			// 
			// menuItem10
			// 
			this.menuItem10.Index = 1;
			this.menuItem10.Text = "-";
			// 
			// menuItem8
			// 
			this.menuItem8.Index = 2;
			this.menuItem8.Text = "�o�[�W�������(&A)...";
			this.menuItem8.Click += new System.EventHandler(this.menuItem8_Click);
			this.menuItem8.Select += new System.EventHandler(this.menuItem8_Select);
			// 
			// helpProvider1
			// 
			this.helpProvider1.HelpNamespace = "csdiff.chm";
			// 
			// contextMenu1
			// 
			this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.miContextCopy});
			this.contextMenu1.Popup += new System.EventHandler(this.contextMenu1_Popup);
			// 
			// miContextCopy
			// 
			this.miContextCopy.Index = 0;
			this.miContextCopy.Text = "�R�s�[";
			this.miContextCopy.Click += new System.EventHandler(this.miEditCopy_Click);
			// 
			// Form1
			// 
			this.AllowDrop = true;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 12);
			this.ClientSize = new System.Drawing.Size(416, 268);
			this.ContextMenu = this.contextMenu1;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.toolBar1,
																		  this.statusBar1});
			this.Font = new System.Drawing.Font("�l�r �S�V�b�N", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Menu = this.mainMenu1;
			this.Name = "Form1";
			this.Text = "diff by C#";
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
			this.MenuComplete += new System.EventHandler(this.Form1_MenuComplete);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// �A�v���P�[�V�����̃��C�� �G���g�� �|�C���g�ł��B
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.Run(new Form1());
		}

		#region ���j���[�I�����̃X�e�[�^�X�o�[�ւ�HelpString�\��
		/// <summary>
		/// ���j���[�I�����̃X�e�[�^�X�o�[�ւ�HelpString�\��
		/// </summary>
		private void Form1_MenuComplete(object sender, System.EventArgs e)
		{
			statusBar1.Text = "";
		}
		private void menuItem4_Select(object sender, System.EventArgs e)
		{
			statusBar1.Text = "�A�v���P�[�V�������I�����܂��B";
		}
		private void menuItem2_Select(object sender, System.EventArgs e)
		{
			statusBar1.Text = "��r����2�̃e�L�X�g�t�@�C����I�����܂��B";
		}
		private void miEditCopy_Select(object sender, System.EventArgs e)
		{
			statusBar1.Text = "�I������Ă���s���N���b�v�{�[�h�փR�s�[���܂��B";
		}
		private void miEditFind_Select(object sender, System.EventArgs e)
		{
			statusBar1.Text = "��������������܂��B";
		}
		private void miEditPrevDiff_Select(object sender, System.EventArgs e)
		{
			statusBar1.Text = "�O�̑���_�܂Ō��ݍs���ړ����܂��B";
		}
		private void miEditNextDiff_Select(object sender, System.EventArgs e)
		{
			statusBar1.Text = "���̑���_�܂Ō��ݍs���ړ����܂��B";
		}
		private void menuItem7_Select(object sender, System.EventArgs e)
		{
			statusBar1.Text = "����̐ݒ���s���܂��B";
		}
		private void menuItem8_Select(object sender, System.EventArgs e)
		{
			statusBar1.Text = "�o�[�W��������\�����܂��B";
		}
		#endregion

		#region ���j���[��c�[���{�^���Ȃǂ̏�Ԃ��X�V
		private void application_Idle(object sender,System.EventArgs e)
		{
			ListBox.SelectedIndexCollection selecteds = view1.SelectedIndices;
			tbbtnCopy.Enabled = ( selecteds.Count != 0 );
			this.tbbtnFind.Enabled     = document.IsLoadedAll();
			this.tbbtnPrevDiff.Enabled = document.IsLoadedAll();
			this.tbbtnNextDiff.Enabled = document.IsLoadedAll();
		}
		private void miEdit_Popup(object sender, System.EventArgs e)
		{
			miEditCopy.Enabled = ( view1.SelectedIndex != -1 );
			//!miEditFind.Enabled     = document.IsLoadedAll();
			miEditPrevDiff.Enabled = document.IsLoadedAll();
			miEditNextDiff.Enabled = document.IsLoadedAll();
		}
		private void contextMenu1_Popup(object sender, System.EventArgs e)
		{
			miContextCopy.Enabled = ( view1.SelectedIndex != -1 );
		}

		#endregion

		private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			switch (toolBar1.Buttons.IndexOf(e.Button))
			{
				case 0:	OnFileOpen(sender,new System.EventArgs());
						break;
				case 2: miEditCopy_Click(sender,new System.EventArgs());
						break;
				case 4: miEditPrevDiff_Click(sender,new System.EventArgs());
						break;
				case 5: miEditNextDiff_Click(sender,new System.EventArgs());
					break;
			}
		}

		private void OnMRUClick(object sender, MRUEventArgs e)
		{
			string fnameLeftAndRighth = e.filename;
			string[] fnames = fnameLeftAndRighth.Split('/');
			OpenFileBoth( fnames[0], fnames[1] );
		}

		private void OnFileOpen(object sender, System.EventArgs e)
		{
			csdiff.openFileDlg dlg = new csdiff.openFileDlg();
			DialogResult result = dlg.ShowDialog(this);
			if( result != DialogResult.OK ) return;

			OpenFileBoth( dlg.edFilePath1.Text, dlg.edFilePath2.Text );
		}

		private void OnFileExit(object sender, System.EventArgs e)
		{
			Close();
		}

		protected void OpenFileBoth(string leftname, string rightname)
		{
			if( leftname  != null ){
				document.Load( DocOf.LEFT, leftname );
			}
			if( rightname != null ){
				document.Load( DocOf.RIGHT, rightname );
			}
			fnameBar.Invalidate();

			if( document.IsLoadedAll() )
			{
				mru.Add( document.fnames[(int)DocOf.LEFT] + "/" + document.fnames[(int)DocOf.RIGHT] );
				document.UpdateCompositSection( option.bIgnoreBlank );
				view1.InitialUpdate(true);
			}
		}

		private void menuItem8_Click(object sender, System.EventArgs e)
		{
			AboutBox dlg = new AboutBox();
			dlg.ShowDialog(this);
		}

		private void menuItem7_Click(object sender, System.EventArgs e)
		{
			OptionDlg dlg = new OptionDlg(option);
			DialogResult result = dlg.ShowDialog(this);
			if( result != DialogResult.OK ) return;

			document.UpdateCompositSection( option.bIgnoreBlank );
			view1.InitialUpdate(false);
			option.SaveState();
		}

		/// <summary>
		/// �I���s���N���b�v�{�[�h�փR�s�[
		/// </summary>
		private void miEditCopy_Click(object sender, System.EventArgs e)
		{
			string text = "";
			foreach( object item in view1.SelectedItems )
			{
				LineIndex lineidx = (LineIndex)item;
				text += lineidx.GetText() + "\r\n";
			}
			Clipboard.SetDataObject(text,true);
		}

		/// <summary>
		/// �w���v�̖ڎ���\��
		/// </summary>
		private void menuItem5_Click(object sender, System.EventArgs e)
		{
			Help.ShowHelp(this,helpProvider1.HelpNamespace,HelpNavigator.TableOfContents);
		}

		/// <summary>
		/// �O�̑���_�փW�����v
		/// </summary>
		private void miEditPrevDiff_Click(object sender, System.EventArgs e)
		{
			ListBox.SelectedIndexCollection selindexes = view1.SelectedIndices;
			if( selindexes.Count == 0 ) return;
			int nRow = selindexes[selindexes.Count-1];
			STATE state = ((LineIndex)view1.Items[nRow]).m_state;
			for( ; nRow>=0; nRow-- )
			{
				LineIndex lineidx = (LineIndex)view1.Items[nRow];
				if( lineidx.m_state != STATE.SAME &&
					lineidx.m_state != state )
				{
					state = lineidx.m_state;
					for( ; nRow>=0; nRow-- )
					{
						lineidx = (LineIndex)view1.Items[nRow];
						if( lineidx.m_state != state ) break;
					}
					nRow++;
					if( nRow >= view1.Items.Count ) nRow = view1.Items.Count-1;
					view1.ClearSelected();
					view1.SelectedIndex = nRow;
					break;
				}
			}
		}

		/// <summary>
		/// ���̑���_�փW�����v
		/// </summary>
		private void miEditNextDiff_Click(object sender, System.EventArgs e)
		{
			ListBox.SelectedIndexCollection selindexes = view1.SelectedIndices;
			int nRow = 0;
			if( selindexes.Count != 0 ) nRow = selindexes[selindexes.Count-1];
			STATE state = ((LineIndex)view1.Items[nRow]).m_state;
			for( ; nRow<view1.Items.Count; nRow++ )
			{
				LineIndex lineidx = (LineIndex)view1.Items[nRow];
				if( lineidx.m_state != STATE.SAME &&
					lineidx.m_state != state )
				{
					view1.ClearSelected();
					view1.SelectedIndex = nRow;
					break;
				}
			}
		}


		/// <summary>
		/// �G�N�X�v���[��������̃t�@�C���̃h���b�O&�h���b�v����t
		/// </summary>
		private void Form1_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if(e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
		}

		private void Form1_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			Array aFileNames = (Array)e.Data.GetData(DataFormats.FileDrop);
			if( aFileNames.Length < 2 )
			{
				// ���E�܂��ǂݍ��܂�Ă��Ȃ����Ńt�@�C�����J��
				if( document.IsLoaded(DocOf.LEFT) == false )
					OpenFileBoth((string)aFileNames.GetValue(0),null);
				else
					OpenFileBoth(null,(string)aFileNames.GetValue(0));
			}

			// ������2��(�ȏ�)�̃t�@�C�����h���b�v���ꂽ��A����2���r�̂��߂ɊJ��
			else OpenFileBoth((string)aFileNames.GetValue(0),(string)aFileNames.GetValue(1));
		}

	}


	public class RecentFileList : adelieworks.RecentFileList
	{
		public RecentFileList() : base(4)
		{
		}
		public override string GetDisplayName(int i)
		{
			string fnameLeftAndRight = base.GetDisplayName(i);
			string[] fnames = fnameLeftAndRight.Split('/');
			return Path.GetFileName(fnames[0]) +"/"+ Path.GetFileName(fnames[1]);
		}
	}
}
