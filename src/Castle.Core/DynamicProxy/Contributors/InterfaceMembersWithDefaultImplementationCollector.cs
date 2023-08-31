// Copyright 2004-2023 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Contributors
{
	using System;
	using System.Diagnostics;
	using System.Reflection;

	using Castle.DynamicProxy.Generators;

	internal sealed class InterfaceMembersWithDefaultImplementationCollector : MembersCollector
	{
		private readonly InterfaceMapping map;

		public InterfaceMembersWithDefaultImplementationCollector(Type interfaceType, Type classToProxy)
			: base(interfaceType)
		{
			Debug.Assert(interfaceType != null);
			Debug.Assert(interfaceType.IsInterface);

			Debug.Assert(classToProxy != null);
			Debug.Assert(classToProxy.IsClass);

			map = classToProxy.GetInterfaceMap(interfaceType);
		}

		protected override MetaMethod GetMethodToGenerate(MethodInfo method, IProxyGenerationHook hook, bool isStandalone)
		{
			if (method.IsAbstract)
			{
				// This collector is only interested in methods with default implementations.
				// All other interface methods should be dealt with in other contributors.
				return null;
			}

			var index = Array.IndexOf(map.InterfaceMethods, method);
			Debug.Assert(index >= 0);

			var methodOnTarget = map.TargetMethods[index];
			if (methodOnTarget.DeclaringType.IsInterface == false)
			{
				// An interface method can have its default implementation "overridden" in the class,
				// in which case this collector isn't interested in it and another should deal with it.
				return null;
			}

			var proxyable = AcceptMethod(method, true, hook);
			if (!proxyable)
			{
				return null;
			}

			return new MetaMethod(method, methodOnTarget, true, proxyable, !method.IsAbstract);
		}
	}
}
