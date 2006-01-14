// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	/// <summary>
	/// A useful representation of a set of IPropertyError instances.
	/// </summary>
	public class ErrorList : ICollection
	{
		private IList list;
		private IDictionary map;

		public ErrorList( IList initialContents )
		{
			this.list = ( initialContents != null ? initialContents : new ArrayList(0) );
			map	= new HybridDictionary( list.Count, true );
			
			foreach ( IPropertyError error in list )
			{
				map.Add( error.Property, error );
			}
		}

		public int Count
		{
			get { return list.Count; }
		}

		public bool Contains( String property )
		{
			return map.Contains( property );
		}

		public IPropertyError this[ String property ]
		{
			get { return map[ property ] as IPropertyError; }
		}

		#region ICollection Members

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		public object SyncRoot
		{
			get { return this; }
		}

		public bool IsSynchronized
		{
			get { return false; }
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}

		#endregion
	}
}
