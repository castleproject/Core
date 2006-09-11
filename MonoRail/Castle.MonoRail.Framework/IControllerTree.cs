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
	/// Represents an binary tree of registered controllers.
	/// <para>
	/// It is used by the controller factory to resolve a controller instance
	/// based on the specified area (which is optional) and controller name
	/// </para>
	/// <seealso cref="Castle.MonoRail.Framework.Services.AbstractControllerFactory"/>
	/// </summary>
	public interface IControllerTree
	{
		/// <summary>
		/// Register a controller on the tree. If the specified
		/// area name matches the current node, the controller is
		/// register on the node itself, otherwise on the right or 
		/// on the left node.
		/// </summary>
		/// <remarks>
		/// Note that the controller is an <c>object</c>. That allows
		/// different implementation of a controller factory to register
		/// different representation of what a controller is (a name, a descriptor etc)
		/// </remarks>
		/// <param name="areaName">The area name, or <c>String.Empty</c></param>
		/// <param name="controllerName">The controller name</param>
		/// <param name="controller">The controller representation</param>
		void AddController(String areaName, String controllerName, object controller);

		/// <summary>
		/// Returns a controller previously registered. 
		/// </summary>
		/// <param name="areaName">The area name, or <c>String.Empty</c></param>
		/// <param name="controllerName">The controller name</param>
		/// <returns>The controller representation or null</returns>
		object GetController(String areaName, String controllerName);
	}
}
