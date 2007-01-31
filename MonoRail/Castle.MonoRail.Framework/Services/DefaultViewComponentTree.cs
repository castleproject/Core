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
	using System.Collections.Generic;

	public class DefaultViewComponentTree : IViewComponentTree
	{
		#region Member Data
		private Dictionary<string, Type> _map = new Dictionary<string, Type>();
		#endregion

		#region IViewComponentTree Members
		public void AddViewComponent(string name, Type type)
		{
			ViewComponentDetailsAttribute details = GetDetails(type);
			if (details != null)
			{
				name = details.Name;
			}

			if (_map.ContainsKey(name))
			{
				throw new ArgumentException(String.Format("ViewComponent '{0}' already registered as {1}!", name, _map[name]));
			}
			if (!typeof(ViewComponent).IsAssignableFrom(type))
			{
				throw new ArgumentException(String.Format("ViewComponent '{0}' is NOT a ViewComponent {1}!", name, type));
			}
			_map[name] = type;
		}

		public Type GetViewComponent(string name)
		{
			if (!_map.ContainsKey(name))
			{
				throw new ArgumentException(String.Format("ViewComponent '{0}' not registered!", name));
			}
			return _map[name];
		}
		#endregion

		protected static ViewComponentDetailsAttribute GetDetails(Type type)
		{
			object[] attributes = type.GetCustomAttributes(typeof(ViewComponentDetailsAttribute), true);
			if (attributes.Length == 0)
			{
				return null;
			}
			return (ViewComponentDetailsAttribute)attributes[0];
		}
	}
}
