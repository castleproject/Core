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
	using System.Collections.Specialized;

	/// <summary>
	/// Default implementation of <see cref="IControllerTree"/>.
	/// Represents an binary tree of registered controllers.
	/// <para>
	/// It is used by the controller factory to resolve a controller instance
	/// based on the specified area (which is optional) and controller name
	/// </para>
	/// <seealso cref="IControllerTree"/>
	/// <seealso cref="Castle.MonoRail.Framework.Services.AbstractControllerFactory"/>
	/// </summary>
	public sealed class DefaultControllerTree : IControllerTree
	{
		/// <summary>
		/// The area the controller belongs to. 
		/// The default area is <c>String.Empty</c>
		/// </summary>
		private readonly String area;

		/// <summary>
		/// A dictionary of controllers that belongs to this node (area)
		/// </summary>
		private readonly IDictionary controllers;

		/// <summary>
		/// The controllers node on the left
		/// </summary>
		private DefaultControllerTree left;

		/// <summary>
		/// The controllers node on the right
		/// </summary>
		private DefaultControllerTree right;

		/// <summary>
		/// Occurs when a controller is added to the controller tree.
		/// </summary>
		public event EventHandler<ControllerAddedEventArgs> ControllerAdded;

		/// <summary>
		/// Constructs a <c>ControllerTree</c> with an empty area
		/// </summary>
		public DefaultControllerTree() : this(String.Empty)
		{
		}

		/// <summary>
		/// Constructs a <c>ControllerTree</c> specifying an area
		/// </summary>
		public DefaultControllerTree(String areaName)
		{
			if (areaName == null) throw new ArgumentNullException("areaName");

			area = areaName;
			controllers = new HybridDictionary(true);
		}

		private void OnControllerAdded(String area, String controllerName, Type controller)
		{
			if (ControllerAdded != null)
			{
				ControllerAddedEventArgs args = new ControllerAddedEventArgs(area, controllerName, controller);
				ControllerAdded(this, args);
			}
		}

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
		public void AddController(String areaName, String controllerName, Type controller)
		{
			if (areaName == null) throw new ArgumentNullException("areaName");
			if (controllerName == null) throw new ArgumentNullException("controllerName");
			if (controller == null) throw new ArgumentNullException("controller");

			int cmp = String.Compare(areaName, area, true);

			if (cmp == 0)
			{
				// If it's the same area, register the controller
				controllers[controllerName] = controller;
			}
			else
			{
				// Otherwise, check if the controller should be registered
				// on the left or on the right

				DefaultControllerTree node;

				if (cmp < 0)
				{
					if (left == null)
					{
						left = new DefaultControllerTree(areaName);
					}
					node = left;
				}
				else
				{
					if (right == null)
					{
						right = new DefaultControllerTree(areaName);
					}
					node = right;
				}

				node.AddController(areaName, controllerName, controller);
				OnControllerAdded(areaName, controllerName, controller);
			}
		}

		/// <summary>
		/// Returns a controller previously registered. 
		/// </summary>
		/// <param name="areaName">The area name, or <c>String.Empty</c></param>
		/// <param name="controllerName">The controller name</param>
		/// <returns>The controller representation or null</returns>
		public Type GetController(String areaName, String controllerName)
		{
			if (areaName == null) throw new ArgumentNullException("areaName");
			if (controllerName == null) throw new ArgumentNullException("controllerName");

			int cmp = String.Compare(areaName, area, true);

			if (cmp == 0)
			{
				return (Type) controllers[controllerName];
			}
			else
			{
				DefaultControllerTree node;

				if (cmp < 0)
				{
					node = left;
				}
				else
				{
					node = right;
				}

				if (node != null)
				{
					return node.GetController(areaName, controllerName);
				}
			}

			return null;
		}
	}

	/// <summary>
	/// Event class that represents details of the controller added
	/// to the controller tree
	/// </summary>
	public sealed class ControllerAddedEventArgs : System.EventArgs
	{
		private readonly String area;
		private readonly String controllerName;
		private readonly Type controllerType;

		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerAddedEventArgs"/> class.
		/// </summary>
		/// <param name="area">The area.</param>
		/// <param name="controllerName">Name of the controller.</param>
		/// <param name="controllerType">Type of the controller.</param>
		public ControllerAddedEventArgs(string area, string controllerName, Type controllerType)
		{
			this.area = area;
			this.controllerName = controllerName;
			this.controllerType = controllerType;
		}

		/// <summary>
		/// Gets the area.
		/// </summary>
		/// <value>The area.</value>
		public string Area
		{
			get { return area; }
		}

		/// <summary>
		/// Gets the name of the controller.
		/// </summary>
		/// <value>The name of the controller.</value>
		public string ControllerName
		{
			get { return controllerName; }
		}

		/// <summary>
		/// Gets the type of the controller.
		/// </summary>
		/// <value>The type of the controller.</value>
		public Type ControllerType
		{
			get { return controllerType; }
		}
	}
}