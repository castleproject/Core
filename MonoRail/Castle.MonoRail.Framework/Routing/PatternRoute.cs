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

namespace Castle.MonoRail.Framework.Routing
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Text;
	using System.Text.RegularExpressions;
	using Castle.MonoRail.Framework.Services.Utils;
	using Descriptors;

	/// <summary>
	/// Pendent
	/// </summary>
	[DebuggerDisplay("PatternRoute {pattern}")]
	public class PatternRoute : IRoutingRule
	{
		private readonly string name;
		private readonly string pattern;
		private readonly NodeCollection nodes = new NodeCollection();

		private readonly Dictionary<string, string> defaults =
			new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

		/// <summary>
		/// Initializes a new instance of the <see cref="PatternRoute"/> class.
		/// </summary>
		/// <param name="pattern">The pattern.</param>
		public PatternRoute(string pattern)
		{
			this.pattern = pattern;
			CreatePatternNodes();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PatternRoute"/> class.
		/// </summary>
		/// <param name="name">The route name.</param>
		/// <param name="pattern">The pattern.</param>
		public PatternRoute(string name, string pattern) : this(pattern)
		{
			this.name = name;
		}

		/// <summary>
		/// Gets the name of the route.
		/// </summary>
		/// <value>The name of the route.</value>
		public string RouteName
		{
			get { return name; }
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		public string CreateUrl(IDictionary parameters)
		{
			StringBuilder text = new StringBuilder();
			IList<string> checkedParameters = new List<string>();

			// int namedParamsToCheck = 0;

			// checks whether we have a named node for every parameter
			foreach (string key in parameters.Keys)
			{
				object param = parameters[key];
				string val = param == null ? null : param.ToString();

				if (string.IsNullOrEmpty(val) ||
					key.Equals("controller", StringComparison.OrdinalIgnoreCase) ||
					key.Equals("action", StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}

				if (nodes.FindByName(key) == null)
				{
					return null;
				}
			}

			foreach(UrlPartSubRule node in nodes)
			{
				AppendSlashOrDot(text, node);

				if (node.name == null)
				{
					text.Append(node.start);
				}
				else
				{
					checkedParameters.Add(node.name);

					object value = parameters[node.name];
					string valAsString = value != null ? value.ToString() : null;

					if (string.IsNullOrEmpty(valAsString))
					{
						if (!node.optional)
						{
							return null;
						}
						else
						{
							break;
						}
					}
					else
					{
						if (node.hasRestriction && !node.Accepts(value.ToString()))
						{
							return null;
						}

						if (node.optional &&
							StringComparer.InvariantCultureIgnoreCase.Compare(node.DefaultVal, value.ToString()) == 0)
						{
							break; // end as there can't be more required nodes after an optional one
						}

						text.Append(value.ToString());
					}
				}
			}

			// Validate that default parameters match parameters passed into to create url.
			foreach(KeyValuePair<string, string> defaultParameter in defaults)
			{
				// Skip parameters we already checked.
				if (checkedParameters.Contains(defaultParameter.Key))
				{
					continue;
				}

				object value = parameters[defaultParameter.Key];
				string valAsString = value != null ? value.ToString() : null;
				if (!string.IsNullOrEmpty(valAsString) &&
					!defaultParameter.Value.Equals(valAsString, StringComparison.OrdinalIgnoreCase))
				{
					return null;
				}
			}

			if (text.Length == 0 || text[text.Length - 1] == '/' || text[text.Length - 1] == '.')
			{
				text.Length = text.Length - 1;
			}

			return text.ToString();
		}

		/// <summary>
		/// Determines if the specified URL matches the
		/// routing rule.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="context">The context</param>
		/// <param name="match">The match.</param>
		/// <returns></returns>
		public int Matches(string url, IRouteContext context, RouteMatch match)
		{
			string[] parts = url.Split(new char[] { '/', '.' }, StringSplitOptions.RemoveEmptyEntries);
			int points = 0;
			int index = 0;

			foreach(UrlPartSubRule node in nodes)
			{
				string part = index < parts.Length ? parts[index] : null;

				if (!node.Matches(part, match, ref points))
				{
					points = 0;
					break;
				}

				index++;
			}

			if (points != 0)
			{
				// Fills parameters set on the route that cannot be fulfilled by the url
				foreach (KeyValuePair<string, string> pair in defaults)
				{
					if (!match.Parameters.ContainsKey(pair.Key))
					{
						match.Parameters.Add(pair.Key, pair.Value);
					}
				}
			}

			return points;
		}

		private void CreatePatternNodes()
		{
			string[] parts = pattern.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

			if (pattern == "/")
			{
				nodes.Add(new EmptyUrlPartSubRule());
			}
			else
			{
				foreach(string part in parts)
				{
					string[] subparts = part.Split(new char[] { '.' }, 2, StringSplitOptions.RemoveEmptyEntries);

					if (subparts.Length == 2)
					{
						bool afterDot = false;

						foreach(string subpart in subparts)
						{
							if (subpart.Contains("["))
							{
								nodes.Add(CreateNamedOptionalNode(subpart, afterDot));
							}
							else
							{
								nodes.Add(CreateRequiredNode(subpart, afterDot));
							}

							afterDot = true;
						}
					}
					else
					{
						if (part.Contains("["))
						{
							nodes.Add(CreateNamedOptionalNode(part, false));
						}
						else
						{
							nodes.Add(CreateRequiredNode(part, false));
						}
					}
				}
			}
		}

		/// <summary>
		/// Adds a default entry.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public void AddDefault(string key, string value)
		{
			defaults[key] = value;
		}

		private UrlPartSubRule CreateNamedOptionalNode(string part, bool afterDot)
		{
			return new UrlPartSubRule(part, true, afterDot);
		}

		private UrlPartSubRule CreateRequiredNode(string part, bool afterDot)
		{
			return new UrlPartSubRule(part, false, afterDot);
		}

		private static void AppendSlashOrDot(StringBuilder text, UrlPartSubRule partSubRule)
		{
			if (text.Length == 0 || text[text.Length - 1] != '/')
			{
				if (partSubRule.afterDot)
				{
					text.Append('.');
				}
				else
				{
					text.Append('/');
				}
			}
		}

		#region NodeCollection

		[DebuggerDisplay("Nodes {Length}")]
		private class NodeCollection : System.Collections.ObjectModel.Collection<UrlPartSubRule>
		{
			private readonly Dictionary<string, UrlPartSubRule> nameToNode = new Dictionary<string, UrlPartSubRule>(StringComparer.OrdinalIgnoreCase);

			protected override void InsertItem(int index, UrlPartSubRule item)
			{
				AddToDictionary(item);
				base.InsertItem(index, item);
			}

			protected override void SetItem(int index, UrlPartSubRule item)
			{
				AddToDictionary(item);
				base.SetItem(index, item);
			}

			private void AddToDictionary(UrlPartSubRule item)
			{
				if (item.name != null)
				{
					nameToNode[item.name] = item;
				}
			}

			public UrlPartSubRule FindByName(string name)
			{
				UrlPartSubRule partSubRule;
				nameToNode.TryGetValue(name, out partSubRule);
				return partSubRule;
			}
		}

		#endregion

		#region UrlPartSubRule

		private class EmptyUrlPartSubRule : UrlPartSubRule
		{
			public EmptyUrlPartSubRule() : base(string.Empty, false, false)
			{
			}

			public override bool Matches(string part, RouteMatch match, ref int points)
			{
				if (part == null)
				{
					points = 1;
					return true;
				}

				return false;
			}
		}

		[DebuggerDisplay("Node {name} Opt: {optional} default: {defaultVal} Regular exp: {exp}")]
		private class UrlPartSubRule
		{
			public readonly string name, start, end;
			public readonly bool optional;
			public readonly bool afterDot;
			public bool hasRestriction, isStaticNode;
			private string defaultVal;
			private string[] acceptedTokens;
			private string notAcceptedToken;
			private Regex exp;
			private string acceptedRegex;

			public UrlPartSubRule(string part, bool optional, bool afterDot)
			{
				this.optional = optional;
				this.afterDot = afterDot;
				int indexStart = part.IndexOfAny(new char[] { '<', '[' });
				int indexEndStart = -1;

				if (indexStart != -1)
				{
					indexEndStart = part.IndexOfAny(new char[] { '>', ']' }, indexStart);
					name = part.Substring(indexStart + 1, indexEndStart - indexStart - 1);
				}

				if (indexStart != -1)
				{
					start = part.Substring(0, indexStart);
				}
				else
				{
					start = part;
				}

				end = "";

				if (indexEndStart != -1)
				{
					end = part.Substring(indexEndStart + 1);
				}

				ReBuildRegularExpression();
			}

			private void ReBuildRegularExpression()
			{
				RegexOptions options = RegexOptions.Compiled | RegexOptions.Singleline;

				if (name != null)
				{
					isStaticNode = false;
					exp = new Regex("^" + CharClass(start) + "(" + GetExpression() + ")" + CharClass(end) + "$", options);
				}
				else
				{
					isStaticNode = true;
					exp = new Regex("^(" + CharClass(start) + ")$");
				}
			}

			private string GetExpression()
			{
				if (!string.IsNullOrEmpty(acceptedRegex))
				{
					return acceptedRegex;
				}
				else if (!string.IsNullOrEmpty(notAcceptedToken))
				{
					// \w+(?<!view|index)\b
					return "\\w+(?<!" + CharClass(notAcceptedToken) + ")\\b";
				}
				else if (acceptedTokens != null && acceptedTokens.Length != 0)
				{
					StringBuilder text = new StringBuilder();

					foreach (string token in acceptedTokens)
					{
						if (text.Length != 0)
						{
							text.Append("|");
						}
						text.Append("(");
						text.Append(CharClass(token));
						text.Append(")");
					}

					return text.ToString();
				}
				else
				{
					return "[a-zA-Z,_,0-9,-]+";
				}
			}

			public virtual bool Matches(string part, RouteMatch match, ref int points)
			{
				if (part == null)
				{
					if (optional)
					{
						if (name != null)
						{
							// matching defaults is better than nothing, so
							// assign at least a very small number of points
							points += 1;
							match.AddNamed(name, defaultVal);
						}

						return true;
					}
					else
					{
						return false;
					}
				}

				Match regExpMatch = exp.Match(part);

				if (regExpMatch.Success)
				{
					if (name != null)
					{
						match.AddNamed(name, part);
					}
					// matching non-default nodes is preferred, static nodes even more,
					// so assign a high number of points.
					// By using very high values it is ensured that defaults are never
					// preferred over non-default matches
					points += isStaticNode ? 4000 : 2000;

					return true;
				}

				return false;
			}

			public void AcceptsAnyOf(string[] names)
			{
				hasRestriction = true;
				acceptedTokens = names;
				ReBuildRegularExpression();
			}

			public void DoesNotAccept(string value)
			{
				hasRestriction = true;
				notAcceptedToken = value;
				ReBuildRegularExpression();
			}

			public string DefaultVal
			{
				get { return defaultVal; }
				set { defaultVal = value; }
			}

			public bool AcceptsIntOnly
			{
				set { AcceptsRegex("[0-9]+"); }
			}

			public bool AcceptsGuidsOnly
			{
				set
				{
					AcceptsRegex("[A-Fa-f0-9]{32}|" +
								 "({|\\()?[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}(}|\\))?|" +
								 "({)?[0xA-Fa-f0-9]{3,10}(, {0,1}[0xA-Fa-f0-9]{3,6}){2}, {0,1}({)([0xA-Fa-f0-9]{3,4}, {0,1}){7}[0xA-Fa-f0-9]{3,4}(}})");
				}
			}

			public void AcceptsRegex(string regex)
			{
				hasRestriction = true;
				acceptedRegex = regex;
				ReBuildRegularExpression();
			}

			public bool Accepts(string val)
			{
				Match regExpMatch = exp.Match(val);

				return (regExpMatch.Success);
			}
		}

		#endregion

		/// <summary>
		/// Configures the default for the named pattern part.
		/// </summary>
		/// <param name="namedPatternPart">The named pattern part.</param>
		/// <returns></returns>
		public DefaultConfigurer DefaultFor(string namedPatternPart)
		{
			return new DefaultConfigurer(this, namedPatternPart);
		}

		/// <summary>
		/// Configures the default for the named pattern part.
		/// </summary>
		/// <returns></returns>
		public DefaultConfigurer DefaultForController()
		{
			return new DefaultConfigurer(this, "controller");
		}

		/// <summary>
		/// Configures the default for the named pattern part.
		/// </summary>
		/// <returns></returns>
		public DefaultConfigurer DefaultForAction()
		{
			return new DefaultConfigurer(this, "action");
		}

		/// <summary>
		/// Configures the default for the named pattern part.
		/// </summary>
		/// <returns></returns>
		public DefaultConfigurer DefaultForArea()
		{
			return new DefaultConfigurer(this, "area");
		}

		/// <summary>
		/// Configures restrictions for the named pattern part.
		/// </summary>
		/// <param name="namedPatternPart">The named pattern part.</param>
		/// <returns></returns>
		public RestrictionConfigurer Restrict(string namedPatternPart)
		{
			return new RestrictionConfigurer(this, namedPatternPart);
		}

		/// <summary>
		/// Pendent
		/// </summary>
		public class RestrictionConfigurer
		{
			private readonly PatternRoute route;
			private readonly UrlPartSubRule targetPartSubRule;

			/// <summary>
			/// Initializes a new instance of the <see cref="RestrictionConfigurer"/> class.
			/// </summary>
			/// <param name="route">The route.</param>
			/// <param name="namedPatternPart">The named pattern part.</param>
			public RestrictionConfigurer(PatternRoute route, string namedPatternPart)
			{
				this.route = route;
				targetPartSubRule = route.GetNamedNode(namedPatternPart, true);
			}

			/// <summary>
			/// Restricts this named pattern part to only accept one of the 
			/// strings passed in.
			/// </summary>
			/// <param name="validNames">The valid names.</param>
			/// <returns></returns>
			public PatternRoute AnyOf(params string[] validNames)
			{
				targetPartSubRule.AcceptsAnyOf(validNames);
				return route;
			}

			/// <summary>
			/// Restricts this named pattern part to only accept content
			/// that does not match the string specified.
			/// </summary>
			/// <param name="name">The name that cannot be matched.</param>
			/// <returns></returns>
			public PatternRoute AnythingBut(string name)
			{
				targetPartSubRule.DoesNotAccept(name);
				return route;
			}

			/// <summary>
			/// Restricts this named pattern part to only accept integers.
			/// </summary>
			/// <value>The valid integer.</value>
			public PatternRoute ValidInteger
			{
				get
				{
					targetPartSubRule.AcceptsIntOnly = true;
					return route;
				}
			}

			/// <summary>
			/// Restricts this named pattern part to only accept guids.
			/// </summary>
			public PatternRoute ValidGuid
			{
				get
				{
					targetPartSubRule.AcceptsGuidsOnly = true;
					return route;
				}
			}

			/// <summary>
			/// Restricts this named pattern part to only accept strings
			/// matching the regular expression passed in.
			/// </summary>
			/// <param name="regex"></param>
			/// <returns></returns>
			public PatternRoute ValidRegex(string regex)
			{
				targetPartSubRule.AcceptsRegex(regex);
				return route;
			}
		}

		/// <summary>
		/// Pendent
		/// </summary>
		public class DefaultConfigurer
		{
			private readonly PatternRoute route;
			private readonly string namedPatternPart;
			private readonly UrlPartSubRule targetPartSubRule;

			/// <summary>
			/// Initializes a new instance of the <see cref="DefaultConfigurer"/> class.
			/// </summary>
			/// <param name="patternRoute">The pattern route.</param>
			/// <param name="namedPatternPart">The named pattern part.</param>
			public DefaultConfigurer(PatternRoute patternRoute, string namedPatternPart)
			{
				route = patternRoute;
				this.namedPatternPart = namedPatternPart;
				targetPartSubRule = route.GetNamedNode(namedPatternPart, false);
			}

			/// <summary>
			/// Sets the default value for this named pattern part.
			/// </summary>
			/// <returns></returns>
			public PatternRoute Is<T>() where T : class, IController
			{
				ControllerDescriptor desc = ControllerInspectionUtil.Inspect(typeof(T));
				if (targetPartSubRule != null)
				{
					targetPartSubRule.DefaultVal = desc.Name;
				}
				route.AddDefault(namedPatternPart, desc.Name);
				return route;
			}

			/// <summary>
			/// Sets the default value for this named pattern part.
			/// </summary>
			/// <param name="value">The value.</param>
			/// <returns></returns>
			public PatternRoute Is(string value)
			{
				if (targetPartSubRule != null)
				{
					targetPartSubRule.DefaultVal = value;
				}
				route.AddDefault(namedPatternPart, value);
				return route;
			}

			/// <summary>
			/// Sets the default value as empty for this named pattern part.
			/// </summary>
			/// <value>The is empty.</value>
			public PatternRoute IsEmpty
			{
				get { return Is(string.Empty); }
			}
		}

		// See http://weblogs.asp.net/justin_rogers/archive/2004/03/20/93379.aspx
		private static string CharClass(string content)
		{
			if (content == String.Empty)
			{
				return string.Empty;
			}

			StringBuilder builder = new StringBuilder();

			foreach (char c in content)
			{
				if (char.IsLetter(c))
				{
					builder.AppendFormat("[{0}{1}]", char.ToLower(c), char.ToUpper(c));
				}
				else
				{
					builder.Append(c);
				}
			}

			return builder.ToString();
		}

		/// <summary>
		/// Gets the named node.
		/// </summary>
		/// <param name="part">The part.</param>
		/// <param name="mustFind">if set to <c>true</c> [must find].</param>
		/// <returns></returns>
		private UrlPartSubRule GetNamedNode(string part, bool mustFind)
		{
			UrlPartSubRule found = nodes.FindByName(part); // (delegate(UrlPartSubRule node) { return node.name == part; });

			if (found == null && mustFind)
			{
				throw new ArgumentException("Could not find pattern node for name " + part);
			}

			return found;
		}
	}
}
