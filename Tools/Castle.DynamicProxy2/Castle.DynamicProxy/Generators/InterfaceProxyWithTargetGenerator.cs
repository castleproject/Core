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
		
		public Type GenerateCode(Type proxyTargetType, Type[] interfaces, ProxyGenerationOptions options)
		{
			Type generatedType;

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
				generationHook = options.Hook;

				String newName = targetType.Name + "Proxy" + Guid.NewGuid().ToString("N");

				// Add Interfaces that the proxy implements 

				ArrayList interfaceList = new ArrayList();

				interfaceList.Add(targetType);

				if (interfaces != null)
				{
					interfaceList.AddRange(interfaces);
				}

				AddDefaultInterfaces(interfaceList);

				ClassEmitter emitter = BuildClassEmitter(newName, options.BaseTypeForInterfaceProxy, interfaceList);
				emitter.DefineCustomAttribute(new XmlIncludeAttribute(targetType));

				// Custom attributes
				ReplicateNonInheritableAttributes(targetType, emitter);

				// Fields generations

				FieldReference interceptorsField = emitter.CreateField("__interceptors", typeof(IInterceptor[]));
				targetField = emitter.CreateField("__target", proxyTargetType);

				emitter.DefineCustomAttributeFor(interceptorsField, new XmlIgnoreAttribute());
				emitter.DefineCustomAttributeFor(targetField, new XmlIgnoreAttribute());

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

				if (interfaces != null && interfaces.Length != 0)
				{
					foreach(Type inter in interfaces)
					{
						ImplementBlankInterface(targetType, inter, emitter, interceptorsField);
					}
				}

				// Create invocation types

				Dictionary<MethodInfo, NestedClassEmitter> method2Invocation = new Dictionary<MethodInfo, NestedClassEmitter>();

				Dictionary<MethodInfo, MethodInfo> method2methodOnTarget = new Dictionary<MethodInfo, MethodInfo>();

				foreach(MethodInfo method in methods)
				{
					MethodInfo methodOnTarget = FindMethodOnTargetType(method, proxyTargetType);

					method2methodOnTarget[method] = methodOnTarget;

					method2Invocation[method] = BuildInvocationNestedType(emitter, proxyTargetType,
																		  proxyTargetType,
																		  method, methodOnTarget, 
					                                                      ConstructorVersion.WithTargetMethod);
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
						targetType, method, emitter, nestedClass, interceptorsField, targetField, 
						ConstructorVersion.WithTargetMethod, method2methodOnTarget[method]);

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
											   nestedClass, interceptorsField, targetField,
											   ConstructorVersion.WithTargetMethod, method2methodOnTarget[propToGen.GetMethod]);

						ReplicateNonInheritableAttributes(propToGen.GetMethod, getEmitter);
					}

					if (propToGen.CanWrite)
					{
						NestedClassEmitter nestedClass = method2Invocation[propToGen.SetMethod];

						MethodAttributes atts = ObtainMethodAttributes(propToGen.SetMethod);

						MethodEmitter setEmitter = propToGen.Emitter.CreateSetMethod(atts);

						ImplementProxiedMethod(targetType, setEmitter,
											   propToGen.SetMethod, emitter,
											   nestedClass, interceptorsField, targetField, 
						                       ConstructorVersion.WithTargetMethod, method2methodOnTarget[propToGen.SetMethod]);

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

		private static MethodInfo FindMethodOnTargetType(MethodInfo methodOnInterface, Type proxyTargetType)
		{
#if DOTNET2
			// The code below assumes that the target
			// class uses the same generic arguments
			// as the interface generic arguments

			MemberInfo[] members = proxyTargetType.FindMembers(MemberTypes.Method,
												   BindingFlags.Public | BindingFlags.Instance,
				delegate(MemberInfo mi, object criteria)
				{
					if (mi.Name != criteria.ToString()) return false;

					MethodInfo methodInfo = (MethodInfo) mi;

					// Check return type equivalence

					if (methodInfo.ReturnType.IsGenericParameter != methodOnInterface.ReturnType.IsGenericParameter)
					{
						return false;
					}

					if (methodInfo.ReturnType.IsGenericParameter)
					{
						if (methodInfo.ReturnType.Name != methodOnInterface.ReturnType.Name)
						{
							return false;
						}
					}
					else
					{
						if (methodInfo.ReturnType != methodOnInterface.ReturnType)
						{
							return false;
						}
					}

					// Check parameters equivalence

					ParameterInfo[] sourceParams = methodOnInterface.GetParameters();
					ParameterInfo[] targetParams = methodInfo.GetParameters();

					if (sourceParams.Length != targetParams.Length)
					{
						return false;
					}

					for (int i = 0; i < sourceParams.Length; i++)
					{
						Type sourceParamType = sourceParams[i].ParameterType;
						Type targetParamType = targetParams[i].ParameterType;

						if (sourceParamType.IsGenericParameter != targetParamType.IsGenericParameter)
						{
							return false;
						}

						if (sourceParamType.IsGenericParameter)
						{
							if (sourceParamType.Name != targetParamType.Name)
							{
								return false;
							}
						}
						else
						{
							if (sourceParamType != targetParamType)
							{
								return false;
							}
						}
					}

					return true;
				}, methodOnInterface.Name);

			if (members.Length > 1)
			{
				throw new GeneratorException("Found more than one method on target " + proxyTargetType.FullName + " matching " + methodOnInterface.Name);
			}
			else if (members.Length == 0)
			{
				throw new GeneratorException("Could not find a matching method on " + proxyTargetType.FullName + ". Method " + methodOnInterface.Name);
			}

			return (MethodInfo) members[0];

#else
			ParameterInfo[] parameters = methodOnInterface.GetParameters();
			Type[] argTypes = new Type[parameters.Length];
			
			for(int i=0; i < parameters.Length; i++)
			{
				argTypes[i] = parameters[i].ParameterType;
			}

			return proxyTargetType.GetMethod(methodOnInterface.Name, argTypes);
#endif
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
