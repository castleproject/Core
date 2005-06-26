// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
	using System.Collections;
	using System.Collections.Specialized;

	/// <summary>
	/// A useful representation of a set of IPropertyError instances.
	/// </summary>
	public class ErrorList : IEnumerable
	{
		private IList list;
		private IDictionary map;

		public ErrorList( IList list )
		{
			this.list = ( list != null ? list : new ArrayList(0) );
			map	= CollectionsUtil.CreateCaseInsensitiveHashtable( list.Count );
			
			foreach ( IPropertyError e in list )
				map.Add( e.Property, e );
		}

		public int Count
		{
			get { return list.Count; }
		}

		public bool Contains( string property )
		{
			return map.Contains( property );
		}

		public IPropertyError this[ string property ]
		{
			get { return map[ property ] as IPropertyError; }
		}

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}

		#endregion
	}
}
