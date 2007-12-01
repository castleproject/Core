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

namespace TestSiteNVelocity.Components
{
	using System;
	using System.Collections;
	using Castle.MonoRail.Framework;

	[ViewComponentDetails("CachedComponent1", Cache=ViewComponentCache.Always)]
	public class CachedComponent1 : ViewComponent
	{
		public override void Render()
		{
			PropertyBag["ticks"] = DateTime.Now.Ticks;

			base.Render();
		}
	}

	public class MyCacheKeyFactory : IViewComponentCacheKeyGenerator
	{
		public CacheKey Create(string viewComponentName, IDictionary parameters, IRailsEngineContext context)
		{
			return new MyCacheKey();
		}

		public class MyCacheKey : CacheKey
		{
			public override string ToString()
			{
				return "a";
			}
		}
	}

	[ViewComponentDetails("CachedComponent2", Cache = ViewComponentCache.Always)]
	public class CachedComponent2 : ViewComponent
	{
		public override void Render()
		{
			RenderBody();
		}
	}

	[ViewComponentDetails("CachedComponent3", Cache = ViewComponentCache.Always)]
	public class CachedComponent3 : ViewComponent
	{
		public override void Render()
		{
			RenderBody();
		}
	}
}
