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
	using System.Reflection;

	using Castle.Core.Interceptor;

	using Emitters;
	using Emitters.SimpleAST;
	using Tokens;

	public class InterfaceInvocationTypeGenerator : InvocationTypeGenerator
	{
		public static readonly Type BaseType = typeof(CompositionInvocation);

		public InterfaceInvocationTypeGenerator(Type target, MetaMethod method, MethodInfo callback, bool canChangeTarget)
			: base(target, method, callback, canChangeTarget)
		{
		}

		protected override FieldReference GetTargetReference()
		{
			return new FieldReference(InvocationMethods.Target);
		}

		protected override Type GetBaseType()
		{
			return BaseType;
		}

		protected override void ImplementInvokeMethodOnTarget(AbstractTypeEmitter @class, ParameterInfo[] parameters, MethodEmitter invokeMethodOnTarget, MethodInfo callbackMethod, Reference targetField)
		{
			invokeMethodOnTarget.CodeBuilder.AddStatement(
				new ExpressionStatement(
					new MethodInvocationExpression(SelfReference.Self, InvocationMethods.EnsureValidTarget)));
			base.ImplementInvokeMethodOnTarget(@class, parameters, invokeMethodOnTarget, callbackMethod, targetField);
		}

		protected override ArgumentReference[] GetCtorArgumentsAndBaseCtorToCall(Type targetFieldType, ProxyGenerationOptions proxyGenerationOptions,out ConstructorInfo baseConstructor)
		{
			if (proxyGenerationOptions.Selector == null)
			{
				baseConstructor = InvocationMethods.CompositionInvocationConstructorNoSelector;
				return new[]
				{
					new ArgumentReference(targetFieldType),
					new ArgumentReference(typeof(object)),
					new ArgumentReference(typeof(IInterceptor[])),
					new ArgumentReference(typeof(MethodInfo)),
					new ArgumentReference(typeof(object[])),
				};
			}

			baseConstructor = InvocationMethods.CompositionInvocationConstructorWithSelector;
			return new[]
			{
				new ArgumentReference(targetFieldType),
				new ArgumentReference(typeof(object)),
				new ArgumentReference(typeof(IInterceptor[])),
				new ArgumentReference(typeof(MethodInfo)),
				new ArgumentReference(typeof(object[])),
				new ArgumentReference(typeof(IInterceptorSelector)),
				new ArgumentReference(typeof(IInterceptor[]).MakeByRefType())
			};
		}
	}
}