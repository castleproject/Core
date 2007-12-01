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

namespace Castle.MonoRail.Framework
{
	/// <summary>
	/// Represents a Cache Key. 
	/// </summary>
	/// <remarks>
	/// Implementors just need to override the <c>ToString</c>
	/// method. The cache key should be immutable and deterministic. 
	/// </remarks>
	public abstract class CacheKey
	{
		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public abstract new string ToString();
	}

	/// <summary>
	/// A simple <see cref="CacheKey"/> implementation. 
	/// </summary>
	public class NamedCacheKey : CacheKey
	{
		private readonly string name;

		/// <summary>
		/// Initializes a new instance of the <see cref="NamedCacheKey"/> class.
		/// </summary>
		/// <param name="name">Key name/content.</param>
		public NamedCacheKey(string name)
		{
			this.name = name;
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			return name;
		}
	}
}
