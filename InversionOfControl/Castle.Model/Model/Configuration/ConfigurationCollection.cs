// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.Model.Configuration
{
	using System;
	using System.Collections;

	/// <summary>
	/// A collection of <see cref="IConfiguration"/> objects.
	/// </summary>
	[Serializable]
	public class ConfigurationCollection: ReadOnlyCollectionBase
	{
		/// <summary>
		/// Creates a new instance of <c>ConfigurationCollection</c>.
		/// </summary>
		public ConfigurationCollection()
		{
		}

		/// <summary>
		/// Creates a new instance of <c>ConfigurationCollection</c>.
		/// </summary>
		public ConfigurationCollection(ConfigurationCollection value)
		{
			this.AddRange(value);
		}

		/// <summary>
		/// Creates a new instance of <c>ConfigurationCollection</c>.
		/// </summary>
		public ConfigurationCollection(IConfiguration[] value)
		{
			this.AddRange(value);
		}

		/// <summary>
		/// Represents the entry at the specified index of the <see cref="IConfiguration"/>.
		/// </summary>
		/// <param name="index">
		/// The zero-based index of the entry to locate in the collection.
		/// </param>
		/// <value>
		/// The entry at the specified index of the collection.
		/// </value>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="index"/> is outside the valid range of indexes for the collection.
		/// </exception>
		public IConfiguration this[int index] 
		{
			get 
			{
				return (IConfiguration) InnerList[index];
			}
			set 
			{
				InnerList[index] = value;
			}
		}

		public IConfiguration this[String name] 
		{
			get 
			{
				foreach(IConfiguration config in InnerList)
				{
					if (name.Equals(config.Name))
					{
						return config;
					}
				}

				return null;
			}
		}

		/// <summary>
		/// Adds an <see cref="IConfiguration"/>.
		/// </summary>
		/// <param name="value">The <see cref="IConfiguration"/> to add.</param>
		/// <returns>
		/// The index at which the new element was inserted.
		/// </returns>
		public IConfiguration Add(IConfiguration value) 
		{
			InnerList.Add(value);
			return value;
		}

		/// <summary>
		/// Adds an array of <see cref="IConfiguration"/>.
		/// </summary>
		/// <param name="value">The Array of <see cref="IConfiguration"/> to add.</param>
		public void AddRange(IConfiguration[] value) 
		{
			foreach(IConfiguration configuration in value)
			{
				this.Add(configuration);
			}
		}

		/// <summary>
		/// Adds a <see cref="ConfigurationCollection"/>.
		/// </summary>
		/// <param name="value">The <see cref="ConfigurationCollection"/> to add.</param>
		public void AddRange(ConfigurationCollection value) 
		{
			foreach(IConfiguration configuration in value)
			{
				this.Add(configuration);
			}
		}

		/// <summary>
		/// Copies the elements to a one-dimensional <see cref="Array"/> instance at the specified index.
		/// </summary>
		/// <param name="array">
		///	The one-dimensional <see cref="Array"/> must have zero-based indexing.
		///	</param>
		/// <param name="index">The zero-based index in array at which copying begins.</param>
		public void CopyTo(IConfiguration[] array, int index)
		{
			InnerList.CopyTo(array, index);
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="IConfiguration"/> contains
		/// in the collection.
		/// </summary>
		/// <param name="value">The <see cref="IConfiguration"/> to locate.</param>
		/// <returns>
		/// <see langword="true"/> if the <see cref="IConfiguration"/> is contained in the collection; 
		/// otherwise, <see langword="false"/>.
		/// </returns>
		public bool Contains(IConfiguration value) 
		{
			return InnerList.Contains(value);
		}

		/// <summary>
		/// Removes a specific <see cref="IConfiguration"/> from the 
		/// collection.   
		/// </summary>
		/// <param name="value">The <see cref="IConfiguration"/> to remove from the collection.</param>
		/// <exception cref="ArgumentException">
		/// <paramref name="value"/> is not found in the collection.
		/// </exception>
		public void Remove(IConfiguration value) 
		{
			InnerList.Remove(value);
		}
	}
}
