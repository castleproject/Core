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

namespace Castle.MonoRail.Framework.Providers
{
	using System;
	using System.Collections;
	using System.Reflection;
	using System.Threading;
	using Castle.Core.Logging;
	using Castle.MonoRail.Framework.Services.Utils;
	using Descriptors;
	using Providers;

	/// <summary>
	/// Constructs and caches all collected information
	/// about a <see cref="IController"/> and its actions.
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
		private ITransformFilterDescriptorProvider transformFilterDescriptorProvider;
		private IReturnBinderDescriptorProvider returnBinderDescriptorProvider;
		private IDynamicActionProviderDescriptorProvider dynamicActionProviderDescriptorProvider;

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultControllerDescriptorProvider"/> class.
		/// </summary>
		public DefaultControllerDescriptorProvider()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultControllerDescriptorProvider"/> class.
		/// </summary>
		/// <param name="helperDescriptorProvider">The helper descriptor provider.</param>
		/// <param name="filterDescriptorProvider">The filter descriptor provider.</param>
		/// <param name="layoutDescriptorProvider">The layout descriptor provider.</param>
		/// <param name="rescueDescriptorProvider">The rescue descriptor provider.</param>
		/// <param name="resourceDescriptorProvider">The resource descriptor provider.</param>
		/// <param name="transformFilterDescriptorProvider">The transform filter descriptor provider.</param>
		/// <param name="returnBinderDescriptorProvider">The return binder descriptor provider.</param>
		/// <param name="dynamicActionProviderDescriptorProvider">The dynamic action provider descriptor provider.</param>
		public DefaultControllerDescriptorProvider(IHelperDescriptorProvider helperDescriptorProvider, 
													IFilterDescriptorProvider filterDescriptorProvider, 
													ILayoutDescriptorProvider layoutDescriptorProvider, 
													IRescueDescriptorProvider rescueDescriptorProvider, 
													IResourceDescriptorProvider resourceDescriptorProvider, 
													ITransformFilterDescriptorProvider transformFilterDescriptorProvider, 
													IReturnBinderDescriptorProvider returnBinderDescriptorProvider, 
													IDynamicActionProviderDescriptorProvider dynamicActionProviderDescriptorProvider)
		{
			this.helperDescriptorProvider = helperDescriptorProvider;
			this.filterDescriptorProvider = filterDescriptorProvider;
			this.layoutDescriptorProvider = layoutDescriptorProvider;
			this.rescueDescriptorProvider = rescueDescriptorProvider;
			this.resourceDescriptorProvider = resourceDescriptorProvider;
			this.transformFilterDescriptorProvider = transformFilterDescriptorProvider;
			this.returnBinderDescriptorProvider = returnBinderDescriptorProvider;
			this.dynamicActionProviderDescriptorProvider = dynamicActionProviderDescriptorProvider;
		}

		#region IMRServiceEnabled implementation

		/// <summary>
		/// Services the specified service provider.
		/// </summary>
		/// <param name="serviceProvider">The service provider.</param>
		public void Service(IMonoRailServices serviceProvider)
		{
			ILoggerFactory loggerFactory = (ILoggerFactory) serviceProvider.GetService(typeof(ILoggerFactory));

			if (loggerFactory != null)
			{
				logger = loggerFactory.Create(typeof(DefaultControllerDescriptorProvider));
			}

			helperDescriptorProvider = serviceProvider.GetService<IHelperDescriptorProvider>();
			filterDescriptorProvider = serviceProvider.GetService<IFilterDescriptorProvider>();
			layoutDescriptorProvider = serviceProvider.GetService<ILayoutDescriptorProvider>();
			rescueDescriptorProvider = serviceProvider.GetService<IRescueDescriptorProvider>();
			resourceDescriptorProvider = serviceProvider.GetService<IResourceDescriptorProvider>();
			transformFilterDescriptorProvider = serviceProvider.GetService<ITransformFilterDescriptorProvider>();
			returnBinderDescriptorProvider = serviceProvider.GetService<IReturnBinderDescriptorProvider>();
			dynamicActionProviderDescriptorProvider = serviceProvider.GetService<IDynamicActionProviderDescriptorProvider>();
		}

		#endregion

		/// <summary>
		/// Occurs when the providers needs to create a <see cref="ControllerMetaDescriptor"/>.
		/// </summary>
		public event MetaCreatorHandler Create;

