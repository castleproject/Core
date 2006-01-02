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


	public abstract class AbstractVisitor : IASTVisitor
	{
		public AbstractVisitor()
		{
		}

		public virtual bool VisitNode(IVisitableNode node)
		{
			if (node == null) return true;

			bool res = node.Accept(this);

			return res;
		}

		public virtual bool VisitNodes(IList nodes)
		{
			foreach(IVisitableNode node in nodes)
			{
				VisitNode( node );
			}

			return true;
		}

		public virtual void VisitCompilationUnit(CompilationUnit compilationUnit)
		{
			VisitNode(compilationUnit.EntryPointSourceUnit);
			VisitNodes(compilationUnit.SourceUnits);
		}

		public virtual bool VisitSourceUnit(SourceUnit unit)
		{
			VisitNodes(unit.Namespaces);
			VisitNodes(unit.Statements);

			return true;
		}

		public virtual bool VisitNamespace(NamespaceDescriptor ns)
		{
			if (VisitEnter(ns))
			{
				VisitNodes(ns.TypeDefinitions);

				return VisitLeave(ns);
			}

			return true;
		}

		public virtual bool VisitEnter(NamespaceDescriptor ns)
		{
			return true;
		}

		public virtual bool VisitLeave(NamespaceDescriptor ns)
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
				VisitNodes(methodDef.Arguments);
				VisitNodes(methodDef.Statements);

				return VisitLeave(methodDef);
			}

			return false;
		}

		public virtual void VisitConstructorDefinitionStatement(ConstructorDefinitionStatement statement)
		{
			if (VisitEnter(statement))
			{
				VisitNodes(statement.Arguments);
				VisitNodes(statement.Statements);

				VisitLeave(statement);
			}
		}

		public virtual bool VisitEnter(MethodDefinitionStatement methodDef)
		{
			return true;
		}

		public virtual bool VisitLeave(MethodDefinitionStatement methodDef)
		{
			return true;
		}

		//
		// References
		//

		public virtual void VisitTypeReference(TypeReference reference)
		{
		}

		public virtual bool VisitIdentifier(Identifier identifier)
		{
			VisitNode(identifier.TypeReference);

			return true;
		}

		public virtual void VisitParameterVarIdentifier(ParameterVarIdentifier parameterIdentifier)
		{
			VisitNode(parameterIdentifier.TypeReference);
			VisitNode(parameterIdentifier.InitExpression);
		}

		public virtual void VisitOpaqueIdentifier(OpaqueIdentifier opaqueIdentifier)
		{
			VisitNode(opaqueIdentifier.TypeReference);
			
		}

		//
		// Statements
		//

		public virtual void VisitExpressionStatement(ExpressionStatement statement)
		{
			VisitNode(statement.InnerExpression);
		}

		//
		// Expressions
		//

		public virtual void VisitMethodInvocationExpression(MethodInvocationExpression invocationExpression)
		{
			VisitNode(invocationExpression.Designator);
			VisitNodes(invocationExpression.Arguments);
			VisitCommonExpNodes(invocationExpression);
		}

		public virtual void VisitVariableReferenceExpression(VariableReferenceExpression expression)
		{
			VisitNode(expression.Identifier);
			VisitCommonExpNodes(expression);
		}

		public virtual void VisitConstExpression(ConstExpression expression)
		{
			VisitCommonExpNodes(expression);
		}

		public virtual void VisitBlockExpression(BlockExpression expression)
		{
			VisitNodes(expression.Parameters);
			VisitNodes(expression.Statements);
			VisitCommonExpNodes(expression);
		}

		public virtual void VisitMemberAccessExpression(MemberAccessExpression accessExpression)
		{
			VisitNode(accessExpression.Inner);
			VisitCommonExpNodes(accessExpression);
		}

		//
		// Helpers
		//

		private void VisitCommonExpNodes(IExpression expression)
		{
			VisitNode(expression.PostfixCondition);
			VisitNode(expression.Block);
		}
	}
}
