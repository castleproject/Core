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


	public class MethodDefinitionStatement : AbstractStatement
	{
		private readonly IList statements = new ArrayList();
		private readonly IList args = new ArrayList();
		private readonly string fullname;
		private TypeReference returnType;

		public MethodDefinitionStatement(String fullname)
		{
			this.fullname = fullname;
		}

		public IList Arguments
		{
			get { return args; }
		}

		public IList Statements
		{
			get { return statements; }
		}

		public TypeReference ReturnType
		{
			get { return returnType; }
			set { returnType = value; }
		}

		public void AddArgument(TypeDeclarationExpression tde)
		{
			args.Add(tde);
		}

		public override bool Accept(IASTVisitor visitor)
		{
			return visitor.VisitMethodDefinitionStatement(this);
		}
	}
}
