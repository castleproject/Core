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
	using System.Reflection;
	using System.Threading;

#if dotNet2
	using ListOfRescue = System.Collections.Generic.List<RescueItem>;
#else
	using ListOfRescue = System.Collections.ArrayList;
#endif

	public class ControllerDescriptorBuilder
	{
		private ReaderWriterLock locker = new ReaderWriterLock();
		private Hashtable descriptorRepository = new Hashtable();

		/// <summary>
		/// Builds the <see cref="ControllerMetaDescriptor"/>.
		/// </summary>
		/// <param name="controller">Controller.</param>
		/// <returns></returns>
		public ControllerMetaDescriptor BuildDescriptor(Controller controller)
		{
			Type controllerType = controller.GetType();

			ControllerMetaDescriptor desc;

			locker.AcquireReaderLock(-1);

			try
			{
				desc = (ControllerMetaDescriptor)
					descriptorRepository[controllerType];

				if (desc != null)
				{
					return desc;
				}

				LockCookie lc = locker.UpgradeToWriterLock(-1);

				try
				{
					desc = BuildDescriptor(controllerType);

					descriptorRepository[controllerType] = desc;
				}
				finally
				{
					locker.DowngradeFromWriterLock(ref lc);
				}
			}
			finally
			{
				locker.ReleaseReaderLock();
			}

			return desc;
		}

		private ControllerMetaDescriptor BuildDescriptor(Type controllerType)
		{
			ControllerMetaDescriptor descriptor = new ControllerMetaDescriptor(controllerType);

			CollectClassLevelAttributes(controllerType, descriptor);

			foreach(object action in descriptor.Actions.Values)
			{
				if (action is IList)
				{
					foreach(MethodInfo overloadedAction in (action as IList))
					{
						CollectActionAttributes(overloadedAction, descriptor);
					}

					continue;
				}

				CollectActionAttributes(action as MethodInfo, descriptor);
			}

			return descriptor;
		}

		private void CollectActionAttributes(MethodInfo method, ControllerMetaDescriptor descriptor)
		{
			ActionMetaDescriptor actionDescriptor = descriptor.GetAction(method);

			object[] attributes;

			CollectResources(actionDescriptor, method);

			attributes = method.GetCustomAttributes(typeof(ISkipFilterAttribute), true);
			foreach (ISkipFilterAttribute attr in attributes)
			{
				actionDescriptor.SkipFilters.Add(attr);
			}

			CollectRescues(actionDescriptor, method);

			attributes = method.GetCustomAttributes(typeof(AccessibleThroughAttribute), true);
			if (attributes.Length != 0)
			{
				actionDescriptor.AccessibleThrough = (AccessibleThroughAttribute) attributes[0];
			}

			attributes = method.GetCustomAttributes(typeof(SkipRescueAttribute), true);
			if (attributes.Length != 0)
			{
				actionDescriptor.SkipRescue = (SkipRescueAttribute) attributes[0];
			}

			attributes = method.GetCustomAttributes(typeof(ILayoutAttribute), true);
			if (attributes.Length > 1)
			{
				throw new RailsException("Only one attribute implementing ILayoutAttribute can be specified per controller.");
			}
			else if (attributes.Length != 0)
			{
				actionDescriptor.Layout = (ILayoutAttribute) attributes[0];
			}
		}

		private void CollectClassLevelAttributes(Type controllerType, ControllerMetaDescriptor descriptor)
		{
			object[] attributes;
			
			attributes = controllerType.GetCustomAttributes(typeof(DefaultActionAttribute), true);
			if (attributes.Length != 0)
			{
				descriptor.DefaultAction = (DefaultActionAttribute) attributes[0];
			}

			CollectHelpers(descriptor, controllerType);
			CollectResources(descriptor, controllerType);
			CollectFilters(descriptor, controllerType);
			CollectLayout(descriptor, controllerType);
			CollectRescues(descriptor, controllerType);

			attributes = controllerType.GetCustomAttributes(typeof(ScaffoldingAttribute), false);
			if (attributes.Length != 0)
			{
				foreach(ScaffoldingAttribute scaffolding in attributes)
				{
					descriptor.Scaffoldings.Add(scaffolding);
				}
			}

			attributes = controllerType.GetCustomAttributes(typeof(DynamicActionProviderAttribute), true);
			if (attributes.Length != 0)
			{
				foreach(DynamicActionProviderAttribute attr in attributes)
				{
					descriptor.ActionProviders.Add(attr.ProviderType);
				}
			}
		}

		private void CollectHelpers(ControllerMetaDescriptor desc, Type mi)
		{
			object[] attributes = mi.GetCustomAttributes(typeof(IHelpersAttribute), true);
			if (attributes.Length != 0)
			{
				foreach (IHelpersAttribute attr in attributes)
				{
					HelperItem[] helpers = attr.GetHelpers();
					if (helpers != null)
						foreach (HelperItem helper in helpers)
							desc.Helpers.Add(helper);
				}
			}
		}

		private void CollectFilters(ControllerMetaDescriptor desc, Type mi)
		{
			object[] attributes = mi.GetCustomAttributes(typeof(IFiltersAttribute), true);

			ArrayList filters = new ArrayList();
			foreach (IFiltersAttribute filter in attributes)
			{
				FilterItem[] filterItems = filter.GetFilters();
				if (filterItems != null)
					foreach (FilterItem filterItem in filterItems)
						filters.Add(new FilterDescriptor(filter, filterItem));
			}

			FilterDescriptor[] filterArray =
				(FilterDescriptor[]) filters.ToArray(typeof(FilterDescriptor));

			Array.Sort(filterArray, FilterDescriptorComparer.Instance);
			
			desc.Filters = filterArray;
		}

		private void CollectRescues(BaseMetaDescriptor desc, MemberInfo mi)
		{
			object[] attributes = mi.GetCustomAttributes(typeof(IRescuesAttribute), true);
			foreach (IRescuesAttribute attr in attributes)
			{
				RescueItem[] rescues = attr.GetRescues();
				if (rescues != null)
					foreach (RescueItem rescue in rescues)
					{
						if (desc.Rescues == null)
							desc.Rescues = new ListOfRescue();
						desc.Rescues.Add(rescue);
					}
			}
		}

		private void CollectResources(BaseMetaDescriptor desc, MemberInfo mi)
		{
			object[] attributes = mi.GetCustomAttributes(typeof(IResourcesAttribute), true);
			if (attributes.Length != 0)
			{
				foreach (IResourcesAttribute attr in attributes)
				{
					ResourceItem[] resources = attr.GetResources();
					if (resources != null)
						foreach (ResourceItem res in resources)
							desc.Resources.Add(res);
				}
			}
		}

		private void CollectLayout(BaseMetaDescriptor desc, MemberInfo mi)
		{
			object[] attributes = mi.GetCustomAttributes(typeof(ILayoutAttribute), true);
			if (attributes.Length > 1)
			{
				throw new RailsException("Only one attribute implementing ILayoutAttribute can be specified: " + mi.ToString());
			}
			else if (attributes.Length != 0)
			{
				desc.Layout = (ILayoutAttribute) attributes[0];
			}
		}

		class FilterDescriptorComparer : IComparer
		{
			private static readonly FilterDescriptorComparer instance = new FilterDescriptorComparer();

			private FilterDescriptorComparer()
			{
			}

			public static FilterDescriptorComparer Instance
			{
				get { return instance; }
			}

			public int Compare(object x, object y)
			{
				return ((FilterDescriptor) x).ExecutionOrder - ((FilterDescriptor) y).ExecutionOrder;
			}
		}
	}
}