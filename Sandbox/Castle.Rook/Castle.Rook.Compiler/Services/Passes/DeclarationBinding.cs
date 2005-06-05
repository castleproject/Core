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
		private readonly IIdentifierNameService identifierService;

		public DeclarationBinding(IErrorReport errorReport, IIdentifierNameService identifierService)
		{
			this.errorReport = errorReport;
			this.identifierService = identifierService;
		}

		public void ExecutePass(CompilationUnit unit)
		{
			VisitNode(unit);
		}

		public override bool VisitNamespace(NamespaceDeclaration ns)
		{
			if (!identifierService.IsValidNamespaceName(ns.Name))
			{
				errorReport.Error( "TODOFILENAME", ns.Position, "'{0}' is an invalid namespace name.", ns.Name );

				return false;
			}

			ns.NameScope.CurrentTypeGraph.DefineNamespace(ns.Name);

			return base.VisitNamespace(ns);
		}

		public override bool VisitTypeDefinitionStatement(TypeDefinitionStatement typeDef)
		{
			if (!identifierService.IsValidTypeName(typeDef.Name))
			{
				errorReport.Error( "TODOFILENAME", typeDef.Position, "'{0}' is an invalid type name.", typeDef.Name );

				return false;
			}

			try
			{
				typeDef.NameScope.Parent.CurrentTypeGraph.DefineType(typeDef);
			}
			catch(Exception)
			{
				errorReport.Error( "TODOFILENAME", typeDef.Position, "'{0}' has multiple definitions.", typeDef.Name );

				return false;
			}

			return base.VisitTypeDefinitionStatement(typeDef);
		}

		public override bool VisitMultipleVariableDeclarationStatement(MultipleVariableDeclarationStatement varDecl)
		{
			ProcessMultipleVariableDeclarationStatement(varDecl);

			return base.VisitMultipleVariableDeclarationStatement(varDecl);
		}

		private void ProcessMultipleVariableDeclarationStatement(MultipleVariableDeclarationStatement decl)
		{
			IList stmts = ConvertToSingleDeclarationStatements(decl);

			ReplaceOriginalMultipleVariableDeclarationStatement(decl, stmts);

			EnsureTypeDeclarationsBelongsToThisScope(decl, stmts);
		}

		private void ReplaceOriginalMultipleVariableDeclarationStatement(MultipleVariableDeclarationStatement decl, IList stmts)
		{
			int index;

			// Replace the VariableDeclarationStatement node by a 
			// (possible) sequence of SingleVariableDeclarationStatement
	
			IStatementContainer stmtContainer = decl.Parent as IStatementContainer;
	
			index = stmtContainer.Statements.IndexOf(decl);
			
			stmtContainer.Statements.RemoveAt(index);

			foreach(SingleVariableDeclarationStatement svDecl in stmts)
			{
				stmtContainer.Statements.Insert( index++, svDecl );
			}
		}

		public override bool VisitParameterIdentifier(ParameterIdentifier parameterIdentifier)
		{
			INameScope namescope = parameterIdentifier.Parent.NameScope;

			System.Diagnostics.Debug.Assert( namescope != null );
			System.Diagnostics.Debug.Assert( namescope.NameScopeType == NameScopeType.Method || namescope.NameScopeType == NameScopeType.Block );

			if (!identifierService.IsValidFormatParameterName(parameterIdentifier.Name))
			{
				errorReport.Error( "TODOFILENAME", parameterIdentifier.Position, "'{0}' is an invalid parameter name.", parameterIdentifier.Name );
				return false;
			}

			System.Diagnostics.Debug.Assert( !namescope.IsDefined(parameterIdentifier.Name) );

			namescope.AddVariable( parameterIdentifier );

			return base.VisitParameterIdentifier(parameterIdentifier);
		}

		private void EnsureTypeDeclarationsBelongsToThisScope(MultipleVariableDeclarationStatement varDecl, IList stmts)
		{
			INameScope namescope = varDecl.Parent.NameScope;

			System.Diagnostics.Debug.Assert( namescope != null );
	
			foreach(SingleVariableDeclarationStatement typeDecl in stmts)
			{
				Identifier ident = typeDecl.Identifier;

				// Most simple of cases: duplicated declaration
				if (namescope.IsDefined(ident.Name))
				{
					errorReport.Error( "TODOFILENAME", typeDecl.Position, "Sorry but '{0}' is already defined.", ident.Name );
					continue;
				}

				// Second simple case: a local var and we are on the right place to 
				// declare it
				if (ident.Type == IdentifierType.Local && 
					(namescope.NameScopeType == NameScopeType.Method || 
					namescope.NameScopeType == NameScopeType.Compound || 
					namescope.NameScopeType == NameScopeType.Block))
				{
					namescope.AddVariable(ident);
					continue;
				}

				// More complex: a block or compound tries to redefine a variable
				if (ident.Type == IdentifierType.Local && 
					(namescope.NameScopeType == NameScopeType.Compound || 
					namescope.NameScopeType == NameScopeType.Block))
				{
					if (namescope.Parent.IsDefined(ident.Name))
					{
						errorReport.Error( "TODOFILENAME", typeDecl.Position, "Sorry but '{0}' is already defined in a parent scope.", ident.Name );
						continue;
					}
				}

				// Local variables at class level?
				// We will support that as a type initializer, but not now.
				if (ident.Type == IdentifierType.Local && namescope.NameScopeType == NameScopeType.Type)
				{
					errorReport.Error( "TODOFILENAME", typeDecl.Position, "At type level, just instance or static fields are allowed (yet)" );
					continue;
				}

				// Static or instance in a method/block/compound are moved
				// to the parent class or source unit level
				if (ident.Type == IdentifierType.InstanceField || 
					ident.Type == IdentifierType.StaticField)
				{
					if (namescope.NameScopeType == NameScopeType.SourceUnit || 
						namescope.NameScopeType == NameScopeType.Type)
					{
						namescope.AddVariable(ident);
					}
					else if (namescope.NameScopeType == NameScopeType.Method || 
						namescope.NameScopeType == NameScopeType.Compound || 
						namescope.NameScopeType == NameScopeType.Block)
					{
						IASTNode node = varDecl.Parent;

						while(node != null && 
							node.NodeType != NodeType.TypeDefinition && 
							node.NodeType != NodeType.SourceUnit)
						{
							node = node.Parent;
						}

						if (node == null || node.NameScope == null)
						{
							errorReport.Error( "TODOFILENAME", typeDecl.Position, 
								"Compiler error: The instance of static declaration '{0}' could not be mapped to a parent type", ident.Name );							
							continue;
						}

						INameScope parentScope = node.NameScope;

						IStatementContainer typeStmtsContainer = node as IStatementContainer;
							
						System.Diagnostics.Debug.Assert( parentScope != null );
						System.Diagnostics.Debug.Assert( typeStmtsContainer != null );

						if (parentScope.IsDefined(ident.Name))
						{
							errorReport.Error( "TODOFILENAME", typeDecl.Position, 
								"Sorry but '{0}' is already defined.", ident.Name );
						}
						else
						{
							parentScope.AddVariable(ident);

							// We can replace the declaration on the method 
							// body with an assignment if and only if this type decl has
							// an init expression, so CreateAssignmentFromTypeDecl can return null
							AssignmentExpression assignExp = CreateAssignmentFromTypeDecl(typeDecl);
							ExpressionStatement assignExpStmt = new ExpressionStatement(assignExp);

							typeDecl.ConvertInitExpressionToDependency();
							
							// Replace the declaration with an assignment
							(varDecl.Parent as IStatementContainer).Statements.Replace(typeDecl, assignExpStmt);

							// Add the member/field declaration to the parent
							typeStmtsContainer.Statements.Add( typeDecl );

							// TODO: Link assignment expression and typeDecl to help
							// find out the type of the field later
						}
					}
				}
			}
		}

		private IList ConvertToSingleDeclarationStatements(MultipleVariableDeclarationStatement varDecl)
		{
			IList newStmts = new ArrayList();

			int index = 0;
			ExpressionCollection initExps = varDecl.InitExpressions; 
	
			foreach(Identifier ident in varDecl.Identifiers)
			{
				// Here we are converting expression from 
				// x:int, y:long = 1, 2L
				// to an AST representation equivalent to
				// x:int = 1; y:long = 2L

				SingleVariableDeclarationStatement svStmt = new SingleVariableDeclarationStatement(ident);

				if (index < initExps.Count)
				{
					svStmt.InitExp = initExps[index];
					EnsureNoPostFixStatement(svStmt.InitExp);
				}
				
				index++;

				newStmts.Add(svStmt);
			}
	
			// We don't need them anymore
			initExps.Clear();

			return newStmts;
		}

		private void EnsureNoPostFixStatement(IExpression initExpression)
		{
			if (initExpression.PostFixStatement != null)
			{
				errorReport.Error( "TODOFILENAME", initExpression.Position, 
					"Sorry but a variable initializer can not be conditional or " + 
					"has a while/until statement attached.");
			}
		}

		private AssignmentExpression CreateAssignmentFromTypeDecl(SingleVariableDeclarationStatement decl)
		{
			if (decl.InitExp == null) return null;

			return new AssignmentExpression( new VariableReferenceExpression(decl.Identifier), decl.InitExp );
		} 
	}
}
