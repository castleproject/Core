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
	using System.Diagnostics;
	using System.Reflection;
	using System.Reflection.Emit;
#if FEATURE_SERIALIZATION
	using System.Xml.Serialization;
#endif

	using Castle.Core.Internal;
	using Castle.DynamicProxy.Contributors;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Internal;
	using Castle.DynamicProxy.Tokens;

	internal class MethodWithInvocationGenerator : MethodGenerator
	{
		private readonly IInvocationCreationContributor contributor;
		private readonly GetTargetExpressionDelegate getTargetExpression;
		private readonly GetTargetExpressionDelegate getTargetTypeExpression;
		private readonly IExpression interceptors;
		private readonly Type invocation;

		public MethodWithInvocationGenerator(MetaMethod method, IExpression interceptors, Type invocation,
		                                     GetTargetExpressionDelegate getTargetExpression,
		                                     OverrideMethodDelegate createMethod, IInvocationCreationContributor contributor)
			: this(method, interceptors, invocation, getTargetExpression, null, createMethod, contributor)
		{
		}

		public MethodWithInvocationGenerator(MetaMethod method, IExpression interceptors, Type invocation,
		                                     GetTargetExpressionDelegate getTargetExpression,
		                                     GetTargetExpressionDelegate getTargetTypeExpression,
		                                     OverrideMethodDelegate createMethod, IInvocationCreationContributor contributor)
			: base(method, createMethod)
		{
			this.invocation = invocation;
			this.getTargetExpression = getTargetExpression;
			this.getTargetTypeExpression = getTargetTypeExpression;
			this.interceptors = interceptors;
			this.contributor = contributor;
		}

		protected FieldReference BuildMethodInterceptorsField(ClassEmitter @class, MethodInfo method, INamingScope namingScope)
		{
			var methodInterceptors = @class.CreateField(
				namingScope.GetUniqueName(string.Format("interceptors_{0}", method.Name)),
				typeof(IInterceptor[]),
				false);
#if FEATURE_SERIALIZATION
			@class.DefineCustomAttributeFor<XmlIgnoreAttribute>(methodInterceptors);
#endif
			return methodInterceptors;
		}

		protected override MethodEmitter BuildProxiedMethodBody(MethodEmitter emitter, ClassEmitter @class, INamingScope namingScope)
		{
			var invocationType = invocation;

			var genericArguments = Type.EmptyTypes;

			var constructor = invocation.GetConstructors()[0];

			IExpression proxiedMethodTokenExpression;
			if (MethodToOverride.IsGenericMethod)
			{
				// Not in the cache: generic method
				genericArguments = emitter.MethodBuilder.GetGenericArguments();
				proxiedMethodTokenExpression = new MethodTokenExpression(MethodToOverride.MakeGenericMethod(genericArguments));

				if (invocationType.IsGenericTypeDefinition)
				{
					// bind generic method arguments to invocation's type arguments
					invocationType = invocationType.MakeGenericType(genericArguments);
					constructor = TypeBuilder.GetConstructor(invocationType, constructor);
				}
			}
			else
			{
				var proxiedMethodToken = @class.CreateStaticField(namingScope.GetUniqueName("token_" + MethodToOverride.Name), typeof(MethodInfo));
				@class.ClassConstructor.CodeBuilder.AddStatement(new AssignStatement(proxiedMethodToken, new MethodTokenExpression(MethodToOverride)));

				proxiedMethodTokenExpression = proxiedMethodToken;
			}

			var methodInterceptors = SetMethodInterceptors(@class, namingScope, emitter, proxiedMethodTokenExpression);

			var argumentsMarshaller = new ArgumentsMarshaller(emitter, MethodToOverride.GetParameters());

			argumentsMarshaller.CopyIn(out var argumentsArray);

			var hasByRefArguments = HasByRefArguments(emitter.Arguments);

			var ctorArguments = GetCtorArguments(@class, proxiedMethodTokenExpression, argumentsArray, methodInterceptors);
			ctorArguments = ModifyArguments(@class, ctorArguments);

			var invocationLocal = emitter.CodeBuilder.DeclareLocal(invocationType);
			emitter.CodeBuilder.AddStatement(new AssignStatement(invocationLocal,
			                                                     new NewInstanceExpression(constructor, ctorArguments)));

			if (MethodToOverride.ContainsGenericParameters)
			{
				EmitLoadGenericMethodArguments(emitter, MethodToOverride.MakeGenericMethod(genericArguments), invocationLocal);
			}

			if (hasByRefArguments)
			{
				emitter.CodeBuilder.AddStatement(TryStatement.Instance);
			}

			var proceed = new MethodInvocationExpression(invocationLocal, InvocationMethods.Proceed);
			emitter.CodeBuilder.AddStatement(proceed);

			if (hasByRefArguments)
			{
				emitter.CodeBuilder.AddStatement(FinallyStatement.Instance);
			}

			argumentsMarshaller.CopyOut(argumentsArray);

			if (hasByRefArguments)
			{
				emitter.CodeBuilder.AddStatement(EndExceptionBlockStatement.Instance);
			}

			argumentsMarshaller.Return(invocationLocal);

			return emitter;
		}

		private IExpression SetMethodInterceptors(ClassEmitter @class, INamingScope namingScope, MethodEmitter emitter, IExpression proxiedMethodTokenExpression)
		{
			var selector = @class.GetField("__selector");
			if(selector == null)
			{
				return null;
			}

			var methodInterceptorsField = BuildMethodInterceptorsField(@class, MethodToOverride, namingScope);

			IExpression targetTypeExpression;
			if (getTargetTypeExpression != null)
			{
				targetTypeExpression = getTargetTypeExpression(@class, MethodToOverride);
			}
			else
			{
				targetTypeExpression = new MethodInvocationExpression(null, TypeUtilMethods.GetTypeOrNull, getTargetExpression(@class, MethodToOverride));
			}

			var emptyInterceptors = new NewArrayExpression(0, typeof(IInterceptor));
			var selectInterceptors = new MethodInvocationExpression(selector, InterceptorSelectorMethods.SelectInterceptors,
			                                                        targetTypeExpression,
			                                                        proxiedMethodTokenExpression, interceptors)
			{ VirtualCall = true };

			emitter.CodeBuilder.AddStatement(
				new IfNullExpression(methodInterceptorsField,
				                     new AssignStatement(methodInterceptorsField,
				                                         new NullCoalescingOperatorExpression(selectInterceptors, emptyInterceptors))));

			return methodInterceptorsField;
		}

		private void EmitLoadGenericMethodArguments(MethodEmitter methodEmitter, MethodInfo method, Reference invocationLocal)
		{
			var genericParameters = Array.FindAll(method.GetGenericArguments(), t => t.IsGenericParameter);
			var genericParamsArrayLocal = methodEmitter.CodeBuilder.DeclareLocal(typeof(Type[]));
			methodEmitter.CodeBuilder.AddStatement(
				new AssignStatement(genericParamsArrayLocal, new NewArrayExpression(genericParameters.Length, typeof(Type))));

			for (var i = 0; i < genericParameters.Length; ++i)
			{
				methodEmitter.CodeBuilder.AddStatement(
					new AssignStatement(
						new ArrayElementReference(genericParamsArrayLocal, i),
						new TypeTokenExpression(genericParameters[i])));
			}
			methodEmitter.CodeBuilder.AddStatement(
				new MethodInvocationExpression(invocationLocal,
				                               InvocationMethods.SetGenericMethodArguments,
				                               genericParamsArrayLocal));
		}

		private IExpression[] GetCtorArguments(ClassEmitter @class, IExpression proxiedMethodTokenExpression, LocalReference argumentsArray, IExpression methodInterceptors)
		{
			return new[]
			{
				getTargetExpression(@class, MethodToOverride),
				ThisExpression.Instance,
				methodInterceptors ?? interceptors,
				proxiedMethodTokenExpression,
				argumentsArray,
			};
		}

		private IExpression[] ModifyArguments(ClassEmitter @class, IExpression[] arguments)
		{
			if (contributor == null)
			{
				return arguments;
			}

			return contributor.GetConstructorInvocationArguments(arguments, @class);
		}

		private bool HasByRefArguments(ArgumentReference[] arguments)
		{
			for (int i = 0; i < arguments.Length; i++ )
			{
				if (arguments[i].Type.IsByRef)
				{
					return true;
				}
			}

			return false;
		}

		private struct ArgumentsMarshaller
		{
			private readonly MethodEmitter method;
			private readonly ParameterInfo[] parameters;

			public ArgumentsMarshaller(MethodEmitter method, ParameterInfo[] parameters)
			{
				this.method = method;
				this.parameters = parameters;
			}

			public void CopyIn(out LocalReference argumentsArray)
			{
				var arguments = method.Arguments;

				argumentsArray = method.CodeBuilder.DeclareLocal(typeof(object[]));

				method.CodeBuilder.AddStatement(
					new AssignStatement(
						argumentsArray,
						new NewArrayExpression(arguments.Length, typeof(object))));

				for (int i = 0, n = arguments.Length; i < n; ++i)
				{
					var argument = arguments[i];
					Reference dereferencedArgument = argument.Type.IsByRef ? new IndirectReference(argument) : argument;
					var dereferencedArgumentType = dereferencedArgument.Type;

#if FEATURE_BYREFLIKE
					if (dereferencedArgumentType.IsByRefLikeSafe())
					{
						// The by-ref-like argument value cannot be put into the `object[]` array,
						// because it cannot be boxed. We need to replace it with some other value.

						// For now, we just erase it by substituting `null`:
						method.CodeBuilder.AddStatement(
							new AssignStatement(
								new ArrayElementReference(argumentsArray, i),
								NullExpression.Instance));
					}
					else
#endif
					{
						method.CodeBuilder.AddStatement(
							new AssignStatement(
								new ArrayElementReference(argumentsArray, i),
								new ConvertArgumentToObjectExpression(dereferencedArgument)));
					}
				}
			}

			public void CopyOut(LocalReference argumentsArray)
			{
				var arguments = method.Arguments;

				for (int i = 0, n = arguments.Length; i < n; ++i)
				{
					Debug.Assert(parameters[i].ParameterType == arguments[i].Type);

					if (parameters[i].IsByRef && !parameters[i].IsReadOnly)
					{
						var dereferencedArgument = new IndirectReference(arguments[i]);
						var dereferencedArgumentType = dereferencedArgument.Type;

#if FEATURE_BYREFLIKE
						if (dereferencedArgumentType.IsByRefLikeSafe())
						{
							// The argument value in the invocation `Arguments` array is an `object`
							// and cannot be converted back to its original by-ref-like type.
							// We need to replace it with some other value.

							// For now, we just substitute the by-ref-like type's default value:
							if (parameters[i].IsOut)
							{
								method.CodeBuilder.AddStatement(
									new AssignStatement(
										dereferencedArgument,
										new DefaultValueExpression(dereferencedArgumentType)));
							}
							else
							{
								// ... except when we're dealing with a `ref` parameter. Unlike with `out`,
								// where we would be expected to definitely assign to it, we are free to leave
								// the original incoming value untouched. For now, that's likely the better
								// interim solution than unconditionally resetting.
							}
						}
						else
#endif
						{
							method.CodeBuilder.AddStatement(
								new AssignStatement(
									dereferencedArgument,
									new ConvertArgumentFromObjectExpression(
										new ArrayElementReference(argumentsArray, i),
										dereferencedArgumentType)));
						}
					}
				}
			}

			public void Return(LocalReference invocation)
			{
				var returnType = method.ReturnType;

				if (returnType == typeof(void))
				{
					method.CodeBuilder.AddStatement(ReturnStatement.Instance);
					return;
				}

#if FEATURE_BYREFLIKE
				if (returnType.IsByRefLikeSafe())
				{
					// The return value in the `ReturnValue` property is an `object`
					// and cannot be converted back to the original by-ref-like return type.
					// We need to replace it with some other value.

					// For now, we just substitute the by-ref-like type's default value:
					method.CodeBuilder.AddStatement(
						new ReturnStatement(
							new DefaultValueExpression(returnType)));
				}
				else
#endif
				{
					var returnValue = method.CodeBuilder.DeclareLocal(typeof(object));
					method.CodeBuilder.AddStatement(
						new AssignStatement(
							returnValue,
							new MethodInvocationExpression(invocation, InvocationMethods.GetReturnValue)));

					// Emit code to ensure a value type return type is not null, otherwise the cast will cause a null-deref
					if (returnType.IsValueType && !returnType.IsNullableType())
					{
						method.CodeBuilder.AddStatement(
							new IfNullExpression(
								returnValue,
								new ThrowStatement(
									typeof(InvalidOperationException),
									"Interceptors failed to set a return value, or swallowed the exception thrown by the target")));
					}

					method.CodeBuilder.AddStatement(
						new ReturnStatement(
							new ConvertArgumentFromObjectExpression(returnValue, returnType)));
				}
			}
		}
	}
}