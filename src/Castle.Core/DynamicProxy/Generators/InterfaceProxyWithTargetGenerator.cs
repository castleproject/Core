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
	using System.Linq;

	using Castle.DynamicProxy.Contributors;
	using Castle.DynamicProxy.Serialization;

	internal sealed class InterfaceProxyWithTargetGenerator : BaseInterfaceProxyGenerator
	{
		public InterfaceProxyWithTargetGenerator(ModuleScope scope, Type targetType, Type[] interfaces,
		                                         Type proxyTargetType, ProxyGenerationOptions options)
			: base(scope, targetType, interfaces, proxyTargetType, options)
		{ }

		protected override bool AllowChangeTarget => false;

		protected override string GeneratorType => ProxyTypeConstants.InterfaceWithTarget;

		protected override CompositeTypeContributor GetProxyTargetContributor(Type proxyTargetType, INamingScope namingScope)
		{
			return new InterfaceProxyTargetContributor(proxyTargetType, AllowChangeTarget, namingScope) { Logger = Logger };
		}

		protected override void AddMappingForAdditionalInterfaces(CompositeTypeContributor contributor, Type[] proxiedInterfaces,
		                                                          IDictionary<Type, ITypeContributor> typeImplementerMapping,
		                                                          ICollection<Type> targetInterfaces)
		{
			foreach (var @interface in interfaces)
			{
				if (!ImplementedByTarget(targetInterfaces, @interface) || proxiedInterfaces.Contains(@interface))
				{
					continue;
				}

				contributor.AddInterfaceToProxy(@interface);
				AddMappingNoCheck(@interface, contributor, typeImplementerMapping);
			}
		}

		private bool ImplementedByTarget(ICollection<Type> targetInterfaces, Type @interface)
		{
			return targetInterfaces.Contains(@interface);
		}
	}
}
