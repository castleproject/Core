// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core
{
	using System;

	/// <summary>
	/// Associates a custom component with a component
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class ComponentActivatorAttribute : Attribute
	{
		private readonly Type componentActivatorType;

		/// <summary>
		/// Initializes a new instance of the <see cref="ComponentActivatorAttribute"/> class.
		/// </summary>
		/// <param name="componentActivatorType">Type of the component activator.</param>
		public ComponentActivatorAttribute(Type componentActivatorType)
		{
			this.componentActivatorType = componentActivatorType;
		}

		/// <summary>
		/// Gets the type of the component activator.
		/// </summary>
		/// <value>The type of the component activator.</value>
		public Type ComponentActivatorType
		{
			get { return componentActivatorType; }
		}
	}
}