		/// <summary>
		/// Occurs when the meta descriptor is about to the returned to the caller.
		/// </summary>
		public event ControllerMetaDescriptorHandler AfterProcess;

		/// <summary>
		/// Occurs when the providers needs to create a <see cref="ActionMetaDescriptor"/>.
		/// </summary>
		public event ActionMetaCreatorHandler ActionCreate;

		/// <summary>
		/// Occurs when the meta descriptor is about to be included on the <see cref="ControllerMetaDescriptor"/>.
		/// </summary>
		public event ActionMetaDescriptorHandler AfterActionProcess;

		/// <summary>
		/// Constructs and populates a <see cref="ControllerMetaDescriptor"/>.
		/// </summary>
		/// <remarks>
		/// This implementation is also responsible for caching 
		/// constructed meta descriptors.
		/// </remarks>
		public ControllerMetaDescriptor BuildDescriptor(IController controller)
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
			try
			{
				desc = (ControllerMetaDescriptor) descriptorRepository[controllerType];

				if (desc != null)
				{
					return desc;
				}
			}
			finally
			{
				locker.ReleaseReaderLock();
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

		/// <summary>
		/// Builds the <see cref="ControllerMetaDescriptor"/> for the specified controller type
		/// </summary>
		/// <param name="controllerType">Type of the controller.</param>
		/// <returns></returns>
		private ControllerMetaDescriptor InternalBuildDescriptor(Type controllerType)
		{
			if (logger.IsDebugEnabled)
			{
				logger.DebugFormat("Building controller descriptor for {0}", controllerType);
			}

			ControllerMetaDescriptor descriptor = CreateMetaDescriptor();

			descriptor.ControllerDescriptor = ControllerInspectionUtil.Inspect(controllerType);

			CollectClassLevelAttributes(controllerType, descriptor);

			CollectActions(controllerType, descriptor);

			CollectActionLevelAttributes(descriptor);

			if (AfterProcess != null)
			{
				AfterProcess(descriptor);
			}

			return descriptor;
		}

		#region Action data

		/// <summary>
		/// Collects the actions.
		/// </summary>
		/// <param name="controllerType">Type of the controller.</param>
		/// <param name="desc">The desc.</param>
		private void CollectActions(Type controllerType, ControllerMetaDescriptor desc)
		{
			// HACK: GetRealControllerType is a workaround for DYNPROXY-14 bug
			// see: http://support.castleproject.org/browse/DYNPROXY-14
			controllerType = GetRealControllerType(controllerType);

			MethodInfo[] methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

			foreach(MethodInfo method in methods)
			{
				Type declaringType = method.DeclaringType;

				if (method.IsSpecialName)
				{
					continue;
				}

				if (declaringType == typeof(Object) ||
				    declaringType == typeof(IController) || declaringType == typeof(Controller))
					// || declaringType == typeof(SmartDispatcherController))
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

			MergeAsyncMethodPairsToSingleAction(desc);
		}

		private void MergeAsyncMethodPairsToSingleAction(ControllerMetaDescriptor desc)
		{
			foreach(string name in new ArrayList(desc.Actions.Keys))
			{
				// skip methods that are not named "BeginXyz"
				if (!name.StartsWith("Begin", StringComparison.InvariantCultureIgnoreCase))
				{
					continue;
				}

				string actionName = name.Substring("Begin".Length);

				ArrayList list = desc.Actions[name] as ArrayList;

				if (list != null)
				{
					foreach(MethodInfo info in list)
					{
						if (info.ReturnType == typeof(IAsyncResult))
						{
							throw new MonoRailException("Action '" + actionName + "' on controller '" + desc.ControllerDescriptor.Name +
							                            "' is an async action, but there are method overloads '" + name +
							                            "(...)', which is not allowed on async actions.");
						}
					}
					continue;
				}

				if (desc.Actions.Contains(actionName))
				{
					throw new MonoRailException("Found both async method '" + name + "' and sync method '" + actionName +
					                            "' on controller '" + desc.ControllerDescriptor.Name +
					                            "'. MonoRail doesn't support mixing sync and async methods for the same action");
				}
				
				MethodInfo beginActionInfo = (MethodInfo) desc.Actions[name];
				// we allow BeginXyz method as sync methods, as long as they do not return
				// IAsyncResult
				if(beginActionInfo.ReturnType != typeof(IAsyncResult))
					continue;
				
				string endActionName = "End" + actionName;
				if (desc.Actions.Contains(endActionName) == false)
				{
					throw new MonoRailException("Found beginning of async pair '" + name + "' but not the end '" + endActionName +
					                            "' on controller '" + desc.ControllerDescriptor.Name + "', did you forget to define " +
					                            endActionName + "(IAsyncResult ar) ?");
				}

				if (desc.Actions[endActionName] is IList)
				{
					throw new MonoRailException("Found more than a single " + endActionName +
					                            " method, for async action '" + actionName + "' on controller '" +
					                            desc.ControllerDescriptor.Name + "', only a single " +
					                            endActionName + " may be defined as part of an async action");
				}

				MethodInfo endActionInfo = (MethodInfo) desc.Actions[endActionName];

				desc.Actions.Remove(name);
				desc.Actions.Remove(endActionName);
				desc.Actions[actionName] = new AsyncActionPair(actionName, beginActionInfo, endActionInfo);
			}
		}

		/// <summary>
		/// Collects the action level attributes.
		/// </summary>
		/// <param name="descriptor">The descriptor.</param>
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

				MethodInfo methodInfo = action as MethodInfo;

				if (methodInfo != null)
				{
					CollectActionAttributes(methodInfo, descriptor);
				}
				else
				{
					AsyncActionPair asyncActionPair = (AsyncActionPair) action;
					CollectActionAttributes(asyncActionPair.BeginActionInfo, descriptor);
					CollectActionAttributes(asyncActionPair.EndActionInfo, descriptor);
				}
			}
		}

