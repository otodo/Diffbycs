using System;

namespace csdiff
{
	/// <summary>
	///
	/// </summary>
	public class Section : ListItem
	{
		public Line first;			/* first line in section */
		public Line last;			/* last line in section */

		public Section link;		/* we match this section */
		public Section correspond;	/* we correspond to this section, but
									 * don't match it */
		public STATE state;			/* compare state for section */

		public uint leftbase;		/* nr in original left list of first line*/
		public uint rightbase;		/* nr in original right list of first line*/

		public Section( Line firstParam, Line lastParam )
		{
			first		= firstParam;
			last		= lastParam;
			link		= null;
			correspond	= null;
			state		= STATE.NONE;
			leftbase	= 1;
			rightbase	= 1;
		}

		public uint	GetLineCount()
		{
			return ( last.linenr - first.linenr ) + 1;
		}

		public bool Match(Section sec,bool isIgnoreBlanks)
		{
			if (/*(this == null)||*/ (sec == null)) return(false);
			if ((first == null) || (sec.first == null)) return(false);
			/* ASSERT if first is non-null, so is last */
			/* attempt to link the first line of each file, and
			 * if matched, expand as long as we keep matching*/
			bool bLinked = false;
			bLinked |= ExpandAnchor(this, first, sec, sec.first, isIgnoreBlanks );

			/* attempt to link the last lines of each file and
			 * expand upwards */
			bLinked |= ExpandAnchor(this, last, sec, sec.last,	  isIgnoreBlanks );

			/* build a tree of lines, indexed by the line hashcode.
			 * a ctree will hold only the first value of any given key, but
			 * it will keep track of the number of items inserted on this key.
			 * thus we can keep count of the number of times this line
			 * (or at least this hashcode) appears.*/
			Tree ctleft	 = MakeCTree(isIgnoreBlanks);
			Tree ctright = sec.MakeCTree(isIgnoreBlanks);

			/* for each unlinked line in one list (doesnt matter which), find if
			 * appears once only in each list. if so, link, and expand
			 * the link to link lines before and after the matching line
			 * as long as they continue to match.*/
			for( Line line = first; line != null; line = (Line)line.GetNext() ) {

				if( ( line.link == null ) &&
				   ( ctleft .getcount( line.GetHashcode(isIgnoreBlanks) ) == 1 ) &&
				   ( ctright.getcount( line.GetHashcode(isIgnoreBlanks) ) == 1) ){

					/* line appears exactly once in each list */
					Line line2 = (Line)ctright.find( line.GetHashcode(isIgnoreBlanks) ).data;
					bLinked |= ExpandAnchor( this, line, sec, line2, isIgnoreBlanks );
				}

				if( line == last ) break;
			}

			return bLinked;
		}


