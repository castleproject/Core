// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.f
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Components.DictionaryAdapter
{
	using System.Collections.Generic;
	using System.ComponentModel;
	using SysPropertyDescriptor = System.ComponentModel.PropertyDescriptor;

	/// <summary>
	///   Provides a generic collection that supports data binding.
	/// </summary>
	/// <remarks>
	///   This class wraps the CLR <see cref="System.ComponentModel.BindingList<T>"/>
	///   in order to implement the Castle-specific <see cref="IBindingList<T>"/>.
	/// </remarks>
	/// <typeparam name="T">The type of elements in the list.</typeparam>
	public class BindingList<T> : System.ComponentModel.BindingList<T>, IBindingList<T>
	{
		/// <summary>
		///   Initializes a new instance of the <see cref="System.ComponentModel.BindingList<T>"/> class
		///   using default values.
		/// </summary>
		public BindingList()
			: base() { }

		/// <summary>
		///   Initializes a new instance of the <see cref="System.ComponentModel.BindingList<T>"/> class
		///   with the specified list.
		/// </summary>
		/// <param name="list">
		///   An <see cref="System.Collections.Generic.IList<T>"/> of items
		///   to be contained in the <see cref="Castle.Components.DictionaryAdapter.BindingList<T>"/>.
		/// </param>
		public BindingList(IList<T> list)
			: base(list) { }

		public IBindingList AsBindingList
		{
			get { return this; }
		}

		bool IBindingList<T>.AllowNew
		{
			get { return AsBindingList.AllowNew; }
		}

		bool IBindingList<T>.AllowEdit
		{
			get { return AsBindingList.AllowEdit; }
		}

		bool IBindingList<T>.AllowRemove
		{
			get { return AsBindingList.AllowRemove; }
		}

		bool IBindingList<T>.SupportsChangeNotification
		{
			get { return AsBindingList.SupportsChangeNotification; }
		}

		bool IBindingList<T>.SupportsSearching
		{
			get { return AsBindingList.SupportsSearching; }
		}

		bool IBindingList<T>.SupportsSorting
		{
			get { return AsBindingList.SupportsSorting; }
		}

		bool IBindingList<T>.IsSorted
		{
			get { return AsBindingList.IsSorted; }
		}

		SysPropertyDescriptor IBindingList<T>.SortProperty
		{
			get { return AsBindingList.SortProperty; }
		}

		ListSortDirection IBindingList<T>.SortDirection
		{
			get { return AsBindingList.SortDirection; }
		}

		int IBindingList<T>.Find(SysPropertyDescriptor property, object key)
		{
			return AsBindingList.Find(property, key);
		}

		void IBindingList<T>.AddIndex(SysPropertyDescriptor property)
		{
			AsBindingList.AddIndex(property);
		}

		void IBindingList<T>.RemoveIndex(SysPropertyDescriptor property)
		{
			AsBindingList.RemoveIndex(property);
		}

		void IBindingList<T>.ApplySort(SysPropertyDescriptor property, ListSortDirection direction)
		{
			AsBindingList.ApplySort(property, direction);
		}

		void IBindingList<T>.RemoveSort()
		{
			AsBindingList.RemoveSort();
		}
	}
}
