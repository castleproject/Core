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

namespace Castle.MonoRail.Framework.Services
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Centralizes the registration and lookup of ViewComponents
	/// </summary>
	public class DefaultViewComponentRegistry : IViewComponentRegistry
	{
		private readonly Dictionary<string,Type> name2Type = new Dictionary<string,Type>(StringComparer.InvariantCultureIgnoreCase);

		#region IViewComponentRegistry

		/// <summary>
		/// Adds the view component.
		/// </summary>
		/// <param name="name">The name that can be used from the view template.</param>
		/// <param name="type">The type.</param>
		public void AddViewComponent(string name, Type type)
		{
			ViewComponentDetailsAttribute details = GetDetails(type);

			if (details != null)
			{
				name = details.Name;
			}

			if (!typeof(ViewComponent).IsAssignableFrom(type))
			{
				throw new MonoRailException(String.Format("You tried to register '{0}' as a view component but it " +
					"doesn't seem the extend the ViewComponent abstract class: {1}", name, type.FullName));
			}

			if (name2Type.ContainsKey(name))
			{
				throw new MonoRailException(String.Format("ViewComponent '{0}' seems to be registered already. " + 
					"This is due to it being registered more than once or a name clash", name));
			}

			name2Type[name] = type;
		}

		/// <summary>
		/// Gets the view component.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public Type GetViewComponent(string name)
		{
			Type viewComponentType;

			if (name2Type.TryGetValue(name, out viewComponentType))
			{
				return viewComponentType;
			}
			else if (name2Type.TryGetValue(name + "component", out viewComponentType))
			{
				return viewComponentType;
			}
			else if (name2Type.TryGetValue(name + "viewcomponent", out viewComponentType))
			{
				return viewComponentType;
			}

			throw new MonoRailException(String.Format("ViewComponent '{0}' could not be found. Was it registered? " + 
				"If you have enabled Windsor Integration, then it's likely that you have forgot to register the " + 
				"view component as a Windsor component. If you are sure you did it, then make sure the name used " + 
				"is the component id or the key passed to ViewComponentDetailsAttribute", name));
		}

		/// <summary>
		/// Checks if the view component is registered.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>True if registered or false if GetViewComponent would throw an exception.</returns>
		public bool HasViewComponent(string name)
		{
			return name2Type.ContainsKey(name) ||
				   name2Type.ContainsKey(name + "component") ||
				   name2Type.ContainsKey(name + "viewcomponent");
		}

	    #endregion

		/// <summary>
		/// Gets the details.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		private static ViewComponentDetailsAttribute GetDetails(Type type)
		{
			// TODO: Add cache here, GetCustomAttributes is a lengthy call.

			object[] attributes = type.GetCustomAttributes(typeof(ViewComponentDetailsAttribute), false);
			
			if (attributes.Length == 0)
			{
				return null;
			}

			return (ViewComponentDetailsAttribute)attributes[0];
		}
	}
}
