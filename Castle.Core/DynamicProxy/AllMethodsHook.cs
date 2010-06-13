// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

using System.Collections.Generic;

namespace Castle.DynamicProxy
{
	using System;
	using System.Reflection;

#if !SILVERLIGHT
	[Serializable]
#endif
	public class AllMethodsHook : IProxyGenerationHook
	{
		private static readonly ICollection<Type> SkippedTypes = new[]
		                                              	{
		                                              		typeof (object),
#if !SILVERLIGHT
		                                              		typeof (MarshalByRefObject),
		                                              		typeof (ContextBoundObject)
#endif
		                                              	};

		public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
		{
			return SkippedTypes.Contains(methodInfo.DeclaringType) == false;
		}

		public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
		{
		}

		public void MethodsInspected()
		{
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj.GetType() == typeof (AllMethodsHook);
		}

		public override int GetHashCode()
		{
			return GetType().GetHashCode();
		}
	}
}