		/// <summary>
		/// Collects the action attributes.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="descriptor">The descriptor.</param>
		private void CollectActionAttributes(MethodInfo method, ControllerMetaDescriptor descriptor)
		{
			if (logger.IsDebugEnabled)
			{
				logger.DebugFormat("Collection attributes for action {0}", method.Name);
			}

			ActionMetaDescriptor actionDescriptor = descriptor.GetAction(method);

			if (actionDescriptor == null)
			{
				actionDescriptor = CreateActionDescriptor();

				descriptor.AddAction(method, actionDescriptor);
			}

			CollectResources(actionDescriptor, method);
			CollectSkipFilter(actionDescriptor, method);
			CollectRescues(actionDescriptor, method);
			CollectAccessibleThrough(actionDescriptor, method);
			CollectSkipRescue(actionDescriptor, method);
			CollectLayout(actionDescriptor, method);
			CollectCacheConfigure(actionDescriptor, method);
			CollectTransformFilter(actionDescriptor, method);
			CollectReturnTypeBinder(actionDescriptor, method);

			if (method.IsDefined(typeof(AjaxActionAttribute), true))
			{
				descriptor.AjaxActions.Add(method);
			}

			if (method.IsDefined(typeof(DefaultActionAttribute), true))
			{
				if (descriptor.DefaultAction != null)
				{
					throw new MonoRailException(
						"Cannot resolve a default action for {0}, DefaultActionAttribute was declared more than once.",
						method.DeclaringType.FullName);
				}
				descriptor.DefaultAction = new DefaultActionAttribute(method.Name);
			}

			if (AfterActionProcess != null)
			{
				AfterActionProcess(actionDescriptor);
			}
		}

		/// <summary>
		/// Collects the skip rescue.
		/// </summary>
		/// <param name="actionDescriptor">The action descriptor.</param>
		/// <param name="method">The method.</param>
		private void CollectSkipRescue(ActionMetaDescriptor actionDescriptor, MethodInfo method)
		{
			object[] attributes = method.GetCustomAttributes(typeof(SkipRescueAttribute), true);

			if (attributes.Length != 0)
			{
				actionDescriptor.SkipRescue = (SkipRescueAttribute) attributes[0];
			}
		}

		/// <summary>
		/// Collects the accessible through.
		/// </summary>
		/// <param name="actionDescriptor">The action descriptor.</param>
		/// <param name="method">The method.</param>
		private void CollectAccessibleThrough(ActionMetaDescriptor actionDescriptor, MethodInfo method)
		{
			object[] attributes = method.GetCustomAttributes(typeof(AccessibleThroughAttribute), true);

			if (attributes.Length != 0)
			{
				actionDescriptor.AccessibleThrough = (AccessibleThroughAttribute) attributes[0];
			}
		}

