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

	using Castle.Rook.Compiler.AST;
	using Castle.Rook.Compiler.Visitors;


	public class ScopeBinding : BreadthFirstVisitor, ICompilerPass
	{
		private readonly IIdentifierNameService identifierService;
		private readonly IErrorReport errorReport;
		private INameScope namescope;

		public ScopeBinding(IIdentifierNameService identifierService, IErrorReport errorReport)
		{
			this.identifierService = identifierService;
			this.errorReport = errorReport;
		}

		public void ExecutePass(CompilationUnit unit)
		{
			VisitNode(unit);
		}

		protected override void BeforeVisitingNode(IVisitableNode node)
		{
			INameScopeAccessor accessor = node as INameScopeAccessor;
			
			if (accessor != null)
			{
				namescope = accessor.Namescope;
			}
		}

		protected override void AfterVisitingNode(IVisitableNode node)
		{
			INameScopeAccessor accessor = node as INameScopeAccessor;
			
			if (accessor != null)
			{
				namescope = accessor.Namescope.Parent;
			}
		}

		public override bool VisitVariableDeclarationStatement(VariableDeclarationStatement varDecl)
		{
			ExpressionCollection initExps = varDecl.InitExpressions; int index = 0;

			foreach(TypeDeclarationExpression typeDecl in varDecl.Declarations)
			{
				if (namescope.IsDefined(typeDecl.Name))
				{
					errorReport.Error( "TODOFILENAME", typeDecl.Position, 
						"Sorry but '{0}' is already defined.", typeDecl.Name );
				}
				else
				{
					// TODO: If its a instance or static, and we're 
					// in an inner block, we need to move these statments
					// to the type/class level statements list

					namescope.AddVariable( typeDecl.Name, typeDecl.TypeReference );
				}

				// Here we are converting expression from 
				// x:int, y:long = 1, 2L
				// to an AST representation equivalent to
				// x:int = 1; y:long = 2L

				if (index < initExps.Count)
				{
					typeDecl.InitExp = initExps[index];
				}

				// TODO: Check if the variable overrides a 
				// variable defined in the parent scope and issue
				// an warning if so

				index++;
			}

			// We don't need them anymore
			initExps.Clear();

			return base.VisitVariableDeclarationStatement(varDecl);
		}

		public override bool VisitAssignmentExpression(AssignmentExpression assignExp)
		{
			return base.VisitAssignmentExpression(assignExp);
		}

		public override bool VisitVariableReferenceExpression(VariableReferenceExpression variableReferenceExpression)
		{
			if (variableReferenceExpression.Type == VariableReferenceType.LocalOrArgument)
			{
				if (!namescope.IsDefined(variableReferenceExpression.Name))
				{
					errorReport.Error( "TODOFILENAME", variableReferenceExpression.Position, 
						"'{0}' is undefined. You can defined it through a formal declaration - '{0}':sometype - or just an " + 
						"assignment ('{0} = something').", variableReferenceExpression.Name );
				}
			}
			else 
			{
				if (!namescope.IsDefinedInParent(variableReferenceExpression.Name))
				{
					errorReport.Error( "TODOFILENAME", variableReferenceExpression.Position, 
						"'{0}' is undefined. You can defined it through a formal declaration - '{0}':sometype, an assignment " + 
						"('{0} = something'), or using the attr family.", variableReferenceExpression.Name );
				}
			}

			return base.VisitVariableReferenceExpression(variableReferenceExpression);
		}

		public override bool VisitMemberAccessExpression(MemberAccessExpression accessExpression)
		{
			return base.VisitMemberAccessExpression(accessExpression);
		}

		public override bool VisitTypeDeclarationExpression(TypeDeclarationExpression typeDeclarationExpression)
		{
			if (!identifierService.IsValidVarOrFieldName( typeDeclarationExpression.Name ))
			{
				errorReport.Error( "TODOFILENAME", 
					typeDeclarationExpression.Position, "Invalid name '{0}'. It possible conflits with an identifier or built-in method.", typeDeclarationExpression.Name );
			}

			return base.VisitTypeDeclarationExpression(typeDeclarationExpression);
		}
	}
}
