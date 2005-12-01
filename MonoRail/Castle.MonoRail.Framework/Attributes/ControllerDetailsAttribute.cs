// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
	/// and optionaly an area which the controller belongs.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class), Serializable]
	public class ControllerDetailsAttribute : Attribute
	{
		private String _name;
		private String _area = String.Empty;

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
			_name = name;
		}

		/// <summary>
		/// The controller's name
		/// </summary>
		public String Name
		{
			get { return _name; }
		}

		/// <summary>
		/// The controller's area
		/// </summary>
		public String Area
		{
			get { return _area; }
			set { _area = value; }
		}
	}
}
