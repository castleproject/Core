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
	using System;
	using System.ComponentModel;

	public class BindingListInitializer<T> : IValueInitializer
	{
		private readonly Action<int, object> addAt;
		private readonly Func<object> addNew;
		private readonly Action<int, object> setAt;
		private readonly Action<int> removeAt;
		private bool addingNew;

		public BindingListInitializer(Action<int, object> addAt, Func<object> addNew,
									  Action<int, object> setAt, Action<int> removeAt)
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
							addAt(args.NewIndex, bindingList[args.NewIndex]);
						}
						addingNew = false;
						break;
					case ListChangedType.ItemChanged:
						if (setAt != null)
						{
							setAt(args.NewIndex, bindingList[args.NewIndex]);
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
	}
}
