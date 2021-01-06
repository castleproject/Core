// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy
{
	using System.Reflection;

	using Castle.DynamicProxy.Serialization;

	/// <summary>
	///   A <see cref="IProxyBuilder"/> that caches generated proxy types for reuse.
	/// </summary>
	public interface IProxyBuilderWithCache : IProxyBuilder
	{
#if FEATURE_SERIALIZATION
		/// <summary>
		///   Loads the generated types from the given assembly into this builder's cache.
		/// </summary>
		/// <param name="assembly">
		///   The assembly to load types from.
		///   <para>
		///     This assembly must have been saved via <see cref="T:Castle.DynamicProxy.PersistentProxyBuilder" />
		///     (which is only available on platforms that support saving generated assemblies to disk), or
		///     it must have the <see cref="CacheMappingsAttribute" /> manually applied.
		///   </para>
		/// </param>
		/// <remarks>
		///   This method can be used to load previously generated and persisted proxy types from disk
		///   into this builder's type cache, e.g. in order to avoid the performance hit
		///   associated with proxy type generation.
		/// </remarks>
		void LoadAssemblyIntoCache(Assembly assembly);
#endif
	}
}
