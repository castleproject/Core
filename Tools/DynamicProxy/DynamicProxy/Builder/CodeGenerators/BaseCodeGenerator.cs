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

	/// <summary>
	/// Summary description for BaseCodeGenerator.
	/// </summary>
	public abstract class BaseCodeGenerator
	{
		private ModuleScope m_moduleScope;
		private GeneratorContext m_context;

		private TypeBuilder m_typeBuilder;
		private FieldBuilder m_interceptorField;
		private FieldBuilder m_cacheField;
		private IList m_generated = new ArrayList();

		protected Type m_baseType = typeof (Object);
		protected MethodBuilder m_method2Invocation;
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

		protected TypeBuilder MainTypeBuilder
		{
			get { return m_typeBuilder; }
		}

		protected FieldBuilder InterceptorFieldBuilder
		{
			get { return m_interceptorField; }
		}

		protected FieldBuilder CacheFieldBuilder
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

		protected FieldBuilder ObtainCachedFieldBuilderDelegate(CallableDelegateBuilder builder)
		{
			foreach(CachedField field in m_cachedFields)
			{
				if (field.Callable == builder)
				{
					return field.Field;
				}
			}

			return null;
		}

		protected void RegisterCacheFieldToBeInitialized(
			MethodInfo method, FieldBuilder field, 
			CallableDelegateBuilder builder, MethodInfo callbackMethod)
		{
			int sourceArgIndex = CachedField.EmptyIndex;

			if (Context.HasMixins && m_interface2mixinIndex.Contains(method.DeclaringType))
			{
				sourceArgIndex = ((int)m_interface2mixinIndex[ method.DeclaringType ]);
			}

			m_cachedFields.Add( new CachedField(field, builder, callbackMethod, sourceArgIndex) );
		}

		protected abstract Type[] GetConstructorSignature();

		protected virtual TypeBuilder CreateTypeBuilder(Type baseType, Type[] interfaces)
		{
			String typeName = GenerateTypeName(baseType, interfaces);

			ModuleBuilder moduleBuilder = ModuleScope.ObtainDynamicModule();

			TypeAttributes flags = TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable;

			m_baseType = baseType;
			m_typeBuilder = moduleBuilder.DefineType(
				typeName, flags, baseType, interfaces);

			return m_typeBuilder;
		}

		protected virtual void GenerateFields()
		{
			m_interceptorField = GenerateField("__interceptor", typeof (IInterceptor));
			m_cacheField = GenerateField("__cache", typeof (HybridDictionary), 
				FieldAttributes.Family|FieldAttributes.NotSerialized);
		}

		protected abstract String GenerateTypeName(Type type, Type[] interfaces);

		protected virtual void ImplementGetObjectData()
		{
			// To prevent re-implementation of this interface.
			m_generated.Add(typeof (ISerializable));

			Type[] args = new Type[] {typeof (SerializationInfo), typeof (StreamingContext)};
			Type[] get_type_args = new Type[] {typeof (String), typeof (bool), typeof (bool)};
			Type[] key_and_object = new Type[] {typeof (String), typeof (Object)};

			MethodAttributes attrs = MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public;
			MethodBuilder builder = MainTypeBuilder.DefineMethod("GetObjectData", attrs, typeof (void), args);

			ILGenerator generator = builder.GetILGenerator();
			LocalBuilder type_local = generator.DeclareLocal(typeof (Type));

			// type name declared
			generator.Emit(OpCodes.Ldstr, "Castle.DynamicProxy.Serialization.ProxyObjectReference, Castle.DynamicProxy");
			generator.Emit(OpCodes.Ldc_I4_1); // true
			generator.Emit(OpCodes.Ldc_I4_0); // false
			generator.Emit(OpCodes.Call, typeof (Type).GetMethod("GetType", get_type_args));

			// We set the class responsible for
			// serialize/desserialize the proxy
			generator.Emit(OpCodes.Stloc_0);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldloc_0);
			generator.Emit(OpCodes.Callvirt, typeof (SerializationInfo).GetMethod("SetType"));

			// We need to save a few things to allow
			// the proxy to be reconstructed later.
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldstr, "interceptor");
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldfld, InterceptorFieldBuilder);
			generator.Emit(OpCodes.Callvirt, typeof (SerializationInfo).GetMethod("AddValue", key_and_object));

			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldstr, "baseType");
			generator.Emit(OpCodes.Ldtoken, this.m_baseType);
			generator.Emit(OpCodes.Call, typeof (Type).GetMethod("GetTypeFromHandle"));
			generator.Emit(OpCodes.Callvirt, typeof (SerializationInfo).GetMethod("AddValue", key_and_object));

			generator.Emit(OpCodes.Ret);
		}

		protected virtual void ImplementCacheInvocationCache()
		{
			Type[] args = new Type[] {typeof (ICallable), typeof (MethodInfo) };
			Type[] invocation_const_args = new Type[] {typeof (ICallable), typeof(object), typeof (MethodInfo)};

			MethodAttributes attrs = MethodAttributes.Private;
			m_method2Invocation = MainTypeBuilder.DefineMethod("_Method2Invocation", 
				attrs, typeof (IInvocation), args);

			ILGenerator gen = m_method2Invocation.GetILGenerator();
			gen.DeclareLocal(typeof (IInvocation));

			Label endMethodBranch = gen.DefineLabel();
			Label setInCacheBranch = gen.DefineLabel();
			Label retValueBranch = gen.DefineLabel();

			// Try to obtain the invocation stored in the Hashtable
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldfld, CacheFieldBuilder);
			gen.Emit(OpCodes.Ldarg_2);
			gen.Emit(OpCodes.Callvirt, typeof(HybridDictionary).GetMethod("get_Item", new Type[] { typeof(object) } ));
			gen.Emit(OpCodes.Castclass, typeof(IInvocation));
			gen.Emit(OpCodes.Stloc_0);

			// If not null, goto to return
			gen.Emit(OpCodes.Ldloc_0);
			gen.Emit(OpCodes.Brtrue_S, endMethodBranch);

			// Create the invocation target
			gen.Emit(OpCodes.Ldarg_1); // Callable
			gen.Emit(OpCodes.Ldarg_0); // this
