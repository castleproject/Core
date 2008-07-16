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

namespace Castle.MonoRail.Views.AspView.Tests.Stubs
{
	using System.Collections;
	using System.Collections.Generic;
	using Framework;
	using Framework.Descriptors;
	using Framework.Resources;

	public class ControllerContextStub : IControllerContext
	{
		string action = "Action";
		string areaName = "Area";
		ControllerMetaDescriptor controllerDescriptor = new ControllerMetaDescriptor();
		IDictionary<string, object> customActionParameters = new Dictionary<string, object>();
		IDictionary<string, IDynamicAction> dynamicActions = new Dictionary<string, IDynamicAction>();
		HelperDictionary helpers = new HelperDictionary();
		string[] layoutNames = new string[1] { "Layout" };
		string name = "Stub";
		IDictionary propertyBag = new Hashtable();
		IDictionary<string, IResource> resources = new Dictionary<string, IResource>();
		Framework.Routing.RouteMatch routeMatch;
		string selectedViewName = "Action";
		string viewFolder = "Area/Stub";
		AsyncInvocationInformation asyncInvocationInformation = new AsyncInvocationInformation();

		#region IControllerContext Members

		public string Action
		{
			get { return action; }
			set { action = value; }
		}

		public string AreaName
		{
			get { return areaName; }
			set { areaName = value; }
		}

		public ControllerMetaDescriptor ControllerDescriptor
		{
			get { return controllerDescriptor; }
			set { controllerDescriptor = value; }
		}
		
		public IDictionary<string, object> CustomActionParameters
		{
			get { return customActionParameters; }
			set { customActionParameters = value; }
		}

		public IDictionary<string, IDynamicAction> DynamicActions
		{
			get { return dynamicActions; }
		}

		public HelperDictionary Helpers
		{
			get { return helpers; }
			set { helpers = value; }
		}

		public string[] LayoutNames
		{
			get { return layoutNames; }
			set { layoutNames = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public IDictionary PropertyBag
		{
			get { return propertyBag; }
			set { propertyBag = value; }
		}

		public IDictionary<string, IResource> Resources
		{
			get { return resources; }
		}

		public Framework.Routing.RouteMatch RouteMatch
		{
			get { return routeMatch; }
			set { routeMatch = value; }
		}

		public AsyncInvocationInformation Async
		{
			get { return asyncInvocationInformation; }
			set { asyncInvocationInformation = value; }
		}

		public string SelectedViewName
		{
			get { return selectedViewName; }
			set { selectedViewName = value; }
		}

		public string ViewFolder
		{
			get { return viewFolder; }
			set { viewFolder = value; }
		}

		#endregion
	}
}
