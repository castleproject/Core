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

namespace Castle.MonoRail.Framework
{
	using System;

	/// <summary>
	/// Enum (flag) to indicate when the filter should 
	/// or is invoked.
	/// </summary>
	[Flags]
	public enum ExecuteWhen
	{
		/// <summary>
		/// The filter is invoked before the action.
		/// </summary>
		[Obsolete("Use ExecuteWhen.BeforeAction")]
		Before = BeforeAction,
		/// <summary>
		/// The filter is invoked after the action.
		/// </summary>
		[Obsolete("Use ExecuteWhen.AfterRendering or ExecuteWhen.AfterAction")]
		After = AfterRendering,
		/// <summary>
		/// The filter is invoked before and after the action.
		/// </summary>
		[Obsolete("Use ExecuteWhen.Always or combine the ExecuteWhen values you want")]
		Around = BeforeAction | AfterAction,

		/// <summary>
		/// The filter is invoked before the action.
		/// </summary>
		BeforeAction = 0x02,
		/// <summary>
		/// The filter is invoked after the action.
		/// </summary>
		AfterAction = 0x04,
		/// <summary>
		/// The filter is invoked after the rendering.
		/// </summary>
		AfterRendering = 0x08,
		/// <summary>
		/// The filter is invoked around all steps.
		/// </summary>
		Always = BeforeAction | AfterAction | AfterRendering
	}

	/// <summary>
	/// Dictates the contract for filters. Implementors 
	/// should use filter to perform any logic before and/or
	/// after the action invocation.
	/// </summary>
	public interface IFilter
	{
		/// <summary>
		/// Implementors should perform they filter logic and
		/// return <c>true</c> if the action should be processed.
		/// </summary>
		/// <param name="exec">When this filter is being invoked</param>
		/// <param name="context">Current context</param>
		/// <param name="controller">The controller instance</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <returns>
		/// 	<c>true</c> if the action
		/// should be invoked, otherwise <c>false</c>
		/// </returns>
		bool Perform(ExecuteWhen exec, IEngineContext context, IController controller, IControllerContext controllerContext);
	}

	/// <summary>
	/// Dictates a contract that the defining 
	/// FilterAttribute can be set
	/// </summary>
	public interface IFilterAttributeAware
	{
		/// <summary>
		/// Sets the filter.
		/// </summary>
		/// <value>The filter.</value>
		FilterAttribute Filter { set; }
	}
}
