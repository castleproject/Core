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

namespace Castle.Rook.Compiler.Services.Passes.ILEmission
{
	using System;
	using System.Collections;

	using Castle.Rook.Compiler.AST;
	using Castle.Rook.Compiler.Visitors;


	public class CreateTypesStep : DepthFirstVisitor, ICompilerPassStep
	{
		public CreateTypesStep()
		{
		}

		public void PerformStep(CompilationUnit unit)
		{
			VisitNode(unit);
		}

		public override bool VisitTypeDefinitionStatement(TypeDefinitionStatement typeDef)
		{
			base.VisitTypeDefinitionStatement(typeDef);

			typeDef.Builder.CreateType();

			return true;
		}
	}
}
