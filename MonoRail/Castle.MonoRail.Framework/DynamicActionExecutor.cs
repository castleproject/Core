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

namespace Castle.MonoRail.Framework
{
	using System;
	using Descriptors;

	/// <summary>
	/// Pendent
	/// </summary>
	public class DynamicActionExecutor : IExecutableAction
	{
		private readonly IDynamicAction action;

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicActionExecutor"/> class.
		/// </summary>
		/// <param name="action">The action.</param>
		public DynamicActionExecutor(IDynamicAction action)
		{
			this.action = action;
		}

		/// <summary>
		/// Gets a value indicating whether no filter should run before execute the action.
		/// </summary>
		/// <value><c>true</c> if they should be skipped; otherwise, <c>false</c>.</value>
		public bool ShouldSkipAllFilters
		{
			get { return false; }
		}

		/// <summary>
		/// Indicates that no rescues whatsoever should be applied to this action.
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		public bool ShouldSkipRescues
		{
			get { return false; }
		}

		/// <summary>
		/// Gets a rescue descriptor for the exception type.
		/// </summary>
		/// <param name="exceptionType">Type of the exception.</param>
		/// <returns></returns>
		public RescueDescriptor GetRescueFor(Type exceptionType)
		{
			return null;
		}

		/// <summary>
		/// Gets the layout override.
		/// </summary>
		/// <value>The layout override.</value>
		public string[] LayoutOverride
		{
			get { return null; }
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="filterType">Type of the filter.</param>
		/// <returns></returns>
		public bool ShouldSkipFilter(Type filterType)
		{
			return false;
		}

		/// <summary>
		/// Gets the http method that the action requires before being executed.
		/// </summary>
		/// <value>The accessible through verb.</value>
		public Verb AccessibleThroughVerb
		{
			get { return Verb.Undefined; }
		}

		/// <summary>
		/// Gets the i18n related resource descriptors.
		/// </summary>
		/// <value>The resources.</value>
		public ResourceDescriptor[] Resources
		{
			get { return new ResourceDescriptor[0]; }
		}

		/// <summary>
		/// Gets the cache policy configurer.
		/// </summary>
		/// <value>The cache policy configurer.</value>
		public ICachePolicyConfigurer CachePolicyConfigurer
		{
			get { return null; }
		}

		/// <summary>
		/// Executes the action this instance represents.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="context">The context.</param>
		public object Execute(IEngineContext engineContext, Controller controller, IControllerContext context)
		{
			return action.Execute(engineContext, controller, context);
		}
	}
}
