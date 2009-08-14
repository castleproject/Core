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
	using System.Diagnostics;
	using System.Reflection;
	using Castle.DynamicProxy.Generators.Emitters;

	public class EventToGenerate
	{
		private readonly string name;
		private readonly Type type;
		private EventEmitter emitter;
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
		/// <param name="adder">The add method.</param>
		/// <param name="remover">The remove method.</param>
		/// <param name="attributes">The attributes.</param>
		public EventToGenerate(string name, Type type, IProxyMethod adder, IProxyMethod remover, EventAttributes attributes)
		{
			this.name = GetName(name, adder, remover);
			this.type = type;
			this.adder = adder;
			this.remover = remover;
			this.Attributes = attributes;
		}

		private string GetName(string name, IProxyMethod adder, IProxyMethod remover)
		{
			Type declaringType = null;
			if (adder != null)
			{
				declaringType = adder.Method.DeclaringType;
			}
			else if (remover != null)
			{
				declaringType = remover.Method.DeclaringType;
			}

			Debug.Assert(declaringType != null);
			if (!declaringType.IsInterface)
			{
				return name;
			}
			return string.Format("{0}.{1}", declaringType, name);
		}

		public EventAttributes Attributes { get; private set; }

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