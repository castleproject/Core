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
	using System.Text;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Threading;

	using Castle.DynamicProxy.Builder.CodeBuilder;
	using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;
	using Castle.DynamicProxy.Builder.CodeBuilder.Utils;

	/// <summary>
	/// Summary description for ClassProxyGenerator.
	/// </summary>
	[CLSCompliant(false)]
	public class ClassProxyGenerator : BaseCodeGenerator
	{
		private bool _delegateToBaseGetObjectData;
		
		protected ConstructorInfo _serializationConstructor;

		public ClassProxyGenerator(ModuleScope scope) : base(scope)
		{
		}

		public ClassProxyGenerator(ModuleScope scope, GeneratorContext context) : base(scope, context)
		{
		}

		protected override Type InvocationType
		{
			get { return Context.SameClassInvocation; }
		}

		protected override String GenerateTypeName(Type type, Type[] interfaces)
		{
			StringBuilder sb = new StringBuilder();
			foreach (Type inter in interfaces)
			{
				sb.Append('_');
				sb.Append(inter.Name);
			}
			// Naive implementation
			return String.Format("CProxyType{0}{3}{1}{2}", type.Name, sb.ToString(), interfaces.Length, NormalizeNamespaceName(type.Namespace) );
		}

		/// <summary>
		/// Generates one public constructor receiving 
		/// the <see cref="IInterceptor"/> instance and instantiating a hashtable
		/// </summary>
		protected virtual EasyConstructor GenerateConstructor( ConstructorInfo baseConstructor )
		{
			ArrayList arguments = new ArrayList();

			ArgumentReference arg1 = new ArgumentReference( Context.Interceptor );
			ArgumentReference arg2 = new ArgumentReference( typeof(object[]) );

			arguments.Add( arg1 );

			ParameterInfo[] parameters = baseConstructor.GetParameters();

			if (Context.HasMixins)
			{
				arguments.Add( arg2 );
			}

			ArgumentReference[] originalArguments = 
				ArgumentsUtil.ConvertToArgumentReference(parameters);

			arguments.AddRange(originalArguments);

			EasyConstructor constructor = MainTypeBuilder.CreateConstructor( 
				(ArgumentReference[]) arguments.ToArray( typeof(ArgumentReference) ) );

			GenerateConstructorCode(constructor.CodeBuilder, arg1, SelfReference.Self, arg2);

			constructor.CodeBuilder.InvokeBaseConstructor( baseConstructor, originalArguments );

			constructor.CodeBuilder.AddStatement( new ReturnStatement() );


			return constructor;
		}

		protected void GenerateSerializationConstructor()
		{
			ArgumentReference arg1 = new ArgumentReference( typeof(SerializationInfo) );
			ArgumentReference arg2 = new ArgumentReference( typeof(StreamingContext) );

			EasyConstructor constr = MainTypeBuilder.CreateConstructor( arg1, arg2 );

			constr.CodeBuilder.AddStatement( new ExpressionStatement(
				new ConstructorInvocationExpression( _serializationConstructor, 
				arg1.ToExpression(), arg2.ToExpression() )) );

			Type[] object_arg = new Type[] { typeof (String), typeof(Type) };
			MethodInfo getValueMethod = typeof (SerializationInfo).GetMethod("GetValue", object_arg);

			VirtualMethodInvocationExpression getInterceptorInvocation =
				new VirtualMethodInvocationExpression(arg1, getValueMethod, 
				new FixedReference("__interceptor").ToExpression(), 
				new TypeTokenExpression( Context.Interceptor ) );

			VirtualMethodInvocationExpression getMixinsInvocation =
				new VirtualMethodInvocationExpression(arg1, getValueMethod, 
				new FixedReference("__mixins").ToExpression(), 
				new TypeTokenExpression( typeof(object[]) ) );

			constr.CodeBuilder.AddStatement( new AssignStatement(
				InterceptorField, getInterceptorInvocation) );

			constr.CodeBuilder.AddStatement( new AssignStatement(
				CacheField, new NewInstanceExpression(
				typeof(HybridDictionary).GetConstructor( new Type[0] )) ) );

			constr.CodeBuilder.AddStatement( new AssignStatement(
				MixinField,  
				getMixinsInvocation) );

			// Initialize the delegate fields
			foreach(CallableField field in _cachedFields)
			{
				field.WriteInitialization(constr.CodeBuilder, SelfReference.Self, MixinField);
			}

			constr.CodeBuilder.AddStatement( new ReturnStatement() );
		}

		protected override void CustomizeGetObjectData(AbstractCodeBuilder codebuilder, 
			ArgumentReference arg1, ArgumentReference arg2)
		{
			Type[] key_and_object = new Type[] {typeof (String), typeof (Object)};
			Type[] key_and_bool = new Type[] {typeof (String), typeof (bool)};
			MethodInfo addValueMethod = typeof (SerializationInfo).GetMethod("AddValue", key_and_object);
			MethodInfo addValueBoolMethod = typeof (SerializationInfo).GetMethod("AddValue", key_and_bool);

			codebuilder.AddStatement( new ExpressionStatement(
				new VirtualMethodInvocationExpression(arg1, addValueBoolMethod, 
				new FixedReference("__delegateToBase").ToExpression(), 
				new FixedReference( _delegateToBaseGetObjectData ? 1 : 0 ).ToExpression() ) ) );

			if (_delegateToBaseGetObjectData)
			{
				MethodInfo baseGetObjectData = _baseType.GetMethod("GetObjectData", 
					new Type[] { typeof(SerializationInfo), typeof(StreamingContext) });

				codebuilder.AddStatement( new ExpressionStatement(
					new MethodInvocationExpression( baseGetObjectData, 
						arg1.ToExpression(), arg2.ToExpression() )) );
			}
			else
			{
				LocalReference members_ref = codebuilder.DeclareLocal( typeof(MemberInfo[]) );
				LocalReference data_ref = codebuilder.DeclareLocal( typeof(object[]) );

				MethodInfo getSerMembers = typeof(FormatterServices).GetMethod("GetSerializableMembers", 
					new Type[] { typeof(Type) });
				MethodInfo getObjData = typeof(FormatterServices).GetMethod("GetObjectData", 
					new Type[] { typeof(object), typeof(MemberInfo[]) });
				
				codebuilder.AddStatement( new AssignStatement( members_ref,
					new MethodInvocationExpression( null, getSerMembers, 
					new TypeTokenExpression( _baseType ) )) );
				
				codebuilder.AddStatement( new AssignStatement( data_ref, 
					new MethodInvocationExpression( null, getObjData, 
					SelfReference.Self.ToExpression(), members_ref.ToExpression() )) );

				codebuilder.AddStatement( new ExpressionStatement(
					new VirtualMethodInvocationExpression(arg1, addValueMethod, 
					new FixedReference("__data").ToExpression(), 
					data_ref.ToExpression() ) ) );
			}
		}

		public virtual Type GenerateCode(Type baseClass)
		{
			return GenerateCode(baseClass, new Type[0]);
		}
		
		public virtual Type GenerateCode(Type baseClass, Type[] interfaces)
		{
			if (baseClass.IsSerializable)
			{
				_delegateToBaseGetObjectData = VerifyIfBaseImplementsGetObjectData(baseClass);
				interfaces = AddISerializable( interfaces );
			}

			ReaderWriterLock rwlock = ModuleScope.RWLock;

			rwlock.AcquireReaderLock(-1);

			Type cacheType = GetFromCache(baseClass, interfaces);
			
			if (cacheType != null)
			{
				rwlock.ReleaseReaderLock();

				return cacheType;
			}

			rwlock.UpgradeToWriterLock(-1);

			try
			{
				CreateTypeBuilder( GenerateTypeName(baseClass, interfaces), baseClass, interfaces );
				GenerateFields();

				if (baseClass.IsSerializable)
				{
					ImplementGetObjectData( interfaces );
				}

				ImplementCacheInvocationCache();
				GenerateTypeImplementation( baseClass, true );
				GenerateInterfaceImplementation(interfaces);
				GenerateConstructors(baseClass);

				if (_delegateToBaseGetObjectData)
				{
					GenerateSerializationConstructor();
				}

				return CreateType();
			}
			finally
			{
				rwlock.ReleaseWriterLock();
			}
		}

		public virtual Type GenerateCustomCode(Type baseClass, Type[] interfaces)
		{
			if (!Context.HasMixins)
			{
				return GenerateCode(baseClass);
			}

			_mixins = Context.MixinsAsArray();
			Type[] mixinInterfaces = InspectAndRegisterInterfaces( _mixins );
			interfaces = Join(interfaces, mixinInterfaces);

			return GenerateCode(baseClass, mixinInterfaces);
		}

		protected virtual void GenerateConstructors(Type baseClass)
		{
			ConstructorInfo[] constructors = 
				baseClass.GetConstructors( BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic );

			foreach(ConstructorInfo constructor in constructors)
			{
				if (constructor.IsPrivate)
				{
					continue;
				}
                if (constructor.IsAssembly && !IsInternalToDynamicProxy(baseClass.Assembly))
                {
                    continue;
                }
				GenerateConstructor(constructor);
			}
		}

		protected bool VerifyIfBaseImplementsGetObjectData(Type baseType)
		{
			// If base type implements ISerializable, we have to make sure
			// the GetObjectData is marked as virtual
			
			if (typeof(ISerializable).IsAssignableFrom(baseType))
			{
				MethodInfo getObjectDataMethod = baseType.GetMethod("GetObjectData",  
					new Type[] { typeof(SerializationInfo), typeof(StreamingContext) });

				if (getObjectDataMethod==null)//explicit interface implementation
				{
					return false;
				}

				if (!getObjectDataMethod.IsVirtual || getObjectDataMethod.IsFinal)
				{
					String message = String.Format("The type {0} implements ISerializable, but GetObjectData is not marked as virtual", 
						baseType.FullName);
					throw new ProxyGenerationException(message);
				}

				Context.AddMethodToSkip(getObjectDataMethod);

				_serializationConstructor = baseType.GetConstructor( 
					BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic, 
					null,  
					new Type[] { typeof(SerializationInfo), typeof(StreamingContext) }, 
					null);

				if (_serializationConstructor == null)
				{
					String message = String.Format("The type {0} implements ISerializable, but failed to provide a deserialization constructor", 
						baseType.FullName);
					throw new ProxyGenerationException(message);
				}

				return true;
			}
			return false;
		}

		protected void SkipDefaultInterfaceImplementation(Type[] interfaces)
		{
			foreach( Type inter in interfaces )
			{
				Context.AddInterfaceToSkip(inter);
			}
		}

		protected Type[] Join(Type[] interfaces, Type[] mixinInterfaces)
		{
			if (interfaces == null) interfaces = new Type[0];
			if (mixinInterfaces == null) mixinInterfaces = new Type[0];
			Type[] union = new Type[ interfaces.Length + mixinInterfaces.Length ];
			Array.Copy( interfaces, 0, union, 0, interfaces.Length );
			Array.Copy( mixinInterfaces, 0, union, interfaces.Length, mixinInterfaces.Length );
			return union;
		}
	}
}
