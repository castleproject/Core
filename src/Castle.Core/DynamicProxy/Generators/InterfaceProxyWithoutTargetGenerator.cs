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
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Internal;
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

		protected override ITypeContributor AddMappingForTargetType(
			IDictionary<Type, ITypeContributor> interfaceTypeImplementerMapping, Type proxyTargetType,
			ICollection<Type> targetInterfaces, INamingScope namingScope)
		{
			var contributor = GetProxyTargetContributor(proxyTargetType, namingScope);
			var proxiedInterfaces = targetType.GetAllInterfaces();
			foreach (var @interface in proxiedInterfaces)
			{
				contributor.AddInterfaceToProxy(@interface);
				AddMappingNoCheck(@interface, contributor, interfaceTypeImplementerMapping);
			}

			AddMappingForAdditionalInterfaces(contributor, proxiedInterfaces, interfaceTypeImplementerMapping, targetInterfaces);
			return contributor;
		}

		protected override Type GenerateType(string typeName, INamingScope namingScope)
		{
			IEnumerable<ITypeContributor> contributors;
			var allInterfaces = GetTypeImplementerMapping(targetType, out contributors, namingScope);
			var model = new MetaType();
			// collect elements
			foreach (var contributor in contributors)
			{
				contributor.CollectElementsToProxy(ProxyGenerationOptions.Hook, model);
			}

			ProxyGenerationOptions.Hook.MethodsInspected();

			ClassEmitter emitter;
			FieldReference interceptorsField;
			var baseType = Init(typeName, out emitter, proxyTargetType, out interceptorsField, allInterfaces);

			// Constructor

			var cctor = GenerateStaticConstructor(emitter);
			var mixinFieldsList = new List<FieldReference>();

			foreach (var contributor in contributors)
			{
				contributor.Generate(emitter);

				// TODO: redo it
				if (contributor is MixinContributor)
				{
					mixinFieldsList.AddRange((contributor as MixinContributor).Fields);
				}
			}

			var ctorArguments = new List<FieldReference>(mixinFieldsList) { interceptorsField, targetField };
			var selector = emitter.GetField("__selector");
			if (selector != null)
			{
				ctorArguments.Add(selector);
			}

			GenerateConstructors(emitter, baseType, ctorArguments.ToArray());

			// Complete type initializer code body
			CompleteInitCacheMethod(cctor.CodeBuilder);

			// Crosses fingers and build type
			var generatedType = emitter.BuildType();

			InitializeStaticFields(generatedType);
			return generatedType;
		}
	}
}