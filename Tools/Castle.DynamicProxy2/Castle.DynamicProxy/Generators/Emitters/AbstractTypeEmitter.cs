// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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


namespace Castle.DynamicProxy.Generators.Emitters
{
	using System;
	using System.Reflection;
	using System.Reflection.Emit;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using System.Collections;

#if DOTNET2
	using System.Collections.Generic;
#endif

	[CLSCompliant(false)]
	public abstract class AbstractTypeEmitter
	{
		protected TypeBuilder typebuilder;
		protected ConstructorCollection constructors;
		protected MethodCollection methods;
		protected PropertiesCollection properties;
		protected EventCollection events;
		protected internal NestedClassCollection nested;
		#if DOTNET2
		protected GenericTypeParameterBuilder[] genericTypeParams;
		protected Dictionary<String, GenericTypeParameterBuilder> name2GenericType;
		#endif

		public AbstractTypeEmitter()
		{
			nested = new NestedClassCollection();
			methods = new MethodCollection();
			constructors = new ConstructorCollection();
			properties = new PropertiesCollection();
			events = new EventCollection();
#if DOTNET2
			name2GenericType = new Dictionary<String, GenericTypeParameterBuilder>();
#endif
		}

		public bool IsGenericArgument(String genericArgumentName)
		{
#if DOTNET2
			return name2GenericType.ContainsKey(genericArgumentName);
#else
			return false;
#endif
		}

		public Type GetGenericArgument(String genericArgumentName)
		{
#if DOTNET2
			return name2GenericType[genericArgumentName];
#else
			throw new NotSupportedException("Generic arguments are not supported for 1.1");
#endif
		}

		public Type[] GetGenericArgumentsFor(Type genericType)
		{
			ArrayList types = new ArrayList();
#if DOTNET2

			foreach (Type genType in genericType.GetGenericArguments())
			{
				if (genType.IsGenericParameter)
				{
					types.Add(name2GenericType[genType.Name]);
				}
				else
				{
					types.Add(genType);
				}
			}
#endif
			return (Type[]) types.ToArray(typeof (Type));
		}

		public Type[] GetGenericArgumentsFor(MethodInfo genericMethod)
		{
#if DOTNET2
			List<Type> types = new List<Type>();
			foreach (Type genType in genericMethod.GetGenericArguments())
			{
				types.Add(name2GenericType[genType.Name]);
			}

			return types.ToArray();
#else
			return new Type[0];
#endif
		}

		public void CreateDefaultConstructor()
		{
			constructors.Add(new ConstructorEmitter(this));
		}

		public ConstructorEmitter CreateConstructor(params ArgumentReference[] arguments)
		{
			ConstructorEmitter member = new ConstructorEmitter(this, arguments);
			constructors.Add(member);
			return member;
		}

		public ConstructorEmitter CreateTypeConstructor()
		{
			ConstructorEmitter member = new TypeConstructorEmitter(this);
			constructors.Add(member);
			return member;
		}

		public MethodEmitter CreateMethod(String name, MethodAttributes attributes)
		{
			MethodEmitter member = new MethodEmitter(this, name, attributes);
			methods.Add(member);
			return member;
		}

		public MethodEmitter CreateMethod(String name, ReturnReferenceExpression returnType, params ArgumentReference[] arguments)
		{
			MethodEmitter member = new MethodEmitter(this, name, returnType, arguments);
			methods.Add(member);
			return member;
		}

		public MethodEmitter CreateMethod(String name, ReturnReferenceExpression returnType, MethodAttributes attributes, params ArgumentReference[] arguments)
		{
			MethodEmitter member = new MethodEmitter(this, name, attributes, returnType, arguments);
			methods.Add(member);
			return member;
		}

		public MethodEmitter CreateMethod(String name, MethodAttributes attrs, ReturnReferenceExpression returnType, params Type[] args)
		{
			MethodEmitter member = new MethodEmitter(this, name, attrs, returnType, ArgumentsUtil.ConvertToArgumentReference(args));
			methods.Add(member);
			return member;
		}

