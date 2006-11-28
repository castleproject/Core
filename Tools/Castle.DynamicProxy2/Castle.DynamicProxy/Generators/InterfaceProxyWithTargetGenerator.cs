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
	using Castle.Core.Interceptor;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	/// <summary>
	/// 
	/// </summary>
	[CLSCompliant(false)]
	public class InterfaceProxyWithTargetGenerator : BaseProxyGenerator
	{
		private FieldReference targetField;
		protected Dictionary<MethodInfo, NestedClassEmitter> method2Invocation = new Dictionary<MethodInfo, NestedClassEmitter>();

		protected Dictionary<MethodInfo, MethodInfo> method2methodOnTarget = new Dictionary<MethodInfo, MethodInfo>();

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
				if (cacheType != null)
				{
					return cacheType;
				}
				
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
				
				Type baseType = options.BaseTypeForInterfaceProxy;

				ClassEmitter emitter = BuildClassEmitter(newName, baseType, interfaceList);
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

				ConstructorEmitter typeInitializer = GenerateStaticConstructor(emitter);

				CreateInitializeCacheMethodBody(targetType, methods, emitter, typeInitializer);
				GenerateConstructors(emitter, baseType, interceptorsField, targetField);
				// GenerateParameterlessConstructor(emitter, interceptorsField, baseType);

				// Implement interfaces

				if (interfaces != null && interfaces.Length != 0)
				{
					foreach(Type inter in interfaces)
					{
						ImplementBlankInterface(targetType, inter, emitter, interceptorsField, typeInitializer);
					}
				}

				// Create invocation types
				
				foreach(MethodInfo method in methods)
				{
					CreateInvocationForMethod(emitter, method, proxyTargetType);
				}

				// Create methods overrides

				Dictionary<MethodInfo, MethodEmitter> method2Emitter = new Dictionary<MethodInfo, MethodEmitter>();

				foreach(MethodInfo method in methods)
				{
					if (method.IsSpecialName && 
					    (method.Name.StartsWith("get_") || method.Name.StartsWith("set_")))
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

					ParameterInfo[] parameters = method.GetParameters();
					// ParameterInfo[] parametersProxy = newProxiedMethod.MethodBuilder.GetParameters();
					
					bool useDefineOverride = true;

					for(int i = 0; i < parameters.Length; i++)
					{
						ParameterInfo paramInfo = parameters[i];
						// ParameterInfo paramInfo2 = parametersProxy[i];
						
						Console.WriteLine("{0} {1} {2} {3}", paramInfo.Name, paramInfo.ParameterType, paramInfo.Attributes, paramInfo.Position);
						// Console.WriteLine("{0} {1} {2} {3}", paramInfo2.Name, paramInfo2.ParameterType, paramInfo2.Attributes, paramInfo2.Position);
					}

					if (useDefineOverride)
					{
						// emitter.TypeBuilder.DefineMethodOverride(newProxiedMethod.MethodBuilder, method);
					}
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

						// emitter.TypeBuilder.DefineMethodOverride(getEmitter.MethodBuilder, propToGen.GetMethod);
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
						
						// emitter.TypeBuilder.DefineMethodOverride(setEmitter.MethodBuilder, propToGen.SetMethod);
					}
				}

				// Complete Initialize 

				CompleteInitCacheMethod(typeInitializer.CodeBuilder);

				// Crosses fingers and build type

				generatedType = emitter.BuildType();

				foreach(MethodInfo m in generatedType.GetMethods())
				{
					ParameterInfo[] parameters = m.GetParameters();

					Console.WriteLine(m.Name);

					for (int i = 0; i < parameters.Length; i++)
					{
						ParameterInfo paramInfo = parameters[i];

						Console.WriteLine("{0} {1} {2} {3}", paramInfo.Name, paramInfo.ParameterType, paramInfo.Attributes, paramInfo.Position);
						// Console.WriteLine("{0} {1} {2} {3}", paramInfo2.Name, paramInfo2.ParameterType, paramInfo2.Attributes, paramInfo2.Position);
					}
				}
				

				AddToCache(cacheKey, generatedType);
			}
			finally
			{
				rwlock.DowngradeFromWriterLock(ref lc);
			}

			Scope.SaveAssembly();

			return generatedType;
		}

		protected virtual void CreateInvocationForMethod(ClassEmitter emitter, MethodInfo method, Type proxyTargetType)
		{
			MethodInfo methodOnTarget = FindMethodOnTargetType(method, proxyTargetType);

			method2methodOnTarget[method] = methodOnTarget;

			method2Invocation[method] = BuildInvocationNestedType(emitter, proxyTargetType,
			                                                      proxyTargetType,
			                                                      method, methodOnTarget, 
			                                                      ConstructorVersion.WithTargetMethod);
		}

		/// <summary>
		/// Finds the type of the method on target.
		/// </summary>
		/// <param name="methodOnInterface">The method on interface.</param>
		/// <param name="proxyTargetType">Type of the proxy target.</param>
		/// <returns></returns>
		private static MethodInfo FindMethodOnTargetType(MethodInfo methodOnInterface, Type proxyTargetType)
		{
			
// #if DOTNET2
			
			// The code below assumes that the target
			// class uses the same generic arguments
			// as the interface generic arguments
			
			MemberInfo[] members = proxyTargetType.FindMembers(MemberTypes.Method,
												   BindingFlags.Public | BindingFlags.Instance,
				delegate(MemberInfo mi, object criteria)
				{
					if (mi.Name != criteria.ToString()) return false;

					MethodInfo methodInfo = (MethodInfo) mi;

					return IsEquivalentMethod(methodInfo, methodOnInterface);
					
				}, methodOnInterface.Name);

			
			if (members.Length == 0)
			{
				// Before throwing an exception, we look for an explicit
				// interface method implementation
				
				MethodInfo[] privateMethods = proxyTargetType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);

				foreach(MethodInfo methodInfo in privateMethods)
				{
					// We make sure it is a method used for explicit implementation

					if (!methodInfo.IsFinal || !methodInfo.IsVirtual || !methodInfo.IsHideBySig)
					{
						continue;
					}

					if (IsEquivalentMethod(methodInfo, methodOnInterface))
					{
						throw new GeneratorException(String.Format("DynamicProxy cannot create an interface (with target) " +
							"proxy for '{0}' as the target '{1}' has an explicit implementation of one of the methods exposed by the interface. " + 
							"The runtime prevents use from invoking the private method on the target. Method {2}", methodOnInterface.DeclaringType.Name, methodInfo.DeclaringType.Name, methodInfo.Name));
					}
				}
			}
			
			if (members.Length > 1)
			{
				throw new GeneratorException("Found more than one method on target " + proxyTargetType.FullName + " matching " + methodOnInterface.Name);
			}
			else if (members.Length == 0)
			{
				throw new GeneratorException("Could not find a matching method on " + proxyTargetType.FullName + ". Method " + methodOnInterface.Name);
			}

			return (MethodInfo) members[0];

