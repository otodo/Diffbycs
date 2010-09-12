using System;
using System.IO;
using System.Text;

namespace csdiff
{
	public enum DocOf { LEFT = 0, RIGHT = 1 };

	/// <summary>
	///
	/// </summary>
	public class Document
	{
		public string[] fnames = new string [2];
		public ListAnchor[] lines;
		//! public CFileStatus	fstat[2];
		public ListAnchor secComposit = new ListAnchor();

		public Document()
		{
			lines = new ListAnchor[2];
			lines[0] = new ListAnchor();
			lines[1] = new ListAnchor();
		}
		public bool IsLoadedAll()
		{
			return IsLoaded(DocOf.LEFT) && IsLoaded(DocOf.RIGHT);
		}
		public bool IsLoaded(DocOf nLeftorRight)
		{
			return !lines[(int)nLeftorRight].IsEmpty();
		}
		public bool Load(DocOf nLeftorRight,string fname)
		{
			fnames[(int)nLeftorRight]	= fname;
			//!CFile::GetStatus(pszFname,fstat[nLeftorRight]);

			ListAnchor list = lines[(int)nLeftorRight];
			list.RemoveAll();
			uint linenr = 1;
			StreamReader sr = new StreamReader( fname, Encoding.GetEncoding(932));	//Shif-JIS
			try{
				string text;
				while( ( text = sr.ReadLine() ) != null ){
					list.AddTail( new Line( text, linenr++ ) );
				}
			}
			finally{
				sr.Close();
			}
			return true;
		}

		public bool UpdateCompositSection(bool isIgnoreBlanks)
		{
			if( !IsLoadedAll() ) return false;
			for( int nLeftorRight=0; nLeftorRight<2; nLeftorRight++ ){
				for( Line line = (Line)lines[nLeftorRight].GetHead(); line != null; line = (Line)line.GetNext() ){
					line.link = null;
				}
			}
			secComposit.RemoveAll();
			compare( secComposit, lines[(int)DocOf.LEFT], lines[(int)DocOf.RIGHT], isIgnoreBlanks );
			return true;
		}
		public bool ResetContent()
		{
			lines[(int)DocOf.LEFT ].RemoveAll();
			fnames[(int)DocOf.LEFT ] = string.Empty;
			lines [(int)DocOf.RIGHT].RemoveAll();
			fnames[(int)DocOf.RIGHT] = string.Empty;
			secComposit.RemoveAll();
			return true;
		}

		public void compare( ListAnchor secsComposite, ListAnchor linesLeft, ListAnchor linesRight, bool isIgnoreBlanks )
		{
			Section wholeLeft, wholeRight;
			ListAnchor secsLeft  = new ListAnchor();
			ListAnchor secsRight = new ListAnchor();

			bool bChanges;
			do {
				bChanges = false;	/* we have made no changes so far this time round the loop */

				/* make a section covering the whole file */
				wholeLeft  = new Section( (Line)linesLeft. GetHead(), (Line)linesLeft.GetTail()  );
				wholeRight = new Section( (Line)linesRight.GetHead(), (Line)linesRight.GetTail() );

				/* link up matching unique lines between these sections */
				if( wholeLeft.Match( wholeRight, isIgnoreBlanks ) ) bChanges = true;

				/* discard previous section lists if made */
				secsLeft. RemoveAll();
				secsRight.RemoveAll();
				/* build new section lists for both files */
				Section.MakeList( secsLeft,  linesLeft,  true ,isIgnoreBlanks);
				Section.MakeList( secsRight, linesRight, false,isIgnoreBlanks);

				/* match up sections - make links and corresponds between
				 * sections. Attempts to section_match corresponding
				 * sections that are not matched. returns true if any
				 * further links were made */
				if( Section.MatchList( secsLeft, secsRight, isIgnoreBlanks ) ) bChanges = true;

			/* repeat as long as we keep adding new links */
			} while( bChanges );

			/* all possible lines linked, and section lists made .
			 * combine the two section lists to get a view of the
			 * whole comparison - the composite section list. This also
			 * sets the state of each section in the composite list. */
			Section.MakeComposite( secsComposite, secsLeft, secsRight );
		}
	}
}
