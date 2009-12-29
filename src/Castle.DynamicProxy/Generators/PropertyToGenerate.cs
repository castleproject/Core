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

namespace Castle.DynamicProxy.Generators
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Reflection;
	using System.Reflection.Emit;
	using Castle.DynamicProxy.Generators.Emitters;

	public class PropertyToGenerate
	{
		private readonly string name;
		private readonly Type type;
		private readonly MethodToGenerate getter;
		private readonly MethodToGenerate setter;
		private readonly PropertyAttributes attributes;
		private readonly IEnumerable<CustomAttributeBuilder> customAttributes;
		private PropertyEmitter emitter;

		public PropertyToGenerate(string name, Type type, MethodToGenerate getter, MethodToGenerate setter, PropertyAttributes attributes, IEnumerable<CustomAttributeBuilder> customAttributes)
		{
			this.name = GetName(name,getter,setter);
			this.type = type;
			this.getter = getter;
			this.setter = setter;
			this.attributes = attributes;
			this.customAttributes = customAttributes;
		}

		private string GetName(string name, MethodToGenerate getter, MethodToGenerate setter)
		{
			Type declaringType = null;
			if (getter != null)
			{
				declaringType = getter.Method.DeclaringType;
			}
			else if (setter != null)
			{
				declaringType = setter.Method.DeclaringType;
			}

			Debug.Assert(declaringType != null);
			if (!declaringType.IsInterface)
			{
				return name;
			}
			return string.Format("{0}.{1}", declaringType, name);
		}

		public bool CanRead
		{
			get { return getter != null; }
		}

		public bool CanWrite
		{
			get { return setter != null; }
		}

		public MethodInfo GetMethod
		{
			get
			{
				if(!CanRead)
				{
					throw new InvalidOperationException();
				}
				return getter.Method;
			}
		}

		public MethodInfo SetMethod
		{
			get
			{
				if(!CanWrite)
				{
					throw new InvalidOperationException();
				}
				return setter.Method;
			}
		}

		public PropertyEmitter Emitter
		{
			get {
				if (emitter == null)
					throw new InvalidOperationException(
						"Emitter is not initialized. You have to initialize it first using 'BuildPropertyEmitter' method");
				return emitter;
			}
		}

		public MethodToGenerate Getter
		{
			get { return getter; }
		}

		public MethodToGenerate Setter
		{
			get { return setter; }
		}

		public bool Equals(PropertyToGenerate other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other.GetMethod, GetMethod) && Equals(other.SetMethod, SetMethod);
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
			if (obj.GetType() != typeof(PropertyToGenerate))
			{
				return false;
			}
			return Equals((PropertyToGenerate) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((GetMethod != null ? GetMethod.GetHashCode() : 0) * 397) ^ (SetMethod != null ? SetMethod.GetHashCode() : 0);
			}
		}

		public void BuildPropertyEmitter(ClassEmitter classEmitter)
		{
			if (emitter != null)
				throw new InvalidOperationException("Emitter is already created. It is illegal to invoke this method twice.");

			emitter = classEmitter.CreateProperty(name, attributes, type);
			foreach (var attribute in customAttributes)
			{
				emitter.DefineCustomAttribute(attribute);
			}
		}
	}
}