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
	using Castle.DynamicProxy.Tokens;

	public enum ConstructorVersion
	{
		WithTargetMethod,
		WithoutTargetMethod
	}

	/// <summary>
	/// Base class that exposes the common functionalities
	/// to proxy generation.
	/// </summary>
	public abstract class BaseProxyGenerator
	{
		protected static readonly InterfaceMapping EmptyInterfaceMapping = new InterfaceMapping { InterfaceMethods = new MethodInfo[0] };

		private readonly ModuleScope scope;
		private readonly IList<MethodInfo> methodsToSkip = new List<MethodInfo>();
		private readonly Dictionary<Type, FieldReference> interface2MixinFieldReference = new Dictionary<Type, FieldReference>();
		private readonly Dictionary<MethodInfo, FieldReference> method2TokenField = new Dictionary<MethodInfo, FieldReference>();
		private int nestedCounter, callbackCounter;
		private int fieldCount = 1;
		private FieldReference typeTokenField;
		private FieldReference proxyGenerationOptionsField;
		private ProxyGenerationOptions proxyGenerationOptions;

		protected readonly Type targetType;
		protected readonly Dictionary<MethodInfo, NestedClassEmitter> method2Invocation = new Dictionary<MethodInfo, NestedClassEmitter>();
		protected ConstructorInfo serializationConstructor;

		protected BaseProxyGenerator(ModuleScope scope, Type targetType)
		{
			this.scope = scope;
			this.targetType = targetType;
		}

		protected ProxyGenerationOptions ProxyGenerationOptions
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

		protected void CheckNotGenericTypeDefinitions(IEnumerable<Type> types, string argumentName)
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

		protected abstract ConstructorVersion ConstructorVersion { get; }

		protected virtual ClassEmitter BuildClassEmitter(String typeName, Type parentType, IEnumerable<Type> interfaces)
		{
			CheckNotGenericTypeDefinition(parentType, "parentType");
			CheckNotGenericTypeDefinitions(interfaces, "interfaces");

			return new ClassEmitter(Scope, typeName, parentType, interfaces);
		}

		/// <summary>
		/// Used by dynamically implement <see cref="Core.Interceptor.IProxyTargetAccessor"/>
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

		protected MethodEmitter CreateProxiedMethod(IProxyMethod proxyMethod, ClassEmitter emitter, NestedClassEmitter invocationImpl, FieldReference interceptorsField, Reference targetRef, ConstructorVersion version, MethodInfo methodOnTarget)
		{
			string name;
			MethodAttributes atts = ObtainMethodAttributes(proxyMethod, out name);
			MethodEmitter methodEmitter = emitter.CreateMethod(name, atts);
			MethodEmitter proxiedMethod = ImplementProxiedMethod(methodEmitter,
																 proxyMethod.Method,
																 emitter,
																 invocationImpl,
																 interceptorsField,
																 targetRef,
																 version,
																 methodOnTarget);

			if (proxyMethod.Method.DeclaringType.IsInterface)
			{
				emitter.TypeBuilder.DefineMethodOverride(methodEmitter.MethodBuilder, proxyMethod.Method);

			}
			return proxiedMethod;
		}

		protected abstract Reference GetMethodTargetReference(MethodInfo method);


		protected MethodEmitter ImplementProxiedMethod(MethodEmitter methodEmitter, MethodInfo method, ClassEmitter emitter, NestedClassEmitter invocationImpl, FieldReference interceptors, Reference target, ConstructorVersion version, MethodInfo methodOnTarget)
		{
			methodEmitter.CopyParametersAndReturnTypeFrom(method, emitter);

			TypeReference[] dereferencedArguments = IndirectReference.WrapIfByRef(methodEmitter.Arguments);

			Type iinvocation = invocationImpl.TypeBuilder;

			Trace.Assert(method.IsGenericMethod == iinvocation.IsGenericTypeDefinition);
			bool isGenericInvocationClass = false;
			Type[] genericMethodArgs = Type.EmptyTypes;
			if (method.IsGenericMethod)
			{
				// bind generic method arguments to invocation's type arguments
				genericMethodArgs = methodEmitter.MethodBuilder.GetGenericArguments();
				iinvocation = iinvocation.MakeGenericType(genericMethodArgs);
				isGenericInvocationClass = true;
			}

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

			ConstructorInfo constructor = invocationImpl.Constructors[0].ConstructorBuilder;
			if (isGenericInvocationClass)
			{
				constructor = TypeBuilder.GetConstructor(iinvocation, constructor);
			}

			Expression targetMethod;
			Expression interfaceMethod;
			if (version == ConstructorVersion.WithTargetMethod)
			{
				if (method2TokenField.ContainsKey(methodOnTarget)) // Token is in the cache
				{
					targetMethod = method2TokenField[methodOnTarget].ToExpression();
				}
				else
				{
					// Not in the cache: generic method
					targetMethod = new MethodTokenExpression(methodOnTarget.MakeGenericMethod(genericMethodArgs));
				}
				interfaceMethod = methodInfoTokenExp;
			}
			else
			{
				targetMethod = methodInfoTokenExp;
				interfaceMethod = NullExpression.Instance;
			}


			NewInstanceExpression newInvocImpl;
			if (proxyGenerationOptions.Selector == null)
			{
				newInvocImpl = //actual contructor call
					new NewInstanceExpression(constructor,
											  target.ToExpression(),
											  SelfReference.Self.ToExpression(),
											  interceptors.ToExpression(),
											  typeTokenField.ToExpression(),
											  targetMethod,
											  interfaceMethod,
											  new ReferencesToObjectArrayExpression(dereferencedArguments));
			}
			else
			{
				// Create the field to store the selected interceptors for this method if an InterceptorSelector is specified
				// NOTE: If no interceptors are returned, should we invoke the base.Method directly? Looks like we should not.
				FieldReference methodInterceptors = emitter.CreateField(string.Format("{0}_interceptors", tokenFieldName), typeof(IInterceptor[]));

				MethodInvocationExpression selector =
				new MethodInvocationExpression(proxyGenerationOptionsField, ProxyGenerationOptionsMethods.GetSelector);
				selector.VirtualCall = true;

				newInvocImpl = //actual contructor call
						new NewInstanceExpression(constructor,
												  target.ToExpression(),
												  SelfReference.Self.ToExpression(),
												  interceptors.ToExpression(),
												  typeTokenField.ToExpression(),
												  targetMethod,
												  interfaceMethod,
												  new ReferencesToObjectArrayExpression(dereferencedArguments),
												  selector,
												  new AddressOfReferenceExpression(methodInterceptors));
			}

			methodEmitter.CodeBuilder.AddStatement(new AssignStatement(invocationImplLocal, newInvocImpl));

			if (method.ContainsGenericParameters)
			{
				EmitLoadGenricMethodArguments(methodEmitter, method.MakeGenericMethod(genericMethodArgs), invocationImplLocal);
			}

			methodEmitter.CodeBuilder.AddStatement(
				new ExpressionStatement(new MethodInvocationExpression(invocationImplLocal, InvocationMethods.Proceed)));

			CopyOutAndRefParameters(dereferencedArguments, invocationImplLocal, method, methodEmitter);

			if (method.ReturnType != typeof(void))
			{
				// Emit code to return with cast from ReturnValue
				MethodInvocationExpression getRetVal =
					new MethodInvocationExpression(invocationImplLocal, InvocationMethods.GetReturnValue);

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
			methodEmitter.CodeBuilder.AddStatement(new ExpressionStatement(
													new MethodInvocationExpression(invocationImplLocal, InvocationMethods.SetGenericMethodArguments,
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
									new MethodInvocationExpression(invocationImplLocal, InvocationMethods.GetArguments)
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

		private MethodAttributes ObtainMethodAttributes(IProxyMethod method, out string name)
		{
			var methodInfo = method.Method;
			MethodAttributes attributes = MethodAttributes.Virtual;
			if (IsInterfaceMethodForExplicitImplementation(method))
			{
				name = methodInfo.DeclaringType.Name + "." + methodInfo.Name;
				attributes |= MethodAttributes.Private |
				        MethodAttributes.HideBySig |
				        MethodAttributes.NewSlot |
				        MethodAttributes.Final;
			}
			else
			{
				if (method.Method.IsFinal)
				{
					attributes |= MethodAttributes.NewSlot;
				}

				if (methodInfo.IsPublic)
				{
					attributes |= MethodAttributes.Public;
				}

				if (methodInfo.IsHideBySig)
				{
					attributes |= MethodAttributes.HideBySig;
				}
				if (InternalsHelper.IsInternal(methodInfo) && InternalsHelper.IsInternalToDynamicProxy(methodInfo.DeclaringType.Assembly))
				{
					attributes |= MethodAttributes.Assembly;
				}
				if (methodInfo.IsFamilyAndAssembly)
				{
					attributes |= MethodAttributes.FamANDAssem;
				}
				else if (methodInfo.IsFamilyOrAssembly)
				{
					attributes |= MethodAttributes.FamORAssem;
				}
				else if (methodInfo.IsFamily)
				{
					attributes |= MethodAttributes.Family;
				}
				name = methodInfo.Name;
			}

			if (methodInfo.Name.StartsWith("set_") || methodInfo.Name.StartsWith("get_"))
			{
				attributes |= MethodAttributes.SpecialName;
			}
			return attributes;
		}

		protected virtual bool IsInterfaceMethodForExplicitImplementation(IProxyMethod method)
		{
			return method.Method.DeclaringType.IsInterface;
		}

		#endregion

		protected MethodBuilder CreateCallbackMethod(ClassEmitter emitter, MethodInfo methodInfo, MethodInfo methodOnTarget)
		{
			MethodInfo targetMethod = methodOnTarget ?? methodInfo;

			if (targetMethod.IsAbstract && !targetMethod.DeclaringType.IsInterface)
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
		/// <param name="emitter">The emitter.</param>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="method">The method info.</param>
		/// <param name="callbackMethod">The callback method.</param>
		/// <returns></returns>
		protected NestedClassEmitter BuildInvocationNestedType(ClassEmitter emitter, Type targetType, MethodToGenerate method, MethodInfo callbackMethod)
		{
			return BuildInvocationNestedType(emitter, targetType, method, callbackMethod, false);
		}

		/// <summary>
		/// If callbackMethod is null the InvokeOnTarget implementation
		/// is just the code to throw an exception
		/// </summary>
		/// <param name="emitter"></param>
		/// <param name="targetForInvocation"></param>
		/// <param name="method"></param>
		/// <param name="callbackMethod"></param>
		/// <param name="allowChangeTarget">If true the invocation will implement the IChangeProxyTarget interface</param>
		/// <returns></returns>
		protected NestedClassEmitter BuildInvocationNestedType(ClassEmitter emitter, Type targetForInvocation, MethodToGenerate method, MethodInfo callbackMethod, bool allowChangeTarget)
		{
			CheckNotGenericTypeDefinition(targetForInvocation, "targetForInvocation");

			nestedCounter++;

			Type[] interfaces = new Type[0];

			if (allowChangeTarget)
			{
				interfaces = new[] { typeof(IChangeProxyTarget) };
			}

			var methodInfo = method.Method;
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

			CreateIInvocationConstructor(targetForInvocation, nested, targetRef);

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

				CreateIInvocationInvokeOnTarget(nested, parameters, targetRef, callbackMethod);
			}
			else if (method.Target is Mixin)
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();
				CreateIInvocationInvokeOnTarget(nested, parameters, targetRef, methodInfo);
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

		protected void CreateIInvocationInvokeOnTarget(NestedClassEmitter nested, ParameterInfo[] parameters, FieldReference targetField, MethodInfo callbackMethod)
		{
			const MethodAttributes methodAtts = MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual;

			MethodEmitter method = nested.CreateMethod("InvokeMethodOnTarget", methodAtts, typeof(void));

			ImplementInvokeMethodOnTarget(nested, parameters, method, callbackMethod, targetField);
		}

		protected virtual void ImplementInvokeMethodOnTarget(NestedClassEmitter nested, ParameterInfo[] parameters, MethodEmitter method, MethodInfo callbackMethod, Reference targetField)
		{
			Expression[] args = new Expression[parameters.Length];

			// Idea: instead of grab parameters one by one
			// we should grab an array
			Dictionary<int, LocalReference> byRefArguments = new Dictionary<int, LocalReference>();

			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo param = parameters[i];

				Type paramType = TypeUtil.GetClosedParameterType(nested, param.ParameterType);
				if (paramType.IsByRef)
				{
					LocalReference localReference = method.CodeBuilder.DeclareLocal(paramType.GetElementType());
					method.CodeBuilder.AddStatement(
						new AssignStatement(localReference,
											new ConvertExpression(paramType.GetElementType(),
																  new MethodInvocationExpression(SelfReference.Self,
																								 InvocationMethods.GetArgumentValue,
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
																			 InvocationMethods.GetArgumentValue,
																			 new LiteralIntExpression(i)));
				}
			}

			if (callbackMethod.IsGenericMethod)
			{
				callbackMethod = callbackMethod.MakeGenericMethod(nested.GetGenericArgumentsFor(callbackMethod));
			}

			MethodInvocationExpression baseMethodInvExp = new MethodInvocationExpression(targetField, callbackMethod, args);
			baseMethodInvExp.VirtualCall = true;

			LocalReference returnValue = null;
			if (callbackMethod.ReturnType != typeof(void))
			{
				Type returnType = TypeUtil.GetClosedParameterType(nested, callbackMethod.ReturnType);
				returnValue = method.CodeBuilder.DeclareLocal(returnType);
				method.CodeBuilder.AddStatement(new AssignStatement(returnValue, baseMethodInvExp));
			}
			else
			{
				method.CodeBuilder.AddStatement(new ExpressionStatement(baseMethodInvExp));
			}

			foreach (KeyValuePair<int, LocalReference> byRefArgument in byRefArguments)
			{
				int index = byRefArgument.Key;
				LocalReference localReference = byRefArgument.Value;
				method.CodeBuilder.AddStatement(
					new ExpressionStatement(
						new MethodInvocationExpression(SelfReference.Self,
													   InvocationMethods.SetArgumentValue,
													   new LiteralIntExpression(index),
													   new ConvertExpression(typeof(object), localReference.Type,
																			 new ReferenceExpression(localReference)))
						));
			}

			if (callbackMethod.ReturnType != typeof(void))
			{
				MethodInvocationExpression setRetVal =
					new MethodInvocationExpression(SelfReference.Self,
												   InvocationMethods.SetReturnValue,
												   new ConvertExpression(typeof(object), returnValue.Type, returnValue.ToExpression()));

				method.CodeBuilder.AddStatement(new ExpressionStatement(setRetVal));
			}

			method.CodeBuilder.AddStatement(new ReturnStatement());
		}





		protected void CreateEmptyIInvocationInvokeOnTarget(NestedClassEmitter nested)
		{
			const MethodAttributes methodAtts = MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual;

			MethodEmitter method =
				nested.CreateMethod("InvokeMethodOnTarget", methodAtts, typeof(void));

			String message = "This is a DynamicProxy2 error: the interceptor attempted " +
							 "to 'Proceed' for a method without a target, for example, an interface method or an abstract method";

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
		protected void CreateIInvocationConstructor(Type targetFieldType, NestedClassEmitter nested, FieldReference targetField)
		{
			ArgumentReference cArg0 = new ArgumentReference(targetFieldType);
			ArgumentReference cArg1 = new ArgumentReference(typeof(object));
			ArgumentReference cArg2 = new ArgumentReference(typeof(IInterceptor[]));
			ArgumentReference cArg3 = new ArgumentReference(typeof(Type));
			ArgumentReference cArg4 = new ArgumentReference(typeof(MethodInfo));
			ArgumentReference cArg5 = new ArgumentReference(typeof(MethodInfo));
			ArgumentReference cArg6 = new ArgumentReference(typeof(object[]));
			ArgumentReference cArg7 = new ArgumentReference(typeof(IInterceptorSelector));
			ArgumentReference cArg8 = new ArgumentReference(typeof(IInterceptor[]).MakeByRefType());

			ConstructorEmitter constructor;
			if (proxyGenerationOptions.Selector != null)
			{
				constructor = nested.CreateConstructor(cArg0, cArg1, cArg2, cArg3, cArg4, cArg5, cArg6, cArg7, cArg8);

				constructor.CodeBuilder.InvokeBaseConstructor(
					InvocationMethods.ConstructorWithTargetMethodWithSelector,
					cArg0, cArg1, cArg2, cArg3, cArg4, cArg5, cArg6, cArg7, cArg8);
			}
			else
			{
				constructor = nested.CreateConstructor(cArg0, cArg1, cArg2, cArg3, cArg4, cArg5, cArg6);

				constructor.CodeBuilder.InvokeBaseConstructor(
					InvocationMethods.ConstructorWithTargetMethod,
					cArg0, cArg1, cArg2, cArg3, cArg4, cArg5, cArg6);
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
			Type targetType, IEnumerable<MethodInfo> methods, ClassEmitter classEmitter, ConstructorEmitter typeInitializerConstructor)
		{
			typeTokenField = classEmitter.CreateStaticField("typeTokenCache", typeof(Type));

			typeInitializerConstructor.CodeBuilder.AddStatement(
				new AssignStatement(typeTokenField, new TypeTokenExpression(targetType)));

			CacheMethodTokens(classEmitter, methods, typeInitializerConstructor);
		}

		protected void CacheMethodTokens(
			ClassEmitter classEmitter, IEnumerable<MethodInfo> methods, ConstructorEmitter typeInitializerConstructor)
		{
			foreach (MethodInfo method in methods)
			{
				if (method.DeclaringType == typeof(object)) continue;
				AddFieldToCacheMethodTokenAndStatementsToInitialize(method, typeInitializerConstructor, classEmitter);
			}
		}

		protected void AddFieldToCacheMethodTokenAndStatementsToInitialize(
			MethodInfo method, ConstructorEmitter typeInitializerConstructor, ClassEmitter classEmitter)
		{
			// Aparently we cannot cache generic methods
			if (method.IsGenericMethod)
				return;

			if (!method2TokenField.ContainsKey(method))
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

		protected void ImplementProxyTargetAccessor(Type targetType, ClassEmitter emitter, FieldReference interceptorsField)
		{
			MethodAttributes attributes = MethodAttributes.Virtual | MethodAttributes.Public;

			MethodEmitter dynProxyGetTarget = emitter.CreateMethod("DynProxyGetTarget", attributes, typeof (object));

			dynProxyGetTarget.CodeBuilder.AddStatement(
				new ReturnStatement(new ConvertExpression(typeof(object), targetType, GetProxyTargetReference().ToExpression())));

			MethodEmitter getInterceptors = emitter.CreateMethod("GetInterceptors", attributes, typeof (IInterceptor[]));

			getInterceptors.CodeBuilder.AddStatement(
				new ReturnStatement(interceptorsField));
		}

		#endregion

		#region Utility methods



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

#if !SILVERLIGHT
		protected virtual void ImplementGetObjectData(ClassEmitter emitter, FieldReference interceptorsField,
													  FieldReference[] mixinFields,
													  Type[] interfaces)
		{
			if (interfaces == null)
			{
				interfaces = new Type[0];
			}

			ArgumentReference serializationInfo = new ArgumentReference(typeof(SerializationInfo));
			ArgumentReference streamingContext = new ArgumentReference(typeof(StreamingContext));
			MethodEmitter getObjectData = emitter.CreateMethod("GetObjectData",
															   typeof(void), serializationInfo, streamingContext);

			LocalReference typeLocal = getObjectData.CodeBuilder.DeclareLocal(typeof(Type));

			getObjectData.CodeBuilder.AddStatement(
				new AssignStatement(
					typeLocal,
					new MethodInvocationExpression(
						null,
						TypeMethods.StaticGetType,
						new ConstReference(typeof(ProxyObjectReference).AssemblyQualifiedName).ToExpression(),
						new ConstReference(1).ToExpression(),
						new ConstReference(0).ToExpression())));

			getObjectData.CodeBuilder.AddStatement(
				new ExpressionStatement(
					new MethodInvocationExpression(
						serializationInfo,
						SerializationInfoMethods.SetType,
						typeLocal.ToExpression())));

			getObjectData.CodeBuilder.AddStatement(
				new ExpressionStatement(
					new MethodInvocationExpression(
						serializationInfo,
						SerializationInfoMethods.AddValue_Object,
						new ConstReference("__interceptors").ToExpression(),
						interceptorsField.ToExpression())));

			foreach (FieldReference mixinFieldReference in mixinFields)
			{
				getObjectData.CodeBuilder.AddStatement(
					new ExpressionStatement(
						new MethodInvocationExpression(
							serializationInfo,
							SerializationInfoMethods.AddValue_Object,
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
						serializationInfo,
						SerializationInfoMethods.AddValue_Object,
						new ConstReference("__interfaces").ToExpression(),
						interfacesLocal.ToExpression())));

			getObjectData.CodeBuilder.AddStatement(
				new ExpressionStatement(
					new MethodInvocationExpression(
						serializationInfo,
						SerializationInfoMethods.AddValue_Object,
						new ConstReference("__baseType").ToExpression(),
						new ConstReference(emitter.BaseType.AssemblyQualifiedName).ToExpression())));

			getObjectData.CodeBuilder.AddStatement(
				new ExpressionStatement(
					new MethodInvocationExpression(
						serializationInfo,
						SerializationInfoMethods.AddValue_Object,
						new ConstReference("__proxyGenerationOptions").ToExpression(),
						proxyGenerationOptionsField.ToExpression())));

			CustomizeGetObjectData(getObjectData.CodeBuilder, serializationInfo, streamingContext);

			getObjectData.CodeBuilder.AddStatement(new ReturnStatement());
		}
#endif

		protected virtual void CustomizeGetObjectData(
			AbstractCodeBuilder codebuilder, ArgumentReference serializationInfo,
			ArgumentReference streamingContext)
		{
		}

		protected bool VerifyIfBaseImplementsGetObjectData(Type baseType)
		{
			// If base type implements ISerializable, we have to make sure
			// the GetObjectData is marked as virtual
#if !SILVERLIGHT

			if (typeof(ISerializable).IsAssignableFrom(baseType))
			{
				MethodInfo getObjectDataMethod = baseType.GetMethod("GetObjectData",
					new Type[] { typeof(SerializationInfo), typeof(StreamingContext) });

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
					new Type[] { typeof(SerializationInfo), typeof(StreamingContext) },
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

		protected Reference GetTargetReference(IProxyMethod method, FieldReference[] mixinFields, Reference target)
		{
			MethodInfo methodInfo = method.Method;
			if (!method.HasTarget)
			{
				return GetMethodTargetReference(methodInfo);
			}

			if (method.Target is Mixin)
			{
				Type interfaceType = (method.Target as Mixin).MixinInterface;
				int mixinIndex = ProxyGenerationOptions.MixinData.GetMixinPosition(interfaceType);
				target = mixinFields[mixinIndex];
				return new AsTypeReference(target, methodInfo.DeclaringType);
			}
			return target;
		}

		protected virtual void AddInterfaceHierarchyMapping(Type @interface, object implementer, IDictionary<Type, object> mapping)
		{
			Debug.Assert(@interface.IsInterface, "@interface.IsInterface");

			if (!mapping.ContainsKey(@interface))
			{
				mapping.Add(@interface, implementer);
			}

			foreach (var baseInterface in @interface.GetInterfaces())
			{
				AddInterfaceHierarchyMapping(baseInterface, implementer, mapping);
			}
		}

		protected static class Proxy
		{
			/// <summary>
			/// Used to mark interface target as proxy instance. Used for special interfaces <see cref="ISerializable"/> and <see cref="IProxyTargetAccessor"/>.
			/// </summary>
			public static readonly object Instance = new object();

			/// <summary>
			/// Used to mark interface target as proxy target.
			/// </summary>
			public static readonly object Target = new object();
		}

		protected virtual void AddMappingForISerializable(IDictionary<Type, object> typeImplementerMapping)
		{
#if SILVERLIGHT
#warning What to do?
#else
			AddInterfaceHierarchyMapping(typeof(ISerializable), Proxy.Instance, typeImplementerMapping);
#endif
		}

		protected ProxyElementTarget CollectElementsToProxy(KeyValuePair<Type, object> mapping, InterfaceMapping map)
		{
			var elementsToProxy = new ProxyElementTarget(mapping, CanOnlyProxyVirtual(), map);
			elementsToProxy.Collect(ProxyGenerationOptions.Hook);
			return elementsToProxy;
		}

		protected IEnumerable<MethodInfo> GetMethods(IEnumerable<ProxyElementTarget> targets)
		{
			foreach (var target in targets)
			{
				foreach (var method in target.MethodInfos)
				{
					yield return method;
				}
			}
		}

		protected abstract MethodInfo GetMethodOnTarget(IProxyMethod proxyPethod);

		protected void ImplementMethod(ClassEmitter emitter, FieldReference interceptorsField, FieldReference[] mixinFields, MethodToGenerate method, Dictionary<MethodInfo, MethodEmitter> method2Emitter)
		{
			var methodInfo = method.Method;
			if (!method.Standalone || methodsToSkip.Contains(methodInfo))
			{
				// TODO: when interceptable == false, we still may need to override the method, like in case of mixin method, where we don't intercept the method, but forward right to the target
				// or interface method without target, where we need to provide a body to get a valid type.
				return;
			}

			NestedClassEmitter nestedClass = method2Invocation[methodInfo];

			MethodEmitter newProxiedMethod = CreateProxiedMethod(method,
			                                                     emitter,
			                                                     nestedClass,
			                                                     interceptorsField,
			                                                     GetTargetReference(method, mixinFields, GetProxyTargetReference()),
			                                                     ConstructorVersion,
																 GetMethodOnTarget(method));

			ReplicateNonInheritableAttributes(methodInfo, newProxiedMethod);

			method2Emitter[methodInfo] = newProxiedMethod;
		}

		protected void ImplementEvent(ClassEmitter emitter, FieldReference interceptorsField, FieldReference[] mixinFields, EventToGenerate @event)
		{
			@event.BuildEventEmitter(emitter);
			NestedClassEmitter add_nestedClass = method2Invocation[@event.AddMethod];
			string name;
			MethodAttributes add_attributes = ObtainMethodAttributes(@event.Adder, out name);

			MethodEmitter addEmitter = @event.Emitter.CreateAddMethod(name, add_attributes);

			MethodEmitter method = ImplementProxiedMethod(addEmitter,
			                                              @event.AddMethod, emitter,
			                                              add_nestedClass, interceptorsField,
			                                              GetTargetReference(@event.Adder, mixinFields, GetProxyTargetReference()),
			                                              ConstructorVersion,
			                                              GetMethodOnTarget(@event.Adder));
			if (@event.AddMethod.DeclaringType.IsInterface)
			{
				emitter.TypeBuilder.DefineMethodOverride(method.MethodBuilder, @event.AddMethod);
			}
			ReplicateNonInheritableAttributes(@event.AddMethod, addEmitter);


			NestedClassEmitter remove_nestedClass = method2Invocation[@event.RemoveMethod];

			MethodAttributes remove_attributes = ObtainMethodAttributes(@event.Remover, out name);

			MethodEmitter removeEmitter = @event.Emitter.CreateRemoveMethod(name, remove_attributes);

			MethodEmitter methodEmitter = ImplementProxiedMethod(removeEmitter,
			                                                     @event.RemoveMethod, emitter,
			                                                     remove_nestedClass, interceptorsField,
			                                                     GetTargetReference(@event.Remover, mixinFields,GetProxyTargetReference()),
			                                                     ConstructorVersion,
			                                                     GetMethodOnTarget(@event.Remover));
			if (@event.RemoveMethod.DeclaringType.IsInterface)
			{
				emitter.TypeBuilder.DefineMethodOverride(methodEmitter.MethodBuilder, @event.RemoveMethod);
			}
			ReplicateNonInheritableAttributes(@event.RemoveMethod, removeEmitter);
		}

		protected void ImplementProperty(ClassEmitter emitter, FieldReference interceptorsField, FieldReference[] mixinFields, PropertyToGenerate property)
		{
			property.BuildPropertyEmitter(emitter);
			if (property.CanRead)
			{
				NestedClassEmitter nestedClass = method2Invocation[property.GetMethod];

				string name;
				MethodAttributes attributes = ObtainMethodAttributes(property.Getter, out name);
				MethodEmitter getEmitter = property.Emitter.CreateGetMethod(name, attributes);

				MethodEmitter method = ImplementProxiedMethod(getEmitter,
				                                              property.GetMethod,
				                                              emitter,
				                                              nestedClass,
				                                              interceptorsField,
				                                              GetTargetReference(property.Getter, mixinFields,GetProxyTargetReference()),
				                                              ConstructorVersion,
				                                              GetMethodOnTarget(property.Getter));
				if (property.GetMethod.DeclaringType.IsInterface)
				{
					emitter.TypeBuilder.DefineMethodOverride(method.MethodBuilder, property.GetMethod);
				}
				ReplicateNonInheritableAttributes(property.GetMethod, getEmitter);
			}

			if (property.CanWrite)
			{
				NestedClassEmitter nestedClass = method2Invocation[property.SetMethod];

				string name;
				MethodAttributes attributes = ObtainMethodAttributes(property.Setter, out name);

				MethodEmitter setEmitter = property.Emitter.CreateSetMethod(name, attributes);

				MethodEmitter method = ImplementProxiedMethod(setEmitter,
				                                              property.SetMethod, emitter,
				                                              nestedClass, interceptorsField,
				                                              GetTargetReference(property.Setter, mixinFields, GetProxyTargetReference()),
				                                              ConstructorVersion,
				                                              GetMethodOnTarget(property.Setter));
				if (property.SetMethod.DeclaringType.IsInterface)
				{
					emitter.TypeBuilder.DefineMethodOverride(method.MethodBuilder, property.SetMethod);
				}
				ReplicateNonInheritableAttributes(property.SetMethod, setEmitter);
			}
		}
		
		protected class Mixin
		{
			public Type ClassUnderMixinInterface { get; set; }
			public Type MixinInterface { get; set; }

			public Mixin(Type classUnderMixinInterface,Type mixinInterface)
			{
				ClassUnderMixinInterface = classUnderMixinInterface;
				MixinInterface = mixinInterface;
			}
		}
	}
}