		/***************************************************************************
		 * Function: ExpandAnchor
		 * Purpose:
		 * Given an anchor point (two lines that we think should match),
		 * try to link them, and the lines above and below them for as long
		 * as the lines can be linked (are the same, are unlinked).
		 * Return true if we make any links.  */
		public static bool ExpandAnchor(Section sec1, Line line1, Section sec2, Line line2,bool isIgnoreBlanks)
		{
			/* when a line is matched we set bChanges.	If we notice some
			 * blank lines, but do NOT link any new non-blank lines, we
			 * do NOT set bChanges.	 (If we did it would cause a closed
			 * loop as they would get noticed again next time.	line_link
			 * only returns true if it is a NEW link).
			 * At this stage we are only interested in making links, not in
			 * the size of the section that results (that fun comes later).
			 * therefore trailing blanks at the end of a section are not
			 * interesting and we don't look for them. */
			bool bChanges = false;

			/* We handle the section limits by using a sentinel which is one
			 * past the end of the section.	 (If the section ends at the end
			 * of the list then the sentinel is null). */
			Line leftend  = (Line)sec1.last.GetNext();
			Line rightend = (Line)sec2.last.GetNext();

			/* null lines shall not match */
			if ((line1 == null) || (line2 == null)) return false;

			/* check all lines forward until fail to link (because null,
			 * not matching, or already linked). include the passed in anchor point since this has not
			 * yet been linked. If blanks are ignorable then skip over any number of whole
			 * blank lines. */
			Line left  = line1;
			Line right = line2;
			for(;;){
				if( left.Link( right, isIgnoreBlanks ) ){
					bChanges = true;
					left  = (Line)left.GetNext();
					right = (Line)right.GetNext();
					if( left == leftend || right == rightend ) break;
				}
				else if( isIgnoreBlanks ){
					/* even though no match, maybe an ignorable blank? */
					bool moved = false;
					moved |= AbsorbAnyBlanks( ref left,  leftend, /*bMoveToNext=*/true);
					moved |= AbsorbAnyBlanks( ref right, rightend,/*bMoveToNext=*/true);
					if( !moved ) break; /* it didn't match and we didn't move on */
					if( left == leftend || right == rightend ) break;
				}
				else break;
			}

			/* check all matches going backwards from anchor point
			   but only if it was a real anchor	 (could have been
			   end-of-section/end-of-file and non-matching). */
			if( line1.link == null ) return bChanges;

			left  = (Line)line1.GetPrev();
			right = (Line)line2.GetPrev();
			if( left == null || right == null ) return bChanges;

			leftend	 = (Line)sec1.first.GetPrev();
			rightend = (Line)sec2.first.GetPrev();

			for(;;){
				if( left.Link(right,isIgnoreBlanks) ){
					bChanges = true;
					left  = (Line)left.GetPrev();
					right = (Line)right.GetPrev();
					if( left == leftend || right == rightend ) break;
				}
				else if( isIgnoreBlanks ){
					/* even though no match, maybe an ignorable blank? */
					bool moved = false;
					moved |= AbsorbAnyBlanks( ref left,  leftend, /*bMoveToNext=*/false);
					moved |= AbsorbAnyBlanks( ref right, rightend, /*bMoveToNext=*/false);
					if( !moved ) break; /* it didn't match and we didn't move on */
					if( left == leftend || right == rightend ) break;
				}
				else break;
			}

			return bChanges;
		}

		/***************************************************************************
		 * Function: AbsorbAnyBlanks
		 * Purpose:
		 * Update PLINE by making it point to the first non-blank
		 * at-or-after from but not after limit.
		 * If they are all blank then make it point to limit
		 * If from is non-blank then leave it alone.
		 * Return true iff PLINE was updated.
		 * It is legit for limit to be null (meaning end of file).*/
		public static bool AbsorbAnyBlanks(ref Line from , Line limit, bool bMoveToNext )
		{
			bool progress = false;

			while ( ( from != null ) && ( from.IsBlank() ) && ( from != limit ) ) {
				if( bMoveToNext ) from = (Line)from.GetNext();
				else			  from = (Line)from.GetPrev();
				progress = true;
			}
			return progress;
		}

		/***************************************************************************
		 * Function: MakeCTree
		 * Purpose:
		 * Build a ctree from the lines in the section given
		 * Remember that we are only interested in the lines that are
		 * not already linked.
		 * The value we store in the tree is the handle of the line. the key
		 * is the line hash code  */
		public Tree MakeCTree(bool isIgnoreBlanks)
		{
			Tree tree = new Tree();	/* make an empty tree */

			for( Line line = first; line != null; line = (Line)line.GetNext() ) {
				if( line.link == null) {
					tree.update( line.GetHashcode(isIgnoreBlanks), line );
				}
				if( line == last ) break;
			}
			return tree;
		}

