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

namespace Castle.Components.DictionaryAdapter
{
#if !SILVERLIGHT
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;

	public class BindingListInitializer<T> : IValueInitializer
	{
		private readonly Func<object> addNew;
		private readonly Func<int, object, object> addAt;
		private readonly Func<int, object, object> setAt;
		private readonly Action<int> removeAt;
		private bool addingNew;

		public BindingListInitializer(Func<int, object, object> addAt, Func<object> addNew, Func<int, object, object> setAt, Action<int> removeAt)
		{
			this.addAt = addAt;
			this.addNew = addNew;
			this.setAt = setAt;
			this.removeAt = removeAt;
		}

		public void Initialize(IDictionaryAdapter dictionaryAdapter, object value)
		{
			var bindingList = (BindingList<T>)value;
			if (addNew != null)
			{
				bindingList.AddingNew += (sender, args) =>
				{
					args.NewObject = addNew();
					addingNew = true;
				};
			}
			bindingList.ListChanged += (sender, args) =>
			{
				switch (args.ListChangedType)
				{
					case ListChangedType.ItemAdded:
						if (addingNew == false && addAt != null)
						{
							var item = addAt(args.NewIndex, bindingList[args.NewIndex]);
							if (item != null)
							{
								using (new SuppressListChangedEvents(bindingList))
								{
									bindingList[args.NewIndex] = (T)item;									
								}
							}
						}
						addingNew = false;
						break;
					case ListChangedType.ItemChanged:
						if (setAt != null)
						{
							var item = setAt(args.NewIndex, bindingList[args.NewIndex]);
							if (item != null)
							{
								using (new SuppressListChangedEvents(bindingList))
								{
									bindingList[args.NewIndex] = (T)item;
								}
							}
						}
						break;
					case ListChangedType.ItemDeleted:
						if (removeAt != null)
						{
							removeAt(args.NewIndex);
						}
						break;
				}
			};
		}

		class SuppressListChangedEvents : IDisposable
		{
			private readonly bool raiseEvents;
			private readonly BindingList<T> bindingList;

			public SuppressListChangedEvents(BindingList<T> bindingList)
			{
				this.bindingList = bindingList;
				raiseEvents = this.bindingList.RaiseListChangedEvents;
				this.bindingList.RaiseListChangedEvents = false;
			}

			public void Dispose()
			{
				bindingList.RaiseListChangedEvents = raiseEvents;
			}
		}
	}

#endif
}
