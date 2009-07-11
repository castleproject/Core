// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
	using Castle.DynamicProxy.Tokens;

	public class PropertyEmitter : IMemberEmitter
	{
		private PropertyBuilder builder;
		private AbstractTypeEmitter parentTypeEmitter;
		private MethodEmitter getMethod;
		private MethodEmitter setMethod;

		// private ParameterInfo[] indexParameters;

		private delegate PropertyBuilder DefineProperty_Clr2_0(
			string name, PropertyAttributes attributes, Type propertyType, Type[] parameters);

		public delegate PropertyBuilder DefineProperty_Clr_2_0_SP1(string name,
		                                                           PropertyAttributes attributes,
		                                                           CallingConventions callingConvention,
		                                                           Type returnType,
		                                                           Type[] returnTypeRequiredCustomModifiers,
		                                                           Type[] returnTypeOptionalCustomModifiers,
		                                                           Type[] parameterTypes,
		                                                           Type[][] parameterTypeRequiredCustomModifiers,
		                                                           Type[][] parameterTypeOptionalCustomModifiers);

		public PropertyEmitter(AbstractTypeEmitter parentTypeEmitter, String name, PropertyAttributes attributes,
		                       Type propertyType)
		{
			this.parentTypeEmitter = parentTypeEmitter;

			// DYNPROXY-73 - AmbiguousMatchException for properties
			// This is a workaround for a framework limitation in CLR 2.0 
			// This limitation was removed in CLR 2.0 SP1, but we don't want to 
			// tie ourselves to that version. This perform the lookup for the new overload
			// dynamically, so we have a nice fallback on vanilla CLR 2.0

			if (TypeBuilderMethods.DefineProperty == null)
			{
				DefineProperty_Clr2_0 oldDefineProperty = parentTypeEmitter.TypeBuilder.DefineProperty;
				builder = oldDefineProperty(name, attributes, propertyType, new Type[0]);
			}
			else
			{
				DefineProperty_Clr_2_0_SP1 newDefinedProperty = (DefineProperty_Clr_2_0_SP1)
				                                                Delegate.CreateDelegate(typeof (DefineProperty_Clr_2_0_SP1),
				                                                                        parentTypeEmitter.TypeBuilder,
				                                                                        TypeBuilderMethods.DefineProperty);
				builder = newDefinedProperty(
					name, attributes, CallingConventions.HasThis, propertyType,
					null, null, new Type[0], null, null);
			}
		}

		public MethodEmitter GetMethod
		{
			get { return getMethod; }
			set { getMethod = value; }
		}

		public MethodEmitter SetMethod
		{
			get { return setMethod; }
			set { setMethod = value; }
		}

		public MethodEmitter CreateGetMethod()
		{
			return CreateGetMethod(MethodAttributes.Public |
			                       MethodAttributes.Virtual |
			                       MethodAttributes.SpecialName);
		}

		public MethodEmitter CreateGetMethod(MethodAttributes attrs, params Type[] parameters)
		{
			return CreateGetMethod("get_" + builder.Name, attrs, parameters);
		}

		public MethodEmitter CreateGetMethod(string name, MethodAttributes attrs, params Type[] parameters)
		{
			if (getMethod != null)
			{
				throw new InvalidOperationException("A getMethod exists");
			}

			if (parameters.Length == 0)
			{
				getMethod = new MethodEmitter(parentTypeEmitter, name, attrs);
			}
			else
			{
				getMethod = new MethodEmitter(parentTypeEmitter, name,
				                              attrs,
				                              ReturnType,
				                              parameters);
			}

			return getMethod;
		}

		public MethodEmitter CreateSetMethod()
		{
			return CreateSetMethod(MethodAttributes.Public |
			                       MethodAttributes.Virtual |
			                       MethodAttributes.SpecialName);
		}

		public MethodEmitter CreateSetMethod(Type arg)
		{
			return CreateSetMethod(MethodAttributes.Public |
			                       MethodAttributes.Virtual |
			                       MethodAttributes.SpecialName, arg);
		}

		public MethodEmitter CreateSetMethod(MethodAttributes attrs, params Type[] parameters)
		{
			return CreateSetMethod("set_" + builder.Name, attrs, parameters);
		}

		public MethodEmitter CreateSetMethod(string name, MethodAttributes attrs, params Type[] parameters)
		{
			if (setMethod != null)
			{
				throw new InvalidOperationException("A setMethod exists");
			}

			if (parameters.Length == 0)
			{
				setMethod = new MethodEmitter(parentTypeEmitter, name, attrs);
			}
			else
			{
				setMethod = new MethodEmitter(parentTypeEmitter, name,
				                              attrs, typeof (void),
				                              parameters);
			}

			return setMethod;
		}

		public MemberInfo Member
		{
			get { return null; }
		}

		public Type ReturnType
		{
			get { return builder.PropertyType; }
		}

		public void Generate()
		{
			if (setMethod != null)
			{
				setMethod.Generate();
				builder.SetSetMethod(setMethod.MethodBuilder);
			}

			if (getMethod != null)
			{
				getMethod.Generate();
				builder.SetGetMethod(getMethod.MethodBuilder);
			}
		}

		public void EnsureValidCodeBlock()
		{
			if (setMethod != null)
			{
				setMethod.EnsureValidCodeBlock();
			}

			if (getMethod != null)
			{
				getMethod.EnsureValidCodeBlock();
			}
		}

		public void DefineCustomAttribute(Attribute attribute)
		{
			CustomAttributeBuilder customAttributeBuilder = CustomAttributeUtil.CreateCustomAttribute(attribute);

			if (customAttributeBuilder == null)
			{
				return;
			}

			builder.SetCustomAttribute(customAttributeBuilder);
		}
	}
}