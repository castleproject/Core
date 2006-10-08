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

	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Associates a helper class with the controller.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true), Serializable]
	public class HelperAttribute : Attribute, IHelperDescriptorBuilder
	{
		private readonly Type helperType;
		private readonly String name;

		/// <summary>
		/// Constructs a <see cref="HelperAttribute"/> 
		/// with the supplied <c>helperType</c>.
		/// </summary>
		/// <param name="helperType">The helper type</param>
		public HelperAttribute(Type helperType) : this(helperType, null)
		{			
		}

		/// <summary>
		/// Constructs a <see cref="HelperAttribute"/> 
		/// with the supplied <c>helperType</c> and a name to be bound to it.
		/// </summary>
		/// <param name="helperType">The helper type</param>
		/// <param name="name">Name bound to the helper. The name will be
		/// used on the view to gain access to it</param>
		public HelperAttribute(Type helperType, String name)
		{
			this.helperType = helperType;
			this.name = (name == null || name.Trim() == String.Empty) ? helperType.Name : name;
		}

		/// <summary>
		/// Gets Name bound to the helper. The name will be
		/// used on the view to gain access to it
		/// </summary>
		public String Name
		{
			get { return name; }
		}		

		/// <summary>
		/// Gets the helper type
		/// </summary>
		public Type HelperType
		{
			get { return helperType; }
		}

		/// <summary>
		/// <see cref="IHelperDescriptorBuilder"/> implementation.
		/// Gets the <seealso cref="HelperDescriptor"/>
		/// that describes the helper.
		/// </summary>
		/// <returns>The descriptor instance</returns>
		public HelperDescriptor[] BuildHelperDescriptors()
		{
			return new HelperDescriptor[] { new HelperDescriptor(helperType, name) };
		}
	}
}
