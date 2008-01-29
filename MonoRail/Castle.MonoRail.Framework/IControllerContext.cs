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
	using System.Collections;
	using System.Collections.Generic;
	using Castle.MonoRail.Framework.Resources;
	using Descriptors;
	using Routing;

	/// <summary>
	/// Pendent
	/// </summary>
	public interface IControllerContext
	{
		/// <summary>
		/// Gets or sets the custom action parameters.
		/// </summary>
		/// <value>The custom action parameters.</value>
		IDictionary<string, object> CustomActionParameters { get; set; }

		/// <summary>
		/// Gets the property bag, which is used
		/// to pass variables to the view.
		/// </summary>
		IDictionary PropertyBag { get; set; }

		/// <summary>
		/// Gets a dictionary of name/helper instance
		/// </summary>
		/// <value>The helpers.</value>
		HelperDictionary Helpers { get; set; }

		/// <summary>
		/// Gets the controller's name.
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Gets the controller's area name.
		/// </summary>
		string AreaName { get; set; }

		/// <summary>
		/// Gets or set the layout being used.
		/// </summary>
		string[] LayoutNames { get; set; }

		/// <summary>
		/// Gets the name of the action being processed.
		/// </summary>
		string Action { get; set; }

		/// <summary>
		/// Gets or sets the view which will be rendered after this action executes.
		/// </summary>
		string SelectedViewName { get; set; }

		/// <summary>
		/// Gets the view folder -- (areaname + 
		/// controllername) or just controller name -- that this controller 
		/// will use by default.
		/// </summary>
		string ViewFolder { get; set; }

		/// <summary>
		/// Gets a dicitionary of name/<see cref="IResource"/>
		/// </summary>
		/// <remarks>It is supposed to be used by MonoRail infrastructure only</remarks>
		/// <value>The resources.</value>
		IDictionary<string, IResource> Resources { get; }

		/// <summary>
		/// Gets the dynamic actions.
		/// </summary>
		/// <value>The dynamic actions.</value>
		IDictionary<string, IDynamicAction> DynamicActions { get; }

		/// <summary>
		/// Gets or sets the controller descriptor.
		/// </summary>
		/// <value>The controller descriptor.</value>
		ControllerMetaDescriptor ControllerDescriptor { get; set; }

		/// <summary>
		/// Gets or sets the route match.
		/// </summary>
		/// <value>The route match.</value>
		RouteMatch RouteMatch { get; set; }
	}
}
