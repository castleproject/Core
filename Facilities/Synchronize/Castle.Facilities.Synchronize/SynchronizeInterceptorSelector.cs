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

namespace Castle.Facilities.Synchronize
{
	using System;
	using System.Linq;
	using System.Reflection;
	using Castle.Core.Interceptor;

	/// <summary>
	/// Selects which methods require synchronization.
	/// </summary>
	[Serializable]
	public class SynchronizeInterceptorSelector : IInterceptorSelector
	{
		private readonly SynchronizeMetaInfo metaInfo;
		private readonly IInterceptorSelector existingSelector;

		/// <summary>
		/// Contructs the selector with the existing selector.
		/// </summary>
		/// <param name="metaInfo">The sync metadata.</param>
		/// <param name="existingSelector">The existing selector.</param>
		public SynchronizeInterceptorSelector(SynchronizeMetaInfo metaInfo,
											  IInterceptorSelector existingSelector)
		{
			this.metaInfo = metaInfo;
			this.existingSelector = existingSelector;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="method"></param>
		/// <param name="interceptors"></param>W
		/// <returns></returns>
		public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
		{
			if (metaInfo.Contains(method))
				return interceptors;

			if (IsInterfaceMappingCandidate(type, method))
			{
				var map = type.GetInterfaceMap(method.DeclaringType);
				var index = Array.IndexOf(map.InterfaceMethods, method);
				if (index >= 0 && metaInfo.Contains(map.TargetMethods[index]))
					return interceptors;
			}

			return interceptors.Where(i => !(i is SynchronizeInterceptor)).ToArray();
		}

		private bool IsInterfaceMappingCandidate(Type type, MemberInfo method)
		{
			return (type != method.DeclaringType && method.DeclaringType.IsInterface);
		}
	}
}
