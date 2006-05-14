// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Builder.CodeBuilder
{
	using System;
	using System.Reflection;
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;
	using Castle.DynamicProxy.Builder.CodeBuilder.Utils;

	/// <summary>
	/// Summary description for ConstructorCodeBuilder.
	/// </summary>
	[CLSCompliant(false)]
	public class ConstructorCodeBuilder : AbstractCodeBuilder
	{
		private Type _baseType;

		public ConstructorCodeBuilder(Type baseType, ILGenerator generator) : base(generator)
		{
			_baseType = baseType;
		}

		public void InvokeBaseConstructor()
		{
			InvokeBaseConstructor(ObtainAvailableConstructor());
		}

		internal void InvokeBaseConstructor(ConstructorInfo constructor)
		{
			AddStatement(
				new ExpressionStatement(
					new ConstructorInvocationExpression(constructor)));
		}

		internal void InvokeBaseConstructor(ConstructorInfo constructor, params ArgumentReference[] arguments)
		{
			AddStatement(
				new ExpressionStatement(
					new ConstructorInvocationExpression(constructor,
					                                    ArgumentsUtil.ConvertArgumentReferenceToExpression(arguments))));
		}

		internal ConstructorInfo ObtainAvailableConstructor()
		{
			return _baseType.GetConstructor(
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
					null, new Type[0], null);
		}
	}
}