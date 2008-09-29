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
	public class JQueryElementGeneratorTestCase
	{
		private JSCodeGenerator jsCodeGenerator;
		private JQueryElementGenerator sut;

		[SetUp]
		public void BeforeTest()
		{
			sut = CreateSUT();
		}

		[TearDown]
		public void AfterTest()
		{
		}

		public JQueryElementGenerator CreateSUT()
		{
			jsCodeGenerator = new JSCodeGenerator();
			JQueryGenerator generator = new JQueryGenerator(jsCodeGenerator);
			return (JQueryElementGenerator)generator.CreateElementGenerator("#my_root");
		}

		[Test]
		public void Replace_GeneratesCorrectJSCall()
		{
			sut.Replace("By this");
			Assert.AreEqual("jQuery(\"#my_root\").replaceWith(\"By this\");\r\n", jsCodeGenerator.Lines.ToString());
		}

		[Test]
		public void ReplaceHtml_GeneratesCorrectJSCall()
		{
			sut.ReplaceHtml("By this");

			Assert.AreEqual("jQuery(\"#my_root\").html(\"By this\");\r\n", jsCodeGenerator.Lines.ToString());
		}
	}
}
