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

namespace Castle.DynamicProxy.Generators.Emitters.CodeBuilders
{
	using System;
	using System.Collections;
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	public abstract class AbstractCodeBuilder
	{
		private bool isEmpty;
		private ILGenerator generator;
		private ArrayList stmts;
		private ArrayList ilmarkers;

		protected AbstractCodeBuilder(ILGenerator generator)
		{
			this.generator = generator;
			stmts = new ArrayList();
			ilmarkers = new ArrayList();
			isEmpty = true;
		}

		public /*protected internal*/ ILGenerator Generator
		{
			get { return generator; }
		}

		public void AddStatement(Statement stmt)
		{
			SetNonEmpty();
			stmts.Add(stmt);
		}

		public LocalReference DeclareLocal(Type type)
		{
			LocalReference local = new LocalReference(type);
			ilmarkers.Add(local);
			return local;
		}

//		public LabelReference CreateLabel()
//		{
//			LabelReference label = new LabelReference();
//			ilmarkers.Add(label);
//			return label;
//		}
//
		public /*protected internal*/ void SetNonEmpty()
		{
			isEmpty = false;
		}

		internal bool IsEmpty
		{
			get { return isEmpty; }
		}

		internal void Generate(IMemberEmitter member, ILGenerator il)
		{
			foreach(Reference local in ilmarkers)
			{
				local.Generate(il);
			}

			foreach(Statement stmt in stmts)
			{
				stmt.Emit(member, il);
			}
		}
	}
}