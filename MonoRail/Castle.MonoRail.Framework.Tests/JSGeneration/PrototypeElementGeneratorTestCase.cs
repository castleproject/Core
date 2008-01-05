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
	using Castle.MonoRail.Framework.JSGeneration;
	using Castle.MonoRail.Framework.JSGeneration.Prototype;
	using NUnit.Framework;

	[TestFixture]
	public class PrototypeElementGeneratorTestCase
	{
		private PrototypeElementGenerator generator;
		private JSCodeGenerator codeGen;

		[SetUp]
		public void Init()
		{
			codeGen = new JSCodeGenerator();
			generator = new PrototypeElementGenerator(new PrototypeGenerator(codeGen), "my_root");
		}

		[Test]
		public void CreationShouldResultInAccess()
		{
			Assert.AreEqual("$('my_root');\r\n", codeGen.Lines.ToString());
		}

		[Test]
		public void Replace_GeneratesCorrectJSCall()
		{
			generator.Replace("By this");

			Assert.AreEqual("$('my_root').replace(\"By this\");\r\n", codeGen.Lines.ToString());
		}

		[Test]
		public void ReplaceHtml_GeneratesCorrectJSCall()
		{
			generator.ReplaceHtml("By this");

			Assert.AreEqual("$('my_root').update(\"By this\");\r\n", codeGen.Lines.ToString());
		}
	}
}
