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

namespace Castle.MonoRail.Framework.Descriptors
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Reflection;

	/// <summary>
	/// Holds all meta information a controller might 
	/// expose, so the attributes are collected only once.
	/// This approach translates into a huge performance boost. 
	/// </summary>
	[Serializable]
	public class ControllerMetaDescriptor : BaseMetaDescriptor
	{
		private ControllerDescriptor controllerDescriptor;
		private DefaultActionAttribute defaultAction;
		private HelperDescriptor[] helpers = new HelperDescriptor[0];
		private FilterDescriptor[] filters = new FilterDescriptor[0];
		private TransformFilterDescriptor[] transformFilters = new TransformFilterDescriptor[0];
		private Dictionary<object, ActionMetaDescriptor> actionMetaDescriptors = new Dictionary<object, ActionMetaDescriptor>();
		private IList<ScaffoldingAttribute> scaffoldings = new List<ScaffoldingAttribute>();
		private IList<Type> actionProviders = new List<Type>();
		private IList<MethodInfo> ajaxActions = new List<MethodInfo>();
		private IDictionary actions = new HybridDictionary(true);

		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerMetaDescriptor"/> class.
		/// </summary>
		public ControllerMetaDescriptor()
		{
		}

		/// <summary>
		/// Gets an action descriptor with information about an action.
		/// </summary>
		/// <param name="actionMethod">The action method.</param>
		/// <returns></returns>
		public ActionMetaDescriptor GetAction(object actionMethod)
		{
			if (actionMethod == null) throw new ArgumentNullException("actionMethod");

			ActionMetaDescriptor desc;

			actionMetaDescriptors.TryGetValue(actionMethod, out desc);

			return desc;
		}

		/// <summary>
		/// Adds the action.
		/// </summary>
		/// <param name="action">The action.</param>
		/// <param name="metaDescriptor">The meta descriptor.</param>
		public void AddAction(object action, ActionMetaDescriptor metaDescriptor)
		{
			if (action == null) throw new ArgumentNullException("action");
			if (metaDescriptor == null) throw new ArgumentNullException("metaDescriptor");

			actionMetaDescriptors[action] = metaDescriptor;
		}

		/// <summary>
		/// Gets or sets the controller descriptor.
		/// </summary>
		/// <value>The controller descriptor.</value>
		public ControllerDescriptor ControllerDescriptor
		{
			get { return controllerDescriptor; }
			set { controllerDescriptor = value; }
		}

		/// <summary>
		/// Gets the actions.
		/// </summary>
		/// <value>The actions.</value>
		public IDictionary Actions
		{
			get { return actions; }
		}

		/// <summary>
		/// Gets the action descriptors dictionary. 
		/// They key is an action (might be a method or something else).
		/// </summary>
		/// <value>The action descriptors.</value>
		public Dictionary<object, ActionMetaDescriptor> ActionDescriptors
		{
			get { return actionMetaDescriptors; }
		}

		/// <summary>
		/// Gets the ajax actions.
		/// </summary>
		/// <value>The ajax actions.</value>
		public IList<MethodInfo> AjaxActions
		{
			get { return ajaxActions; }
		}

		/// <summary>
		/// Gets or sets the default action.
		/// </summary>
		/// <value>The default action.</value>
		public DefaultActionAttribute DefaultAction
		{
			get { return defaultAction; }
			set { defaultAction = value; }
		}

		/// <summary>
		/// Gets or sets the helpers.
		/// </summary>
		/// <value>The helpers.</value>
		public HelperDescriptor[] Helpers
		{
			get { return helpers; }
			set { helpers = value; }
		}

		/// <summary>
		/// Gets or sets the filters.
		/// </summary>
		/// <value>The filters.</value>
		public FilterDescriptor[] Filters
		{
			get { return filters; }
			set { filters = value; }
		}

		/// <summary>
		/// Gets the scaffoldings.
		/// </summary>
		/// <value>The scaffoldings.</value>
		public IList<ScaffoldingAttribute> Scaffoldings
		{
			get { return scaffoldings; }
		}

		/// <summary>
		/// Gets the action providers.
		/// </summary>
		/// <value>The action providers.</value>
		public IList<Type> ActionProviders
		{
			get { return actionProviders; }
		}

		/// <summary>
		/// Gets or sets the transform filters.
		/// </summary>
		/// <value>The transform filters.</value>
		public TransformFilterDescriptor[] TransformFilters
		{
			get { return transformFilters; }
			set { transformFilters = value; }
		}
	}
}
