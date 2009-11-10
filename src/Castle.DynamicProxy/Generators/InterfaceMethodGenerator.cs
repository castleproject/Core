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

	public class InterfaceMethodGenerator : MethodGenerator
	{
		private readonly NestedClassEmitter invocation;
		private readonly FieldReference interceptors;
		private readonly CreateMethodDelegate createMethod;
		private readonly GetTargetExpressionDelegate getTargetExpression;
		private readonly MethodToGenerate method;

		public InterfaceMethodGenerator(MethodToGenerate method, NestedClassEmitter invocation, FieldReference interceptors, CreateMethodDelegate createMethod,GetTargetExpressionDelegate getTargetExpression)
		{
			Debug.Assert(method.Method.DeclaringType.IsInterface, "method.Method.DeclaringType.IsInterface");
			this.method = method;
			this.invocation = invocation;
			this.interceptors = interceptors;
			this.createMethod = createMethod;
			this.getTargetExpression = getTargetExpression;
		}

		public override MethodEmitter Generate(ClassEmitter @class, ProxyGenerationOptions options, INamingScope namingScope)
		{
			string name;
			MethodAttributes atts = ObtainMethodAttributes(out name);
			MethodEmitter methodEmitter = createMethod(name, atts);
			MethodEmitter proxiedMethod = ImplementProxiedMethod(methodEmitter,
			                                                     @class,
			                                                     options, namingScope);

			@class.TypeBuilder.DefineMethodOverride(methodEmitter.MethodBuilder, method.Method);
			return proxiedMethod;
		}

		private MethodAttributes ObtainMethodAttributes(out string name)
		{
			var methodInfo = method.Method;
			name = methodInfo.DeclaringType.Name + "." + methodInfo.Name;
			var attributes = MethodAttributes.Virtual |
			                 MethodAttributes.Private |
			                 MethodAttributes.HideBySig |
			                 MethodAttributes.NewSlot |
			                 MethodAttributes.Final;

			if (method.Standalone == false)
			{
				attributes |= MethodAttributes.SpecialName;
			}
			return attributes;
		}

		private MethodEmitter ImplementProxiedMethod(MethodEmitter emitter, ClassEmitter @class, ProxyGenerationOptions options,INamingScope namingScope)
		{
			emitter.CopyParametersAndReturnTypeFrom(method.Method, @class);

			TypeReference[] dereferencedArguments = IndirectReference.WrapIfByRef(emitter.Arguments);

			Type invocationType = invocation.TypeBuilder;

			Trace.Assert(method.Method.IsGenericMethod == invocationType.IsGenericTypeDefinition);


			Expression interfaceMethod;
			Expression targetMethod;
			bool isGenericInvocationClass = false;
			Type[] genericMethodArgs = Type.EmptyTypes;
			string tokenFieldName;
			Expression targetType;

			// TODO: clean up all these if / else
			if(method.MethodOnTarget!=null)
			{
				targetType = new TypeTokenExpression(method.MethodOnTarget.DeclaringType);
			}
			else
			{
				targetType = new TypeTokenExpression(method.Method.DeclaringType);
			}
			if (method.Method.IsGenericMethod)
			{
				// bind generic method arguments to invocation's type arguments
				genericMethodArgs = emitter.MethodBuilder.GetGenericArguments();
				invocationType = invocationType.MakeGenericType(genericMethodArgs);
				isGenericInvocationClass = true;

				// Not in the cache: generic method
				MethodInfo genericMethod = method.Method.MakeGenericMethod(genericMethodArgs);


				tokenFieldName = namingScope.GetUniqueName("token_" + method.Method.Name);
				interfaceMethod = new MethodTokenExpression(genericMethod);
				if (method.MethodOnTarget != null)
				{
					targetMethod = new MethodTokenExpression(method.MethodOnTarget.MakeGenericMethod(genericMethodArgs));
				}
				else
				{
					targetMethod = NullExpression.Instance;
				}
			}
			else
			{
				var cctor = @class.ClassConstructor;
				var interfaceMethodToken = @class.CreateStaticField(namingScope.GetUniqueName("token_" + method.Method.Name),
				                                                    typeof(MethodInfo));

				tokenFieldName = interfaceMethodToken.Reference.Name;
				interfaceMethod = interfaceMethodToken.ToExpression();
				cctor.CodeBuilder.AddStatement(new AssignStatement(interfaceMethodToken, new MethodTokenExpression(method.Method)));
				if (method.MethodOnTarget != null)
				{
					var targetMethodToken = @class.CreateStaticField(namingScope.GetUniqueName("token_" + method.MethodOnTarget.Name),
					                                                 typeof(MethodInfo));
					targetMethod = targetMethodToken.ToExpression();
					cctor.CodeBuilder.AddStatement(
						new AssignStatement(targetMethodToken, new MethodTokenExpression(method.MethodOnTarget)));
				}
				else
				{
					targetMethod = NullExpression.Instance;
				}
			}

			LocalReference invocationImplLocal = emitter.CodeBuilder.DeclareLocal(invocationType);

			// TODO: Initialize iinvocation instance with ordinary arguments and in and out arguments

			ConstructorInfo constructor = invocation.Constructors[0].ConstructorBuilder;
			if (isGenericInvocationClass)
			{
				constructor = TypeBuilder.GetConstructor(invocationType, constructor);
			}


			Expression target = getTargetExpression(@class, method.Method);

			NewInstanceExpression newInvocImpl;
			if (options.Selector == null)
			{
				newInvocImpl = //actual contructor call
					new NewInstanceExpression(constructor,
					                          target,
					                          SelfReference.Self.ToExpression(),
					                          interceptors.ToExpression(),
					                          // NOTE: there's no need to cache type token
					                          // profiling showed that it gives no real performance gain
					                          targetType,
					                          targetMethod,
					                          interfaceMethod,
					                          new ReferencesToObjectArrayExpression(dereferencedArguments));
			}
			else
			{
				// Create the field to store the selected interceptors for this method if an InterceptorSelector is specified
				// NOTE: If no interceptors are returned, should we invoke the base.Method directly? Looks like we should not.
				FieldReference methodInterceptors = @class.CreateField(string.Format("{0}_interceptors", tokenFieldName),
				                                                       typeof(IInterceptor[]), false);
#if !SILVERLIGHT
				@class.DefineCustomAttributeFor<XmlIgnoreAttribute>(methodInterceptors);
#endif

				var selector = new MethodInvocationExpression(
					@class.GetField("proxyGenerationOptions"),
					ProxyGenerationOptionsMethods.GetSelector) { VirtualCall = true };

				newInvocImpl = //actual contructor call
					new NewInstanceExpression(constructor,
					                          target,
					                          SelfReference.Self.ToExpression(),
					                          interceptors.ToExpression(),
					                          targetType,
					                          targetMethod,
					                          interfaceMethod,
					                          new ReferencesToObjectArrayExpression(dereferencedArguments),
					                          selector,
					                          new AddressOfReferenceExpression(methodInterceptors));
			}

			emitter.CodeBuilder.AddStatement(new AssignStatement(invocationImplLocal, newInvocImpl));

			if (method.Method.ContainsGenericParameters)
			{
				EmitLoadGenricMethodArguments(emitter, method.Method.MakeGenericMethod(genericMethodArgs), invocationImplLocal);
			}

			emitter.CodeBuilder.AddStatement(
				new ExpressionStatement(new MethodInvocationExpression(invocationImplLocal, InvocationMethods.Proceed)));

			CopyOutAndRefParameters(dereferencedArguments, invocationImplLocal, method.Method, emitter);

			if (method.Method.ReturnType != typeof(void))
			{
				// Emit code to return with cast from ReturnValue
				var getRetVal = new MethodInvocationExpression(invocationImplLocal, InvocationMethods.GetReturnValue);

				emitter.CodeBuilder.AddStatement(
					new ReturnStatement(new ConvertExpression(emitter.ReturnType, getRetVal)));
			}
			else
			{
				emitter.CodeBuilder.AddStatement(new ReturnStatement());
			}

			return emitter;
		}

		private static void CopyOutAndRefParameters(TypeReference[] dereferencedArguments, LocalReference invocationImplLocal, MethodInfo method, MethodEmitter methodEmitter)
		{
			var parameters = method.GetParameters();
			if(!ArgumentsUtil.IsAnyByRef(parameters))
			{
				return; //saving the need to create locals if there is no need
			}
			LocalReference invocationArgs = methodEmitter.CodeBuilder.DeclareLocal(typeof(object[]));
			methodEmitter.CodeBuilder.AddStatement(
				new AssignStatement(invocationArgs,
				                    new MethodInvocationExpression(invocationImplLocal, InvocationMethods.GetArguments)
					)
				);
			for (int i = 0; i < parameters.Length; i++)
			{
				if (parameters[i].ParameterType.IsByRef)
				{
					methodEmitter.CodeBuilder.AddStatement(
						new AssignStatement(dereferencedArguments[i],
						                    new ConvertExpression(dereferencedArguments[i].Type,
						                                          new LoadRefArrayElementExpression(i, invocationArgs)
						                    	)
							));
				}
			}
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