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


	public class NamespaceDeclaration : AbstractCodeNode, INameScopeAccessor
	{
		private String name;
		private IList statements = new ArrayList();
		private INameScope namescope;

		public NamespaceDeclaration(INameScope parentScope)
		{
			this.namescope = new NameScope(NameScopeType.Namespace, parentScope);
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public IList Statements
		{
			get { return statements; }
		}

		public override bool Accept(IASTVisitor visitor)
		{
			return visitor.VisitNamespace(this);
		}

		public INameScope Namescope
		{
			get { return namescope; }
		}
	}
}
