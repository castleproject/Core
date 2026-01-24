// Copyright 2004-2026 Castle Project - http://www.castleproject.org/
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
	using Castle.DynamicProxy.Tokens;

	internal abstract class InvocationTypeGenerator : IGenerator<ClassEmitter>
	{
		protected readonly MetaMethod method;
		protected readonly Type targetType;
		private readonly MethodInfo callback;
		private readonly bool canChangeTarget;
		private readonly IInvocationCreationContributor contributor;

		protected InvocationTypeGenerator(Type targetType, MetaMethod method, MethodInfo callback, bool canChangeTarget,
		                                  IInvocationCreationContributor contributor)
		{
			this.targetType = targetType;
			this.method = method;
			this.callback = callback;
			this.canChangeTarget = canChangeTarget;
			this.contributor = contributor;
		}

		/// <summary>
		///   Generates the constructor for the class that extends
		///   <see cref = "AbstractInvocation" />
		/// </summary>
		protected abstract ArgumentReference[] GetBaseCtorArguments(Type targetFieldType,
		                                                            out ConstructorInfo baseConstructor);

		protected abstract Type GetBaseType();

		protected abstract FieldReference GetTargetReference();

		public ClassEmitter Generate(ClassEmitter @class, INamingScope namingScope)
		{
			var methodInfo = method.Method;

			var interfaces = Type.EmptyTypes;

			if (canChangeTarget)
			{
				interfaces = new[] { typeof(IChangeProxyTarget) };
			}
			var invocation = GetEmitter(@class, interfaces, namingScope, methodInfo);

			// invocation only needs to mirror the generic parameters of the MethodInfo
			// targetType cannot be a generic type definition (YET!)
			invocation.CopyGenericParametersFromMethod(methodInfo);

			CreateConstructor(invocation);

			var targetField = GetTargetReference();
			if (canChangeTarget)
			{
				ImplementChangeProxyTargetInterface(@class, invocation, targetField);
			}

			ImplementInvokeMethodOnTarget(invocation, methodInfo.GetParameters(), targetField, callback);

#if FEATURE_SERIALIZATION
			invocation.DefineCustomAttribute<SerializableAttribute>();
#endif

			return invocation;
		}

		protected virtual MethodInvocationExpression GetCallbackMethodInvocation(ClassEmitter invocation,
		                                                                         IExpression[] args, MethodInfo callbackMethod,
		                                                                         Reference targetField,
		                                                                         MethodEmitter invokeMethodOnTarget)
		{
			if (contributor != null)
			{
				return contributor.GetCallbackMethodInvocation(invocation, args, targetField, invokeMethodOnTarget);
			}
			var methodOnTargetInvocationExpression = new MethodInvocationExpression(
				new AsTypeExpression(targetField, callbackMethod.DeclaringType),
				callbackMethod,
				args) { VirtualCall = true };
			return methodOnTargetInvocationExpression;
		}

		protected virtual void ImplementInvokeMethodOnTarget(ClassEmitter invocation, ParameterInfo[] parameters,
		                                                     MethodEmitter invokeMethodOnTarget,
		                                                     Reference targetField)
		{
			var callbackMethod = GetCallbackMethod(invocation);
			if (callbackMethod == null)
			{
				EmitCallThrowOnNoTarget(invokeMethodOnTarget);
				return;
			}

			var argumentsMarshaller = new ArgumentsMarshaller(invocation, invokeMethodOnTarget, parameters);

			argumentsMarshaller.CopyOut(out var args, out var byRefArguments);

			if (byRefArguments.Count > 0)
			{
				invokeMethodOnTarget.CodeBuilder.AddStatement(TryStatement.Instance);
			}

			var methodOnTargetInvocationExpression = GetCallbackMethodInvocation(invocation, args, callbackMethod, targetField, invokeMethodOnTarget);

			LocalReference returnValue = null;
			if (callbackMethod.ReturnType != typeof(void))
			{
				var returnType = invocation.GetClosedParameterType(callbackMethod.ReturnType);
				returnValue = invokeMethodOnTarget.CodeBuilder.DeclareLocal(returnType);
				invokeMethodOnTarget.CodeBuilder.AddStatement(new AssignStatement(returnValue, methodOnTargetInvocationExpression));
			}
			else
			{
				invokeMethodOnTarget.CodeBuilder.AddStatement(methodOnTargetInvocationExpression);
			}

			if (byRefArguments.Count > 0)
			{
				invokeMethodOnTarget.CodeBuilder.AddStatement(FinallyStatement.Instance);
				argumentsMarshaller.CopyIn(byRefArguments);
				invokeMethodOnTarget.CodeBuilder.AddStatement(EndExceptionBlockStatement.Instance);
			}

			if (callbackMethod.ReturnType != typeof(void))
			{
				argumentsMarshaller.SetReturnValue(returnValue);
			}

			invokeMethodOnTarget.CodeBuilder.AddStatement(ReturnStatement.Instance);
		}

		private void CreateConstructor(ClassEmitter invocation)
		{
			ConstructorInfo baseConstructor;
			var baseCtorArguments = GetBaseCtorArguments(targetType, out baseConstructor);

			var constructor = CreateConstructor(invocation, baseCtorArguments);
			constructor.CodeBuilder.AddStatement(new ConstructorInvocationStatement(baseConstructor, baseCtorArguments));
			constructor.CodeBuilder.AddStatement(ReturnStatement.Instance);
		}

		private ConstructorEmitter CreateConstructor(ClassEmitter invocation, ArgumentReference[] baseCtorArguments)
		{
			if (contributor == null)
			{
				return invocation.CreateConstructor(baseCtorArguments);
			}
			return contributor.CreateConstructor(baseCtorArguments, invocation);
		}

		private void EmitCallThrowOnNoTarget(MethodEmitter invokeMethodOnTarget)
		{
			var throwOnNoTarget = new MethodInvocationExpression(InvocationMethods.ThrowOnNoTarget);

			invokeMethodOnTarget.CodeBuilder.AddStatement(throwOnNoTarget);
			invokeMethodOnTarget.CodeBuilder.AddStatement(ReturnStatement.Instance);
		}

		private MethodInfo GetCallbackMethod(ClassEmitter invocation)
		{
			if (contributor != null)
			{
				return contributor.GetCallbackMethod();
			}
			var callbackMethod = callback;
			if (callbackMethod == null)
			{
				return null;
			}

			if (!callbackMethod.IsGenericMethod)
			{
				return callbackMethod;
			}

			return callbackMethod.MakeGenericMethod(invocation.GetGenericArgumentsFor(callbackMethod));
		}

		private ClassEmitter GetEmitter(ClassEmitter @class, Type[] interfaces, INamingScope namingScope, MethodInfo methodInfo)
		{
			var suggestedName = string.Format("Castle.Proxies.Invocations.{0}_{1}", methodInfo.DeclaringType.Name,
			                                  methodInfo.Name);
			var uniqueName = namingScope.ParentScope.GetUniqueName(suggestedName);
			return new ClassEmitter(@class.ModuleScope, uniqueName, GetBaseType(), interfaces, ClassEmitter.DefaultTypeAttributes, forceUnsigned: @class.InStrongNamedModule == false);
		}

		private void ImplementInvokeMethodOnTarget(ClassEmitter invocation, ParameterInfo[] parameters,
		                                           FieldReference targetField, MethodInfo callbackMethod)
		{
			var invokeMethodOnTarget = invocation.CreateMethod("InvokeMethodOnTarget", typeof(void));
			ImplementInvokeMethodOnTarget(invocation, parameters, invokeMethodOnTarget, targetField);
		}

		private void ImplementChangeInvocationTarget(ClassEmitter invocation, FieldReference targetField)
		{
			var changeInvocationTarget = invocation.CreateMethod("ChangeInvocationTarget", typeof(void), new[] { typeof(object) });
			changeInvocationTarget.CodeBuilder.AddStatement(
				new AssignStatement(targetField,
				                    new ConvertExpression(targetType, changeInvocationTarget.Arguments[0])));
			changeInvocationTarget.CodeBuilder.AddStatement(ReturnStatement.Instance);
		}

		private void ImplementChangeProxyTarget(ClassEmitter invocation, ClassEmitter @class)
		{
			var changeProxyTarget = invocation.CreateMethod("ChangeProxyTarget", typeof(void), new[] { typeof(object) });

			var proxyObject = new FieldReference(InvocationMethods.ProxyObject);
			var localProxy = changeProxyTarget.CodeBuilder.DeclareLocal(typeof(IProxyTargetAccessor));
			changeProxyTarget.CodeBuilder.AddStatement(
				new AssignStatement(localProxy,
					new ConvertExpression(localProxy.Type, proxyObject)));

			var dynSetProxy = typeof(IProxyTargetAccessor).GetMethod(nameof(IProxyTargetAccessor.DynProxySetTarget));

			changeProxyTarget.CodeBuilder.AddStatement(
				new MethodInvocationExpression(localProxy, dynSetProxy, changeProxyTarget.Arguments[0])
				{
					VirtualCall = true
				});

			changeProxyTarget.CodeBuilder.AddStatement(ReturnStatement.Instance);
		}

		private void ImplementChangeProxyTargetInterface(ClassEmitter @class, ClassEmitter invocation,
		                                                 FieldReference targetField)
		{
			ImplementChangeInvocationTarget(invocation, targetField);

			ImplementChangeProxyTarget(invocation, @class);
		}

		private struct ArgumentsMarshaller
		{
			private readonly ClassEmitter invocation;
			private readonly MethodEmitter method;
			private readonly ParameterInfo[] parameters;

			public ArgumentsMarshaller(ClassEmitter invocation, MethodEmitter method, ParameterInfo[] parameters)
			{
				this.invocation = invocation;
				this.method = method;
				this.parameters = parameters;
			}

			public void CopyOut(out IExpression[] arguments, out Dictionary<int, LocalReference> byRefArguments)
			{
				arguments = new IExpression[parameters.Length];

				// Idea: instead of grab parameters one by one
				// we should grab an array
				byRefArguments = new Dictionary<int, LocalReference>();

				for (int i = 0, n = parameters.Length; i < n; ++i)
				{
					var param = parameters[i];

					var paramType = invocation.GetClosedParameterType(param.ParameterType);
					if (paramType.IsByRef)
					{
						var localReference = method.CodeBuilder.DeclareLocal(paramType.GetElementType());
						IExpression localValue;

#if FEATURE_BYREFLIKE
						if (paramType.GetElementType().IsByRefLikeSafe())
						{
							// The argument value in the invocation `Arguments` array is an `object`
							// and cannot be converted back to its original by-ref-like type.
							// We need to replace it with some other value.

							// For now, we just substitute the by-ref-like type's default value:
							localValue = new DefaultValueExpression(localReference.Type);
						}
						else
#endif
						{
							localValue = new ConvertExpression(
								paramType.GetElementType(),
								new MethodInvocationExpression(
									ThisExpression.Instance,
									InvocationMethods.GetArgumentValue,
									new LiteralIntExpression(i)));
						}

						method.CodeBuilder.AddStatement(new AssignStatement(localReference, localValue));
						var localByRef = new AddressOfExpression(localReference);
						arguments[i] = localByRef;
						byRefArguments[i] = localReference;
					}
					else
					{
#if FEATURE_BYREFLIKE
						if (paramType.IsByRefLikeSafe())
						{
							// The argument value in the invocation `Arguments` array is an `object`
							// and cannot be converted back to its original by-ref-like type.
							// We need to replace it with some other value.

							// For now, we just substitute the by-ref-like type's default value:
							arguments[i] = new DefaultValueExpression(paramType);
						}
						else
#endif
						{
							arguments[i] = new ConvertExpression(
								paramType,
								new MethodInvocationExpression(
									ThisExpression.Instance,
									InvocationMethods.GetArgumentValue,
									new LiteralIntExpression(i)));
						}
					}
				}
			}

			public void CopyIn(Dictionary<int, LocalReference> byRefArguments)
			{
				foreach (var byRefArgument in byRefArguments)
				{
					var index = byRefArgument.Key;
					var localReference = byRefArgument.Value;
					IExpression localValue;

#if FEATURE_BYREFLIKE
					if (localReference.Type.IsByRefLikeSafe())
					{
						// The by-ref-like value in the local buffer variable cannot be put back
						// into the invocation `Arguments` array, because it cannot be boxed.
						// We need to replace it with some other value.

						// For now, we just erase it by substituting `null`:
						localValue = NullExpression.Instance;
					}
					else
#endif
					{
						localValue = new ConvertExpression(typeof(object), localReference.Type, localReference);
					}

					method.CodeBuilder.AddStatement(
						new MethodInvocationExpression(
							ThisExpression.Instance,
							InvocationMethods.SetArgumentValue,
							new LiteralIntExpression(index),
							localValue));
				}
			}

			public void SetReturnValue(LocalReference returnValue)
			{
				IExpression retVal;

#if FEATURE_BYREFLIKE
				if (returnValue.Type.IsByRefLikeSafe())
				{
					// The by-ref-like return value cannot be put into the `ReturnValue` property,
					// because it cannot be boxed. We need to replace it with some other value.

					// For now, we just erase it by substituting `null`:
					retVal = NullExpression.Instance;
				}
				else
#endif
				{
					retVal = new ConvertExpression(typeof(object), returnValue.Type, returnValue);
				}

				var setRetVal = new MethodInvocationExpression(
					ThisExpression.Instance,
					InvocationMethods.SetReturnValue,
					retVal);

				method.CodeBuilder.AddStatement(setRetVal);
			}
		}
	}
}