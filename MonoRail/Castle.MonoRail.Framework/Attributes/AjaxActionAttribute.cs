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

namespace Castle.MonoRail.Framework
{
	using System;

	using Castle.MonoRail.Framework.Helpers;

	/// <summary>
	/// Defines that an action is accessible through AJAX calls,
	/// so <see cref="AjaxHelper"/> can generate a JavaScript proxy for it.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
	public class AjaxActionAttribute : Attribute
	{
		private string name;

		/// <summary>
		/// Method marked with this attribute will be accessible through AJAX calls,
		/// and <see cref="AjaxHelper"/> will be able to generate a JavaScript proxy for them.
		/// </summary>
		public AjaxActionAttribute()
		{
		}

		/// <summary>
		/// Method marked with this attribute will be accessible through AJAX calls,
		/// and <see cref="AjaxHelper"/> will be able to generate a JavaScript proxy for them.
		/// </summary>
		/// <param name="name">
		/// A name for the action, on the JavaScript proxy. Useful when dealing with
		/// overloaded Ajax actions, as JavaScript does not support function overloading.
		/// </param>
		public AjaxActionAttribute(string name)
		{
			this.name = name;
		}

		/// <summary>
		/// A name for the action, on the JavaScript proxy. Useful when dealing with
		/// overloaded Ajax actions, as JavaScript does not support function overloading.
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}
	}
}
