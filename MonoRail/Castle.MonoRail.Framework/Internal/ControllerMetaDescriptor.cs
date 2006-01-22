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

	public abstract class BaseMetaDescriptor
	{
		private LayoutAttribute layout;
		private IList rescues;
		private IList resources = new ArrayList();

		public LayoutAttribute Layout
		{
			get { return layout; }
			set { layout = value; }
		}

		public IList Rescues
		{
			get { return rescues; }
			set { rescues = value; }
		}

		public IList Resources
		{
			get { return resources; }
		}
	}

	public class ActionMetaDescriptor : BaseMetaDescriptor
	{
		private SkipRescueAttribute skipRescue;
		private IList skipFilters = new ArrayList();

		public SkipRescueAttribute SkipRescue
		{
			get { return skipRescue; }
			set { skipRescue = value; }
		}

		public IList SkipFilters
		{
			get { return skipFilters; }
		}
	}

	/// <summary>
	/// Holds all monorail attributes a controller might 
	/// expose, so the attributes are collected only once.
	/// This is a huge performance boost. 
	/// </summary>
	[Serializable]
	public class ControllerMetaDescriptor : BaseMetaDescriptor
	{
		private DefaultActionAttribute defaultAction;
		private IList scaffoldings = new ArrayList();
		private IList helpers = new ArrayList();
		private IList filters = new ArrayList();
		private IList actionProviders = new ArrayList();
		private Hashtable actionMetaDescriptors = new Hashtable();
		private IDictionary _actions = new HybridDictionary(true);
		private Type _controllerType;

		public ControllerMetaDescriptor( Type controllerType )
		{
			_controllerType = controllerType;
			
			CollectActions(_controllerType);
		}

		public ActionMetaDescriptor GetAction( MethodInfo actionMethod )
		{
			ActionMetaDescriptor desc = (ActionMetaDescriptor) actionMetaDescriptors[actionMethod];

			if (desc == null)
			{
				desc = new ActionMetaDescriptor();
				actionMetaDescriptors[actionMethod] = desc;
			}

			return desc;
		}

		private void CollectActions( Type controllerType )
		{
			// HACK: GetRealControllerType is a workaround for DYNPROXY-14 bug
			// see: http://support.castleproject.org/jira/browse/DYNPROXY-14
			controllerType = GetRealControllerType(controllerType);

			MethodInfo[] methods = controllerType.GetMethods( BindingFlags.Public | BindingFlags.Instance );

			foreach (MethodInfo method in methods)
			{
				Type declaringType = method.DeclaringType;

				if (declaringType == typeof(Object) || declaringType == typeof(Controller) || declaringType == typeof(SmartDispatcherController))
				{
					continue;
				}

				if (_actions.Contains(method.Name))
				{
					ArrayList list = _actions[method.Name] as ArrayList;

					if (list == null)
					{
						list = new ArrayList();
						list.Add(_actions[method.Name]);

						_actions[method.Name] = list;
					}

					list.Add(method);
				}
				else
				{
					_actions[method.Name] = method;
				}
			}
		}

		private Type GetRealControllerType(Type controllerType)
		{
			Type prev = controllerType;

			// try to get the first type which is not a proxy
			// TODO: skip it in case of mixins
			while (controllerType.Assembly.FullName.StartsWith("DynamicAssemblyProxyGen"))
			{
				controllerType = controllerType.BaseType;
				if (controllerType == typeof(SmartDispatcherController) || controllerType == typeof(Controller))
				{
					// oops, it's a pure-proxy controller. just let it go.
					controllerType = prev;
					break;
				}
			}
			return controllerType;
		}

		public DefaultActionAttribute DefaultAction
		{
			get { return defaultAction; }
			set { defaultAction = value; }
		}

		public IList Helpers
		{
			get { return helpers; }
		}

		public IDictionary Actions
		{
			get { return _actions; }
		}

		public IList Filters
		{
			get { return filters; }
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
