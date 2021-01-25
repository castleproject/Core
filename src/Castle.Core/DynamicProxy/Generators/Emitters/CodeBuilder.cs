// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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
	using System.Collections.Generic;
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	internal sealed class CodeBuilder
	{
		private readonly List<LocalReference> locals;
		private readonly List<IStatement> statements;
		private bool isEmpty;

		public CodeBuilder()
		{
			statements = new List<IStatement>();
			locals = new List<LocalReference>();
			isEmpty = true;
		}

		internal bool IsEmpty
		{
			get { return isEmpty; }
		}

		public CodeBuilder AddExpression(IExpression expression)
		{
			return AddStatement(new ExpressionStatement(expression));
		}

		public CodeBuilder AddStatement(IStatement statement)
		{
			isEmpty = false;
			statements.Add(statement);
			return this;
		}

		public LocalReference DeclareLocal(Type type)
		{
			var local = new LocalReference(type);
			locals.Add(local);
			return local;
		}

		internal void Generate(IMemberEmitter member, ILGenerator il)
		{
			foreach (var local in locals)
			{
				local.Generate(il);
			}

			foreach (var statement in statements)
			{
				statement.Emit(member, il);
			}
		}
	}
}