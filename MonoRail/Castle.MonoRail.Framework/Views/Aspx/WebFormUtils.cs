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

using System;

namespace Castle.MonoRail.Framework.Views.Aspx
{
	using System.Web.UI;
	using System.Reflection;

	/// <summary>
	/// Pendent
	/// </summary>
	public static class WebFormUtils
	{
		private static readonly BindingFlags BindingFlagsGet
			= BindingFlags.Public
			| BindingFlags.NonPublic
			| BindingFlags.Instance
			| BindingFlags.GetProperty
			| BindingFlags.GetField
			;

		private static readonly BindingFlags BindingFlagsSet
			= BindingFlags.Public
			| BindingFlags.NonPublic
			| BindingFlags.Instance
			| BindingFlags.SetProperty
			| BindingFlags.SetField
			;

		/// <summary>
		/// Finds the first control that matches the id, rescursively.
		/// </summary>
		/// <param name="rootControl">The root control.</param>
		/// <param name="controlId">The id of the control to search.</param>
		/// <returns>The matching control, or null if not found.</returns>
		public static Control FindControlRecursive(Control rootControl, string controlId)
		{
			if (rootControl.ID == controlId)
				return rootControl;

			foreach (Control control in rootControl.Controls)
			{
				Control candidate = FindControlRecursive(control, controlId);
				if (candidate != null) return candidate;
			}

			return null;
		}

		/// <summary>
		/// Gets the field or property of the specified target.
		/// </summary>
		/// <param name="target">The target to act on.</param>
		/// <param name="name">The name of the field or property.</param>
		/// <returns>The retrieved field or property.</returns>
		public static object GetFieldOrProperty(object target, string name)
		{
			if (target == null) throw new ArgumentNullException("target");

			return target.GetType().InvokeMember(name, BindingFlagsGet, null, target, null);
		}

		/// <summary>
		/// Sets the field or property of the specified target.
		/// </summary>
		/// <param name="target">The target to act on.</param>
		/// <param name="name">The name of the field or property.</param>
		/// <param name="value">The value to set.</param>
		public static void SetFieldOrProperty(object target, string name, object value)
		{
			if (target == null) throw new ArgumentNullException("target");

			target.GetType().InvokeMember(name, BindingFlagsSet, null, target, new object[] { value });
		}
	}
}
