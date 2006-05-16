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
	using System.Collections;
	using System.Reflection;
	using System.Threading;

	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	public class ClassProxyGenerator
	{
		private readonly ModuleScope scope;

		private Type baseType;
		private Type[] interfaces;
		private ClassEmitter emitter;
		private FieldReference interceptorsField;
		private FieldReference interceptorSelectorField;
		private FieldReference useSelectorField;
		private int nestedCounter;

		public ClassProxyGenerator(ModuleScope scope)
		{
			this.scope = scope;
		}

		public Type GenerateCode(Type theClass, ProxyGenerationOptions options)
		{
			ReaderWriterLock rwlock = scope.RWLock;

			rwlock.AcquireReaderLock(-1);

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

				this.emitter = BuildClassEmitter(Guid.NewGuid().ToString("N"), theClass);

				GenerateFields();

				IList methods = ComputeTargetMembers(options.Hook);

				foreach(MethodInfo method in methods)
				{
					NestedClassEmitter iinvocationImpl = CreateIInvocationImplementation(baseType, method);

					MethodEmitter methodEmitter = emitter.CreateMethod(method.Name, 
						MethodAttributes.Public|MethodAttributes.Virtual, new ReturnReferenceExpression(typeof(void)));

					LocalReference invocationImplLocal = 
						methodEmitter.CodeBuilder.DeclareLocal(iinvocationImpl.TypeBuilder);

					// TODO: Initialize iinvocation instance 
					// with ordinary arguments and in and out arguments

					if (options.UseSelector)
					{
						
					}
					else
					{
						methodEmitter.CodeBuilder.AddStatement(new AssignStatement(invocationImplLocal, 
							new NewInstanceExpression(iinvocationImpl.Constructors[0].Builder, 
								SelfReference.Self.ToExpression(), 
								interceptorsField.ToExpression())));

						methodEmitter.CodeBuilder.AddStatement(new ExpressionStatement(
							new MethodInvocationExpression(invocationImplLocal, 
								Constants.AbstractInvocationProceed)));

						// TODO: If return type != void, process it here

						methodEmitter.CodeBuilder.AddStatement(new ReturnStatement());
					}
				}

				GenerateConstructor();

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

		private void GenerateConstructor()
		{
			ArgumentReference cArg0 = new ArgumentReference(typeof(IInterceptor[]));

			ConstructorEmitter constructor = emitter.CreateConstructor(cArg0);

			constructor.CodeBuilder.AddStatement(new AssignStatement(interceptorsField, cArg0.ToExpression()));
			constructor.CodeBuilder.InvokeBaseConstructor();
			constructor.CodeBuilder.AddStatement(new ReturnStatement());
		}

		private MethodInfo[] ComputeTargetMembers(IProxyGenerationHook hook)
		{
			ArrayList methodsApproved = new ArrayList();

			// TODO: interfaces and mixins need to go through the same process

			BindingFlags flags = BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance;

			MethodInfo[] methods = baseType.GetMethods(flags);

			foreach(MethodInfo method in methods)
			{
				bool shouldIntercept = hook.ShouldInterceptMethod(baseType, method);

				if (shouldIntercept)
				{
					if (!method.IsVirtual)
					{
						hook.NonVirtualMemberNotification(baseType, method);
						continue;
					}

					methodsApproved.Add(method);
				}
			}

			hook.MethodsInspected();

			return (MethodInfo[]) methodsApproved.ToArray(typeof(MethodInfo));
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
			useSelectorField = emitter.CreateField("__useSelector", typeof(bool));
			interceptorsField = emitter.CreateField("__interceptors", typeof(IInterceptor[]));
			interceptorSelectorField = emitter.CreateField("__interceptorSelector", typeof(IInterceptorSelector));
		}

		private NestedClassEmitter CreateIInvocationImplementation(Type targetType, MethodInfo methodInfo)
		{
			nestedCounter++;

			NestedClassEmitter nested = new NestedClassEmitter(emitter, 
				"Invocation" + nestedCounter.ToString(), typeof(AbstractInvocation), new Type[0]);

			// Create the fields

			FieldReference targetField = nested.CreateField("target", targetType);

			// Create constructor

			ArgumentReference cArg0 = new ArgumentReference(targetType);
			ArgumentReference cArg1 = new ArgumentReference(typeof(IInterceptor[]));

			ConstructorEmitter constructor = nested.CreateConstructor(cArg0, cArg1);

			constructor.CodeBuilder.AddStatement(new AssignStatement(targetField, cArg0.ToExpression()));
			constructor.CodeBuilder.InvokeBaseConstructor(Constants.AbstractInvocationConstructor, cArg1);
			constructor.CodeBuilder.AddStatement(new ReturnStatement());

			// InvokeMethodOnTarget implementation

			MethodEmitter method = nested.CreateMethod("InvokeMethodOnTarget", 
				new ReturnReferenceExpression(typeof(void)), 
					MethodAttributes.Public|MethodAttributes.Final|MethodAttributes.Virtual);

			method.CodeBuilder.AddStatement(new ExpressionStatement(
				new MethodInvocationExpression(targetField, methodInfo)));
			method.CodeBuilder.AddStatement(new ReturnStatement());

			return nested;
		}
	}
}