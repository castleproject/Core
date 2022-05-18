// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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

			var dereferencedArguments = IndirectReference.WrapIfByRef(emitter.Arguments);
			var hasByRefArguments = HasByRefArguments(emitter.Arguments);

			var arguments = GetCtorArguments(@class, proxiedMethodTokenExpression, dereferencedArguments, methodInterceptors);
			var ctorArguments = ModifyArguments(@class, arguments);

			var invocationLocal = emitter.CodeBuilder.DeclareLocal(invocationType);
			emitter.CodeBuilder.AddStatement(new AssignStatement(invocationLocal,
			                                                     new NewInstanceExpression(constructor, ctorArguments)));

			if (MethodToOverride.ContainsGenericParameters)
			{
				EmitLoadGenericMethodArguments(emitter, MethodToOverride.MakeGenericMethod(genericArguments), invocationLocal);
			}

			if (hasByRefArguments)
			{
				emitter.CodeBuilder.AddStatement(new TryStatement());
			}

			var proceed = new MethodInvocationExpression(invocationLocal, InvocationMethods.Proceed);
			emitter.CodeBuilder.AddStatement(proceed);

			if (hasByRefArguments)
			{
				emitter.CodeBuilder.AddStatement(new FinallyStatement());
			}

			GeneratorUtil.CopyOutAndRefParameters(dereferencedArguments, invocationLocal, MethodToOverride, emitter);

			if (hasByRefArguments)
			{
				emitter.CodeBuilder.AddStatement(new EndExceptionBlockStatement());
			}

			if (MethodToOverride.ReturnType != typeof(void))
			{
				var getRetVal = new MethodInvocationExpression(invocationLocal, InvocationMethods.GetReturnValue);

				// Emit code to ensure a value type return type is not null, otherwise the cast will cause a null-deref
				if (emitter.ReturnType.IsValueType && !emitter.ReturnType.IsNullableType())
				{
					LocalReference returnValue = emitter.CodeBuilder.DeclareLocal(typeof(object));
					emitter.CodeBuilder.AddStatement(new AssignStatement(returnValue, getRetVal));

					emitter.CodeBuilder.AddStatement(new IfNullExpression(returnValue, new ThrowStatement(typeof(InvalidOperationException),
						"Interceptors failed to set a return value, or swallowed the exception thrown by the target")));
				}

				// Emit code to return with cast from ReturnValue
				emitter.CodeBuilder.AddStatement(new ReturnStatement(new ConvertExpression(emitter.ReturnType, getRetVal)));
			}
			else
			{
				emitter.CodeBuilder.AddStatement(new ReturnStatement());
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
					new AssignArrayStatement(genericParamsArrayLocal, i, new TypeTokenExpression(genericParameters[i])));
			}
			methodEmitter.CodeBuilder.AddStatement(
				new MethodInvocationExpression(invocationLocal,
				                               InvocationMethods.SetGenericMethodArguments,
				                               genericParamsArrayLocal));
		}

		private IExpression[] GetCtorArguments(ClassEmitter @class, IExpression proxiedMethodTokenExpression, TypeReference[] dereferencedArguments, IExpression methodInterceptors)
		{
			return new[]
			{
				getTargetExpression(@class, MethodToOverride),
				SelfReference.Self,
				methodInterceptors ?? interceptors,
				proxiedMethodTokenExpression,
				new ReferencesToObjectArrayExpression(dereferencedArguments)
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
	}
}