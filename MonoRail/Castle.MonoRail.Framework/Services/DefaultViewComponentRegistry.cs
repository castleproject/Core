// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

	public class DefaultViewComponentRegistry : IViewComponentRegistry
	{
		private readonly Hashtable name2Type = new Hashtable();

		#region IViewComponentRegistry

		public void AddViewComponent(string name, Type type)
		{
			ViewComponentDetailsAttribute details = GetDetails(type);

			if (details != null)
			{
				name = details.Name;
			}

			name = NormalizeName(name);

			if (name2Type.Contains(name))
			{
				throw new RailsException(String.Format("ViewComponent '{0}' already registered as {1}!", name, name2Type[name]));
			}
			
			if (!typeof(ViewComponent).IsAssignableFrom(type))
			{
				throw new RailsException(String.Format("ViewComponent '{0}' is NOT a ViewComponent {1}!", name, type));
			}

			name2Type[name] = type;
		}

		public Type GetViewComponent(string name)
		{
			name = NormalizeName(name);

			if (!name2Type.Contains(name))
			{
				throw new RailsException(String.Format("ViewComponent '{0}' not registered!", name));
			}

			return (Type) name2Type[name];
		}

		#endregion

		private string NormalizeName(string name)
		{
			if (!name.EndsWith("Component"))
			{
				return name + "Component";
			}
			return name;
		}

		private static ViewComponentDetailsAttribute GetDetails(Type type)
		{
			// TODO: Add cache here, GetCustomAttributes is a lengthy call.

			object[] attributes = type.GetCustomAttributes(typeof(ViewComponentDetailsAttribute), true);
			
			if (attributes.Length == 0)
			{
				return null;
			}

			return (ViewComponentDetailsAttribute)attributes[0];
		}
	}
}
