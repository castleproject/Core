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
	/// Represents the information about a Helper class
	/// associated with a <see cref="IController"/>
	/// </summary>
	public class HelperDescriptor
	{
		private readonly String name;
		private readonly Type helperType;

		/// <summary>
		/// Initializes a new instance of the <see cref="HelperDescriptor"/> class.
		/// </summary>
		/// <param name="helperType">Type of the helper.</param>
		public HelperDescriptor(Type helperType)
		{
			this.helperType = helperType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HelperDescriptor"/> class.
		/// </summary>
		/// <param name="helperType">Type of the helper.</param>
		/// <param name="name">A custom name to use to access the helper from the view.</param>
		public HelperDescriptor(Type helperType, String name) : this(helperType)
		{
			this.name = name;
		}

		/// <summary>
		/// Gets the helper name.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// Gets the type of the helper.
		/// </summary>
		/// <value>The type of the helper.</value>
		public Type HelperType
		{
			get { return helperType; }
		}
	}
}
