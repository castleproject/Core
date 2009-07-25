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
	using System.Reflection;
	using Castle.DynamicProxy.Generators.Emitters;

	public class EventToGenerate
	{
		private readonly string name;
		private readonly Type type;
		private EventEmitter emitter;
		private EventAttributes attributes;
		private readonly IProxyMethod adder;
		private readonly IProxyMethod remover;

		public bool Equals(EventToGenerate other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other.adder.Method, adder.Method) && Equals(other.remover.Method, remover.Method) && Equals(other.Attributes, Attributes);
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
			if (obj.GetType() != typeof(EventToGenerate))
			{
				return false;
			}
			return Equals((EventToGenerate) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int result = (adder.Method != null ? adder.Method.GetHashCode() : 0);
				result = (result * 397) ^ (remover.Method != null ? remover.Method.GetHashCode() : 0);
				result = (result * 397) ^ Attributes.GetHashCode();
				return result;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EventToGenerate"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="addMethod">The add method.</param>
		/// <param name="removeMethod">The remove method.</param>
		/// <param name="attributes">The attributes.</param>
		/// <param name="hasTarget"></param>
		public EventToGenerate(string name, Type type, MethodInfo addMethod, MethodInfo removeMethod, EventAttributes attributes, bool hasTarget)
		{
			this.name = name;
			this.type = type;
			this.adder = new ProxyMethod(addMethod, hasTarget);
			this.remover = new ProxyMethod(removeMethod, hasTarget);
			this.attributes = attributes;
		}

		public EventAttributes Attributes
		{
			get { return attributes; }
			set { attributes = value; }
		}

		public EventEmitter Emitter
		{
			get {
				if(emitter==null)
					throw new InvalidOperationException("Emitter is not initialized. You have to initialize it first using 'BuildEventEmitter' method");
				return emitter;
			}
		}

		public MethodInfo AddMethod
		{
			get { return adder.Method; }

		}

		public MethodInfo RemoveMethod
		{
			get { return remover.Method; }

		}
		public IProxyMethod Adder
		{
			get {
				return adder;
			}
		}

		public IProxyMethod Remover
		{
			get {
				return remover;
			}
		}

		public void BuildEventEmitter(ClassEmitter classEmitter)
		{
			if(emitter!=null)
				throw new InvalidOperationException();

			emitter = classEmitter.CreateEvent(name, Attributes, type);
		}
	}
}