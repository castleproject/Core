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
	using System.Diagnostics;
	using System.Reflection;
	using System.Reflection.Emit;
#if !Silverlight
	using System.Xml.Serialization;
#endif
	using Castle.Core.Interceptor;
	using Castle.DynamicProxy.Contributors;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Tokens;

	public class InterfaceMethodGenerator : InterfaceMethodGeneratorBase
	{
		private readonly NestedClassEmitter invocation;
		private readonly FieldReference interceptors;
		private readonly GetTargetExpressionDelegate getTargetExpression;

		public InterfaceMethodGenerator(MethodToGenerate method, NestedClassEmitter invocation, FieldReference interceptors, CreateMethodDelegate createMethod,GetTargetExpressionDelegate getTargetExpression)
		:base(method,createMethod)
		{
			Debug.Assert(method.MethodOnTarget != null, "method.MethodOnTarget != null");
			this.invocation = invocation;
			this.interceptors = interceptors;
			this.getTargetExpression = getTargetExpression;
		}

		protected override MethodEmitter ImplementProxiedMethod(MethodEmitter emitter, ClassEmitter @class, ProxyGenerationOptions options,INamingScope namingScope)
		{
			emitter.CopyParametersAndReturnTypeFrom(Method.Method, @class);

			Type invocationType = invocation.TypeBuilder;

			//TODO: can this ever happen? Should we throw instead?
			Trace.Assert(Method.Method.IsGenericMethod == invocationType.IsGenericTypeDefinition);

			Expression interfaceMethod;

			ConstructorInfo constructor = invocation.Constructors[0].ConstructorBuilder;
			Type[] genericMethodArgs = Type.EmptyTypes;
			string tokenFieldName = namingScope.GetUniqueName("token_" + Method.Method.Name);

			if (Method.Method.IsGenericMethod)
			{
				// bind generic method arguments to invocation's type arguments
				genericMethodArgs = emitter.MethodBuilder.GetGenericArguments();
				invocationType = invocationType.MakeGenericType(genericMethodArgs);

				constructor = TypeBuilder.GetConstructor(invocationType, constructor);

				// Not in the cache: generic method
				interfaceMethod = new MethodTokenExpression(Method.Method.MakeGenericMethod(genericMethodArgs));
				new MethodTokenExpression(Method.MethodOnTarget.MakeGenericMethod(genericMethodArgs));

			}
			else
			{
				var proxiedMethodToken = @class.CreateStaticField(tokenFieldName, typeof(MethodInfo));
				interfaceMethod = proxiedMethodToken.ToExpression();

				var cctor = @class.ClassConstructor;
				cctor.CodeBuilder.AddStatement(new AssignStatement(proxiedMethodToken, new MethodTokenExpression(Method.Method)));
			}


			var dereferencedArguments = IndirectReference.WrapIfByRef(emitter.Arguments);

			Expression[] ctorArguments;

			if (options.Selector == null)
			{
				ctorArguments = new[]
				{
					getTargetExpression(@class, Method.Method),
					new TypeTokenExpression(Method.MethodOnTarget.DeclaringType),
					SelfReference.Self.ToExpression(),
					interceptors.ToExpression(),
					interfaceMethod,
					new ReferencesToObjectArrayExpression(dereferencedArguments)
				};
			}
			else
			{
				ctorArguments = new[]
				{
					getTargetExpression(@class, Method.Method),
					new TypeTokenExpression(Method.MethodOnTarget.DeclaringType),
					SelfReference.Self.ToExpression(),
					interceptors.ToExpression(),
					interfaceMethod,
					new ReferencesToObjectArrayExpression(dereferencedArguments),
					BuildGetSelectorInvocation(@class),
					new AddressOfReferenceExpression(BuildMethodInterceptorsFiled(@class, tokenFieldName))
				};
			}

			var invocationLocal = emitter.CodeBuilder.DeclareLocal(invocationType);
			emitter.CodeBuilder.AddStatement(new AssignStatement(invocationLocal,
			                                                     new NewInstanceExpression(constructor, ctorArguments)));

			if (Method.Method.ContainsGenericParameters)
			{
				EmitLoadGenricMethodArguments(emitter, Method.Method.MakeGenericMethod(genericMethodArgs), invocationLocal);
			}

			emitter.CodeBuilder.AddStatement(
				new ExpressionStatement(new MethodInvocationExpression(invocationLocal, InvocationMethods.Proceed)));

			GeneratorUtil.CopyOutAndRefParameters(dereferencedArguments, invocationLocal, Method.Method, emitter);

			if (Method.Method.ReturnType != typeof(void))
			{
				// Emit code to return with cast from ReturnValue
				var getRetVal = new MethodInvocationExpression(invocationLocal, InvocationMethods.GetReturnValue);
				emitter.CodeBuilder.AddStatement(new ReturnStatement(new ConvertExpression(emitter.ReturnType, getRetVal)));
			}
			else
			{
				emitter.CodeBuilder.AddStatement(new ReturnStatement());
			}

			return emitter;
		}

		private MethodInvocationExpression BuildGetSelectorInvocation(ClassEmitter @class)
		{
			return new MethodInvocationExpression(
				@class.GetField("proxyGenerationOptions"),
				ProxyGenerationOptionsMethods.GetSelector) { VirtualCall = true };
		}

		private FieldReference BuildMethodInterceptorsFiled(ClassEmitter @class, string tokenFieldName)
		{
			FieldReference methodInterceptors = @class.CreateField(string.Format("{0}_interceptors", tokenFieldName),
			                                                       typeof(IInterceptor[]), false);
#if !SILVERLIGHT
			@class.DefineCustomAttributeFor<XmlIgnoreAttribute>(methodInterceptors);
#endif
			return methodInterceptors;
		}

		private void EmitLoadGenricMethodArguments(MethodEmitter methodEmitter, MethodInfo method,
		                                           LocalReference invocationImplLocal)
		{
#if SILVERLIGHT
			Type[] genericParameters =
				Castle.Core.Extensions.SilverlightExtensions.FindAll(method.GetGenericArguments(), delegate(Type t) { return t.IsGenericParameter; });
#else
			Type[] genericParameters = Array.FindAll(method.GetGenericArguments(), t => t.IsGenericParameter);
#endif
			LocalReference genericParamsArrayLocal = methodEmitter.CodeBuilder.DeclareLocal(typeof(Type[]));
			methodEmitter.CodeBuilder.AddStatement(
				new AssignStatement(genericParamsArrayLocal, new NewArrayExpression(genericParameters.Length, typeof(Type))));

			for (int i = 0; i < genericParameters.Length; ++i)
			{
				methodEmitter.CodeBuilder.AddStatement(
					new AssignArrayStatement(genericParamsArrayLocal, i, new TypeTokenExpression(genericParameters[i])));
			}
			methodEmitter.CodeBuilder.AddStatement(new ExpressionStatement(
			                                       	new MethodInvocationExpression(invocationImplLocal, InvocationMethods.SetGenericMethodArguments,
			                                       	                               new ReferenceExpression(
			                                       	                               	genericParamsArrayLocal))));
		}

	}
}