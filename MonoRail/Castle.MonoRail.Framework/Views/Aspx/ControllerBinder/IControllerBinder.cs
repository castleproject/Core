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

#if NET

namespace Castle.MonoRail.Framework.Views.Aspx
{
	using System.Web.UI;

	/// <summary>
	/// Pendent
	/// </summary>
	public interface IControllerBinder : IBindingScope
	{
		/// <summary>
		/// Gets a value indicating whether [design mode].
		/// </summary>
		/// <value><c>true</c> if [design mode]; otherwise, <c>false</c>.</value>
		bool DesignMode { get; }

		/// <summary>
		/// Gets the controller bindings.
		/// </summary>
		/// <value>The controller bindings.</value>
		ControllerBindingCollection ControllerBindings { get; }

		/// <summary>
		/// Gets the action arguments.
		/// </summary>
		/// <value>The action arguments.</value>
		ActionArgumentCollection ActionArguments { get; }

		/// <summary>
		/// Determines whether [is bindable control] [the specified control].
		/// </summary>
		/// <param name="control">The control.</param>
		/// <returns>
		/// 	<c>true</c> if [is bindable control] [the specified control]; otherwise, <c>false</c>.
		/// </returns>
		bool IsBindableControl(Control control);

		/// <summary>
		/// Adds the binding.
		/// </summary>
		/// <param name="control">The control.</param>
		/// <returns></returns>
		ControllerBinding AddBinding(Control control);

		/// <summary>
		/// Finds the control with ID.
		/// </summary>
		/// <param name="controlID">The control ID.</param>
		/// <returns></returns>
		Control FindControlWithID(string controlID);

		/// <summary>
		/// Gets the controller actions.
		/// </summary>
		/// <returns></returns>
		string[] GetControllerActions();
	}
}

#endif
