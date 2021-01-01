﻿// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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

	public interface IBindingList<T> : IList<T>, IBindingListSource, ICancelAddNew, IRaiseItemChangedEvents
	{
		bool AllowNew                      { get; }
		bool AllowEdit                     { get; }
		bool AllowRemove                   { get; }
		bool SupportsChangeNotification    { get; }
		bool SupportsSearching             { get; }
		bool SupportsSorting               { get; }
		bool IsSorted                      { get; }
		SysPropertyDescriptor SortProperty { get; }
		ListSortDirection SortDirection    { get; }

		event ListChangedEventHandler ListChanged;

		T    AddNew     ();
		int  Find       (SysPropertyDescriptor property, object key);
		void AddIndex   (SysPropertyDescriptor property);
		void RemoveIndex(SysPropertyDescriptor property);
		void ApplySort  (SysPropertyDescriptor property, ListSortDirection direction);
		void RemoveSort ();
	}
}