		/***************************************************************************
		* Function: section_makelist
		* Purpose:
		* Make a list of sections by traversing a list of lines. Consecutive
		* linked lines that are linked to consecutive lines are put in a single
		* section. Blocks of unlinked lines are placed in a section.
		* If isIgnoreBlanks is set then we first try to link them as normal.
		* but if they won't link then we just skip over them and keep them
		* in the same section.
		* Left must be set true iff the list of lines is a left hand section.
		* Returns a handle to a list of sections */
		public static void MakeList(ListAnchor sections, ListAnchor linelist, bool left,bool isIgnoreBlanks)
		{
			/* for each line in the list */
			for( Line line1 = (Line)linelist.GetHead(); line1 != null; line1 = (Line)line1.GetNext() ){
				/* is it linked ? */
				bool matched;
				Line line2;
				if( line1.link != null || ( isIgnoreBlanks && line1.IsBlank() ) ){
					line2 = FindEndOfMatched(line1,isIgnoreBlanks);
					matched = true;
				} else {
					line2 = FindEndOfUnmatched(line1);
					matched = false;
				}
				Section sect = new Section(line1, line2);	/* create the section and add to list */
				sections.AddTail( sect );
				sect.state = (
					  matched ? STATE.SAME
					: left	  ? STATE.LEFTONLY
					:			STATE.RIGHTONLY
					);
				line1 = line2;	/* advance to end of section (no-op if 1 line section) */
			}
		}

		/***************************************************************************
		 * Function: FindEndOfMatched
		 * Purpose:
		 * Given that line is either linked or an ignorable blank:
		 * Return a Line which is the last line in a matched section
		 * containing (probably starting with) line.
		 * This could mean returning the line we were given.
		 * If the lines linked to are not consecutive then the section ends.
		 * If blanks are being ignored, then any blank line is deemed
		 * to match (even if it doesn't match).	 In this case we need the
		 * links of the lines before and after the blanks to be consecutive
		 * in order to carry on.  There could be blank lines on either or both ends of the links. */
		public static Line FindEndOfMatched(Line line,bool isIgnoreBlanks)
		{
			Line next;				/* next non-ignored or linked line */
			Line nextlink;			/* next in other file */

			/* The basic algorithm is to set up next and nextlink to point to
			   candidate lines.	 Examine them.	If they are good then step
			   on to them, else return the line one before.
			   There are confusion factors associated with the beginning and end of the file. */

			/* ASSERT( line is either an ignorable blank or else is linked) */
			/* As a section (at least at the start of the file) might start with an ignored non-linked blank line, first step over any such */
			if( line.link == null && line.IsBlank() ) {
				next = NextNonIgnorable(line,isIgnoreBlanks);
				/* There are unfortunately 6 cases to deal with
				   * marks where next will be. * against eof means next==null
				   blank(s) refer to ignorable unlinked blanks.
						  A			B		 C		  D		   E		F
				   line. xxxxx		xxxxx	 xxxxx	  xxxxx	   xxxxx	xxxxx
						 *unlinked	blanks	*linked	  blanks  *eof	   *blanks
								   *unlinked		 *linked			eof

				   next could be:

					  null - case E => return line
					  unlinked ignorable blank - case F => return that blank line
					  unlinked other - cases A,B return prev(that unlinked line)
					  linked - cases C,D continue from that linked line */
				if( next == null) return line;
				if( next.link == null) {
					if( isIgnoreBlanks && next.IsBlank() ) return next;
					return (Line)next.GetPrev();
				}
				line = next;
			}

			/* we have stepped over inital blanks and now do have a link */
			for(;;){
				next = NextNonIgnorable( line, isIgnoreBlanks );
				/* Same 6 cases - basically same again */
				if( next == null ) return line;
				if( next.link == null ){
					if( isIgnoreBlanks && next.IsBlank() ) return next;
					return (Line)next.GetPrev();
				}
				nextlink = NextNonIgnorable( line.link, isIgnoreBlanks );

				/* WEAK LOOP INVARIANT
				   line is linked.
				   next is the next non-ignorable line in this list after line.
				   nextlink is the next non-ignorable line after link(line)
										in the other list (could be null etc). */
				if( next.link != nextlink ) return (Line)next.GetPrev();
				line = next;
			}
		}

