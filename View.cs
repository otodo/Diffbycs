using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace csdiff
{
	public class LineIndex
	{
		public Line		m_line;
		public STATE	m_state;
		public string GetText()
		{
			return m_line.text;
		}
		//! int GetLineNoLeft() const;
		//! int GetLineNoRight() const;
	};

	/// <summary>
	/// View �̊T�v�̐����ł��B
	/// </summary>
	public class View : System.Windows.Forms.ListBox
	{
		/// <summary>
		/// �K�v�ȃf�U�C�i�ϐ��ł��B
		/// </summary>
		private System.ComponentModel.Container components = null;
		private Document m_document;
		private Option   m_option;

		public View(Document document,Option option)
		{
			m_document = document;
			m_option   = option;
			InitializeComponent();	// ���̌Ăяo���́AWindows.Forms �t�H�[�� �f�U�C�i�ŕK�v�ł��B
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
			// View
			// 
			this.CausesValidation = false;
			this.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.Font = new System.Drawing.Font("�l�r �S�V�b�N", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.IntegralHeight = false;
			this.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.Size = new System.Drawing.Size(304, 240);
			this.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.View_DrawItem);

		}
		#endregion

		public void InitialUpdate(bool bClearSelPos)
		{
			// �s�������W�A�\��
			BeginUpdate();
			Items.Clear();
			for( Section sh = (Section)m_document.secComposit.GetHead(); sh != null; sh = (Section)sh.GetNext() ){

				STATE state = sh.state;
				Line line1 = sh.first;
				Line line2 = sh.last;

				for( ; line1 != null ; line1 = (Line)line1.GetNext() ){
					LineIndex lineidx = new LineIndex();
					lineidx.m_line = line1;
					lineidx.m_state = state;
					Items.Add( lineidx );
					if( line1 == line2 ) break;
				}
			}
			EndUpdate();
		}

		public int GetLineHeight(Graphics g)
		{
			if( g == null ) g = CreateGraphics();
			return (int)Font.GetHeight(g);
		}

		private void vScrollBar1_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			if( e.Type != ScrollEventType.EndScroll ) Invalidate();
		}

		private void View_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
		{
			Brush brushBack = new SolidBrush(BackColor);
			Brush brushBackHi = new SolidBrush(SystemColors.Highlight);
			Brush brushLeft  = new SolidBrush(m_option.leftColor);
			Brush brushRight = new SolidBrush(m_option.rightColor);

			Brush brushText   = new SolidBrush(SystemColors.WindowText);
			Brush brushTextHi = new SolidBrush(SystemColors.HighlightText);

			Graphics g = e.Graphics;

			if( e.Index == -1 )
			{
				g.FillRectangle( brushBack, ClientRectangle );
				return;
			}

			Rectangle rcLine = e.Bounds;
			LineIndex lineidx = (LineIndex)Items[e.Index];

			Brush brshBack = brushBack;
			Brush brshFore = brushText;
			if( ( e.State & DrawItemState.Selected ) != 0 )
			{
				brshBack = brushBackHi;
				brshFore = brushTextHi;
			}
			else
			{
				switch( lineidx.m_state )
				{
					case STATE.MOVEDLEFT:
					case STATE.LEFTONLY:
						brshBack = brushLeft;
						break;
					case STATE.MOVEDRIGHT:
					case STATE.RIGHTONLY:
						brshBack = brushRight;
						break;
				}
			}
			g.FillRectangle( brshBack, rcLine );
			g.DrawString(lineidx.GetText(),Font,brshFore,rcLine);
		}

	}
}
