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
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#if !DOTNET35

namespace Castle.Components.DictionaryAdapter
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Dynamic;
	using System.Linq;

	/// <summary>
	/// Wraps a <see cref="IDictionary"/> with a dynamic object to expose a bit better looking API.
	/// The implementation is trivial and assumes keys are <see cref="string"/>s.
	/// </summary>
	public class DynamicDictionary : DynamicObject
	{
		private readonly IDictionary dictionary;

		public DynamicDictionary(IDictionary dictionary)
		{
			this.dictionary = dictionary;
		}

		public override IEnumerable<string> GetDynamicMemberNames()
		{
			return from object key in dictionary.Keys select key.ToString();
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			result = dictionary[binder.Name];
			return true;
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			dictionary[binder.Name] = value;
			return true;
		}
	}
}

#endif