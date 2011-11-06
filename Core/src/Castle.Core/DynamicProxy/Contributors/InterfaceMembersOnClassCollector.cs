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

namespace Castle.DynamicProxy.Contributors
{
	using System;
	using System.Reflection;

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Internal;

	public class InterfaceMembersOnClassCollector : MembersCollector
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
			if (method.IsAccessible() == false)
			{
				return null;
			}

			if (onlyProxyVirtual && IsVirtuallyImplementedInterfaceMethod(method))
			{
				return null;
			}

			var methodOnTarget = GetMethodOnTarget(method);

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

		private bool IsVirtuallyImplementedInterfaceMethod(MethodInfo method)
		{
			var info = GetMethodOnTarget(method);
			return info != null && info.IsFinal == false;
		}
	}
}