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

namespace Castle.Rook.Compiler.Visitors
{
	using System;
	using System.Collections;

	using Castle.Rook.Compiler.AST;


	public class BreadthFirstVisitor : AbstractVisitor
	{
		private Queue nodesToBeVisited = new Queue();

		public override void VisitCompilationUnit(CompilationUnit compilationUnit)
		{
			base.VisitCompilationUnit(compilationUnit);

			while(nodesToBeVisited.Count != 0)
			{
				VisitNode(nodesToBeVisited.Dequeue() as IVisitableNode);
			}
		}

		public override bool VisitNamespace(NamespaceDeclaration ns)
		{
			if (VisitEnter(ns))
			{
				EnqueueNodes(ns.Statements);

				return VisitLeave(ns);
			}

			return false;
		}

		public override bool VisitTypeDefinitionStatement(TypeDefinitionStatement typeDef)
		{
			if (VisitEnter(typeDef))
			{
				EnqueueNodes(typeDef.Statements);

				return VisitLeave(typeDef);
			}

			return false;
		}

		public override bool VisitMethodDefinitionStatement(MethodDefinitionStatement methodDef)
		{
			if (VisitEnter(methodDef))
			{
				EnqueueNode(methodDef.ReturnType);
				EnqueueNodes(methodDef.Statements);

				return VisitLeave(methodDef);
			}

			return false;
		}

		private void EnqueueNodes(IList nodes)
		{
			foreach(IVisitableNode node in nodes)
			{
				EnqueueNode(node);
			}
		}

		private void EnqueueNode(IVisitableNode node)
		{
			nodesToBeVisited.Enqueue(node);
		}
	}
}
