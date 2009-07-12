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

	public class EventEmitter : IMemberEmitter
	{
		private readonly AbstractTypeEmitter typeEmitter;
		private Type type;
		private string name;
		private EventBuilder eventBuilder;
		private MethodEmitter addMethod;
		private MethodEmitter removeMethod;

		public EventEmitter(AbstractTypeEmitter typeEmitter, string name, EventAttributes attributes, Type type)
		{
			this.typeEmitter = typeEmitter;
			this.type = type;
			this.name = name;
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

		public void Generate()
		{
			MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName;
			if (addMethod == null)
				CreateAddMethod(methodAttributes);
			if (removeMethod == null)
				CreateRemoveMethod(methodAttributes);

			addMethod.Generate();
			removeMethod.Generate();

			eventBuilder.SetAddOnMethod(addMethod.MethodBuilder);
			eventBuilder.SetRemoveOnMethod(removeMethod.MethodBuilder);
		}

		public MethodEmitter CreateAddMethod(MethodAttributes atts)
		{
			return CreateAddMethod("add_" + name, atts);
		}

		public MethodEmitter CreateAddMethod(string addMethodName, MethodAttributes atts)
		{
			if (addMethod != null)
			{
				throw new InvalidOperationException("An add method exists");
			}
			addMethod = typeEmitter.CreateMethod(addMethodName, atts);
			return addMethod;
		}

		public MethodEmitter CreateRemoveMethod(MethodAttributes atts)
		{
			return CreateRemoveMethod("remove_" + name, atts);
		}

		public void EnsureValidCodeBlock()
		{
			addMethod.EnsureValidCodeBlock();
			removeMethod.EnsureValidCodeBlock();
		}

		public MethodEmitter CreateRemoveMethod(string removeMethodName, MethodAttributes attributes)
		{

			if (removeMethod != null)
			{
				throw new InvalidOperationException("A remove method exists");
			}
			removeMethod = typeEmitter.CreateMethod(removeMethodName, attributes);
			return removeMethod;
		}
	}
}