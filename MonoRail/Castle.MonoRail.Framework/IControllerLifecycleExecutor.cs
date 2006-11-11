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

	/// <summary>
	/// Manages the execution of a controller action.
	/// <para>
	/// The order of methods invocation is the following:
	/// </para>
	/// <para>
	/// 1. InitializeController
	/// </para>
	/// <para>
	/// 2. SelectAction
	/// </para>
	/// <para>
	/// 3. RunStartRequestFilters (if false is returned - or an exception - 
	/// you might want to invoke PerformErrorHandling)
	/// </para>
	/// <para>
	/// 4. ProcessSelectedAction
	/// </para>
	/// <para>
	/// 5. Dispose
	/// </para>
	/// </summary>
	public interface IControllerLifecycleExecutor : IDisposable
	{
		/// <summary>
		/// Should bring the controller to an usable
		/// state by populating its fields with values that
		/// represent the current request
		/// </summary>
		/// <param name="action">The action name</param>
		/// <param name="area">The area name</param>
		/// <param name="controller">The controller name</param>
		void InitializeController(string area, string controller, string action);
		
		/// <summary>
		/// Should resolve the action to be executed (method or dynamic
		/// action) based on the parameters
		/// </summary>
		/// <param name="action">The action name</param>
		/// <param name="controller">The controller name</param>
		/// <returns><c>true</c> if it was able to resolve it</returns>
		bool SelectAction(string action, string controller);
		
		/// <summary>
		/// Runs the action (or the dynamic action),
		/// process the rescue or the view accordingly 
		/// to the process result.
		/// </summary>
		/// <param name="actionArgs">Custom arguments</param>
		void ProcessSelectedAction(params object[] actionArgs);

		/// <summary>
		/// Should performs the rescue (if available), raise
		/// the global error event and throw the exception
		/// if the rescue was not found
		/// </summary>
		void PerformErrorHandling();
		
		/// <summary>
		/// Runs the start request filters.
		/// </summary>
		/// <returns><c>false</c> if the process should be stopped</returns>
		bool RunStartRequestFilters();
				
		/// <summary>
		/// Gets the controller instance.
		/// </summary>
		/// <value>The controller.</value>
		Controller Controller { get; }

		/// <summary>
		/// Gets a value indicating whether an error has happened during controller processing
		/// </summary>
		/// <value>
		/// 	<see langword="true"/> if has error; otherwise, <see langword="false"/>.
		/// </value>
		bool HasError { get; }
	}
}
