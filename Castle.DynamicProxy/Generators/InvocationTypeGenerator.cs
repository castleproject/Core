// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

	using Castle.Core.Interceptor;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Tokens;

	public abstract class InvocationTypeGenerator : IGenerator<AbstractTypeEmitter>
	{
		private readonly MethodInfo callback;
		private readonly bool canChangeTarget;
		private readonly MetaMethod method;
		protected readonly Type targetType;

		protected InvocationTypeGenerator(Type targetType, MetaMethod method, MethodInfo callback, bool canChangeTarget)
		{
			this.targetType = targetType;
			this.method = method;
			this.callback = callback;
			this.canChangeTarget = canChangeTarget;
		}

		public AbstractTypeEmitter Generate(ClassEmitter @class, ProxyGenerationOptions options, INamingScope namingScope)
		{
			var methodInfo = method.Method;

			var interfaces = new Type[0];

			if (canChangeTarget)
			{
				interfaces = new[] { typeof(IChangeProxyTarget) };
			}
			var type = GetEmitter(@class, interfaces, namingScope, methodInfo);

			// invocation only needs to mirror the generic parameters of the MethodInfo
			// targetType cannot be a generic type definition
			type.CopyGenericParametersFromMethod(methodInfo);

			// Create constructor

			ConstructorInfo baseConstructor;
			var arguments = GetCtorArgumentsAndBaseCtorToCall(targetType, options, out baseConstructor);

			var constructor = type.CreateConstructor(arguments);
			CustomizeCtor(constructor, arguments, type);
			var baseArguments = GetArgumentsForBaseCtor(arguments);
			constructor.CodeBuilder.InvokeBaseConstructor(baseConstructor, baseArguments);
			constructor.CodeBuilder.AddStatement(new ReturnStatement());

			var targetField = GetTargetReference();
			if (canChangeTarget)
			{
				ImplementChangeProxyTargetInterface(@class, type, targetField);
			}

			// InvokeMethodOnTarget implementation

			ImplemementInvokeMethodOnTarget(type, methodInfo.GetParameters(), targetField, callback);

#if !SILVERLIGHT
			type.DefineCustomAttribute<SerializableAttribute>();
#endif

			return type;
		}

		protected abstract Type GetBaseType();

		/// <summary>
		///   Generates the constructor for the class that extends
		///   <see cref="AbstractInvocation" />
		/// </summary>
		/// <param name="targetFieldType"></param>
		/// <param name="proxyGenerationOptions"></param>
		/// <param name="baseConstructor"></param>
		protected abstract ArgumentReference[] GetCtorArgumentsAndBaseCtorToCall(Type targetFieldType,
		                                                                         ProxyGenerationOptions proxyGenerationOptions,
		                                                                         out ConstructorInfo baseConstructor);

		protected abstract FieldReference GetTargetReference();

		protected virtual void CustomizeCtor(ConstructorEmitter constructor, ArgumentReference[] arguments,
		                                     AbstractTypeEmitter @class)
		{
		}

		protected virtual ArgumentReference[] GetArgumentsForBaseCtor(ArgumentReference[] arguments)
		{
			return arguments;
		}

		protected virtual MethodInfo GetCallbackMethod(MethodInfo callbackMethod, AbstractTypeEmitter @class)
		{
			if (callbackMethod == null)
			{
				return null;
			}

			if (!callbackMethod.IsGenericMethod)
			{
				return callbackMethod;
			}

			return callbackMethod.MakeGenericMethod(@class.GetGenericArgumentsFor(callbackMethod));
		}

		protected virtual MethodInvocationExpression GetCallbackMethodInvocation(AbstractTypeEmitter @class, Expression[] args,
		                                                                         MethodInfo callbackMethod,
		                                                                         Reference targetField)
		{
			var methodOnTargetInvocationExpression = new MethodInvocationExpression(
				new AsTypeReference(targetField, callbackMethod.DeclaringType),
				callbackMethod,
				args) { VirtualCall = true };
			return methodOnTargetInvocationExpression;
		}

		protected virtual void ImplementInvokeMethodOnTarget(AbstractTypeEmitter @class, ParameterInfo[] parameters,
		                                                     MethodEmitter invokeMethodOnTarget, MethodInfo callbackMethod,
		                                                     Reference targetField)
		{
			callbackMethod = GetCallbackMethod(callbackMethod, @class);
			if (callbackMethod == null)
			{
				var throwOnNoTarget = new ExpressionStatement(new MethodInvocationExpression(InvocationMethods.ThrowOnNoTarget));

				invokeMethodOnTarget.CodeBuilder.AddStatement(throwOnNoTarget);
				invokeMethodOnTarget.CodeBuilder.AddStatement(new ReturnStatement());
				return;
			}

			if (canChangeTarget)
			{
				invokeMethodOnTarget.CodeBuilder.AddStatement(
				                                             	new ExpressionStatement(
				                                             		new MethodInvocationExpression(SelfReference.Self,
				                                             		                               InvocationMethods.EnsureValidTarget)));
			}

			Expression[] args = new Expression[parameters.Length];

			// Idea: instead of grab parameters one by one
			// we should grab an array
			Dictionary<int, LocalReference> byRefArguments = new Dictionary<int, LocalReference>();

			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo param = parameters[i];

				Type paramType = TypeUtil.GetClosedParameterType(@class, param.ParameterType);
				if (paramType.IsByRef)
				{
					LocalReference localReference = invokeMethodOnTarget.CodeBuilder.DeclareLocal(paramType.GetElementType());
					invokeMethodOnTarget.CodeBuilder.AddStatement(
					                                             	new AssignStatement(localReference,
					                                             	                    new ConvertExpression(paramType.GetElementType(),
					                                             	                                          new MethodInvocationExpression
					                                             	                                          	(SelfReference.Self,
					                                             	                                          	 InvocationMethods.
					                                             	                                          	 	GetArgumentValue,
					                                             	                                          	 new LiteralIntExpression(
					                                             	                                          	 	i)))));
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

			var methodOnTargetInvocationExpression = GetCallbackMethodInvocation(@class, args, callbackMethod, targetField);

			LocalReference returnValue = null;
			if (callbackMethod.ReturnType != typeof(void))
			{
				Type returnType = TypeUtil.GetClosedParameterType(@class, callbackMethod.ReturnType);
				returnValue = invokeMethodOnTarget.CodeBuilder.DeclareLocal(returnType);
				invokeMethodOnTarget.CodeBuilder.AddStatement(new AssignStatement(returnValue, methodOnTargetInvocationExpression));
			}
			else
			{
				invokeMethodOnTarget.CodeBuilder.AddStatement(new ExpressionStatement(methodOnTargetInvocationExpression));
			}

			foreach (KeyValuePair<int, LocalReference> byRefArgument in byRefArguments)
			{
				int index = byRefArgument.Key;
				LocalReference localReference = byRefArgument.Value;
				invokeMethodOnTarget.CodeBuilder.AddStatement(
				                                             	new ExpressionStatement(
				                                             		new MethodInvocationExpression(SelfReference.Self,
				                                             		                               InvocationMethods.SetArgumentValue,
				                                             		                               new LiteralIntExpression(index),
				                                             		                               new ConvertExpression(typeof(object),
				                                             		                                                     localReference.
				                                             		                                                     	Type,
				                                             		                                                     new ReferenceExpression
				                                             		                                                     	(localReference)))
				                                             		));
			}

			if (callbackMethod.ReturnType != typeof(void))
			{
				var setRetVal =
					new MethodInvocationExpression(SelfReference.Self,
					                               InvocationMethods.SetReturnValue,
					                               new ConvertExpression(typeof(object), returnValue.Type, returnValue.ToExpression()));

				invokeMethodOnTarget.CodeBuilder.AddStatement(new ExpressionStatement(setRetVal));
			}

			invokeMethodOnTarget.CodeBuilder.AddStatement(new ReturnStatement());
		}

		protected void CreateEmptyIInvocationInvokeOnTarget(AbstractTypeEmitter nested)
		{
			var invokeMethodOnTarget = nested.CreateMethod("InvokeMethodOnTarget");
			var throwOnNoTarget = new ExpressionStatement(new MethodInvocationExpression(InvocationMethods.ThrowOnNoTarget));

			invokeMethodOnTarget.CodeBuilder.AddStatement(throwOnNoTarget);
			invokeMethodOnTarget.CodeBuilder.AddStatement(new ReturnStatement());
		}

		private AbstractTypeEmitter GetEmitter(ClassEmitter @class, Type[] interfaces, INamingScope namingScope,
		                                       MethodInfo methodInfo)
		{
			var suggestedName = string.Format("Castle.Proxies.Invocations.{0}_{1}", methodInfo.DeclaringType.Name,
			                                  methodInfo.Name);
			var uniqueName = namingScope.ParentScope.GetUniqueName(suggestedName);
			return new ClassEmitter(@class.ModuleScope, uniqueName, GetBaseType(), interfaces);
		}

		private void ImplemementInvokeMethodOnTarget(AbstractTypeEmitter @class, ParameterInfo[] parameters,
		                                             FieldReference targetField, MethodInfo callbackMethod)
		{
			var invokeMethodOnTarget = @class.CreateMethod("InvokeMethodOnTarget");
			ImplementInvokeMethodOnTarget(@class, parameters, invokeMethodOnTarget, callbackMethod, targetField);
		}

		private void ImplementChangeInvocationTarget(AbstractTypeEmitter invocation, FieldReference targetField)
		{
			var argument = new ArgumentReference(typeof(object));
			var changeInvocationTarget = invocation.CreateMethod("ChangeInvocationTarget", argument);
			changeInvocationTarget.CodeBuilder.AddStatement(
			                                               	new AssignStatement(targetField,
			                                               	                    new ConvertExpression(targetType,
			                                               	                                          argument.ToExpression())));
			changeInvocationTarget.CodeBuilder.AddStatement(new ReturnStatement());
		}

		private void ImplementChangeProxyTarget(AbstractTypeEmitter invocation, ClassEmitter @class)
		{
			var argument = new ArgumentReference(typeof(object));
			var changeInvocationTarget = invocation.CreateMethod("ChangeProxyTarget", argument);
			changeInvocationTarget.CodeBuilder.AddStatement(
			                                               	new ExpressionStatement(
			                                               		new ConvertExpression(@class.TypeBuilder,
			                                               		                      new FieldReference(
			                                               		                      	InvocationMethods.ProxyObject).ToExpression())));

			var field = @class.GetField("__target");
			changeInvocationTarget.CodeBuilder.AddStatement(
			                                               	new AssignStatement(
			                                               		new FieldReference(field.Reference) { OwnerReference = null },
			                                               		new ConvertExpression(field.Fieldbuilder.FieldType,
			                                               		                      argument.ToExpression())));

			changeInvocationTarget.CodeBuilder.AddStatement(new ReturnStatement());
		}

		private void ImplementChangeProxyTargetInterface(ClassEmitter @class, AbstractTypeEmitter invocation,
		                                                 FieldReference targetField)
		{
			ImplementChangeInvocationTarget(invocation, targetField);

			ImplementChangeProxyTarget(invocation, @class);
		}
	}
}