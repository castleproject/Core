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
	using System.Linq;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Runtime.CompilerServices;
#if FEATURE_SERIALIZATION
	using System.Xml.Serialization;
#endif

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

			argumentsMarshaller.CopyIn(out var argumentsArray, out var hasByRefArguments, out var hasByRefLikeArguments);

			var ctorArguments = GetCtorArguments(@class, proxiedMethodTokenExpression, argumentsArray, methodInterceptors);
			ctorArguments = ModifyArguments(@class, ctorArguments);

			var invocationLocal = emitter.CodeBuilder.DeclareLocal(invocationType);
			emitter.CodeBuilder.AddStatement(new AssignStatement(invocationLocal,
			                                                     new NewInstanceExpression(constructor, ctorArguments)));

			if (MethodToOverride.ContainsGenericParameters)
			{
				EmitLoadGenericMethodArguments(emitter, MethodToOverride.MakeGenericMethod(genericArguments), invocationLocal);
			}

			argumentsMarshaller.PrepareReturnValueBuffer(invocationLocal, out var returnValueBuffer);

			if (hasByRefArguments || hasByRefLikeArguments || returnValueBuffer != null)
			{
				emitter.CodeBuilder.AddStatement(TryStatement.Instance);
			}

			var proceed = new MethodInvocationExpression(invocationLocal, InvocationMethods.Proceed);
			emitter.CodeBuilder.AddStatement(proceed);

			argumentsMarshaller.GetReturnValue(invocationLocal, out var returnValue);

			if (hasByRefArguments || hasByRefLikeArguments || returnValueBuffer != null)
			{
				emitter.CodeBuilder.AddStatement(FinallyStatement.Instance);

				if (hasByRefArguments)
				{
					argumentsMarshaller.CopyOut(argumentsArray);
				}

				if (hasByRefLikeArguments)
				{
					argumentsMarshaller.InvalidateByRefLikeProxies(argumentsArray);
				}

				if (returnValueBuffer != null)
				{
					argumentsMarshaller.InvalidateReturnValueBuffer(invocationLocal, returnValueBuffer);
				}

				emitter.CodeBuilder.AddStatement(EndExceptionBlockStatement.Instance);
			}

			if (returnValue == null)
			{
				Debug.Assert(emitter.ReturnType == typeof(void));
				emitter.CodeBuilder.AddStatement(ReturnStatement.Instance);
			}
			else
			{
				Debug.Assert(emitter.ReturnType != typeof(void));
				Debug.Assert(returnValue.Type == emitter.ReturnType);
				emitter.CodeBuilder.AddStatement(new ReturnStatement(returnValue));
			}

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

		private struct ArgumentsMarshaller
		{
			private readonly MethodEmitter method;
			private readonly ParameterInfo[] parameters;

			public ArgumentsMarshaller(MethodEmitter method, ParameterInfo[] parameters)
			{
				this.method = method;
				this.parameters = parameters;
			}

			public void CopyIn(out LocalReference argumentsArray, out bool hasByRefArguments, out bool hasByRefLikeArguments)
			{
				var arguments = method.Arguments;

				argumentsArray = method.CodeBuilder.DeclareLocal(typeof(object[]));
				hasByRefArguments = false;
				hasByRefLikeArguments = false;

				method.CodeBuilder.AddStatement(
					new AssignStatement(
						argumentsArray,
						new NewArrayExpression(arguments.Length, typeof(object))));

				for (int i = 0, n = arguments.Length; i < n; ++i)
				{
					var argument = arguments[i];
					Reference dereferencedArgument;
					if (argument.Type.IsByRef)
					{
						dereferencedArgument = new IndirectReference(argument);
						hasByRefArguments = true;
					}
					else
					{
						dereferencedArgument = argument;
					}
					var dereferencedArgumentType = dereferencedArgument.Type;

#if FEATURE_BYREFLIKE
					if (dereferencedArgumentType.IsByRefLikeSafe())
					{
						hasByRefLikeArguments = true;

						// Byref-like values live exclusively on the stack and cannot be boxed to `object`.
						// Instead of them, we prepare instances of `ByRefLikeReference` wrappers that reference them.
						var referenceCtor = GetByRefLikeReferenceCtorFor(dereferencedArgumentType);
						var reference = method.CodeBuilder.DeclareLocal(typeof(ByRefLikeReference));
						method.CodeBuilder.AddStatement(
							new AssignStatement(
								reference,
								new NewInstanceExpression(
									referenceCtor,
									new TypeTokenExpression(dereferencedArgumentType),
									new AddressOfExpression(dereferencedArgument))));

						dereferencedArgument = reference;
					}
#endif

						method.CodeBuilder.AddStatement(
						new AssignStatement(
							new ArrayElementReference(argumentsArray, i),
							new ConvertArgumentToObjectExpression(dereferencedArgument)));
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

						// Note that we don't need special logic for byref-like values / `ByRefLikeReference` here,
						// since `ConvertArgumentFromObjectExpression` knows how to deal with those.

						method.CodeBuilder.AddStatement(
							new AssignStatement(
								dereferencedArgument,
								new ConvertArgumentFromObjectExpression(
									new ArrayElementReference(argumentsArray, i),
									dereferencedArgumentType)));
					}
				}
			}

			public void InvalidateByRefLikeProxies(LocalReference argumentsArray)
			{
#if FEATURE_BYREFLIKE
				var arguments = method.Arguments;

				for (int i = 0, n = arguments.Length; i < n; ++i)
				{
					var argument = arguments[i];
					var argumentType = argument.Type;
					var dereferencedArgumentType = argumentType.IsByRef ? argumentType.GetElementType()! : argumentType;

					if (dereferencedArgumentType.IsByRefLikeSafe())
					{
						// The `ByRefLikeReference` invocation argument must be rendered unusable
						// at the end of the (intercepted) method invocation, since it references
						// a method argument that is about to be popped off the stack.
						method.CodeBuilder.AddStatement(
							new MethodInvocationExpression(
								new AsTypeExpression(
									new ArrayElementReference(argumentsArray, i),
									typeof(ByRefLikeReference)),
								ByRefLikeReferenceMethods.Invalidate,
								argumentType.IsByRef ? argument : new AddressOfExpression(argument)));

						// Make the unusable substitute value unreachable by erasing it from `IInvocation.Arguments`.
						method.CodeBuilder.AddStatement(
							new AssignStatement(
								new ArrayElementReference(argumentsArray, i),
								NullExpression.Instance));
					}
				}
#endif
			}

			public void PrepareReturnValueBuffer(LocalReference invocation, out LocalReference returnValueBuffer)
			{
#if FEATURE_BYREFLIKE
				var returnType = method.ReturnType;

				// DynamicProxy does not (yet?) support `ref` returns. Revisit this method once it does!
				Debug.Assert(returnType.IsByRef == false);

				if (returnType.IsByRefLikeSafe() == false)
				{
					returnValueBuffer = null;
					return;
				}

				returnValueBuffer = method.CodeBuilder.DeclareLocal(returnType);

				var referenceCtor = GetByRefLikeReferenceCtorFor(returnType);
				method.CodeBuilder.AddStatement(
					new MethodInvocationExpression(
						invocation,
						InvocationMethods.SetReturnValue,
						new NewInstanceExpression(
							referenceCtor,
							new TypeTokenExpression(returnType),
							new AddressOfExpression(returnValueBuffer))));
#else
				returnValueBuffer = null;
#endif
			}

			public void InvalidateReturnValueBuffer(LocalReference invocation, LocalReference returnValueBuffer)
			{
#if FEATURE_BYREFLIKE
				Debug.Assert(returnValueBuffer != null);

				// The `ByRefLikeReference` return value must be rendered unusable
				// at the end of the (intercepted) method invocation, since it references
				// a local variable (the buffer) that is about to be popped off the stack.
				method.CodeBuilder.AddStatement(
					new MethodInvocationExpression(
						new AsTypeExpression(
							new MethodInvocationExpression(invocation, InvocationMethods.GetReturnValue),
							typeof(ByRefLikeReference)),
						ByRefLikeReferenceMethods.Invalidate,
						new AddressOfExpression(returnValueBuffer)));

				// Make the unusable proxy unreachable by erasing it from the invocation arguments array.
				method.CodeBuilder.AddStatement(
					new MethodInvocationExpression(invocation, InvocationMethods.SetReturnValue, NullExpression.Instance));
#endif
			}

			public void GetReturnValue(LocalReference invocation, out LocalReference returnValue)
			{
				var returnType = method.ReturnType;

				if (returnType == typeof(void))
				{
					returnValue = null;
					return;
				}

				var invocationReturnValue = method.CodeBuilder.DeclareLocal(typeof(object));
				method.CodeBuilder.AddStatement(
					new AssignStatement(
						invocationReturnValue,
						new MethodInvocationExpression(invocation, InvocationMethods.GetReturnValue)));

#if FEATURE_BYREFLIKE
				if (returnType.IsByRefLikeSafe() == false)
#endif
				{
					// Emit code to ensure a value type return type is not null, otherwise the cast will cause a null-deref
					if (returnType.IsValueType && !returnType.IsNullableType())
					{
						method.CodeBuilder.AddStatement(
							new IfNullExpression(
								invocationReturnValue,
								new ThrowStatement(
									typeof(InvalidOperationException),
									"Interceptors failed to set a return value, or swallowed the exception thrown by the target")));
					}
				}

				// Note that we don't need special logic for byref-like values / `ByRefLikeReference` here,
				// since `ConvertArgumentFromObjectExpression` knows how to deal with those.

				returnValue = method.CodeBuilder.DeclareLocal(returnType);
				method.CodeBuilder.AddStatement(
					new AssignStatement(
						returnValue,
						new ConvertArgumentFromObjectExpression(invocationReturnValue, returnType)));
			}

