﻿// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Generators.Emitters.SimpleAST
{
	using Castle.DynamicProxy.Internal;
	using System;
	using System.Reflection;
	using System.Reflection.Emit;

	public class BindDelegateExpression : Expression
	{
		private readonly ConstructorInfo delegateCtor;
		private readonly MethodInfo methodToBindTo;
		private readonly Expression owner;

		public BindDelegateExpression(Type @delegate, Expression owner, MethodInfo methodToBindTo,
		                              GenericTypeParameterBuilder[] genericTypeParams)
		{
			delegateCtor = @delegate.GetConstructors()[0];
			this.methodToBindTo = methodToBindTo;
			if (@delegate.GetTypeInfo().IsGenericTypeDefinition)

			{
#if NETCORE
				var genericTypeParameters = genericTypeParams.AsTypeArray();
				var closedDelegate = @delegate.MakeGenericType(genericTypeParameters);
				delegateCtor = TypeBuilder.GetConstructor(closedDelegate, delegateCtor);
				this.methodToBindTo = methodToBindTo.MakeGenericMethod(genericTypeParameters);
#else
				var closedDelegate = @delegate.MakeGenericType(genericTypeParams);
				delegateCtor = TypeBuilder.GetConstructor(closedDelegate, delegateCtor);
				this.methodToBindTo = methodToBindTo.MakeGenericMethod(genericTypeParams);
#endif
			}
			this.owner = owner;
		}

		public override void Emit(IMemberEmitter member, ILGenerator gen)
		{
			owner.Emit(member, gen);
			gen.Emit(OpCodes.Dup);
			if (methodToBindTo.IsFinal)
			{
				gen.Emit(OpCodes.Ldftn, methodToBindTo);
			}
			else
			{
				gen.Emit(OpCodes.Ldvirtftn, methodToBindTo);
			}
			gen.Emit(OpCodes.Newobj, delegateCtor);
		}
	}
}