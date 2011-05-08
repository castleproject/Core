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

namespace Castle.DynamicProxy.Generators.Emitters.CodeBuilders
{
	using System;
	using System.Reflection;
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	public class ConstructorCodeBuilder : AbstractCodeBuilder
	{
		private readonly Type baseType;

		public ConstructorCodeBuilder(Type baseType, ILGenerator generator) : base(generator)
		{
			this.baseType = baseType;
		}

		public void InvokeBaseConstructor()
		{
			var type = baseType;
			if (type.ContainsGenericParameters)
			{
				type = type.GetGenericTypeDefinition();
					// need to get generic type definition, otherwise the GetConstructor method might throw NotSupportedException
			}

			var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			var baseDefaultCtor = type.GetConstructor(flags, null, new Type[0], null);

			InvokeBaseConstructor(baseDefaultCtor);
		}

		public void InvokeBaseConstructor(ConstructorInfo constructor)
		{
			AddStatement(new ConstructorInvocationStatement(constructor));
		}

		public void InvokeBaseConstructor(ConstructorInfo constructor, params ArgumentReference[] arguments)
		{
			AddStatement(
				new ConstructorInvocationStatement(constructor,
				                                   ArgumentsUtil.ConvertArgumentReferenceToExpression(arguments)));
		}
	}
}