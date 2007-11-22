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
	using System.Collections;
	using System.Collections.Generic;
	using System.Text;
	using System.Text.RegularExpressions;

	/// <summary>
	/// Pendent
	/// </summary>
	public class PatternRule : IRoutingRule
	{
		private readonly string routeName;
		private readonly string path;
		private readonly Type controllerType;
		private readonly string action;
		private UrlPathNode[] nodes;
		private bool hasGreedyNode;

		/// <summary>
		/// Initializes a new instance of the <see cref="PatternRule"/> class.
		/// </summary>
		/// <param name="routeName">Name of the rule.</param>
		/// <param name="path">The path.</param>
		/// <param name="controllerType">Type of the controller.</param>
		/// <param name="action">The action.</param>
		protected PatternRule(string routeName, string path, Type controllerType, string action)
		{
			this.routeName = routeName;
			this.path = path;
			this.controllerType = controllerType;
			this.action = action;
		}

		/// <summary>
		/// Gets the name of the route.
		/// </summary>
		/// <value>The name of the route.</value>
		public string RouteName
		{
			get { return routeName; }
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
		/// Matcheses the specified URL.
		/// </summary>
		/// <param name="hostname"></param>
		/// <param name="virtualPath"></param>
		/// <param name="url">The URL.</param>
		/// <param name="match">The match.</param>
		/// <returns></returns>
		public bool Matches(string hostname, string virtualPath, string url, RouteMatch match)
		{
			string[] pieces = url.Split('/');

			if (!hasGreedyNode)
			{
				if (pieces.Length != nodes.Length)
				{
					return false;
				}
			}

			int curPiece = 0;

			foreach(UrlPathNode node in nodes)
			{
				if (curPiece == pieces.Length) // no left pieces, but still there are nodes to match
				{
					// if the node allows match empties, ok, 
					// otherwise it wont match this rule
					return node.MatchesEmpty; 
				}

				if (!node.Matches(pieces[curPiece], match))
				{
					return false;
				}

				curPiece++;
			}

			return true;
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="hostname">The hostname.</param>
		/// <param name="virtualPath">The virtual path.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		public string CreateUrl(string hostname, string virtualPath, IDictionary parameters)
		{
			if( virtualPath != null && !virtualPath.StartsWith("/"))
			{
				virtualPath = "/" + virtualPath;
			}
			StringBuilder sb = new StringBuilder(virtualPath);

			foreach(UrlPathNode node in nodes)
			{
				if (sb.Length == 0 || sb[sb.Length - 1] != '/')
				{
					sb.Append('/');
				}

				sb.Append(node.CreateUrlPiece(parameters));
			}

			return sb.ToString();
		}

		/// <summary>
		/// Builds the specified rule name.
		/// </summary>
		/// <param name="ruleName">Name of the rule.</param>
		/// <param name="path">The path.</param>
		/// <param name="controllerType">Type of the controller.</param>
		/// <param name="action">The action.</param>
		/// <returns></returns>
		public static PatternRule Build(string ruleName, string path, Type controllerType, string action)
		{
			if (string.IsNullOrEmpty(ruleName))
			{
				throw new ArgumentNullException("ruleName");
			}
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			if (string.IsNullOrEmpty(action))
			{
				throw new ArgumentNullException("action");
			}
			if (controllerType.IsAssignableFrom(typeof(Controller)))
			{
				throw new ArgumentException("The specified type does not inherit from the Controller class", "controllerType");
			}

			PatternRule rule = new PatternRule(ruleName, path, controllerType, action);
			rule.Prepare();
			return rule;
		}

		/// <summary>
		/// Prepares this instance.
		/// </summary>
		private void Prepare()
		{
			List<UrlPathNode> tempTokens = new List<UrlPathNode>();

			foreach(string part in path.Split('/'))
			{
				if (string.IsNullOrEmpty(part))
				{
					break;
				}

				UrlPathNode node = UrlPathNodeFactory.Create(part);

				tempTokens.Add(node);

				if (node.MatchesEmpty)
				{
					hasGreedyNode = true;
				}
			}

			nodes = tempTokens.ToArray();
		}

		/// <summary>
		/// Pendent
		/// </summary>
		public enum UrlNodeType
		{
			/// <summary>
			/// Pendent
			/// </summary>
			Number,
			/// <summary>
			/// Pendent
			/// </summary>
			String,
			/// <summary>
			/// Pendent
			/// </summary>
			Choice
		}

		/// <summary>
		/// Pendent
		/// </summary>
		public abstract class UrlPathNode
		{
			/// <summary>
			/// Matches the specified piece.
			/// </summary>
			/// <param name="piece">The piece.</param>
			/// <param name="match">The match.</param>
			/// <returns></returns>
			public abstract bool Matches(string piece, RouteMatch match);

			/// <summary>
			/// Gets a value indicating whether this node matches even empty pieces
			/// </summary>
			/// <value><c>true</c> if allows empty as match; otherwise, <c>false</c>.</value>
			public abstract bool MatchesEmpty { get; }

			/// <summary>
			/// Creates the URL piece.
			/// </summary>
			/// <param name="parameters">The parameters.</param>
			/// <returns></returns>
			public abstract string CreateUrlPiece(IDictionary parameters);
		}

		/// <summary>
		/// Pendent
		/// </summary>
		public class LiteralNode : UrlPathNode
		{
			private readonly string token;

			/// <summary>
			/// Initializes a new instance of the <see cref="LiteralNode"/> class.
			/// </summary>
			/// <param name="token">The token.</param>
			public LiteralNode(string token)
			{
				this.token = token;
			}

			/// <summary>
			/// Matches the specified piece.
			/// </summary>
			/// <param name="piece">The piece.</param>
			/// <param name="match">The match.</param>
			/// <returns></returns>
			public override bool Matches(string piece, RouteMatch match)
			{
				if (string.Compare(piece, token, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					match.Add(token);
					return true;
				}

				return false;
			}

			/// <summary>
			/// Gets a value indicating whether this node matches even empty pieces
			/// </summary>
			/// <value><c>true</c> if allows empty as match; otherwise, <c>false</c>.</value>
			public override bool MatchesEmpty
			{
				get { return false; }
			}

			/// <summary>
			/// Creates the URL piece.
			/// </summary>
			/// <param name="parameters">The parameters.</param>
			/// <returns></returns>
			public override string CreateUrlPiece(IDictionary parameters)
			{
				return token;
			}
		}

		/// <summary>
		/// Pendent
		/// </summary>
		public class MatchAllNode : UrlPathNode
		{
			/// <summary>
			/// Matches the specified piece.
			/// </summary>
			/// <param name="piece">The piece.</param>
			/// <param name="match">The match.</param>
			/// <returns></returns>
			public override bool Matches(string piece, RouteMatch match)
			{
				return true;
			}

			/// <summary>
			/// Gets a value indicating whether this node matches even empty pieces
			/// </summary>
			/// <value><c>true</c> if allows empty as match; otherwise, <c>false</c>.</value>
			public override bool MatchesEmpty
			{
				get { return true; }
			}

			/// <summary>
			/// Creates the URL piece.
			/// </summary>
			/// <param name="parameters">The parameters.</param>
			/// <returns></returns>
			public override string CreateUrlPiece(IDictionary parameters)
			{
				return string.Empty;
			}
		}

		/// <summary>
		/// Pendent
		/// </summary>
		public class RequiredChoiceNode : UrlPathNode
		{
			private readonly string nodeName;
			private readonly Regex choicePattern;

			/// <summary>
			/// Initializes a new instance of the <see cref="RequiredChoiceNode"/> class.
			/// </summary>
			/// <param name="nodeName">Name of the node.</param>
			/// <param name="choices">The choices.</param>
			public RequiredChoiceNode(string nodeName, string choices)
			{
				this.nodeName = nodeName;

				StringBuilder sb = new StringBuilder();
				foreach(string choice in choices.Split('|'))
				{
					if (sb.Length != 0) sb.Append('|');
					sb.Append("^(" + choice + ")$");
				}

				RegexOptions options = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase;

				choicePattern = new Regex(sb.ToString(), options);
			}

			/// <summary>
			/// Matches the specified piece.
			/// </summary>
			/// <param name="piece">The piece.</param>
			/// <param name="match">The match.</param>
			/// <returns></returns>
			public override bool Matches(string piece, RouteMatch match)
			{
				Match regExMatch = choicePattern.Match(piece);

				foreach(Group group in regExMatch.Groups)
				{
					if (!group.Success) continue;

					match.AddNamed(nodeName, group.Value);

					return true;
				}

				return false;
			}

			/// <summary>
			/// Gets a value indicating whether this node matches even empty pieces
			/// </summary>
			/// <value><c>true</c> if allows empty as match; otherwise, <c>false</c>.</value>
			public override bool MatchesEmpty
			{
				get { return false; }
			}

			/// <summary>
			/// Creates the URL piece.
			/// </summary>
			/// <param name="parameters">The parameters.</param>
			/// <returns></returns>
			public override string CreateUrlPiece(IDictionary parameters)
			{
				object val = parameters[nodeName];

				if (val == null)
				{
					throw new ArgumentException("Missing parameter '" + nodeName + "' used to build url from routing rule");
				}

				return val.ToString();
			}
		}

		/// <summary>
		/// Pendent
		/// </summary>
		public class RequiredNode : UrlPathNode
		{
			private readonly static Regex NumberPattern = new Regex("\\d", RegexOptions.Compiled|RegexOptions.Singleline);
			private readonly string nodeName;
			private readonly UrlNodeType type;

			/// <summary>
			/// Initializes a new instance of the <see cref="RequiredNode"/> class.
			/// </summary>
			/// <param name="nodeName">Name of the node.</param>
			/// <param name="type">The type.</param>
			public RequiredNode(string nodeName, UrlNodeType type)
			{
				this.nodeName = nodeName;
				this.type = type;
			}

			/// <summary>
			/// Matches the specified piece.
			/// </summary>
			/// <param name="piece">The piece.</param>
			/// <param name="match">The match.</param>
			/// <returns></returns>
			public override bool Matches(string piece, RouteMatch match)
			{
				if (type == UrlNodeType.String)
				{
					match.AddNamed(nodeName, piece);
					return !string.IsNullOrEmpty(piece);
				}
				else if (type == UrlNodeType.Number)
				{
					if (NumberPattern.Match(piece).Success)
					{
						match.AddNamed(nodeName, piece);
						return true;
					}
				}

				return false;
			}

			/// <summary>
			/// Gets a value indicating whether this node matches even empty pieces
			/// </summary>
			/// <value><c>true</c> if allows empty as match; otherwise, <c>false</c>.</value>
			public override bool MatchesEmpty
			{
				get { return false; }
			}

			/// <summary>
			/// Creates the URL piece.
			/// </summary>
			/// <param name="parameters">The parameters.</param>
			/// <returns></returns>
			public override string CreateUrlPiece(IDictionary parameters)
			{
				object val = parameters[nodeName];

				if (val == null)
				{
					throw new ArgumentException("Missing parameter '" + nodeName + "' used to build url from routing rule");
				}

				return val.ToString();
			}
		}

		/// <summary>
		/// Pendent
		/// </summary>
		public class UrlPathNodeFactory
		{
			/// <summary>
			/// Creates the specified token.
			/// </summary>
			/// <param name="token">The token.</param>
			/// <returns></returns>
			public static UrlPathNode Create(string token)
			{
				if (string.IsNullOrEmpty(token))
				{
					throw new ArgumentNullException("token");
				}

				if (token[0] == '<' || token[0] == '[')
				{
					return ProcessPattern(token);
				}
				else if (token == "*")
				{
					return new MatchAllNode();
				}
				else
				{
					return new LiteralNode(token);
				}
			}

			private static UrlPathNode ProcessPattern(string token)
			{
				bool isOptional = token[0] == '[';
				char lastChar = token[token.Length - 1];
				bool isWellFormed = lastChar == (isOptional ? ']' : '>');
				bool hasSpaces = token.IndexOf(' ') != -1;

				if (!isWellFormed)
				{
					throw new ArgumentException("Token is not wellformed. It should end with '>' or ']'");
				}
				if (hasSpaces)
				{
					throw new ArgumentException("Spaces are not allowed on a pattern token. Please check the pattern '" + token + "'");
				}

				token = token.Substring(0, token.Length - 1);

				// Has type?

				UrlNodeType type = UrlNodeType.String;

				int index = token.IndexOf(':');

				string paramArg = null;

				if (index != -1)
				{
					// Yeah, so let's check it

					paramArg = token.Substring(index + 1);

					if (paramArg.IndexOf('|') != -1)
					{
						type = UrlNodeType.Choice;
					}
					else
					{
						switch(paramArg)
						{
							case "number":
								type = UrlNodeType.Number;
								break;
							case "string":
								type = UrlNodeType.String;
								break;
							default:
								throw new ArgumentException("token has invalid value '" + paramArg + "'. Expected 'int' or 'string'");
						}
					}
				}

				index = index == -1 ? token.Length : index;

				if (isOptional)
				{
					throw new NotImplementedException("Support for optional nodes not implemented yet");
				}
				else if (type == UrlNodeType.Choice)
				{
					return new RequiredChoiceNode(token.Substring(1, index - 1), paramArg);
				}
				else
				{
					return new RequiredNode(token.Substring(1, index - 1), type);
				}
			}
		}
	}
}