//			gen.Emit(OpCodes.Ldarg_0); // this
			gen.Emit(OpCodes.Ldarg_2); // MethodInfo
			gen.Emit(OpCodes.Newobj, InvocationType.GetConstructor(invocation_const_args) );
			gen.Emit(OpCodes.Stloc_0);
			gen.Emit(OpCodes.Br_S, setInCacheBranch);

			gen.MarkLabel(setInCacheBranch);
			gen.Emit(OpCodes.Ldarg_0); // this
			gen.Emit(OpCodes.Ldfld, CacheFieldBuilder); 
			gen.Emit(OpCodes.Ldarg_2); // MethodInfo as key
			gen.Emit(OpCodes.Ldloc_0); // IInvocation instance as value
			gen.Emit(OpCodes.Callvirt, typeof(HybridDictionary).GetMethod("Add", new Type[] { typeof(object), typeof(object) } ));
			
			gen.MarkLabel(endMethodBranch);
			gen.Emit(OpCodes.Br_S, retValueBranch);

			gen.MarkLabel(retValueBranch);
			gen.Emit(OpCodes.Ldloc_0); 
			gen.Emit(OpCodes.Ret);
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
			Type newType = MainTypeBuilder.CreateType();

			foreach(CallableDelegateBuilder builder in m_method2Delegate.Values)
			{
				builder.CreateType();
			}

			RegisterInCache(newType);

			return newType;
		}

		/// <summary>
		/// Generates a protected field
		/// </summary>
		/// <param name="name">Field's name</param>
		/// <param name="type">Field's type</param>
		/// <returns></returns>
		protected FieldBuilder GenerateField(String name, Type type)
		{
			return GenerateField(name, type, FieldAttributes.Family);
		}

		protected FieldBuilder GenerateField(String name, Type type, FieldAttributes attrs)
		{
			return m_typeBuilder.DefineField(name, type, attrs);
		}

		/// <summary>
		/// Generates one public constructor receiving 
		/// the <see cref="IInterceptor"/> instance and instantiating a hashtable
		/// </summary>
		protected abstract void GenerateConstructor();

		protected virtual void GenerateConstructorCode(ILGenerator ilGenerator, OpCode defaultArg, OpCode mixinArrayArgument)
		{
			// Stores the interceptor in the field
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Ldarg_1);
			ilGenerator.Emit(OpCodes.Stfld, InterceptorFieldBuilder);

			// Instantiates the hashtable
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Newobj, typeof(HybridDictionary).GetConstructor( new Type[0] ));
			ilGenerator.Emit(OpCodes.Stfld, CacheFieldBuilder);

			// Initialize the delegate fields
			foreach(CachedField field in m_cachedFields)
			{
				field.WriteInitialization(ilGenerator, defaultArg, mixinArrayArgument);
			}
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
				if (!Context.ShouldSkip(inter))
				{
					GenerateTypeImplementation(inter, false);
				}
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
			if (m_generated.Contains(type))
			{
				return;
			}
			else
			{
				m_generated.Add(type);
			}

			if (!ignoreInterfaces)
			{
				Type[] baseInterfaces = type.FindInterfaces(new TypeFilter(NoFilterImpl), type);
				GenerateInterfaceImplementation(baseInterfaces);
			}

			PropertyBuilder[] propertiesBuilder = GenerateProperties(type);
			GenerateMethods(type, propertiesBuilder);
		}

		protected virtual PropertyBuilder[] GenerateProperties(Type inter)
		{
			PropertyInfo[] properties = inter.GetProperties();
			PropertyBuilder[] propertiesBuilder = new PropertyBuilder[properties.Length];

			for (int i = 0; i < properties.Length; i++)
			{
				propertiesBuilder[i] = GeneratePropertyImplementation(properties[i]);
			}

			return propertiesBuilder;
		}

		protected virtual void GenerateMethods(Type inter, PropertyBuilder[] propertiesBuilder)
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
				GenerateMethodImplementation(method, propertiesBuilder);
			}
		}

		/// <summary>
		/// Generate property implementation
		/// </summary>
		/// <param name="property"></param>
		protected PropertyBuilder GeneratePropertyImplementation(PropertyInfo property)
		{
			return m_typeBuilder.DefineProperty(
				property.Name, property.Attributes, property.PropertyType, null);
		}

		/// <summary>
		/// Generates implementation for each method.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="properties"></param>
		protected void GenerateMethodImplementation(
			MethodInfo method, PropertyBuilder[] properties)
		{
			ParameterInfo[] parameterInfo = method.GetParameters();

			Type[] parameters = new Type[parameterInfo.Length];

			for (int i = 0; i < parameterInfo.Length; i++)
			{
				parameters[i] = parameterInfo[i].ParameterType;
			}

			MethodAttributes atts = MethodAttributes.Virtual;

			if (method.IsPublic)
			{
				atts |= MethodAttributes.Public;
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

			MethodBuilder methodBuilder =
				m_typeBuilder.DefineMethod(method.Name, atts, CallingConventions.Standard,
				                           method.ReturnType, parameters);

			if (method.Name.StartsWith("set_") || method.Name.StartsWith("get_"))
			{
				foreach (PropertyBuilder property in properties)
				{
					if (property == null)
					{
						break;
					}

					if (!property.Name.Equals(method.Name.Substring(4)))
					{
						continue;
					}

					if (methodBuilder.Name.StartsWith("set_"))
					{
						property.SetSetMethod(methodBuilder);
						break;
					}
					else
					{
						property.SetGetMethod(methodBuilder);
						break;
					}
				}
			}

			PreProcessMethod(method);
			
			WriteILForMethod(method, methodBuilder, parameters);
			
			PostProcessMethod(method);
		}

		protected virtual void PreProcessMethod(MethodInfo method)
		{
			MethodInfo callbackMethod = GenerateMethodActualInvoke(method);

			CallableDelegateBuilder delegateBuilder = CallableDelegateBuilder.BuildForMethod( 
				MainTypeBuilder, method, ModuleScope );

			m_method2Delegate[method] = delegateBuilder;

			FieldBuilder field = GenerateField( 
				String.Format("_cached_{0}", delegateBuilder.ID), 
				typeof(ICallable) );

			RegisterCacheFieldToBeInitialized(method, field, delegateBuilder, callbackMethod);
		}

		protected virtual MethodInfo GenerateMethodActualInvoke(MethodInfo method)
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
		/// <param name="builder"><see cref="MethodBuilder"/> being constructed.</param>
		/// <param name="parameters"></param>
		protected virtual void WriteILForMethod(MethodInfo method, MethodBuilder builder, Type[] parameters)
		{
			ILGenerator gen = builder.GetILGenerator();

			gen.DeclareLocal(typeof (MethodBase));    // 0
			gen.DeclareLocal(typeof (object[]));      // 1
			gen.DeclareLocal(typeof (IInvocation));   // 2

			if (builder.ReturnType != typeof (void))
			{
				gen.DeclareLocal(builder.ReturnType); // 3
			}

			CallableDelegateBuilder delegateType = m_method2Delegate[method] as CallableDelegateBuilder;
			FieldBuilder fieldDelegate = ObtainCachedFieldBuilderDelegate( delegateType );

			// Obtains the MethodBase from ldtoken method
			gen.Emit(OpCodes.Ldtoken, method);
			gen.Emit(OpCodes.Call, typeof (MethodBase).GetMethod("GetMethodFromHandle"));
			gen.Emit(OpCodes.Stloc_0);

			// Invokes the Method2Invocation to obtain a proper IInvocation instance
			gen.Emit(OpCodes.Ldarg_0);

			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldfld, fieldDelegate);
			gen.Emit(OpCodes.Ldloc_0);
			gen.Emit(OpCodes.Castclass, typeof (MethodInfo)); // Cast MethodBase to MethodInfo
			gen.Emit(OpCodes.Call, m_method2Invocation);
			gen.Emit(OpCodes.Stloc_2);

			gen.Emit(OpCodes.Ldc_I4, parameters.Length); // push the array size
			gen.Emit(OpCodes.Newarr, typeof (object)); // creates an array of objects
			gen.Emit(OpCodes.Stloc_1); // store the array into local field
			
			// Here we set all the arguments in the array
			for( int i=0; i < parameters.Length; i++ )
			{
				gen.Emit(OpCodes.Ldloc_1); // load the array
				gen.Emit(OpCodes.Ldc_I4, i); // set the index
				gen.Emit(OpCodes.Ldarg, i + 1); // set the value
				// box if necessary
				if (parameters[i].IsValueType)
				{
					gen.Emit(OpCodes.Box, parameters[i].UnderlyingSystemType);
				}
				gen.Emit(OpCodes.Stelem_Ref); // set the value
			}

			gen.Emit(OpCodes.Ldarg_0); // push this
			gen.Emit(OpCodes.Ldfld, InterceptorFieldBuilder); // push interceptor reference
			
			gen.Emit(OpCodes.Ldloc_2); // push the invocation
			gen.Emit(OpCodes.Ldloc_1); // push the array into stack

			gen.Emit(OpCodes.Callvirt, typeof (IInterceptor).GetMethod("Intercept"));

			if (builder.ReturnType == typeof (void))
			{
				gen.Emit(OpCodes.Pop);
			}
			else
			{
				if (!builder.ReturnType.IsValueType)
				{
					gen.Emit(OpCodes.Castclass, builder.ReturnType);
				}
				else
				{
					gen.Emit(OpCodes.Unbox, builder.ReturnType);
					OpCodeUtil.ConvertTypeToOpCode(gen, builder.ReturnType);
				}

				gen.Emit(OpCodes.Stloc, 3);

				Label label = gen.DefineLabel();
				gen.Emit(OpCodes.Br_S, label);
				gen.MarkLabel(label);
				gen.Emit(OpCodes.Ldloc, 3); // Push the return value
			}

			gen.Emit(OpCodes.Ret);
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
	internal class CachedField
	{
		private FieldBuilder m_field; 
		private CallableDelegateBuilder m_callable; 
		private MethodInfo m_callback;
		private int m_sourceArgIndex;

		public CachedField(FieldBuilder field, CallableDelegateBuilder callable, MethodInfo callback, int sourceArgIndex)
		{
			this.m_field = field;
			this.m_callable = callable;
			this.m_callback = callback;
			this.m_sourceArgIndex = sourceArgIndex;
		}

		public FieldBuilder Field
		{
			get { return m_field; }
		}

		public CallableDelegateBuilder Callable
		{
			get { return m_callable; }
		}

		public int SourceArgIndex
		{
			get { return m_sourceArgIndex; }
		}

		public void WriteInitialization(ILGenerator gen, OpCode argIndex, OpCode arrayArgumentIndex)
		{
			if (SourceArgIndex == EmptyIndex)
			{
				// target is an argument
				gen.Emit(OpCodes.Ldarg_0);
				gen.Emit(argIndex);
			}
			else
			{
				// Obtain target from array
				gen.Emit(arrayArgumentIndex);
				gen.Emit(OpCodes.Ldc_I4, SourceArgIndex);
				gen.Emit(OpCodes.Ldelem_Ref);
				gen.Emit(OpCodes.Stloc_0);
				gen.Emit(OpCodes.Ldarg_0);
				gen.Emit(OpCodes.Ldloc_0);
			}
			
			gen.Emit(OpCodes.Ldftn, m_callback);
			gen.Emit(OpCodes.Newobj, m_callable.Constructor);
			gen.Emit(OpCodes.Stfld, m_field);
		}

		public static int EmptyIndex
		{
			get { return -1; }
		}
	}
}