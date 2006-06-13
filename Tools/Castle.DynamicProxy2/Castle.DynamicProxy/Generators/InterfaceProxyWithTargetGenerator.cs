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

	
	public class InterfaceProxyWithTargetGenerator : BaseProxyGenerator
	{
		private Type targetType;
		private FieldReference targetField;

		public InterfaceProxyWithTargetGenerator(ModuleScope scope) : base(scope)
		{
			canOnlyProxyVirtuals = false;
		}

		public Type GenerateCode(Type theInterface, Type targetType, ProxyGenerationOptions options)
		{
			ReaderWriterLock rwlock = Scope.RWLock;

			rwlock.AcquireReaderLock(-1);

#if DOTNET2
			if (theInterface.IsGenericType)
			{
				theInterface = theInterface.GetGenericTypeDefinition();
			}

			if (targetType.IsGenericType)
			{
				targetType = targetType.GetGenericTypeDefinition();
			}
#endif

			CacheKey cacheKey = new CacheKey(theInterface, new Type[] { targetType }, options);

			Type cacheType = GetFromCache(cacheKey);

			if (cacheType != null)
			{
				rwlock.ReleaseReaderLock();

				return cacheType;
			}

			this.targetType = targetType;

			LockCookie lc = rwlock.UpgradeToWriterLock(-1);

			try
			{
				emitter = BuildClassEmitter(Guid.NewGuid().ToString("N"), baseType, new Type[] { theInterface });

				GenerateFields();

				IProxyGenerationHook hook = options.Hook;

				GenerateMethods(theInterface, targetField, hook, options.UseSelector);

				// TODO: Add interfaces and mixins

				// hook.MethodsInspected();

				GenerateConstructor();
				// GenerateIProxyTargetAccessor();

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

				Type type = CreateType();

				AddToCache(cacheKey, type);

				return type;
			}
			finally
			{
				rwlock.DowngradeFromWriterLock(ref lc);

				Scope.SaveAssembly();
			}
		}

		protected new void GenerateMethods(Type interfaceType, Reference targetRef, 
		                                   IProxyGenerationHook hook, bool useSelector)
		{
			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

			MethodInfo[] methods = interfaceType.GetMethods(flags);

			foreach (MethodInfo method in methods)
			{
				bool shouldIntercept = hook.ShouldInterceptMethod(interfaceType, method);

				if (!shouldIntercept)
				{
					continue;
				}

				if (shouldIntercept && canOnlyProxyVirtuals && !method.IsVirtual)
				{
					hook.NonVirtualMemberNotification(baseType, method);
					continue;
				}

				ParameterInfo[] parametersInfo = method.GetParameters();

				Type[] parameters = new Type[parametersInfo.Length];

				for (int i = 0; i < parametersInfo.Length; i++)
				{
					parameters[i] = parametersInfo[i].ParameterType;
				}

				MethodAttributes atts = ObtainMethodAttributes(method);

				MethodInfo methodOnTarget = targetType.GetMethod(method.Name, parameters);

				NestedClassEmitter iinvocationImpl =
					CreateIInvocationImplementation(interfaceType, targetField.Reference.FieldType, method, methodOnTarget);
				// NestedClassEmitter iinvocationImpl = CreateIInvocationImplementation(baseType, method);

				MethodEmitter methodEmitter = emitter.CreateMethod(method.Name,
					atts, new ReturnReferenceExpression(method.ReturnType), parameters);

				methodEmitter.DefineParameters(parametersInfo);

				TypeReference[] dereferencedArguments = IndirectReference.WrapIfByRef(methodEmitter.Arguments);

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
				MethodTokenExpression methodTokenExp = new MethodTokenExpression(method, interfaceType);
				MethodTokenExpression interMethodTokenExp = new MethodTokenExpression(methodOnTarget, targetType);

				NewInstanceExpression newInvocImpl = new NewInstanceExpression(
					iinvocationImpl.Constructors[0].Builder,
					targetRef.ToExpression(),
					interceptors,
					typeExp,
					methodTokenExp,
					interMethodTokenExp, 
					new ReferencesToObjectArrayExpression(dereferencedArguments));

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

		protected override void GenerateFields()
		{
			base.GenerateFields();

			targetField = emitter.CreateField("__target", targetType.MakeGenericType(emitter.GenericTypeParams));
		}

		protected override Reference GetProxyTargetReference()
		{
			return targetField;
		}

		private void GenerateConstructor()
		{
			ArgumentReference cArg0 = new ArgumentReference(targetType.MakeGenericType(emitter.GenericTypeParams));
			ArgumentReference cArg1 = new ArgumentReference(typeof(IInterceptor[]));

			ConstructorEmitter constructor = emitter.CreateConstructor(cArg0, cArg1);

			constructor.CodeBuilder.AddStatement(new AssignStatement(targetField, cArg0.ToExpression()));
			constructor.CodeBuilder.AddStatement(new AssignStatement(interceptorsField, cArg1.ToExpression()));
			constructor.CodeBuilder.InvokeBaseConstructor();
			constructor.CodeBuilder.AddStatement(new ReturnStatement());
		}
	}
}
