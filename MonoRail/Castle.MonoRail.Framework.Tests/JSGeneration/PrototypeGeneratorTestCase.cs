// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Tests.JSGeneration
{
	using System;
	using Castle.MonoRail.Framework.JSGeneration;
	using Castle.MonoRail.Framework.JSGeneration.Prototype;
	using NUnit.Framework;

	[TestFixture]
	public class PrototypeGeneratorTestCase
	{
		private PrototypeGenerator generator;
		private JSCodeGenerator codeGen;

		[SetUp]
		public void Init()
		{
			codeGen = new JSCodeGenerator();
			generator = new PrototypeGenerator(codeGen);
		}

		[Test]
		public void InsertHtml_SupportsTopPosition()
		{
			generator.InsertHtml("Top", "myid", "something");
			Assert.AreEqual("new Insertion.Top(\"myid\",\"something\");\r\n", codeGen.Lines.ToString());
		}

		[Test]
		public void InsertHtml_SupportsBottomPosition()
		{
			generator.InsertHtml("bottom", "myid", "something");
			Assert.AreEqual("new Insertion.Bottom(\"myid\",\"something\");\r\n", codeGen.Lines.ToString());
		}

		[Test]
		public void InsertHtml_SupportsBeforePosition()
		{
			generator.InsertHtml("Before", "myid", "something");
			Assert.AreEqual("new Insertion.Before(\"myid\",\"something\");\r\n", codeGen.Lines.ToString());
		}

		[Test]
		public void InsertHtml_SupportsAfterPosition()
		{
			generator.InsertHtml("AFTER", "myid", "something");
			Assert.AreEqual("new Insertion.After(\"myid\",\"something\");\r\n", codeGen.Lines.ToString());
		}

		[Test, ExpectedException(typeof(ArgumentException))]
		public void InsertHtml_DoesNotSupportAnyOtherPosition()
		{
			generator.InsertHtml("Head", "myid", "something");
		}

		[Test]
		public void ReplaceHtml_GeneratesCorrectCallUsingIdAndContent()
		{
			generator.ReplaceHtml("myid", "something");
			Assert.AreEqual("Element.update(\"myid\",\"something\");\r\n", codeGen.Lines.ToString());
		}

		[Test]
		public void Replace_GeneratesCorrectCallUsingIdAndContent()
		{
			generator.Replace("myid", "something");
			Assert.AreEqual("Element.replace(\"myid\",\"something\");\r\n", codeGen.Lines.ToString());
		}

		[Test]
		public void Show_GeneratesCorrectCallForAllIds()
		{
			generator.Show("myid", "otherid");
			Assert.AreEqual("Element.show(\"myid\",\"otherid\");\r\n", codeGen.Lines.ToString());
		}

		[Test]
		public void Hide_GeneratesCorrectCallForAllIds()
		{
			generator.Hide("myid", "otherid");
			Assert.AreEqual("Element.hide(\"myid\",\"otherid\");\r\n", codeGen.Lines.ToString());
		}

		[Test]
		public void Toggle_GeneratesCorrectCallForAllIds()
		{
			generator.Toggle("myid", "otherid");
			Assert.AreEqual("Element.toggle(\"myid\",\"otherid\");\r\n", codeGen.Lines.ToString());
		}

		[Test]
		public void Remove_GeneratesCorrectCallForAllIds()
		{
			generator.Remove("myid", "otherid");
			Assert.AreEqual("[\"myid\",\"otherid\"].each(Element.remove);\r\n", codeGen.Lines.ToString());
		}

		[Test]
		public void CreateElementGenerator_CreatesPrototypeElementGenerator()
		{
			object elementGen = generator.CreateElementGenerator("my.root");
			Assert.IsInstanceOfType(typeof(PrototypeElementGenerator), elementGen);
		}
	}
}
