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
	using System.Collections;

	/// <summary>
	/// Contract for implementation of cache key generators.
	/// </summary>
	/// 
	/// <remarks>
	/// Implementors should use the parameters to the method <see cref="Create"/>
	/// to return an immutable representation of a cache key. 
	/// </remarks>
	public interface IViewComponentCacheKeyGenerator
	{
		/// <summary>
		/// Creates a cache key that uniquely relates to the parameters specified. 
		/// </summary>
		/// <param name="viewComponentName">Name of the view component.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		CacheKey Create(string viewComponentName, IDictionary parameters, IRailsEngineContext context);
	}
}