#if FEATURE_BYREFLIKE
			private static ConstructorInfo GetByRefLikeReferenceCtorFor(Type dereferencedArgumentType)
			{
#if NET9_0_OR_GREATER
				// TODO: perhaps we should cache these `ConstructorInfo`s?
				ConstructorInfo referenceCtor = typeof(ByRefLikeReference<>).MakeGenericType(dereferencedArgumentType).GetConstructors().Single();
#else
				ConstructorInfo referenceCtor = ByRefLikeReferenceMethods.Constructor;
#endif
				if (dereferencedArgumentType.IsConstructedGenericType)
				{
					var typeDef = dereferencedArgumentType.GetGenericTypeDefinition();
					if (typeDef == typeof(ReadOnlySpan<>))
					{
						var typeArg = dereferencedArgumentType.GetGenericArguments()[0];
						referenceCtor = typeof(ReadOnlySpanReference<>).MakeGenericType(typeArg).GetConstructors().Single();
					}
					else if (typeDef == typeof(Span<>))
					{
						var typeArg = dereferencedArgumentType.GetGenericArguments()[0];
						referenceCtor = typeof(SpanReference<>).MakeGenericType(typeArg).GetConstructors().Single();
					}
				}

				return referenceCtor;
			}
#endif
		}
	}
}