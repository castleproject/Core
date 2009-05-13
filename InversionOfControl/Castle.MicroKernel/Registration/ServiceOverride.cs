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

namespace Castle.MicroKernel.Registration
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Represents a service override.
	/// </summary>
	public class ServiceOverride : Property
	{
		private readonly Type type;

		internal ServiceOverride(String key, object value)
			: base(key, value)
		{
		}

		internal ServiceOverride(String key, object value, Type type)
			: base(key, value)
		{
			this.type = type;
		}

		/// <summary>
		/// Gets the optional value type specifier.
		/// </summary>
		public Type Type
		{
			get { return type; }
		}

		/// <summary>
		/// Creates a <see cref="ServiceOverrideKey"/> with key.
		/// </summary>
		/// <param name="key">The service override key.</param>
		/// <returns>The new <see cref="ServiceOverrideKey"/></returns>
		public new static ServiceOverrideKey ForKey(String key)
		{
			return new ServiceOverrideKey(key);
		}
	}

	/// <summary>
	/// Represents a service override key.
	/// </summary>
	public class ServiceOverrideKey
	{
		private readonly String name;

		internal ServiceOverrideKey(String name)
		{
			this.name = name;
		}

		/// <summary>
		/// Gets the service override key name.
		/// </summary>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// Builds the <see cref="ServiceOverride"/> with key/value.
		/// </summary>
		/// <param name="value">The service overeride value.</param>
		/// <returns>The new <see cref="ServiceOverride"/></returns>
		public ServiceOverride Eq(String value)
		{
			return new ServiceOverride(name, value);
		}

		/// <summary>
		/// Builds the <see cref="ServiceOverride"/> with key/values.
		/// </summary>
		/// <param name="value">The service overeride values.</param>
		/// <returns>The new <see cref="ServiceOverride"/></returns>
		public ServiceOverride Eq(params String[] value)
		{
			return new ServiceOverride(name, value);
		}

		/// <summary>
		/// Builds the <see cref="ServiceOverride"/> with key/values.
		/// </summary>
		/// <param name="value">The service overeride values.</param>
		/// <returns>The new <see cref="ServiceOverride"/></returns>
		/// <typeparam name="V">The value type.</typeparam>
		public ServiceOverride Eq<V>(params String[] value)
		{
			return new ServiceOverride(name, value, typeof(V));
		}

		/// <summary>
		/// Builds the <see cref="ServiceOverride"/> with key/values.
		/// </summary>
		/// <param name="value">The service overeride values.</param>
		/// <returns>The new <see cref="ServiceOverride"/></returns>
		public ServiceOverride Eq(IEnumerable<String> value)
		{
			return new ServiceOverride(name, value);
		}

		/// <summary>
		/// Builds the <see cref="ServiceOverride"/> with key/values.
		/// </summary>
		/// <param name="value">The service overeride values.</param>
		/// <returns>The new <see cref="ServiceOverride"/></returns>
		/// <typeparam name="V">The value type.</typeparam>
		public ServiceOverride Eq<V>(IEnumerable<String> value)
		{
			return new ServiceOverride(name, value, typeof(V));
		}
	}
}