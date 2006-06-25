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

namespace Castle.DynamicProxy.Generators.Emitters
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	public abstract class AbstractTypeEmitter
	{
		// private int counter;

		protected TypeBuilder typebuilder;

		protected ConstructorCollection constructors;
		protected MethodCollection methods;
		protected PropertiesCollection properties;
		protected EventCollection events;
		protected internal NestedClassCollection nested;
//#if DOTNET2
		protected GenericTypeParameterBuilder[] genericTypeParams;
		protected IDictionary<String, Type> name2GenericType;
//#endif

		public AbstractTypeEmitter()
		{
			nested = new NestedClassCollection();
			methods = new MethodCollection();
			constructors = new ConstructorCollection();
			properties = new PropertiesCollection();
			events = new EventCollection();
			name2GenericType = new Dictionary<String, Type>();
		}
		
		public bool IsGenericArgument(String genericArgumentName)
		{
			return name2GenericType.ContainsKey(genericArgumentName);
		}
		
		public Type GetGenericArgument(String genericArgumentName)
		{
			return name2GenericType[genericArgumentName];
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

//		public EasyConstructor CreateRuntimeConstructor(params ArgumentReference[] arguments)
//		{
//			EasyRuntimeConstructor member = new EasyRuntimeConstructor(this, arguments);
//			constructors.Add(member);
//			return member;
//		}

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

//		public EasyProperty CreateProperty(String name, Type returnType)
//		{
//			EasyProperty prop = new EasyProperty(this, name, returnType);
//			properties.Add(prop);
//			return prop;
//		}
//
//		public EasyProperty CreateProperty(PropertyInfo property)
//		{
//			EasyProperty prop = new EasyProperty(this, property.Name, property.PropertyType);
//			prop.IndexParameters = property.GetIndexParameters();
//			properties.Add(prop);
//			return prop;
//		}
//
//		public EasyEvent CreateEvent(String name, Type eventHandlerType)
//		{
//			EasyEvent easyEvent = new EasyEvent(this, name, eventHandlerType);
//			events.Add(easyEvent);
//			return easyEvent;
//		}

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

//#if DOTNET2

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
			
			// Type[] genericArguments = targetType.GetGenericArguments();

			String[] argumentNames = new String[genericArguments.Length];

			for (int i = 0; i < argumentNames.Length; i++)
			{
				argumentNames[i] = genericArguments[i].Name;
			}

			genericTypeParams = typebuilder.DefineGenericParameters(argumentNames);

			for (int i = 0; i < genericTypeParams.Length; i++)
			{
				try
				{
					// TODO: Review this
					// GenericParameterAttributes attributes = genericArguments[i].GenericParameterAttributes;
					// genericTypeParams[i].SetGenericParameterAttributes(attributes);
				}
				catch(NotSupportedException)
				{
					// Doesnt matter
				}

				name2GenericType.Add(genericArguments[i].Name, genericArguments[i]);
			}
		}

//#endif

		public virtual Type BuildType()
		{
			EnsureBuildersAreInAValidState();

			Type type = typebuilder.CreateType();

			foreach(NestedClassEmitter builder in nested)
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

			foreach(IMemberEmitter builder in properties)
			{
				builder.EnsureValidCodeBlock();
				builder.Generate();
			}
			foreach(IMemberEmitter builder in events)
			{
				builder.EnsureValidCodeBlock();
				builder.Generate();
			}
			foreach(IMemberEmitter builder in constructors)
			{
				builder.EnsureValidCodeBlock();
				builder.Generate();
			}
			foreach(IMemberEmitter builder in methods)
			{
				builder.EnsureValidCodeBlock();
				builder.Generate();
			}
		}
	}
}