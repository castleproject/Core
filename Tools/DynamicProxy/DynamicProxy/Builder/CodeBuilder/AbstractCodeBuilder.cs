// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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
	public abstract class AbstractCodeBuilder
	{
		private ILGenerator m_generator;
		private bool m_isEmpty;
		private ArrayList m_stmts;
		private ArrayList m_ilmarkers;

		protected AbstractCodeBuilder( ILGenerator generator ) 
		{
			m_stmts = new ArrayList();
			m_ilmarkers = new ArrayList();
			
			m_generator = generator;
			m_isEmpty = true;
		}

		protected ILGenerator Generator
		{
			get { return m_generator; }
		}

		public void AddStatement( Statement stmt )
		{
			SetNonEmpty();
			m_stmts.Add( stmt );
		}

		public LocalReference DeclareLocal( Type type )
		{
			LocalReference local = new LocalReference(type);
			m_ilmarkers.Add( local );
			return local;
		}

		public LabelReference CreateLabel()
		{
			LabelReference label = new LabelReference();
			m_ilmarkers.Add(label);
			return label;
		}

		protected internal void SetNonEmpty()
		{
			m_isEmpty = false;
		}

		internal bool IsEmpty
		{
			get { return m_isEmpty; }
		}

		internal void Generate( IEasyMember member, ILGenerator il )
		{
			foreach(Reference local in m_ilmarkers)
			{
				local.Generate(il);
			}

			foreach(Statement stmt in m_stmts)
			{
				stmt.Emit(member, il);
			}
		}
	}
}
