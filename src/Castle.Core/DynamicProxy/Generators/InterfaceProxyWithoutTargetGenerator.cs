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

	using Castle.DynamicProxy.Contributors;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Serialization;

	internal sealed class InterfaceProxyWithoutTargetGenerator : BaseInterfaceProxyGenerator
	{
		public InterfaceProxyWithoutTargetGenerator(ModuleScope scope, Type targetType, Type[] interfaces,
		                                            Type proxyTargetType, ProxyGenerationOptions options)
			: base(scope, targetType, interfaces, proxyTargetType, options)
		{
		}

		protected override bool AllowChangeTarget => false;

		protected override string GeneratorType => ProxyTypeConstants.InterfaceWithoutTarget;

		protected override CompositeTypeContributor GetProxyTargetContributor(Type proxyTargetType, INamingScope namingScope)
		{
			return new InterfaceProxyWithoutTargetContributor(namingScope, (c, m) => NullExpression.Instance) { Logger = Logger };
		}

		protected override void AddMappingForAdditionalInterfaces(CompositeTypeContributor contributor, Type[] proxiedInterfaces,
		                                                          IDictionary<Type, ITypeContributor> typeImplementerMapping,
		                                                          ICollection<Type> targetInterfaces)
		{
		}

		protected override IEnumerable<Type> GetTypeImplementerMapping(Type _, out IEnumerable<ITypeContributor> contributors, INamingScope namingScope)
		{
			return base.GetTypeImplementerMapping(proxyTargetType: targetType, out contributors, namingScope);
		}
	}
}