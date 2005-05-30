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

	using Castle.Rook.Compiler.Visitors;
	using Castle.Rook.Compiler.AST.Util;

	public enum DefinitionScope
	{
		Local,
		Instance, 
		Static
	}

	public class TypeDeclarationExpression : AbstractExpression
	{
		private DefinitionScope defScope;
		private TypeReference typeRef;
		private IExpression initExp;
		private string name;

		public TypeDeclarationExpression(String name, TypeReference typeRef)
		{
			this.name = name;
			this.typeRef = typeRef;

			defScope = ASTUtils.GetScopeFromName(name);
		}

		public string Name
		{
			get { return name; }
		}

		public DefinitionScope DefScope
		{
			get { return defScope; }
		}

		public IExpression InitExp
		{
			get { return initExp; }
			set
			{
				initExp = value; 
				if (value != null) value.Parent = this;
			}
		}

		public TypeReference TypeReference
		{
			get { return typeRef; }
			set { typeRef = value; }
		}

		public override bool Accept(IASTVisitor visitor)
		{
			return visitor.VisitTypeDeclarationExpression(this);
		}
	}
}
