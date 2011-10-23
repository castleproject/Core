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

	public class EventEmitter : IMemberEmitter
	{
		private readonly EventBuilder eventBuilder;
		private readonly Type type;
		private readonly AbstractTypeEmitter typeEmitter;
		private MethodEmitter addMethod;
		private MethodEmitter removeMethod;

		public EventEmitter(AbstractTypeEmitter typeEmitter, string name, EventAttributes attributes, Type type)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			this.typeEmitter = typeEmitter;
			this.type = type;
			eventBuilder = typeEmitter.TypeBuilder.DefineEvent(name, attributes, type);
		}

		public MemberInfo Member
		{
			get { return null; }
		}

		public Type ReturnType
		{
			get { return type; }
		}

		public MethodEmitter CreateAddMethod(string addMethodName, MethodAttributes attributes, MethodInfo methodToOverride)
		{
			if (addMethod != null)
			{
				throw new InvalidOperationException("An add method exists");
			}

			addMethod = new MethodEmitter(typeEmitter, addMethodName, attributes, methodToOverride);
			return addMethod;
		}

		public MethodEmitter CreateRemoveMethod(string removeMethodName, MethodAttributes attributes,
		                                        MethodInfo methodToOverride)
		{
			if (removeMethod != null)
			{
				throw new InvalidOperationException("A remove method exists");
			}
			removeMethod = new MethodEmitter(typeEmitter, removeMethodName, attributes, methodToOverride);
			return removeMethod;
		}

		public void EnsureValidCodeBlock()
		{
			addMethod.EnsureValidCodeBlock();
			removeMethod.EnsureValidCodeBlock();
		}

		public void Generate()
		{
			if (addMethod == null)
			{
				throw new InvalidOperationException("Event add method was not created");
			}
			if (removeMethod == null)
			{
				throw new InvalidOperationException("Event remove method was not created");
			}
			addMethod.Generate();
			eventBuilder.SetAddOnMethod(addMethod.MethodBuilder);

			removeMethod.Generate();
			eventBuilder.SetRemoveOnMethod(removeMethod.MethodBuilder);
		}
	}
}