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

namespace Castle.MonoRail.Framework
{
	using System.Collections.Specialized;

	/// <summary>
	/// Dictionary containing helpers.
	/// </summary>
	public class HelperDictionary : HybridDictionary
	{
		/// <summary>
		/// Instatiates a new HelperDictionary.
		/// </summary>
		public HelperDictionary() : base(true)
		{
		}

		/// <summary>
		/// Adds the supplied helper using the standard key naming rules.
		/// </summary>
		/// <param name="helper">The helper to be added</param>
		public void Add(object helper)
		{
			string helperName = helper.GetType().Name;

			if (!Contains(helperName))
			{
				Add(helperName, helper);
			}

			// Also makes the helper available with a less verbose name
			// i.e. FormHelper and Form, AjaxHelper and Ajax
			if (helperName.EndsWith("Helper"))
			{
				string alias = helperName.Substring(0, helperName.Length - 6);

				if (!Contains(alias))
				{
					Add(alias, helper);
				}
			}
		}
	}
}
