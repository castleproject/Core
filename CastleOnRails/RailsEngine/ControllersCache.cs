// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.CastleOnRails.Engine
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Reflection;

	using Castle.CastleOnRails.Framework;

	/// <summary>
	/// Summary description for ControllersCache.
	/// </summary>
	public class ControllersCache
	{
		private IDictionary _name2ControllerType;

		public ControllersCache()
		{
			_name2ControllerType = new HybridDictionary(true);
		}

		public void Inspect(String assemblyFileName)
		{
			Assembly assembly = Assembly.Load( assemblyFileName );
			Inspect(assembly);
		}

		public void Inspect(Assembly assembly)
		{
			// Due to our goal to be compabible
			// with Mono, we use GetTypes instead of GetExportedTypes

			Type[] types = assembly.GetTypes();

			foreach(Type type in types)
			{
				if (!type.IsPublic || type.IsAbstract || type.IsInterface)
				{
					continue;
				}

				if ( typeof(Controller).IsAssignableFrom(type) )
				{
					_name2ControllerType.Add( type.Name, type );
				}
			}		
		}

		/// <summary>
		/// Returns the controller's type if found.
		/// </summary>
		/// <remarks>
		/// We try different variations to obtain the controller:
		/// <ol>
		/// <li>The name specified</li>
		/// <li>The name + Controller</li>
		/// </ol>
		/// </remarks>
		/// <param name="name"></param>
		/// <returns></returns>
		public Type GetController(String name)
		{
			Type type = (Type) _name2ControllerType[name];

			if (type == null)
			{
				type = (Type) _name2ControllerType[name + "controller"];
			}

			return type;
		}
	}
}
