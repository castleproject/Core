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

namespace Castle.MonoRail.Framework.Descriptors
{
	using System;

	/// <summary>
	/// Represents the configuration associated with a <see cref="IController"/>.
	/// </summary>
	public class ControllerDescriptor
	{
		private readonly Type controllerType;
		private readonly String name;
		private readonly String area;
		private readonly bool sessionless;

		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerDescriptor"/> class.
		/// </summary>
		/// <param name="controllerType">Type of the controller.</param>
		/// <param name="name">The name of the controller.</param>
		/// <param name="area">The area associated with the controller.</param>
		/// <param name="sessionless">if set to <c>true</c> [sessionless].</param>
		public ControllerDescriptor(Type controllerType, String name, String area, bool sessionless)
		{
			this.controllerType = controllerType;
			this.name = name;
			this.area = area;
			this.sessionless = sessionless;
		}

		/// <summary>
		/// Gets the type of the controller.
		/// </summary>
		/// <value>The type of the controller.</value>
		public Type ControllerType
		{
			get { return controllerType; }
		}

		/// <summary>
		/// Gets the controller's name.
		/// </summary>
		/// <value>The name.</value>
		public String Name
		{
			get { return name; }
		}

		/// <summary>
		/// Gets the controller's area.
		/// </summary>
		/// <value>The area.</value>
		public String Area
		{
			get { return area; }
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="IController"/> needs a session.
		/// </summary>
		/// <value><c>true</c> if sessionless; otherwise, <c>false</c>.</value>
		public bool Sessionless
		{
			get { return sessionless; }
		}
	}
}
