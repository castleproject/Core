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
		public void MethodDef1()
		{
			String contents = 
				"class MyClass \r\n" + 
				"" + 
				" def self.method1()" + 
				"" + 
				"" + 
				"end \r\n";

			CompilationUnitNode unit = RookParser.ParseContents(contents);
			Assert.IsNotNull(unit);
			Assert.AreEqual(0, unit.Namespaces.Count);
			Assert.AreEqual(1, unit.ClassesTypes.Count);
			
			ClassNode classNode = unit.ClassesTypes[0] as ClassNode;
			Assert.IsNotNull(classNode);
			Assert.AreEqual("MyClass", classNode.Name);
			Assert.AreEqual(0, classNode.BaseTypes.Count);
		}	
	}
}
