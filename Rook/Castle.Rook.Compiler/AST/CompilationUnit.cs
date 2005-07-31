// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.Rook.Compiler.AST
{
	using System;
	using System.Reflection.Emit;
	using Castle.Model.Internal;
	using Castle.Rook.Compiler.Visitors;


	public class CompilationUnit : ASTNode
	{
		private ASTNodeCollection sourceUnits;
		private SourceUnit entryPointSourceUnit;
		private AssemblyBuilder assemblyBuilder;
		private ModuleBuilder moduleBuilder;

		public CompilationUnit()
		{
			sourceUnits = new ASTNodeCollection(this);
			DefiningSymbolTable = new SymbolTable(null, ScopeType.Global);
		}

		public ASTNodeCollection SourceUnits
		{
			get { return sourceUnits; }
		}

		public SourceUnit EntryPointSourceUnit
		{
			get { return entryPointSourceUnit; }
			set { entryPointSourceUnit = value; }
		}

		public AssemblyBuilder AssemblyBuilder
		{
			get { return assemblyBuilder; }
			set { assemblyBuilder = value; }
		}

		public ModuleBuilder ModuleBuilder
		{
			get { return moduleBuilder; }
			set { moduleBuilder = value; }
		}

		public override bool Accept(IASTVisitor visitor)
		{
			visitor.VisitCompilationUnit(this);
			return true;
		}
	}
}
