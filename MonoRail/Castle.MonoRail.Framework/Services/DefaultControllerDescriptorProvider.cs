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

namespace Castle.MonoRail.Framework.Services
{
	using System;
	using System.Collections;
	using System.Reflection;
	using System.Threading;

	using Castle.Core.Logging;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Constructs and caches all collected information
	/// about a <see cref="Controller"/> and its actions.
	/// <seealso cref="ControllerMetaDescriptor"/>
	/// </summary>
	public class DefaultControllerDescriptorProvider : IControllerDescriptorProvider
	{
		/// <summary>
		/// The logger instance
		/// </summary>
		private ILogger logger = NullLogger.Instance;

		/// <summary>
		/// Used to lock the cache
		/// </summary>
		private ReaderWriterLock locker = new ReaderWriterLock();
		private Hashtable descriptorRepository = new Hashtable();
		private IHelperDescriptorProvider helperDescriptorProvider;
		private IFilterDescriptorProvider filterDescriptorProvider;
		private ILayoutDescriptorProvider layoutDescriptorProvider;
		private IRescueDescriptorProvider rescueDescriptorProvider;
		private IResourceDescriptorProvider resourceDescriptorProvider;

		#region IServiceEnabledComponent implementation

		public void Service(IServiceProvider serviceProvider)
		{
			ILoggerFactory loggerFactory = (ILoggerFactory) serviceProvider.GetService(typeof(ILoggerFactory));
			
			if (loggerFactory != null)
			{
				logger = loggerFactory.Create(typeof(DefaultControllerDescriptorProvider));
			}

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

		#endregion

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
			
			return BuildDescriptor(controllerType);
		}

		/// <summary>
		/// Constructs and populates a <see cref="ControllerMetaDescriptor"/>.
		/// </summary>
		/// <remarks>
		/// This implementation is also responsible for caching 
		/// constructed meta descriptors.
		/// </remarks>
		public ControllerMetaDescriptor BuildDescriptor(Type controllerType)
		{
			ControllerMetaDescriptor desc;

			locker.AcquireReaderLock(-1);

			desc = (ControllerMetaDescriptor) descriptorRepository[controllerType];

			if (desc != null)
			{
				locker.ReleaseReaderLock();
				return desc;
			}

			try
			{
				locker.UpgradeToWriterLock(-1);

				// We need to recheck after getting the writer lock
				desc = (ControllerMetaDescriptor) descriptorRepository[controllerType];

				if (desc != null)
				{
					return desc;
				}
				
				desc = InternalBuildDescriptor(controllerType);

				descriptorRepository[controllerType] = desc;
			}
			finally
			{
				locker.ReleaseWriterLock();
			}

			return desc;
		}
		
		private ControllerMetaDescriptor InternalBuildDescriptor(Type controllerType)
		{
			if (logger.IsDebugEnabled)
			{
				logger.Debug("Building controller descriptor for {0}", controllerType);
			}

			ControllerMetaDescriptor descriptor = new ControllerMetaDescriptor();

			CollectClassLevelAttributes(controllerType, descriptor);
			
			CollectActions(controllerType, descriptor);

			CollectActionLevelAttributes(descriptor);
			
			return descriptor;
		}

		#region Action data
		
		private void CollectActions(Type controllerType, ControllerMetaDescriptor desc)
		{
			// HACK: GetRealControllerType is a workaround for DYNPROXY-14 bug
			// see: http://support.castleproject.org/browse/DYNPROXY-14
			controllerType = GetRealControllerType(controllerType);

			MethodInfo[] methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

			foreach(MethodInfo method in methods)
			{
				Type declaringType = method.DeclaringType;

				if (declaringType == typeof(Object) || 
					declaringType == typeof(Controller) || 
					declaringType == typeof(SmartDispatcherController))
				{
					continue;
				}

				if (desc.Actions.Contains(method.Name))
				{
					ArrayList list = desc.Actions[method.Name] as ArrayList;

					if (list == null)
					{
						list = new ArrayList();
						list.Add(desc.Actions[method.Name]);

						desc.Actions[method.Name] = list;
					}

					list.Add(method);
				}
				else
				{
					desc.Actions[method.Name] = method;
				}
			}
		}

		private void CollectActionLevelAttributes(ControllerMetaDescriptor descriptor)
		{
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
		}

		private void CollectActionAttributes(MethodInfo method, ControllerMetaDescriptor descriptor)
		{
			if (logger.IsDebugEnabled)
			{
				logger.Debug("Collection attributes for action {0}", method.Name);
			}

			ActionMetaDescriptor actionDescriptor = descriptor.GetAction(method);

			CollectResources(actionDescriptor, method);
			CollectSkipFilter(actionDescriptor, method);
			CollectRescues(actionDescriptor, method);
			CollectAccessibleThrough(actionDescriptor, method);
			CollectSkipRescue(actionDescriptor, method);
			CollectLayout(actionDescriptor, method);
			CollectCacheConfigures(actionDescriptor, method);
			
			if (method.IsDefined(typeof(AjaxActionAttribute), true))
			{
				descriptor.AjaxActions.Add(method);
			}
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
			
			foreach(SkipFilterAttribute attr in attributes)
			{
				actionDescriptor.SkipFilters.Add(attr);
			}
		}

		private void CollectResources(BaseMetaDescriptor desc, MemberInfo memberInfo)
		{
			desc.Resources = resourceDescriptorProvider.CollectResources(memberInfo);
		}
		
		/// <summary>
		/// Gets the real controller type, instead of the proxy type.
		/// </summary>
		/// <remarks>
		/// Workaround for DYNPROXY-14 bug. See: http://support.castleproject.org/browse/DYNPROXY-14
		/// </remarks>
		private Type GetRealControllerType(Type controllerType)
		{
			Type prev = controllerType;

			// try to get the first non-proxy type
			while(controllerType.Assembly.FullName.StartsWith("DynamicProxyGenAssembly2") || controllerType.Assembly.FullName.StartsWith("DynamicAssemblyProxyGen"))
			{
				controllerType = controllerType.BaseType;

				if (controllerType == typeof(SmartDispatcherController) || 
				    controllerType == typeof(Controller))
				{
					// oops, it's a pure-proxy controller. just let it go.
					controllerType = prev;
					break;
				}
			}

			return controllerType;
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
			descriptor.Helpers = helperDescriptorProvider.CollectHelpers(controllerType);
		}

		private void CollectFilters(ControllerMetaDescriptor descriptor, Type controllerType)
		{
			descriptor.Filters = filterDescriptorProvider.CollectFilters(controllerType);

			Array.Sort(descriptor.Filters, FilterDescriptorComparer.Instance);
		}

		private void CollectLayout(BaseMetaDescriptor descriptor, MemberInfo memberInfo)
		{
			descriptor.Layout = layoutDescriptorProvider.CollectLayout(memberInfo);
		}

		private void CollectRescues(BaseMetaDescriptor descriptor, MemberInfo memberInfo)
		{
			descriptor.Rescues = rescueDescriptorProvider.CollectRescues(memberInfo);
		}

		private void CollectCacheConfigures(ActionMetaDescriptor descriptor, MemberInfo memberInfo)
		{
			object[] configurers = memberInfo.GetCustomAttributes(typeof(ICachePolicyConfigurer), true);

			if (configurers.Length != 0)
			{
				foreach(ICachePolicyConfigurer cacheConfigurer in configurers)
				{
					descriptor.CacheConfigurers.Add(cacheConfigurer);
				}
			}
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