// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

	public class ControllerDescriptorBuilder
	{
		private ReaderWriterLock locker = new ReaderWriterLock();
		private Hashtable descriptorRepository = new Hashtable();

		public ControllerMetaDescriptor BuildDescriptor(Controller controller)
		{
			Type controllerType = controller.GetType();

			locker.AcquireReaderLock(-1);

			ControllerMetaDescriptor desc = (ControllerMetaDescriptor) descriptorRepository[controllerType];

			if (desc != null)
			{
				locker.ReleaseReaderLock();
				
				return desc;
			}

			locker.UpgradeToWriterLock(-1);

			try
			{
				desc = BuildDescriptor(controller, controllerType);

				descriptorRepository[controllerType] = desc;
			}
			finally
			{
				locker.ReleaseWriterLock();
			}

			return desc;
		}

		private ControllerMetaDescriptor BuildDescriptor(Controller controller, Type controllerType)
		{
			ControllerMetaDescriptor descriptor = new ControllerMetaDescriptor();

			CollectClassLevelAttributes(controllerType, descriptor);

			foreach(object action in controller.Actions)
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

		private static void CollectActionAttributes(MethodInfo method, ControllerMetaDescriptor descriptor)
		{
			object[] attributes = method.GetCustomAttributes(typeof(ResourceAttribute), true);

			foreach(ResourceAttribute resource in attributes)
			{
				descriptor.GetAction(method).Resources.Add(resource);
			}

			attributes = method.GetCustomAttributes(typeof(SkipFilterAttribute), true);

			foreach(SkipFilterAttribute resource in attributes)
			{
				descriptor.GetAction(method).SkipFilters.Add(resource);
			}

			attributes = method.GetCustomAttributes(typeof(RescueAttribute), true);

			if (attributes.Length != 0)
			{
				descriptor.GetAction(method).Rescues = attributes;
			}

			attributes = method.GetCustomAttributes(typeof(SkipRescueAttribute), true);

			if (attributes.Length != 0)
			{
				descriptor.GetAction(method).SkipRescue = (SkipRescueAttribute) attributes[0];
			}

			// TODO: CustomRescue
//			attributes = method.GetCustomAttributes(typeof(RescueAttribute), true);
//
//			foreach(RescueAttribute rescue in attributes)
//			{
//				descriptor.GetAction(method).Rescue = rescue;
//			}

			attributes = method.GetCustomAttributes( typeof(LayoutAttribute), true );
	
			if (attributes.Length != 0)
			{
				descriptor.GetAction(method).Layout = (LayoutAttribute) attributes[0];
			}
		}

		private static void CollectClassLevelAttributes(Type controllerType, ControllerMetaDescriptor descriptor)
		{
			object[] attributes = controllerType.GetCustomAttributes( typeof(DefaultActionAttribute), true );
	
			if (attributes.Length != 0)
			{
				descriptor.DefaultAction = (DefaultActionAttribute) attributes[0];
			}
	
			attributes = controllerType.GetCustomAttributes( typeof(HelperAttribute), true );
	
			if (attributes.Length != 0)
			{
				foreach(HelperAttribute helper in attributes)
				{
					descriptor.Helpers.Add(helper);
				}
			}
	
			attributes = controllerType.GetCustomAttributes( typeof(ResourceAttribute), true );
	
			if (attributes.Length != 0)
			{
				foreach(ResourceAttribute resource in attributes)
				{
					descriptor.Resources.Add(resource);
				}
			}
	
			attributes = controllerType.GetCustomAttributes( typeof(FilterAttribute), true );
	
			if (attributes.Length != 0)
			{
				foreach(FilterAttribute filter in attributes)
				{
					descriptor.Filters.Add(filter);
				}
			}
	
			attributes = controllerType.GetCustomAttributes( typeof(LayoutAttribute), true );
	
			if (attributes.Length != 0)
			{
				descriptor.Layout = (LayoutAttribute) attributes[0];
			}
	
			attributes = controllerType.GetCustomAttributes( typeof(RescueAttribute), true );
	
			if (attributes.Length != 0)
			{
				descriptor.Rescues = attributes;
			}
	
			attributes = controllerType.GetCustomAttributes( typeof(ScaffoldingAttribute), false );
	
			if (attributes.Length != 0)
			{
				descriptor.Scaffolding = (ScaffoldingAttribute) attributes[0];
			}
	
			attributes = controllerType.GetCustomAttributes( typeof(DynamicActionProviderAttribute), true );
	
			if (attributes.Length != 0)
			{
				foreach(DynamicActionProviderAttribute actionProvider in attributes)
				{
					descriptor.ActionProviders.Add(actionProvider);
				}
			}
		}
	}
}
