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

namespace Castle.Rook.Compiler.Visitors
{
	using System;
	using System.Collections;

	using Castle.Rook.Compiler.AST;


	public class BreadthFirstVisitor : AbstractVisitor
	{
		private bool nowQueue = false;
		private Queue nodesToBeVisited = new Queue();

		public override bool VisitNode(IVisitableNode node)
		{
			if (nowQueue)
			{
				EnqueueNode(node);
				return true;
			}
			else
			{
				return base.VisitNode(node);
			}
		}

		public override void VisitCompilationUnit(CompilationUnit compilationUnit)
		{
			base.VisitCompilationUnit(compilationUnit);

			ProcessNodesInQueue();
		}

		public override bool VisitSourceUnit(SourceUnit unit)
		{
			base.VisitSourceUnit(unit);

			ProcessNodesInQueue();

			return true;
		}

		public override bool VisitEnter(NamespaceDeclaration ns)
		{
			nowQueue = true;
			return base.VisitEnter(ns);
		}

		public override bool VisitLeave(NamespaceDeclaration ns)
		{
			nowQueue = false;
			return base.VisitLeave(ns);
		}

		public override bool VisitEnter(TypeDefinitionStatement typeDef)
		{
			nowQueue = true;
			return base.VisitEnter(typeDef);
		}

		public override bool VisitLeave(TypeDefinitionStatement typeDef)
		{
			nowQueue = false;
			return base.VisitLeave(typeDef);
		}

		public override bool VisitEnter(MethodDefinitionStatement methodDef)
		{
			nowQueue = true;
			return base.VisitEnter(methodDef);
		}

		public override bool VisitLeave(MethodDefinitionStatement methodDef)
		{
			nowQueue = false;
			return base.VisitLeave(methodDef);
		}

//		private void EnqueueNodes(IList nodes)
//		{
//			foreach(IVisitableNode node in nodes)
//			{
//				EnqueueNode(node);
//			}
//		}
//
		private void EnqueueNode(IVisitableNode node)
		{
			nodesToBeVisited.Enqueue(node);
		}

		private void ProcessNodesInQueue()
		{
			while(nodesToBeVisited.Count != 0)
			{
				VisitNode(nodesToBeVisited.Dequeue() as IVisitableNode);
			}
		}
	}
}
