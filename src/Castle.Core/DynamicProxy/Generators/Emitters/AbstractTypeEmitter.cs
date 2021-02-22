// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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
	using System.Diagnostics;
	using System.Reflection;
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Internal;

	internal abstract class AbstractTypeEmitter
	{
		private const MethodAttributes defaultAttributes =
			MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public;

		private readonly List<ConstructorEmitter> constructors;
		private readonly List<EventEmitter> events;

		private readonly IDictionary<string, FieldReference> fields =
			new Dictionary<string, FieldReference>(StringComparer.OrdinalIgnoreCase);

		private readonly List<MethodEmitter> methods;

		private readonly Dictionary<string, GenericTypeParameterBuilder> name2GenericType;
		private readonly List<NestedClassEmitter> nested;
		private readonly List<PropertyEmitter> properties;
		private readonly TypeBuilder typebuilder;

		private GenericTypeParameterBuilder[] genericTypeParams;

		protected AbstractTypeEmitter(TypeBuilder typeBuilder)
		{
			typebuilder = typeBuilder;
			nested = new List<NestedClassEmitter>();
			methods = new List<MethodEmitter>();
			constructors = new List<ConstructorEmitter>();
			properties = new List<PropertyEmitter>();
			events = new List<EventEmitter>();
			name2GenericType = new Dictionary<string, GenericTypeParameterBuilder>();
		}

		public Type BaseType
		{
			get
			{
				if (TypeBuilder.IsInterface)
				{
					throw new InvalidOperationException("This emitter represents an interface; interfaces have no base types.");
				}
				return TypeBuilder.BaseType;
			}
		}

		public TypeConstructorEmitter ClassConstructor { get; private set; }

		public GenericTypeParameterBuilder[] GenericTypeParams
		{
			get { return genericTypeParams; }
		}

		public TypeBuilder TypeBuilder
		{
			get { return typebuilder; }
		}

		public void AddCustomAttributes(IEnumerable<CustomAttributeInfo> additionalAttributes)
		{
			foreach (var attribute in additionalAttributes)
			{
				typebuilder.SetCustomAttribute(attribute.Builder);
			}
		}

		public void AddNestedClass(NestedClassEmitter nestedClass)
		{
			nested.Add(nestedClass);
		}

		public virtual Type BuildType()
		{
			EnsureBuildersAreInAValidState();

			var type = CreateType(typebuilder);

			foreach (var builder in nested)
			{
				builder.BuildType();
			}

			return type;
		}

		public void CopyGenericParametersFromMethod(MethodInfo methodToCopyGenericsFrom)
		{
			// big sanity check
			if (genericTypeParams != null)
			{
				throw new InvalidOperationException("Cannot invoke me twice");
			}

			SetGenericTypeParameters(GenericUtil.CopyGenericArguments(methodToCopyGenericsFrom, typebuilder, name2GenericType));
		}

		public ConstructorEmitter CreateConstructor(params ArgumentReference[] arguments)
		{
			if (TypeBuilder.IsInterface)
			{
				throw new InvalidOperationException("Interfaces cannot have constructors.");
			}

			var member = new ConstructorEmitter(this, arguments);
			constructors.Add(member);
			return member;
		}

		public void CreateDefaultConstructor()
		{
			if (TypeBuilder.IsInterface)
			{
				throw new InvalidOperationException("Interfaces cannot have constructors.");
			}

			constructors.Add(new ConstructorEmitter(this));
		}

		public EventEmitter CreateEvent(string name, EventAttributes atts, Type type)
		{
			var eventEmitter = new EventEmitter(this, name, atts, type);
			events.Add(eventEmitter);
			return eventEmitter;
		}

		public FieldReference CreateField(string name, Type fieldType)
		{
			return CreateField(name, fieldType, true);
		}

		public FieldReference CreateField(string name, Type fieldType, bool serializable)
		{
			var atts = FieldAttributes.Private;

			if (!serializable)
			{
				atts |= FieldAttributes.NotSerialized;
			}

			return CreateField(name, fieldType, atts);
		}

		public FieldReference CreateField(string name, Type fieldType, FieldAttributes atts)
		{
			var fieldBuilder = typebuilder.DefineField(name, fieldType, atts);
			var reference = new FieldReference(fieldBuilder);
			fields[name] = reference;
			return reference;
		}

		public MethodEmitter CreateMethod(string name, MethodAttributes attrs, Type returnType, params Type[] argumentTypes)
		{
			var member = new MethodEmitter(this, name, attrs, returnType, argumentTypes ?? Type.EmptyTypes);
			methods.Add(member);
			return member;
		}

		public MethodEmitter CreateMethod(string name, Type returnType, params Type[] parameterTypes)
		{
			return CreateMethod(name, defaultAttributes, returnType, parameterTypes);
		}

		public MethodEmitter CreateMethod(string name, MethodInfo methodToUseAsATemplate)
		{
			return CreateMethod(name, defaultAttributes, methodToUseAsATemplate);
		}

		public MethodEmitter CreateMethod(string name, MethodAttributes attributes, MethodInfo methodToUseAsATemplate)
		{
			var method = new MethodEmitter(this, name, attributes, methodToUseAsATemplate);
			methods.Add(method);
			return method;
		}

		public PropertyEmitter CreateProperty(string name, PropertyAttributes attributes, Type propertyType, Type[] arguments)
		{
			var propEmitter = new PropertyEmitter(this, name, attributes, propertyType, arguments);
			properties.Add(propEmitter);
			return propEmitter;
		}

		public FieldReference CreateStaticField(string name, Type fieldType)
		{
			return CreateStaticField(name, fieldType, FieldAttributes.Private);
		}

		public FieldReference CreateStaticField(string name, Type fieldType, FieldAttributes atts)
		{
			atts |= FieldAttributes.Static;
			return CreateField(name, fieldType, atts);
		}

		public ConstructorEmitter CreateTypeConstructor()
		{
			var member = new TypeConstructorEmitter(this);
			constructors.Add(member);
			ClassConstructor = member;
			return member;
		}

		public void DefineCustomAttribute(CustomAttributeBuilder attribute)
		{
			typebuilder.SetCustomAttribute(attribute);
		}

		public void DefineCustomAttribute<TAttribute>(object[] constructorArguments) where TAttribute : Attribute
		{
			var customAttributeInfo = AttributeUtil.CreateInfo(typeof(TAttribute), constructorArguments);
			typebuilder.SetCustomAttribute(customAttributeInfo.Builder);
		}

		public void DefineCustomAttribute<TAttribute>() where TAttribute : Attribute, new()
		{
			var customAttributeInfo = AttributeUtil.CreateInfo<TAttribute>();
			typebuilder.SetCustomAttribute(customAttributeInfo.Builder);
		}

		public void DefineCustomAttributeFor<TAttribute>(FieldReference field) where TAttribute : Attribute, new()
		{
			var customAttributeInfo = AttributeUtil.CreateInfo<TAttribute>();
			var fieldbuilder = field.Fieldbuilder;
			if (fieldbuilder == null)
			{
				throw new ArgumentException(
					"Invalid field reference.This reference does not point to field on type being generated", nameof(field));
			}
			fieldbuilder.SetCustomAttribute(customAttributeInfo.Builder);
		}

		public IEnumerable<FieldReference> GetAllFields()
		{
			return fields.Values;
		}

		public FieldReference GetField(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}

			FieldReference value;
			fields.TryGetValue(name, out value);
			return value;
		}

		public Type GetClosedParameterType(Type parameter)
		{
			if (parameter.IsGenericTypeDefinition)
			{
				return parameter.GetGenericTypeDefinition().MakeGenericType(GetGenericArgumentsFor(parameter));
			}

			if (parameter.IsGenericType)
			{
				var arguments = parameter.GetGenericArguments();
				if (CloseGenericParametersIfAny(arguments))
				{
					return parameter.GetGenericTypeDefinition().MakeGenericType(arguments);
				}
			}

			if (parameter.IsGenericParameter)
			{
				return GetGenericArgument(parameter.Name);
			}

			if (parameter.IsArray)
			{
				var elementType = GetClosedParameterType(parameter.GetElementType());
				int rank = parameter.GetArrayRank();
				return rank == 1
					? elementType.MakeArrayType()
					: elementType.MakeArrayType(rank);
			}

			if (parameter.IsByRef)
			{
				var elementType = GetClosedParameterType(parameter.GetElementType());
				return elementType.MakeByRefType();
			}

			return parameter;

			bool CloseGenericParametersIfAny(Type[] arguments)
			{
				var hasAnyGenericParameters = false;
				for (var i = 0; i < arguments.Length; i++)
				{
					var newType = GetClosedParameterType(arguments[i]);
					if (newType != null && !ReferenceEquals(newType, arguments[i]))
					{
						arguments[i] = newType;
						hasAnyGenericParameters = true;
					}
				}
				return hasAnyGenericParameters;
			}
		}

		public Type GetGenericArgument(string genericArgumentName)
		{
			if (name2GenericType.TryGetValue(genericArgumentName, out var genericTypeParameterBuilder))
				return genericTypeParameterBuilder;

			return null;
		}

		public Type[] GetGenericArgumentsFor(Type genericType)
		{
			var types = new List<Type>();

			foreach (var genType in genericType.GetGenericArguments())
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
			var types = new List<Type>();
			foreach (var genType in genericMethod.GetGenericArguments())
			{
				types.Add(name2GenericType[genType.Name]);
			}

			return types.ToArray();
		}

		public void SetGenericTypeParameters(GenericTypeParameterBuilder[] genericTypeParameterBuilders)
		{
			genericTypeParams = genericTypeParameterBuilders;
		}

		protected Type CreateType(TypeBuilder type)
		{
			return type.CreateTypeInfo();
		}

		protected virtual void EnsureBuildersAreInAValidState()
		{
			if (!typebuilder.IsInterface && constructors.Count == 0)
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