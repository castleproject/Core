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

namespace Castle.DynamicProxy.Generators
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Generators.Emitters;

	internal class MetaProperty : MetaTypeElement, IEquatable<MetaProperty>
	{
		private readonly Type[] arguments;
		private readonly PropertyAttributes attributes;
		private readonly IEnumerable<CustomAttributeBuilder> customAttributes;
		private readonly MetaMethod getter;
		private readonly MetaMethod setter;
		private PropertyEmitter emitter;

		public MetaProperty(PropertyInfo property, MetaMethod getter, MetaMethod setter,
		                    IEnumerable<CustomAttributeBuilder> customAttributes, Type[] arguments)
			: base(property)
		{
			this.getter = getter;
			this.setter = setter;
			attributes = PropertyAttributes.None;
			this.customAttributes = customAttributes;
			this.arguments = arguments ?? Type.EmptyTypes;
		}

		public Type[] Arguments
		{
			get { return arguments; }
		}

		public bool CanRead
		{
			get { return getter != null; }
		}

		public bool CanWrite
		{
			get { return setter != null; }
		}

		public PropertyEmitter Emitter
		{
			get
			{
				if (emitter == null)
				{
					throw new InvalidOperationException(
						"Emitter is not initialized. You have to initialize it first using 'BuildPropertyEmitter' method");
				}
				return emitter;
			}
		}

		public MethodInfo GetMethod
		{
			get
			{
				if (!CanRead)
				{
					throw new InvalidOperationException();
				}
				return getter.Method;
			}
		}

		public MetaMethod Getter
		{
			get { return getter; }
		}

		public MethodInfo SetMethod
		{
			get
			{
				if (!CanWrite)
				{
					throw new InvalidOperationException();
				}
				return setter.Method;
			}
		}

		public MetaMethod Setter
		{
			get { return setter; }
		}

		private Type Type
		{
			get { return ((PropertyInfo)Member).PropertyType; }
		}

		public void BuildPropertyEmitter(ClassEmitter classEmitter)
		{
			if (emitter != null)
			{
				throw new InvalidOperationException("Emitter is already created. It is illegal to invoke this method twice.");
			}

			emitter = classEmitter.CreateProperty(Name, attributes, Type, arguments);
			foreach (var attribute in customAttributes)
			{
				emitter.DefineCustomAttribute(attribute);
			}
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != typeof(MetaProperty))
			{
				return false;
			}
			return Equals((MetaProperty)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((GetMethod != null ? GetMethod.GetHashCode() : 0)*397) ^ (SetMethod != null ? SetMethod.GetHashCode() : 0);
			}
		}

		public bool Equals(MetaProperty other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			if (!Type.Equals(other.Type))
			{
				return false;
			}

			if (!StringComparer.OrdinalIgnoreCase.Equals(Name, other.Name))
			{
				return false;
			}
			if (Arguments.Length != other.Arguments.Length)
			{
				return false;
			}
			for (var i = 0; i < Arguments.Length; i++)
			{
				if (Arguments[i].Equals(other.Arguments[i]) == false)
				{
					return false;
				}
			}

			return true;
		}

		public override void SwitchToExplicitImplementation()
		{
			SwitchToExplicitImplementationName();
			if (setter != null)
			{
				setter.SwitchToExplicitImplementation();
			}
			if (getter != null)
			{
				getter.SwitchToExplicitImplementation();
			}
		}
	}
}