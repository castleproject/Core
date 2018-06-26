// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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
	using System.Collections.Generic;
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	public abstract class AbstractCodeBuilder
	{
		private readonly ILGenerator generator;
		private readonly List<Reference> ilmarkers;
		private readonly List<IILEmitter> emitters;
		private bool isEmpty;

		protected AbstractCodeBuilder(ILGenerator generator)
		{
			this.generator = generator;
			emitters = new List<IILEmitter>();
			ilmarkers = new List<Reference>();
			isEmpty = true;
		}

		//NOTE: should we make this obsolete if no one is using it?
		public /*protected internal*/ ILGenerator Generator
		{
			get { return generator; }
		}

		internal bool IsEmpty
		{
			get { return isEmpty; }
		}

		public AbstractCodeBuilder AddExpression(Expression expression)
		{
			Add(expression);
			return this;
		}

		public AbstractCodeBuilder AddStatement(Statement stmt)
		{
			Add(stmt);
			return this;
		}

		internal void Add(IILEmitter emitter)
		{
			SetNonEmpty();
			emitters.Add(emitter);
		}

		public LocalReference DeclareLocal(Type type)
		{
			var local = new LocalReference(type);
			ilmarkers.Add(local);
			return local;
		}

		public /*protected internal*/ void SetNonEmpty()
		{
			isEmpty = false;
		}

		internal void Generate(IMemberEmitter member, ILGenerator il)
		{
			foreach (var local in ilmarkers)
			{
				local.Generate(il);
			}

			foreach (var emitter in emitters)
			{
				emitter.Emit(member, il);
			}
		}
	}
}