		/***************************************************************************
		 * Function: FindEndOfUnmatched
		 * Purpose:
		 * Returns a Line which is the last line in an unmatched section
		 * containing (probably starting with) Line.
		 * Note that it does not necessarily make progress.
		 * As noted above, even if blank lines are being ignored, we don't
		 * mind tagging them onto the end of an already unmatching section.
		 * This means we carry on until we find the first real link */
		public static Line FindEndOfUnmatched(Line line)
		{
			for(;;){
				Line next = (Line)line.GetNext();
				if( next == null ) return line;
				if( next.link != null ) return line;
				line = next;
			}
		}

		/***************************************************************************
		 * Function: NextNonIgnorable
		 * Purpose:
		 * An ignorable line is a blank line with no link and isIgnoreBlanks set
		 * Given that line is initially not null and not ignorable:
		 * If line is the last line in the list then return null
		 * Else If isIgnoreBlanks is false then return the next line after line
		 * else return next line which has a link or which is non-blank.
		 * If there is no such line then return the last line in the list.
		 * Note that this does always make progress (at the cost of sometimes returning null). */
		public static Line NextNonIgnorable(Line line,bool isIgnoreBlanks)
		{
			Line next = (Line)line.GetNext();
			if (next==null) return null;
			for(;;){
				line = next;
				if( line.link != null )	return line;
				if( !isIgnoreBlanks )	return line;
				if( !line.IsBlank() )	return line;
				next = (Line)line.GetNext();
				if( next == null ) return line;
			}
		}

		/***************************************************************************
		 * Function: MatchList
		 * Purpose:
		 * Match up two lists of sections. Establish links between sections
		 * that match, and establish 'correspondence' between sections that
		 * are in the same place, but don't match.
		 * For each pair of corresponding sections, we also call section_match
		 * to try and link up more lines.
		 * We return true if we made any more links between lines, or false
		 * otherwise. */
		public static bool MatchList(ListAnchor secsleft, ListAnchor secsright,bool isIgnoreBlanks)
		{
			bool bLinked = false;
			Section sec1, sec2;

			/* match up linked sections - We know whether a section is
			   supposed to link from its state, but we don't know what section
			   it links to.	 Also we can have sections which are defined to
			   be matching but actually contain nothing but ignorable blank lines */

			/*	for each linked section try to find the section	 linked to it. */
			for( sec1 = (Section)secsleft.GetHead(); sec1 != null; sec1 = (Section)sec1.GetNext() ) {
				if( sec1.state == STATE.SAME ){
					Line FirstWithLink = sec1.FindFirstWithLink();
					for( sec2 = (Section)secsright.GetHead(); sec2 != null; sec2 = (Section)sec2.GetNext() ) {
						if( sec2.state == STATE.SAME &&
							FirstWithLink.link == sec2.FindFirstWithLink() )
						{
							break;
						}
					}
					if( sec2 != null ){		/* sec2 could be null if sec1 is all allowable blanks */
						sec1.link = sec2;
						sec2.link = sec1;
					}
				}
			}

			/* go through all unmatched sections. Note that we need to complete
			 * the link-up of matching sections before this, since we need
			 * all the links in place for this to work. */
			for( sec1 = (Section)secsleft.GetHead(); sec1 != null; sec1 = (Section)sec1.GetNext() ) {

				if( sec1.state == STATE.SAME ) continue; /* skip the linked sections */

				/* check that the previous and next sections, if
				 * they exist, are linked. this should not fail since
				 * two consecutive unlinked sections should be made into one section */
				Section secTemp = (Section)sec1.GetPrev();
				if( secTemp != null && secTemp.state != STATE.SAME ) continue;
				secTemp = (Section)sec1.GetNext();
				if( secTemp != null && secTemp.state != STATE.SAME ) continue;

				/* find the section that corresponds to this - that is, the
				 * section following the section linked to our previous section.
				 * we could be at beginning or end of list. */
				if( sec1.GetPrev() != null ){
					Section secOther = ((Section)sec1.GetPrev()).link;
					if( secOther == null ) continue;

					/* check this section is not linked */
					sec2 = (Section)secOther.GetNext();
					if( (sec2 == null) || (sec2.link != null) ) continue;

					/* check that the section after these are linked to each other (or both are at end of list).*/
					if( sec1.GetNext() != null ){
						if( ((Section)sec1.GetNext()).link != sec2.GetNext() ) continue;
					}
					else {
						if( sec2.GetNext() == null) continue;
					}
				}
				else if( sec1.GetNext() != null ){
					Section secOther = ((Section)sec1.GetNext()).link;
					if( secOther == null ) continue;

					/* check this section is not linked */
					sec2 = (Section)secOther.GetPrev();
					if( (sec2 == null) || (sec2.link != null) ) continue;

					/* check that the section before these are linked to each other (or both are at start of list).*/
					if( sec1.GetPrev() != null ){
						if( ((Section)sec1.GetPrev()).link != (Section)sec2.GetPrev() ) continue;
					}
					else {
						if( sec2.GetPrev() == null ) continue;
					}
				}
				else {
					/* there must be at most one section in each file, and they are unmatched. make these correspond.*/
					sec2 = (Section)secsright.GetHead();
				}

				/* make the correspondence links */
				if ((sec1 != null) && (sec2 != null)) {
					sec1.correspond = sec2;
					sec2.correspond = sec1;
				}

				if ( sec1.Match( sec2,isIgnoreBlanks ) ) bLinked = true;	/* attempt to link up lines */
			}

			return bLinked;
		}

