// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Views.AspView.Compiler
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Text;
	using System.Text.RegularExpressions;
	using Castle.MonoRail.Framework.Helpers;

	public static class Utilities
	{

		public static string GetAttributesStringFrom(string attributes) 
		{
			StringBuilder attributesString = new StringBuilder();
			List<string> pairs = new List<string>();
			MatchCollection matches = Internal.RegularExpressions.Attributes.Matches(attributes);
			if (matches.Count == 0)
				return null;
			DoSomethingWithAttributes(
				matches,
				delegate(string name, string value) 
				{
					if (value.Trim().StartsWith("<%=")) 
					{
						value = value.Trim().Substring(3, value.Length - 5).Trim();
					}
					else 
					{
						// string literals should be escaped (CONTRIB-110)
						value = value.Replace("\\", "\\\\");
						value = string.Format("\"{0}\"", value);
					}
					pairs.Add(string.Format("N(\"{0}\", {1})", name, value));
				});
			
			return ", " + string.Join(".", pairs.ToArray());
		}

		public static IDictionary GetAttributesDictionaryFrom(string attributes)
		{
			IDictionary attributesDictionary = new DictHelper.MonoRailDictionary();
			MatchCollection matches = Internal.RegularExpressions.Attributes.Matches(attributes);
			DoSomethingWithAttributes(matches,
				delegate (string name, string value)
				{
					attributesDictionary.Add(name, value);
				});
			return attributesDictionary;
		}

		private delegate void KeyValuePairFunctor(string key, string value);

		private static void DoSomethingWithAttributes(MatchCollection matches,  KeyValuePairFunctor functor)
		{
			foreach (Match attribute in matches)
			{
				string name = attribute.Groups["name"].Value;
				string value = attribute.Groups["value"].Value;
				functor(name, value);
			}
		}
	}
}
