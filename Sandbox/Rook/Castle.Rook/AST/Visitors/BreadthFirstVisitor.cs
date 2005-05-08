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

namespace Castle.Rook.AST.Visitors
{
	using System;
	using System.Collections;


	public class BreadthFirstVisitor : IVisitor
	{
		Queue scheduled = new Queue();

		public void OnCompilationUnit(CompilationUnitNode node)
		{
			Visit(node.Namespaces);
			Visit(node.ClassesTypes);
			Visit(node.MixinTypes);
			Visit(node.StructTypes);
		}

		public void OnNamespace(NamespaceNode namespaceNode)
		{
			OnIdentifier(namespaceNode.Identifier);
		}

		public void OnType(TypeNode typeNode)
		{
			OnIdentifier(typeNode.Name);
		}

		public void OnType(ClassNode typeNode)
		{
			
		}

		public void OnType(InterfaceNode typeNode)
		{
			
		}

		public void OnIdentifier(String identifier)
		{
			
		}

		public void OnIdentifier(Identifier qualifiedIdentifier)
		{
			
		}

		public void OnIdentifier(QualifiedIdentifier qualifiedIdentifier)
		{
			
		}

		public void OnStatement(AbstractStatement statement)
		{
			
		}

		public void OnExpression(Expression expression)
		{
			
		}

		protected void Visit(IList list)
		{
			foreach(IASTNode node in list)
			{
				node.Visit(this);
			}
		}
	}
}
