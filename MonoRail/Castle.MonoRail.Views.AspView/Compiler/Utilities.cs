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
	using System.Collections.Generic;
	using System.Text.RegularExpressions;

	public static class Utilities
	{
		public static string GetAttributesStringFrom(string attributes)
		{
			MatchCollection mathces = Internal.RegularExpressions.Attributes.Matches(attributes);
			
			if (mathces.Count == 0)
			{
				return null;
			}

			List<string> pairs = new List<string>(mathces.Count);

			foreach (Match attribute in mathces)
			{
				string name = attribute.Groups["name"].Value;
				string value = attribute.Groups["value"].Value;
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
			}
			return ", " + string.Join(".", pairs.ToArray());
		}
	}
}
