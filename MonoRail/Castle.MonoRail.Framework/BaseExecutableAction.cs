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
	using System;
	using Descriptors;
	using Internal;

	/// <summary>
	/// 
	/// </summary>
	public abstract class BaseExecutableAction : IExecutableAction
	{
		private readonly ActionMetaDescriptor actionMetaDescriptor;

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseExecutableAction"/> class.
		/// </summary>
		/// <param name="actionMetaDescriptor">The action meta descriptor.</param>
		protected BaseExecutableAction(ActionMetaDescriptor actionMetaDescriptor)
		{
			if (actionMetaDescriptor == null)
			{
				throw new ArgumentNullException("actionMetaDescriptor");
			}

			this.actionMetaDescriptor = actionMetaDescriptor;
		}

		/// <summary>
		/// Gets the action meta descriptor.
		/// </summary>
		/// <value>The action meta descriptor.</value>
		public ActionMetaDescriptor ActionMetaDescriptor
		{
			get { return actionMetaDescriptor; }
		}

		#region IExecutableAction

		/// <summary>
		/// Indicates that no rescues whatsoever should be applied to this action.
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		public bool ShouldSkipRescues
		{
			get { return actionMetaDescriptor.SkipRescue != null; }
		}

		/// <summary>
		/// Gets a value indicating whether no filter should run before execute the action.
		/// </summary>
		/// <value><c>true</c> if they should be skipped; otherwise, <c>false</c>.</value>
		public bool ShouldSkipAllFilters
		{
			get 
			{
				foreach(SkipFilterAttribute skip in actionMetaDescriptor.SkipFilters)
				{
					if (skip.BlanketSkip)
					{
						return true;
					}
				}

				return false;
			}
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="filterType">Type of the filter.</param>
		/// <returns></returns>
		public bool ShouldSkipFilter(Type filterType)
		{
			foreach(SkipFilterAttribute skip in actionMetaDescriptor.SkipFilters)
			{
				if (skip.FilterType == filterType)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Gets the layout override.
		/// </summary>
		/// <value>The layout override.</value>
		public string[] LayoutOverride
		{
			get
			{
				if (actionMetaDescriptor.Layout == null)
				{
					return null;
				}
				return actionMetaDescriptor.Layout.LayoutNames;
			}
		}

		/// <summary>
		/// Gets the http method that the action requires before being executed.
		/// </summary>
		/// <value>The accessible through verb.</value>
		public Verb AccessibleThroughVerb
		{
			get
			{
				if (actionMetaDescriptor.AccessibleThrough == null)
				{
					return Verb.Undefined;
				}

				return actionMetaDescriptor.AccessibleThrough.Verb;
			}
		}

		/// <summary>
		/// Gets a rescue descriptor for the exception type.
		/// </summary>
		/// <param name="exceptionType">Type of the exception.</param>
		/// <returns></returns>
		public RescueDescriptor GetRescueFor(Type exceptionType)
		{
			return RescueUtils.SelectBest(actionMetaDescriptor.Rescues, exceptionType);
		}

		/// <summary>
		/// Gets the i18n related resource descriptors.
		/// </summary>
		/// <value>The resources.</value>
		public ResourceDescriptor[] Resources
		{
			get { return actionMetaDescriptor.Resources; }
		}

		/// <summary>
		/// Gets the return binder descriptor.
		/// </summary>
		/// <value>The return binder descriptor.</value>
		public ReturnBinderDescriptor ReturnBinderDescriptor
		{
			get { return actionMetaDescriptor.ReturnDescriptor; }
		}

		/// <summary>
		/// Gets the cache policy configurer.
		/// </summary>
		/// <value>The cache policy configurer.</value>
		public ICachePolicyConfigurer CachePolicyConfigurer
		{
			get { return actionMetaDescriptor.CacheConfigurer; }
		}

		/// <summary>
		/// Executes the action this instance represents.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public abstract object Execute(IEngineContext engineContext, Controller controller, IControllerContext context);

		#endregion
	}
}
