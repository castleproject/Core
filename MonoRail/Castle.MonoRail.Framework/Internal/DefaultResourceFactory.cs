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
	using System.Resources;
	using System.Reflection;
	using System.Globalization;

	/// <summary>
	/// Standard implementation of <see cref="IResourceFactory" />
	/// </summary>
	public class DefaultResourceFactory : IResourceFactory
	{
		// TODO: Cache assembly resolution

		public IResource Create(ResourceDescriptor descriptor, Assembly appAssembly)
		{
			Assembly resAssembly = ResolveAssembly(descriptor.AssemblyName, appAssembly);
			CultureInfo culture = ResolveCulture(descriptor.CultureName);

			ResourceManager rm = new ResourceManager(
				descriptor.ResourceName, resAssembly, descriptor.ResourceType);

			return new ResourceFacade(rm.GetResourceSet(culture, true, true));
		}

		public void Release(IResource resource)
		{
			resource.Dispose();
		}

		private CultureInfo ResolveCulture(String name)
		{
			if ("neutral".Equals(name))
			{
				return CultureInfo.InvariantCulture;
			}
			else if (name != null)
			{
				return CultureInfo.CreateSpecificCulture(name);
			}

			return CultureInfo.CurrentCulture;
		}

		private Assembly ResolveAssembly(String name, Assembly assembly)
		{
			if (name == null) return assembly;

			return Assembly.Load(name);
		}
	}
}