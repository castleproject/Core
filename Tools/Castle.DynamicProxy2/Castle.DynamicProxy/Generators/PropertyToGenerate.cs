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
	using System.Reflection;
	using Castle.DynamicProxy.Generators.Emitters;

	public class PropertyToGenerate
	{
		private readonly string name;
		private readonly Type type;
		private readonly bool canRead;
		private readonly bool canWrite;
		private PropertyEmitter emitter;
		private readonly PropertyAttributes attributes;
		private readonly IEnumerable<Attribute> customAttributes;
		private IProxyMethod getter;
		private ProxyMethod setter;

		public PropertyToGenerate(string name, Type type, bool canRead, bool canWrite, MethodInfo getMethod, MethodInfo setMethod, PropertyAttributes attributes, IEnumerable<Attribute> customAttributes, bool hasTarget)
		{
			this.name = name;
			this.type = type;
			this.canRead = canRead;
			this.canWrite = canWrite;
			this.getter = new ProxyMethod(getMethod, hasTarget);
			this.setter = new ProxyMethod(setMethod, hasTarget);
			this.attributes = attributes;
			this.customAttributes = customAttributes;
		}

		public bool CanRead
		{
			get { return canRead; }
		}

		public bool CanWrite
		{
			get { return canWrite; }
		}

		public MethodInfo GetMethod
		{
			get { return getter.Method; }
		}

		public MethodInfo SetMethod
		{
			get { return setter.Method; }
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

		public IProxyMethod Getter
		{
			get { return getter; }
		}

		public IProxyMethod Setter
		{
			get {
				return setter;
			}
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
			if(emitter!=null)
				throw new InvalidOperationException();

			emitter = classEmitter.CreateProperty(name, attributes, type);
			foreach(var attribute in customAttributes)
				emitter.DefineCustomAttribute(attribute);
		}
	}
}