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

namespace Castle.MonoRail.Framework.Descriptors
{
	using Castle.MonoRail.Framework;

	/// <summary>
	/// Represents a <see cref="ViewComponent"/> cache configuration.
	/// </summary>
	public class ViewComponentDescriptor
	{
		/// <summary>
		/// Represents an empty descriptor
		/// </summary>
		public static readonly ViewComponentDescriptor Empty = new ViewComponentDescriptor(false, ViewComponentCache.Disabled, null);

		private readonly bool isCacheable;
		private readonly ViewComponentCache cacheStrategy;
		private readonly IViewComponentCacheKeyGenerator cacheKeyGenerator;

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewComponentDescriptor"/> class.
		/// </summary>
		/// <param name="isCacheable">if set to <c>true</c> [is cacheable].</param>
		/// <param name="cacheStrategy">The cache strategy.</param>
		/// <param name="cacheKeyGenerator">The cache key generator.</param>
		public ViewComponentDescriptor(bool isCacheable, ViewComponentCache cacheStrategy, IViewComponentCacheKeyGenerator cacheKeyGenerator)
		{
			this.isCacheable = isCacheable;
			this.cacheStrategy = cacheStrategy;
			this.cacheKeyGenerator = cacheKeyGenerator;
		}

		/// <summary>
		/// Gets a value indicating whether the view component is cacheable.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the View Component is cacheable; otherwise, <c>false</c>.
		/// </value>
		public bool IsCacheable
		{
			get { return isCacheable; }
		}

		/// <summary>
		/// Gets the cache strategy.
		/// </summary>
		/// <value>The cache strategy.</value>
		public ViewComponentCache CacheStrategy
		{
			get { return cacheStrategy; }
		}

		/// <summary>
		/// Gets the cache key generator.
		/// </summary>
		/// <value>The cache key generator.</value>
		public IViewComponentCacheKeyGenerator CacheKeyGenerator
		{
			get { return cacheKeyGenerator; }
		}
	}
}
