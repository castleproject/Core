// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
	using System.Collections.Generic;

	/// <summary>
	/// Abstracts the strategy for getting the current <see cref="IEngineContext"/>.
	/// The default strategy uses <see cref="MonoRailHttpHandlerFactory.CurrentEngineContext"/>
	/// </summary>
	public class EngineContextLocator : IEngineContextLocator
	{
		private static readonly EngineContextLocator instance = new EngineContextLocator();

		private readonly IList<IEngineContextLocator> locatorStrategies = new List<IEngineContextLocator>();

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceProviderLocator"/> class.
		/// </summary>
		public EngineContextLocator()
		{
			AddLocatorStrategy(new DefaultEngineContextLocatorStrategy());
		}

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static EngineContextLocator Instance
		{
			get { return instance; }
		}

		/// <summary>
		/// Locates the engine context using the registered strategies.
		/// </summary>
		/// <returns>The current <see cref="IEngineContext"/>, or <c>null</c> if none could be found.</returns>
		public IEngineContext LocateCurrentContext()
		{
			foreach (var strategy in locatorStrategies)
			{
				var engineContext = strategy.LocateCurrentContext();

				if (engineContext != null)
				{
					return engineContext;
				}
			}
			return null;
		}

		/// <summary>
		/// Adds a locator strategy.
		/// </summary>
		/// <param name="strategy">The strategy.</param>
		public void AddLocatorStrategy(IEngineContextLocator strategy)
		{
			locatorStrategies.Add(strategy);
		}

		/// <summary>
		/// Default strategy to access a service provider
		/// </summary>
		class DefaultEngineContextLocatorStrategy : IEngineContextLocator
		{
			/// <summary>
			/// Locates the provider using <see cref="MonoRailHttpHandlerFactory.CurrentEngineContext"/>
			/// </summary>
			/// <returns></returns>
			public IEngineContext LocateCurrentContext()
			{
				return MonoRailHttpHandlerFactory.CurrentEngineContext;
			}
		}
	}
}