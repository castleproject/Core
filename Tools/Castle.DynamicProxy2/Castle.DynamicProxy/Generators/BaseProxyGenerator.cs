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
	using Castle.Core.Interceptor;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	public enum ConstructorVersion
	{
		WithTargetMethod,
		WithoutTargetMethod
	}

	/// <summary>
	/// Base class that exposes the common functionalities
	/// to proxy generation.
	/// </summary>
	/// <remarks>
	/// TODO: 
	/// - Add events so people can hook into the proxy generation and change the generated code
	/// - Add serialization support
	/// - Add Xml serialization support
	/// - Allow one to specify the base class for interface proxies
	/// - Use the interceptor selector if provided
	/// - Expose parameters of non-parameterless constructors on the generated constructor
	/// - Add tests and fixes for 'leaking this' problem
	/// - Mixin support
	/// </remarks>
	[CLSCompliant(false)]
	public abstract class BaseProxyGenerator
	{
		private readonly ModuleScope scope;
		private int nestedCounter, callbackCounter;
		private int fieldCount = 1;
		private FieldReference typeTokenField;
		private Dictionary<MethodInfo, FieldReference> method2TokenField = new Dictionary<MethodInfo, FieldReference>();

		protected readonly Type targetType;
		protected IProxyGenerationHook generationHook;
		// protected MethodEmitter initCacheMethod;

		protected BaseProxyGenerator(ModuleScope scope, Type targetType)
		{
			this.scope = scope;
			this.targetType = targetType;
		}

		protected ModuleScope Scope
		{
			get { return scope; }
		}

		protected virtual ClassEmitter BuildClassEmitter(String typeName, Type parentType, IList interfaceList)
		{
			Type[] interfaces = new Type[interfaceList.Count];
			
			interfaceList.CopyTo(interfaces, 0);

			return BuildClassEmitter(typeName, parentType, interfaces);
		}
		
		protected virtual ClassEmitter BuildClassEmitter(String typeName, Type parentType, Type[] interfaces)
		{
			if (interfaces == null)
			{
				interfaces = new Type[0];
			}

			return new ClassEmitter(Scope, typeName, parentType, interfaces, true);
		}

		/// <summary>
		/// Used by dinamically implement <see cref="Core.Interceptor.IProxyTargetAccessor"/>
		/// </summary>
		/// <returns></returns>
		protected abstract Reference GetProxyTargetReference();

		protected abstract bool CanOnlyProxyVirtual();

		#region Cache related

		protected Type GetFromCache(CacheKey key)
		{
			return scope.GetFromCache(key);
		}

		protected void AddToCache(CacheKey key, Type type)
		{
			scope.RegisterInCache(key, type);
		}

		#endregion

		protected MethodEmitter CreateProxiedMethod(Type targetType, MethodInfo method, ClassEmitter emitter,
		                                            NestedClassEmitter invocationImpl,
		                                            FieldReference interceptorsField,
													Reference targetRef, 
		                                            ConstructorVersion version, MethodInfo methodOnTarget)
		{
			MethodAttributes atts = ObtainMethodAttributes(method);
			MethodEmitter methodEmitter = emitter.CreateMethod(method.Name, atts);

			return ImplementProxiedMethod(targetType, methodEmitter, method, 
			                              emitter, invocationImpl, interceptorsField, targetRef, version, methodOnTarget);
		}

		protected void ImplementBlankInterface(Type targetType, Type _interface, 
		                                       ClassEmitter emitter, FieldReference interceptorsField, 
		                                       ConstructorEmitter typeInitializerConstructor)
		{
			PropertyToGenerate[] propsToGenerate;
			MethodInfo[] methods = CollectMethodsAndProperties(emitter, _interface, false, out propsToGenerate);

			Dictionary<MethodInfo, NestedClassEmitter> method2Invocation = new Dictionary<MethodInfo, NestedClassEmitter>();

			foreach(MethodInfo method in methods)
			{
				AddFieldToCacheMethodTokenAndStatementsToInitialize(method, typeInitializerConstructor, emitter);
				
				method2Invocation[method] = BuildInvocationNestedType(emitter, targetType,
																	  emitter.TypeBuilder,
																	  method, null,
																	  ConstructorVersion.WithoutTargetMethod);
			}
			
			foreach(MethodInfo method in methods)
			{
				if (method.IsSpecialName &&
					(method.Name.StartsWith("get_") || method.Name.StartsWith("set_")))
				{
					continue;
				}

				NestedClassEmitter nestedClass = method2Invocation[method];

				MethodEmitter newProxiedMethod = CreateProxiedMethod(
					targetType, method, emitter, nestedClass, interceptorsField, SelfReference.Self, 
					ConstructorVersion.WithoutTargetMethod, null);

				ReplicateNonInheritableAttributes(method, newProxiedMethod);
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
					NestedClassEmitter nestedClass = method2Invocation[propToGen.GetMethod];

					MethodAttributes atts = ObtainMethodAttributes(propToGen.SetMethod);

					MethodEmitter setEmitter = propToGen.Emitter.CreateSetMethod(atts);

					ImplementProxiedMethod(targetType, setEmitter,
										   propToGen.SetMethod, emitter,
										   nestedClass, interceptorsField, SelfReference.Self,
										   ConstructorVersion.WithoutTargetMethod, null);

					ReplicateNonInheritableAttributes(propToGen.SetMethod, setEmitter);
				}
			}
		}

		protected MethodEmitter ImplementProxiedMethod(Type targetType, 
		                                               MethodEmitter methodEmitter, MethodInfo method,
		                                               ClassEmitter emitter,
		                                               NestedClassEmitter invocationImpl,
		                                               FieldReference interceptorsField,
		                                               Reference targetRef, ConstructorVersion version, 
		                                               MethodInfo methodOnTarget)
		{
			methodEmitter.CopyParametersAndReturnTypeFrom(method, emitter);

			TypeReference[] dereferencedArguments = IndirectReference.WrapIfByRef(methodEmitter.Arguments);

			Type iinvocation = invocationImpl.TypeBuilder;

			Type[] set1 = null;
			Type[] set2 = null;

			if (iinvocation.IsGenericType)
			{
				// get type generics
				set1 = targetType.GetGenericArguments();
			}

			if (method.IsGenericMethod)
			{
				// get method generics
				set2 = method.GetGenericArguments();
			}

			bool isGenericInvocationClass = false;

			if (set1 != null || set2 != null)
			{
				iinvocation = iinvocation.MakeGenericType(TypeUtil.Union(set1, set2));

				isGenericInvocationClass = true;
			}

			LocalReference invocationImplLocal = methodEmitter.CodeBuilder.DeclareLocal(iinvocation);

			// TODO: Initialize iinvocation instance 
			// with ordinary arguments and in and out arguments

			Expression interceptors;

			// if (useSelector)
			{
				// TODO: Generate code that checks the return of selector
				// if no interceptors is returned, should we invoke the base.Method directly?
			}
			// else
			{
				interceptors = interceptorsField.ToExpression();
			}

			Expression typeTokenFieldExp = typeTokenField.ToExpression();
			Expression methodInfoTokenExp;

			if (method2TokenField.ContainsKey(method)) // Token is in the cache
			{
				methodInfoTokenExp = method2TokenField[method].ToExpression();
			}
			else
			{
				// Not in the cache: generic method
				
				methodInfoTokenExp = new MethodTokenExpression(method);
			}

			ConstructorInfo constructor = invocationImpl.Constructors[0].Builder;

			if (isGenericInvocationClass)
			{
				constructor = TypeBuilder.GetConstructor(iinvocation, invocationImpl.Constructors[0].Builder);
			}

			NewInstanceExpression newInvocImpl;

			if (version == ConstructorVersion.WithTargetMethod)
			{
				Expression methodOnTargetTokenExp;

				if (method2TokenField.ContainsKey(methodOnTarget)) // Token is in the cache
				{
					methodOnTargetTokenExp = method2TokenField[methodOnTarget].ToExpression();
				}
				else
				{
					// Not in the cache: generic method

					methodOnTargetTokenExp = new MethodTokenExpression(methodOnTarget);
				}
				
				newInvocImpl = new NewInstanceExpression(
					constructor,
					targetRef.ToExpression(),
					interceptors,
					typeTokenFieldExp,
					methodOnTargetTokenExp,
					methodInfoTokenExp,
					new ReferencesToObjectArrayExpression(dereferencedArguments), 
					SelfReference.Self.ToExpression());
			}
			else
			{
				newInvocImpl = new NewInstanceExpression(
					constructor,
					targetRef.ToExpression(),
					interceptors,
					typeTokenFieldExp,
					methodInfoTokenExp,
					new ReferencesToObjectArrayExpression(dereferencedArguments), 
					SelfReference.Self.ToExpression());
			}

			methodEmitter.CodeBuilder.AddStatement(
				new AssignStatement(invocationImplLocal, newInvocImpl));

			methodEmitter.CodeBuilder.AddStatement(
				new ExpressionStatement(
					new MethodInvocationExpression(invocationImplLocal, Constants.AbstractInvocationProceed)));

			if (method.ReturnType != typeof(void))
			{
				// Emit code to return with cast from ReturnValue
				MethodInvocationExpression getRetVal =
					new MethodInvocationExpression(invocationImplLocal,
					                               typeof(AbstractInvocation).GetMethod("get_ReturnValue"));

				methodEmitter.CodeBuilder.AddStatement(new ReturnStatement(
				                                       	new ConvertExpression(method.ReturnType, getRetVal)));
			}
			else
			{
				methodEmitter.CodeBuilder.AddStatement(new ReturnStatement());
			}

			return methodEmitter;
		}

		protected void GenerateConstructor(ClassEmitter emitter,
										   params FieldReference[] fields)
		{
			GenerateConstructor(emitter, null, fields);
		}

		protected void GenerateConstructor(ClassEmitter emitter, 
		                                   ConstructorInfo baseConstructor,
		                                   params FieldReference[] fields)
		{
			ArgumentReference[] args;
			ParameterInfo[] baseConstructorParams = null;

			if (baseConstructor != null)
			{
				baseConstructorParams = baseConstructor.GetParameters();
			}

			if (baseConstructorParams != null && baseConstructorParams.Length != 0)
			{
				args = new ArgumentReference[fields.Length + baseConstructorParams.Length];

				int offset = fields.Length;

				for(int i = offset; i < offset + baseConstructorParams.Length; i++)
				{
					ParameterInfo paramInfo = baseConstructorParams[i - offset];
					args[i] = new ArgumentReference(paramInfo.ParameterType);
				}
			}
			else
			{
				args = new ArgumentReference[fields.Length];
			}

			for(int i = 0; i < fields.Length; i++)
			{
				args[i] = new ArgumentReference(fields[i].Reference.FieldType);
			}

			ConstructorEmitter constructor = emitter.CreateConstructor(args);

			for(int i = 0; i < fields.Length; i++)
			{
				constructor.CodeBuilder.AddStatement(new AssignStatement(fields[i], args[i].ToExpression()));
			}
			
			// Invoke base constructor

			if (baseConstructor != null)
			{
				ArgumentReference[] slice = new ArgumentReference[baseConstructorParams.Length];
				Array.Copy(args, fields.Length, slice, 0, baseConstructorParams.Length);

				constructor.CodeBuilder.InvokeBaseConstructor(baseConstructor, slice);
			}
			else
			{
				constructor.CodeBuilder.InvokeBaseConstructor();
			}

			// Invoke initialize method

			// constructor.CodeBuilder.AddStatement(
			// 	new ExpressionStatement(new MethodInvocationExpression(SelfReference.Self, initCacheMethod)));

			constructor.CodeBuilder.AddStatement(new ReturnStatement());
		}

		/// <summary>
		/// Generates a parameters constructor that initializes the proxy
		/// state with <see cref="StandardInterceptor"/> just to make it non-null.
		/// <para>
		/// This constructor is important to allow proxies to be XML serializable
		/// </para>
		/// </summary>
		protected void GenerateParameterlessConstructor(ClassEmitter emitter, Type baseClass, FieldReference interceptorField)
		{
			// Check if the type actually has a default constructor

			ConstructorInfo defaultConstructor = baseClass.GetConstructor(BindingFlags.Public, null, Type.EmptyTypes, null);
			
			if (defaultConstructor == null)
			{
				defaultConstructor = baseClass.GetConstructor(BindingFlags.NonPublic, null, Type.EmptyTypes, null);

				if (defaultConstructor == null || defaultConstructor.IsPrivate)
				{
					return;
				}
			}
			
			ConstructorEmitter constructor = emitter.CreateConstructor();
			
			// initialize fields with an empty interceptor

			constructor.CodeBuilder.AddStatement(
				new AssignStatement(interceptorField, new NewArrayExpression(1, typeof(IInterceptor))));
			constructor.CodeBuilder.AddStatement(new AssignArrayStatement(interceptorField, 0, new NewInstanceExpression(typeof(StandardInterceptor), new Type[0])));

			// Invoke base constructor

			constructor.CodeBuilder.InvokeBaseConstructor(defaultConstructor);

			constructor.CodeBuilder.AddStatement(new ReturnStatement());
		}

		#region First level attributes

		protected MethodAttributes ObtainMethodAttributes(MethodInfo method)
		{
			MethodAttributes atts = MethodAttributes.Virtual;

			if (method.IsPublic)
			{
				atts |= MethodAttributes.Public;
			}

			if (method.IsHideBySig)
			{
				atts |= MethodAttributes.HideBySig;
			}

			if (method.IsFamilyAndAssembly)
			{
				atts |= MethodAttributes.FamANDAssem;
			}
			else if (method.IsFamilyOrAssembly)
			{
				atts |= MethodAttributes.FamORAssem;
			}
			else if (method.IsFamily)
			{
				atts |= MethodAttributes.Family;
			}

			if (method.Name.StartsWith("set_") || method.Name.StartsWith("get_"))
			{
				atts |= MethodAttributes.SpecialName;
			}

			return atts;
		}

		private PropertyAttributes ObtainPropertyAttributes(PropertyInfo property)
		{
			PropertyAttributes atts = PropertyAttributes.None;

			return atts;
		}

		#endregion

		protected MethodBuilder CreateCallbackMethod(ClassEmitter emitter,
		                                             MethodInfo methodInfo,
		                                             MethodInfo methodOnTarget)
		{
			MethodInfo targetMethod = methodOnTarget != null ? methodOnTarget : methodInfo;

			// MethodBuild creation

			MethodAttributes atts = MethodAttributes.Family;

			String name = methodInfo.Name + "_callback_" + ++callbackCounter;

			MethodEmitter callBackMethod = emitter.CreateMethod(name, atts);

			callBackMethod.CopyParametersAndReturnTypeFrom(targetMethod, emitter);

			// Generic definition

			if (targetMethod.IsGenericMethod)
			{
				targetMethod = targetMethod.MakeGenericMethod(callBackMethod.GenericTypeParams);
			}

			// Parameters exp

			Expression[] exps = new Expression[callBackMethod.Arguments.Length];

			for(int i = 0; i < callBackMethod.Arguments.Length; i++)
			{
				exps[i] = callBackMethod.Arguments[i].ToExpression();
			}

			// invocation on base class

			callBackMethod.CodeBuilder.AddStatement(
				new ReturnStatement(
					new MethodInvocationExpression(GetProxyTargetReference(), targetMethod, exps)));

			return callBackMethod.MethodBuilder;
		}

		#region IInvocation related 

		/// <summary>
		/// If callbackMethod is null the InvokeOnTarget implementation 
		/// is just the code to throw an exception
		/// </summary>
		/// <param name="emitter"></param>
		/// <param name="targetType"></param>
		/// <param name="targetForInvocation"></param>
		/// <param name="methodInfo"></param>
		/// <param name="callbackMethod"></param>
		/// <param name="version"></param>
		/// <returns></returns>
		protected NestedClassEmitter BuildInvocationNestedType(
			ClassEmitter emitter, Type targetType, Type targetForInvocation,
			MethodInfo methodInfo, MethodInfo callbackMethod, ConstructorVersion version)
		{
			nestedCounter++;

			NestedClassEmitter nested = new NestedClassEmitter(
				emitter, "Invocation" + methodInfo.Name + "_" + nestedCounter.ToString(), 
					typeof(AbstractInvocation), new Type[0]);

			Type[] genTypes = TypeUtil.Union(targetType.GetGenericArguments(), methodInfo.GetGenericArguments());

			nested.CreateGenericParameters(genTypes);

			// Create the invocation fields

			FieldReference targetRef = nested.CreateField("target", targetForInvocation);

			// Create constructor

			CreateIInvocationConstructor(targetForInvocation, nested, targetRef, version);

			// InvokeMethodOnTarget implementation
			
			if (callbackMethod != null)
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();

				CreateIInvocationInvokeOnTarget(emitter, nested, parameters, targetRef, callbackMethod);
			}
			else
			{
				CreateEmptyIInvocationInvokeOnTarget(nested);
			}

			return nested;
		}

		protected void CreateIInvocationInvokeOnTarget(ClassEmitter targetTypeEmitter, 
		                                               NestedClassEmitter nested,
		                                               ParameterInfo[] parameters,
		                                               FieldReference targetField,
													   MethodInfo callbackMethod)
		{
			const MethodAttributes methodAtts = MethodAttributes.Public |
			                                    MethodAttributes.Final |
			                                    MethodAttributes.Virtual;

			MethodEmitter method =
				nested.CreateMethod("InvokeMethodOnTarget",
				                    new ReturnReferenceExpression(typeof(void)), methodAtts);

			Expression[] args = new Expression[parameters.Length];

			// Idea: instead of grab parameters one by one
			// we should grab an array

			for(int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo param = parameters[i];

				if (!param.IsOut && !param.IsRetval)
				{
					Type paramType = param.ParameterType;

					if (HasGenericParameters(paramType))
					{
						paramType = paramType.GetGenericTypeDefinition().MakeGenericType(
							nested.GetGenericArgumentsFor(paramType));
					}
					else if (paramType.IsGenericParameter)
					{
						paramType = nested.GetGenericArgument(paramType.Name);
					}

					args[i] = 
						new ConvertExpression(
							paramType, new MethodInvocationExpression(
							           	SelfReference.Self, 
							           	typeof(AbstractInvocation).GetMethod("GetArgumentValue"), 
							           	new LiteralIntExpression(i)));
				}
				else
				{
					throw new NotImplementedException("Out/Ref parameters are not supported yet");
				}
			}

			MethodInvocationExpression baseMethodInvExp;
			
			if (callbackMethod.IsGenericMethod)
			{
				callbackMethod = callbackMethod.MakeGenericMethod(nested.GetGenericArgumentsFor(callbackMethod));
			}

			baseMethodInvExp = new MethodInvocationExpression(targetField, callbackMethod, args);

			LocalReference ret_local = null;

			if (callbackMethod.ReturnType != typeof(void))
			{
				if (callbackMethod.ReturnType.IsGenericParameter)
				{
					ret_local = method.CodeBuilder.DeclareLocal(
						nested.GetGenericArgument(callbackMethod.ReturnType.Name));
				}
				else if (HasGenericParameters(callbackMethod.ReturnType))
				{
					ret_local = method.CodeBuilder.DeclareLocal(
						callbackMethod.ReturnType.GetGenericTypeDefinition().MakeGenericType(nested.GetGenericArgumentsFor(callbackMethod.ReturnType)));
				}
				else
				{
					ret_local = method.CodeBuilder.DeclareLocal(callbackMethod.ReturnType);
				}

				method.CodeBuilder.AddStatement(new AssignStatement(ret_local, baseMethodInvExp));
			}
			else
			{
				method.CodeBuilder.AddStatement(new ExpressionStatement(baseMethodInvExp));
			}

			if (callbackMethod.ReturnType != typeof(void))
			{
				MethodInvocationExpression setRetVal =
					new MethodInvocationExpression(
						SelfReference.Self, 
						typeof(AbstractInvocation).GetMethod("set_ReturnValue"), 
						new ConvertExpression(
							typeof(object), 
							ret_local.Type, 
							ret_local.ToExpression()));

				method.CodeBuilder.AddStatement(new ExpressionStatement(setRetVal));
			}

			method.CodeBuilder.AddStatement(new ReturnStatement());
		}

		protected void CreateEmptyIInvocationInvokeOnTarget(NestedClassEmitter nested)
		{
			const MethodAttributes methodAtts = MethodAttributes.Public |
												MethodAttributes.Final |
												MethodAttributes.Virtual;

			MethodEmitter method =
				nested.CreateMethod("InvokeMethodOnTarget",
									new ReturnReferenceExpression(typeof(void)), methodAtts);

			// TODO: throw exception
			
			String message = String.Format("This is a DynamicProxy2 error: the interceptor attempted " + 
				"to 'Proceed' for a method without a target, for example, an interface method");

			method.CodeBuilder.AddStatement(new ThrowStatement(typeof(NotImplementedException), message));

			method.CodeBuilder.AddStatement(new ReturnStatement());
		}

		/// <summary>
		/// Generates the constructor for the nested class that extends
		/// <see cref="AbstractInvocation"/>
		/// </summary>
		/// <param name="targetFieldType"></param>
		/// <param name="nested"></param>
		/// <param name="targetField"></param>
		/// <param name="version"></param>
		protected void CreateIInvocationConstructor(Type targetFieldType, 
		                                            NestedClassEmitter nested,
													FieldReference targetField, 
		                                            ConstructorVersion version)
		{
			ArgumentReference cArg0 = new ArgumentReference(targetFieldType);
			ArgumentReference cArg1 = new ArgumentReference(typeof(IInterceptor[]));
			ArgumentReference cArg2 = new ArgumentReference(typeof(Type));
			ArgumentReference cArg3 = new ArgumentReference(typeof(MethodInfo));
			ArgumentReference cArg4 = null;
			ArgumentReference cArg6 = new ArgumentReference(typeof(object));
			
			if (version == ConstructorVersion.WithTargetMethod)
			{
				cArg4 = new ArgumentReference(typeof(MethodInfo));
			}
			
			ArgumentReference cArg5 = new ArgumentReference(typeof(object[]));

			ConstructorEmitter constructor;
			
			if (cArg4 == null)
			{
				constructor = nested.CreateConstructor(cArg0, cArg1, cArg2, cArg3, cArg5, cArg6);
			}
			else
			{
				constructor = nested.CreateConstructor(cArg0, cArg1, cArg2, cArg3, cArg4, cArg5, cArg6);
			}

			constructor.CodeBuilder.AddStatement(new AssignStatement(targetField, cArg0.ToExpression()));

			if (cArg4 == null)
			{
				constructor.CodeBuilder.InvokeBaseConstructor(Constants.AbstractInvocationConstructorWithoutTargetMethod,
															  cArg0, cArg6, cArg1, cArg2, cArg3, cArg5);
			}
			else
			{
				constructor.CodeBuilder.InvokeBaseConstructor(Constants.AbstractInvocationConstructorWithTargetMethod,
															  cArg0, cArg6, cArg1, cArg2, cArg3, cArg4, cArg5);
			}
			
			constructor.CodeBuilder.AddStatement(new ReturnStatement());
		}

		#endregion

		#region Custom Attribute handling

		protected void ReplicateNonInheritableAttributes(Type targetType, ClassEmitter emitter)
		{
			object[] attrs = targetType.GetCustomAttributes(false);

			foreach(Attribute attribute in attrs)
			{
				if (IsInheritable(attribute)) continue;
				
				emitter.DefineCustomAttribute(attribute);
			}
		}

		protected void ReplicateNonInheritableAttributes(MethodInfo method, MethodEmitter emitter)
		{
			object[] attrs = method.GetCustomAttributes(false);

			foreach(Attribute attribute in attrs)
			{
				if (IsInheritable(attribute)) continue;
				
				emitter.DefineCustomAttribute(attribute);
			}
		}

		#endregion

		#region Type tokens related operations

		protected void GenerateConstructors(ClassEmitter emitter, Type baseType, params FieldReference[] fields)
		{
			ConstructorInfo[] constructors = baseType.GetConstructors(
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			foreach(ConstructorInfo constructor in constructors)
			{
				if (constructor.IsPrivate) continue;
				
				GenerateConstructor(emitter, constructor, fields);
			}
		}

		protected ConstructorEmitter GenerateStaticConstructor(ClassEmitter emitter)
		{
			return emitter.CreateTypeConstructor();
		}
		
		/// <summary>
		/// Improvement: this cache should be static. We should generate a
		/// type constructor instead
		/// </summary>
		protected void CreateInitializeCacheMethodBody(Type targetType, MethodInfo[] methods,
													   ClassEmitter classEmitter, 
		                                               ConstructorEmitter typeInitializerConstructor)
		{
			typeTokenField = classEmitter.CreateStaticField("typeTokenCache", typeof(Type));

			typeInitializerConstructor.CodeBuilder.AddStatement(
				new AssignStatement(typeTokenField, new TypeTokenExpression(targetType)));

			CacheMethodTokens(classEmitter, methods, typeInitializerConstructor);
		}

		protected void CacheMethodTokens(ClassEmitter classEmitter, MethodInfo[] methods, ConstructorEmitter typeInitializerConstructor)
		{
			foreach(MethodInfo method in methods)
			{
				// Aparently we cannot cache generic methods
				if (method.IsGenericMethod) continue;
				
				AddFieldToCacheMethodTokenAndStatementsToInitialize(method, typeInitializerConstructor, classEmitter);
			}
		}

		protected void AddFieldToCacheMethodTokenAndStatementsToInitialize(MethodInfo method, 
																		   ConstructorEmitter typeInitializerConstructor, 
		                                                                   ClassEmitter classEmitter)
		{
			FieldReference fieldCache = classEmitter.CreateStaticField("tokenCache" + fieldCount++, typeof(MethodInfo));

			if (method2TokenField.ContainsKey(method))
			{
				throw new ArgumentException(String.Format("Duplicate method {0} on type {1}", 
				                                          method.ToString(), method.DeclaringType.FullName));
			}
			
			method2TokenField.Add(method, fieldCache);

			typeInitializerConstructor.CodeBuilder.AddStatement(
				new AssignStatement(fieldCache, new MethodTokenExpression(method)));
		}

		protected void CompleteInitCacheMethod(ConstructorCodeBuilder constCodeBuilder)
		{
			constCodeBuilder.AddStatement(new ReturnStatement());
		}

		protected void AddDefaultInterfaces(IList interfaceList)
		{
			if (!interfaceList.Contains(typeof(IProxyTargetAccessor)))
			{
				interfaceList.Add(typeof(IProxyTargetAccessor));
			}
		}

		protected void ImplementProxyTargetAccessor(Type targetType, ClassEmitter emitter)
		{
			MethodAttributes attributes = MethodAttributes.Virtual | MethodAttributes.Public;

			MethodEmitter methodEmitter = emitter.CreateMethod("DynProxyGetTarget", attributes,
															   new ReturnReferenceExpression(typeof(object)));

			methodEmitter.CodeBuilder.AddStatement(
				new ReturnStatement(
					new ConvertExpression(
						typeof(object), targetType, GetProxyTargetReference().ToExpression())));
		}

		#endregion

		#region Utility methods

		protected void CollectMethodsToProxy(ArrayList methodList, Type type, bool onlyVirtuals)
		{
			CollectMethods(methodList, type, onlyVirtuals);

			if (type.IsInterface)
			{
				Type[] typeChain = type.FindInterfaces(new TypeFilter(NoFilter), null);

				foreach (Type interType in typeChain)
				{
					CollectMethods(methodList, interType, onlyVirtuals);
				}
			}
		}

		protected void CollectPropertyMethodsToProxy(ArrayList methodList, Type type, bool onlyVirtuals,
													 ClassEmitter emitter, out PropertyToGenerate[] propsToGenerate)
		{
			if (type.IsInterface)
			{
				ArrayList toGenerateList = new ArrayList();

				toGenerateList.AddRange(CollectProperties(methodList, type, onlyVirtuals, emitter));

				Type[] typeChain = type.FindInterfaces(new TypeFilter(NoFilter), null);

				foreach (Type interType in typeChain)
				{
					toGenerateList.AddRange(CollectProperties(methodList, interType, onlyVirtuals, emitter));
				}

				propsToGenerate = (PropertyToGenerate[])toGenerateList.ToArray(typeof(PropertyToGenerate));
			}
			else
			{
				propsToGenerate = CollectProperties(methodList, type, onlyVirtuals, emitter);
			}
		}

		/// <summary>
		/// Performs some basic screening and invokes the <see cref="IProxyGenerationHook"/>
		/// to select methods.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="onlyVirtuals"></param>
		/// <returns></returns>
		protected bool AcceptMethod(MethodInfo method, bool onlyVirtuals)
		{
			// we can never intercept a sealed (final) method
			if (method.IsFinal)
				return false;

			if (IsInternal(method))
				return false;

			if (onlyVirtuals && !method.IsVirtual)
			{
				if (method.DeclaringType != typeof(object) && method.DeclaringType != typeof(MarshalByRefObject))
				{
					generationHook.NonVirtualMemberNotification(targetType, method);
				}

				return false;
			}

			// TODO: Only protected and public should accepted

			if (method.DeclaringType == typeof(object))
			{
				return false;
			}
			if (method.DeclaringType == typeof(MarshalByRefObject))
			{
				return false;
			}

			return generationHook.ShouldInterceptMethod(targetType, method); ;
		}

		private static bool IsInternal(MethodInfo method)
		{
			return (method.Attributes & MethodAttributes.FamANDAssem) != 0 //internal
				   && (method.Attributes & MethodAttributes.Family) == 0;//b
		}

		protected MethodInfo[] CollectMethodsAndProperties(ClassEmitter emitter, Type targetType,
														   out PropertyToGenerate[] propsToGenerate)
		{
			bool onlyVirtuals = CanOnlyProxyVirtual();

			return CollectMethodsAndProperties(emitter, targetType, onlyVirtuals, out propsToGenerate);
		}

		protected MethodInfo[] CollectMethodsAndProperties(ClassEmitter emitter, Type targetType, bool onlyVirtuals,
														   out PropertyToGenerate[] propsToGenerate)
		{
			ArrayList methodsList = new ArrayList();

			CollectMethodsToProxy(methodsList, targetType, onlyVirtuals);
			CollectPropertyMethodsToProxy(methodsList, targetType, onlyVirtuals, emitter, out propsToGenerate);

			return (MethodInfo[])methodsList.ToArray(typeof(MethodInfo));
		}
		
		/// <summary>
		/// Checks if the method is public or protected.
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		private bool IsAccessible(MethodInfo method)
		{
			return method.IsPublic || method.IsFamily || method.IsFamilyAndAssembly || method.IsFamilyOrAssembly;
		}

		private bool HasGenericParameters(Type type)
		{
			if (type.IsGenericType)
			{
				Type[] genTypes = type.GetGenericArguments();

				foreach(Type genType in genTypes)
				{
					if (genType.IsGenericParameter)
					{
						return true;
					}
				}
			}

			return false;
		}

		private bool NoFilter(Type type, object filterCriteria)
		{
			return true;
		}

		private void CollectMethods(ArrayList methodsList, Type type, bool onlyVirtuals)
		{
			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

			MethodInfo[] methods = type.GetMethods(flags);

			foreach(MethodInfo method in methods)
			{
				if (method.IsSpecialName)
				{
					continue;
				}

				if (AcceptMethod(method, onlyVirtuals))
				{
					methodsList.Add(method);
				}
			}
		}

		private PropertyToGenerate[] CollectProperties(ArrayList methodList, Type type, bool onlyVirtuals, ClassEmitter emitter)
		{
			ArrayList toGenerateList = new ArrayList();

			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

			PropertyInfo[] properties = type.GetProperties(flags);

			foreach(PropertyInfo propInfo in properties)
			{
				bool generateReadable, generateWritable;

				generateWritable = generateReadable = false;

				MethodInfo setMethod, getMethod;
				setMethod = getMethod = null;

				if (propInfo.CanRead)
				{
					getMethod = propInfo.GetGetMethod(true);

					if (IsAccessible(getMethod) && AcceptMethod(getMethod, onlyVirtuals))
					{
						methodList.Add(getMethod);
						generateReadable = true;
					}
				}

				if (propInfo.CanWrite)
				{
					setMethod = propInfo.GetSetMethod(true);

					if (IsAccessible(setMethod) && AcceptMethod(setMethod, onlyVirtuals))
					{
						methodList.Add(setMethod);
						generateWritable = true;
					}
				}

				if (!generateWritable && !generateReadable)
				{
					continue;
				}

				PropertyAttributes atts = ObtainPropertyAttributes(propInfo);

				PropertyEmitter propEmitter =
					emitter.CreateProperty(propInfo.Name, atts, propInfo.PropertyType);

				PropertyToGenerate propToGenerate =
					new PropertyToGenerate(generateReadable, generateWritable, propEmitter, getMethod, setMethod);

				toGenerateList.Add(propToGenerate);
			}

			return (PropertyToGenerate[])toGenerateList.ToArray(typeof(PropertyToGenerate));
		}

		private bool IsInheritable(Attribute attribute)
		{
			object[] attrs = attribute.GetType().GetCustomAttributes(typeof(AttributeUsageAttribute), true);

			if (attrs.Length != 0)
			{
				AttributeUsageAttribute usage = (AttributeUsageAttribute)attrs[0];

				return usage.Inherited;
			}

			return true;
		}

		#endregion
	}
}