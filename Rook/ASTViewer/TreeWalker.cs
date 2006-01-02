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

namespace ASTViewer
{
	using System;
	using System.Collections;

	using System.Windows.Forms;
	
	using Castle.Rook.Compiler.AST;
	using Castle.Rook.Compiler.Visitors;


	public class TreeWalker : DepthFirstVisitor
	{
		private TreeNodeCollection nodes;
		private Stack nodeStack = new Stack();

		public TreeWalker(CompilationUnit unit, TreeNodeCollection nodes)
		{
			this.nodes = nodes;

			VisitNode(unit);
		}

		public TreeNode CurrentNode
		{
			get { return nodeStack.Peek() as TreeNode; }
		}

		public override void VisitCompilationUnit(CompilationUnit compilationUnit)
		{
			nodeStack.Push( nodes.Add("CompilationUnit") );
			CurrentNode.Tag = compilationUnit;

			base.VisitCompilationUnit(compilationUnit);

			nodeStack.Pop();
		}

		public override bool VisitSourceUnit(SourceUnit unit)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("SourceUnit") );
			CurrentNode.Tag = unit;

			base.VisitSourceUnit(unit);

			nodeStack.Pop();

			return true;
		}

		public override bool VisitNamespace(NamespaceDescriptor ns)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("Namespace " + ns.Name) );
			CurrentNode.Tag = ns;

			CurrentNode.EnsureVisible();

			base.VisitNamespace(ns);

			nodeStack.Pop();

			return true;
		}

		public override bool VisitTypeDefinitionStatement(TypeDefinitionStatement typeDef)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("Type " + typeDef.Name ) );
			CurrentNode.Tag = typeDef;

			CurrentNode.EnsureVisible();

			base.VisitTypeDefinitionStatement(typeDef);

			nodeStack.Pop();

			return true;
		}

		public override bool VisitMethodDefinitionStatement(MethodDefinitionStatement methodDef)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("Method " + methodDef.Name + "[ret " + methodDef.ReturnType + "]" ));
			CurrentNode.Tag = methodDef;

			CurrentNode.EnsureVisible();

			base.VisitMethodDefinitionStatement(methodDef);

			nodeStack.Pop();

			return true;
		}

		public override void VisitConstructorDefinitionStatement(ConstructorDefinitionStatement statement)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("Constructor " + statement.Name ));
			CurrentNode.Tag = statement;

			CurrentNode.EnsureVisible();

			base.VisitConstructorDefinitionStatement(statement);

			nodeStack.Pop();
		}

		public override void VisitTypeReference(TypeReference reference)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("TypeReference " + reference.TypeName + " Resolved to [" + reference.ResolvedType + "]"));
			CurrentNode.Tag = reference;

			CurrentNode.EnsureVisible();

			base.VisitTypeReference(reference);

			nodeStack.Pop();
		}

		public override bool VisitIdentifier(Identifier identifier)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("Identifier " + identifier.Name + " - " + identifier.TypeReference ));
			CurrentNode.Tag = identifier;

			CurrentNode.EnsureVisible();

			base.VisitIdentifier(identifier);

			nodeStack.Pop();

			return true;
		}

		public override void VisitParameterVarIdentifier(ParameterVarIdentifier identifier)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("ParameterVarIdentifier " + identifier.Name + " - " + identifier.TypeReference ));
			CurrentNode.Tag = identifier;
			
			base.VisitParameterVarIdentifier(identifier);

			nodeStack.Pop();
		}

		public override void VisitOpaqueIdentifier(OpaqueIdentifier identifier)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("OpaqueIdentifier " + identifier.Name + " - " + identifier.TypeReference ));
			CurrentNode.Tag = identifier;
			
			base.VisitOpaqueIdentifier(identifier);

			nodeStack.Pop();
		}

		public override void VisitExpressionStatement(ExpressionStatement statement)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("ExpressionStatement "));
			CurrentNode.Tag = statement;

			base.VisitExpressionStatement(statement);

			nodeStack.Pop();
		}

		public override void VisitMethodInvocationExpression(MethodInvocationExpression invocationExpression)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("MethodInvocationExpression " + invocationExpression.Designator));
			CurrentNode.Tag = invocationExpression;

			base.VisitMethodInvocationExpression(invocationExpression);

			nodeStack.Pop();
		}

		public override void VisitVariableReferenceExpression(VariableReferenceExpression expression)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("VariableReferenceExpression - " + expression.Identifier.Name));
			CurrentNode.Tag = expression;

			base.VisitVariableReferenceExpression(expression);

			nodeStack.Pop();
		}

		public override void VisitConstExpression(ConstExpression expression)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("ConstExpression " + expression.Value + " " + expression.ValueType ));
			CurrentNode.Tag = expression;

			base.VisitConstExpression(expression);

			nodeStack.Pop();
		}

		public override void VisitBlockExpression(BlockExpression expression)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("BlockExpression [" + expression.Parameters.Count + " params ] " ));
			CurrentNode.Tag = expression;

			base.VisitBlockExpression(expression);

			nodeStack.Pop();
		}

		public override void VisitMemberAccessExpression(MemberAccessExpression accessExpression)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("MemberAccessExpression [" + accessExpression.Name + " ] " ));
			CurrentNode.Tag = accessExpression;

			base.VisitMemberAccessExpression(accessExpression);

			nodeStack.Pop();
		}


	}
}
