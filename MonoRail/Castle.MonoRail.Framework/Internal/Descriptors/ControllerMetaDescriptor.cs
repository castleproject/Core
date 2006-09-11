// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Internal
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Reflection;

	/// <summary>
	/// Holds all meta information a controller might 
	/// expose, so the attributes are collected only once.
	/// This is a huge performance boost. 
	/// </summary>
	[Serializable]
	public class ControllerMetaDescriptor : BaseMetaDescriptor
	{
		private DefaultActionAttribute defaultAction;
		private IList scaffoldings = new ArrayList();
		private HelperDescriptor[] helpers;
		private IList actionProviders = new ArrayList();
		private IList ajaxActions = new ArrayList();
		private Hashtable actionMetaDescriptors = new Hashtable();
		private IDictionary actions = new HybridDictionary(true);
		private FilterDescriptor[] filters;

		public ControllerMetaDescriptor()
		{
		}

		public ActionMetaDescriptor GetAction(MethodInfo actionMethod)
		{
			ActionMetaDescriptor desc = (ActionMetaDescriptor) actionMetaDescriptors[actionMethod];

			if (desc == null)
			{
				desc = new ActionMetaDescriptor();
				actionMetaDescriptors[actionMethod] = desc;
			}

			return desc;
		}

		public IDictionary Actions
		{
			get { return actions; }
		}

		public IList AjaxActions
		{
			get { return ajaxActions; }
		}

		public DefaultActionAttribute DefaultAction
		{
			get { return defaultAction; }
			set { defaultAction = value; }
		}

		public HelperDescriptor[] Helpers
		{
			get { return helpers; }
			set { helpers = value; }
		}

		public FilterDescriptor[] Filters
		{
			get { return filters; }
			set { filters = value; }
		}

		public IList Scaffoldings
		{
			get { return scaffoldings; }
		}

		public IList ActionProviders
		{
			get { return actionProviders; }
		}
	}
}