		/***************************************************************************
		 * Function: section_makecomposite
		 * Purpose:
		 * Make a composite list of sections by traversing a list of sections.
		 * Return a handle to a list of sections.
		 * During this, set state, leftbase and rightbase for sections.
		 * Comments:
		 * This function creates a list that corresponds to the 'best' view
		 * of the differences between the two lists. We place sections from the
		 * two lists into one composite list. Sections that match each other are only
		 * inserted once (from the right list). Sections that match, but in different
		 * positions in the two lists are inserted twice, once in each position, with
		 * status to indicate this. Unmatched sections are inserted in the correct
		 * position.
		 * - Take sections from the left list until the section is linked to one not
		 *	 already taken.
		 * - Then take sections from right until we find a section linked to one not
		 *	 already taken.
		 * - If the two sections waiting are linked to each other, take them both
		 *	 (once- we take the right one and advance past both).
		 * - Now we have to decide which to take in place and which to declare
		 *	 'moved'. Consider the case where the only change is that the first line
		 *	 has been moved to the end. We should take the first line (as a move),
		 *	 then the bulk of the file (SAME) then the last line (as a move). Hence,
		 *	 in difficult cases, we take the smaller section first, to ensure that
		 *	 the larger section is taken as SAME.
		 *	 To indicate which section has been output, we set the state field
		 *	 to STATE.MARKED once we have taken it.	  States in left and right
		 *	 lists are of no further interest once we have built the composite.
		 *	 Up to this point we have worked off the STATE of a section.  By now
		 *	 all the section links are in place, so we can use them too. */
		public static void MakeComposite(ListAnchor compo,ListAnchor secsleft, ListAnchor secsright)
		{
			Section left, right;

			compo.RemoveAll();

			left  = (Section)secsleft.GetHead();
			right = (Section)secsright.GetHead();

			while ( (left != null) || (right != null)) {

				if (left == null) {
					/* no more in left list - take right section is it moved or just unmatched ? */
					if( right.link == null ){
						TakeSection( compo, null, right, STATE.RIGHTONLY );
						right = (Section)right.GetNext();
					} else {
						TakeSection( compo, right.link, right, STATE.MOVEDRIGHT );
						right = (Section)right.GetNext();
					}
				}
				else if (right == null) {
					/* right list empty - must be left next is it moved or just unmatched ? */
					if (left.link == null) {
						TakeSection(compo, left, null, STATE.LEFTONLY);
						left = (Section)left.GetNext();
					} else {
						TakeSection(compo, left, left.link, STATE.MOVEDLEFT);
						left = (Section)left.GetNext();
					}
				}
				else if(left.state == STATE.LEFTONLY) {
					/* unlinked section on left */
					TakeSection(compo, left, null, STATE.LEFTONLY);
					left = (Section)left.GetNext();
				} else if ( left.link == null ) {
					/* This is an ignorable blank section on the left. We ignore it. (We will take any such from the right) */
					left = (Section)left.GetNext();

				} else if (left.link.state==STATE.MARKED) {
					/* left is linked to section that is already taken*/
					TakeSection(compo, left, left.link, STATE.MOVEDLEFT);
					left = (Section)left.GetNext();

				} else	if (right.link == null) {
					/* take unlinked section on right Either unmatched or ignorable blanks */
					TakeSection(compo, null, right, right.state);
					right = (Section)right.GetNext();

				} else if (right.link.state==STATE.MARKED) {
					/* right is linked to section that's already taken */
					TakeSection(compo, right.link, right, STATE.MOVEDRIGHT);
					right = (Section)right.GetNext();

				} else if (left.link == right) {
					/* sections match */
					TakeSection(compo, left, right, STATE.SAME);
					right = (Section)right.GetNext();
					left  = (Section)left.GetNext();
				} else {
					/* both sections linked to forward sections
					 * decide first based on size of sections
					 * - smallest first as a move so that largest
					 * is an unchanged. */
					if ( right.GetLineCount() > left.GetLineCount() ){
						TakeSection(compo, left, left.link, STATE.MOVEDLEFT);
						left = (Section)left.GetNext();
					} else {
						TakeSection(compo, right.link, right, STATE.MOVEDRIGHT);
						right = (Section)right.GetNext();
					}
				}
			}
		}

