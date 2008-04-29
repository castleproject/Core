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
	/// Represents the meta information and type of
	/// an implementation of <see cref="IDynamicActionProvider"/>.
	/// </summary>
	public class DynamicActionProviderDescriptor : ICloneable
	{
		private readonly Type dynamicActionProviderType;
		private IDynamicActionProvider dynamicActionProviderInstance;
		private DynamicActionProviderAttribute attribute;
		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicActionProviderDescriptor"/> class.
		/// </summary>
		/// <param name="dynamicActionProviderType">Type of the provider.</param>
		/// <param name="attribute">The attribute.</param>
		public DynamicActionProviderDescriptor(Type dynamicActionProviderType, DynamicActionProviderAttribute attribute)
		{
			this.dynamicActionProviderType = dynamicActionProviderType;
			this.attribute = attribute;
		}

		/// <summary>
		/// Gets the type of the dynamic action provider.
		/// </summary>
		/// <value>The type of the dynamic action provider.</value>
		public Type DynamicActionProviderType
		{
			get { return dynamicActionProviderType; }
		}

		/// <summary>
		/// Gets or sets the dynamic action provider instance.
		/// </summary>
		/// <value>The dynamic action provider instance.</value>
		public IDynamicActionProvider DynamicActionProviderInstance
		{
			get { return dynamicActionProviderInstance; }
			set { dynamicActionProviderInstance = value; }
		}

		/// <summary>
		/// Gets the attribute.
		/// </summary>
		/// <value>The attribute.</value>
		public DynamicActionProviderAttribute Attribute
		{
			get { return attribute; }
			set { attribute = value; }
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		public object Clone()
		{
			return new DynamicActionProviderDescriptor(dynamicActionProviderType, attribute);
		}
	}
}
