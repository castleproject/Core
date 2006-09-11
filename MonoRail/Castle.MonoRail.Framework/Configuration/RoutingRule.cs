// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Configuration
{
	using System;
	using System.Text.RegularExpressions;

	/// <summary>
	/// Pendent
	/// </summary>
	public class RoutingRule
	{
		private String pattern, replace;
		private Regex rule;

		public RoutingRule(string pattern, string replace)
		{
			this.pattern = pattern;
			this.replace = replace;
			
			rule = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
		}

		public String Pattern
		{
			get { return pattern; }
		}

		public String Replace
		{
			get { return replace; }
		}

		public Regex CompiledRule
		{
			get { return rule; }
		}
	}
}