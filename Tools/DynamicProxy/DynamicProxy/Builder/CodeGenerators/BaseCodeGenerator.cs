// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

	using Castle.DynamicProxy.Builder.CodeBuilder;
	using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;

	/// <summary>
	/// Summary description for BaseCodeGenerator.
	/// </summary>
	public abstract class BaseCodeGenerator
	{
		private ModuleScope m_moduleScope;
		private GeneratorContext m_context;

		private EasyType m_typeBuilder;
		private FieldReference m_interceptorField;
		private FieldReference m_cacheField;
		private FieldReference m_mixinField;
		private IList m_generated = new ArrayList();

		protected Type m_baseType = typeof (Object);
		protected EasyMethod m_method2Invocation;
		protected object[] m_mixins = new object[0];

		/// <summary>
		/// Holds instance fields which points to delegates instantiated
		/// </summary>
		protected ArrayList m_cachedFields = new ArrayList();

		/// <summary>
		/// MethodInfo => Callable delegate
		/// </summary>
		protected Hashtable m_method2Delegate = new Hashtable();

		protected HybridDictionary m_interface2mixinIndex = new HybridDictionary();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="moduleScope"></param>
		protected BaseCodeGenerator(ModuleScope moduleScope) : this(moduleScope, new GeneratorContext())
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="moduleScope"></param>
		/// <param name="context"></param>
		protected BaseCodeGenerator(ModuleScope moduleScope, GeneratorContext context)
		{
			m_moduleScope = moduleScope;
			m_context = context;
		}

		protected ModuleScope ModuleScope
		{
			get { return m_moduleScope; }
		}

		protected GeneratorContext Context
		{
			get { return m_context; }
		}

		protected EasyType MainTypeBuilder
		{
			get { return m_typeBuilder; }
		}

		protected FieldReference InterceptorField
		{
			get { return m_interceptorField; }
		}

		protected FieldReference MixinField
		{
			get { return m_mixinField; }
		}

		protected FieldReference CacheField
		{
			get { return m_cacheField; }
		}

		protected abstract Type InvocationType
		{
			get;
		}

		protected Type GetFromCache(Type baseClass, Type[] interfaces)
		{
			return ModuleScope[GenerateTypeName(baseClass, interfaces)] as Type;
		}

		protected void RegisterInCache(Type generatedType)
		{
			ModuleScope[generatedType.Name] = generatedType;
		}

		protected FieldReference ObtainCallableFieldBuilderDelegate(EasyCallable builder)
		{
			foreach(CallableField field in m_cachedFields)
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

			if (Context.HasMixins && m_interface2mixinIndex.Contains(method.DeclaringType))
			{
				sourceArgIndex = ((int)m_interface2mixinIndex[ method.DeclaringType ]);
			}

			m_cachedFields.Add( new CallableField(field, builder, callbackMethod, sourceArgIndex) );
		}

		protected virtual EasyType CreateTypeBuilder(Type baseType, Type[] interfaces)
		{
			String typeName = GenerateTypeName(baseType, interfaces);

			m_baseType = baseType;
			m_typeBuilder = new EasyType(ModuleScope, typeName, baseType, interfaces, true);

			return m_typeBuilder;
		}

		protected virtual void GenerateFields()
		{
			m_interceptorField = m_typeBuilder.CreateField("__interceptor", typeof (IInterceptor));
			m_cacheField = m_typeBuilder.CreateField("__cache", typeof (HybridDictionary), false);
			m_mixinField = m_typeBuilder.CreateField("__mixin", typeof (object[]));
		}

		protected abstract String GenerateTypeName(Type type, Type[] interfaces);

		protected virtual void ImplementGetObjectData( Type[] interfaces )
		{
			// To prevent re-implementation of this interface.
			m_generated.Add(typeof (ISerializable));

			Type[] get_type_args = new Type[] {typeof (String), typeof (bool), typeof (bool)};
			Type[] key_and_object = new Type[] {typeof (String), typeof (Object)};
			MethodInfo addValueMethod = typeof (SerializationInfo).GetMethod("AddValue", key_and_object);

			ArgumentReference arg1 = new ArgumentReference( typeof(SerializationInfo) );
			ArgumentReference arg2 = new ArgumentReference( typeof(StreamingContext) );
			EasyMethod getObjectData = MainTypeBuilder.CreateMethod( "GetObjectData", 
				new ReturnReferenceExpression(typeof(void)), arg1, arg2 );

			LocalReference typeLocal = getObjectData.CodeBuilder.DeclareLocal( typeof(Type) );
			
			getObjectData.CodeBuilder.AddStatement( new AssignStatement(
				typeLocal, new MethodInvocationExpression( null, typeof(Type).GetMethod("GetType", get_type_args), 
				new FixedReference( Context.ProxyObjectReference.AssemblyQualifiedName ).ToExpression(), 
				new FixedReference(1).ToExpression(),
				new FixedReference(0).ToExpression() ) ) );

			getObjectData.CodeBuilder.AddStatement( new ExpressionStatement(
				new VirtualMethodInvocationExpression( 
				arg1, typeof (SerializationInfo).GetMethod("SetType"),
				typeLocal.ToExpression() ) ) );

			getObjectData.CodeBuilder.AddStatement( new ExpressionStatement(
				new VirtualMethodInvocationExpression(arg1, addValueMethod, 
				new FixedReference("__interceptor").ToExpression(), 
				InterceptorField.ToExpression() ) ) );

			getObjectData.CodeBuilder.AddStatement( new ExpressionStatement(
				new VirtualMethodInvocationExpression(arg1, addValueMethod, 
				new FixedReference("__mixins").ToExpression(), 
				MixinField.ToExpression() ) ) );

			LocalReference interfacesLocal = 
				getObjectData.CodeBuilder.DeclareLocal( typeof(Type[]) );

			getObjectData.CodeBuilder.AddStatement(
				new AssignStatement(interfacesLocal, 
				new NewArrayExpression( interfaces.Length, typeof(Type) )));

			for(int i=0; i < interfaces.Length; i++)
			{
				getObjectData.CodeBuilder.AddStatement( new AssignArrayStatement(
					interfacesLocal, i, new TypeTokenExpression( interfaces[i] )) );
			}

			getObjectData.CodeBuilder.AddStatement( new ExpressionStatement(
				new VirtualMethodInvocationExpression(arg1, addValueMethod, 
				new FixedReference("__interfaces").ToExpression(), 
				interfacesLocal.ToExpression() ) ) );

			getObjectData.CodeBuilder.AddStatement( new ExpressionStatement(
				new VirtualMethodInvocationExpression(arg1, addValueMethod, 
				new FixedReference("__baseType").ToExpression(), 
				new TypeTokenExpression( m_baseType ) ) ) );

			CustomizeGetObjectData(getObjectData.CodeBuilder, arg1, arg2);

			getObjectData.CodeBuilder.AddStatement( new ReturnStatement() );
		}

		protected virtual void CustomizeGetObjectData(AbstractCodeBuilder codebuilder, ArgumentReference arg1, ArgumentReference arg2)
		{
		}

		protected virtual void ImplementCacheInvocationCache()
		{
			MethodInfo get_ItemMethod = typeof(HybridDictionary).GetMethod("get_Item", new Type[] { typeof(object) });
			MethodInfo set_ItemMethod = typeof(HybridDictionary).GetMethod("Add", new Type[] { typeof(object), typeof(object) });

			Type[] args = new Type[] {typeof (ICallable), typeof (MethodInfo) };
			Type[] invocation_const_args = new Type[] {typeof (ICallable), typeof(object), typeof (MethodInfo)};

			ArgumentReference arg1 = new ArgumentReference( typeof(ICallable) );
			ArgumentReference arg2 = new ArgumentReference( typeof(MethodInfo) );
			m_method2Invocation = MainTypeBuilder.CreateMethod("_Method2Invocation", 
				new ReturnReferenceExpression(typeof(IInvocation)), arg1, arg2);

			LocalReference invocation_local = 
				m_method2Invocation.CodeBuilder.DeclareLocal(typeof (IInvocation));

			m_method2Invocation.CodeBuilder.AddStatement( new AssignStatement( invocation_local,
				new ConvertExpression( typeof(IInvocation), 
					new VirtualMethodInvocationExpression(CacheField, 
					get_ItemMethod, 
					arg2.ToExpression() ) ) ) );

			ConditionExpression cond1 = new ConditionExpression( OpCodes.Brfalse_S, 
				invocation_local.ToExpression() );

			cond1.AddTrueStatement( new AssignStatement( 
				invocation_local, 
				new NewInstanceExpression( InvocationType.GetConstructor(invocation_const_args), 
				arg1.ToExpression(), SelfReference.Self.ToExpression(), arg2.ToExpression() ) ) );
			
			cond1.AddTrueStatement( new ExpressionStatement( 
				new VirtualMethodInvocationExpression( CacheField, 
				set_ItemMethod, arg2.ToExpression(), invocation_local.ToExpression())) );

			m_method2Invocation.CodeBuilder.AddStatement( new ExpressionStatement(cond1) );
			m_method2Invocation.CodeBuilder.AddStatement( new ReturnStatement(invocation_local) );
		}

		protected virtual Type[] AddISerializable(Type[] interfaces)
		{
			if (Array.IndexOf(interfaces, typeof (ISerializable)) != -1)
			{
				return interfaces;
			}

			int len = interfaces.Length;
			Type[] newlist = new Type[len + 1];
			Array.Copy(interfaces, newlist, len);
			newlist[len] = typeof (ISerializable);
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
		protected abstract EasyConstructor GenerateConstructor();

		/// <summary>
		/// Common initializatio code for the default constructor
		/// </summary>
		/// <param name="codebuilder"></param>
		/// <param name="interceptorArg"></param>
		/// <param name="targetArgument"></param>
		/// <param name="mixinArray"></param>
		protected virtual void GenerateConstructorCode(ConstructorCodeBuilder codebuilder, 
			Reference interceptorArg, Reference targetArgument, Reference mixinArray)
		{
			codebuilder.AddStatement( new AssignStatement(
				InterceptorField, interceptorArg.ToExpression()) );

			codebuilder.AddStatement( new AssignStatement(
				CacheField, new NewInstanceExpression(
					typeof(HybridDictionary).GetConstructor( new Type[0] )) ) );

			int mixins = Context.MixinsAsArray().Length;

			codebuilder.AddStatement( new AssignStatement(
				MixinField, new NewArrayExpression(mixins, typeof(object))) );

			if (Context.HasMixins)
			{
				for(int i=0; i < mixins; i++)
				{
					codebuilder.AddStatement( new AssignArrayStatement(
						MixinField, i, 
						new LoadRefArrayElementExpression(i, mixinArray)) );
				}
			}

			// Initialize the delegate fields
			foreach(CallableField field in m_cachedFields)
			{
				field.WriteInitialization(codebuilder, targetArgument, mixinArray);
			}

			codebuilder.AddStatement( new ReturnStatement() );
		}

		protected ConstructorInfo ObtainAvailableConstructor(Type target)
		{
			return target.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="interfaces"></param>
		protected void GenerateInterfaceImplementation(Type[] interfaces)
		{
			foreach (Type inter in interfaces)
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
			if (m_generated.Contains(type))	return;
			if (Context.ShouldSkip(type)) return;

			m_generated.Add(type);

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
			PropertyInfo[] properties = inter.GetProperties();
			
			EasyProperty[] propertiesBuilder = new EasyProperty[properties.Length];

			for (int i = 0; i < properties.Length; i++)
			{
				propertiesBuilder[i] = CreateProperty(properties[i]);
			}

			return propertiesBuilder;
		}

		protected virtual void GenerateMethods(Type inter, EasyProperty[] properties)
		{
			MethodInfo[] methods = inter.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			foreach (MethodInfo method in methods)
			{
				if (method.IsPrivate || !method.IsVirtual || method.IsFinal)
				{
					continue;
				}
				if (method.DeclaringType.Equals(typeof (Object)) && !method.IsVirtual)
				{
					continue;
				}
				if (method.DeclaringType.Equals(typeof (Object)) && "Finalize".Equals(method.Name))
				{
					continue;
				}
				GenerateMethodImplementation(method, properties);
			}
		}

		/// <summary>
		/// Generate property implementation
		/// </summary>
		/// <param name="property"></param>
		protected EasyProperty CreateProperty(PropertyInfo property)
		{
			return m_typeBuilder.CreateProperty(property.Name, property.PropertyType);
		}

		/// <summary>
		/// Generates implementation for each method.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="properties"></param>
		protected void GenerateMethodImplementation(MethodInfo method, EasyProperty[] properties)
		{
			if (Context.ShouldSkip(method)) return;

			ParameterInfo[] parameterInfo = method.GetParameters();

			Type[] parameters = new Type[parameterInfo.Length];

			for (int i = 0; i < parameterInfo.Length; i++)
			{
				parameters[i] = parameterInfo[i].ParameterType;
			}

			MethodAttributes atts = ObtainMethodAttributes(method);

			PreProcessMethod(method);

			EasyMethod easyMethod = null;

			bool isSetMethod = method.Name.StartsWith("set_");
			bool isGetMethod = method.Name.StartsWith("get_");

			if (!isSetMethod && !isGetMethod)
			{
				easyMethod = m_typeBuilder.CreateMethod(method.Name, 
					atts, new ReturnReferenceExpression(method.ReturnType), parameters);
			}
			else
			{
				if (isSetMethod || isGetMethod)
				{
					foreach (EasyProperty property in properties)
					{
						if (property == null)
						{
							break;
						}
	
						if (!property.Name.Equals(method.Name.Substring(4)))
						{
							continue;
						}
	
						if (isSetMethod)
						{
							easyMethod = property.CreateSetMethod( atts, parameters );
							break;
						}
						else
						{
							easyMethod = property.CreateGetMethod( atts, parameters );
							break;
						}
					}
				}
			}

			WriteInterceptorInvocationMethod(method, easyMethod);

			PostProcessMethod(method);
		}

		private MethodAttributes ObtainMethodAttributes(MethodInfo method)
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

		protected virtual void PreProcessMethod(MethodInfo method)
		{
			MethodInfo callbackMethod = GenerateCallbackMethodIfNecessary(method);

			EasyCallable callable = MainTypeBuilder.CreateCallable( method.ReturnType, method.GetParameters() );

			m_method2Delegate[method] = callable;

			FieldReference field = MainTypeBuilder.CreateField( 
				String.Format("_cached_{0}", callable.ID), 
				callable.TypeBuilder );

			RegisterDelegateFieldToBeInitialized(method, field, callable, callbackMethod);
		}

		protected virtual MethodInfo GenerateCallbackMethodIfNecessary(MethodInfo method)
		{
			return method;
		}

		protected virtual void PostProcessMethod(MethodInfo method)
		{
		}

		/// <summary>
		/// Writes the method implementation. This 
		/// method generates the IL code for property get/set method and
		/// ordinary methods.
		/// </summary>
		/// <param name="builder"><see cref="EasyMethod"/> being constructed.</param>
		protected virtual void WriteInterceptorInvocationMethod(MethodInfo method, EasyMethod builder)
		{
			ArgumentReference[] arguments = builder.Arguments;

			LocalReference local_inv = builder.CodeBuilder.DeclareLocal(typeof (IInvocation));   

			EasyCallable callable = m_method2Delegate[method] as EasyCallable;
			FieldReference fieldDelegate = ObtainCallableFieldBuilderDelegate( callable );

			builder.CodeBuilder.AddStatement( 
				new AssignStatement( local_inv, 
					new VirtualMethodInvocationExpression(m_method2Invocation, 
						fieldDelegate.ToExpression(),
						new MethodTokenExpression(method)) ) );

			LocalReference ret_local = builder.CodeBuilder.DeclareLocal( typeof(object) );

			builder.CodeBuilder.AddStatement( 
				new AssignStatement( ret_local,
					new VirtualMethodInvocationExpression( InterceptorField,
						typeof (IInterceptor).GetMethod("Intercept"), 
						local_inv.ToExpression(),
						new ReferencesToObjectArrayExpression(arguments))));
			
			if (builder.ReturnType == typeof (void))
			{
				builder.CodeBuilder.AddStatement( new ReturnStatement() );
			}
			else
			{
				builder.CodeBuilder.AddStatement( new ReturnStatement(
					new ConvertExpression(builder.ReturnType, ret_local.ToExpression())) );
			}
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
				
				interfaces.AddArray( mixinInterfaces );
				
				// Later we gonna need to say which mixin
				// handle the method of a specific interface
				foreach(Type inter in mixinInterfaces)
				{
					m_interface2mixinIndex.Add(inter, i);
				}
			}

			return (Type[]) interfaces.ToArray( typeof(Type) );
		}

		protected static Type[] Filter(Type[] mixinInterfaces)
		{
			if (Array.IndexOf(mixinInterfaces, typeof(ISerializable)) != -1)
			{
				// TODO: Implement this
			}
			
			return mixinInterfaces;
		}

		public static bool NoFilterImpl(Type type, object criteria)
		{
			return true;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	internal class CallableField
	{
		private FieldReference m_field; 
		private EasyCallable m_callable; 
		private MethodInfo m_callback;
		private int m_sourceArgIndex;

		public CallableField(FieldReference field, EasyCallable callable, 
			MethodInfo callback, int sourceArgIndex)
		{
			m_field = field;
			m_callable = callable;
			m_callback = callback;
			m_sourceArgIndex = sourceArgIndex;
		}

		public FieldReference Field
		{
			get { return m_field; }
		}

		public EasyCallable Callable
		{
			get { return m_callable; }
		}

		public int SourceArgIndex
		{
			get { return m_sourceArgIndex; }
		}

		public void WriteInitialization(AbstractCodeBuilder codebuilder, 
			Reference targetArgument, Reference mixinArray)
		{
			NewInstanceExpression newInst = null;

			if (SourceArgIndex == EmptyIndex)
			{
				newInst = new NewInstanceExpression(Callable, 
					targetArgument.ToExpression(), new MethodPointerExpression(m_callback) );
			}
			else
			{
				newInst = new NewInstanceExpression(Callable, 
					new LoadRefArrayElementExpression(SourceArgIndex, mixinArray), 
					new MethodPointerExpression(m_callback) );
			}

			codebuilder.AddStatement( new AssignStatement(
				Field, newInst) );
		}

		public static int EmptyIndex
		{
			get { return -1; }
		}
	}
}
