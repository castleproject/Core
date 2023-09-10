// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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
	using System.Reflection;

	using Castle.DynamicProxy.Generators;

	internal class InterfaceMembersOnClassCollector : MembersCollector
	{
		private readonly InterfaceMapping map;
		private readonly bool onlyProxyVirtual;

		public InterfaceMembersOnClassCollector(Type type, bool onlyProxyVirtual, InterfaceMapping map) : base(type)
		{
			this.onlyProxyVirtual = onlyProxyVirtual;
			this.map = map;
		}

		protected override MetaMethod GetMethodToGenerate(MethodInfo method, IProxyGenerationHook hook, bool isStandalone)
		{
			var methodOnTarget = GetMethodOnTarget(method);

			if (onlyProxyVirtual)
			{
				// The (somewhat confusingly named) `onlyProxyVirtual` flag may need some explaining.
				//
				// This collector type is used in two distinct scenarios:
				//
				//  1. When generating a class proxy for some class `T` which implements interface `I`,
				//     and `I` is again specified as an additional interface to add to the proxy type.
				//     In this case, this collector gets invoked for `I` and `onlyProxyVirtual == true`,
				//     and below logic prevents `I` methods from being implemented a second time when
				//     the main "target" contributor already took care of them (which happens when they
				//     are overridable, or more specifically, when they are implicitly implemented and
				//     marked as `virtual`).
				//
				//  2. When generating an interface proxy with target for some interface `I` and target
				//     type `T`. In this case, `onlyProxyVirtual == false`, which forces members of `I`
				//     to get implemented. Unlike in (1), the target of such proxies will be separate
				//     objects, so it doesn't matter if & how they implement members of `I` or not;
				//     those `I` members still need to be implemented on the proxy type regardless.

				var isVirtuallyImplementedInterfaceMethod = methodOnTarget != null && methodOnTarget.IsFinal == false;

				if (isVirtuallyImplementedInterfaceMethod)
				{
					return null;
				}
			}

			var proxyable = AcceptMethod(method, onlyProxyVirtual, hook);
			return new MetaMethod(method, methodOnTarget, isStandalone, proxyable, methodOnTarget.IsPrivate == false);
		}

		private MethodInfo GetMethodOnTarget(MethodInfo method)
		{
			var index = Array.IndexOf(map.InterfaceMethods, method);
			if (index == -1)
			{
				return null;
			}

			return map.TargetMethods[index];
		}
	}
}