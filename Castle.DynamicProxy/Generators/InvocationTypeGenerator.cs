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
		private readonly Type target;
		private readonly IProxyMethod method;
		private readonly MethodInfo callback;
		private readonly bool canChangeTarget;

		public InvocationTypeGenerator(Type target, IProxyMethod method, MethodInfo callback, bool canChangeTarget)
		{
			this.target = target;
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

			// Create the invocation fields

			FieldReference targetRef = nested.CreateField("target", target);

			// Create constructor

			CreateIInvocationConstructor(target, nested, targetRef,options);

			if (canChangeTarget)
			{
				var argument1 = new ArgumentReference(typeof (object));
				MethodEmitter methodEmitter =
					nested.CreateMethod("ChangeInvocationTarget", MethodAttributes.Public | MethodAttributes.Virtual,
					                    typeof (void), argument1);
				methodEmitter.CodeBuilder.AddStatement(
					new AssignStatement(targetRef,
					                    new ConvertExpression(target, argument1.ToExpression())
						)
					);
				methodEmitter.CodeBuilder.AddStatement(new ReturnStatement());
			}

			// InvokeMethodOnTarget implementation

			if (callback != null)
			{
				CreateIInvocationInvokeOnTarget(nested, methodInfo.GetParameters(), targetRef, callback);
			}
			else if (method.HasTarget)
			{
				CreateIInvocationInvokeOnTarget(nested, methodInfo.GetParameters(), targetRef, methodInfo);
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

		protected void CreateIInvocationInvokeOnTarget(NestedClassEmitter nested, ParameterInfo[] parameters, FieldReference targetField, MethodInfo callbackMethod)
		{
			const MethodAttributes methodAtts = MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual;

			MethodEmitter method = nested.CreateMethod("InvokeMethodOnTarget", methodAtts, typeof(void));

			ImplementInvokeMethodOnTarget(nested, parameters, method, callbackMethod, targetField);
		}

		protected virtual void ImplementInvokeMethodOnTarget(NestedClassEmitter nested, ParameterInfo[] parameters, MethodEmitter method, MethodInfo callbackMethod, Reference targetField)
		{
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

			MethodInvocationExpression baseMethodInvExp = new MethodInvocationExpression(targetField, callbackMethod, args);
			baseMethodInvExp.VirtualCall = true;

			LocalReference returnValue = null;
			if (callbackMethod.ReturnType != typeof(void))
			{
				Type returnType = TypeUtil.GetClosedParameterType(nested, callbackMethod.ReturnType);
				returnValue = method.CodeBuilder.DeclareLocal(returnType);
				method.CodeBuilder.AddStatement(new AssignStatement(returnValue, baseMethodInvExp));
			}
			else
			{
				method.CodeBuilder.AddStatement(new ExpressionStatement(baseMethodInvExp));
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
		/// <param name="targetField"></param>
		/// <param name="proxyGenerationOptions"></param>
		protected void CreateIInvocationConstructor(Type targetFieldType, NestedClassEmitter nested, FieldReference targetField, ProxyGenerationOptions proxyGenerationOptions)
		{
			var cArg0 = new ArgumentReference(targetFieldType);
			var cArg1 = new ArgumentReference(typeof(object));
			var cArg2 = new ArgumentReference(typeof(IInterceptor[]));
			var cArg3 = new ArgumentReference(typeof(Type));
			var cArg4 = new ArgumentReference(typeof(MethodInfo));
			var cArg5 = new ArgumentReference(typeof(MethodInfo));
			var cArg6 = new ArgumentReference(typeof(object[]));

			ConstructorEmitter constructor;
			if (proxyGenerationOptions.Selector != null)
			{
				var cArg7 = new ArgumentReference(typeof(IInterceptorSelector));
				var cArg8 = new ArgumentReference(typeof(IInterceptor[]).MakeByRefType());
				constructor = nested.CreateConstructor(cArg0, cArg1, cArg2, cArg3, cArg4, cArg5, cArg6, cArg7, cArg8);

				constructor.CodeBuilder.InvokeBaseConstructor(
					InvocationMethods.ConstructorWithTargetMethodWithSelector,
					cArg0, cArg1, cArg2, cArg3, cArg4, cArg5, cArg6, cArg7, cArg8);
			}
			else
			{
				constructor = nested.CreateConstructor(cArg0, cArg1, cArg2, cArg3, cArg4, cArg5, cArg6);

				constructor.CodeBuilder.InvokeBaseConstructor(
					InvocationMethods.ConstructorWithTargetMethod,
					cArg0, cArg1, cArg2, cArg3, cArg4, cArg5, cArg6);
			}

			constructor.CodeBuilder.AddStatement(new AssignStatement(targetField, cArg0.ToExpression()));
			constructor.CodeBuilder.AddStatement(new ReturnStatement());
		}
	}
}