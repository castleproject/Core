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

namespace Castle.MonoRail.Framework.ViewComponents
{
	using System.Collections;

	/// <summary>
	/// Simple implementation of a cache key that vary only by the view component name.
	/// </summary>
	public class AlwaysCacheKeyGenerator : IViewComponentCacheKeyGenerator
	{
		/// <summary>
		/// Returns a <see cref="NamedCacheKey"/> with the view component's name.
		/// </summary>
		/// <param name="viewComponentName">Name of the view component.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public CacheKey Create(string viewComponentName, IDictionary parameters, IEngineContext context)
		{
			return new NamedCacheKey(viewComponentName);
		}
	}
}
