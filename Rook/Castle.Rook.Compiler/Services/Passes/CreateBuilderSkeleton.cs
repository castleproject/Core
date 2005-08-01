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
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Threading;

	using Castle.Rook.Compiler.AST;
	using Castle.Rook.Compiler.TypeSystem;


	public class CreateBuilderSkeleton : ICompilerPass
	{
		private readonly ITypeContainer typeContainer;
		private readonly INameResolver resolver;
		private readonly IErrorReport errorReport;

		public CreateBuilderSkeleton(ITypeContainer typeContainer, 
			INameResolver resolver, IErrorReport errorReport)
		{
			this.typeContainer = typeContainer;
			this.resolver = resolver;
			this.errorReport = errorReport;
		}

		public void ExecutePass(CompilationUnit unit)
		{
			DeclareAndPopulateGlobalSourceUnit(unit);

			AssemblyName assemblyName = new AssemblyName();
			
			assemblyName.Name = "RookGenAssembly";

			AssemblyBuilder assembly = Thread.GetDomain().DefineDynamicAssembly( 
				assemblyName, AssemblyBuilderAccess.Save );

			ModuleBuilder module = assembly.DefineDynamicModule(
				"RookModule", "RookModule.mod", true);

			unit.AssemblyBuilder = assembly;

			unit.ModuleBuilder = module;

			TypeBuilderSkeletonStep builderVisitor = 
				new TypeBuilderSkeletonStep( module, typeContainer, resolver, errorReport );

			builderVisitor.VisitNode( unit );
		}

		private void DeclareAndPopulateGlobalSourceUnit(CompilationUnit unit)
		{
			SourceUnit globalUnit = new SourceUnit(unit, "<global>");
	
			TypeDefinitionStatement globalType = new TypeDefinitionStatement( AccessLevel.Public, "RookGlobal" );
	
			globalUnit.Statements.Add( globalType );
	
			MethodDefinitionStatement entryPoint = new MethodDefinitionStatement( AccessLevel.Public );
			entryPoint.Name = "self.main";
	
			foreach(SourceUnit sunit in unit.SourceUnits)
			{
				foreach(IStatement stmt in sunit.Statements)
				{
					if (stmt.StatementType == StatementType.TypeDefinition) continue;

					stmt.Parent.RemoveChild( stmt );

					if (stmt.StatementType == StatementType.MethodDef)
					{
						// (stmt as MethodDefinitionStatement).IsStatic = true;
					}
					if (stmt.StatementType == StatementType.MultipleVarDeclaration)
					{
						// (stmt as MultipleVariableDeclarationStatement).IsStatic = true;
					}

					if (stmt.StatementType == StatementType.ExpressionStmt || 
						stmt.StatementType == StatementType.MultipleVarDeclaration )
					{
						entryPoint.Statements.Add( stmt );
					}
					else
					{
						globalType.Statements.Add( stmt );
					}
				}
			}
	
			if (entryPoint.Statements.Count != 0)
			{
				globalType.Statements.Add( entryPoint );

				// Might be necessary
				// unit.EntryPointMethod = entryPoint;
			}
	
			unit.SourceUnits.Add(globalUnit);
		}
	}
}