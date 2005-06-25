using System;
using System.Collections;
using System.Collections.Specialized;

namespace Castle.MonoRail.Framework.Internal
{
	/// <summary>
	/// Simple strong typed dictionary for IResource instances.
	/// </summary>
	public class ResourceDictionary : ICollection
	{
		private HybridDictionary map = new HybridDictionary( true );
		
		public void Add( object key, IResource resource )
		{
			map.Add( key, resource );
		}

		public IResource this[ object key ]
		{
			get { return map[ key ] as IResource; }
			set { map[ key ] = value; }
		}

		public int Count
		{
			get { return map.Count; }
		}

		public void Remove( object key )
		{
			map.Remove( key );
		}
		
		public bool Contains(object key)
		{
			return map.Contains( key );
		}

		public void Clear()
		{
			map.Clear();
		}

		public ICollection Values
		{
			get { return map.Values; }
		}

		public ICollection Keys
		{
			get { return map.Keys; }
		}

		#region ICollection Members

		public bool IsSynchronized
		{
			get { return map.IsSynchronized; }
		}

		public void CopyTo(Array array, int index)
		{
			map.CopyTo( array, index );
		}

		public object SyncRoot
		{
			get { return map.SyncRoot; }
		}

		#endregion

		#region IEnumerable Members

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return map.GetEnumerator();
		}

		#endregion
	}
}
