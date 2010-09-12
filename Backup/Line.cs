using System;

namespace csdiff
{
	/// <summary>
	///
	/// </summary>
	public class Line : ListItem
	{
		public ulong flags;
		const ulong LF_HASHVALID = 2;

		public string	text;				/* null-terminated copy of line text */
		public ulong	hash;				/* hashcode for line */
		public Line		link;				/* handle for linked line */
		public uint		linenr;				/* line number (any arbitrary value) */

		public Line(string textParam,uint linenrParam)
		{
			text   = textParam;
			link   = null;
			linenr = linenrParam;
			flags  = 0;
			hash   = 0;
		}
		public ulong GetHashcode(bool isIgnoreBlanks)
		{
			if( ( flags & LF_HASHVALID ) == 0 ){
				/* hashcode needs to be recalced */
				hash = hash_string( text, isIgnoreBlanks );
				flags |= LF_HASHVALID;
			}
			return hash;
		}

		public bool IsBlank()
		{
			return /* ( this!=NULL ) && */ utils_isblank(text);
		}

		public bool Link(Line line,bool isIgnoreBlanks)
		{
			if ( /*(this == NULL) || */ (line == null) ) return false;
			if ( (link != null) || (line.link != null)) return false;

			if ( compare( line, isIgnoreBlanks ) ) {
				link = line;
				line.link = this;
				return true;
			}
			return false;
		}

		public bool	compare(Line line,bool isIgnoreBlanks)
		{
			/* check that the hashcodes match */
			if( GetHashcode(isIgnoreBlanks) != line.GetHashcode(isIgnoreBlanks) ) return false;

			/* hashcodes match - are the lines really the same ? */
			/* note that this is coupled to gutils\utils.c in definition of blank */
			string s1 = text;
			string s2 = line.text;
			if( isIgnoreBlanks ){
				s1 = remove_blank(s1);
				s2 = remove_blank(s2);
			}
			return ( s1 == s2 );
		}

		// --------------------- helper functions. ---------------------------------
		protected ulong hash_string(string str, bool bIgnoreBlanks)
		{
			const ulong LARGENUMBER = 6293815;
			ulong sum = 0;
			ulong multiple = LARGENUMBER;
			ulong index = 1;

			for( int i=0; i<str.Length; i++ ){

				if (bIgnoreBlanks) {
					while ( (str[i] == ' ') || (str[i] == '\t') ) i++;
				}

				sum += multiple * index++ * str[i];
				multiple *= LARGENUMBER;
			}
			return(sum);
		}

		protected bool utils_isblank(string str)
		{
			/* having skipped all the blanks, do we see the end delimiter? */
			char[] trimChars = { ' ','\t' };
			str = str.TrimStart( trimChars );
			return ( str.Length == 0 || str[0] == '\r' || str[0] == '\n' );
		}

		protected string remove_blank(string str)
		{
			char[] trimChars = {' ','\t'};
			string[] splited = str.Split(trimChars);
			str = string.Empty;
			for( int i=0; i<splited.Length; i++ )
			{
				str += splited[i];
			}
			return str;
		}

	}
}
