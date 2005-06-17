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

namespace AspectSharp.Lang.Tests
{
	using System;

	using antlr;

	using NUnit.Framework;

	using AspectSharp.Lang.AST;

	/// <summary>
	/// Summary description for ParserImportTestCase.
	/// </summary>
	[TestFixture]
	public class ParserImportTestCase : ParserTestCaseBase
	{
		[Test]
		public void ParsingImportDeclarations()
		{
			AspectParser parser = CreateParser("import my.ns.name");
			EngineConfiguration conf = parser.Parse();
			Assert.IsNotNull(conf);
			Assert.IsNotNull(conf.Imports);
			Assert.AreEqual(1, conf.Imports.Count);
			ImportDirective import = conf.Imports[0];
			Assert.IsNotNull(import);
			Assert.AreEqual("my.ns.name", import.Namespace);
		}

		[Test]
		public void ParsingImportDeclarationsWithAssemblies()
		{
			AspectParser parser = CreateParser("import my.ns.name in my.assembly.Name");
			EngineConfiguration conf = parser.Parse();
			Assert.IsNotNull(conf);
			Assert.IsNotNull(conf.Imports);
			Assert.AreEqual(1, conf.Imports.Count);
			ImportDirective import = conf.Imports[0];
			Assert.IsNotNull(import);
			Assert.AreEqual("my.ns.name", import.Namespace);
			Assert.AreEqual("my.assembly.Name", import.AssemblyReference.AssemblyName);
		}

		[Test]
		public void ParsingSimpleImportDeclarations()
		{
			AspectParser parser = CreateParser("import my\r\nimport two");
			EngineConfiguration conf = parser.Parse();
			Assert.IsNotNull(conf);
			Assert.IsNotNull(conf.Imports);
			Assert.AreEqual(2, conf.Imports.Count);
			ImportDirective import = conf.Imports[0];
			Assert.IsNotNull(import);
			Assert.AreEqual("my", import.Namespace);
			import = conf.Imports[1];
			Assert.IsNotNull(import);
			Assert.AreEqual("two", import.Namespace);
		}

		[Test]
		[ExpectedException(typeof(MismatchedTokenException))]
		public void InvalidImportDeclaration()
		{
			AspectParser parser = CreateParser("import \r\nimport \r\n");
			parser.Parse();
		}
	}
}
