using System;

namespace csdiff
{
	/// <summary>
	///
	/// </summary>
	public class Tree
	{
		public TreeItem first;
		public Tree()
		{
			first = null;
		}

		/***************************************************************************
		 * Function: ctree_update
		 * Purpose:
		 * Insert an element in the tree. If the element is not there,
		 * insert the data and set the reference count for this key to 1.
		 * If the key was there already, don't change the data, just increment
		 * the reference count
		 * If the value pointer is not null, we initialise the value block
		 * in the tree to contain this.
		 * We return a pointer to the users data in the tree */
		public object update(ulong key, object value)
		{
			TreeItem place;
			TreeItem item = search( key, out place );

			if( item == null ){
				/* element not found - insert a new one the data block for this element should be
				 * the user's block with our reference count at the beginning */
				item = addafter( place, key, value );
				item.refCount = 1;
				return item.data;
			}

			item.refCount++;	/* key was already there - increment reference count and return pointer to data */
			return item.data;	/* add on size of one long to get the start of the user data */
		}

		/***************************************************************************
		 * Method: search
		 * Purpose:
		 * Find an element. If not, find it's correct parent item */
		public TreeItem search(ulong key, out TreeItem place)
		{
			TreeItem item = getitem( key );
			if( item == null ){
				place = null;		/* no items in tree. set placeholder to null to * indicate insert at top of tree */
				return null;		/* return null to indicate key not found */
			}
			if( item.key == key ){
				place = null;		/* found the key already there set pplace to null just for safety */
				return item;		/* give the user a pointer to his data */
			}

			place = item;			/* key was not found - getitem has returned the parent - set this as the place for new insertions */
			return null;			/* return null to indicate that the key was not found */
		}

		/***************************************************************************
		 * Method: getitem
		 * Purpose:
		 * Finds the item with the given key. If it does not exist, return
		 * the parent item to which it would be attached. Returns null if
		 * no items in the tree */
		public TreeItem getitem(ulong key)
		{
			TreeItem prev = null;
			for( TreeItem item = first; item != null; ){
				if( item.key == key ) return item;

				/* not this item - go on to the correct child item.
				 * remember this item as if the child is null, this item
				 * will be the correct insertion point for the new item */
				prev = item;
				if( key < item.key ) item = item.left;
				else 				 item = item.right;
			}
			return prev;	/* prev is the parent - or null if nothing in tree */
		}

		/***************************************************************************
		 * Method: addafter
		 * Purpose:
		 * Insert a key in the position already found by tree_search.
		 * Return a pointer to the user's data in the tree. If the value
		 * pointer passed in is null, then we allocate the block, but don't
		 * initialise it to anything. */
		public TreeItem addafter(TreeItem place, ulong key, object value )
		{
			if( place == null ){
				first = new TreeItem( this, key, value );
				return first;
			}

			TreeItem child = new TreeItem( this, key, value );
			if( child.key < place.key )	place.left	= child;	/* should go on left leg */
			else						place.right = child;
			return child;
		}

		/***************************************************************************
		 * Method: getcount
		 * Purpose:
		 * Return the reference count for this key */
		public uint getcount(ulong key)
		{
			TreeItem item = find( key );
			if( item == null ) return 0;
			return item.refCount;
		}

		/***************************************************************************
		 * Method: find
		 * Purpose:
		 * Returns a pointer to the value (data block) for a given key. Returns
		 * null if not found. */
		public TreeItem find(ulong key)
		{
			TreeItem item = getitem(key);		/* find the correct place in the tree */
			if( item == null	) return null;
			if( item.key != key ) return null;	/* this key not in. getitem has returned parent */
			return item;						/* found the right element - return pointer to the data block */
		}

	}

	public class TreeItem
	{
		public Tree		root;
		public ulong	key;
		public TreeItem left;
		public TreeItem right;
		public object	data;
		public uint		refCount;

		public TreeItem( Tree rootParam, ulong keyParam, object value )
		{
			root = rootParam;
			key  = keyParam;
			left  = null;
			right = null;
			data  = value;
			refCount = 0;
		}
	}
}
