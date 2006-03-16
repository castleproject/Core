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

namespace Castle.MonoRail.Framework.Internal
{
	using System;
	using System.Web;

	public class DefaultCacheProvider : ICacheProvider
	{
		public void Init(IServiceProvider serviceProvider)
		{
		}

		public bool HasKey(String key)
		{
			return Get(key) != null;
		}

		public object Get(String key)
		{
			return GetCurrentContext().Cache.Get(key);
		}

		public void Store(String key, object data)
		{
			GetCurrentContext().Cache.Insert(key, data);
		}

		public void Delete(String key)
		{
			GetCurrentContext().Cache.Remove(key);
		}

		private HttpContext GetCurrentContext()
		{
			return HttpContext.Current;
		}
	}
}
