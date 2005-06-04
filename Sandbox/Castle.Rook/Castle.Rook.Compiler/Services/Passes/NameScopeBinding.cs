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


	public class NameScopeBinding : BreadthFirstVisitor, ICompilerPass
	{
		private readonly IErrorReport errorReport;

		public NameScopeBinding(IErrorReport errorReport)
		{
			this.errorReport = errorReport;
		}

		public void ExecutePass(CompilationUnit unit)
		{
			VisitNode(unit);
		}

		public override bool VisitTypeDefinitionStatement(TypeDefinitionStatement typeDef)
		{
			INameScope namescope = (typeDef.Parent as INameScopeAccessor).Namescope;
			
			if (namescope.HasTypeDefinition(typeDef.Name))
			{
				errorReport.Error("TODOFILENAME", typeDef.Position, "This identifier in being used elsewhere.", typeDef.Name);
			}
			else
			{
				namescope.AddTypeDefinition(typeDef);
			}

			return base.VisitTypeDefinitionStatement(typeDef);
		}

		public override bool VisitNamespace(NamespaceDeclaration ns)
		{
			INameScope namescope = (ns.Parent as INameScopeAccessor).Namescope;
			
			if (!namescope.HasNamespace(ns.Name))
			{
				namescope.AddNamespace(ns);
			}

			return base.VisitNamespace(ns);
		}

		public override bool VisitMethodDefinitionStatement(MethodDefinitionStatement methodDef)
		{
			INameScope namescope = (methodDef.Parent as INameScopeAccessor).Namescope;
			
			// TODO: Support overloads!
			if (namescope.HasMethod(methodDef.Name))
			{
				errorReport.Error("TODOFILENAME", methodDef.Position, "This identifier in being used elsewhere.", methodDef.Name);
			}
			else
			{
				namescope.AddMethodDefintion(methodDef);
			}

			return base.VisitMethodDefinitionStatement(methodDef);
		}

		public override bool VisitVariableReferenceExpression(VariableReferenceExpression variableReferenceExpression)
		{
			return base.VisitVariableReferenceExpression(variableReferenceExpression);
		}
	}
}
