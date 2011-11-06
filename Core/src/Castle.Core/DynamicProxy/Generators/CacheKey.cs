// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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

	[Serializable]
	public class CacheKey
	{
		private readonly MemberInfo target;
		private readonly Type[] interfaces;
		private readonly ProxyGenerationOptions options;
		private readonly Type type;

		/// <summary>
		///   Initializes a new instance of the <see cref = "CacheKey" /> class.
		/// </summary>
		/// <param name = "target">Target element. This is either target type or target method for invocation types.</param>
		/// <param name = "type">The type of the proxy. This is base type for invocation types.</param>
		/// <param name = "interfaces">The interfaces.</param>
		/// <param name = "options">The options.</param>
		public CacheKey(MemberInfo target, Type type, Type[] interfaces, ProxyGenerationOptions options)
		{
			this.target = target;
			this.type = type;
			this.interfaces = interfaces ?? Type.EmptyTypes;
			this.options = options;
		}

		/// <summary>
		///   Initializes a new instance of the <see cref = "CacheKey" /> class.
		/// </summary>
		/// <param name = "target">Type of the target.</param>
		/// <param name = "interfaces">The interfaces.</param>
		/// <param name = "options">The options.</param>
		public CacheKey(Type target, Type[] interfaces, ProxyGenerationOptions options)
			: this(target, null, interfaces, options)
		{
		}

		public override int GetHashCode()
		{
			var result = target.GetHashCode();
			foreach (var inter in interfaces)
			{
				result += 29 + inter.GetHashCode();
			}
			if (options != null)
			{
				result = 29*result + options.GetHashCode();
			}
			if (type != null)
			{
				result = 29*result + type.GetHashCode();
			}
			return result;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}

			var cacheKey = obj as CacheKey;
			if (cacheKey == null)
			{
				return false;
			}

			if (!Equals(type, cacheKey.type))
			{
				return false;
			}
			if (!Equals(target, cacheKey.target))
			{
				return false;
			}
			if (interfaces.Length != cacheKey.interfaces.Length)
			{
				return false;
			}
			for (var i = 0; i < interfaces.Length; i++)
			{
				if (!Equals(interfaces[i], cacheKey.interfaces[i]))
				{
					return false;
				}
			}
			if (!Equals(options, cacheKey.options))
			{
				return false;
			}
			return true;
		}
	}
}