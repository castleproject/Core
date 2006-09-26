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
	using System.Threading;
	using System.Xml.Serialization;
	
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	
	/// <summary>
	/// 
	/// </summary>
	public class InterfaceProxyWithTargetGenerator : BaseProxyGenerator
	{
		private FieldReference targetField;

		public InterfaceProxyWithTargetGenerator(ModuleScope scope, Type theInterface) : base(scope, theInterface)
		{
		}
		
		public Type GenerateCode(Type proxyTargetType, ProxyGenerationOptions options)
		{
			Type generatedType;

			ReaderWriterLock rwlock = Scope.RWLock;

			rwlock.AcquireReaderLock(-1);

			// CacheKey cacheKey = new CacheKey(targetType, new Type[] { proxyTargetType }, options);
			CacheKey cacheKey = new CacheKey(targetType, null, options);

			Type cacheType = GetFromCache(cacheKey);

			if (cacheType != null)
			{
				rwlock.ReleaseReaderLock();

				return cacheType;
			}

			LockCookie lc = rwlock.UpgradeToWriterLock(-1);

			try
			{
				generationHook = options.Hook;

				// String newName = Guid.NewGuid().ToString("N");
				String newName = "Proxy";

				// Add Interfaces that the proxy implements 

				ArrayList interfaceList = new ArrayList();

				interfaceList.Add(targetType);

//				if (interfaces != null)
//				{
//					interfaceList.AddRange(interfaces);
//				}

				AddDefaultInterfaces(interfaceList);

				ClassEmitter emitter = BuildClassEmitter(newName, options.BaseTypeForInterfaceProxy, interfaceList);
				emitter.DefineCustomAttribute(new XmlIncludeAttribute(targetType));

				// Custom attributes
				ReplicateNonInheritableAttributes(targetType, emitter);

				// Fields generations

				FieldReference interceptorsField = emitter.CreateField("__interceptors", typeof(IInterceptor[]));
				targetField = emitter.CreateField("__target", proxyTargetType);

				////emitter.DefineCustomAttributeFor(interceptorsField, new XmlIgnoreAttribute());
				////emitter.DefineCustomAttributeFor(targetField, new XmlIgnoreAttribute());

				// Implement builtin Interfaces

				ImplementProxyTargetAccessor(targetType, emitter);

				// Collect methods

				PropertyToGenerate[] propsToGenerate;
				MethodInfo[] methods = CollectMethodsAndProperties(emitter, targetType, out propsToGenerate);

				options.Hook.MethodsInspected();

				// Constructor

				initCacheMethod = CreateInitializeCacheMethod(targetType, methods, emitter);

				GenerateConstructor(initCacheMethod, emitter, interceptorsField, targetField);
				// GenerateParameterlessConstructor(initCacheMethod, emitter, interceptorsField);

				// Implement interfaces

//				if (interfaces != null && interfaces.Length != 0)
//				{
//					foreach(Type inter in interfaces)
//					{
//						ImplementBlankInterface(targetType, inter, emitter, interceptorsField);
//					}
//				}

				// Create invocation types

				Dictionary<MethodInfo, NestedClassEmitter> method2Invocation = new Dictionary<MethodInfo, NestedClassEmitter>();

				foreach(MethodInfo method in methods)
				{
					ParameterInfo[] parameters = method.GetParameters();
					Type[] argTypes = new Type[parameters.Length];
					
					for(int i=0; i < parameters.Length; i++)
					{
						argTypes[i] = parameters[i].ParameterType;
					}
					
					MethodInfo methodOnTarget = proxyTargetType.GetMethod(method.Name, argTypes);

					method2Invocation[method] = BuildInvocationNestedType(emitter, proxyTargetType,
																		  proxyTargetType,
																		  method, methodOnTarget);
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

					// TODO: Should the targetType be a generic definition or instantiation

					MethodEmitter newProxiedMethod = CreateProxiedMethod(
						targetType, method, emitter, nestedClass, interceptorsField, targetField);

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
											   nestedClass, interceptorsField, targetField);

						ReplicateNonInheritableAttributes(propToGen.GetMethod, getEmitter);
					}

					if (propToGen.CanWrite)
					{
						NestedClassEmitter nestedClass = method2Invocation[propToGen.SetMethod];

						MethodAttributes atts = ObtainMethodAttributes(propToGen.SetMethod);

						MethodEmitter setEmitter = propToGen.Emitter.CreateSetMethod(atts);

						ImplementProxiedMethod(targetType, setEmitter,
											   propToGen.SetMethod, emitter,
											   nestedClass, interceptorsField, targetField);

						ReplicateNonInheritableAttributes(propToGen.SetMethod, setEmitter);
					}
				}

				// Complete Initialize 

				CompleteInitCacheMethod(initCacheMethod);

				// Crosses fingers and build type

				generatedType = emitter.BuildType();

				AddToCache(cacheKey, generatedType);
			}
			finally
			{
				rwlock.DowngradeFromWriterLock(ref lc);
			}

			Scope.SaveAssembly();

			return generatedType;
		}
		
		protected override Reference GetProxyTargetReference()
		{
			return targetField;
		}

		protected override bool CanOnlyProxyVirtual()
		{
			return false;
		}
	}
}
