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

namespace Castle.Rook.Compiler.Tests.AnnotatedTree
{
	using System;

	using NUnit.Framework;

	using Castle.Rook.Compiler.AST;
	using Castle.Rook.Compiler.Services.Passes;


	[TestFixture]
	public class VarScopesTestCase : AbstractContainerTestCase
	{
		[Test]
		public void VariableBindings()
		{
			CompilationUnit unit = container.ParserService.Parse("x:int = 1\r\nputs(x)\r\n");

			AssertNoErrorOrWarnings();

			Assert.IsNotNull(unit);
			Assert.AreEqual(2, unit.Statements.Count);
		}	


		[Test]
		public void MethodUsingGlobals()
		{
			String contents = 
				"@x:int = 1        \r\n" + 
				"                 \r\n" + 
				"def some()       \r\n" + 
				"  @x += 1         \r\n" + 
				"end              \r\n" + 
				"";

			CompilationUnit unit = container.ParserService.Parse(contents);

			AssertNoErrorOrWarnings();

			Assert.IsNotNull(unit);
			Assert.AreEqual(2, unit.Statements.Count);

			ScopeBinding sb = container[ typeof(ScopeBinding) ] as ScopeBinding;

			sb.ExecutePass(unit);

			AssertNoErrorOrWarnings();

			// String message = container.ErrorReport.ErrorSBuilder.ToString();
			// Assert.AreEqual(":2,24\terror:  unexpected token: [\"end\",<6>,line=2,col=24]\r\n", message);
		}
	}
}
