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
	/// Generates a new GUID on demand.
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property, AllowMultiple = false)]
	public class NewGuidAttribute : DictionaryBehaviorAttribute, IDictionaryPropertyGetter
	{
		private static readonly Guid UnassignedGuid = new Guid();

		public object GetPropertyValue(IDictionaryAdapter dictionaryAdapter,
			string key, object storedValue, PropertyDescriptor property, bool ifExists)
		{
			if (storedValue == null || storedValue.Equals(UnassignedGuid))
			{
				storedValue = Guid.NewGuid();
				property.SetPropertyValue(dictionaryAdapter, key, ref storedValue, dictionaryAdapter.This.Descriptor);
			}

			return storedValue;
		}
	}
}
