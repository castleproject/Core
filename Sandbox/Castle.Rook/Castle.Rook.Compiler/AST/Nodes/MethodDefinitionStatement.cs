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
	using System.Collections;

	using Castle.Rook.Compiler.Visitors;
	using Castle.Rook.Compiler.AST.Util;


	public class MethodDefinitionStatement : AbstractStatement
	{
		private String name;
		private String fullname;
		private TypeReference boundTo;
		private TypeReference returnType;
		private bool isStatic;
		private readonly AccessLevel accessLevel;
		private readonly IList args = new ArrayList();
		private readonly StatementCollection statements;

		public MethodDefinitionStatement(INameScope parentScope, AccessLevel accessLevel, String fullname) : base(NodeType.MethodDefinition)
		{
			statements = new StatementCollection(this);
			nameScope = new NameScope(NameScopeType.Method, parentScope);
			this.fullname = fullname;
			this.accessLevel = accessLevel;

			String boundToName;
			ASTUtils.CollectMethodInformation(fullname, out boundToName, out name, out isStatic);

			if (boundToName != null)
			{
				boundTo = new TypeReference( ":" + boundToName );
			}
		}

		public string FullName
		{
			get { return fullname; }
		}

		public bool IsStatic
		{
			get { return isStatic; }
		}

		public string Name
		{
			get { return name; }
		}

		public AccessLevel AccessLevel
		{
			get { return accessLevel; }
		}

		public TypeReference BoundTo
		{
			get { return boundTo; }
		}

		public IList Parameters
		{
			get { return args; }
		}

		public StatementCollection Statements
		{
			get { return statements; }
		}

		public TypeReference ReturnType
		{
			get { return returnType; }
			set { returnType = value; }
		}

		public void AddParameter(ParameterIdentifier paramIdent)
		{
			args.Add(paramIdent); paramIdent.Parent = this;
		}

		public override bool Accept(IASTVisitor visitor)
		{
			return visitor.VisitMethodDefinitionStatement(this);
		}
	}
}
