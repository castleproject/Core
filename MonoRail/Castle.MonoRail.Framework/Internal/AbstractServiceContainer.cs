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
	using System.Collections.Specialized;
	using System.ComponentModel.Design;

	public abstract class AbstractServiceContainer : MarshalByRefObject, IServiceContainer
	{
		private readonly IServiceContainer parent;
		private IDictionary type2Service;

		public AbstractServiceContainer()
		{
		}

		public AbstractServiceContainer(IServiceContainer parent)
		{
			this.parent = parent;
		}

		public void AddService(Type serviceType, object serviceInstance)
		{
			AddService(serviceType, serviceInstance, false);
		}

		public void AddService(Type serviceType, object serviceInstance, bool promote)
		{
			if (promote)
			{
				IServiceContainer parentContainer = ParentContainer;

				if (parentContainer != null)
				{
					parentContainer.AddService(serviceType, serviceInstance, promote);
					return;
				}
			}

			if (type2Service == null)
			{
				type2Service = new HybridDictionary();
			}

			type2Service[serviceType] = serviceInstance;		
		}

		public void AddService(Type serviceType, ServiceCreatorCallback callback)
		{
			throw new NotImplementedException();
		}

		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
		{
			throw new NotImplementedException();
		}

		public void RemoveService(Type serviceType)
		{
			RemoveService(serviceType, false);
		}

		public void RemoveService(Type serviceType, bool promote)
		{
			if (promote)
			{
				IServiceContainer parentContainer = ParentContainer;

				if (parentContainer != null)
				{
					parentContainer.RemoveService(serviceType, promote);
					return;
				}
			}

			if (type2Service != null)
			{
				type2Service.Remove(serviceType);
			}
		}

		public object GetService(Type serviceType)
		{
			object service = null;

			if (serviceType == typeof(IServiceContainer))
			{
				return this;
			}
			
			if (type2Service != null)
			{
				service = type2Service[serviceType];
			}

			if (service == null && parent != null)
			{
				service = parent.GetService(serviceType);
			}

			return service;
		}

		private IServiceContainer ParentContainer
		{
			get
			{
				IServiceContainer container = null;
				
				if (parent != null)
				{
					container = (IServiceContainer) parent.GetService(typeof(IServiceContainer));
				}
				
				return container;
			}
		}
	}
}
