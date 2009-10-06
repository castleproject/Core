// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
// 
namespace Castle.Components.Binder
{
	using System;
	using System.Collections;

	/// <summary>
	/// A useful representation of a set of IPropertyError instances.
	/// </summary>
	[Serializable]
	public class ErrorList : ICollection
	{
		private readonly SortedList entries = new SortedList();

		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorList"/> class.
		/// </summary>
		/// <param name="initialContents">The initial contents.</param>
		public ErrorList(IList initialContents)
		{
			IList list = (initialContents != null ? initialContents : new ArrayList(0));

			foreach (DataBindError error in list)
			{
				entries[error.Property] = error;
			}
		}

		/// <summary>
		/// Gets the number of elements contained in the <see cref="T:System.Collections.ICollection"></see>.
		/// </summary>
		/// <value></value>
		/// <returns>The number of elements contained in the <see cref="T:System.Collections.ICollection"></see>.</returns>
		public int Count
		{
			get { return entries.Count; }
		}

		/// <summary>
		/// Determines whether [contains] [the specified property].
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns>
		/// 	<c>true</c> if [contains] [the specified property]; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(String property)
		{
			return entries.Contains(property);
		}

		/// <summary>
		/// Gets the <see cref="Castle.Components.Binder.DataBindError"/> with the specified property.
		/// </summary>
		/// <value></value>
		public DataBindError this[String property]
		{
			get { return entries[property] as DataBindError; }
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
			return entries.Values.GetEnumerator();
		}

		#endregion
	}
}