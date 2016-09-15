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

	/// <summary>
	/// Conract for traversing a <see cref="IDictionaryAdapter"/>.
	/// </summary>
	public interface IDictionaryAdapterVisitor
	{
		bool VisitDictionaryAdapter(IDictionaryAdapter dictionaryAdapter, object state);

		bool VisitDictionaryAdapter(IDictionaryAdapter dictionaryAdapter, Func<PropertyDescriptor, bool> selector, object state);

		void VisitProperty(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property, object state);

		void VisitInterface(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property, object state);

		void VisitCollection(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property, Type collectionItemType, object state);
	}
}
