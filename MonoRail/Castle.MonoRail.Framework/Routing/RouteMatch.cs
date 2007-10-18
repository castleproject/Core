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
	using System.Collections.Generic;

	/// <summary>
	/// Pendent
	/// </summary>
	public class RouteMatch
	{
		private readonly Type controllerType;
		private readonly string ruleName;
		private readonly string action;
		private readonly List<string> literals = new List<string>();
		private readonly Dictionary<string, string> parameters = new Dictionary<string, string>();

		/// <summary>
		/// Initializes a new instance of the <see cref="RouteMatch"/> class.
		/// </summary>
		/// <param name="controllerType">Type of the controller.</param>
		/// <param name="ruleName">Name of the rule.</param>
		/// <param name="action">The action.</param>
		public RouteMatch(Type controllerType, string ruleName, string action)
		{
			this.controllerType = controllerType;
			this.ruleName = ruleName;
			this.action = action;
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
		/// Gets the name of the rule.
		/// </summary>
		/// <value>The name of the rule.</value>
		public string RuleName
		{
			get { return ruleName; }
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
		/// Gets the literals.
		/// </summary>
		/// <value>The literals.</value>
		public List<string> Literals
		{
			get { return literals; }
		}

		/// <summary>
		/// Gets the parameters.
		/// </summary>
		/// <value>The parameters.</value>
		public Dictionary<string, string> Parameters
		{
			get { return parameters; }
		}

		/// <summary>
		/// Adds the specified literal.
		/// </summary>
		/// <param name="literal">The literal.</param>
		public void Add(string literal)
		{
			literals.Add(literal);
		}

		/// <summary>
		/// Adds the named.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public void AddNamed(string name, string value)
		{
			parameters[name] = value;
		}
	}
}