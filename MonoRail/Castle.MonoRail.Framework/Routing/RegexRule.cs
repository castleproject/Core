// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Routing
{
	using System;
	using System.Text.RegularExpressions;

	/// <summary>
	/// 
	/// </summary>
	public class RegexRule : IRoutingRule
	{
		private readonly string ruleName;
		private readonly string action;
		private readonly Type controllerType;
		private readonly Regex regExp;

		/// <summary>
		/// Initializes a new instance of the <see cref="RegexRule"/> class.
		/// </summary>
		/// <param name="ruleName">Name of the rule.</param>
		/// <param name="regExp">The reg exp.</param>
		/// <param name="controllerType">Type of the controller.</param>
		/// <param name="action">The action.</param>
		private RegexRule(string ruleName, Regex regExp, Type controllerType, string action)
		{
			this.ruleName = ruleName;
			this.regExp = regExp;
			this.controllerType = controllerType;
			this.action = action;
		}

		/// <summary>
		/// Gets the name of the rule.
		/// </summary>
		/// <value>The name of the rule.</value>
		public string RuleName
		{
			get { return ruleName; }
		}

		/// <summary>
		/// Gets the type of the controller.
		/// </summary>
		/// <value>The type of the controller.</value>
		public Type ControllerType
		{
			get { return controllerType; }
		}

		/// <summary>
		/// Gets the action.
		/// </summary>
		/// <value>The action.</value>
		public string Action
		{
			get { return action; }
		}

		/// <summary>
		/// Determines if the specified URL matches the
		/// routing rule.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="match">The match.</param>
		/// <returns></returns>
		public bool Matches(string url, RouteMatch match)
		{
			Match regExpMatch = regExp.Match(url);

			int index = 0;

			foreach(Group group in regExpMatch.Groups)
			{
				if (!group.Success)
				{
					return false;
				}

				string name = regExp.GroupNameFromNumber(index++);

				match.AddNamed(name, group.Value);

				foreach(Capture cap in group.Captures)
				{
					string temp = cap.Value;
				}
			}

			return regExpMatch.Success;
		}

		/// <summary>
		/// Builds the specified rule name.
		/// </summary>
		/// <param name="ruleName">Name of the rule.</param>
		/// <param name="expression">The expression.</param>
		/// <param name="controllerType">Type of the controller.</param>
		/// <param name="action">The action.</param>
		/// <returns></returns>
		public static RegexRule Build(string ruleName, string expression, Type controllerType, string action)
		{
			if (string.IsNullOrEmpty(ruleName))
			{
				throw new ArgumentNullException("ruleName");
			}
			if (string.IsNullOrEmpty(expression))
			{
				throw new ArgumentNullException("expression");
			}
			if (string.IsNullOrEmpty(action))
			{
				throw new ArgumentNullException("action");
			}
			if (controllerType.IsAssignableFrom(typeof(Controller)))
			{
				throw new ArgumentException("The specified type does not inherit from the Controller class", "controllerType");
			}

			Regex regExp = new Regex(expression, RegexOptions.Compiled|RegexOptions.Singleline);

			return new RegexRule(ruleName, regExp, controllerType, action);
		}
	}
}
