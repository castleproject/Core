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
	using System.Text;
	using System.Collections;
	using System.Reflection;
	using System.Reflection.Emit;

	class CachedField
	{
		FieldBuilder field; 
		CallableDelegateBuilder callable; 
		MethodBuilder callback;

		public CachedField(FieldBuilder field, CallableDelegateBuilder callable, MethodBuilder callback)
		{
			this.field = field;
			this.callable = callable;
			this.callback = callback;
		}

		public FieldBuilder Field
		{
			get { return field; }
		}

		public CallableDelegateBuilder Callable
		{
			get { return callable; }
		}

		public void WriteInitialization(ILGenerator gen)
		{
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldarg_0);
//			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldftn, callback);
			gen.Emit(OpCodes.Newobj, callable.Constructor);
			gen.Emit(OpCodes.Stfld, field);
		}
	}

	/// <summary>
	/// Summary description for ClassProxyGenerator.
	/// </summary>
	public class ClassProxyGenerator : BaseCodeGenerator
	{
		private Hashtable m_method2Delegate = new Hashtable();
		private Hashtable m_method2BaseCall = new Hashtable();
		private ArrayList m_cachedFields = new ArrayList();

		public ClassProxyGenerator(ModuleScope scope) : base(scope)
		{
		}

		public ClassProxyGenerator(ModuleScope scope, GeneratorContext context) : base(scope, context)
		{
		}

		protected override String GenerateTypeName(Type type, Type[] interfaces)
		{
			StringBuilder sb = new StringBuilder();
			foreach (Type inter in interfaces)
			{
				sb.Append('_');
				sb.Append(inter.Name);
			}
			/// Naive implementation
			return String.Format("CProxyType{0}{1}", type.Name, sb.ToString());
		}

		/// <summary>
		/// Generates one public constructor receiving 
		/// the <see cref="IInterceptor"/> instance and instantiating a hashtable
		/// </summary>
		/// <returns><see cref="ConstructorBuilder"/> instance</returns>
		protected override ConstructorBuilder GenerateConstructor()
		{
			ConstructorBuilder consBuilder = MainTypeBuilder.DefineConstructor(
				MethodAttributes.Public,
				CallingConventions.Standard,
				new Type[] {typeof (IInterceptor)});

			ILGenerator ilGenerator = consBuilder.GetILGenerator();
			
			// Calls the base constructor
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Call, ObtainAvailableConstructor(m_baseType));
			
			// Stores the interceptor in the field
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Ldarg_1);
			ilGenerator.Emit(OpCodes.Stfld, InterceptorFieldBuilder);

			// Instantiates the hashtable
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Newobj, typeof(Hashtable).GetConstructor( new Type[0] ));
			ilGenerator.Emit(OpCodes.Stfld, CacheFieldBuilder);

			// Initialize the delegate fields

			foreach(CachedField field in m_cachedFields)
			{
				field.WriteInitialization(ilGenerator);
			}

			ilGenerator.Emit(OpCodes.Ret);

			return consBuilder;
		}

		public Type GenerateCode(Type baseClass)
		{
			Type[] interfaces = new Type[0];
			interfaces = AddISerializable(interfaces);

			Type cacheType = GetFromCache(baseClass, interfaces);
			
			if (cacheType != null)
			{
				return cacheType;
			}

			CreateTypeBuilder( baseClass, interfaces );
			GenerateFields();
			ImplementGetObjectData();
			ImplementCacheInvocationCache();
			GenerateTypeImplementation( baseClass, true );
			GenerateConstructor();
			return CreateType();
		}

		protected override void PreProcessMethod(MethodInfo method)
		{
			MethodBuilder baseInvokeMethod = GenerateMethodBaseInvoke(method);
			m_method2BaseCall[method] = baseInvokeMethod;

			CallableDelegateBuilder delegateBuilder = CallableDelegateBuilder.BuildForMethod( 
				MainTypeBuilder, method, ModuleScope );
			m_method2Delegate[method] = delegateBuilder;

			FieldBuilder field = GenerateField( 
				String.Format("_cached_{0}", delegateBuilder.ID), 
				typeof(ICallable) );
			RegisterCacheFieldToBeInitialized(field, delegateBuilder, baseInvokeMethod);
		}

		private FieldBuilder ObtainCachedFieldBuilderDelegate(CallableDelegateBuilder builder)
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

		private void RegisterCacheFieldToBeInitialized(FieldBuilder field, CallableDelegateBuilder builder, MethodBuilder baseInvokeMethod)
		{
			m_cachedFields.Add( new CachedField(field, builder, baseInvokeMethod) );
		}

		protected MethodBuilder GenerateMethodBaseInvoke(MethodInfo method)
		{
			MethodAttributes atts = MethodAttributes.Private;

			ParameterInfo[] parameterInfo = method.GetParameters();

			Type[] parameters = new Type[parameterInfo.Length];

			for (int i = 0; i < parameterInfo.Length; i++)
			{
				parameters[i] = parameterInfo[i].ParameterType;
			}

			String name = String.Format("callback__{0}", method.Name);

			MethodBuilder baseMethodCall = MainTypeBuilder.DefineMethod(name, atts, method.ReturnType, parameters);

			ILGenerator gen = baseMethodCall.GetILGenerator();

			if (method.ReturnType != typeof(void))
			{
				gen.DeclareLocal( method.ReturnType );
			}
			
			gen.Emit(OpCodes.Ldarg_0);

			for (int i = 0; i < parameterInfo.Length; i++)
			{
				gen.Emit(OpCodes.Ldarg, i + 1);
			}

			gen.Emit(OpCodes.Call, method);

			if (method.ReturnType != typeof(void))
			{
				gen.Emit(OpCodes.Stloc_0);
				Label retBranch = gen.DefineLabel();
				gen.Emit(OpCodes.Br_S, retBranch);
				gen.MarkLabel(retBranch);
				gen.Emit(OpCodes.Ldloc_0);
			}

			gen.Emit(OpCodes.Ret);

			return baseMethodCall;
		}

		protected override void WriteILForMethod(MethodInfo method, MethodBuilder builder, Type[] parameters)
		{
			ILGenerator gen = builder.GetILGenerator();

			gen.DeclareLocal(typeof (MethodBase));   // 0
			gen.DeclareLocal(typeof (object[]));     // 1
			gen.DeclareLocal(typeof (IInvocation));  // 2

			if (builder.ReturnType != typeof (void))
			{
				gen.DeclareLocal(builder.ReturnType);// 3
			}

			CallableDelegateBuilder delegateType = m_method2Delegate[method] as CallableDelegateBuilder;
			//			MethodBuilder delegateTarget = m_method2BaseCall[method] as MethodBuilder;
			FieldBuilder fieldDelegate = ObtainCachedFieldBuilderDelegate( delegateType );

			// Obtains the MethodBase from ldtoken method
			gen.Emit(OpCodes.Ldtoken, method);
			gen.Emit(OpCodes.Call, typeof (MethodBase).GetMethod("GetMethodFromHandle"));
			gen.Emit(OpCodes.Stloc_0);

			// Invokes the Method2Invocation to obtain a proper IInvocation instance
			gen.Emit(OpCodes.Ldarg_0);

			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldfld, fieldDelegate);
//			gen.Emit(OpCodes.Ldftn, delegateTarget);
//			gen.Emit(OpCodes.Newobj, delegateType.Constructor);
			gen.Emit(OpCodes.Ldloc_0);
			gen.Emit(OpCodes.Castclass, typeof (MethodInfo)); // Cast MethodBase to MethodInfo
			gen.Emit(OpCodes.Ldarg_0);
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

		protected override Type CreateType()
		{
			Type newType = MainTypeBuilder.CreateType();

			foreach(CallableDelegateBuilder builder in m_method2Delegate.Values)
			{
				builder.CreateType();
			}

			RegisterInCache(newType);

			return newType;
		}
	}
}
