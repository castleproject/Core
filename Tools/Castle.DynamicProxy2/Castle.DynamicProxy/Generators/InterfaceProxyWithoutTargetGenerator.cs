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

namespace Castle.DynamicProxy.Generators
{
	using System;
	using System.Collections.Generic;
	using Castle.DynamicProxy.Generators.Emitters;

	public class InterfaceProxyWithoutTargetGenerator : InterfaceProxyWithTargetGenerator
	{
		public InterfaceProxyWithoutTargetGenerator(ModuleScope scope, Type theInterface) : base(scope, theInterface)
		{
		}

		protected override void AddMappingForTargetType(IDictionary<Type, object> interfaceTypeImplementerMapping)
		{
			AddInterfaceHierarchyMapping(targetType, null /*because we're in proxy withOUT target*/, interfaceTypeImplementerMapping);
		}

		protected override void CreateInvocationForMethod(ClassEmitter emitter, MethodToGenerate method, Type proxyTargetType)
		{
			var methodInfo = method.Method;
			method2methodOnTarget[methodInfo] = methodInfo;

			Type targetForInvocation = methodInfo.DeclaringType;
			method2Invocation[methodInfo] = BuildInvocationNestedType(emitter, targetForInvocation, method, null);
		}

		protected override InterfaceGeneratorType GeneratorType
		{
			get { return InterfaceGeneratorType.WithoutTarget; }
		}
	}
}