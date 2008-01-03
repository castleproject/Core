// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	using NUnit.Framework;

	[TestFixture]
	public class JSCodeGeneratorTestCase
	{
		private JSCodeGenerator codeGen;

		[SetUp]
		public void Init()
		{
			codeGen = new JSCodeGenerator();
		}

		[Test]
		public void Call_CorrectlyRecordsFunctionCall()
		{
			codeGen.Call("alert", "'hey'");

			Assert.AreEqual("alert('hey');\r\n", codeGen.Lines.ToString());
		}

		[Test]
		public void Call_CorrectlyRecordsFunctionCallsInARow()
		{
			codeGen.Call("alert", "'hey'");
			codeGen.Call("window.href.change");
			codeGen.Call("math.sum", 1, 2);

			Assert.AreEqual("alert('hey');\r\nwindow.href.change();\r\nmath.sum(1,2);\r\n", codeGen.Lines.ToString());
		}

		[Test]
		public void Write_JustTransferContentToBuffer()
		{
			codeGen.Write("something");

			Assert.AreEqual("something", codeGen.Lines.ToString());
		}

		[Test]
		public void Record_JustTransferContentToBufferWithNewLine()
		{
			codeGen.AppendLine("something");

			Assert.AreEqual("something;\r\n", codeGen.Lines.ToString());
		}

		[Test]
		public void AppendLine_JustTransferContentToBufferWithNewLine()
		{
			codeGen.AppendLine("something");

			Assert.AreEqual("something;\r\n", codeGen.Lines.ToString());
		}

		[Test]
		public void ReplaceTailByPeriod_FixesTheContentCorrectly()
		{
			codeGen.AppendLine("something");
			codeGen.ReplaceTailByPeriod();

			Assert.AreEqual("something.", codeGen.Lines.ToString());
		}

		[Test]
		public void ReplaceTailByPeriod_IgnoresEmptyBuffer()
		{
			codeGen.ReplaceTailByPeriod();

			Assert.AreEqual("", codeGen.Lines.ToString());
		}

		[Test]
		public void RemoveTail_FixesTheContentCorrectly()
		{
			codeGen.AppendLine("something");
			codeGen.RemoveTail();

			Assert.AreEqual("something", codeGen.Lines.ToString());
		}

		[Test]
		public void GenerateFinalJsCode_CanGenerateEmptyCall()
		{
			string expected = "try \n{\n}\ncatch(e)\n{\n" + 
				"alert('JS error ' + e.toString());\n" +
				"alert(\"Generated content: \\n\");\n}";

			Assert.AreEqual(expected, codeGen.GenerateFinalJsCode());
		}
	}
}
