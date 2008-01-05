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
	/// Decorates a controller with a different name
	/// and optionally an area which the controller belongs to.
	/// This is used to override the convention for controller
	/// names and to optionally associate a controller with an
	/// area name.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class), Serializable]
	public class ControllerDetailsAttribute : Attribute
	{
		private String name;
		private String area = String.Empty;
		private bool sessionless = false;

		/// <summary>
		/// Constructs a ControllerDetailsAttribute
		/// </summary>
		public ControllerDetailsAttribute()
		{
		}

		/// <summary>
		/// Constructs a ControllerDetailsAttribute
		/// with a name for the controller.
		/// </summary>
		/// <param name="name">The specified Controller Name</param>
		public ControllerDetailsAttribute( String name )
		{
			this.name = name;
		}

		/// <summary>
		/// The controller's name
		/// </summary>
		public String Name
		{
			get { return name; }
		}

		/// <summary>
		/// The controller's area
		/// </summary>
		public String Area
		{
			get { return area; }
			set { area = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the controller does not need a session. 
		/// Defaults to <c>false</c>.
		/// </summary>
		/// <value><c>true</c> if sessionless; otherwise, <c>false</c>.</value>
		public bool Sessionless
		{
			get { return sessionless; }
			set { sessionless = value; }
		}
	}
}