		/// <summary>
		/// Collects the skip filter.
		/// </summary>
		/// <param name="actionDescriptor">The action descriptor.</param>
		/// <param name="method">The method.</param>
		private void CollectSkipFilter(ActionMetaDescriptor actionDescriptor, MethodInfo method)
		{
			object[] attributes = method.GetCustomAttributes(typeof(SkipFilterAttribute), true);

			foreach(SkipFilterAttribute attr in attributes)
			{
				actionDescriptor.SkipFilters.Add(attr);
			}
		}

		/// <summary>
		/// Collects the resources.
		/// </summary>
		/// <param name="desc">The desc.</param>
		/// <param name="memberInfo">The member info.</param>
		private void CollectResources(BaseMetaDescriptor desc, MemberInfo memberInfo)
		{
			desc.Resources = resourceDescriptorProvider.CollectResources(memberInfo);
		}

		/// <summary>
		/// Collects the transform filter.
		/// </summary>
		/// <param name="actionDescriptor">The action descriptor.</param>
		/// <param name="method">The method.</param>
		private void CollectTransformFilter(ActionMetaDescriptor actionDescriptor, MethodInfo method)
		{
			actionDescriptor.TransformFilters = transformFilterDescriptorProvider.CollectFilters((method));
			Array.Sort(actionDescriptor.TransformFilters, TransformFilterDescriptorComparer.Instance);
		}

