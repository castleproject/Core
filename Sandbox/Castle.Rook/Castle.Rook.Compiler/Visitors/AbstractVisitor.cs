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


	public abstract class AbstractVisitor : IASTVisitor
	{
		public AbstractVisitor()
		{
		}

		public virtual bool VisitNode(IVisitableNode node)
		{
			if (node == null) return true;

			return node.Accept(this);
		}

		public virtual bool VisitNodes(IList nodes)
		{
			BeforeVisitingNodes();

			foreach(IVisitableNode node in nodes)
			{
				VisitNode(node);
			}

			AfterVisitingNodes();

			return true;
		}

		protected virtual void AfterVisitingNodes()
		{
		}

		protected virtual void BeforeVisitingNodes()
		{
		}

		public virtual bool VisitNamespace(NamespaceDeclaration ns)
		{
			if (VisitEnter(ns))
			{
				VisitNodes(ns.Statements);

				return VisitLeave(ns);
			}

			return false;
		}

		public virtual bool VisitEnter(NamespaceDeclaration ns)
		{
			return true;
		}

		public virtual bool VisitLeave(NamespaceDeclaration ns)
		{
			return true;
		}

		public virtual bool VisitTypeDefinitionStatement(TypeDefinitionStatement typeDef)
		{
			if (VisitEnter(typeDef))
			{
				VisitNodes(typeDef.Statements);

				return VisitLeave(typeDef);
			}

			return false;
		}

		public virtual bool VisitEnter(TypeDefinitionStatement typeDef)
		{
			return true;
		}

		public virtual bool VisitLeave(TypeDefinitionStatement typeDef)
		{
			return true;
		}

		public virtual bool VisitMethodDefinitionStatement(MethodDefinitionStatement methodDef)
		{
			if (VisitEnter(methodDef))
			{
				VisitNode(methodDef.ReturnType);
				VisitNodes(methodDef.Statements);

				return VisitLeave(methodDef);
			}

			return false;
		}

		public virtual bool VisitEnter(MethodDefinitionStatement methodDef)
		{
			return true;
		}

		public virtual bool VisitLeave(MethodDefinitionStatement methodDef)
		{
			return true;
		}

		public virtual bool VisitTypeReference(TypeReference reference)
		{
			return true;
		}

		public virtual bool VisitVariableDeclarationStatement(VariableDeclarationStatement varDecl)
		{
			return true;
		}

		public virtual bool VisitRepeatStatement(RepeatStatement statement)
		{
			return true;
		}

		public virtual bool VisitPostfixCondition(PostfixCondition postfixCondition)
		{
			return true;
		}

		public virtual bool VisitAssignmentExpression(AssignmentExpression assignExp)
		{
			return true;
		}

		public virtual bool VisitAugAssignmentExpression(AugAssignmentExpression auAssignExp)
		{
			return true;
		}

		public virtual bool VisitYieldExpression(YieldExpression yieldExpression)
		{
			return true;
		}

		public virtual bool VisitVariableReferenceExpression(VariableReferenceExpression variableReferenceExpression)
		{
			return true;
		}

		public virtual bool VisitUnaryExpression(UnaryExpression unaryExpression)
		{
			return true;
		}

		public virtual bool VisitTypeDeclarationExpression(TypeDeclarationExpression typeDeclarationExpression)
		{
			return true;
		}

		public virtual bool VisitRetryExpression(RetryExpression expression)
		{
			return true;
		}

		public virtual bool VisitNextExpression(NextExpression expression)
		{
			return true;
		}

		public virtual bool VisitRedoExpression(RedoExpression expression)
		{
			return true;
		}

		public virtual bool VisitRangeExpression(RangeExpression rangeExpression)
		{
			return true;
		}

		public virtual bool VisitRaiseExpression(RaiseExpression expression)
		{
			return true;
		}

		public virtual bool VisitMethodInvocationExpression(MethodInvocationExpression invocationExpression)
		{
			return true;
		}

		public virtual bool VisitMemberAccessExpression(MemberAccessExpression accessExpression)
		{
			return true;
		}

		public virtual bool VisitLiteralReferenceExpression(LiteralReferenceExpression expression)
		{
			return true;
		}

		public virtual bool VisitListExpression(ListExpression expression)
		{
			return true;
		}

		public virtual bool VisitLambdaExpression(LambdaExpression expression)
		{
			return true;
		}

		public virtual bool VisitIfStatement(IfStatement ifStatement)
		{
			return true;
		}

		public virtual bool VisitForStatement(ForStatement statement)
		{
			return true;
		}

		public virtual bool VisitExpressionStatement(ExpressionStatement statement)
		{
			return true;
		}

		public virtual bool VisitDictExpression(DictExpression expression)
		{
			return true;
		}

		public virtual bool VisitCompoundExpression(CompoundExpression expression)
		{
			return true;
		}

		public virtual void VisitCompilationUnit(CompilationUnit compilationUnit)
		{
			VisitNodes(compilationUnit.Namespaces);
			VisitNodes(compilationUnit.Statements);
		}

		public virtual bool VisitBreakExpression(BreakExpression breakExpression)
		{
			return true;
		}

		public virtual bool VisitBlockExpression(BlockExpression expression)
		{
			return true;
		}

		public virtual bool VisitBinaryExpression(BinaryExpression expression)
		{
			return true;
		}

		public virtual bool VisitAusAssignmentExpression(AugAssignmentExpression expression)
		{
			return true;
		}
	}
}
