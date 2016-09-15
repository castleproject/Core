// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.DictionaryAdapter.Tests
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;

	[OnDemand]
	public interface IItemContainer<TItem> : IDictionaryAdapter, IDictionaryCreate,
	                                         IEditableObject, INotifyPropertyChanged
	{
		Guid Id { get; set; }

#pragma warning disable 108
		TItem Item { get; set; }
#pragma warning restore 108

		[OnDemand(5)]
		int Count { get; set; }

		Address Address { get; set; }

		Uri EmailAddress { get; set; }
		IPhone Phone { get; set; }

		int[] Positions { get; set; }

		IDynamicValue<int> ReducePositions { get; set; }

		IList<TItem> GenericItems { get; set; }

#if FEATURE_BINDINGLIST
		BindingList<TItem> Bindingtems { get; set; }
#endif

		IList Items { get; set; }
	}
}