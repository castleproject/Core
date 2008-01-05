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
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using Descriptors;
	using Resources;
	using Routing;

	/// <summary>
	/// Pendent
	/// </summary>
	public class ControllerContext : IControllerContext
	{
		private string name;
		private string areaName;
		private string action;
		private string selectedViewName;
		private string viewFolder;
		private string[] layoutNames;
		private ControllerMetaDescriptor metaDescriptor;
		private IDictionary<string, object> customActionParameters = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
		private IDictionary propertyBag = new HybridDictionary(true);
		private IDictionary helpers = new HybridDictionary(true);
		private readonly IDictionary<string, IDynamicAction> dynamicActions = new Dictionary<string, IDynamicAction>();
		private readonly IDictionary<string, IResource> resources = new Dictionary<string, IResource>(StringComparer.InvariantCultureIgnoreCase);
		private RouteMatch routeMatch;

		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerContext"/> class.
		/// </summary>
		public ControllerContext()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerContext"/> class.
		/// </summary>
		/// <param name="name">The controller name.</param>
		/// <param name="action">The action name.</param>
		/// <param name="metaDescriptor">The meta descriptor.</param>
		public ControllerContext(string name, string action, ControllerMetaDescriptor metaDescriptor) : 
			this(name, string.Empty, action, metaDescriptor)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerContext"/> class.
		/// </summary>
		/// <param name="name">The controller name.</param>
		/// <param name="areaName">The area name.</param>
		/// <param name="action">The action name.</param>
		/// <param name="metaDescriptor">The meta descriptor.</param>
		public ControllerContext(string name, string areaName, string action, ControllerMetaDescriptor metaDescriptor)
		{
			this.name = name;
			this.areaName = areaName;
			this.action = action;
			this.metaDescriptor = metaDescriptor;
		}

		/// <summary>
		/// Gets or sets the custom action parameters.
		/// </summary>
		/// <value>The custom action parameters.</value>
		public IDictionary<string, object> CustomActionParameters
		{
			get { return customActionParameters; }
			set { customActionParameters = value; }
		}

		/// <summary>
		/// Gets the property bag, which is used
		/// to pass variables to the view.
		/// </summary>
		/// <value></value>
		public IDictionary PropertyBag
		{
			get { return propertyBag; }
			set { propertyBag = value; }
		}

		/// <summary>
		/// Gets a dictionary of name/helper instance
		/// </summary>
		/// <value>The helpers.</value>
		public IDictionary Helpers
		{
			get { return helpers; }
			set { helpers = value; }
		}

		/// <summary>
		/// Gets the controller's name.
		/// </summary>
		/// <value></value>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// Gets the controller's area name.
		/// </summary>
		/// <value></value>
		public string AreaName
		{
			get { return areaName; }
			set { areaName = value; }
		}

		/// <summary>
		/// Gets or set the layout being used.
		/// </summary>
		/// <value></value>
		public string[] LayoutNames
		{
			get { return layoutNames; }
			set { layoutNames = value; }
		}

		/// <summary>
		/// Gets the name of the action being processed.
		/// </summary>
		/// <value></value>
		public string Action
		{
			get { return action; }
			set { action = value; }
		}

		/// <summary>
		/// Gets or sets the view which will be rendered after this action executes.
		/// </summary>
		/// <value></value>
		public string SelectedViewName
		{
			get { return selectedViewName; }
			set { selectedViewName = value; }
		}

		/// <summary>
		/// Gets the view folder -- (areaname +
		/// controllername) or just controller name -- that this controller
		/// will use by default.
		/// </summary>
		/// <value></value>
		public string ViewFolder
		{
			get { return viewFolder; }
			set { viewFolder = value; }
		}

		/// <summary>
		/// Gets a dicitionary of name/<see cref="IResource"/>
		/// </summary>
		/// <value>The resources.</value>
		/// <remarks>It is supposed to be used by MonoRail infrastructure only</remarks>
		public IDictionary<string, IResource> Resources
		{
			get { return resources; }
		}

		/// <summary>
		/// Gets the dynamic actions.
		/// </summary>
		/// <value>The dynamic actions.</value>
		public IDictionary<string, IDynamicAction> DynamicActions
		{
			get { return dynamicActions; }
		}

		/// <summary>
		/// Gets or sets the controller descriptor.
		/// </summary>
		/// <value>The controller descriptor.</value>
		public ControllerMetaDescriptor ControllerDescriptor
		{
			get { return metaDescriptor; }
			set { metaDescriptor = value; }
		}

		/// <summary>
		/// Gets or sets the route match.
		/// </summary>
		/// <value>The route match.</value>
		public RouteMatch RouteMatch
		{
			get { return routeMatch; }
			set { routeMatch = value; }
		}
	}
}
