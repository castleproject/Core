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

#if NET

namespace Castle.MonoRail.Framework.Views.Aspx.Design
{
	using System;
	using System.ComponentModel;

	/// <summary>
	/// Pendent
	/// </summary>
	public class StandardTarget : ITarget
	{
		private readonly Type type;
		private readonly string name;
		private string[] propertyNames;

		/// <summary>
		/// Initializes a new instance of the <see cref="StandardTarget"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		public StandardTarget(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			this.name = name;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StandardTarget"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="name">The name.</param>
		public StandardTarget(Type type, string name) : this(name)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}

			this.type = type;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StandardTarget"/> class.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="name">The name.</param>
		public StandardTarget(object instance, string name) : this(name)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}

			type = instance.GetType();
		}

		/// <summary>
		/// Gets the property names.
		/// </summary>
		/// <value>The property names.</value>
		public string[] PropertyNames
		{
			get
			{
				if (propertyNames == null)
				{
					propertyNames = CollectPropertyNames(type);
					Array.Sort(propertyNames);
				}
				return propertyNames;
			}
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </returns>
		public override string ToString()
		{
			return name;	
		}

		/// <summary>
		/// Collects the property names.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		protected virtual string[] CollectPropertyNames(Type type)
		{
			if (type == null) return new string[0];

			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(type);

			string[] propertyDescNames = new string[properties.Count];

			for (int i = 0; i < properties.Count; ++i)
			{
				propertyDescNames[i] = properties[i].Name;
			}

			return propertyDescNames;
		}
	}
}

#endif
