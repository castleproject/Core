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
	using System.Collections;

	using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;

	/// <summary>
	/// Summary description for AbstractCodeBuilder.
	/// </summary>
	[CLSCompliant(false)]
	public abstract class AbstractCodeBuilder
	{
		private ILGenerator _generator;
		private bool _isEmpty;
		private ArrayList _stmts;
		private ArrayList _ilmarkers;

		protected AbstractCodeBuilder( ILGenerator generator ) 
		{
			_stmts = new ArrayList();
			_ilmarkers = new ArrayList();
			
			_generator = generator;
			_isEmpty = true;
		}

		protected ILGenerator Generator
		{
			get { return _generator; }
		}

		public void AddStatement( Statement stmt )
		{
			SetNonEmpty();
			_stmts.Add( stmt );
		}

		public LocalReference DeclareLocal( Type type )
		{
			LocalReference local = new LocalReference(type);
			_ilmarkers.Add( local );
			return local;
		}

		public LabelReference CreateLabel()
		{
			LabelReference label = new LabelReference();
			_ilmarkers.Add(label);
			return label;
		}

		protected internal void SetNonEmpty()
		{
			_isEmpty = false;
		}

		internal bool IsEmpty
		{
			get { return _isEmpty; }
		}

		internal void Generate( IEasyMember member, ILGenerator il )
		{
			foreach(Reference local in _ilmarkers)
			{
				local.Generate(il);
			}

			foreach(Statement stmt in _stmts)
			{
				stmt.Emit(member, il);
			}
		}
	}
}