		//		public EasyRuntimeMethod CreateRuntimeMethod(String name, ReturnReferenceExpression returnType, params ArgumentReference[] arguments)
		//		{
		//			EasyRuntimeMethod member = new EasyRuntimeMethod(this, name, returnType, arguments);
		//			methods.Add(member);
		//			return member;
		//		}

		public FieldReference CreateStaticField(string name, Type fieldType)
		{
			FieldAttributes atts = FieldAttributes.Public | FieldAttributes.Static;

			FieldBuilder fieldBuilder = typebuilder.DefineField(name, fieldType, atts);

			return new FieldReference(fieldBuilder);
		}

		public FieldReference CreateField(string name, Type fieldType)
		{
			return CreateField(name, fieldType, true);
		}

		public FieldReference CreateField(string name, Type fieldType, bool serializable)
		{
			FieldAttributes atts = FieldAttributes.Public;

			if (!serializable)
			{
				atts |= FieldAttributes.NotSerialized;
			}

			FieldBuilder fieldBuilder = typebuilder.DefineField(name, fieldType, atts);

			return new FieldReference(fieldBuilder);
		}

		public PropertyEmitter CreateProperty(String name, PropertyAttributes attributes, Type propertyType)
		{
			PropertyEmitter propEmitter = new PropertyEmitter(this, name, attributes, propertyType);
			properties.Add(propEmitter);
			return propEmitter;
		}


		public EventEmitter CreateEvent(string name, EventAttributes atts, Type type)
		{
			EventEmitter eventEmitter = new EventEmitter(this, name, atts, type);
			events.Add(eventEmitter);
			return eventEmitter;
		}

		public void DefineCustomAttribute(Attribute attribute)
		{
			CustomAttributeBuilder customAttributeBuilder = CustomAttributeUtil.CreateCustomAttribute(attribute);
			if(customAttributeBuilder==null)
				return;
			typebuilder.SetCustomAttribute(customAttributeBuilder);
		}

		public void DefineCustomAttributeFor(FieldReference field, Attribute attribute)
		{
			CustomAttributeBuilder customAttributeBuilder = CustomAttributeUtil.CreateCustomAttribute(attribute);
			if(customAttributeBuilder==null)
				return;
			field.Reference.SetCustomAttribute(customAttributeBuilder);
		}

		public ConstructorCollection Constructors
		{
			get { return constructors; }
		}

		public MethodCollection Methods
		{
			get { return methods; }
		}

		public TypeBuilder TypeBuilder
		{
			get { return typebuilder; }
		}

		internal Type BaseType
		{
			get { return TypeBuilder.BaseType; }
		}

#if DOTNET2

		public GenericTypeParameterBuilder[] GenericTypeParams
		{
			get { return genericTypeParams; }
		}

		public void CreateGenericParameters(Type[] genericArguments)
		{
			// big sanity check
			if (genericTypeParams != null)
			{
				throw new ApplicationException("CreateGenericParameters: cannot invoke me twice");
			}

			genericTypeParams = GenericUtil.DefineGenericArguments(genericArguments, typebuilder, name2GenericType);
		}
#endif

		public virtual Type BuildType()
		{
			EnsureBuildersAreInAValidState();

			Type type = typebuilder.CreateType();

			foreach (NestedClassEmitter builder in nested)
			{
				builder.BuildType();
			}

			return type;
		}

		protected virtual void EnsureBuildersAreInAValidState()
		{
			if (constructors.Count == 0)
			{
				CreateDefaultConstructor();
			}

			foreach (IMemberEmitter builder in properties)
			{
				builder.EnsureValidCodeBlock();
				builder.Generate();
			}
			foreach (IMemberEmitter builder in events)
			{
				builder.EnsureValidCodeBlock();
				builder.Generate();
			}
			foreach (IMemberEmitter builder in constructors)
			{
				builder.EnsureValidCodeBlock();
				builder.Generate();
			}
			foreach (IMemberEmitter builder in methods)
			{
				builder.EnsureValidCodeBlock();
				builder.Generate();
			}
		}
	}
}
