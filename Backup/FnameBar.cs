using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace csdiff
{
	/// <summary>
	/// FnameBar �̊T�v�̐����ł��B
	/// </summary>
	public class FnameBar : System.Windows.Forms.Panel
	{
		/// <summary>
		/// �K�v�ȃf�U�C�i�ϐ��ł��B
		/// </summary>
		private System.ComponentModel.Container components = null;

		private Document document = null;
		private Option option = null;

		public FnameBar(Document paramDocument,Option paramOption)
		{
			/// <summary>
			/// Windows.Forms �N���X�쐬�f�U�C�i �T�|�[�g�ɕK�v�ł��B
			/// </summary>
			InitializeComponent();
			this.SetStyle(ControlStyles.ResizeRedraw,true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint,true);
			this.SetStyle(ControlStyles.UserPaint,true);

			document = paramDocument;
			option	 = paramOption;
		}

		/// <summary>
		/// �g�p����Ă��郊�\�[�X�Ɍ㏈�������s���܂��B
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


		#region Component Designer generated code
		/// <summary>
		/// �f�U�C�i �T�|�[�g�ɕK�v�ȃ��\�b�h�ł��B���̃��\�b�h�̓��e��
		/// �R�[�h �G�f�B�^�ŕύX���Ȃ��ł��������B
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// FnameBar
			// 
			this.CausesValidation = false;
			this.Size = new System.Drawing.Size(200, 18);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.FnameBar_Paint);

		}
		#endregion

		private void FnameBar_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			Rectangle[] rcPanes = { GetLeftRect(), GetRightRect() };
			Color[] backColors  = { option.leftColor, option.rightColor };
			Array DocOfValues = DocOf.GetValues( Type.GetType("csdiff.DocOf") );
			foreach( int nLeftOrRight in DocOfValues )
			{
				Rectangle rcPane = rcPanes[nLeftOrRight];
				Point[] pointsD = {	new Point(rcPane.Left,rcPane.Bottom),
									new Point(rcPane.Left,rcPane.Top),
									new Point(rcPane.Right,rcPane.Top) };
				g.DrawLines( new Pen(SystemColors.ControlDark), pointsD );
				Point[] pointsL = {	new Point(rcPane.Left,rcPane.Bottom),
									new Point(rcPane.Right,rcPane.Bottom),
									new Point(rcPane.Right,rcPane.Top) };
				g.DrawLines( new Pen(SystemColors.ControlLightLight), pointsL );
				rcPane.Inflate(-1,-1);
				g.FillRectangle(new SolidBrush(backColors[nLeftOrRight]), rcPane );

				if( document.IsLoaded((DocOf)nLeftOrRight) == false ) continue;

				// �}�[�W����݂���
				rcPanes[nLeftOrRight].Inflate(-1,-1);

				// �t�@�C������`��
				StringFormat format = new StringFormat();
				format.Trimming     |= StringTrimming.EllipsisPath;
				format.LineAlignment = StringAlignment.Center;
				format.FormatFlags  |= StringFormatFlags.NoWrap;
				g.DrawString( document.fnames[nLeftOrRight],
					Font,
					SystemBrushes.ControlText,
					rcPanes[nLeftOrRight],
					format );
			}

		}

		public Rectangle GetLeftRect()
		{
			Rectangle rcLeft = ClientRectangle;
			rcLeft.Size = new Size(rcLeft.Width / 2-1, rcLeft.Height-1 );
			return rcLeft;
		}

		public Rectangle GetRightRect()
		{
			Rectangle rcRight = ClientRectangle;
			rcRight.Size = new Size(ClientRectangle.Width - GetLeftRect().Width-2, rcRight.Height-1 );
			rcRight.Offset(GetLeftRect().Width+1,0);
			return rcRight;
		}
	}
}
