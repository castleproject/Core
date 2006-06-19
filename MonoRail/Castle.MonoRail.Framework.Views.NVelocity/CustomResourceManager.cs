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

using IRuntimeServices = NVelocity.Runtime.IRuntimeServices;
using NVelocity.Runtime.Resource;
using ResourceNotFoundException = NVelocity.Exception.ResourceNotFoundException;

namespace Castle.MonoRail.Framework.Views.NVelocity
{
	using System;

	using Castle.MonoRail.Framework.Internal;


	public class CustomResourceManager : IResourceManager
	{
		private IRuntimeServices runtimeServices;
		private ResourceLoaderAdapter resourceLoaderAdapter;
		private ICacheProvider cacheProvider;

		public CustomResourceManager(IViewSourceLoader sourceLoader)
		{
			resourceLoaderAdapter = new ResourceLoaderAdapter(sourceLoader);
		}

		public void Initialize(IRuntimeServices runtimeServices)
		{
			this.runtimeServices = runtimeServices;

			IServiceProvider serviceProvider = (IServiceProvider) 
				runtimeServices.GetApplicationAttribute(NVelocityViewEngine.ServiceProvider);

			cacheProvider = (ICacheProvider) serviceProvider.GetService(typeof(ICacheProvider));
		}

		public Resource GetResource(string resourceName, ResourceType resourceType, string encoding)
		{
			Resource resource = (Resource) cacheProvider.Get(resourceName);

			if (resource == null)
			{
				resource = new CustomTemplate();

				InitializeResource(resource, resourceName, encoding);

				resource.ResourceLoader = resourceLoaderAdapter;

				resource.Process();

				if (resource.Data == null)
				{
					throw new ResourceNotFoundException("Unable to find resource '" + resourceName + "'");
				}

				resource.LastModified = DateTime.Now.Ticks;
				resource.ModificationCheckInterval = resourceLoaderAdapter.ModificationCheckInterval;
				resource.Touch();

				if (!resource.IsSourceModified())
				{
					cacheProvider.Store(resourceName, resource);
				}
			}
			else
			{
				if (resource.IsSourceModified())
				{
					resource.Process();
				}
			}

			return resource;
		}

		public string GetLoaderNameForResource(string resourceName)
		{
			return "default";
		}

		private void InitializeResource(Resource resource, string resourceName, string encoding)
		{
			resource.RuntimeServices = runtimeServices;
			resource.Name = resourceName;
			resource.Encoding = encoding;
		}
	}
}
