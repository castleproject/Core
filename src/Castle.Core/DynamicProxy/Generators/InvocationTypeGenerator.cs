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

#nullable enable

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
				new AsTypeExpression(targetField, callbackMethod.DeclaringType!),
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

			argumentsMarshaller.CopyOut(out var args, out var byRefArguments, out var hasByRefArguments);

			if (hasByRefArguments)
			{
				invokeMethodOnTarget.CodeBuilder.AddStatement(TryStatement.Instance);
			}

			var methodOnTargetInvocationExpression = GetCallbackMethodInvocation(invocation, args, callbackMethod, targetField, invokeMethodOnTarget);

			LocalReference? returnValue = null;
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

			if (hasByRefArguments)
			{
				invokeMethodOnTarget.CodeBuilder.AddStatement(FinallyStatement.Instance);
				argumentsMarshaller.CopyIn(byRefArguments);
				invokeMethodOnTarget.CodeBuilder.AddStatement(EndExceptionBlockStatement.Instance);
			}

			if (callbackMethod.ReturnType != typeof(void))
			{
				argumentsMarshaller.SetReturnValue(returnValue!);
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

		private MethodInfo? GetCallbackMethod(ClassEmitter invocation)
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
			var suggestedName = string.Format("Castle.Proxies.Invocations.{0}_{1}", methodInfo.DeclaringType!.Name,
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

			var dynSetProxy = typeof(IProxyTargetAccessor).GetMethod(nameof(IProxyTargetAccessor.DynProxySetTarget))!;

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

			public void CopyOut(out IExpression[] arguments, out LocalReference?[] byRefArguments, out bool hasByRefArguments)
			{
				if (parameters.Length == 0)
				{
					arguments = [];
					byRefArguments = [];
					hasByRefArguments = false;
					return;
				}

				arguments = new IExpression[parameters.Length];
				byRefArguments = new LocalReference?[parameters.Length];
				hasByRefArguments = false;

				for (int i = 0, n = parameters.Length; i < n; ++i)
				{
					var argumentType = invocation.GetClosedParameterType(parameters[i].ParameterType);
					var dereferencedArgumentType = argumentType.IsByRef ? argumentType.GetElementType()! : argumentType;

					IExpression dereferencedArgument;

					// Note that we don't need special logic for byref-like values / `ByRefLikeReference` here,
					// since `ConvertArgumentFromObjectExpression` knows how to deal with those.

					dereferencedArgument = new ConvertArgumentFromObjectExpression(
						new MethodInvocationExpression(
							ThisExpression.Instance,
							InvocationMethods.GetArgumentValue,
							new LiteralIntExpression(i)),
						dereferencedArgumentType);

					if (argumentType.IsByRef)
					{
						var localCopy = method.CodeBuilder.DeclareLocal(dereferencedArgumentType);
						method.CodeBuilder.AddStatement(new AssignStatement(localCopy, dereferencedArgument));
						arguments[i] = new AddressOfExpression(localCopy);
						byRefArguments[i] = localCopy;
						hasByRefArguments = true;
					}
					else
					{
						arguments[i] = dereferencedArgument;
					}
				}
			}

			public void CopyIn(LocalReference?[] byRefArguments)
			{
				for (int i = 0, n = byRefArguments.Length; i < n; ++i)
				{
					var localCopy = byRefArguments[i];
					if (localCopy == null) continue;

#if FEATURE_BYREFLIKE
					if (localCopy.Type.IsByRefLikeSafe())
					{
						// For byref-like values, a `ByRefLikeReference` has previously been placed
						// in `IInvocation.Arguments`. We must not replace that substitute value,
						// but use it to update the referenced byref-like parameter:
						method.CodeBuilder.AddStatement(
							new AssignStatement(
								new PointerReference(
									new MethodInvocationExpression(
										new MethodInvocationExpression(
											ThisExpression.Instance,
											InvocationMethods.GetArgumentValue,
											new LiteralIntExpression(i)),
										ByRefLikeReferenceMethods.GetPtr,
										new TypeTokenExpression(localCopy.Type)),
									localCopy.Type),
								localCopy));
					}
					else
#endif
					{
						method.CodeBuilder.AddStatement(
							new MethodInvocationExpression(
								ThisExpression.Instance,
								InvocationMethods.SetArgumentValue,
								new LiteralIntExpression(i),
								new ConvertArgumentToObjectExpression(localCopy)));
					}
				}
			}

			public void SetReturnValue(LocalReference returnValue)
			{
#if FEATURE_BYREFLIKE
				// TODO: For byref-like return values, we will need to read `IInvocation.ReturnValue`
				// and set the return value via pointer indirection (`ByRefLikeReference.GetPtr`).
#endif

				method.CodeBuilder.AddStatement(new MethodInvocationExpression(
					ThisExpression.Instance,
					InvocationMethods.SetReturnValue,
					new ConvertArgumentToObjectExpression(returnValue)));
			}
		}
	}
}