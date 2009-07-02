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
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Runtime.Serialization;
	using Castle.Core.Interceptor;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
#if SILVERLIGHT
	using Castle.DynamicProxy.SilverlightExtensions;
#else
	using Castle.DynamicProxy.Serialization;
#endif

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
	/// - Add tests and fixes for 'leaking this' problem
	/// </remarks>
	public abstract class BaseProxyGenerator
	{
		private static MethodInfo invocation_getArgumentsMethod = typeof(AbstractInvocation).GetMethod("get_Arguments");

		private static readonly MethodInfo proxyGenerationOptions_Selector = typeof(ProxyGenerationOptions).GetProperty("Selector").GetGetMethod();

		private readonly ModuleScope scope;
		private int nestedCounter, callbackCounter;
		private int fieldCount = 1;
		private FieldReference typeTokenField;
		private Dictionary<MethodInfo, FieldReference> method2TokenField = new Dictionary<MethodInfo, FieldReference>();
		private IList generateNewSlot = new List<MethodInfo>();
		private ProxyGenerationOptions proxyGenerationOptions;
		private FieldReference proxyGenerationOptionsField;

		protected readonly Type targetType;
		protected ConstructorInfo serializationConstructor;
		protected IList methodsToSkip = new List<MethodInfo>();
		protected Dictionary<MethodInfo, Type> method2MixinType = new Dictionary<MethodInfo, Type>();
		protected Dictionary<Type, FieldReference> interface2MixinFieldReference = new Dictionary<Type, FieldReference>();

		protected BaseProxyGenerator(ModuleScope scope, Type targetType)
		{
			this.scope = scope;
			this.targetType = targetType;
		}

		public ProxyGenerationOptions ProxyGenerationOptions
		{
			get
			{
				if (proxyGenerationOptions == null)
				{
					throw new InvalidOperationException("ProxyGenerationOptions must be set before being retrieved.");
				}
				return proxyGenerationOptions;
			}
		}

		protected void SetGenerationOptions(ProxyGenerationOptions options)
		{
			if (proxyGenerationOptions != null)
			{
				throw new InvalidOperationException("ProxyGenerationOptions can only be set once.");
			}
			proxyGenerationOptions = options;
		}

		protected void CreateOptionsField(ClassEmitter emitter)
		{
			proxyGenerationOptionsField = emitter.CreateStaticField("proxyGenerationOptions", typeof(ProxyGenerationOptions));
		}

		protected void InitializeStaticFields(Type builtType)
		{
			builtType.GetField(proxyGenerationOptionsField.Reference.Name).SetValue(null, ProxyGenerationOptions);
		}

		protected void CheckNotGenericTypeDefinition(Type type, string argumentName)
		{
			if (type != null && type.IsGenericTypeDefinition)
			{
				throw new ArgumentException("Type cannot be a generic type definition. Type: " + type.FullName, argumentName);
			}
		}

		protected void CheckNotGenericTypeDefinitions(IEnumerable types, string argumentName)
		{
			if (types != null)
			{
				foreach (Type t in types)
				{
					CheckNotGenericTypeDefinition(t, argumentName);
				}
			}
		}

		protected ModuleScope Scope
		{
			get { return scope; }
		}

		protected virtual ClassEmitter BuildClassEmitter(String typeName, Type parentType, IList interfaceList)
		{
			CheckNotGenericTypeDefinition(parentType, "parentType");
			CheckNotGenericTypeDefinitions(interfaceList, "interfaceList");

			Type[] interfaces = new Type[interfaceList.Count];

			interfaceList.CopyTo(interfaces, 0);

			return BuildClassEmitter(typeName, parentType, interfaces);
		}

		protected virtual ClassEmitter BuildClassEmitter(String typeName, Type parentType, Type[] interfaces)
		{
			CheckNotGenericTypeDefinition(parentType, "parentType");
			CheckNotGenericTypeDefinitions(interfaces, "interfaceList");

			if (interfaces == null)
			{
				interfaces = new Type[0];
			}

			return new ClassEmitter(Scope, typeName, parentType, interfaces);
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

		protected MethodEmitter CreateProxiedMethod(
			Type targetType,
			MethodInfo method,
			ClassEmitter emitter,
			NestedClassEmitter invocationImpl,
			FieldReference interceptorsField,
			Reference targetRef,
			ConstructorVersion version,
			MethodInfo methodOnTarget)
		{
			CheckNotGenericTypeDefinition(targetType, "targetType");

			MethodAttributes atts = ObtainMethodAttributes(method);
			MethodEmitter methodEmitter = emitter.CreateMethod(method.Name, atts);

			return
				ImplementProxiedMethod(targetType,
									   methodEmitter,
									   method,
									   emitter,
									   invocationImpl,
									   interceptorsField,
									   targetRef,
									   version,
									   methodOnTarget);
		}

		protected void ImplementBlankInterfaces(Type targetType, Type[] _interfaces, ClassEmitter emitter, FieldReference interceptorsField, ConstructorEmitter typeInitializerConstructor, bool allowChangeTarget, MethodInfo[] targetMethods, PropertyToGenerate[] targetProperties, EventToGenerate[] targetEvents)
		{
			CheckNotGenericTypeDefinition(targetType, "targetType");
			List<PropertyToGenerate> properties = new List<PropertyToGenerate>();
			List<EventToGenerate> events = new List<EventToGenerate>();
			List<MethodInfo> methods = new List<MethodInfo>();
			foreach(var type in _interfaces)
			{
				CheckNotGenericTypeDefinition(type, "_interfaces");

				PropertyToGenerate[] propsToGenerate;
				EventToGenerate[] eventsToGenerate;
				MethodInfo[] methodsToGenerate = CollectMethodsAndProperties(emitter, type, false, out propsToGenerate, out eventsToGenerate);
				AddIfNew(methods,targetMethods, methodsToGenerate);
				AddIfNew(properties,targetProperties, propsToGenerate);
				AddIfNew(events, targetEvents, eventsToGenerate);

			}


			Dictionary<MethodInfo, NestedClassEmitter> method2Invocation = new Dictionary<MethodInfo, NestedClassEmitter>();

			foreach (MethodInfo method in methods)
			{
				AddFieldToCacheMethodTokenAndStatementsToInitialize(method, typeInitializerConstructor, emitter);

				method2Invocation[method] =
					BuildInvocationNestedType(emitter,
											  targetType,
											  allowChangeTarget ? method.DeclaringType : emitter.TypeBuilder,
											  method,
											  allowChangeTarget ? method : null,
											  ConstructorVersion.WithoutTargetMethod,
											  allowChangeTarget);
			}

			foreach (MethodInfo method in methods)
			{
				if (method.IsSpecialName && (
					method.Name.StartsWith("get_") || 
					method.Name.StartsWith("set_") ||
					method.Name.StartsWith("add_") || 
					method.Name.StartsWith("remove_")))
				{
					continue;
				}

				NestedClassEmitter nestedClass = method2Invocation[method];

				MethodEmitter newProxiedMethod =
					CreateProxiedMethod(targetType,
										method,
										emitter,
										nestedClass,
										interceptorsField,
										SelfReference.Self,
										ConstructorVersion.WithoutTargetMethod,
										null);

				ReplicateNonInheritableAttributes(method, newProxiedMethod);
			}

			foreach (PropertyToGenerate propToGen in properties)
			{
				propToGen.BuildPropertyEmitter(emitter);
				if (propToGen.CanRead)
				{
					NestedClassEmitter nestedClass = method2Invocation[propToGen.GetMethod];

					MethodAttributes atts = ObtainMethodAttributes(propToGen.GetMethod);

					MethodEmitter getEmitter = propToGen.Emitter.CreateGetMethod(atts);

					ImplementProxiedMethod(targetType,
										   getEmitter,
										   propToGen.GetMethod,
										   emitter,
										   nestedClass,
										   interceptorsField,
										   SelfReference.Self,
										   ConstructorVersion.WithoutTargetMethod,
										   null);

					ReplicateNonInheritableAttributes(propToGen.GetMethod, getEmitter);
				}

				if (propToGen.CanWrite)
				{
					NestedClassEmitter nestedClass = method2Invocation[propToGen.SetMethod];

					MethodAttributes atts = ObtainMethodAttributes(propToGen.SetMethod);

					MethodEmitter setEmitter = propToGen.Emitter.CreateSetMethod(atts);

					ImplementProxiedMethod(targetType,
										   setEmitter,
										   propToGen.SetMethod,
										   emitter,
										   nestedClass,
										   interceptorsField,
										   SelfReference.Self,
										   ConstructorVersion.WithoutTargetMethod,
										   null);

					ReplicateNonInheritableAttributes(propToGen.SetMethod, setEmitter);
				}
			}

			foreach (EventToGenerate eventToGenerate in events)
			{

				eventToGenerate.BuildEventEmitter(emitter);
				NestedClassEmitter add_nestedClass = method2Invocation[eventToGenerate.AddMethod];

				MethodAttributes add_atts = ObtainMethodAttributes(eventToGenerate.AddMethod);

				MethodEmitter addEmitter = eventToGenerate.Emitter.CreateAddMethod(add_atts);

				ImplementProxiedMethod(targetType,
									   addEmitter,
									   eventToGenerate.AddMethod,
									   emitter,
									   add_nestedClass,
									   interceptorsField,
									   SelfReference.Self,
									   ConstructorVersion.WithoutTargetMethod,
									   null);

				ReplicateNonInheritableAttributes(eventToGenerate.AddMethod, addEmitter);

				NestedClassEmitter remove_nestedClass = method2Invocation[eventToGenerate.RemoveMethod];

				MethodAttributes remove_atts = ObtainMethodAttributes(eventToGenerate.RemoveMethod);

				MethodEmitter removeEmitter = eventToGenerate.Emitter.CreateRemoveMethod(remove_atts);

				ImplementProxiedMethod(targetType,
									   removeEmitter,
									   eventToGenerate.RemoveMethod,
									   emitter,
									   remove_nestedClass,
									   interceptorsField,
									   SelfReference.Self,
									   ConstructorVersion.WithoutTargetMethod,
									   null);

				ReplicateNonInheritableAttributes(eventToGenerate.RemoveMethod, removeEmitter);
			}
		}

		protected void AddIfNew<T>(ICollection<T> list, ICollection<T> existingItems, IEnumerable<T> newItems)
		{
			foreach(var item in newItems)
			{
				if (!list.Contains(item)&&!existingItems.Contains(item))
				{
					list.Add(item);
				}
			}
		}

		protected void AddIfNew<T>(IList<T> list,  IEnumerable<T> newItems)
		{
			foreach (var item in newItems)
			{
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
		}

		protected MethodEmitter ImplementProxiedMethod(
			Type targetType,
			MethodEmitter methodEmitter,
			MethodInfo method,
			ClassEmitter emitter,
			NestedClassEmitter invocationImpl,
			FieldReference interceptorsField,
			Reference targetRef,
			ConstructorVersion version,
			MethodInfo methodOnTarget)
		{
			CheckNotGenericTypeDefinition(targetType, "targetType");

			methodEmitter.CopyParametersAndReturnTypeFrom(method, emitter);

			TypeReference[] dereferencedArguments = IndirectReference.WrapIfByRef(methodEmitter.Arguments);

			Type iinvocation = invocationImpl.TypeBuilder;

			Trace.Assert(method.IsGenericMethod == iinvocation.IsGenericTypeDefinition);
			bool isGenericInvocationClass = false;
			Type[] genericMethodArgs = new Type[0];
			if (method.IsGenericMethod)
			{
				// bind generic method arguments to invocation's type arguments
				genericMethodArgs = methodEmitter.MethodBuilder.GetGenericArguments();
				iinvocation = iinvocation.MakeGenericType(genericMethodArgs);
				isGenericInvocationClass = true;
			}

			Expression typeTokenFieldExp = typeTokenField.ToExpression();
			Expression methodInfoTokenExp;

			string tokenFieldName;
			if (method2TokenField.ContainsKey(method)) // Token is in the cache
			{
				FieldReference methodTokenField = method2TokenField[method];
				tokenFieldName = methodTokenField.Reference.Name;
				methodInfoTokenExp = methodTokenField.ToExpression();
			}
			else
			{
				// Not in the cache: generic method
				MethodInfo genericMethod = method.MakeGenericMethod(genericMethodArgs);

				// Need random suffix added to the name, so that we don't end up with duplicate field names for
				// methods with the same name, but different generic parameters
				tokenFieldName = string.Format("{0}_{1}_{2}", genericMethod.Name, genericMethodArgs.Length,
				                               Guid.NewGuid().ToString("N"));
				methodInfoTokenExp = new MethodTokenExpression(genericMethod);
			}

			LocalReference invocationImplLocal = methodEmitter.CodeBuilder.DeclareLocal(iinvocation);

			// TODO: Initialize iinvocation instance with ordinary arguments and in and out arguments

			Expression interceptors = interceptorsField.ToExpression();

			// Create the field to store the selected interceptors for this method if an InterceptorSelector is specified
			FieldReference methodInterceptors = null;
			if (proxyGenerationOptions.Selector != null)
			{
				// If no interceptors are returned, should we invoke the base.Method directly? Looks like we should not.
				methodInterceptors = emitter.CreateField(string.Format("{0}_interceptors", tokenFieldName), typeof(IInterceptor[]));
			}

			ConstructorInfo constructor = invocationImpl.Constructors[0].ConstructorBuilder;
			if (isGenericInvocationClass)
			{
				constructor = TypeBuilder.GetConstructor(iinvocation, constructor);
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

					methodOnTargetTokenExp = new MethodTokenExpression(methodOnTarget.MakeGenericMethod(genericMethodArgs));
				}
				if (methodInterceptors == null)
				{
					newInvocImpl = //actual contructor call
						new NewInstanceExpression(constructor,
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
					MethodInvocationExpression methodInvocationExpression =
						new MethodInvocationExpression(proxyGenerationOptionsField, proxyGenerationOptions_Selector);
					methodInvocationExpression.VirtualCall = true;

					newInvocImpl = //actual contructor call
						new NewInstanceExpression(constructor,
						                          targetRef.ToExpression(),
						                          interceptors,
						                          typeTokenFieldExp,
						                          methodOnTargetTokenExp,
						                          methodInfoTokenExp,
						                          new ReferencesToObjectArrayExpression(dereferencedArguments),
						                          SelfReference.Self.ToExpression(),
						                          methodInvocationExpression,
						                          new AddressOfReferenceExpression(methodInterceptors));
				}
			}
			else
			{
				if (methodInterceptors == null)
				{
					newInvocImpl =
						new NewInstanceExpression(constructor,
						                          targetRef.ToExpression(),
						                          interceptors,
						                          typeTokenFieldExp,
						                          methodInfoTokenExp,
						                          new ReferencesToObjectArrayExpression(dereferencedArguments),
						                          SelfReference.Self.ToExpression());
				}
				else
				{
					MethodInvocationExpression methodInvocationExpression =
						new MethodInvocationExpression(proxyGenerationOptionsField, proxyGenerationOptions_Selector);
					methodInvocationExpression.VirtualCall = true;

					newInvocImpl =
						new NewInstanceExpression(constructor,
						                          targetRef.ToExpression(),
						                          interceptors,
						                          typeTokenFieldExp,
						                          methodInfoTokenExp,
						                          new ReferencesToObjectArrayExpression(dereferencedArguments),
						                          SelfReference.Self.ToExpression(),
						                          methodInvocationExpression,
						                          new AddressOfReferenceExpression(methodInterceptors));
				}
			}

			methodEmitter.CodeBuilder.AddStatement(new AssignStatement(invocationImplLocal, newInvocImpl));

			if (method.ContainsGenericParameters)
			{
				EmitLoadGenricMethodArguments(methodEmitter, method.MakeGenericMethod(genericMethodArgs), invocationImplLocal);
			}

			methodEmitter.CodeBuilder.AddStatement(
				new ExpressionStatement(new MethodInvocationExpression(invocationImplLocal, Constants.AbstractInvocationProceed)));

			CopyOutAndRefParameters(dereferencedArguments, invocationImplLocal, method, methodEmitter);

			if (method.ReturnType != typeof(void))
			{
				// Emit code to return with cast from ReturnValue
				MethodInvocationExpression getRetVal =
					new MethodInvocationExpression(invocationImplLocal, typeof(AbstractInvocation).GetMethod("get_ReturnValue"));

				methodEmitter.CodeBuilder.AddStatement(
					new ReturnStatement(new ConvertExpression(methodEmitter.ReturnType, getRetVal)));
			}
			else
			{
				methodEmitter.CodeBuilder.AddStatement(new ReturnStatement());
			}

			return methodEmitter;
		}

		private void EmitLoadGenricMethodArguments(MethodEmitter methodEmitter, MethodInfo method,
												   LocalReference invocationImplLocal)
		{
#if SILVERLIGHT
			Type[] genericParameters =
				Castle.Core.Extensions.SilverlightExtensions.FindAll(method.GetGenericArguments(), delegate(Type t) { return t.IsGenericParameter; });
#else
			Type[] genericParameters =
				Array.FindAll(method.GetGenericArguments(), delegate(Type t) { return t.IsGenericParameter; });
#endif
			LocalReference genericParamsArrayLocal = methodEmitter.CodeBuilder.DeclareLocal(typeof(Type[]));
			methodEmitter.CodeBuilder.AddStatement(
				new AssignStatement(genericParamsArrayLocal, new NewArrayExpression(genericParameters.Length, typeof(Type))));

			for (int i = 0; i < genericParameters.Length; ++i)
			{
				methodEmitter.CodeBuilder.AddStatement(
					new AssignArrayStatement(genericParamsArrayLocal, i, new TypeTokenExpression(genericParameters[i])));
			}
			MethodInfo setGenericsArgs =
				typeof(AbstractInvocation).GetMethod("SetGenericMethodArguments", new Type[] { typeof(Type[]) });
			methodEmitter.CodeBuilder.AddStatement(new ExpressionStatement(
													new MethodInvocationExpression(invocationImplLocal, setGenericsArgs,
																				   new ReferenceExpression(
																					genericParamsArrayLocal))));
		}

		private static void CopyOutAndRefParameters(
			TypeReference[] dereferencedArguments, LocalReference invocationImplLocal, MethodInfo method,
			MethodEmitter methodEmitter)
		{
			ParameterInfo[] parameters = method.GetParameters();
			bool hasByRefParam = false;
			for (int i = 0; i < parameters.Length; i++)
			{
				if (parameters[i].ParameterType.IsByRef)
					hasByRefParam = true;
			}
			if (!hasByRefParam)
				return; //saving the need to create locals if there is no need
			LocalReference invocationArgs = methodEmitter.CodeBuilder.DeclareLocal(typeof(object[]));
			methodEmitter.CodeBuilder.AddStatement(
				new AssignStatement(invocationArgs,
									new MethodInvocationExpression(invocationImplLocal, invocation_getArgumentsMethod)
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

		protected void GenerateConstructor(ClassEmitter emitter, params FieldReference[] fields)
		{
			GenerateConstructor(emitter, null, fields);
		}

		protected void GenerateConstructor(
			ClassEmitter emitter, ConstructorInfo baseConstructor, params FieldReference[] fields)
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

				for (int i = offset; i < offset + baseConstructorParams.Length; i++)
				{
					ParameterInfo paramInfo = baseConstructorParams[i - offset];
					args[i] = new ArgumentReference(paramInfo.ParameterType);
				}
			}
			else
			{
				args = new ArgumentReference[fields.Length];
			}

			for (int i = 0; i < fields.Length; i++)
			{
				args[i] = new ArgumentReference(fields[i].Reference.FieldType);
			}

			ConstructorEmitter constructor = emitter.CreateConstructor(args);

			for (int i = 0; i < fields.Length; i++)
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

			ConstructorInfo defaultConstructor = baseClass.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);

			if (defaultConstructor == null)
			{
				defaultConstructor = baseClass.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);

				if (defaultConstructor == null || defaultConstructor.IsPrivate)
				{
					return;
				}
			}

			ConstructorEmitter constructor = emitter.CreateConstructor();

			// initialize fields with an empty interceptor

			constructor.CodeBuilder.AddStatement(
				new AssignStatement(interceptorField, new NewArrayExpression(1, typeof(IInterceptor))));
			constructor.CodeBuilder.AddStatement(
				new AssignArrayStatement(interceptorField, 0, new NewInstanceExpression(typeof(StandardInterceptor), new Type[0])));

			// Invoke base constructor

			constructor.CodeBuilder.InvokeBaseConstructor(defaultConstructor);

			constructor.CodeBuilder.AddStatement(new ReturnStatement());
		}

		#region First level attributes

		protected MethodAttributes ObtainMethodAttributes(MethodInfo method)
		{
			MethodAttributes atts = MethodAttributes.Virtual;

			if (ShouldCreateNewSlot(method))
			{
				atts |= MethodAttributes.NewSlot;
			}

			if (method.IsPublic)
			{
				atts |= MethodAttributes.Public;
			}

			if (method.IsHideBySig)
			{
				atts |= MethodAttributes.HideBySig;
			}
			if (InternalsHelper.IsInternal(method) && InternalsHelper.IsInternalToDynamicProxy(method.DeclaringType.Assembly))
			{
				atts |= MethodAttributes.Assembly;
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
			return PropertyAttributes.None;
		}

		#endregion

		protected MethodBuilder CreateCallbackMethod(ClassEmitter emitter, MethodInfo methodInfo, MethodInfo methodOnTarget)
		{
			MethodInfo targetMethod = methodOnTarget ?? methodInfo;

			if (targetMethod.IsAbstract)
				return null;

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

			for (int i = 0; i < callBackMethod.Arguments.Length; i++)
			{
				exps[i] = callBackMethod.Arguments[i].ToExpression();
			}

			// invocation on base class

			callBackMethod.CodeBuilder.AddStatement(
				new ReturnStatement(new MethodInvocationExpression(GetProxyTargetReference(), targetMethod, exps)));

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
			ClassEmitter emitter,
			Type targetType,
			Type targetForInvocation,
			MethodInfo methodInfo,
			MethodInfo callbackMethod,
			ConstructorVersion version)
		{
			CheckNotGenericTypeDefinition(targetType, "targetType");
			CheckNotGenericTypeDefinition(targetForInvocation, "targetForInvocation");
			return
				BuildInvocationNestedType(emitter, targetType, targetForInvocation, methodInfo, callbackMethod, version, false);
		}

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
		/// <param name="allowChangeTarget">If true the invocation will implement the IChangeProxyTarget interface</param>
		/// <returns></returns>
		protected NestedClassEmitter BuildInvocationNestedType(
			ClassEmitter emitter,
			Type targetType,
			Type targetForInvocation,
			MethodInfo methodInfo,
			MethodInfo callbackMethod,
			ConstructorVersion version,
			bool allowChangeTarget)
		{
			CheckNotGenericTypeDefinition(targetType, "targetType");
			CheckNotGenericTypeDefinition(targetForInvocation, "targetForInvocation");

			nestedCounter++;

			Type[] interfaces = new Type[0];

			if (allowChangeTarget)
			{
				interfaces = new Type[] { typeof(IChangeProxyTarget) };
			}

			NestedClassEmitter nested =
				new NestedClassEmitter(emitter,
									   string.Format("Invocation{0}_{1}", methodInfo.Name, nestedCounter.ToString()),
									   typeof(AbstractInvocation),
									   interfaces);

			// invocation only needs to mirror the generic parameters of the MethodInfo
			// targetType cannot be a generic type definition
			nested.CopyGenericParametersFromMethod(methodInfo);

			// Create the invocation fields

			FieldReference targetRef = nested.CreateField("target", targetForInvocation);

			// Create constructor

			CreateIInvocationConstructor(targetForInvocation, nested, targetRef, version);

			if (allowChangeTarget)
			{
				ArgumentReference argument1 = new ArgumentReference(typeof(object));
				MethodEmitter methodEmitter =
					nested.CreateMethod("ChangeInvocationTarget", MethodAttributes.Public | MethodAttributes.Virtual,
										typeof(void), argument1);
				methodEmitter.CodeBuilder.AddStatement(
					new AssignStatement(targetRef,
										new ConvertExpression(targetForInvocation, argument1.ToExpression())
						)
					);
				methodEmitter.CodeBuilder.AddStatement(new ReturnStatement());
			}

			// InvokeMethodOnTarget implementation

			if (callbackMethod != null)
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();

				CreateIInvocationInvokeOnTarget(emitter, nested, parameters, targetRef, callbackMethod);
			}
			else if (IsMixinMethod(methodInfo))
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();
				CreateIInvocationInvokeOnTarget(emitter, nested, parameters, targetRef, methodInfo);
			}
			else
			{
				CreateEmptyIInvocationInvokeOnTarget(nested);
			}

#if !SILVERLIGHT
			nested.DefineCustomAttribute(new SerializableAttribute());
#endif

			return nested;
		}

		protected void CreateIInvocationInvokeOnTarget(
			ClassEmitter targetTypeEmitter,
			NestedClassEmitter nested,
			ParameterInfo[] parameters,
			FieldReference targetField,
			MethodInfo callbackMethod)
		{
			const MethodAttributes methodAtts = MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual;

			MethodEmitter method =
				nested.CreateMethod("InvokeMethodOnTarget", methodAtts, typeof(void));

			Expression[] args = new Expression[parameters.Length];

			// Idea: instead of grab parameters one by one
			// we should grab an array
			Dictionary<int, LocalReference> byRefArguments = new Dictionary<int, LocalReference>();

			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo param = parameters[i];

				Type paramType = GetParamType(nested, param.ParameterType);
				if (paramType.IsByRef)
				{
					LocalReference localReference = method.CodeBuilder.DeclareLocal(paramType.GetElementType());
					method.CodeBuilder.AddStatement(
						new AssignStatement(localReference,
											new ConvertExpression(paramType.GetElementType(),
																  new MethodInvocationExpression(SelfReference.Self,
																								 typeof(AbstractInvocation).GetMethod(
																									"GetArgumentValue"),
																								 new LiteralIntExpression(i)))));
					ByRefReference byRefReference = new ByRefReference(localReference);
					args[i] = new ReferenceExpression(byRefReference);
					byRefArguments[i] = localReference;
				}
				else
				{
					args[i] =
						new ConvertExpression(paramType,
											  new MethodInvocationExpression(SelfReference.Self,
																			 typeof(AbstractInvocation).GetMethod("GetArgumentValue"),
																			 new LiteralIntExpression(i)));
				}
			}

			if (callbackMethod.IsGenericMethod)
			{
				callbackMethod = callbackMethod.MakeGenericMethod(nested.GetGenericArgumentsFor(callbackMethod));
			}

			MethodInvocationExpression baseMethodInvExp = new MethodInvocationExpression(targetField, callbackMethod, args);
			baseMethodInvExp.VirtualCall = true;

			LocalReference ret_local = null;

			if (callbackMethod.ReturnType != typeof(void))
			{
				if (callbackMethod.ReturnType.IsGenericParameter)
				{
					ret_local = method.CodeBuilder.DeclareLocal(nested.GetGenericArgument(callbackMethod.ReturnType.Name));
				}
				else if (HasGenericParameters(callbackMethod.ReturnType))
				{
					ret_local =
						method.CodeBuilder.DeclareLocal(
							callbackMethod.ReturnType.GetGenericTypeDefinition().MakeGenericType(
								nested.GetGenericArgumentsFor(callbackMethod.ReturnType)));
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

			foreach (KeyValuePair<int, LocalReference> byRefArgument in byRefArguments)
			{
				int index = (int)byRefArgument.Key;
				LocalReference localReference = (LocalReference)byRefArgument.Value;
				method.CodeBuilder.AddStatement(
					new ExpressionStatement(
						new MethodInvocationExpression(SelfReference.Self,
													   typeof(AbstractInvocation).GetMethod("SetArgumentValue"),
													   new LiteralIntExpression(index),
													   new ConvertExpression(typeof(object), localReference.Type,
																			 new ReferenceExpression(localReference)))
						));
			}

			if (callbackMethod.ReturnType != typeof(void))
			{
				MethodInvocationExpression setRetVal =
					new MethodInvocationExpression(SelfReference.Self,
												   typeof(AbstractInvocation).GetMethod("set_ReturnValue"),
												   new ConvertExpression(typeof(object), ret_local.Type, ret_local.ToExpression()));

				method.CodeBuilder.AddStatement(new ExpressionStatement(setRetVal));
			}

			method.CodeBuilder.AddStatement(new ReturnStatement());
		}

		private Type GetParamType(NestedClassEmitter nested, Type paramType)
		{
			if (HasGenericParameters(paramType))
			{
				return paramType.GetGenericTypeDefinition().MakeGenericType(nested.GetGenericArgumentsFor(paramType));
			}
			if(paramType.IsGenericType)
			{
				Type[] arguments = paramType.GetGenericArguments();
				if (HasAnyGenericParameters(nested, arguments))
					return paramType.GetGenericTypeDefinition().MakeGenericType(arguments);
			}
			if (paramType.IsGenericParameter)
			{
				return nested.GetGenericArgument(paramType.Name);
			}
			if (paramType.IsArray)
			{
				var elementType = GetParamType(nested, paramType.GetElementType());
				return elementType.MakeArrayType();
			}
			return paramType;
		}

		private bool HasAnyGenericParameters(NestedClassEmitter emitter, Type[] arguments)
		{
			bool hasAnyGenericParameters = false;
			for(int i = 0; i < arguments.Length; i++)
			{
				var newType = GetParamType(emitter, arguments[i]);
				if(!ReferenceEquals(newType,arguments[i]))
				{
					arguments[i] = newType;
					hasAnyGenericParameters = true;
				}
			}
			return hasAnyGenericParameters;
		}

		protected void CreateEmptyIInvocationInvokeOnTarget(NestedClassEmitter nested)
		{
			const MethodAttributes methodAtts = MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual;

			MethodEmitter method =
				nested.CreateMethod("InvokeMethodOnTarget", methodAtts, typeof(void));

			// TODO: throw exception

			String message =
				String.Format("This is a DynamicProxy2 error: the interceptor attempted " +
							  "to 'Proceed' for a method without a target, for example, an interface method or an abstract method");

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
		protected void CreateIInvocationConstructor(
			Type targetFieldType, NestedClassEmitter nested, FieldReference targetField, ConstructorVersion version)
		{
			ArgumentReference cArg0 = new ArgumentReference(targetFieldType);
			ArgumentReference cArg1 = new ArgumentReference(typeof(IInterceptor[]));
			ArgumentReference cArg2 = new ArgumentReference(typeof(Type));
			ArgumentReference cArg3 = new ArgumentReference(typeof(MethodInfo));
			ArgumentReference cArg4 = null;
			ArgumentReference cArg5 = new ArgumentReference(typeof(object[]));
			ArgumentReference cArg6 = new ArgumentReference(typeof(object));
			ArgumentReference cArg7 = new ArgumentReference(typeof(IInterceptorSelector));
			ArgumentReference cArg8 = new ArgumentReference(typeof(IInterceptor[]).MakeByRefType());

			if (version == ConstructorVersion.WithTargetMethod)
			{
				cArg4 = new ArgumentReference(typeof(MethodInfo), 4);
			}

			ConstructorEmitter constructor;
			if (cArg4 == null)
			{
				if (proxyGenerationOptions.Selector != null)
				{
					constructor = nested.CreateConstructor(cArg0, cArg1, cArg2, cArg3, cArg5, cArg6, cArg7, cArg8);
				}
				else
				{
					constructor = nested.CreateConstructor(cArg0, cArg1, cArg2, cArg3, cArg5, cArg6);
				}
			}
			else
			{
				if (proxyGenerationOptions.Selector != null)
				{
					constructor = nested.CreateConstructor(cArg0, cArg1, cArg2, cArg3, cArg4, cArg5, cArg6, cArg7, cArg8);
				}
				else
				{
					constructor = nested.CreateConstructor(cArg0, cArg1, cArg2, cArg3, cArg4, cArg5, cArg6);
				}
			}

			if (cArg4 == null)
			{
				if (proxyGenerationOptions.Selector != null)
				{
					constructor.CodeBuilder.InvokeBaseConstructor(
						Constants.AbstractInvocationConstructorWithoutTargetMethodWithSelector,
						cArg0, cArg6, cArg1, cArg2, cArg3, cArg5, cArg7, cArg8);
				}
				else
				{
					constructor.CodeBuilder.InvokeBaseConstructor(
						Constants.AbstractInvocationConstructorWithoutTargetMethod,
						cArg0, cArg6, cArg1, cArg2, cArg3, cArg5);
				}
			}
			else
			{
				if (proxyGenerationOptions.Selector != null)
				{
					constructor.CodeBuilder.InvokeBaseConstructor(
						Constants.AbstractInvocationConstructorWithTargetMethodWithSelector,
						cArg0, cArg6, cArg1, cArg2, cArg3, cArg4, cArg5, cArg7, cArg8);
				}
				else
				{
					constructor.CodeBuilder.InvokeBaseConstructor(
						Constants.AbstractInvocationConstructorWithTargetMethod,
						cArg0, cArg6, cArg1, cArg2, cArg3, cArg4, cArg5);
				}
			}

			constructor.CodeBuilder.AddStatement(new AssignStatement(targetField, cArg0.ToExpression()));
			constructor.CodeBuilder.AddStatement(new ReturnStatement());
		}

		#endregion

		#region Custom Attribute handling

		protected void ReplicateNonInheritableAttributes(Type targetType, ClassEmitter emitter)
		{
			object[] attrs = targetType.GetCustomAttributes(false);

			foreach (Attribute attribute in attrs)
			{
				if (ShouldSkipAttributeReplication(attribute)) continue;

				emitter.DefineCustomAttribute(attribute);
			}
		}

		protected void ReplicateNonInheritableAttributes(MethodInfo method, MethodEmitter emitter)
		{
			object[] attrs = method.GetCustomAttributes(false);

			foreach (Attribute attribute in attrs)
			{
				if (ShouldSkipAttributeReplication(attribute)) continue;

				emitter.DefineCustomAttribute(attribute);
			}
		}

		protected IEnumerable<Attribute> GetNonInheritableAttributes(PropertyInfo propertyInfo)
		{
			object[] attrs = propertyInfo.GetCustomAttributes(false);

			foreach (Attribute attribute in attrs)
			{
				if (ShouldSkipAttributeReplication(attribute)) continue;

				yield return attribute;
			}
		}

		#endregion

		#region Type tokens related operations

		protected void GenerateConstructors(ClassEmitter emitter, Type baseType, params FieldReference[] fields)
		{
			ConstructorInfo[] constructors =
				baseType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			foreach (ConstructorInfo constructor in constructors)
			{
				if (constructor.IsPublic || constructor.IsFamily || constructor.IsFamilyOrAssembly
					|| (constructor.IsAssembly && InternalsHelper.IsInternalToDynamicProxy(constructor.DeclaringType.Assembly)))
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
		protected void CreateInitializeCacheMethodBody(
			Type targetType, MethodInfo[] methods, ClassEmitter classEmitter, ConstructorEmitter typeInitializerConstructor)
		{
			typeTokenField = classEmitter.CreateStaticField("typeTokenCache", typeof(Type));

			typeInitializerConstructor.CodeBuilder.AddStatement(
				new AssignStatement(typeTokenField, new TypeTokenExpression(targetType)));

			CacheMethodTokens(classEmitter, methods, typeInitializerConstructor);
		}

		protected void CacheMethodTokens(
			ClassEmitter classEmitter, MethodInfo[] methods, ConstructorEmitter typeInitializerConstructor)
		{
			foreach (MethodInfo method in methods)
			{
				AddFieldToCacheMethodTokenAndStatementsToInitialize(method, typeInitializerConstructor, classEmitter);
			}
		}

		protected void AddFieldToCacheMethodTokenAndStatementsToInitialize(
			MethodInfo method, ConstructorEmitter typeInitializerConstructor, ClassEmitter classEmitter)
		{
			// Aparently we cannot cache generic methods
			if (method.IsGenericMethod)
				return;

			if (!method2TokenField.ContainsKey (method))
			{
				FieldReference fieldCache =
					classEmitter.CreateStaticField("tokenCache" + fieldCount++, typeof(MethodInfo));

				method2TokenField.Add(method, fieldCache);

				typeInitializerConstructor.CodeBuilder.AddStatement(
					new AssignStatement(fieldCache, new MethodTokenExpression(method)));
			}
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

		protected void ImplementProxyTargetAccessor(Type targetType, ClassEmitter emitter, FieldReference interceptorsField)
		{
			MethodAttributes attributes = MethodAttributes.Virtual | MethodAttributes.Public;

			MethodEmitter DynProxyGetTarget =
				emitter.CreateMethod("DynProxyGetTarget", attributes, typeof(object));

			DynProxyGetTarget.CodeBuilder.AddStatement(
				new ReturnStatement(new ConvertExpression(typeof(object), targetType, GetProxyTargetReference().ToExpression())));

			MethodEmitter GetInterceptors =
				emitter.CreateMethod("GetInterceptors", attributes, typeof(IInterceptor[]));

			GetInterceptors.CodeBuilder.AddStatement(
				new ReturnStatement(interceptorsField)
				);
		}

		#endregion

		#region Utility methods

		protected void CollectMethodsToProxy(List<MethodInfo> methodList, Type type, bool onlyVirtuals)
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

		protected void CollectPropertyMethodsToProxy(
			List<MethodInfo> methodList, Type type, bool onlyVirtuals, ClassEmitter emitter, out PropertyToGenerate[] propsToGenerate)
		{
			if (type.IsInterface)
			{
				List<PropertyToGenerate> toGenerateList = new List<PropertyToGenerate>();

				toGenerateList.AddRange(CollectProperties(methodList, type, onlyVirtuals, emitter));

				Type[] typeChain = type.FindInterfaces(new TypeFilter(NoFilter), null);

				foreach (Type interType in typeChain)
				{
					toGenerateList.AddRange(CollectProperties(methodList, interType, onlyVirtuals, emitter));
				}

				propsToGenerate = toGenerateList.ToArray();
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

			bool isInternalsAndNotVisibleToDynamicProxy = InternalsHelper.IsInternal(method);
			if (isInternalsAndNotVisibleToDynamicProxy)
			{
				isInternalsAndNotVisibleToDynamicProxy = InternalsHelper.IsInternalToDynamicProxy(method.DeclaringType.Assembly) ==
														 false;
			}

			if (isInternalsAndNotVisibleToDynamicProxy)
				return false;

			if (onlyVirtuals && !method.IsVirtual)
			{
#if SILVERLIGHT
				if (method.DeclaringType != typeof(object))
#else
				if (method.DeclaringType != typeof (object) && method.DeclaringType != typeof (MarshalByRefObject))
#endif
				{
					ProxyGenerationOptions.Hook.NonVirtualMemberNotification(targetType, method);
				}

				return false;
			}

			//can only proxy methods that are public or protected (or internals that have already been checked above)
			if ((method.IsPublic || method.IsFamily || method.IsAssembly || method.IsFamilyOrAssembly) == false)
				return false;

			if (method.DeclaringType == typeof(object))
			{
				return false;
			}

#if !SILVERLIGHT
			if (method.DeclaringType == typeof (MarshalByRefObject))
			{
				return false;
			}
#endif

			return ProxyGenerationOptions.Hook.ShouldInterceptMethod(targetType, method);
		}

		protected MethodInfo[] CollectMethodsAndProperties(
			ClassEmitter emitter,
			Type targetType,
			out PropertyToGenerate[] propsToGenerate,
			out EventToGenerate[] eventsToGenerate)
		{
			bool onlyVirtuals = CanOnlyProxyVirtual();

			return CollectMethodsAndProperties(emitter, targetType, onlyVirtuals, out propsToGenerate, out eventsToGenerate);
		}

		protected MethodInfo[] CollectMethodsAndProperties(
			ClassEmitter emitter,
			Type targetType,
			bool onlyVirtuals,
			out PropertyToGenerate[] propsToGenerate,
			out EventToGenerate[] eventsToGenerate)
		{
			List<MethodInfo> methodsList = new List<MethodInfo>();

			CollectMethodsToProxy(methodsList, targetType, onlyVirtuals);
			CollectPropertyMethodsToProxy(methodsList, targetType, onlyVirtuals, emitter, out propsToGenerate);
			CollectEventMethodsToProxy(methodsList, targetType, onlyVirtuals, emitter, out eventsToGenerate);
			return methodsList.ToArray();
		}

		private void CollectEventMethodsToProxy(
			List<MethodInfo> methodList, Type type, bool onlyVirtuals, ClassEmitter emitter, out EventToGenerate[] eventsToGenerates)
		{
			if (type.IsInterface)
			{
				List<EventToGenerate> toGenerateList = new List<EventToGenerate>();

				toGenerateList.AddRange(CollectEvents(methodList, type, onlyVirtuals));

				Type[] typeChain = type.FindInterfaces(new TypeFilter(NoFilter), null);

				foreach (Type interType in typeChain)
				{
					toGenerateList.AddRange(CollectEvents(methodList, interType, onlyVirtuals));
				}

				eventsToGenerates = toGenerateList.ToArray();
			}
			else
			{
				eventsToGenerates = CollectEvents(methodList, type, onlyVirtuals);
			}
		}

		/// <summary>
		/// Checks if the method is public or protected.
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		private bool IsAccessible(MethodInfo method)
		{
			if (method.IsPublic
				|| method.IsFamily
				|| method.IsFamilyAndAssembly
				|| method.IsFamilyOrAssembly)
			{
				return true;
			}

			if (InternalsHelper.IsInternalToDynamicProxy(method.DeclaringType.Assembly)
				&& method.IsAssembly)
			{
				return true;
			}

			return false;
		}

		private bool HasGenericParameters(Type type)
		{
			if (type.IsGenericType)
			{
				Type[] genTypes = type.GetGenericArguments();

				foreach (Type genType in genTypes)
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

		private void CollectMethods(List<MethodInfo> methodsList, Type type, bool onlyVirtuals)
		{
			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

			MethodInfo[] methods = MethodFinder.GetAllInstanceMethods(type, flags);

			foreach (MethodInfo method in methods)
			{
				if (method.IsFinal)
				{
					AddMethodToGenerateNewSlot(method);
					continue;
				}

				if (method.IsSpecialName
					// This is here so we can proxy COM Types built in VB6, where properties
					// are let_Foo and set_Foo.
					&& method.Name.StartsWith("let_") == false)
				{
					continue;
				}

				if (AcceptMethod(method, onlyVirtuals))
				{
					methodsList.Add(method);
				}
			}
		}

		private EventToGenerate[] CollectEvents(List<MethodInfo> methodList, Type type, bool onlyVirtuals)
		{
			List<EventToGenerate> toGenerateList = new List<EventToGenerate>();

			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

			EventInfo[] events = type.GetEvents(flags);

			foreach (EventInfo eventInfo in events)
			{
				MethodInfo addMethod = eventInfo.GetAddMethod(true);
				MethodInfo removeMethod = eventInfo.GetRemoveMethod(true);
				bool shouldGenerate = false;

				if (addMethod != null && IsAccessible(addMethod) && AcceptMethod(addMethod, onlyVirtuals))
				{
					shouldGenerate = true;
					methodList.Add(addMethod);
				}

				if (removeMethod != null && IsAccessible(removeMethod) && AcceptMethod(removeMethod, onlyVirtuals))
				{
					shouldGenerate = true;
					methodList.Add(removeMethod);
				}

				if (shouldGenerate == false)
					continue;

				EventAttributes atts = ObtainEventAttributes(eventInfo);

				EventToGenerate eventToGenerate = new EventToGenerate(eventInfo.Name,
				                                                      eventInfo.EventHandlerType, addMethod, removeMethod, atts);

				toGenerateList.Add(eventToGenerate);
			}

			return toGenerateList.ToArray();
		}

		private EventAttributes ObtainEventAttributes(EventInfo eventInfo)
		{
			return EventAttributes.None;
		}

		private PropertyToGenerate[] CollectProperties(
			List<MethodInfo> methodList, Type type, bool onlyVirtuals, ClassEmitter emitter)
		{
			List<PropertyToGenerate> toGenerateList = new List<PropertyToGenerate>();

			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

			PropertyInfo[] properties = type.GetProperties(flags);

			foreach (PropertyInfo propInfo in properties)
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
				IEnumerable<Attribute> nonInheritableAttributes = GetNonInheritableAttributes(propInfo);
				PropertyToGenerate propToGenerate =
					new PropertyToGenerate(propInfo.Name, propInfo.PropertyType, generateReadable, generateWritable, getMethod,
					                       setMethod, atts, nonInheritableAttributes);

				toGenerateList.Add(propToGenerate);
			}

			return toGenerateList.ToArray();
		}

		/// <summary>
		/// Attributes should be replicated if they are non-inheritable,
		/// but there are some special cases where the attributes means
		/// something to the CLR, where they should be skipped.
		/// </summary>
		private bool ShouldSkipAttributeReplication(Attribute attribute)
		{
			if (SpecialCaseAttributThatShouldNotBeReplicated(attribute))
				return true;

			object[] attrs = attribute.GetType()
				.GetCustomAttributes(typeof(AttributeUsageAttribute), true);

			if (attrs.Length != 0)
			{
				AttributeUsageAttribute usage = (AttributeUsageAttribute)attrs[0];

				return usage.Inherited;
			}

			return true;
		}

		#endregion

		protected void AddMethodToGenerateNewSlot(MethodInfo method)
		{
			generateNewSlot.Add(method);
		}

		/// <summary>
		/// Checks if the method has the same signature as a method that was marked as
		/// one that should generate a new vtable slot.
		/// </summary>
		protected bool ShouldCreateNewSlot(MethodInfo method)
		{
			string methodStr = method.ToString();
			foreach (MethodInfo candidate in generateNewSlot)
			{
				if (candidate.ToString() == methodStr)
					return true;
			}
			return false;
		}

#if !SILVERLIGHT
		protected virtual void ImplementGetObjectData(ClassEmitter emitter, FieldReference interceptorsField,
		                                              FieldReference[] mixinFields,
		                                              Type[] interfaces)
		{
			if (interfaces == null)
			{
				interfaces = new Type[0];
			}

			Type[] get_type_args = new Type[] {typeof (String), typeof (bool), typeof (bool)};
			Type[] key_and_object = new Type[] {typeof (String), typeof (Object)};
			MethodInfo addValueMethod = typeof (SerializationInfo).GetMethod("AddValue", key_and_object);

			ArgumentReference arg1 = new ArgumentReference(typeof (SerializationInfo));
			ArgumentReference arg2 = new ArgumentReference(typeof (StreamingContext));
			MethodEmitter getObjectData = emitter.CreateMethod("GetObjectData",
			                                                   typeof (void), arg1, arg2);

			LocalReference typeLocal = getObjectData.CodeBuilder.DeclareLocal(typeof (Type));

			getObjectData.CodeBuilder.AddStatement(
				new AssignStatement(
					typeLocal,
					new MethodInvocationExpression(
						null,
						typeof(Type).GetMethod("GetType", get_type_args),
						new ConstReference(typeof (ProxyObjectReference).AssemblyQualifiedName).ToExpression(),
						new ConstReference(1).ToExpression(),
						new ConstReference(0).ToExpression())));

			getObjectData.CodeBuilder.AddStatement(
				new ExpressionStatement(
					new MethodInvocationExpression(
						arg1,
						typeof (SerializationInfo).GetMethod("SetType"),
						typeLocal.ToExpression())));

			getObjectData.CodeBuilder.AddStatement(
				new ExpressionStatement(
					new MethodInvocationExpression(
						arg1,
						addValueMethod,
						new ConstReference("__interceptors").ToExpression(),
						interceptorsField.ToExpression())));

			foreach (FieldReference mixinFieldReference in mixinFields)
			{
				getObjectData.CodeBuilder.AddStatement(
					new ExpressionStatement(
						new MethodInvocationExpression(
							arg1,
							addValueMethod,
							new ConstReference(
								mixinFieldReference.Reference.Name).ToExpression(),
								mixinFieldReference.ToExpression())));
			}

			LocalReference interfacesLocal = getObjectData.CodeBuilder.DeclareLocal(typeof(string[]));

			getObjectData.CodeBuilder.AddStatement(
				new AssignStatement(
					interfacesLocal,
					new NewArrayExpression(interfaces.Length, typeof(string))));

			for (int i = 0; i < interfaces.Length; i++)
			{
				getObjectData.CodeBuilder.AddStatement(
					new AssignArrayStatement(
						interfacesLocal,
						i,
						new ConstReference(interfaces[i].AssemblyQualifiedName).ToExpression()));
			}

			getObjectData.CodeBuilder.AddStatement(
				new ExpressionStatement(
					new MethodInvocationExpression(
						arg1,
						addValueMethod,
						new ConstReference("__interfaces").ToExpression(),
						interfacesLocal.ToExpression())));

			getObjectData.CodeBuilder.AddStatement(
				new ExpressionStatement(
					new MethodInvocationExpression(
						arg1,
						addValueMethod,
						new ConstReference("__baseType").ToExpression(),
						new ConstReference(emitter.BaseType.AssemblyQualifiedName).ToExpression())));

			getObjectData.CodeBuilder.AddStatement(
				new ExpressionStatement(
					new MethodInvocationExpression(
						arg1,
						addValueMethod,
						new ConstReference("__proxyGenerationOptions").ToExpression(),
						proxyGenerationOptionsField.ToExpression())));

			CustomizeGetObjectData(getObjectData.CodeBuilder, arg1, arg2);

			getObjectData.CodeBuilder.AddStatement(new ReturnStatement());
		}
#endif

		protected virtual void CustomizeGetObjectData(
			AbstractCodeBuilder codebuilder, ArgumentReference arg1,
			ArgumentReference arg2)
		{
		}

		protected bool VerifyIfBaseImplementsGetObjectData(Type baseType)
		{
			// If base type implements ISerializable, we have to make sure
			// the GetObjectData is marked as virtual
#if !SILVERLIGHT

			if (typeof (ISerializable).IsAssignableFrom(baseType))
			{
				MethodInfo getObjectDataMethod = baseType.GetMethod("GetObjectData",
					new Type[] {typeof(SerializationInfo), typeof(StreamingContext)});

				if (getObjectDataMethod == null) //explicit interface implementation
				{
					return false;
				}

				if (!getObjectDataMethod.IsVirtual || getObjectDataMethod.IsFinal)
				{
					String message = String.Format("The type {0} implements ISerializable, but GetObjectData is not marked as virtual",
					                               baseType.FullName);
					throw new ArgumentException(message);
				}

				methodsToSkip.Add(getObjectDataMethod);

				serializationConstructor = baseType.GetConstructor(
					BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
					null,
					new Type[] {typeof (SerializationInfo), typeof (StreamingContext)},
					null);

				if (serializationConstructor == null)
				{
					String message =
						String.Format("The type {0} implements ISerializable, but failed to provide a deserialization constructor",
						              baseType.FullName);
					throw new ArgumentException(message);
				}

				return true;
			}
#endif
			return false;
		}

		private static bool SpecialCaseAttributThatShouldNotBeReplicated(Attribute attribute)
		{
			return AttributesToAvoidReplicating.Contains(attribute.GetType());
		}

		protected void RegisterMixinMethodsAndProperties(ClassEmitter emitter, ref MethodInfo[] methods,
														 ref PropertyToGenerate[] propsToGenerate,
														 ref EventToGenerate[] eventsToGenerate)
		{
			List<MethodInfo> withMixinMethods = new List<MethodInfo>(methods);
			List<PropertyToGenerate> withMixinProperties = null;
			List<EventToGenerate> withMixinEvents = null;

			foreach (Type mixinInterface in ProxyGenerationOptions.MixinData.MixinInterfaces)
			{
				PropertyToGenerate[] mixinPropsToGenerate;
				EventToGenerate[] mixinEventsToGenerate;
				MethodInfo[] mixinMethods = CollectMethodsAndProperties(emitter, mixinInterface, false,
																		out mixinPropsToGenerate, out mixinEventsToGenerate);
				foreach (MethodInfo mixinMethod in mixinMethods)
				{
					if (!method2MixinType.ContainsKey(mixinMethod))
					{
						method2MixinType[mixinMethod] = mixinInterface;
						withMixinMethods.Add(mixinMethod);
					}
				}

				if (mixinPropsToGenerate.Length > 0)
				{
					if (withMixinProperties == null)
					{
						withMixinProperties = new List<PropertyToGenerate>(propsToGenerate);
					}
					withMixinProperties.AddRange(mixinPropsToGenerate);
				}

				if (mixinEventsToGenerate.Length > 0)
				{
					if (withMixinEvents == null)
					{
						withMixinEvents = new List<EventToGenerate>(eventsToGenerate);
					}
					withMixinEvents.AddRange(mixinEventsToGenerate);
				}
			}

			if (withMixinMethods != null)
			{
				methods = withMixinMethods.ToArray();
			}

			if (withMixinProperties != null)
			{
				propsToGenerate = withMixinProperties.ToArray();
			}

			if (withMixinEvents != null)
			{
				eventsToGenerate = withMixinEvents.ToArray();
			}
		}

		protected FieldReference[] AddMixinFields(ClassEmitter emitter)
		{
			List<FieldReference> mixins = new List<FieldReference>();
			foreach (Type type in ProxyGenerationOptions.MixinData.MixinInterfaces)
			{
				FieldReference fieldReference = emitter.CreateField("__mixin_" + type.FullName.Replace(".", "_"), type);
				interface2MixinFieldReference[type] = fieldReference;
				mixins.Add(fieldReference);
			}
			return mixins.ToArray();
		}

		protected void ValidateMixinInterfaces(IEnumerable<Type> interfacesToCheckAgainst, string roleOfCheckedInterfaces)
		{
			foreach (Type interfaceType in interfacesToCheckAgainst)
			{
				ValidateMixinInterface(interfaceType, roleOfCheckedInterfaces);
			}
		}

		protected void ValidateMixinInterface(Type interfaceType, string roleOfCheckedInterface)
		{
			if (ProxyGenerationOptions.MixinData.ContainsMixin(interfaceType))
			{
				object mixinWithSameInterface = ProxyGenerationOptions.MixinData.GetMixinInstance(interfaceType);
				string message = string.Format(
						"The mixin {0} adds the interface '{1}' to the generated proxy, but the interface already exists in the proxy's {2}. " +
						"A mixin cannot add an interface already implemented in another way.",
						mixinWithSameInterface.GetType().Name,
						interfaceType.FullName,
						roleOfCheckedInterface);
				throw new InvalidMixinConfigurationException(message);
			}

			// since interfaces have to form an inheritance graph without cycles, this recursion should be safe
			ValidateMixinInterfaces(interfaceType.GetInterfaces(), roleOfCheckedInterface);
		}

		protected void AddMixinInterfaces(List<Type> interfaceList)
		{
			interfaceList.AddRange(ProxyGenerationOptions.MixinData.MixinInterfaces);
		}

		protected Reference GetTargetRef(MethodInfo method, FieldReference[] mixinFields, Reference targetRef)
		{
			if (IsMixinMethod(method))
			{
				Type interfaceType = method2MixinType[method];
				int mixinIndex = ProxyGenerationOptions.MixinData.GetMixinPosition(interfaceType);
				targetRef = mixinFields[mixinIndex];
			}
			return targetRef;
		}

		protected bool IsMixinMethod(MethodInfo methodInfo)
		{
			return method2MixinType.ContainsKey(methodInfo);
		}
	}
}
