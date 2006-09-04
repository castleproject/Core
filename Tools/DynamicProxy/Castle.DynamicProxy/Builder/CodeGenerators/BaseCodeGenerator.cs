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

namespace Castle.DynamicProxy.Builder.CodeGenerators
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Runtime.Serialization;
	using System.Threading;
	using Castle.DynamicProxy.Builder.CodeBuilder;
	using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;
#if DOTNET2
    using System.Runtime.CompilerServices;
#endif

	/// <summary>
	/// Summary description for BaseCodeGenerator.
	/// </summary>
	[CLSCompliant(false)]
	public abstract class BaseCodeGenerator
	{
		private ModuleScope _moduleScope;
		private GeneratorContext _context;

		private EasyType _typeBuilder;
		private FieldReference _interceptorField;
		private FieldReference _cacheField;
		private FieldReference _mixinField;
		private IList _generated = new ArrayList();
#if DOTNET2
        private ReaderWriterLock internalsToDynProxyLock = new ReaderWriterLock();
        private System.Collections.Generic.IDictionary<Assembly, bool> internalsToDynProxy = new System.Collections.Generic.Dictionary<Assembly, bool>();
#endif
		protected Type _baseType = typeof(Object);
		protected EasyMethod _method2Invocation;
		protected object[] _mixins = new object[0];

		/// <summary>
		/// Holds instance fields which points to delegates instantiated
		/// </summary>
		protected ArrayList _cachedFields = new ArrayList();

		/// <summary>
		/// MethodInfo => Callable delegate
		/// </summary>
		protected Hashtable _method2Delegate = new Hashtable();

		protected HybridDictionary _interface2mixinIndex = new HybridDictionary();

		protected BaseCodeGenerator(ModuleScope moduleScope)
			: this(moduleScope, new GeneratorContext())
		{
		}

		protected BaseCodeGenerator(ModuleScope moduleScope, GeneratorContext context)
		{
			_moduleScope = moduleScope;
			_context = context;
		}

		protected ModuleScope ModuleScope
		{
			get { return _moduleScope; }
		}

		protected GeneratorContext Context
		{
			get { return _context; }
		}

		protected EasyType MainTypeBuilder
		{
			get { return _typeBuilder; }
		}

		protected FieldReference InterceptorField
		{
			get { return _interceptorField; }
		}

		protected FieldReference MixinField
		{
			get { return _mixinField; }
		}

		protected FieldReference CacheField
		{
			get { return _cacheField; }
		}

		protected abstract Type InvocationType { get; }

		protected Type GetFromCache(Type baseClass, Type[] interfaces)
		{
			return ModuleScope[GenerateTypeName(baseClass, interfaces)] as Type;
		}

		protected void RegisterInCache(Type generatedType)
		{
			ModuleScope[generatedType.FullName] = generatedType;
		}

		protected FieldReference ObtainCallableFieldBuilderDelegate(EasyCallable builder)
		{
			foreach(CallableField field in _cachedFields)
			{
				if (field.Callable == builder)
				{
					return field.Field;
				}
			}

			return null;
		}

		protected void RegisterDelegateFieldToBeInitialized(
			MethodInfo method, FieldReference field,
			EasyCallable builder, MethodInfo callbackMethod)
		{
			int sourceArgIndex = CallableField.EmptyIndex;

			if (Context.HasMixins && _interface2mixinIndex.Contains(method.DeclaringType))
			{
				sourceArgIndex = ((int) _interface2mixinIndex[method.DeclaringType]);
			}

			_cachedFields.Add(new CallableField(field, builder, callbackMethod, sourceArgIndex));
		}

		protected virtual EasyType CreateTypeBuilder(String typeName, Type baseType, Type[] interfaces)
		{
			_baseType = baseType;
			_typeBuilder = new EasyType(ModuleScope, typeName, baseType, interfaces, true);

			return _typeBuilder;
		}

		protected virtual void GenerateFields()
		{
			_interceptorField = _typeBuilder.CreateField("__interceptor", Context.Interceptor);
			_cacheField = _typeBuilder.CreateField("__cache", typeof(HybridDictionary), false);
			_mixinField = _typeBuilder.CreateField("__mixin", typeof(object[]));
		}

		protected abstract String GenerateTypeName(Type type, Type[] interfaces);

		protected virtual void ImplementGetObjectData(Type[] interfaces)
		{
			// To prevent re-implementation of this interface.
			_generated.Add(typeof(ISerializable));

			Type[] get_type_args = new Type[] {typeof(String), typeof(bool), typeof(bool)};
			Type[] key_and_object = new Type[] {typeof(String), typeof(Object)};
			MethodInfo addValueMethod = typeof(SerializationInfo).GetMethod("AddValue", key_and_object);

			ArgumentReference arg1 = new ArgumentReference(typeof(SerializationInfo));
			ArgumentReference arg2 = new ArgumentReference(typeof(StreamingContext));
			EasyMethod getObjectData = MainTypeBuilder.CreateMethod("GetObjectData",
			                                                        new ReturnReferenceExpression(typeof(void)), arg1, arg2);

			LocalReference typeLocal = getObjectData.CodeBuilder.DeclareLocal(typeof(Type));

			getObjectData.CodeBuilder.AddStatement(new AssignStatement(
			                                       	typeLocal,
			                                       	new MethodInvocationExpression(null,
			                                       	                               typeof(Type).GetMethod("GetType",
			                                       	                                                      get_type_args),
			                                       	                               new FixedReference(
			                                       	                               	Context.ProxyObjectReference.
			                                       	                               		AssemblyQualifiedName).ToExpression(),
			                                       	                               new FixedReference(1).ToExpression(),
			                                       	                               new FixedReference(0).ToExpression())));

			getObjectData.CodeBuilder.AddStatement(new ExpressionStatement(
			                                       	new VirtualMethodInvocationExpression(
			                                       		arg1, typeof(SerializationInfo).GetMethod("SetType"),
			                                       		typeLocal.ToExpression())));

			getObjectData.CodeBuilder.AddStatement(new ExpressionStatement(
			                                       	new VirtualMethodInvocationExpression(arg1, addValueMethod,
			                                       	                                      new FixedReference("__interceptor").
			                                       	                                      	ToExpression(),
			                                       	                                      InterceptorField.ToExpression())));

			getObjectData.CodeBuilder.AddStatement(new ExpressionStatement(
			                                       	new VirtualMethodInvocationExpression(arg1, addValueMethod,
			                                       	                                      new FixedReference("__mixins").
			                                       	                                      	ToExpression(),
			                                       	                                      MixinField.ToExpression())));

			LocalReference interfacesLocal =
				getObjectData.CodeBuilder.DeclareLocal(typeof(String[]));

			getObjectData.CodeBuilder.AddStatement(
				new AssignStatement(interfacesLocal,
				                    new NewArrayExpression(interfaces.Length, typeof(String))));

			for(int i = 0; i < interfaces.Length; i++)
			{
				getObjectData.CodeBuilder.AddStatement(new AssignArrayStatement(
				                                       	interfacesLocal, i,
				                                       	new FixedReference(interfaces[i].AssemblyQualifiedName).ToExpression()));
			}

			getObjectData.CodeBuilder.AddStatement(new ExpressionStatement(
			                                       	new VirtualMethodInvocationExpression(arg1, addValueMethod,
			                                       	                                      new FixedReference("__interfaces").
			                                       	                                      	ToExpression(),
			                                       	                                      interfacesLocal.ToExpression())));

			getObjectData.CodeBuilder.AddStatement(new ExpressionStatement(
			                                       	new VirtualMethodInvocationExpression(arg1, addValueMethod,
			                                       	                                      new FixedReference("__baseType").
			                                       	                                      	ToExpression(),
			                                       	                                      new TypeTokenExpression(_baseType))));

			CustomizeGetObjectData(getObjectData.CodeBuilder, arg1, arg2);

			getObjectData.CodeBuilder.AddStatement(new ReturnStatement());
		}

		protected virtual void CustomizeGetObjectData(AbstractCodeBuilder codebuilder, ArgumentReference arg1,
		                                              ArgumentReference arg2)
		{
		}

		protected virtual void ImplementCacheInvocationCache()
		{
			MethodInfo get_ItemMethod = typeof(HybridDictionary).GetMethod("get_Item", new Type[] {typeof(object)});
			MethodInfo set_ItemMethod = typeof(HybridDictionary).GetMethod("Add", new Type[] {typeof(object), typeof(object)});

			Type[] args = new Type[] {typeof(ICallable), typeof(MethodInfo)};
			Type[] invocation_const_args = new Type[] {typeof(ICallable), typeof(object), typeof(MethodInfo), typeof(object)};

			ArgumentReference arg1 = new ArgumentReference(typeof(ICallable));
			ArgumentReference arg2 = new ArgumentReference(typeof(MethodInfo));
			ArgumentReference arg3 = new ArgumentReference(typeof(object));

			_method2Invocation = MainTypeBuilder.CreateMethod("_Method2Invocation",
			                                                  new ReturnReferenceExpression(Context.Invocation),
			                                                  MethodAttributes.Family | MethodAttributes.HideBySig, arg1, arg2,
			                                                  arg3);

			LocalReference invocation_local =
				_method2Invocation.CodeBuilder.DeclareLocal(Context.Invocation);

			LockBlockExpression block = new LockBlockExpression(SelfReference.Self);

			block.AddStatement(new AssignStatement(invocation_local,
			                                       new ConvertExpression(Context.Invocation,
			                                                             new VirtualMethodInvocationExpression(CacheField,
			                                                                                                   get_ItemMethod,
			                                                                                                   arg2.ToExpression()))));

			ConditionExpression cond1 = new ConditionExpression(OpCodes.Brfalse_S,
			                                                    invocation_local.ToExpression());

			cond1.AddTrueStatement(new AssignStatement(
			                       	invocation_local,
			                       	new NewInstanceExpression(InvocationType.GetConstructor(invocation_const_args),
			                       	                          arg1.ToExpression(), SelfReference.Self.ToExpression(),
			                       	                          arg2.ToExpression(), arg3.ToExpression())));

			cond1.AddTrueStatement(new ExpressionStatement(
			                       	new VirtualMethodInvocationExpression(CacheField,
			                       	                                      set_ItemMethod, arg2.ToExpression(),
			                       	                                      invocation_local.ToExpression())));

			block.AddStatement(new ExpressionStatement(cond1));

			_method2Invocation.CodeBuilder.AddStatement(new ExpressionStatement(block));
			_method2Invocation.CodeBuilder.AddStatement(new ReturnStatement(invocation_local));
		}

		protected virtual Type[] AddISerializable(Type[] interfaces)
		{
			if (Array.IndexOf(interfaces, typeof(ISerializable)) != -1)
			{
				return interfaces;
			}

			int len = interfaces.Length;
			Type[] newlist = new Type[len + 1];
			Array.Copy(interfaces, newlist, len);
			newlist[len] = typeof(ISerializable);
			return newlist;
		}

		protected virtual Type CreateType()
		{
			Type newType = MainTypeBuilder.BuildType();

			RegisterInCache(newType);

			return newType;
		}

		/// <summary>
		/// Generates one public constructor receiving 
		/// the <see cref="IInterceptor"/> instance and instantiating a hashtable
		/// </summary>
		/// <remarks>
		/// Should be overrided to provided specific semantics, if necessary
		/// </remarks>
		protected virtual EasyConstructor GenerateConstructor()
		{
			return null;
		}

		/// <summary>
		/// Common initializatio code for the default constructor
		/// </summary>
		/// <param name="codebuilder"></param>
		/// <param name="interceptorArg"></param>
		/// <param name="targetArgument"></param>
		/// <param name="mixinArray"></param>
		protected virtual void GenerateConstructorCode(ConstructorCodeBuilder codebuilder,
		                                               Reference interceptorArg, Reference targetArgument,
		                                               Reference mixinArray)
		{
			codebuilder.AddStatement(new AssignStatement(
			                         	InterceptorField, interceptorArg.ToExpression()));

			int mixins = Context.MixinsAsArray().Length;

			codebuilder.AddStatement(new AssignStatement(
			                         	MixinField, new NewArrayExpression(mixins, typeof(object))));

			if (Context.HasMixins)
			{
				for(int i = 0; i < mixins; i++)
				{
					codebuilder.AddStatement(new AssignArrayStatement(
					                         	MixinField, i,
					                         	new LoadRefArrayElementExpression(i, mixinArray)));
				}
			}

			codebuilder.AddStatement(new AssignStatement(
			                         	CacheField, new NewInstanceExpression(
			                         	            	typeof(HybridDictionary).GetConstructor(new Type[0]))));

			// Initialize the delegate fields
			foreach(CallableField field in _cachedFields)
			{
				field.WriteInitialization(codebuilder, targetArgument, mixinArray);
			}
		}

		protected ConstructorInfo ObtainAvailableConstructor(Type target)
		{
			return
				target.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="interfaces"></param>
		protected void GenerateInterfaceImplementation(Type[] interfaces)
		{
			foreach(Type inter in interfaces)
			{
				GenerateTypeImplementation(inter, false);
			}
		}

		/// <summary>
		/// Iterates over the interfaces and generate implementation 
		/// for each method in it.
		/// </summary>
		/// <param name="type">Type class</param>
		/// <param name="ignoreInterfaces">if true, we inspect the 
		/// type for implemented interfaces</param>
		protected void GenerateTypeImplementation(Type type, bool ignoreInterfaces)
		{
			if (_generated.Contains(type)) return;
			if (Context.ShouldSkip(type)) return;

			_generated.Add(type);

			if (!ignoreInterfaces)
			{
				Type[] baseInterfaces = type.FindInterfaces(new TypeFilter(NoFilterImpl), type);
				GenerateInterfaceImplementation(baseInterfaces);
			}

			EasyProperty[] properties = GenerateProperties(type);
			GenerateMethods(type, properties);
		}

		protected virtual EasyProperty[] GenerateProperties(Type inter)
		{
			PropertyInfo[] properties = inter.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			EasyProperty[] propertiesBuilder = new EasyProperty[properties.Length];

			for(int i = 0; i < properties.Length; i++)
			{
				propertiesBuilder[i] = CreateProperty(properties[i]);
			}

			return propertiesBuilder;
		}

		protected virtual void GenerateMethods(Type inter, EasyProperty[] properties)
		{
			MethodInfo[] methods = inter.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			foreach(MethodInfo method in methods)
			{
				if (method.IsFinal)
				{
					Context.AddMethodToGenerateNewSlot(method);
				}
				if (method.IsPrivate || !method.IsVirtual || method.IsFinal)
				{
					continue;
				}
				if (method.IsAssembly && !IsInternalToDynamicProxy(inter.Assembly))
				{
					continue;
				}
				if (method.DeclaringType.Equals(typeof(Object)) && !method.IsVirtual)
				{
					continue;
				}
				if (method.DeclaringType.Equals(typeof(Object)) && "Finalize".Equals(method.Name))
				{
					continue;
				}

				GenerateMethodImplementation(method, properties);
			}
		}

		/// <summary>
		/// Naive implementation, but valid for long namespaces
		/// Works by using only the last piece of the namespace
		/// </summary>
		protected String NormalizeNamespaceName(String nsName)
		{
			if (nsName == null || nsName == String.Empty) return String.Empty;

			String[] parts = nsName.Split('.', '+');

			return parts[parts.Length - 1];
		}

		/// <summary>
		/// Gets the name of a type, taking into consideration nested types.
		/// </summary>
		protected String GetTypeName(Type type)
		{
			System.Text.StringBuilder nameBuilder = new System.Text.StringBuilder();
			if (type.Namespace != null)
			{
				nameBuilder.Append(type.Namespace.Replace('.', '_'));
			}
			if (type.DeclaringType != null)
			{
				nameBuilder.Append(type.DeclaringType.Name).Append("_");
			}
#if DOTNET2
            if (type.IsGenericType)
            {
                Type[] args = type.GetGenericArguments();
                foreach (Type arg in args)
                {
                    string argName = GetTypeName(arg);
                    nameBuilder.Append(argName).Append("_");
                }
            }
#endif
			if (type.IsArray)
			{
				nameBuilder.Append("ArrayOf").Append(GetTypeName(type.GetElementType()));
			}
			else
			{
				nameBuilder.Append(type.Name);
			}
			return nameBuilder.ToString();
		}

		/// <summary>
		/// Generate property implementation
		/// </summary>
		/// <param name="property"></param>
		protected EasyProperty CreateProperty(PropertyInfo property)
		{
			return _typeBuilder.CreateProperty(property);
		}

		/// <summary>
		/// Generates implementation for each method.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="properties"></param>
		protected void GenerateMethodImplementation(MethodInfo method, EasyProperty[] properties)
		{
			if (Context.ShouldSkip(method)) return;

			ParameterInfo[] parametersInfo = method.GetParameters();

			Type[] parameters = new Type[parametersInfo.Length];

			for(int i = 0; i < parametersInfo.Length; i++)
			{
				parameters[i] = parametersInfo[i].ParameterType;
			}

			MethodAttributes atts = ObtainMethodAttributes(method);

			PreProcessMethod(method);

			EasyMethod easyMethod = null;

			bool isSetMethod = method.IsSpecialName && method.Name.StartsWith("set_");
			bool isGetMethod = method.IsSpecialName && method.Name.StartsWith("get_");

			if (!isSetMethod && !isGetMethod)
			{
				easyMethod = _typeBuilder.CreateMethod(method.Name,
				                                       atts, new ReturnReferenceExpression(method.ReturnType), parameters);
			}
			else
			{
				if (isSetMethod || isGetMethod)
				{
					foreach(EasyProperty property in properties)
					{
						if (property == null)
						{
							break;
						}

						if (!property.Name.Equals(method.Name.Substring(4)))
						{
							continue;
						}

						if (property.IndexParameters != null)
						{
							bool signatureMatches = true;
							int numOfIndexes = parametersInfo.Length;
							//A set method already has a value parameter, and everything after 
							//that is an indexer.
							if (isSetMethod)
								numOfIndexes--;
							if (numOfIndexes != property.IndexParameters.Length)
								continue;
							for(int i = 0; i < property.IndexParameters.Length; i++)
							{
								if (property.IndexParameters[i].ParameterType != parametersInfo[i].ParameterType)
								{
									signatureMatches = false;
									break;
								}
							}

							if (!signatureMatches) continue;
						}

						if (isSetMethod)
						{
							easyMethod = property.CreateSetMethod(atts, parameters);
							break;
						}
						else
						{
							easyMethod = property.CreateGetMethod(atts, parameters);
							break;
						}
					}
				}
			}

			easyMethod.DefineParameters(parametersInfo);

			WriteInterceptorInvocationMethod(method, easyMethod);

			PostProcessMethod(method);
		}

		private MethodAttributes ObtainMethodAttributes(MethodInfo method)
		{
			MethodAttributes atts;
			if (Context.ShouldCreateNewSlot(method))
				atts = MethodAttributes.NewSlot;
			else
				atts = MethodAttributes.Virtual;

			if (method.IsPublic)
			{
				atts |= MethodAttributes.Public;
			}
			if (IsInternalToDynamicProxy(method.DeclaringType.Assembly) && method.IsAssembly)
			{
				atts |= MethodAttributes.Assembly;
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

		protected virtual void PreProcessMethod(MethodInfo method)
		{
			MethodInfo callbackMethod = GenerateCallbackMethodIfNecessary(method, null);

			EasyCallable callable = MainTypeBuilder.CreateCallable(method.ReturnType, method.GetParameters());

			_method2Delegate[method] = callable;

			FieldReference field = MainTypeBuilder.CreateField(
				String.Format("_cached_{0}", callable.ID),
				callable.TypeBuilder);

			RegisterDelegateFieldToBeInitialized(method, field, callable, callbackMethod);
		}

		protected virtual MethodInfo GenerateCallbackMethodIfNecessary(MethodInfo method, Reference invocationTarget)
		{
			if (Context.HasMixins && _interface2mixinIndex.Contains(method.DeclaringType))
			{
				return method;
			}

			String name = String.Format("callback__{0}", method.Name);

			ParameterInfo[] parameters = method.GetParameters();

			ArgumentReference[] args = new ArgumentReference[parameters.Length];

			for(int i = 0; i < args.Length; i++)
			{
				args[i] = new ArgumentReference(parameters[i].ParameterType);
			}

			EasyMethod easymethod = MainTypeBuilder.CreateMethod(name,
			                                                     new ReturnReferenceExpression(method.ReturnType),
			                                                     MethodAttributes.HideBySig | MethodAttributes.Public, args);

			Expression[] exps = new Expression[parameters.Length];

			for(int i = 0; i < args.Length; i++)
			{
				exps[i] = args[i].ToExpression();
			}

			if (invocationTarget == null)
			{
				easymethod.CodeBuilder.AddStatement(
					new ReturnStatement(
						new MethodInvocationExpression(method, exps)));
			}
			else
			{
				easymethod.CodeBuilder.AddStatement(
					new ReturnStatement(
						new MethodInvocationExpression(invocationTarget, method, exps)));
			}

			return easymethod.MethodBuilder;
		}

		protected virtual void PostProcessMethod(MethodInfo method)
		{
		}

		/// <summary>
		/// Writes the method implementation. This 
		/// method generates the IL code for property get/set method and
		/// ordinary methods.
		/// </summary>
		/// <param name="method">The method to implement.</param>
		/// <param name="builder"><see cref="EasyMethod"/> being constructed.</param>
		protected virtual void WriteInterceptorInvocationMethod(MethodInfo method, EasyMethod builder)
		{
			ArgumentReference[] arguments = builder.Arguments;
			TypeReference[] dereferencedArguments = IndirectReference.WrapIfByRef(builder.Arguments);

			LocalReference local_inv = builder.CodeBuilder.DeclareLocal(Context.Invocation);

			EasyCallable callable = _method2Delegate[method] as EasyCallable;
			FieldReference fieldDelegate = ObtainCallableFieldBuilderDelegate(callable);

			builder.CodeBuilder.AddStatement(
				new AssignStatement(local_inv,
				                    new MethodInvocationExpression(_method2Invocation,
				                                                   fieldDelegate.ToExpression(),
				                                                   new MethodTokenExpression(GetCorrectMethod(method)),
				                                                   GetPseudoInvocationTarget(method))));

			LocalReference ret_local = builder.CodeBuilder.DeclareLocal(typeof(object));
			LocalReference args_local = builder.CodeBuilder.DeclareLocal(typeof(object[]));

			// Store arguments into an object array.
			builder.CodeBuilder.AddStatement(
				new AssignStatement(args_local,
				                    new ReferencesToObjectArrayExpression(dereferencedArguments)));

			// Invoke the interceptor.
			builder.CodeBuilder.AddStatement(
				new AssignStatement(ret_local,
				                    new VirtualMethodInvocationExpression(InterceptorField,
				                                                          Context.Interceptor.GetMethod("Intercept"),
				                                                          local_inv.ToExpression(),
				                                                          args_local.ToExpression())));

			// Load possibly modified ByRef arguments from the array.
			for(int i = 0; i < arguments.Length; i++)
			{
				if (arguments[i].Type.IsByRef)
				{
					builder.CodeBuilder.AddStatement(
						new AssignStatement(dereferencedArguments[i],
						                    new ConvertExpression(dereferencedArguments[i].Type,
						                                          new LoadRefArrayElementExpression(i, args_local))));
				}
			}

			if (builder.ReturnType == typeof(void))
			{
				builder.CodeBuilder.AddStatement(new ReturnStatement());
			}
			else
			{
				builder.CodeBuilder.AddStatement(new ReturnStatement(
				                                 	new ConvertExpression(builder.ReturnType, ret_local.ToExpression())));
			}
		}

		protected virtual Expression GetPseudoInvocationTarget(MethodInfo method)
		{
			return NullExpression.Instance;
		}

		protected virtual MethodInfo GetCorrectMethod(MethodInfo method)
		{
			return method;
		}

		protected Type[] InspectAndRegisterInterfaces(object[] mixins)
		{
			if (mixins == null) return new Type[0];

			Set interfaces = new Set();

			for(int i = 0; i < mixins.Length; ++i)
			{
				object mixin = mixins[i];

				Type[] mixinInterfaces = mixin.GetType().GetInterfaces();
				mixinInterfaces = Filter(mixinInterfaces);

				interfaces.AddArray(mixinInterfaces);

				// Later we gonna need to say which mixin
				// handle the method of a specific interface
				foreach(Type inter in mixinInterfaces)
				{
					_interface2mixinIndex.Add(inter, i);
				}
			}

			return (Type[]) interfaces.ToArray(typeof(Type));
		}

		protected Type[] Filter(Type[] mixinInterfaces)
		{
			ArrayList retType = new ArrayList();
			foreach(Type type in mixinInterfaces)
			{
				if (!Context.ShouldSkip(type))
					retType.Add(type);
			}

			return (Type[]) retType.ToArray(typeof(Type));
		}

		public static bool NoFilterImpl(Type type, object criteria)
		{
			return true;
		}

		protected bool IsInternalToDynamicProxy(Assembly asm)
		{
#if DOTNET2
			internalsToDynProxyLock.AcquireReaderLock(-1);

			if (internalsToDynProxy.ContainsKey(asm))
			{
				internalsToDynProxyLock.ReleaseReaderLock();
				
				return internalsToDynProxy[asm];
			}

			internalsToDynProxyLock.UpgradeToWriterLock(-1);

			try
			{
				if (internalsToDynProxy.ContainsKey(asm))
				{
					return internalsToDynProxy[asm];
				}

				InternalsVisibleToAttribute[] atts = (InternalsVisibleToAttribute[]) 
					asm.GetCustomAttributes(typeof(InternalsVisibleToAttribute), false);
				
				bool found = false;
				
				foreach(InternalsVisibleToAttribute internals in atts)
				{
					if (internals.AssemblyName.Contains(ModuleScope.ASSEMBLY_NAME))
					{
						found = true;
						break;
					}
				}
				
				internalsToDynProxy.Add(asm, found);

				return found;
			}
			finally
			{
				internalsToDynProxyLock.ReleaseWriterLock();
			}
#else
			return false;
#endif
			}
		}

		/// <summary>
		/// 
		/// </summary>
		internal class CallableField
		{
			private FieldReference _field;
			private EasyCallable _callable;
			private MethodInfo _callback;
			private int _sourceArgIndex;

			public CallableField(FieldReference field, EasyCallable callable,
			                     MethodInfo callback, int sourceArgIndex)
			{
				_field = field;
				_callable = callable;
				_callback = callback;
				_sourceArgIndex = sourceArgIndex;
			}

			public FieldReference Field
			{
				get { return _field; }
			}

			public EasyCallable Callable
			{
				get { return _callable; }
			}

			public int SourceArgIndex
			{
				get { return _sourceArgIndex; }
			}

			public void WriteInitialization(AbstractCodeBuilder codebuilder,
			                                Reference targetArgument, Reference mixinArray)
			{
				NewInstanceExpression newInst = null;

				if (SourceArgIndex == EmptyIndex)
				{
					newInst = new NewInstanceExpression(Callable,
					                                    targetArgument.ToExpression(), new MethodPointerExpression(_callback));
				}
				else
				{
					newInst = new NewInstanceExpression(Callable,
					                                    new LoadRefArrayElementExpression(SourceArgIndex, mixinArray),
					                                    new MethodPointerExpression(_callback));
				}

				codebuilder.AddStatement(new AssignStatement(
				                         	Field, newInst));
			}

			public static int EmptyIndex
			{
				get { return -1; }
			}
		}
	}