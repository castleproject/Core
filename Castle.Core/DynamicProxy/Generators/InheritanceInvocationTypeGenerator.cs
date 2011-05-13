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

namespace Castle.DynamicProxy.Generators
{
	using System;
	using System.Reflection;

	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Internal;
	using Castle.DynamicProxy.Tokens;

	public class InheritanceInvocationTypeGenerator : InvocationTypeGenerator
	{
		public static readonly Type BaseType = typeof(InheritanceInvocation);

		public InheritanceInvocationTypeGenerator(Type targetType, MetaMethod method, MethodInfo callback,
		                                          IInvocationCreationContributor contributor)
			: base(targetType, method, callback, false, contributor)
		{
		}

		protected override ArgumentReference[] GetBaseCtorArguments(Type targetFieldType,
		                                                            ProxyGenerationOptions proxyGenerationOptions,
		                                                            out ConstructorInfo baseConstructor)
		{
			if (proxyGenerationOptions.Selector == null)
			{
				baseConstructor = InvocationMethods.InheritanceInvocationConstructorNoSelector;
				return new[]
				{
					new ArgumentReference(typeof(Type)),
					new ArgumentReference(typeof(object)),
					new ArgumentReference(typeof(IInterceptor[])),
					new ArgumentReference(typeof(MethodInfo)),
					new ArgumentReference(typeof(object[]))
				};
			}

			baseConstructor = InvocationMethods.InheritanceInvocationConstructorWithSelector;
			return new[]
			{
				new ArgumentReference(typeof(Type)),
				new ArgumentReference(typeof(object)),
				new ArgumentReference(typeof(IInterceptor[])),
				new ArgumentReference(typeof(MethodInfo)),
				new ArgumentReference(typeof(object[])),
				new ArgumentReference(typeof(IInterceptorSelector)),
				new ArgumentReference(typeof(IInterceptor[]).MakeByRefType())
			};
		}

		protected override Type GetBaseType()
		{
			return BaseType;
		}

		protected override FieldReference GetTargetReference()
		{
			return new FieldReference(InvocationMethods.ProxyObject);
		}
	}
}