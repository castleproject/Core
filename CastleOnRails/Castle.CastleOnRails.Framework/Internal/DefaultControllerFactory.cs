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

namespace Castle.CastleOnRails.Framework.Internal
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Summary description for DefaultControllerFactory.
	/// </summary>
	public class DefaultControllerFactory : AbstractControllerFactory
	{
		public DefaultControllerFactory()
		{
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
					Inspect( type );
				}
			}		
		}

		public virtual void Inspect(Type controllerType)
		{
			if (controllerType.IsDefined(typeof(ControllerDetailsAttribute), false))
			{
				object[] attrs = controllerType.GetCustomAttributes( 
					typeof(ControllerDetailsAttribute), false );

				ControllerDetailsAttribute details = attrs[0] as ControllerDetailsAttribute;

				RegisterController( details, controllerType );
			}
			else
			{
				RegisterController( controllerType );
			}
		}

		private void RegisterController(ControllerDetailsAttribute details, Type controller)
		{
			Tree.AddController( 
				details.Area, ObtainControllerName(details.Name, controller), controller );
		}

		private void RegisterController(Type controller)
		{
			Tree.AddController( 
				String.Empty, ObtainControllerName(null, controller), controller );
		}

		private String ObtainControllerName(String name, Type controller)
		{
			if (name == null || name.Length == 0)
			{
				return Strip(controller.Name);
			}
			return name;
		}

		private String Strip(String name)
		{
			if (name.EndsWith("Controller"))
			{
				return name.Substring(0, name.IndexOf("Controller"));
			}
			return name;
		}
	}
}
