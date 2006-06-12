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
	using System.Reflection;

	abstract class Constants
	{
		internal static ConstructorInfo AbstractInvocationConstructor = 
			typeof(AbstractInvocation).GetConstructor(BindingFlags.Instance|BindingFlags.NonPublic, 
				null, new Type[] { typeof(IInterceptor[]), typeof(Type), typeof(MethodInfo), typeof(object[]) }, null);

		internal static MethodInfo AbstractInvocationProceed =
			typeof(AbstractInvocation).GetMethod("Proceed", BindingFlags.Instance|BindingFlags.Public);

		internal static MethodInfo GetMethodFromHandle1 =
			typeof(MethodBase).GetMethod(
				"GetMethodFromHandle", BindingFlags.Static | BindingFlags.Public, null,
				new Type[] { typeof(RuntimeMethodHandle) }, null);

#if DOTNET2
		internal static MethodInfo GetMethodFromHandle2 =
			typeof(MethodBase).GetMethod(
				"GetMethodFromHandle", BindingFlags.Static | BindingFlags.Public, null,
				new Type[] { typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle) }, null);
#endif
	}
}
