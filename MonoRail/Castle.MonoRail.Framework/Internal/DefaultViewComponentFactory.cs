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
	using System.Reflection;

	public class DefaultViewComponentFactory : IViewComponentFactory
	{
		private IViewEngine viewEngine;
		private readonly IDictionary components;

		public DefaultViewComponentFactory()
		{
			components = new HybridDictionary(true);
		}

		/// <summary>
		/// Loads the assembly and inspect its public types.
		/// </summary>
		/// <param name="assemblyFileName"></param>
		public void Inspect(String assemblyFileName)
		{
			Assembly assembly = Assembly.Load( assemblyFileName );
			Inspect(assembly);
		}

		/// <summary>
		/// Inspect the assembly's public types.
		/// </summary>
		public void Inspect(Assembly assembly)
		{
			Type[] types = assembly.GetExportedTypes();

			foreach(Type type in types)
			{
				if (!type.IsPublic || type.IsAbstract || type.IsInterface || type.IsValueType)
				{
					continue;
				}

				if ( typeof(ViewComponent).IsAssignableFrom(type) )
				{					
					components[ type.Name ] = type;
				}
			}		
		}

		public IViewEngine ViewEngine
		{
			get { return viewEngine; }
			set { viewEngine = value; }
		}

		public virtual ViewComponent Create(String name)
		{
			Type viewCompType = (Type) components[name];

			if (viewCompType == null)
			{
				throw new RailsException("No ViewComponent found for name " + name);
			}

			return (ViewComponent) Activator.CreateInstance( viewCompType );
		}

		public virtual void Release(ViewComponent instance)
		{
			
		}
	}
}
