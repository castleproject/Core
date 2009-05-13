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

	/// <summary>
	/// Represents a key/value pair.
	/// </summary>
	public class Property
	{
		private readonly String key;
		private readonly object value;

		internal Property(String key, Object value)
		{
			this.key = key;
			this.value = value;
		}

		/// <summary>
		/// Gets the property key.
		/// </summary>
		public string Key
		{
			get { return key; }
		}

		/// <summary>
		/// Gets the property value.
		/// </summary>
		public object Value
		{
			get { return value; }
		}

		/// <summary>
		/// Create a <see cref="PropertyKey"/> with key.
		/// </summary>
		/// <param name="key">The property key.</param>
		/// <returns>The new <see cref="PropertyKey"/></returns>
		public static PropertyKey ForKey(String key)
		{
			return new PropertyKey(key);
		}
	}

	/// <summary>
	/// Represents a property key.
	/// </summary>
	public class PropertyKey
	{
		private readonly String name;

		internal PropertyKey(String name)
		{
			this.name = name;
		}

		/// <summary>
		/// The property key name.
		/// </summary>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// Builds the <see cref="Property"/> with key/value.
		/// </summary>
		/// <param name="value">The property value.</param>
		/// <returns>The new <see cref="Property"/></returns>
		public Property Eq(Object value)
		{
			return new Property(name, value);
		}
	}
}