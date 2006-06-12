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

namespace Castle.DynamicProxy.Generators
{
	using System;

	public class CacheKey
	{
		private readonly Type targetType;
		private readonly Type[] interfaces;
		private readonly ProxyGenerationOptions options;

		public CacheKey(Type targetType, Type[] interfaces, ProxyGenerationOptions options)
		{
			this.targetType = targetType;
			this.interfaces = interfaces;
			this.options = options;
		}

		public override int GetHashCode()
		{
			int result = targetType.GetHashCode();
			// result = 29 * result + (interfaces != null ? interfaces.GetHashCode() : 0);
			if (interfaces != null)
			{
				foreach (Type inter in interfaces)
					result += 29 + inter.GetHashCode();
			}
			result = 29 * result + options.GetHashCode();
			return result;
		}

		public override bool Equals(object obj)
		{
			if (this == obj) return true;
			CacheKey cacheKey = obj as CacheKey;
			if (cacheKey == null) return false;
			if (!Equals(targetType, cacheKey.targetType)) return false;
			// if (!Equals(interfaces, cacheKey.interfaces)) return false;
			if (interfaces != null && cacheKey.interfaces == null) return false;
			if (interfaces == null && cacheKey.interfaces != null) return false;
			if (interfaces != null && interfaces.Length != cacheKey.interfaces.Length) return false;
			if (interfaces != null)
			{
				for(int i=0;i<interfaces.Length;i++)
				{
					if (!Equals(interfaces[i], cacheKey.interfaces[i])) return false;
				}
			}
			if (!Equals(options, cacheKey.options)) return false;
			return true;
		}
	}
}
