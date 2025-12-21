// Copyright 2004-2025 Castle Project - http://www.castleproject.org/
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

	internal sealed class ClassEmitter
	{
		private const MethodAttributes defaultAttributes =
			MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public;

		private readonly ModuleScope moduleScope;

		private readonly List<ConstructorEmitter> constructors;
		private readonly List<EventEmitter> events;

		private readonly IDictionary<string, FieldReference> fields =
			new Dictionary<string, FieldReference>(StringComparer.OrdinalIgnoreCase);

		private readonly List<MethodEmitter> methods;

		private readonly List<PropertyEmitter> properties;
		private readonly TypeBuilder typeBuilder;

		private GenericTypeParameterBuilder[] genericTypeParams;

		internal const TypeAttributes DefaultTypeAttributes =
			TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable;

		public ClassEmitter(TypeBuilder typeBuilder)
		{
			Debug.Assert(typeBuilder.IsClass, $"{nameof(ClassEmitter)} only supports class types.");

			this.typeBuilder = typeBuilder;
			methods = new List<MethodEmitter>();
			constructors = new List<ConstructorEmitter>();
			properties = new List<PropertyEmitter>();
			events = new List<EventEmitter>();
		}

		public ClassEmitter(ModuleScope moduleScope, string name, Type baseType, IEnumerable<Type> interfaces, TypeAttributes flags, bool forceUnsigned)
			: this(CreateTypeBuilder(moduleScope, name, baseType, interfaces, flags, forceUnsigned))
		{
			interfaces = InitializeGenericArgumentsFromBases(ref baseType, interfaces);

			if (interfaces != null)
			{
				foreach (var inter in interfaces)
				{
					if (inter.IsInterface)
					{
						TypeBuilder.AddInterfaceImplementation(inter);
					}
					else
					{
						Debug.Assert(inter.IsDelegateType());
					}
				}
			}

			TypeBuilder.SetParent(baseType);
			this.moduleScope = moduleScope;
		}

		public ClassEmitter(ModuleScope moduleScope, string name, Type baseType, IEnumerable<Type> interfaces)
			: this(moduleScope, name, baseType, interfaces, DefaultTypeAttributes, forceUnsigned: false)
		{
		}

		private static IEnumerable<Type> InitializeGenericArgumentsFromBases(ref Type baseType, IEnumerable<Type> interfaces)
		{
			if (baseType != null && baseType.IsGenericTypeDefinition)
			{
				throw new NotSupportedException("ClassEmitter does not support open generic base types. Type: " + baseType.FullName);
			}

			if (interfaces == null)
			{
				return interfaces;
			}

			foreach (var inter in interfaces)
			{
				if (inter.IsGenericTypeDefinition)
				{
					throw new NotSupportedException("ClassEmitter does not support open generic interfaces. Type: " + inter.FullName);
				}
			}
			return interfaces;
		}

		private static TypeBuilder CreateTypeBuilder(ModuleScope moduleScope, string name, Type baseType,
													 IEnumerable<Type> interfaces,
													 TypeAttributes flags, bool forceUnsigned)
		{
			var isAssemblySigned = !forceUnsigned && !StrongNameUtil.IsAnyTypeFromUnsignedAssembly(baseType, interfaces);
			return moduleScope.DefineType(isAssemblySigned, name, flags);
		}

		public Type BaseType
		{
			get
			{
				return TypeBuilder.BaseType;
			}
		}

		public ConstructorEmitter ClassConstructor { get; private set; }

		internal bool InStrongNamedModule
		{
			get { return StrongNameUtil.IsAssemblySigned(TypeBuilder.Assembly); }
		}

		public GenericTypeParameterBuilder[] GenericTypeParams
		{
			get { return genericTypeParams; }
		}

		public ModuleScope ModuleScope
		{
			get { return moduleScope; }
		}

		public TypeBuilder TypeBuilder
		{
			get { return typeBuilder; }
		}

		public void AddCustomAttributes(IEnumerable<CustomAttributeInfo> additionalAttributes)
		{
			foreach (var attribute in additionalAttributes)
			{
				typeBuilder.SetCustomAttribute(attribute.Builder);
			}
		}

		public Type BuildType()
		{
			EnsureBuildersAreInAValidState();

			var type = CreateType(typeBuilder);

			return type;
		}

		public void CopyGenericParametersFromMethod(MethodInfo methodToCopyGenericsFrom)
		{
			// big sanity check
			if (genericTypeParams != null)
			{
				throw new InvalidOperationException("Cannot invoke me twice");
			}

			SetGenericTypeParameters(GenericUtil.CopyGenericArguments(methodToCopyGenericsFrom, typeBuilder));
		}

		public ConstructorEmitter CreateConstructor(params ArgumentReference[] arguments)
		{
			var member = new ConstructorEmitter(this, arguments);
			constructors.Add(member);
			return member;
		}

		public void CreateDefaultConstructor()
		{
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
			var fieldBuilder = typeBuilder.DefineField(name, fieldType, atts);
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
			var member = new ConstructorEmitter(this, TypeBuilder.DefineTypeInitializer());
			constructors.Add(member);
			ClassConstructor = member;
			return member;
		}

		public void DefineCustomAttribute(CustomAttributeBuilder attribute)
		{
			typeBuilder.SetCustomAttribute(attribute);
		}

		public void DefineCustomAttribute<TAttribute>(object[] constructorArguments) where TAttribute : Attribute
		{
			var customAttributeInfo = AttributeUtil.CreateInfo(typeof(TAttribute), constructorArguments);
			typeBuilder.SetCustomAttribute(customAttributeInfo.Builder);
		}

		public void DefineCustomAttribute<TAttribute>() where TAttribute : Attribute, new()
		{
			var customAttributeInfo = AttributeUtil.CreateInfo<TAttribute>();
			typeBuilder.SetCustomAttribute(customAttributeInfo.Builder);
		}

		public void DefineCustomAttributeFor<TAttribute>(FieldReference field) where TAttribute : Attribute, new()
		{
			var customAttributeInfo = AttributeUtil.CreateInfo<TAttribute>();
			var fieldBuilder = field.FieldBuilder;
			if (fieldBuilder == null)
			{
				throw new ArgumentException(
					"Invalid field reference.This reference does not point to field on type being generated", nameof(field));
			}
			fieldBuilder.SetCustomAttribute(customAttributeInfo.Builder);
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
			if (parameter.IsGenericType)
			{
				// ECMA-335 section II.9.4: "The CLI does not support partial instantiation
				// of generic types. And generic types shall not appear uninstantiated any-
				// where in metadata signature blobs." (And parameters are defined there!)
				Debug.Assert(parameter.IsGenericTypeDefinition == false);

				var arguments = parameter.GetGenericArguments();
				if (CloseGenericParametersIfAny(arguments))
				{
					return parameter.GetGenericTypeDefinition().MakeGenericType(arguments);
				}
			}

			if (parameter.IsGenericParameter)
			{
				return GetGenericArgument(parameter.GenericParameterPosition);
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

		public Type GetGenericArgument(int position)
		{
			Debug.Assert(0 <= position && position < genericTypeParams.Length);

			return genericTypeParams[position];
		}

		public Type[] GetGenericArgumentsFor(MethodInfo genericMethod)
		{
			Debug.Assert(genericMethod.GetGenericArguments().Length == genericTypeParams.Length);

			return genericTypeParams;
		}

		public void SetGenericTypeParameters(GenericTypeParameterBuilder[] genericTypeParameterBuilders)
		{
			genericTypeParams = genericTypeParameterBuilders;
		}

		public Type CreateType(TypeBuilder type)
		{
			return type.CreateTypeInfo();
		}

		protected void EnsureBuildersAreInAValidState()
		{
			if (constructors.Count == 0)
			{
				CreateDefaultConstructor();
			}

			foreach (var builder in properties)
			{
				builder.Generate();
			}
			foreach (var builder in events)
			{
				builder.Generate();
			}
			foreach (var builder in constructors)
			{
				builder.Generate();
			}
			foreach (var builder in methods)
			{
				builder.Generate();
			}
		}
	}
}