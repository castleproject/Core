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

	/// <summary>
	/// Constructs and caches all collected information
	/// about a <see cref="Controller"/> and its actions.
	/// <seealso cref="ControllerMetaDescriptor"/>
	/// </summary>
	public class ControllerDescriptorProvider : IControllerDescriptorProvider
	{
		private ReaderWriterLock locker = new ReaderWriterLock();
		private Hashtable descriptorRepository = new Hashtable();
		private IHelperDescriptorProvider helperDescriptorProvider;
		private IFilterDescriptorProvider filterDescriptorProvider;
		private ILayoutDescriptorProvider layoutDescriptorProvider;
		private IRescueDescriptorProvider rescueDescriptorProvider;
		private IResourceDescriptorProvider resourceDescriptorProvider;

		#region IControllerDescriptorProvider implementation

		public void Init(IServiceProvider serviceProvider)
		{
			helperDescriptorProvider = (IHelperDescriptorProvider) 
				serviceProvider.GetService(typeof(IHelperDescriptorProvider));

			filterDescriptorProvider = (IFilterDescriptorProvider)
				serviceProvider.GetService(typeof(IFilterDescriptorProvider));

			layoutDescriptorProvider = (ILayoutDescriptorProvider)
				serviceProvider.GetService(typeof(ILayoutDescriptorProvider));

			rescueDescriptorProvider = (IRescueDescriptorProvider)
				serviceProvider.GetService(typeof(IRescueDescriptorProvider));
		
			resourceDescriptorProvider = (IResourceDescriptorProvider)
				serviceProvider.GetService(typeof(IResourceDescriptorProvider));
		}

		/// <summary>
		/// Constructs and populates a <see cref="ControllerMetaDescriptor"/>.
		/// </summary>
		/// <remarks>
		/// This implementation is also responsible for caching 
		/// constructed meta descriptors.
		/// </remarks>
		public ControllerMetaDescriptor BuildDescriptor(Controller controller)
		{
			Type controllerType = controller.GetType();

			ControllerMetaDescriptor desc;

			locker.AcquireReaderLock(-1);

			try
			{
				desc = (ControllerMetaDescriptor) descriptorRepository[controllerType];

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

		#endregion

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

		#region Action data

		private void CollectActionAttributes(MethodInfo method, ControllerMetaDescriptor descriptor)
		{
			ActionMetaDescriptor actionDescriptor = descriptor.GetAction(method);

			CollectResources(actionDescriptor, method);
			CollectSkipFilter(actionDescriptor, method);
			CollectRescues(actionDescriptor, method);
			CollectAccessibleThrough(actionDescriptor, method);
			CollectSkipRescue(actionDescriptor, method);
			CollectLayout(actionDescriptor, method);
		}

		private void CollectSkipRescue(ActionMetaDescriptor actionDescriptor, MethodInfo method)
		{
			object[] attributes = method.GetCustomAttributes(typeof(SkipRescueAttribute), true);
			
			if (attributes.Length != 0)
			{
				actionDescriptor.SkipRescue = (SkipRescueAttribute) attributes[0];
			}
		}

		private void CollectAccessibleThrough(ActionMetaDescriptor actionDescriptor, MethodInfo method)
		{
			object[] attributes = method.GetCustomAttributes(typeof(AccessibleThroughAttribute), true);
			
			if (attributes.Length != 0)
			{
				actionDescriptor.AccessibleThrough = (AccessibleThroughAttribute) attributes[0];
			}
		}

		private void CollectSkipFilter(ActionMetaDescriptor actionDescriptor, MethodInfo method)
		{
			object[] attributes = method.GetCustomAttributes(typeof(SkipFilterAttribute), true);
			
			foreach (SkipFilterAttribute attr in attributes)
			{
				actionDescriptor.SkipFilters.Add(attr);
			}
		}

		private void CollectResources(BaseMetaDescriptor desc, MemberInfo memberInfo)
		{
			desc.Resources = resourceDescriptorProvider.CollectResources(memberInfo);
		}

		#endregion

		#region Controller data

		private void CollectClassLevelAttributes(Type controllerType, ControllerMetaDescriptor descriptor)
		{
			CollectHelpers(descriptor, controllerType);
			CollectResources(descriptor, controllerType);
			CollectFilters(descriptor, controllerType);
			CollectLayout(descriptor, controllerType);
			CollectRescues(descriptor, controllerType);
			CollectDefaultAction(descriptor, controllerType);
			CollectScaffolding(descriptor, controllerType);
			CollectDynamicAction(descriptor, controllerType);
		}

		private void CollectDefaultAction(ControllerMetaDescriptor descriptor, Type controllerType)
		{
			object[] attributes = controllerType.GetCustomAttributes(typeof(DefaultActionAttribute), true);

			if (attributes.Length != 0)
			{
				descriptor.DefaultAction = (DefaultActionAttribute) attributes[0];
			}
		}

		private void CollectScaffolding(ControllerMetaDescriptor descriptor, Type controllerType)
		{
			object[] attributes = controllerType.GetCustomAttributes(typeof(ScaffoldingAttribute), false);

			if (attributes.Length != 0)
			{
				foreach(ScaffoldingAttribute scaffolding in attributes)
				{
					descriptor.Scaffoldings.Add(scaffolding);
				}
			}
		}

		private void CollectDynamicAction(ControllerMetaDescriptor descriptor, Type controllerType)
		{
			object[] attributes = controllerType.GetCustomAttributes(typeof(DynamicActionProviderAttribute), true);

			if (attributes.Length != 0)
			{
				foreach(DynamicActionProviderAttribute attr in attributes)
				{
					descriptor.ActionProviders.Add(attr.ProviderType);
				}
			}
		}

		private void CollectHelpers(ControllerMetaDescriptor descriptor, Type controllerType)
		{
			descriptor.Helpers = 
				helperDescriptorProvider.CollectHelpers(controllerType);
		}

		private void CollectFilters(ControllerMetaDescriptor descriptor, Type controllerType)
		{
			descriptor.Filters = 
				filterDescriptorProvider.CollectFilters(controllerType);

			Array.Sort(descriptor.Filters, FilterDescriptorComparer.Instance);
		}

		private void CollectLayout(BaseMetaDescriptor descriptor, MemberInfo memberInfo)
		{
			descriptor.Layout = 
				layoutDescriptorProvider.CollectLayout(memberInfo);
		}

		private void CollectRescues(BaseMetaDescriptor descriptor, MemberInfo memberInfo)
		{
			descriptor.Rescues = 
				rescueDescriptorProvider.CollectRescues(memberInfo);
		}

		#endregion

		/// <summary>
		/// This <see cref="IComparer"/> implementation
		/// is used to sort the filters based on their Execution Order.
		/// </summary>
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

			public int Compare(object left, object right)
			{
				return ((FilterDescriptor) left).ExecutionOrder - ((FilterDescriptor) right).ExecutionOrder;
			}
		}
	}
}