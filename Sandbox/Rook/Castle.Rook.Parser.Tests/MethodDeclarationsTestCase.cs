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

namespace Castle.Rook.Parse.Tests
{
	using System;

	using NUnit.Framework;

	using Castle.Rook.AST;

	[TestFixture]
	public class MethodDeclarationsTestCase
	{
		[Test]
		public void MethodDefWithReturnType()
		{
			String contents = 
				"class MyClass \r\n" + 
				"" + 
				" def self.method1() : int" + 
				" end " + 
				"" + 
				"end \r\n";

			CompilationUnitNode unit = RookParser.ParseContents(contents);
			Assert.IsNotNull(unit);
			Assert.AreEqual(0, unit.Namespaces.Count);
			Assert.AreEqual(1, unit.ClassesTypes.Count);
			
			ClassDefinitionStatement ClassDefinitionStatement = unit.ClassesTypes[0] as ClassDefinitionStatement;
			Assert.IsNotNull(ClassDefinitionStatement);
			Assert.AreEqual("MyClass", ClassDefinitionStatement.Name);
			Assert.AreEqual(0, ClassDefinitionStatement.BaseTypes.Count);
			Assert.AreEqual(1, ClassDefinitionStatement.Statements.Count);
			
			MethodDefinitionStatement stmt = ClassDefinitionStatement.Statements[0] as MethodDefinitionStatement;
			Assert.IsNotNull(stmt);
		}

		[Test]
		public void MethodDefWithArguments()
		{
			String contents = 
				"class MyClass \r\n" + 
				"" + 
				" def self.method1(x, y, z: int) : int" + 
				" end " + 
				"" + 
				"end \r\n";

			CompilationUnitNode unit = RookParser.ParseContents(contents);
			Assert.IsNotNull(unit);
			Assert.AreEqual(0, unit.Namespaces.Count);
			Assert.AreEqual(1, unit.ClassesTypes.Count);
			
			ClassDefinitionStatement ClassDefinitionStatement = unit.ClassesTypes[0] as ClassDefinitionStatement;
			Assert.IsNotNull(ClassDefinitionStatement);
			Assert.AreEqual("MyClass", ClassDefinitionStatement.Name);
			Assert.AreEqual(0, ClassDefinitionStatement.BaseTypes.Count);
			Assert.AreEqual(1, ClassDefinitionStatement.Statements.Count);

			MethodDefinitionStatement stmt = ClassDefinitionStatement.Statements[0] as MethodDefinitionStatement;
			Assert.IsNotNull(stmt);
		}	

		[Test]
		public void MethodDefAndStatements()
		{
			String contents = 
				"class MyClass \r\n" + 
				"" + 
				" def method1(x, y, z:int):int" + 
				"     " + 
				"   def method1(x, y, z:int):int  " + 
				"   end  " + 
				"   myvariable = 1  " + 
				" end " + 
				"" + 
				"end \r\n";

			CompilationUnitNode unit = RookParser.ParseContents(contents);
			Assert.IsNotNull(unit);
			Assert.AreEqual(0, unit.Namespaces.Count);
			Assert.AreEqual(1, unit.ClassesTypes.Count);
			
			ClassDefinitionStatement ClassDefinitionStatement = unit.ClassesTypes[0] as ClassDefinitionStatement;
			Assert.IsNotNull(ClassDefinitionStatement);
			Assert.AreEqual("MyClass", ClassDefinitionStatement.Name);
			Assert.AreEqual(0, ClassDefinitionStatement.BaseTypes.Count);
		}

		[Test]
		public void MethodDefAndStatements2()
		{
			String contents = 
				"class MyClass \r\n" + 
				"" + 
				" def method1(x:long, y:string, z:int) :int" + 
				"     " + 
				"     x.call()" + 
				"     " + 
				" end " + 
				"" + 
				"end \r\n";

			CompilationUnitNode unit = RookParser.ParseContents(contents);
			Assert.IsNotNull(unit);
			Assert.AreEqual(0, unit.Namespaces.Count);
			Assert.AreEqual(1, unit.ClassesTypes.Count);
			
			ClassDefinitionStatement ClassDefinitionStatement = unit.ClassesTypes[0] as ClassDefinitionStatement;
			Assert.IsNotNull(ClassDefinitionStatement);
			Assert.AreEqual("MyClass", ClassDefinitionStatement.Name);
			Assert.AreEqual(0, ClassDefinitionStatement.BaseTypes.Count);
		}

		[Test]
		public void MethodDefAndStatements3()
		{
			String contents = 
				"class MyClass \r\n" + 
				"" + 
				" def method1(x, y, z: int): System.Int32" + 
				"     " + 
				"     self.method2(10, 11)" + 
				"     self.propertyx.save(100)" + 
				"     self(100)" + 
				"     " + 
				" end " + 
				"" + 
				"end \r\n";

			CompilationUnitNode unit = RookParser.ParseContents(contents);
			Assert.IsNotNull(unit);
			Assert.AreEqual(0, unit.Namespaces.Count);
			Assert.AreEqual(1, unit.ClassesTypes.Count);
			
			ClassDefinitionStatement ClassDefinitionStatement = unit.ClassesTypes[0] as ClassDefinitionStatement;
			Assert.IsNotNull(ClassDefinitionStatement);
			Assert.AreEqual("MyClass", ClassDefinitionStatement.Name);
			Assert.AreEqual(0, ClassDefinitionStatement.BaseTypes.Count);
		}

		[Test]
		public void Block()
		{
			String contents = 
				"class MyClass \r\n" + 
				"" + 
				" def method1() : void" + 
				"     " + 
				"     do " + 
				"       method(10, 12);  " + 
				"     end " + 
				"     " + 
				" end " + 
				"" + 
				"end \r\n";

			CompilationUnitNode unit = RookParser.ParseContents(contents);
			Assert.IsNotNull(unit);
			Assert.AreEqual(0, unit.Namespaces.Count);
			Assert.AreEqual(1, unit.ClassesTypes.Count);
			
			ClassDefinitionStatement ClassDefinitionStatement = unit.ClassesTypes[0] as ClassDefinitionStatement;
			Assert.IsNotNull(ClassDefinitionStatement);
			Assert.AreEqual("MyClass", ClassDefinitionStatement.Name);
			Assert.AreEqual(0, ClassDefinitionStatement.BaseTypes.Count);
		}
	}
}
