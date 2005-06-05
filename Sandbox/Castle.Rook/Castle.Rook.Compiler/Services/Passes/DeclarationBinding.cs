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

namespace Castle.Rook.Compiler.Services.Passes
{
	using System;
	using System.Collections;

	using Castle.Rook.Compiler.AST;
	using Castle.Rook.Compiler.Visitors;


	public class DeclarationBinding : BreadthFirstVisitor, ICompilerPass
	{
		private readonly IErrorReport errorReport;

		public DeclarationBinding(IErrorReport errorReport)
		{
			this.errorReport = errorReport;
		}

		public void ExecutePass(CompilationUnit unit)
		{
			VisitNode(unit);
		}

//		public override bool VisitParameterIdentifier(ParameterIdentifier parameterIdentifier)
//		{
//			INameScope namescope = (parameterIdentifier.Parent as INameScopeAccessor).Namescope;
//
//			System.Diagnostics.Debug.Assert( namescope != null );
//			System.Diagnostics.Debug.Assert( namescope.NameScopeType == NameScopeType.Method || namescope.NameScopeType == NameScopeType.Block );
//
//			if (parameterIdentifier.Name.StartsWith("@"))
//			{
//				errorReport.Error( "TODOFILENAME", parameterIdentifier.Position, 
//					"'{0}' is an invalid parameter name.", parameterIdentifier.Name );
//				return false;
//			}
//
//			System.Diagnostics.Debug.Assert( !namescope.IsDefined(parameterIdentifier.Name) );
//
//			namescope.AddVariable( parameterIdentifier );
//
//			return base.VisitParameterIdentifier(parameterIdentifier);
//		}
//
//		public override bool VisitMultipleVariableDeclarationStatement(MultipleVariableDeclarationStatement varDecl)
//		{
//			ProcessMultipleVariableDeclarationStatement(varDecl);
//
//			return base.VisitMultipleVariableDeclarationStatement(varDecl);
//		}
//
//		private void ProcessMultipleVariableDeclarationStatement(MultipleVariableDeclarationStatement decl)
//		{
//			IList stmts = ConvertToSingleDeclarationStatements(decl);
//
//			ReplaceOriginalMultDeclarations(decl, stmts);
//
//			EnsureTypeDeclarationsBelongsToThisScope(decl, stmts);
//		}
//
//		public override bool VisitAssignmentExpression(AssignmentExpression assignExp)
//		{
//			VariableReferenceExpression varRef = (assignExp.Target as VariableReferenceExpression);
//
//			// TODO: Convert assignment to declaration if and only 
//			// if the identifier is not found on the scope.
//			// It might also be good to issue a warning when doing that
//
////			if (varRef != null)
////			{
////				// varRef.Identifier
////				
////				if (varRef.Identifier.Type == IdentifierType.Local)
////				{
////					
////				}
////			}
//
//			return base.VisitAssignmentExpression(assignExp);
//		}
//
//		private void EnsureTypeDeclarationsBelongsToThisScope(MultipleVariableDeclarationStatement varDecl, IList stmts)
//		{
//			INameScope namescope = (varDecl.Parent as INameScopeAccessor).Namescope;
//	
//			foreach(SingleVariableDeclarationStatement typeDecl in stmts)
//			{
//				Identifier ident = typeDecl.Identifier;
//
//				if (namescope.IsDefined(ident.Name))
//				{
//					errorReport.Error( "TODOFILENAME", typeDecl.Position, 
//						"Sorry but '{0}' is already defined.", ident.Name );
//				}
//				else
//				{
//					// TODO: If its a instance or static, and we're 
//					// in an inner block, we need to move these statments
//					// to the type/class level statements list
//
//					if (ident.Type == IdentifierType.Local)
//					{
//						namescope.AddVariable( ident );
//					}
//					else if (namescope.NameScopeType == NameScopeType.Global || 
//						namescope.NameScopeType == NameScopeType.Type) //||
//						// namescope.NameScopeType == NameScopeType.Namespace)
//					{
//						namescope.AddVariable( ident );
//					}
//					else
//					{
//						IASTNode parent = varDecl.Parent;
//						
//						while(parent != null && 
//							parent.NodeType != NodeType.TypeDefinition && 
//							// parent.NodeType != NodeType.NamespaceDefinition && 
//							parent.NodeType != NodeType.Global)
//						{
//							parent = parent.Parent;
//						}
//
//						if (parent != null)
//						{
//							INameScopeAccessor accessor  = parent as INameScopeAccessor;
//							IStatementContainer typeOrGlobalStmtsContainer = parent as IStatementContainer;
//							
//							System.Diagnostics.Debug.Assert( accessor != null );
//							System.Diagnostics.Debug.Assert( typeOrGlobalStmtsContainer != null );
//
//							if (accessor.Namescope.IsDefined(ident.Name))
//							{
//								errorReport.Error( "TODOFILENAME", typeDecl.Position, 
//									"Sorry but '{0}' is already defined.", ident.Name );
//							}
//							else
//							{
//								accessor.Namescope.AddVariable(ident);
//
//								// We can replace the declaration on the method 
//								// body with an assignment if and only if this type decl has
//								// an init expression, so CreateAssignmentFromTypeDecl can return null
//								AssignmentExpression assignExp = CreateAssignmentFromTypeDecl(typeDecl);
//								ExpressionStatement assignExpStmt = new ExpressionStatement(assignExp);
//
//								// Clear the InitExp as it might be invalid aside from this context
//								typeDecl.InitExp = null;
//							
//								// Replace the declaration with an assignment
//								(varDecl.Parent as IStatementContainer).Statements.Replace(typeDecl, assignExpStmt);
//
//								// Add the member/field declaration to the parent
//								typeOrGlobalStmtsContainer.Statements.Add( typeDecl );
//
//								// TODO: Link assignment expression and typeDecl to help
//								// find out the type of the field later
//							}
//						}
//						else
//						{
//							errorReport.Error( "TODOFILENAME", typeDecl.Position, 
//								"The instance of static declaration '{0}' could not be mapped to the parent type", ident.Name );
//						}
//					}
//				}
//			}
//		}
//
//		private IList ConvertToSingleDeclarationStatements(MultipleVariableDeclarationStatement varDecl)
//		{
//			IList newStmts = new ArrayList();
//
//			int index = 0;
//			ExpressionCollection initExps = varDecl.InitExpressions; 
//	
//			foreach(Identifier ident in varDecl.Identifiers)
//			{
//				// Here we are converting expression from 
//				// x:int, y:long = 1, 2L
//				// to an AST representation equivalent to
//				// x:int = 1; y:long = 2L
//
//				SingleVariableDeclarationStatement svStmt = new SingleVariableDeclarationStatement(ident);
//
//				if (index < initExps.Count)
//				{
//					svStmt.InitExp = initExps[index];
//					EnsureNoPostFixStatement(svStmt.InitExp);
//				}
//				
//				index++;
//
//				newStmts.Add(svStmt);
//			}
//	
//			// We don't need them anymore
//			initExps.Clear();
//
//			return newStmts;
//		}
//
//		private void ReplaceOriginalMultDeclarations(MultipleVariableDeclarationStatement varDecl, IList stmts)
//		{
//			int index;
//
//			// Replace the VariableDeclarationStatement node by a 
//			// (possible) sequence of SingleVariableDeclarationStatement
//	
//			IStatementContainer stmtContainer = varDecl.Parent as IStatementContainer;
//	
//			index = stmtContainer.Statements.IndexOf(varDecl);
//			stmtContainer.Statements.RemoveAt(index);
//	
//			foreach(SingleVariableDeclarationStatement svDecl in stmts)
//			{
//				stmtContainer.Statements.Insert( index++, svDecl );
//			}
//		}
//
//		private void EnsureNoPostFixStatement(IExpression initExpression)
//		{
//			if (initExpression.PostFixStatement != null)
//			{
//				errorReport.Error( "TODOFILENAME", initExpression.Position, 
//					"Sorry but a variable initializer can not be conditional or " + 
//					"has a while/until statement attached.");
//			}
//		}
//
//		private AssignmentExpression CreateAssignmentFromTypeDecl(SingleVariableDeclarationStatement decl)
//		{
//			if (decl.InitExp == null) return null;
//
//			return new AssignmentExpression( new VariableReferenceExpression(decl.Identifier), decl.InitExp );
//		} 
	}
}
