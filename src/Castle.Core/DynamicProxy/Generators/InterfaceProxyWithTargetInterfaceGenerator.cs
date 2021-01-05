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

namespace Castle.DynamicProxy.Generators
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	using Castle.DynamicProxy.Contributors;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Internal;
	using Castle.DynamicProxy.Serialization;

	internal sealed class InterfaceProxyWithTargetInterfaceGenerator : BaseInterfaceProxyGenerator
	{
		public InterfaceProxyWithTargetInterfaceGenerator(ModuleScope scope, Type targetType, Type[] interfaces,
		                                                  Type proxyTargetType, ProxyGenerationOptions options)
			: base(scope, targetType, interfaces, proxyTargetType, options)
		{
		}

		protected override bool AllowChangeTarget => true;

		protected override string GeneratorType => ProxyTypeConstants.InterfaceWithTargetInterface;

		protected override CompositeTypeContributor GetProxyTargetContributor(Type proxyTargetType, INamingScope namingScope)
		{
			return new InterfaceProxyWithTargetInterfaceTargetContributor(proxyTargetType, AllowChangeTarget, namingScope) { Logger = Logger };
		}

		protected override ProxyTargetAccessorContributor GetProxyTargetAccessorContributor()
		{
			return new ProxyTargetAccessorContributor(
				getTargetReference: () => targetField,
				proxyTargetType);
		}

		protected override void AddMappingForAdditionalInterfaces(CompositeTypeContributor contributor, Type[] proxiedInterfaces,
		                                                          IDictionary<Type, ITypeContributor> typeImplementerMapping,
		                                                          ICollection<Type> targetInterfaces)
		{
		}

		protected override InterfaceProxyWithoutTargetContributor GetContributorForAdditionalInterfaces(
			INamingScope namingScope)
		{
			return new InterfaceProxyWithOptionalTargetContributor(namingScope, GetTargetExpression, GetTarget)
			{ Logger = Logger };
		}

		private Reference GetTarget(ClassEmitter @class, MethodInfo method)
		{
			return new AsTypeReference(@class.GetField("__target"), method.DeclaringType);
		}

		private Expression GetTargetExpression(ClassEmitter @class, MethodInfo method)
		{
			return GetTarget(@class, method).ToExpression();
		}
	}
}