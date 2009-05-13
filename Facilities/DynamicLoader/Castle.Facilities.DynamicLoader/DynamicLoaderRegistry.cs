// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.DynamicLoader
{
	using System;
	using System.Collections.Generic;

	using Castle.MicroKernel.Facilities;

	/// <summary>
	/// Stores instances of <see cref="RemoteLoader"/>.
	/// </summary>
	public class DynamicLoaderRegistry : IDisposable
	{
		private Dictionary<string, RemoteLoader> loaders = new Dictionary<string, RemoteLoader>();

		/// <summary>
		/// Register a new loader, for the specified <paramref name="domainId"/>.
		/// </summary>
		public void RegisterLoader(string domainId, RemoteLoader loader)
		{
			if (loaders == null)
				throw new ObjectDisposedException("DynamicLoaderRegistry");

			loaders.Add(domainId, loader);
		}

		/// <summary>
		/// Gets the <see cref="RemoteLoader"/> instance for the specified <paramref name="domainId"/>.
		/// </summary>
		public RemoteLoader GetLoader(string domainId)
		{
			if (loaders == null)
				throw new ObjectDisposedException("DynamicLoaderRegistry");

			RemoteLoader loader;
			if (!loaders.TryGetValue(domainId, out loader))
				throw new FacilityException("Domain not found: " + domainId);

			return loader;
		}

		/// <summary>
		/// Registers a specific component on a specific domain.
		/// </summary>
		/// <remarks>
		/// The implementation simply calls <see cref="GetLoader"/> to get the correct
		/// <see cref="RemoteLoader"/>, then add the component to the <see cref="RemoteLoader.Kernel"/>.
		/// </remarks>
		public void RegisterComponentOnDomain(string domainId, string key, Type service, Type component)
		{
			if (loaders == null)
				throw new ObjectDisposedException("DynamicLoaderRegistry");

			GetLoader(domainId).Kernel.AddComponent(key, service, component);
		}

		/// <summary>
		/// Implementation of <see cref="IDisposable"/>.
		/// </summary>
		public void Dispose()
		{
			foreach (RemoteLoader loader in loaders.Values)
			{
				loader.Kernel.Parent.RemoveChildKernel(loader.Kernel);
				
				loader.Dispose();

				AppDomain.Unload(loader.AppDomain);
			}
			
			loaders.Clear();
			loaders = null;
		}
	}
}