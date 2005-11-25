// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
		private String _pattern, _replace;
		private Regex _rule;

		public RoutingRule(String pattern, String replace)
		{
			_pattern = pattern;
			_replace = replace;

			_rule = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
		}

		public String Pattern
		{
			get { return _pattern; }
		}

		public String Replace
		{
			get { return _replace; }
		}

		public Regex CompiledRule
		{
			get { return _rule; }
		}
	}
}