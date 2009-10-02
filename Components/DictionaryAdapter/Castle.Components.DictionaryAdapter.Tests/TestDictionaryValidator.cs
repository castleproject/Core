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

namespace Castle.Components.DictionaryAdapter.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class TestDictionaryValidator : IDictionaryValidator, IDictionaryInitializer
	{
		private List<Func<IDictionaryAdapter, String, String[]>> validationRules;

		public TestDictionaryValidator()
		{
			validationRules = new List<Func<IDictionaryAdapter, string, string[]>>();
		}

		void IDictionaryInitializer.Initialize(IDictionaryAdapter dictionaryAdapter)
		{
			dictionaryAdapter.Validator = this;
		}

		public TestDictionaryValidator AddValidationRules(params Func<IDictionaryAdapter, String, String[]>[] rules)
		{
			validationRules.AddRange(rules);
			return this;
		}

		public string Validate(IDictionaryAdapter dictionaryAdapter)
		{
			return String.Join(Environment.NewLine, validationRules.SelectMany(
				rule => rule(dictionaryAdapter, null)).ToArray());
		}

		public string Validate(IDictionaryAdapter dictionaryAdapter, string propertyName)
		{
			return String.Join(Environment.NewLine, validationRules.SelectMany(
				rule => rule(dictionaryAdapter, propertyName)).ToArray());
		}
	}
}
