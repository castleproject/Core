// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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

	public class PropertyEmitter : IMemberEmitter
	{
		private readonly PropertyBuilder builder;
		private readonly AbstractTypeEmitter parentTypeEmitter;
		private MethodEmitter getMethod;
		private MethodEmitter setMethod;

		public PropertyEmitter(AbstractTypeEmitter parentTypeEmitter, string name, PropertyAttributes attributes,
		                       Type propertyType, Type[] arguments)
		{
			this.parentTypeEmitter = parentTypeEmitter;

			builder = parentTypeEmitter.TypeBuilder.DefineProperty(
				name, attributes, CallingConventions.HasThis, propertyType,
				null, null, arguments, null, null);
		}

		public MemberInfo Member
		{
			get { return null; }
		}

		public Type ReturnType
		{
			get { return builder.PropertyType; }
		}

		public MethodEmitter CreateGetMethod(string name, MethodAttributes attrs, MethodInfo methodToOverride,
		                                     params Type[] parameters)
		{
			if (getMethod != null)
			{
				throw new InvalidOperationException("A get method exists");
			}

			getMethod = new MethodEmitter(parentTypeEmitter, name, attrs, methodToOverride);
			return getMethod;
		}

		public MethodEmitter CreateGetMethod(string name, MethodAttributes attributes, MethodInfo methodToOverride)
		{
			return CreateGetMethod(name, attributes, methodToOverride, Type.EmptyTypes);
		}

		public MethodEmitter CreateSetMethod(string name, MethodAttributes attrs, MethodInfo methodToOverride,
		                                     params Type[] parameters)
		{
			if (setMethod != null)
			{
				throw new InvalidOperationException("A set method exists");
			}

			setMethod = new MethodEmitter(parentTypeEmitter, name, attrs, methodToOverride);
			return setMethod;
		}

		public MethodEmitter CreateSetMethod(string name, MethodAttributes attributes, MethodInfo methodToOverride)
		{
			var method = CreateSetMethod(name, attributes, methodToOverride, Type.EmptyTypes);
			return method;
		}

		public void DefineCustomAttribute(CustomAttributeBuilder attribute)
		{
			builder.SetCustomAttribute(attribute);
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
	}
}