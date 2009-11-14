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
	using System.Reflection;
	using Core.Interceptor;
	using Emitters;
	using Emitters.SimpleAST;
	using Tokens;

	public class InvocationTypeGenerator : IGenerator<NestedClassEmitter>
	{
		private readonly Type targetType;
		private readonly IProxyMethod method;
		private readonly MethodInfo callback;
		private readonly bool canChangeTarget;

		public InvocationTypeGenerator(Type targetType, IProxyMethod method, MethodInfo callback, bool canChangeTarget)
		{
			this.targetType = targetType;
			this.method = method;
			this.callback = callback;
			this.canChangeTarget = canChangeTarget;
		}

		public NestedClassEmitter Generate(ClassEmitter @class, ProxyGenerationOptions options, INamingScope namingScope)
		{
			var methodInfo = method.Method;

			Type[] interfaces = new Type[0];

			if (canChangeTarget)
			{
				interfaces = new[] { typeof(IChangeProxyTarget) };
			}
			var nested =
				new NestedClassEmitter(@class,
				                       "Invocation_" + namingScope.GetUniqueName(methodInfo.Name),
				                       typeof (AbstractInvocation),
				                       interfaces);

			// invocation only needs to mirror the generic parameters of the MethodInfo
			// targetType cannot be a generic type definition
			nested.CopyGenericParametersFromMethod(methodInfo);


			// Create constructor

			CreateIInvocationConstructor(targetType, nested,options);

			var targetField = new FieldReference(InvocationMethods.Target);
			if (canChangeTarget)
			{
				ImplementChangeProxyTargetInterface(@class, nested, targetField);
			}

			// InvokeMethodOnTarget implementation

			if (callback != null)
			{
				CreateIInvocationInvokeOnTarget(nested, methodInfo.GetParameters(), targetField, callback);
			}
			else if (method.HasTarget)
			{
				CreateIInvocationInvokeOnTarget(nested, methodInfo.GetParameters(), targetField, methodInfo);
			}
			else
			{
				CreateEmptyIInvocationInvokeOnTarget(nested);
			}

#if !SILVERLIGHT
			nested.DefineCustomAttribute<SerializableAttribute>();
#endif

			return nested;
		}

		private void ImplementChangeProxyTargetInterface(ClassEmitter @class, NestedClassEmitter invocation, FieldReference targetField)
		{
			ImplementChangeInvocationTarget(invocation, targetField);

			ImplementChangeProxyTarget(invocation, @class);
		}

		private void ImplementChangeProxyTarget(NestedClassEmitter invocation, ClassEmitter @class)
		{
			var argument = new ArgumentReference(typeof(object));
			var changeInvocationTarget = invocation.CreateMethod("ChangeProxyTarget",
			                                                     MethodAttributes.Public | MethodAttributes.Virtual,
			                                                     typeof(void),
			                                                     argument);
			changeInvocationTarget.CodeBuilder.AddStatement(
				new ExpressionStatement(
					new ConvertExpression(@class.TypeBuilder, new FieldReference(InvocationMethods.ProxyObject).ToExpression())));

			var field = @class.GetField("__target");
			changeInvocationTarget.CodeBuilder.AddStatement(
				new AssignStatement(
					new FieldReference(field.Reference) { OwnerReference = null },
					new ConvertExpression(field.Fieldbuilder.FieldType, argument.ToExpression())));

			changeInvocationTarget.CodeBuilder.AddStatement(new ReturnStatement());
		}

		private void ImplementChangeInvocationTarget(NestedClassEmitter invocation, FieldReference targetField)
		{
			var argument = new ArgumentReference(typeof (object));
			var changeInvocationTarget = invocation.CreateMethod("ChangeInvocationTarget",
			                                                     MethodAttributes.Public | MethodAttributes.Virtual,
			                                                     typeof(void),
			                                                     argument);
			changeInvocationTarget.CodeBuilder.AddStatement(
				new AssignStatement(targetField,
				                    new ConvertExpression(targetType, argument.ToExpression())));
			changeInvocationTarget.CodeBuilder.AddStatement(new ReturnStatement());
		}

		protected void CreateIInvocationInvokeOnTarget(NestedClassEmitter nested, ParameterInfo[] parameters, FieldReference targetField, MethodInfo callbackMethod)
		{
			const MethodAttributes methodAtts = MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual;

			MethodEmitter method = nested.CreateMethod("InvokeMethodOnTarget", methodAtts, typeof(void));

			ImplementInvokeMethodOnTarget(nested, parameters, method, callbackMethod, targetField);
		}

		protected virtual void ImplementInvokeMethodOnTarget(NestedClassEmitter nested, ParameterInfo[] parameters, MethodEmitter method, MethodInfo callbackMethod, Reference targetField)
		{

			if (canChangeTarget)
			{
				method.CodeBuilder.AddStatement(
					new ExpressionStatement(new MethodInvocationExpression(SelfReference.Self, InvocationMethods.EnsureValidTarget)));
			}
			

			Expression[] args = new Expression[parameters.Length];

			// Idea: instead of grab parameters one by one
			// we should grab an array
			Dictionary<int, LocalReference> byRefArguments = new Dictionary<int, LocalReference>();

			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo param = parameters[i];

				Type paramType = TypeUtil.GetClosedParameterType(nested, param.ParameterType);
				if (paramType.IsByRef)
				{
					LocalReference localReference = method.CodeBuilder.DeclareLocal(paramType.GetElementType());
					method.CodeBuilder.AddStatement(
						new AssignStatement(localReference,
											new ConvertExpression(paramType.GetElementType(),
																  new MethodInvocationExpression(SelfReference.Self,
																								 InvocationMethods.GetArgumentValue,
																								 new LiteralIntExpression(i)))));
					ByRefReference byRefReference = new ByRefReference(localReference);
					args[i] = new ReferenceExpression(byRefReference);
					byRefArguments[i] = localReference;
				}
				else
				{
					args[i] =
						new ConvertExpression(paramType,
											  new MethodInvocationExpression(SelfReference.Self,
																			 InvocationMethods.GetArgumentValue,
																			 new LiteralIntExpression(i)));
				}
			}

			if (callbackMethod.IsGenericMethod)
			{
				callbackMethod = callbackMethod.MakeGenericMethod(nested.GetGenericArgumentsFor(callbackMethod));
			}

			var methodOnTargetInvocationExpression = new MethodInvocationExpression(
				new AsTypeReference(targetField, callbackMethod.DeclaringType),
				callbackMethod,
				args) { VirtualCall = true };

			LocalReference returnValue = null;
			if (callbackMethod.ReturnType != typeof(void))
			{
				Type returnType = TypeUtil.GetClosedParameterType(nested, callbackMethod.ReturnType);
				returnValue = method.CodeBuilder.DeclareLocal(returnType);
				method.CodeBuilder.AddStatement(new AssignStatement(returnValue, methodOnTargetInvocationExpression));
			}
			else
			{
				method.CodeBuilder.AddStatement(new ExpressionStatement(methodOnTargetInvocationExpression));
			}

			foreach (KeyValuePair<int, LocalReference> byRefArgument in byRefArguments)
			{
				int index = byRefArgument.Key;
				LocalReference localReference = byRefArgument.Value;
				method.CodeBuilder.AddStatement(
					new ExpressionStatement(
						new MethodInvocationExpression(SelfReference.Self,
													   InvocationMethods.SetArgumentValue,
													   new LiteralIntExpression(index),
													   new ConvertExpression(typeof(object), localReference.Type,
																			 new ReferenceExpression(localReference)))
						));
			}

			if (callbackMethod.ReturnType != typeof(void))
			{
				MethodInvocationExpression setRetVal =
					new MethodInvocationExpression(SelfReference.Self,
												   InvocationMethods.SetReturnValue,
												   new ConvertExpression(typeof(object), returnValue.Type, returnValue.ToExpression()));

				method.CodeBuilder.AddStatement(new ExpressionStatement(setRetVal));
			}

			method.CodeBuilder.AddStatement(new ReturnStatement());
		}





		protected void CreateEmptyIInvocationInvokeOnTarget(NestedClassEmitter nested)
		{
			const MethodAttributes methodAtts = MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual;

			MethodEmitter invokeMethodOnTarget =
				nested.CreateMethod("InvokeMethodOnTarget", methodAtts, typeof(void));

			String message = "This is a DynamicProxy2 error: the interceptor attempted " +
							 "to 'Proceed' for a method without a target, for example, an interface method or an abstract method";

			invokeMethodOnTarget.CodeBuilder.AddStatement(new ThrowStatement(typeof(NotImplementedException), message));

			invokeMethodOnTarget.CodeBuilder.AddStatement(new ReturnStatement());
		}

		/// <summary>
		/// Generates the constructor for the nested class that extends
		/// <see cref="AbstractInvocation"/>
		/// </summary>
		/// <param name="targetFieldType"></param>
		/// <param name="nested"></param>
		/// <param name="proxyGenerationOptions"></param>
		protected void CreateIInvocationConstructor(Type targetFieldType, NestedClassEmitter nested, ProxyGenerationOptions proxyGenerationOptions)
		{
			var target = new ArgumentReference(targetFieldType);
			var targetType = new ArgumentReference(typeof(Type));
			var proxy = new ArgumentReference(typeof(object));
			var interceptors = new ArgumentReference(typeof(IInterceptor[]));
			var proxiedMethod = new ArgumentReference(typeof(MethodInfo));
			var arguments = new ArgumentReference(typeof(object[]));

			ConstructorEmitter constructor;
			if (proxyGenerationOptions.Selector != null)
			{
				var selector = new ArgumentReference(typeof(IInterceptorSelector));
				var methodInterceptorsByRef = new ArgumentReference(typeof(IInterceptor[]).MakeByRefType());
				constructor = nested.CreateConstructor(target, targetType, proxy, interceptors, proxiedMethod, arguments,
				                                       selector, methodInterceptorsByRef);

				constructor.CodeBuilder.InvokeBaseConstructor(
					InvocationMethods.ConstructorWithTargetMethodWithSelector,
					target, targetType, proxy, interceptors, proxiedMethod, arguments, selector, methodInterceptorsByRef);
			}
			else
			{
				constructor = nested.CreateConstructor(target, targetType, proxy, interceptors, proxiedMethod, arguments);

				constructor.CodeBuilder.InvokeBaseConstructor(
					InvocationMethods.ConstructorWithTargetMethod,
					target, targetType, proxy, interceptors, proxiedMethod, arguments);
			}
			constructor.CodeBuilder.AddStatement(new ReturnStatement());
		}
	}
}