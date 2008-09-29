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
	using Framework.JSGeneration;
	using Framework.JSGeneration.jQuery;
	using NUnit.Framework;

	[TestFixture]
	public class JQueryGeneratorTestCase
	{
		private JSCodeGenerator codeGen;
		private JQueryGenerator sut;

		[SetUp]
		public void BeforeTest()
		{
			sut = CreateSUT();
		}
		public JQueryGenerator CreateSUT()
		{
			codeGen = new JSCodeGenerator();
			return new JQueryGenerator(codeGen);
		}

		[Test]
		public void InsertHtml_SupportsAppend()
		{
			sut.InsertHtml("append", "#foo", "something");
			Assert.AreEqual("jQuery(\"#foo\").append(\"something\");\r\n", codeGen.Lines.ToString());
		}

		[Test]
		public void InsertHtml_SupportsAppendTo()
		{
			sut.InsertHtml("appendTo", "#foo", "something");
			Assert.AreEqual("jQuery(\"something\").appendTo(\"#foo\");\r\n", codeGen.Lines.ToString());
		}

		[Test]
		public void ReplaceHtml()
		{
			sut.ReplaceHtml("#foo", "something");
			Assert.AreEqual("jQuery(\"#foo\").html(\"something\");\r\n", codeGen.Lines.ToString());
		}

		[Test]
		public void Show()
		{
			sut.Show("#foo", "#bar", ".hotdog");
			Assert.AreEqual("jQuery(\"#foo,#bar,.hotdog\").show();\r\n", codeGen.Lines.ToString());
		}
	}
}
