using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.IO.IsolatedStorage; // 隔離ストレージ
using System.Runtime.Serialization.Formatters.Binary; // シリアライズ

namespace csdiff
{
	/// <summary>
	/// OptionDlg の概要の説明です。
	/// </summary>
	public class OptionDlg : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.CheckBox checkboxIgnoreBlank;
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;
		private Option option;

		public OptionDlg(Option paramOption)
		{
			option = paramOption;
			InitializeComponent();	// Windows フォーム デザイナ サポートに必要です。
			checkboxIgnoreBlank.Checked = option.bIgnoreBlank;
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.checkboxIgnoreBlank = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnOK.Location = new System.Drawing.Point(125, 80);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 0;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnCancel.Location = new System.Drawing.Point(208, 80);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "キャンセル";
			// 
			// checkboxIgnoreBlank
			// 
			this.checkboxIgnoreBlank.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkboxIgnoreBlank.Location = new System.Drawing.Point(22, 21);
			this.checkboxIgnoreBlank.Name = "checkboxIgnoreBlank";
			this.checkboxIgnoreBlank.Size = new System.Drawing.Size(186, 24);
			this.checkboxIgnoreBlank.TabIndex = 2;
			this.checkboxIgnoreBlank.Text = "空白は無視して比較する(&I)";
			// 
			// OptionDlg
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(292, 114);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.checkboxIgnoreBlank,
																		  this.btnCancel,
																		  this.btnOK});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OptionDlg";
			this.ShowInTaskbar = false;
			this.Text = "設定";
			this.ResumeLayout(false);

		}
		#endregion

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			option.bIgnoreBlank = checkboxIgnoreBlank.Checked;
		}
	}

	public class Option
	{
		public bool bIgnoreBlank;	// 空白を無視して比較
		public Color leftColor;		// テキスト1の表示色
		public Color rightColor;	// テキスト2の表示色
		private const string StoreName = "options";
		public Option()
		{
			// 既定値を設定
			this.bIgnoreBlank = true;
			this.leftColor    = Color.SkyBlue;
			this.rightColor   = Color.SandyBrown;

			IsolatedStorageFile isoStore = 
				IsolatedStorageFile.GetStore(IsolatedStorageScope.Assembly|IsolatedStorageScope.User|IsolatedStorageScope.Roaming,null,null);
			try{
				Stream stream = new IsolatedStorageFileStream(StoreName,FileMode.Open,isoStore);
				try{
					BinaryFormatter formatter = new BinaryFormatter();
					this.bIgnoreBlank = (bool) formatter.Deserialize(stream);
					this.leftColor    = (Color)formatter.Deserialize(stream);
					this.rightColor   = (Color)formatter.Deserialize(stream);
				}
				finally{
					stream.Close();
				}
			}
			catch( FileNotFoundException ){}
		}
		public void SaveState()
		{
			IsolatedStorageFile isoStore = 
				IsolatedStorageFile.GetStore(IsolatedStorageScope.Assembly|IsolatedStorageScope.User|IsolatedStorageScope.Roaming,null,null);
			Stream stream = new IsolatedStorageFileStream(StoreName,FileMode.Create,isoStore);
			try
			{
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize( stream, bIgnoreBlank );
				formatter.Serialize( stream, this.leftColor );
				formatter.Serialize( stream, this.rightColor);
			}
			finally
			{
				stream.Close();
			}
		}
	}
}
