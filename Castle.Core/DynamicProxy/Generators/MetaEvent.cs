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

namespace Castle.DynamicProxy.Generators
{
	using System;
	using System.Reflection;

	using Castle.DynamicProxy.Generators.Emitters;

	public class MetaEvent : MetaTypeElement, IEquatable<MetaEvent>
	{
		private readonly MetaMethod adder;
		private readonly MetaMethod remover;
		private readonly Type type;
		private EventEmitter emitter;
		private string name;

		/// <summary>
		///   Initializes a new instance of the <see cref = "MetaEvent" /> class.
		/// </summary>
		/// <param name = "name">The name.</param>
		/// <param name = "declaringType">Type declaring the original event being overriten, or null.</param>
		/// <param name = "eventDelegateType"></param>
		/// <param name = "adder">The add method.</param>
		/// <param name = "remover">The remove method.</param>
		/// <param name = "attributes">The attributes.</param>
		public MetaEvent(string name, Type declaringType, Type eventDelegateType, MetaMethod adder, MetaMethod remover,
		                 EventAttributes attributes)
			: base(declaringType)
		{
			if (adder == null)
			{
				throw new ArgumentNullException("adder");
			}
			if (remover == null)
			{
				throw new ArgumentNullException("remover");
			}
			this.name = name;
			type = eventDelegateType;
			this.adder = adder;
			this.remover = remover;
			Attributes = attributes;
		}

		public MetaMethod Adder
		{
			get { return adder; }
		}

		public EventAttributes Attributes { get; private set; }

		public EventEmitter Emitter
		{
			get
			{
				if (emitter != null)
				{
					return emitter;
				}

				throw new InvalidOperationException(
					"Emitter is not initialized. You have to initialize it first using 'BuildEventEmitter' method");
			}
		}

		public MetaMethod Remover
		{
			get { return remover; }
		}

		public void BuildEventEmitter(ClassEmitter classEmitter)
		{
			if (emitter != null)
			{
				throw new InvalidOperationException();
			}
			emitter = classEmitter.CreateEvent(name, Attributes, type);
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
			if (obj.GetType() != typeof(MetaEvent))
			{
				return false;
			}
			return Equals((MetaEvent)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var result = (adder.Method != null ? adder.Method.GetHashCode() : 0);
				result = (result*397) ^ (remover.Method != null ? remover.Method.GetHashCode() : 0);
				result = (result*397) ^ Attributes.GetHashCode();
				return result;
			}
		}

		public bool Equals(MetaEvent other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			if (!type.Equals(other.type))
			{
				return false;
			}

			if (!StringComparer.OrdinalIgnoreCase.Equals(name, other.name))
			{
				return false;
			}

			return true;
		}

		internal override void SwitchToExplicitImplementation()
		{
			name = string.Format("{0}.{1}", sourceType.Name, name);
			adder.SwitchToExplicitImplementation();
			remover.SwitchToExplicitImplementation();
		}
	}
}