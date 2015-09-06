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
#if DOTNET45
	using System;
	using System.Diagnostics;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Threading.Tasks;
#if FEATURE_SERIALIZATION
	using System.Xml.Serialization;
#endif

	using Castle.Core.Internal;
	using Castle.DynamicProxy.Contributors;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Tokens;

    public class AsyncMethodWithInvocationGenerator : MethodGenerator
	{
		private readonly IInvocationCreationContributor contributor;
		private readonly GetTargetExpressionDelegate getTargetExpression;
		private readonly Reference interceptors;
		private readonly Type invocation;

		public AsyncMethodWithInvocationGenerator(MetaMethod method, Reference interceptors, Type invocation,
		                                     GetTargetExpressionDelegate getTargetExpression,
		                                     OverrideMethodDelegate createMethod, IInvocationCreationContributor contributor)
			: base(method, createMethod)
		{
			this.invocation = invocation;
			this.getTargetExpression = getTargetExpression;
			this.interceptors = interceptors;
			this.contributor = contributor;
		}

		protected FieldReference BuildMethodInterceptorsField(ClassEmitter @class, MethodInfo method, INamingScope namingScope)
		{
			var methodInterceptors = @class.CreateField(
				namingScope.GetUniqueName(string.Format("interceptors_{0}", method.Name)),
				typeof(IInterceptorBase[]),
				false);
#if FEATURE_SERIALIZATION
			@class.DefineCustomAttributeFor<XmlIgnoreAttribute>(methodInterceptors);
#endif
			return methodInterceptors;
		}

		protected override MethodEmitter BuildProxiedMethodBody(MethodEmitter emitter, ClassEmitter @class, ProxyGenerationOptions options, INamingScope namingScope)
		{
			var invocationType = invocation;

			Trace.Assert(MethodToOverride.IsGenericMethod == invocationType.GetTypeInfo().IsGenericTypeDefinition);
			var genericArguments = Type.EmptyTypes;

			var constructor = invocation.GetConstructors()[0];

			Expression proxiedMethodTokenExpression;
			if (MethodToOverride.IsGenericMethod)
			{
				// bind generic method arguments to invocation's type arguments
				genericArguments = emitter.MethodBuilder.GetGenericArguments();
				invocationType = invocationType.MakeGenericType(genericArguments);
				constructor = TypeBuilder.GetConstructor(invocationType, constructor);

				// Not in the cache: generic method
				proxiedMethodTokenExpression = new MethodTokenExpression(MethodToOverride.MakeGenericMethod(genericArguments));
			}
			else
			{
				var proxiedMethodToken = @class.CreateStaticField(namingScope.GetUniqueName("token_" + MethodToOverride.Name), typeof(MethodInfo));
				@class.ClassConstructor.CodeBuilder.AddStatement(new AssignStatement(proxiedMethodToken, new MethodTokenExpression(MethodToOverride)));

				proxiedMethodTokenExpression = proxiedMethodToken.ToExpression();
			}

			var methodInterceptors = SetMethodInterceptors(@class, namingScope, emitter, proxiedMethodTokenExpression);

			var dereferencedArguments = IndirectReference.WrapIfByRef(emitter.Arguments);

            if (HasByRefArguments(emitter.Arguments))
                throw new GeneratorException("Async proxy does not support out/ref arguments");

			var arguments = GetCtorArguments(@class, proxiedMethodTokenExpression, dereferencedArguments, methodInterceptors);
			var ctorArguments = ModifyArguments(@class, arguments);

			var invocationLocal = emitter.CodeBuilder.DeclareLocal(invocationType);
			emitter.CodeBuilder.AddStatement(new AssignStatement(invocationLocal,
			                                                     new NewInstanceExpression(constructor, ctorArguments)));

			if (MethodToOverride.ContainsGenericParameters)
			{
				EmitLoadGenricMethodArguments(emitter, MethodToOverride.MakeGenericMethod(genericArguments), invocationLocal);
			}

		    ExpressionStatement proceedStatement;
            if (emitter.ReturnType == typeof(Task))
            {
                proceedStatement = new ExpressionStatement(new MethodInvocationExpression(invocationLocal, InvocationMethods.AsyncProceedTask));
            }
            else if (emitter.ReturnType.IsGenericType && emitter.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                proceedStatement = new ExpressionStatement(new MethodInvocationExpression(invocationLocal, InvocationMethods.AsyncProceedTaskReturn.MakeGenericMethod(emitter.ReturnType.GenericTypeArguments[0])));
            }
            else
            {
                throw new GeneratorException("Return Type must be of type Task or Task<>");
            }
			emitter.CodeBuilder.AddStatement(proceedStatement);

    		emitter.CodeBuilder.AddStatement(new ReturnStatement(true));

			return emitter;
		}

		private Expression SetMethodInterceptors(ClassEmitter @class, INamingScope namingScope, MethodEmitter emitter, Expression proxiedMethodTokenExpression)
		{
			var selector = @class.GetField("__selector");
			if(selector == null)
			{
				return null;
			}

			var methodInterceptorsField = BuildMethodInterceptorsField(@class, MethodToOverride, namingScope);

			var emptyInterceptors = new NewArrayExpression(0, typeof(IInterceptorBase));
			var selectInterceptors = new MethodInvocationExpression(selector, InterceptorSelectorMethods.SelectInterceptors,
			                                                        new MethodInvocationExpression(null,
				                                                        TypeUtilMethods.GetTypeOrNull,
				                                                        getTargetExpression(@class, MethodToOverride)),
			                                                        proxiedMethodTokenExpression, interceptors.ToExpression())
			{ VirtualCall = true };

			emitter.CodeBuilder.AddExpression(
				new IfNullExpression(methodInterceptorsField,
				                     new AssignStatement(methodInterceptorsField,
				                                         new NullCoalescingOperatorExpression(selectInterceptors, emptyInterceptors))));

			return methodInterceptorsField.ToExpression();
		}

		private void EmitLoadGenricMethodArguments(MethodEmitter methodEmitter, MethodInfo method, Reference invocationLocal)
		{
			var genericParameters = method.GetGenericArguments().FindAll(t => t.GetTypeInfo().IsGenericParameter);
			var genericParamsArrayLocal = methodEmitter.CodeBuilder.DeclareLocal(typeof(Type[]));
			methodEmitter.CodeBuilder.AddStatement(
				new AssignStatement(genericParamsArrayLocal, new NewArrayExpression(genericParameters.Length, typeof(Type))));

			for (var i = 0; i < genericParameters.Length; ++i)
			{
				methodEmitter.CodeBuilder.AddStatement(
					new AssignArrayStatement(genericParamsArrayLocal, i, new TypeTokenExpression(genericParameters[i])));
			}
			methodEmitter.CodeBuilder.AddExpression(
				new MethodInvocationExpression(invocationLocal,
				                               InvocationMethods.SetArgumentValue,
				                               new ReferenceExpression(
					                               genericParamsArrayLocal)));
		}

		private Expression[] GetCtorArguments(ClassEmitter @class, Expression proxiedMethodTokenExpression, TypeReference[] dereferencedArguments, Expression methodInterceptors)
		{
			return new[]
			{
				getTargetExpression(@class, MethodToOverride),
				SelfReference.Self.ToExpression(),
				methodInterceptors ?? interceptors.ToExpression(),
				proxiedMethodTokenExpression,
				new ReferencesToObjectArrayExpression(dereferencedArguments)
			};
		}

		private Expression[] ModifyArguments(ClassEmitter @class, Expression[] arguments)
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
				if (arguments[i].Type.GetTypeInfo().IsByRef)
				{
					return true;
				}
			}

			return false;
		}
	}
#endif
}
