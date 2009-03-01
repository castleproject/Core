// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Reflection.Emit;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	public abstract class AbstractTypeEmitter
	{
		private readonly TypeBuilder typebuilder;
		private readonly ConstructorCollection constructors;
		private readonly MethodCollection methods;
		private readonly PropertiesCollection properties;
		private readonly EventCollection events;
		private readonly NestedClassCollection nested;
		private readonly Dictionary<String, GenericTypeParameterBuilder> name2GenericType;

		private GenericTypeParameterBuilder[] genericTypeParams;

		public AbstractTypeEmitter(TypeBuilder typeBuilder)
		{
			this.typebuilder = typeBuilder;
			nested = new NestedClassCollection();
			methods = new MethodCollection();
			constructors = new ConstructorCollection();
			properties = new PropertiesCollection();
			events = new EventCollection();
			name2GenericType = new Dictionary<String, GenericTypeParameterBuilder>();
		}

		public bool IsGenericArgument(String genericArgumentName)
		{
			return name2GenericType.ContainsKey(genericArgumentName);
		}

		public Type GetGenericArgument(String genericArgumentName)
		{
			return name2GenericType[genericArgumentName];
		}

		public Type[] GetGenericArgumentsFor(Type genericType)
		{
			List<Type> types = new List<Type>();

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

			return types.ToArray();
		}

		public Type[] GetGenericArgumentsFor(MethodInfo genericMethod)
		{
			List<Type> types = new List<Type>();
			foreach (Type genType in genericMethod.GetGenericArguments())
			{
				types.Add(name2GenericType[genType.Name]);
			}

			return types.ToArray();
		}

		public void AddCustomAttributes(ProxyGenerationOptions proxyGenerationOptions)
		{
			foreach (Attribute attr in proxyGenerationOptions.AttributesToAddToGeneratedTypes)
			{
				DefineCustomAttribute(attr);
			}
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

		public MethodEmitter CreateMethod(String name, MethodAttributes attrs, Type returnType)
		{
			return CreateMethod(name, attrs, returnType, new Type[0]);
		}

		public MethodEmitter CreateMethod(String name, MethodAttributes attrs, Type returnType,
		                                  params Type[] argumentTypes)
		{
			MethodEmitter member =
				new MethodEmitter(this, name, attrs, returnType, argumentTypes);
			methods.Add(member);
			return member;
		}

		public MethodEmitter CreateMethod(String name, Type returnType,
		                                  params ArgumentReference[] argumentReferences)
		{
			Type[] argumentTypes = ArgumentsUtil.InitializeAndConvert(argumentReferences);
			const MethodAttributes defaultAttributes =
				MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public;
			return CreateMethod(name, defaultAttributes, returnType, argumentTypes);
		}

		public MethodEmitter CreateMethod(String name, MethodAttributes attrs, Type returnType,
		                                  params ArgumentReference[] argumentReferences)
		{
			Type[] argumentTypes = ArgumentsUtil.InitializeAndConvert(argumentReferences);
			return CreateMethod(name, attrs, returnType, argumentTypes);
		}

		public FieldReference CreateStaticField(string name, Type fieldType)
		{
			FieldAttributes atts = FieldAttributes.Public;

			return CreateStaticField(name, fieldType, atts);
		}

		public FieldReference CreateStaticField(string name, Type fieldType, FieldAttributes atts)
		{
			atts |= FieldAttributes.Static;
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

			return CreateField(name, fieldType, atts);
		}

		public FieldReference CreateField(string name, Type fieldType, FieldAttributes atts)
		{
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
			if (customAttributeBuilder == null)
				return;
			typebuilder.SetCustomAttribute(customAttributeBuilder);
		}

		public void DefineCustomAttributeFor(FieldReference field, Attribute attribute)
		{
			CustomAttributeBuilder customAttributeBuilder = CustomAttributeUtil.CreateCustomAttribute(attribute);
			if (customAttributeBuilder == null)
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

		public PropertiesCollection Properties
		{
			get { return properties; }
		}

		public EventCollection Events
		{
			get { return events; }
		}

		public NestedClassCollection Nested
		{
			get { return nested; }
		}

		public TypeBuilder TypeBuilder
		{
			get { return typebuilder; }
		}

		internal Type BaseType
		{
			get { return TypeBuilder.BaseType; }
		}

		public GenericTypeParameterBuilder[] GenericTypeParams
		{
			get { return genericTypeParams; }
		}

		public void SetGenericTypeParameters(GenericTypeParameterBuilder[] genericTypeParameterBuilders)
		{
			this.genericTypeParams = genericTypeParameterBuilders;
		}

		public void CreateGenericParameters(Type[] genericArguments)
		{
			// big sanity check
			if (genericTypeParams != null)
			{
				throw new ProxyGenerationException("CreateGenericParameters: cannot invoke me twice");
			}

			SetGenericTypeParameters(GenericUtil.DefineGenericArguments(genericArguments, typebuilder, name2GenericType));
		}

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
