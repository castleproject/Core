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
	using Castle.Model.Internal;
	using Castle.Rook.Compiler.Visitors;


	public class MethodDefinitionStatement : Statement
	{
		private AccessLevel accessLevel;
		private string name;
		private TypeReference returnType;
		private ASTNodeCollection statements;
		private ASTNodeCollection arguments;

		private bool isOverride, isNewSlot, isAbstract, isVirtual, isStatic, isFinal;

		public MethodDefinitionStatement(AccessLevel accessLevel) : base(StatementType.MethodDef)
		{
			statements = new ASTNodeCollection(this);
			arguments = new ASTNodeCollection(this);
			this.accessLevel = accessLevel;
		}

		public AccessLevel AccessLevel
		{
			get { return accessLevel; }
			set { accessLevel = value; }
		}

		public string Name
		{
			get { return name; }
			set
			{
				if (value.StartsWith("self."))
				{
					name = value.Substring( "self.".Length );
					isStatic = true;
				}
				else
				{
					name = value;
				}
			}
		}

		public TypeReference ReturnType
		{
			get { return returnType; }
			set { returnType = value; }
		}

		public ASTNodeCollection Statements
		{
			get { return statements; }
		}

		public ASTNodeCollection Arguments
		{
			get { return arguments; }
		}

		public void AddFormalParameter(ParameterVarIdentifier param)
		{
			arguments.Add(param);
		}

		public bool IsOverride
		{
			get { return isOverride; }
			set { isOverride = value; }
		}

		public bool IsNewSlot
		{
			get { return isNewSlot; }
			set { isNewSlot = value; }
		}

		public bool IsAbstract
		{
			get { return isAbstract; }
			set { isAbstract = value; }
		}

		public bool IsVirtual
		{
			get { return isVirtual; }
			set { isVirtual = value; }
		}

		public bool IsStatic
		{
			get { return isStatic; }
			set { isStatic = value; }
		}

		public bool IsFinal
		{
			get { return isFinal; }
			set { isFinal = value; }
		}

		public override bool Accept(IASTVisitor visitor)
		{
			visitor.VisitMethodDefinitionStatement(this);
			return true;
		}
	}
}
