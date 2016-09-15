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
	/// Contract for manipulating the Dictionary adapter.
	/// </summary>
	public interface IDictionaryAdapter : IDictionaryEdit, IDictionaryNotify, IDictionaryValidate, IDictionaryCreate
	{
		DictionaryAdapterMeta Meta { get; }

		DictionaryAdapterInstance This { get; }

		string GetKey(string propertyName);

		object GetProperty(string propertyName, bool ifExists);

		object ReadProperty(string key);

		T GetPropertyOfType<T>(string propertyName);

		bool SetProperty(string propertyName, ref object value);

		void StoreProperty(PropertyDescriptor property, string key, object value);

		void ClearProperty(PropertyDescriptor property, string key);

		bool ShouldClearProperty(PropertyDescriptor property, object value);

		void CopyTo(IDictionaryAdapter other);

		void CopyTo(IDictionaryAdapter other, Func<PropertyDescriptor, bool> selector);

		T Coerce<T>() where T : class;

		object Coerce(Type type);
	}
}
