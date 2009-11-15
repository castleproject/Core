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

namespace Castle.DynamicProxy.Generators
{
	using System;
	using System.Reflection;

#if SILVERLIGHT
#else
	[Serializable]
#endif
	public class CacheKey
	{
		private readonly MemberInfo target;
		private readonly Type[] interfaces;
		private readonly ProxyGenerationOptions options;
		private readonly Type proxyForType;

		/// <summary>
		/// Initializes a new instance of the <see cref="CacheKey"/> class.
		/// </summary>
		/// <param name="target">Type of the target.</param>
		/// <param name="interfaces">The interfaces.</param>
		/// <param name="options">The options.</param>
		public CacheKey(MemberInfo target, Type[] interfaces, ProxyGenerationOptions options)
		{
			this.target = target;
			this.interfaces = interfaces ?? Type.EmptyTypes;
			this.options = options;
		}

		public CacheKey(Type proxyForType, Type targetType, Type[] interfaces, ProxyGenerationOptions options)
			: this(targetType, interfaces, options)
		{
			this.proxyForType = proxyForType;
		}

		public override int GetHashCode()
		{
			int result = target.GetHashCode();
			foreach (Type inter in interfaces)
			{
				result += 29 + inter.GetHashCode();
			}
			if (options != null)
				result = 29*result + options.GetHashCode();
			if (proxyForType != null)
				result = 29*result + proxyForType.GetHashCode();
			return result;
		}

		public override bool Equals(object obj)
		{
			if (this == obj) return true;

			CacheKey cacheKey = obj as CacheKey;
			if (cacheKey == null) return false;

			if (!Equals(proxyForType, cacheKey.proxyForType)) return false;
			if (!Equals(target, cacheKey.target)) return false;
			if (interfaces.Length != cacheKey.interfaces.Length) return false;
			for (int i = 0; i < interfaces.Length; i++)
			{
				if (!Equals(interfaces[i], cacheKey.interfaces[i])) return false;
			}
			if (!Equals(options, cacheKey.options)) return false;
			return true;
		}
	}
}
