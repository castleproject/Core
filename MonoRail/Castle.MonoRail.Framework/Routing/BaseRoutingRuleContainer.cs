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
	using System.Threading;
	using Core;

	/// <summary>
	/// Pendent
	/// </summary>
	public class RoutingRuleContainer : IRoutingRuleContainer
	{
		private readonly List<DecoratedRule> rules = new List<DecoratedRule>();
		private readonly Dictionary<string, IRoutingRule> name2Rule = new Dictionary<string, IRoutingRule>();
		private readonly ReaderWriterLock locker = new ReaderWriterLock();

		/// <summary>
		/// Adds the specified rule.
		/// </summary>
		/// <param name="rule">The rule.</param>
		public void Add(IRoutingRule rule)
		{
			Add(new DecoratedRule(rule));
		}

		/// <summary>
		/// Adds the specified rule.
		/// </summary>
		/// <param name="rule">The rule.</param>
		public void AddFirst(IRoutingRule rule)
		{
			AddFirst(new DecoratedRule(rule));
		}

		/// <summary>
		/// Pendent
		/// </summary>
		public void Add(IRoutingRule rule, RouteAction action)
		{
			Add(new DecoratedRule(rule, action));
		}

		/// <summary>
		/// Pendent
		/// </summary>
		public void AddFirst(IRoutingRule rule, RouteAction action)
		{
			AddFirst(new DecoratedRule(rule, action));
		}

		/// <summary>
		/// Creates the URL.
		/// </summary>
		/// <param name="routeName">Name of the route.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		public string CreateUrl(string routeName, IDictionary parameters)
		{
			// lock for reading

			IRoutingRule rule;

			if (!name2Rule.TryGetValue(routeName, out rule))
			{
				throw new MonoRailException("Could not find named route: " + routeName);
			}

			return rule.CreateUrl(parameters);
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		public string CreateUrl(IDictionary parameters)
		{
			foreach(IRoutingRule rule in rules)
			{
				string url = rule.CreateUrl(parameters);

				if (url != null)
				{
					return url;
				}
			}

			return null;
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		public string CreateUrl(object parameters)
		{
			return CreateUrl(new ReflectionBasedDictionaryAdapter(parameters));
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="routeName">Name of the route.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		public string CreateUrl(string routeName, object parameters)
		{
			return CreateUrl(routeName, new ReflectionBasedDictionaryAdapter(parameters));
		}

		/// <summary>
		/// Finds the match.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public RouteMatch FindMatch(string url, IRouteContext context)
		{
			// TODO: lock for reading

			int winnerPoints = 0;
			RouteMatch winner = null;
			DecoratedRule winnerule = null;

			foreach(DecoratedRule rule in rules)
			{
				RouteMatch match = new RouteMatch();

				int points = rule.Matches(url, context, match);

				if (points != 0 && points > winnerPoints)
				{
					winnerPoints = points;
					winner = match;
					winnerule = rule;
				}
			}

			if (winner != null && winnerule.SelectionAction != null)
			{
				winnerule.SelectionAction(context, winner);
			}

			return winner;
		}

		/// <summary>
		/// Gets a value indicating whether this container is empty.
		/// </summary>
		/// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
		public bool IsEmpty
		{
			get { return rules.Count == 0; }
		}

		private void Add(DecoratedRule rule)
		{
			// Lock for writing
			rules.Add(rule);

			RegisterNamedRoute(rule);
		}

		private void AddFirst(DecoratedRule rule)
		{
			// Lock for writing
			rules.Insert(0, rule);

			RegisterNamedRoute(rule);
		}

		private void RegisterNamedRoute(DecoratedRule rule)
		{
			// For really fast access
			if (rule.RouteName != null)
			{
				if (name2Rule.ContainsKey(rule.RouteName))
				{
					throw new InvalidOperationException("Attempt to register route with duplicated name: " + rule.RouteName);
				}

				name2Rule[rule.RouteName] = rule.inner;
			}
		}

		/// <summary>
		/// Determines whether the given url should be excluded from routing.
		/// </summary>
		/// <param name="appRelativeUrl">The app relative url</param>
		/// <returns><c>true</c> if the given url should be excluded from routing; 
		/// otherwise, <c>false</c></returns>
		/// <remarks>
		/// The <paramref name="appRelativeUrl"/> is expected to be relative to the
		/// application directory. 
		/// </remarks>
		protected virtual bool IsExcludedUrl(string appRelativeUrl)
		{
			// do not route MonoRail resource file requests
			return appRelativeUrl.ToLowerInvariant().StartsWith("/monorail/files");
		}

		[DebuggerDisplay("DecoratedRule {inner}")]
		class DecoratedRule : IRoutingRule
		{
			internal readonly IRoutingRule inner;
			private readonly RouteAction selectionAction;

			public DecoratedRule(IRoutingRule inner)
			{
				this.inner = inner;
			}

			public DecoratedRule(IRoutingRule inner, RouteAction selectionAction) : this(inner)
			{
				this.selectionAction = selectionAction;
			}

			public string CreateUrl(IDictionary parameters)
			{
				return inner.CreateUrl(parameters);
			}

			public int Matches(string url, IRouteContext context, RouteMatch match)
			{
				return inner.Matches(url, context, match);
			}

			public string RouteName
			{
				get { return inner.RouteName; }
			}

			public RouteAction SelectionAction
			{
				get { return selectionAction; }
			}
		}
	}
}