		/// <summary>
		/// Collects the return type binder.
		/// </summary>
		/// <param name="actionDescriptor">The action descriptor.</param>
		/// <param name="method">The method.</param>
		private void CollectReturnTypeBinder(ActionMetaDescriptor actionDescriptor, MethodInfo method)
		{
			actionDescriptor.ReturnDescriptor = returnBinderDescriptorProvider.Collect(method);
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
			while(controllerType.Assembly.FullName.StartsWith("DynamicProxyGenAssembly2") ||
			      controllerType.Assembly.FullName.StartsWith("DynamicAssemblyProxyGen"))
			{
				controllerType = controllerType.BaseType;

				if ( // controllerType == typeof(SmartDispatcherController) || 
					controllerType == typeof(IController))
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

		/// <summary>
		/// Collects the class level attributes.
		/// </summary>
		/// <param name="controllerType">Type of the controller.</param>
		/// <param name="descriptor">The descriptor.</param>
		private void CollectClassLevelAttributes(Type controllerType, ControllerMetaDescriptor descriptor)
		{
			CollectHelpers(descriptor, controllerType);
			CollectResources(descriptor, controllerType);
			CollectFilters(descriptor, controllerType);
			CollectLayout(descriptor, controllerType);
			CollectRescues(descriptor, controllerType);
			CollectDefaultAction(descriptor, controllerType);
			CollectScaffolding(descriptor, controllerType);
			CollectDynamicActionProviders(descriptor, controllerType);
			CollectCacheConfigure(descriptor, controllerType);
		}

		/// <summary>
		/// Collects the default action.
		/// </summary>
		/// <param name="descriptor">The descriptor.</param>
		/// <param name="controllerType">Type of the controller.</param>
		private void CollectDefaultAction(ControllerMetaDescriptor descriptor, Type controllerType)
		{
			object[] attributes = controllerType.GetCustomAttributes(typeof(DefaultActionAttribute), true);

			if (attributes.Length != 0)
			{
				descriptor.DefaultAction = (DefaultActionAttribute) attributes[0];
			}
		}

		/// <summary>
		/// Collects the scaffolding.
		/// </summary>
		/// <param name="descriptor">The descriptor.</param>
		/// <param name="controllerType">Type of the controller.</param>
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

		/// <summary>
		/// Collects the dynamic action providers.
		/// </summary>
		/// <param name="descriptor">The descriptor.</param>
		/// <param name="controllerType">Type of the controller.</param>
		private void CollectDynamicActionProviders(ControllerMetaDescriptor descriptor, Type controllerType)
		{
			descriptor.DynamicActionProviders = dynamicActionProviderDescriptorProvider.CollectProviders(controllerType);
		}

		/// <summary>
		/// Collects the helpers.
		/// </summary>
		/// <param name="descriptor">The descriptor.</param>
		/// <param name="controllerType">Type of the controller.</param>
		private void CollectHelpers(ControllerMetaDescriptor descriptor, Type controllerType)
		{
			descriptor.Helpers = helperDescriptorProvider.CollectHelpers(controllerType);
		}

		/// <summary>
		/// Collects the filters.
		/// </summary>
		/// <param name="descriptor">The descriptor.</param>
		/// <param name="controllerType">Type of the controller.</param>
		private void CollectFilters(ControllerMetaDescriptor descriptor, Type controllerType)
		{
			descriptor.Filters = filterDescriptorProvider.CollectFilters(controllerType);

			Array.Sort(descriptor.Filters, FilterDescriptorComparer.Instance);
		}

		/// <summary>
		/// Collects the layout.
		/// </summary>
		/// <param name="descriptor">The descriptor.</param>
		/// <param name="memberInfo">The member info.</param>
		private void CollectLayout(BaseMetaDescriptor descriptor, MemberInfo memberInfo)
		{
			descriptor.Layout = layoutDescriptorProvider.CollectLayout(memberInfo);
		}

		/// <summary>
		/// Collects the rescues.
		/// </summary>
		/// <param name="descriptor">The descriptor.</param>
		/// <param name="memberInfo">The member info.</param>
		private void CollectRescues(BaseMetaDescriptor descriptor, MethodInfo memberInfo)
		{
			descriptor.Rescues = rescueDescriptorProvider.CollectRescues(memberInfo);
		}

		/// <summary>
		/// Collects the rescues.
		/// </summary>
		/// <param name="descriptor">The descriptor.</param>
		/// <param name="type">The type.</param>
		private void CollectRescues(BaseMetaDescriptor descriptor, Type type)
		{
			descriptor.Rescues = rescueDescriptorProvider.CollectRescues(type);
		}

		/// <summary>
		/// Collects the cache configures.
		/// </summary>
		/// <param name="descriptor">The descriptor.</param>
		/// <param name="memberInfo">The member info.</param>
		private void CollectCacheConfigure(BaseMetaDescriptor descriptor, MemberInfo memberInfo)
		{
			object[] configurers = memberInfo.GetCustomAttributes(typeof(ICachePolicyConfigurer), true);

			if (configurers.Length != 0)
			{
				descriptor.CacheConfigurer = (ICachePolicyConfigurer) configurers[0];
			}
		}

		#endregion

		private ControllerMetaDescriptor CreateMetaDescriptor()
		{
			if (Create != null)
			{
				return Create();
			}
			else
			{
				return new ControllerMetaDescriptor();
			}
		}

		private ActionMetaDescriptor CreateActionDescriptor()
		{
			if (ActionCreate != null)
			{
				return ActionCreate();
			}
			else
			{
				return new ActionMetaDescriptor();
			}
		}

		/// <summary>
		/// This <see cref="IComparer"/> implementation
		/// is used to sort the filters based on their Execution Order.
		/// </summary>
		private class FilterDescriptorComparer : IComparer
		{
			private static readonly FilterDescriptorComparer instance = new FilterDescriptorComparer();

			/// <summary>
			/// Initializes a new instance of the <see cref="FilterDescriptorComparer"/> class.
			/// </summary>
			private FilterDescriptorComparer()
			{
			}

			/// <summary>
			/// Gets the instance.
			/// </summary>
			/// <value>The instance.</value>
			public static FilterDescriptorComparer Instance
			{
				get { return instance; }
			}

			/// <summary>
			/// Compares the specified left.
			/// </summary>
			/// <param name="left">The left.</param>
			/// <param name="right">The right.</param>
			/// <returns></returns>
			public int Compare(object left, object right)
			{
				return ((FilterDescriptor) left).ExecutionOrder - ((FilterDescriptor) right).ExecutionOrder;
			}
		}

		/// <summary>
		/// This <see cref="IComparer"/> implementation
		/// is used to sort the transformfilters based on their Execution Order.
		/// </summary>
		private class TransformFilterDescriptorComparer : IComparer
		{
			private static readonly TransformFilterDescriptorComparer instance = new TransformFilterDescriptorComparer();

			/// <summary>
			/// Initializes a new instance of the <see cref="TransformFilterDescriptorComparer"/> class.
			/// </summary>
			private TransformFilterDescriptorComparer()
			{
			}

			/// <summary>
			/// Gets the instance.
			/// </summary>
			/// <value>The instance.</value>
			public static TransformFilterDescriptorComparer Instance
			{
				get { return instance; }
			}

			/// <summary>
			/// Compares the specified left.
			/// </summary>
			/// <param name="left">The left.</param>
			/// <param name="right">The right.</param>
			/// <returns></returns>
			public int Compare(object left, object right)
			{
				return ((TransformFilterDescriptor) right).ExecutionOrder - ((TransformFilterDescriptor) left).ExecutionOrder;
			}
		}
	}
}
