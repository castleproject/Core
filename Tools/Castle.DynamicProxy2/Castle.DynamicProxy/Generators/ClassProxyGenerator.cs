// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	using System.Reflection;
	using System.Threading;

	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	public class ClassProxyGenerator
	{
		private readonly ModuleScope scope;

		private Type baseType;
		private ClassEmitter emitter;
		private FieldReference interceptorsField;
//		private FieldReference interceptorSelectorField;
//		private FieldReference useSelectorField;
		private int nestedCounter;
		private bool canOnlyProxyVirtuals = true;

		public ClassProxyGenerator(ModuleScope scope)
		{
			this.scope = scope;
		}

		public Type GenerateCode(Type theClass, ProxyGenerationOptions options)
		{
			ReaderWriterLock rwlock = scope.RWLock;

			rwlock.AcquireReaderLock(-1);
			
#if DOTNET2
			if (theClass.IsGenericType)
			{
				theClass = theClass.GetGenericTypeDefinition();
			}
#endif

//			Type cacheType = GetFromCache(theClass, interfaces);
//			
//			if (cacheType != null)
//			{
//				rwlock.ReleaseReaderLock();
//
//				return cacheType;
//			}

			rwlock.UpgradeToWriterLock(-1);

			try
			{
				baseType = theClass;

				emitter = BuildClassEmitter(Guid.NewGuid().ToString("N"), theClass);

				// baseType = baseType.MakeGenericType(emitter.GenericTypeParams);

				GenerateFields();
				
				IProxyGenerationHook hook = options.Hook;
				
				GenerateMethods(theClass, hook, options.UseSelector);
				
				// TODO: Add interfaces and mixins
				
				hook.MethodsInspected();

				GenerateConstructor();
				GenerateIProxyTargetAccessor();

//				if (theClass.IsSerializable)
//				{
//					ImplementGetObjectData( interfaces );
//				}
//
//				GenerateInterfaceImplementation(interfaces);
//				GenerateConstructors(theClass);
//
//				if (_delegateToBaseGetObjectData)
//				{
//					GenerateSerializationConstructor();
//				}

				return CreateType();
			}
			finally
			{
				rwlock.ReleaseWriterLock();

				scope.SaveAssembly();
			}
		}

		private void GenerateIProxyTargetAccessor()
		{
			emitter.TypeBuilder.AddInterfaceImplementation(typeof(IProxyTargetAccessor));

			MethodEmitter methodEmitter = emitter.CreateMethod("GetTarget",
				MethodAttributes.Public | MethodAttributes.Virtual, 
			    new ReturnReferenceExpression(typeof(object)));

			methodEmitter.CodeBuilder.AddStatement(
				new ReturnStatement(SelfReference.Self));
		}

		private void GenerateMethods(Type targetType, IProxyGenerationHook hook, bool useSelector)
		{
			BindingFlags flags = BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance;

			MethodInfo[] methods = baseType.GetMethods(flags);

			foreach(MethodInfo method in methods)
			{
				bool shouldIntercept = hook.ShouldInterceptMethod(targetType, method);

				if (!shouldIntercept)
				{
					continue;
				}
				
				if (shouldIntercept && canOnlyProxyVirtuals && !method.IsVirtual)
				{
					hook.NonVirtualMemberNotification(baseType, method);
					continue;
				}
				
				NestedClassEmitter iinvocationImpl = CreateIInvocationImplementation(baseType, method);

				MethodEmitter methodEmitter = emitter.CreateMethod(method.Name, 
					MethodAttributes.Public|MethodAttributes.Virtual,
					new ReturnReferenceExpression(method.ReturnType));

				LocalReference invocationImplLocal = 
					methodEmitter.CodeBuilder.DeclareLocal(iinvocationImpl.TypeBuilder);

				// TODO: Initialize iinvocation instance 
				// with ordinary arguments and in and out arguments

				Expression interceptors = null;
				
				if (useSelector)
				{
					// TODO: Generate code that checks the return of selector
					// if no interceptors is returned, should we invoke the base.Method directly?
				}
				else
				{
					interceptors = interceptorsField.ToExpression();
				}

				TypeTokenExpression typeExp = new TypeTokenExpression(baseType);
				MethodTokenExpression methodTokenExp = new MethodTokenExpression(method, targetType);
				
				NewInstanceExpression newInvocImpl = new NewInstanceExpression(
					iinvocationImpl.Constructors[0].Builder, 
					SelfReference.Self.ToExpression(), 
					interceptors,
					typeExp,
					methodTokenExp);

				methodEmitter.CodeBuilder.AddStatement(new AssignStatement(invocationImplLocal, newInvocImpl));

				methodEmitter.CodeBuilder.AddStatement(new ExpressionStatement(
					new MethodInvocationExpression(invocationImplLocal, 
					Constants.AbstractInvocationProceed)));

				if (method.ReturnType != typeof(void))
				{
					// Emit code to return with cast from ReturnValue
					MethodInvocationExpression getRetVal =
						new MethodInvocationExpression(invocationImplLocal,
							typeof(AbstractInvocation).GetMethod("get_ReturnValue"));

					methodEmitter.CodeBuilder.AddStatement(new ReturnStatement(
						new ConvertExpression(method.ReturnType, getRetVal)));
				}

				methodEmitter.CodeBuilder.AddStatement(new ReturnStatement());
			}
		}

		private void GenerateConstructor()
		{
			ArgumentReference cArg0 = new ArgumentReference(typeof(IInterceptor[]));

			ConstructorEmitter constructor = emitter.CreateConstructor(cArg0);

			constructor.CodeBuilder.AddStatement(new AssignStatement(interceptorsField, cArg0.ToExpression()));
			constructor.CodeBuilder.InvokeBaseConstructor();
			constructor.CodeBuilder.AddStatement(new ReturnStatement());
		}

		protected virtual ClassEmitter BuildClassEmitter(String typeName, Type baseType)
		{			
			return new ClassEmitter(scope, typeName, baseType, new Type[0], false);
		}

		protected virtual Type CreateType()
		{
			Type newType = emitter.BuildType();

//			RegisterInCache(newType);

			return newType;
		}

		protected virtual void GenerateFields()
		{
			interceptorsField = emitter.CreateField("__interceptors", typeof(IInterceptor[]));
			// useSelectorField = emitter.CreateField("__useSelector", typeof(bool));
			// interceptorSelectorField = emitter.CreateField("__interceptorSelector", typeof(IInterceptorSelector));
		}

		private NestedClassEmitter CreateIInvocationImplementation(Type targetType, MethodInfo methodInfo)
		{
			nestedCounter++;

			NestedClassEmitter nested = new NestedClassEmitter(emitter, 
				"Invocation" + nestedCounter.ToString(), typeof(AbstractInvocation), new Type[0]);

#if DOTNET2
			nested.CreateGenericParameters(targetType);
#endif
			// Create the fields

			FieldReference targetField = nested.CreateField("target", targetType);

			// Create constructor

			ArgumentReference cArg0 = new ArgumentReference(targetType);
			ArgumentReference cArg1 = new ArgumentReference(typeof(IInterceptor[]));
			ArgumentReference cArg2 = new ArgumentReference(typeof(Type));
			ArgumentReference cArg3 = new ArgumentReference(typeof(MethodInfo));

			ConstructorEmitter constructor = nested.CreateConstructor(cArg0, cArg1, cArg2, cArg3);

			constructor.CodeBuilder.AddStatement(new AssignStatement(targetField, cArg0.ToExpression()));
			constructor.CodeBuilder.InvokeBaseConstructor(Constants.AbstractInvocationConstructor, 
				cArg1, cArg2, cArg3);
			constructor.CodeBuilder.AddStatement(new ReturnStatement());

			// InvokeMethodOnTarget implementation

			MethodEmitter method = nested.CreateMethod("InvokeMethodOnTarget", 
				new ReturnReferenceExpression(typeof(void)), 
					MethodAttributes.Public|MethodAttributes.Final|MethodAttributes.Virtual);

			LocalReference ret_local = null;
			MethodInvocationExpression baseMethodInvExp = new MethodInvocationExpression(targetField, methodInfo);
			
			if (methodInfo.ReturnType != typeof(void))
			{
				ret_local = method.CodeBuilder.DeclareLocal(typeof(object));
				method.CodeBuilder.AddStatement(
					new AssignStatement(ret_local, 
						new ConvertExpression(typeof(object), methodInfo.ReturnType, baseMethodInvExp)));
			}
			else
			{
				method.CodeBuilder.AddStatement(new ExpressionStatement(baseMethodInvExp));
			}

			if (methodInfo.ReturnType != typeof(void))
			{
				MethodInvocationExpression setRetVal = 
					new MethodInvocationExpression(SelfReference.Self,
						typeof(AbstractInvocation).GetMethod("set_ReturnValue"), ret_local.ToExpression());

				method.CodeBuilder.AddStatement(new ExpressionStatement(setRetVal));
			}
			
			method.CodeBuilder.AddStatement(new ReturnStatement());

			return nested;
		}
	}
}