#if !DOTNET2
			ParameterInfo[] parameters = methodOnInterface.GetParameters();
			Type[] argTypes = new Type[parameters.Length];
			
			for(int i=0; i < parameters.Length; i++)
			{
				argTypes[i] = parameters[i].ParameterType;
			}

			return proxyTargetType.GetMethod(methodOnInterface.Name, argTypes);
#endif
		}

		/// <summary>
		/// Checks whether the given types are the same. This is 
		/// more complicated than it looks.
		/// </summary>
		/// <param name="sourceType"></param>
		/// <param name="targetType"></param>
		/// <returns></returns>
		public static bool IsTypeEquivalent(Type sourceType, Type targetType)
		{
			if (sourceType.IsGenericParameter)
			{
				if (sourceType.Name != targetType.Name)
				{
					return false;
				}
			}
			else
			{
				if (sourceType.IsGenericType != targetType.IsGenericType)
				{
					return false;
				}
				else if (sourceType.IsArray != targetType.IsArray)
				{
					return false;
				}

				if (sourceType.IsGenericType)
				{
					if (sourceType.GetGenericTypeDefinition() != targetType.GetGenericTypeDefinition())
					{
						return false;
					}
					
					// Compare generic arguments
					
					Type[] sourceGenArgs = sourceType.GetGenericArguments();
					Type[] targetGenArgs = targetType.GetGenericArguments();
					
					for(int i=0; i < sourceGenArgs.Length; i++)
					{
						if (!IsTypeEquivalent(sourceGenArgs[i], targetGenArgs[i]))
						{
							return false;
						}
					}
				}
				else if (sourceType.IsArray)
				{
					Type sourceArrayType = sourceType.GetElementType();
					Type targetArrayType = targetType.GetElementType();
					
					if (!IsTypeEquivalent(sourceArrayType, targetArrayType))
					{
						return false;
					}
					
					int sourceRank = sourceType.GetArrayRank();
					int targetRank = targetType.GetArrayRank();
					
					if (sourceRank != targetRank)
					{
						return false;
					}
				}
				else if (sourceType != targetType)
				{
					return false;
				}
			}

			return true;
		}

		protected override Reference GetProxyTargetReference()
		{
			return targetField;
		}

		protected override bool CanOnlyProxyVirtual()
		{
			return false;
		}

		private static bool IsEquivalentMethod(MethodInfo methodInfo, MethodInfo methodOnInterface)
		{
			// Check return type equivalence

			if (!IsTypeEquivalent(methodInfo.ReturnType, methodOnInterface.ReturnType))
			{
				return false;
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

				if (!IsTypeEquivalent(sourceParamType, targetParamType))
				{
					return false;
				}
			}

			return true;
		}
	}
}