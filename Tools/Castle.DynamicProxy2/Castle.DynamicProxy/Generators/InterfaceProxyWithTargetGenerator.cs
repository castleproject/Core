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
		private readonly Type interfaceType;
		private readonly Type targetType2;
		
		private FieldReference targetField;

		public InterfaceProxyWithTargetGenerator(ModuleScope scope, Type theInterface, Type targetType)
			: base(scope, typeof(Object))
		{
			canOnlyProxyVirtuals = false;
			
			this.interfaceType = interfaceType;

			if (theInterface.IsGenericType)
			{
				theInterface = theInterface.GetGenericTypeDefinition();
			}

			if (targetType.IsGenericType)
			{
				targetType2 = targetType.GetGenericTypeDefinition();
			}
		}

		public Type GenerateCode(ProxyGenerationOptions options)
		{
			ReaderWriterLock rwlock = Scope.RWLock;

			rwlock.AcquireReaderLock(-1);

// #if DOTNET2
						
// #endif

			CacheKey cacheKey = new CacheKey(interfaceType, new Type[] { targetType2 }, options);

			Type cacheType = GetFromCache(cacheKey);

			if (cacheType != null)
			{
				rwlock.ReleaseReaderLock();

				return cacheType;
			}

			LockCookie lc = rwlock.UpgradeToWriterLock(-1);

			try
			{
				emitter = BuildClassEmitter(Guid.NewGuid().ToString("N"), 
				                            classBaseType, new Type[] { interfaceType });

				GenerateFields();

				IProxyGenerationHook hook = options.Hook;

				GenerateMethods(interfaceType, targetField, hook, options.UseSelector);

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

		protected override MethodInfo GetMethodOnTarget(MethodInfo method)
		{
			ParameterInfo[] parametersInfo = method.GetParameters();

			Type[] parameters = new Type[parametersInfo.Length];

			for (int i = 0; i < parametersInfo.Length; i++)
			{
				parameters[i] = parametersInfo[i].ParameterType;
			}

			return targetType2.GetMethod(method.Name, parameters);
		}
		
		protected override void GenerateFields()
		{
			base.GenerateFields();

			targetField = emitter.CreateField("__target", targetType2.MakeGenericType(emitter.GenericTypeParams));
		}

		protected override Reference GetProxyTargetReference()
		{
			return targetField;
		}

		private void GenerateConstructor()
		{
			ArgumentReference cArg0 = new ArgumentReference(targetType2.MakeGenericType(emitter.GenericTypeParams));
			ArgumentReference cArg1 = new ArgumentReference(typeof(IInterceptor[]));

			ConstructorEmitter constructor = emitter.CreateConstructor(cArg0, cArg1);

			constructor.CodeBuilder.AddStatement(new AssignStatement(targetField, cArg0.ToExpression()));
			constructor.CodeBuilder.AddStatement(new AssignStatement(interceptorsField, cArg1.ToExpression()));
			constructor.CodeBuilder.InvokeBaseConstructor();
			constructor.CodeBuilder.AddStatement(new ReturnStatement());
		}
	}
}
