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

namespace Castle.Components.DictionaryAdapter.Tests
{
	using System.Collections.Generic;

	using NUnit.Framework;

	public class DynamicDictionaryTests
	{
		[Test]
		public void Can_add_to_dictionary_via_dynamic_object()
		{
			var dictionary = new Dictionary<string, object>();
			dynamic adapter = new DynamicDictionary(dictionary);
			adapter.Name = "stefan mucha";

			Assert.AreEqual("stefan mucha", dictionary["Name"]);
		}

		[Test]
		public void Can_override_value_from_dictionary_via_dynamic_object()
		{
			var dictionary = new Dictionary<string, object>();
			dynamic adapter = new DynamicDictionary(dictionary);
			dictionary["Name"] = "adam mickiewicz";
			adapter.Name = "stefan mucha";

			Assert.AreEqual("stefan mucha", adapter.Name);
		}

		[Test]
		public void Can_read_from_dictionary_via_dynamic_object()
		{
			var dictionary = new Dictionary<string, object>();
			dynamic adapter = new DynamicDictionary(dictionary);
			dictionary["Name"] = "stefan mucha";

			Assert.AreEqual("stefan mucha", adapter.Name);
		}

		[Test]
		public void Can_read_non_existing_value_from_dictionary_via_dynamic_object()
		{
			var dictionary = new Dictionary<string, object>();
			dynamic adapter = new DynamicDictionary(dictionary);

			Assert.IsNull(adapter.Name);
		}
	}
}

#endif