		/***************************************************************************
		 * Function: FindFirstWithLink
		 * Purpose:
		 * Return the first line in the range first..last
		 * which has a link.  Return last if none of them have a link.
		 * List_Next must lead from first to last eventually.
		 * It is legit for last to be null. */
		public Line FindFirstWithLink()
		{
			/* The strategy of including blanks on the ENDS of sections rather
			   than the start of new sections will mean that this function
			   usually strikes gold immediately.  A file with a leading blank section is its raison d'etre. */
			while ( first.link == null && first != last )
				first = (Line)first.GetNext();
			return first;
		}


		/***************************************************************************
		 * Function: TakeSection
		 * Purpose:
		 * Add a section to the composite list. Called from make_composites
		 * to copy a section, add it to the composite list and set the state,
		 * leftbase and rightbase.	 Note that the state could be STATE.SAME
		 * with a null section on the left.	 May NOT call with STATE.SAME and
		 * a null right section! */
		public static void TakeSection(ListAnchor compo, Section left, Section right, STATE state)
		{
			Section sec = null;

			/* select which section is being output, and change the state to indicate it has been output */
			switch( state ){
				case STATE.SAME:
					/* both the same. we mark both as output, and
					 * take the right one.	It is possible that the
					 * left one could be null (an ignorable blank section) */
					if( left != null ) left.state = STATE.MARKED;
					right.state = STATE.MARKED;
					sec = right;
					break;

				case STATE.LEFTONLY:
				case STATE.MOVEDLEFT:
					sec = left;
					left.state = STATE.MARKED;
					break;

				case STATE.RIGHTONLY:
				case STATE.MOVEDRIGHT:
					sec = right;
					right.state = STATE.MARKED;
					break;
			}

			/* create a new section on the list */
			Section newsec = new Section( sec.first, sec.last );
			compo.AddTail( newsec );

			newsec.state = state;

			if (left != null)	newsec.leftbase = left.first.linenr;
			else				newsec.leftbase = 0;

			if (right != null)	newsec.rightbase = right.first.linenr;
			else				newsec.rightbase = 0;
		}

	}

}
