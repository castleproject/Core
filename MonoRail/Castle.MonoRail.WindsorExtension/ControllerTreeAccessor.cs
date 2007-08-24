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

namespace Castle.MonoRail.WindsorExtension
{
	using System;
	using Castle.MonoRail.Framework;

	/// <summary>
	/// Bridge between the windsor controlled controller tree and
	/// the monorail service provider.
	/// </summary>
	public class ControllerTreeAccessor : IControllerTree
	{
		private IControllerTree tree;

		/// <summary>
		/// Construct the controller tree accessor
		/// </summary>
		public ControllerTreeAccessor()
		{
			tree = WindsorContainerAccessorUtil.ObtainContainer().Resolve(typeof(IControllerTree)) as IControllerTree;
		}

		#region IControllerTree Members

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
		public void AddController(string areaName, string controllerName, Type controller)
		{
			tree.AddController(areaName, controllerName, controller);
		}

		/// <summary>
		/// Returns a controller previously registered. 
		/// </summary>
		/// <param name="areaName">The area name, or <c>String.Empty</c></param>
		/// <param name="controllerName">The controller name</param>
		/// <returns>The controller representation or null</returns>
		public Type GetController(string areaName, string controllerName)
		{
			return tree.GetController(areaName, controllerName);
		}

		#endregion
	}
}