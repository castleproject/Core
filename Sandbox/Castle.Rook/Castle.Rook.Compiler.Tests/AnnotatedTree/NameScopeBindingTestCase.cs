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
	public class NameScopeBindingTestCase : AbstractContainerTestCase
	{
		[Test]
		public void GlobalMethods()
		{
			String contents = 
				"@x:int = 1       \r\n" + 
				"                 \r\n" + 
				"def some()       \r\n" + 
				"  @x += 1        \r\n" + 
				"  puts(@x)       \r\n" + 
				"end              \r\n" + 
				"";

			CompilationUnit unit = container.ParserService.Parse(contents);

			AssertNoErrorOrWarnings();

			Assert.IsNotNull(unit);
			Assert.AreEqual(2, unit.Statements.Count);

			RunPasses(unit);

			AssertNoErrorOrWarnings();
		}

		[Test]
		public void UndeclaredUse1()
		{
			String contents = 
				"@x:int = 1       \r\n" + 
				"                 \r\n" + 
				"def some()       \r\n" + 
				"  @x += 1        \r\n" + 
				"  puts(x)        \r\n" + 
				"end              \r\n" + 
				"";

			CompilationUnit unit = container.ParserService.Parse(contents);

			AssertNoErrorOrWarnings();

			Assert.IsNotNull(unit);
			Assert.AreEqual(2, unit.Statements.Count);

			RunPasses(unit);

			String message = container.ErrorReport.ErrorSBuilder.ToString();
			Assert.AreEqual("TODOFILENAME:0\terror:  Sorry but 'x' is undefined. Perhaps you meant '@x'? \r\n", message);
		}

		[Test]
		public void FormalArgumentsUsage()
		{
			String contents = 
				"def some(x:int, y:int) \r\n" + 
				"  puts(x + y)          \r\n" + 
				"end                    \r\n" + 
				"";

			CompilationUnit unit = container.ParserService.Parse(contents);

			AssertNoErrorOrWarnings();

			Assert.IsNotNull(unit);
			Assert.AreEqual(2, unit.Statements.Count);

			RunPasses(unit);

			AssertNoErrorOrWarnings();
		}

		[Test]
		public void TypeNameCollisions()
		{
			String contents = 
				"class Controller           \r\n" + 
				"end                        \r\n" + 
				"                           \r\n" + 
				"class AbstractController   \r\n" + 
				"end                        \r\n" + 
				"                           \r\n" + 
				"class Controller           \r\n" + 
				"end                        \r\n" + 
				"                           \r\n" + 
				"";

			CompilationUnit unit = container.ParserService.Parse(contents);

			AssertNoErrorOrWarnings();

			Assert.IsNotNull(unit);
			Assert.AreEqual(2, unit.Statements.Count);

			RunPasses(unit);

			String message = container.ErrorReport.ErrorSBuilder.ToString();
			Assert.AreEqual("TODOFILENAME:0\terror:  Duplicate declaration of type 'Controller' \r\n", message);
		}

		[Test]
		public void MethodNameCollisions()
		{
			String contents = 
				"class Controller           \r\n" + 
				"                           \r\n" + 
				"  def method1()            \r\n" + 
				"  end                      \r\n" + 
				"                           \r\n" + 
				"  def method1()            \r\n" + 
				"  end                      \r\n" + 
				"                           \r\n" + 
				"end                        \r\n" + 
				"                           \r\n" + 
				"";

			CompilationUnit unit = container.ParserService.Parse(contents);

			AssertNoErrorOrWarnings();

			Assert.IsNotNull(unit);
			Assert.AreEqual(2, unit.Statements.Count);

			RunPasses(unit);

			String message = container.ErrorReport.ErrorSBuilder.ToString();
			Assert.AreEqual("TODOFILENAME:0\terror:  Duplicate declaration of method 'method1' \r\n", message);
		}

		[Test]
		public void ValidQualifiedTypes()
		{
			String contents = 
				"namespace X1               \r\n" + 
				"  class Controller         \r\n" + 
				"  end                      \r\n" + 
				"end                        \r\n" + 
				"                           \r\n" + 
				"namespace X2               \r\n" + 
				"  class Controller         \r\n" + 
				"  end                      \r\n" + 
				"end                        \r\n" + 
				"";

			CompilationUnit unit = container.ParserService.Parse(contents);

			AssertNoErrorOrWarnings();

			Assert.IsNotNull(unit);
			Assert.AreEqual(2, unit.Statements.Count);

			RunPasses(unit);

			AssertNoErrorOrWarnings();
		}

		[Test]
		public void QualifiedTypeAccess()
		{
			String contents = 
				"System::Console.Write(\"hi\")       \r\n" + 
				"System::Console.WriteLine(\"hi\")   \r\n" + 
				"";

			CompilationUnit unit = container.ParserService.Parse(contents);

			AssertNoErrorOrWarnings();

			Assert.IsNotNull(unit);
			Assert.AreEqual(2, unit.Statements.Count);

			RunPasses(unit);

			AssertNoErrorOrWarnings();
		}

		[Test]
		public void InvalidQualifiedTypeAccess()
		{
			String contents = 
				"System.Console.Write(\"hi\")       \r\n" + 
				"System.Console.WriteLine(\"hi\")   \r\n" + 
				"";

			CompilationUnit unit = container.ParserService.Parse(contents);

			AssertNoErrorOrWarnings();

			Assert.IsNotNull(unit);
			Assert.AreEqual(2, unit.Statements.Count);

			RunPasses(unit);

			String message = container.ErrorReport.ErrorSBuilder.ToString();
			Assert.AreEqual("TODOFILENAME:0\terror:  No type 'System' found, perhaps " + 
				"you want to access a namespace. If so, use System::ClassOrNamespace instead. \r\n", message);
		}

		[Test]
		public void RequireStatement()
		{
			String contents = 
				"require System              \r\n" +
				"                            \r\n" +
				"Console.Write(\"hi\")       \r\n" + 
				"Console.WriteLine(\"hi\")   \r\n" + 
				"                            \r\n" +
				"require System.Collections  \r\n" +
				"                            \r\n" +
				"x:IList                     \r\n" +
				"                            \r\n" +
				"ArrayList.Adapter( x )      \r\n" +
				"                            \r\n" +
				"";

			CompilationUnit unit = container.ParserService.Parse(contents);

			AssertNoErrorOrWarnings();

			Assert.IsNotNull(unit);
			Assert.AreEqual(2, unit.Statements.Count);

			RunPasses(unit);

			String message = container.ErrorReport.ErrorSBuilder.ToString();
			Assert.AreEqual("TODOFILENAME:0\terror:  No type 'System' found, perhaps " + 
				"you want to access a namespace. If so, use System::ClassOrNamespace instead. \r\n", message);
		}

		[Test]
		public void NamespaceHierarchy()
		{
			String contents = 
				"namespace Parent                    \r\n" + 
				"  class BaseController              \r\n" + 
				"  end                               \r\n" + 
				"end                                 \r\n" + 
				"                                    \r\n" + 
				"namespace Parent.NestedLevel        \r\n" + 
				"  class Controller < BaseController \r\n" + 
				"  end                               \r\n" + 
				"end                                 \r\n" + 
				"";

			CompilationUnit unit = container.ParserService.Parse(contents);

			AssertNoErrorOrWarnings();

			Assert.IsNotNull(unit);
			Assert.AreEqual(2, unit.Statements.Count);

			RunPasses(unit);

			AssertNoErrorOrWarnings();
		}


		private void RunPasses(CompilationUnit unit)
		{
			DeclarationBinding sb = container[ typeof(DeclarationBinding) ] as DeclarationBinding;
			NameScopeBinding ns = container[ typeof(NameScopeBinding) ] as NameScopeBinding;
	
			sb.ExecutePass(unit);
			ns.ExecutePass(unit);
		}
	}
}
