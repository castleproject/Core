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

namespace Castle.DynamicProxy.Internal
{
	using System;
	using System.ComponentModel;
	using System.Reflection;

	public static class InternalsUtil
	{
		/// <summary>
		///   Determines whether the specified method is internal.
		/// </summary>
		/// <param name = "method">The method.</param>
		/// <returns>
		///   <c>true</c> if the specified method is internal; otherwise, <c>false</c>.
		/// </returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete]
		public static bool IsInternal(this MethodBase method)
		{
			return ProxyUtil.IsInternal(method);
		}

		/// <summary>
		///   Determines whether this assembly has internals visible to dynamic proxy.
		/// </summary>
		/// <param name = "asm">The assembly to inspect.</param>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete]
		public static bool IsInternalToDynamicProxy(this Assembly asm)
		{
			return ProxyUtil.AreInternalsVisibleToDynamicProxy(asm);
		}

		/// <summary>
		///   Checks if the method is public or protected.
		/// </summary>
		/// <param name = "method"></param>
		/// <returns></returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use " + nameof(ProxyUtil) + "." + nameof(ProxyUtil.IsAccessible) + " instead, " +
		          "which performs a more accurate accessibility check.")]
		public static bool IsAccessible(this MethodBase method)
		{
			return ProxyUtil.IsAccessibleMethod(method);
		}
	}
}