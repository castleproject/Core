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

namespace Castle.Rook.Compiler.AST
{
	using System;
	using System.Collections;
	using System.Reflection.Emit;
	
	using Castle.Rook.Compiler.Visitors;


	public class TypeDefinitionStatement : Statement
	{
		private string name;
		private AccessLevel accessLevel;
		private ASTNodeCollection statements;
		private TypeBuilder builder;
		private Type parentType;
		private IList baseTypes = new ArrayList();
		private IList interfaces = new ArrayList();

		public TypeDefinitionStatement(AccessLevel accessLevel, String name) : base(StatementType.TypeDefinition)
		{
			statements = new ASTNodeCollection(this);

			this.accessLevel = accessLevel;
			this.name = name;
		}

		public AccessLevel AccessLevel
		{
			get { return accessLevel; }
			set { accessLevel = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public Type ParentType
		{
			get { return parentType; }
			set { parentType = value; }
		}

		public TypeBuilder Builder
		{
			get { return builder; }
			set { builder = value; }
		}

		public IList Interfaces
		{
			get { return interfaces; }
		}

		public ASTNodeCollection Statements
		{
			get { return statements; }
		}

		public IList BaseTypes
		{
			get { return baseTypes; }
		}

		public override bool Accept(IASTVisitor visitor)
		{
			visitor.VisitTypeDefinitionStatement(this);
			return true;
		}
	}
}
