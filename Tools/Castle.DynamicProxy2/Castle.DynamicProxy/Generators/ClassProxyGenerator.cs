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
	using System.Collections.Generic;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Threading;
	using System.Xml.Serialization;
	using Castle.Core.Interceptor;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	/// <summary>
	/// 
	/// </summary>
	public class ClassProxyGenerator : BaseProxyGenerator
	{
		public ClassProxyGenerator(ModuleScope scope, Type targetType) : base(scope, targetType)
		{
		}

		public Type GenerateCode(Type[] interfaces, ProxyGenerationOptions options)
		{
			Type type;
			
			ReaderWriterLock rwlock = Scope.RWLock;

			rwlock.AcquireReaderLock(-1);

			CacheKey cacheKey = new CacheKey(targetType, interfaces, options);

			Type cacheType = GetFromCache(cacheKey);

			if (cacheType != null)
			{
				rwlock.ReleaseReaderLock();

				return cacheType;
			}

			LockCookie lc = rwlock.UpgradeToWriterLock(-1);

			try
			{
				cacheType = GetFromCache(cacheKey);

				if (cacheType != null)
				{
					return cacheType;
				}
				
				generationHook = options.Hook;
				
				String newName = targetType.Name + "Proxy" + Guid.NewGuid().ToString("N");
				
				// Add Interfaces that the proxy implements 

				ArrayList interfaceList = new ArrayList();

				if (interfaces != null)
				{
					interfaceList.AddRange(interfaces);
				}

				AddDefaultInterfaces(interfaceList);

				ClassEmitter emitter = BuildClassEmitter(newName, targetType, interfaceList);
				emitter.DefineCustomAttribute(new XmlIncludeAttribute(targetType));

				// Custom attributes
				
				ReplicateNonInheritableAttributes(targetType, emitter);

				// Implement builtin Interfaces

				ImplementProxyTargetAccessor(targetType, emitter);

				// Fields generations

				FieldReference interceptorsField =
					emitter.CreateField("__interceptors", typeof(IInterceptor[]));
				
				emitter.DefineCustomAttributeFor(interceptorsField, new XmlIgnoreAttribute());

				// Collect methods

				PropertyToGenerate[] propsToGenerate;
				MethodInfo[] methods = CollectMethodsAndProperties(emitter, targetType, out propsToGenerate);

				options.Hook.MethodsInspected();

				// Constructor

				ConstructorEmitter typeInitializer = GenerateStaticConstructor(emitter);

				CreateInitializeCacheMethodBody(targetType, methods, emitter, typeInitializer);
				GenerateConstructors(emitter, targetType, interceptorsField);
				GenerateParameterlessConstructor(emitter, targetType, interceptorsField);

				// Implement interfaces

				if (interfaces != null && interfaces.Length != 0)
				{
					foreach(Type inter in interfaces)
					{
						ImplementBlankInterface(targetType, inter, emitter, interceptorsField, typeInitializer);
					}
				}

				// Create callback methods

				Dictionary<MethodInfo, MethodBuilder> method2Callback = new Dictionary<MethodInfo, MethodBuilder>();

				foreach(MethodInfo method in methods)
				{
					method2Callback[method] = CreateCallbackMethod(emitter, method, method);
				}

				// Create invocation types

				Dictionary<MethodInfo, NestedClassEmitter> method2Invocation = new Dictionary<MethodInfo, NestedClassEmitter>();

				foreach(MethodInfo method in methods)
				{
					MethodBuilder callbackMethod = method2Callback[method];

					method2Invocation[method] = BuildInvocationNestedType(emitter, targetType,
					                                                      emitter.TypeBuilder,
																		  method, callbackMethod, 
					                                                      ConstructorVersion.WithoutTargetMethod);
				}

				// Create methods overrides

				Dictionary<MethodInfo, MethodEmitter> method2Emitter = new Dictionary<MethodInfo, MethodEmitter>();

				foreach(MethodInfo method in methods)
				{
					if (method.IsSpecialName && (method.Name.StartsWith("get_") || method.Name.StartsWith("set_")))
					{
						continue;
					}
					
					NestedClassEmitter nestedClass = method2Invocation[method];

					// TODO: Should the targetType be a generic definition or instantiation?

					MethodEmitter newProxiedMethod = CreateProxiedMethod(
						targetType, method, emitter, nestedClass, interceptorsField, SelfReference.Self, 
						ConstructorVersion.WithoutTargetMethod, null);

					ReplicateNonInheritableAttributes(method, newProxiedMethod);
					
					method2Emitter[method] = newProxiedMethod;
				}

				foreach(PropertyToGenerate propToGen in propsToGenerate)
				{
					if (propToGen.CanRead)
					{
						NestedClassEmitter nestedClass = method2Invocation[propToGen.GetMethod];

						MethodAttributes atts = ObtainMethodAttributes(propToGen.GetMethod);

						MethodEmitter getEmitter = propToGen.Emitter.CreateGetMethod(atts);

						ImplementProxiedMethod(targetType, getEmitter,
						                       propToGen.GetMethod, emitter,
											   nestedClass, interceptorsField, SelfReference.Self, 
						                       ConstructorVersion.WithoutTargetMethod, null);

						ReplicateNonInheritableAttributes(propToGen.GetMethod, getEmitter);
					}
					
					if (propToGen.CanWrite)
					{
						NestedClassEmitter nestedClass = method2Invocation[propToGen.SetMethod];

						MethodAttributes atts = ObtainMethodAttributes(propToGen.SetMethod);

						MethodEmitter setEmitter = propToGen.Emitter.CreateSetMethod(atts);

						ImplementProxiedMethod(targetType, setEmitter,
						                       propToGen.SetMethod, emitter,
											   nestedClass, interceptorsField, SelfReference.Self, 
						                       ConstructorVersion.WithoutTargetMethod, null);

						ReplicateNonInheritableAttributes(propToGen.SetMethod, setEmitter);
					}
				}
				
				// Complete type initializer code body

				CompleteInitCacheMethod(typeInitializer.CodeBuilder);
				
				// Build type

				type = emitter.BuildType();

				AddToCache(cacheKey, type);
			}
			finally
			{
				rwlock.DowngradeFromWriterLock(ref lc);
			}

			Scope.SaveAssembly();

			return type;
		}

		protected override Reference GetProxyTargetReference()
		{
			return SelfReference.Self;
		}

		protected override bool CanOnlyProxyVirtual()
		{
			return true;
		}
	}
}