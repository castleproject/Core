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

namespace Castle.MonoRail.Views.Brail.Tests
{
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.IO;
	using NUnit.Framework;

	[TestFixture]
	public class BrailPreProcessorFixture
	{
		[Test]
		public void MR_248_PreProcessorBugWithRegardToNestedExpressions()
		{
			string code =
				@"<script type='text/javascript'>
    Event.observe(window, 'load', function() { ${controlIdToFocus}.focus(); });
</script>";
			string expected = "output \"\"\"<script type='text/javascript'>\r\n" +
			                  "    Event.observe(window, 'load', function() { \"\"\"\r\n" +
							  "output controlIdToFocus\r\n" +
			                  "output \"\"\".focus(); });\r\n" +
			                  "</script>\"\"\"\r\n" ;
			string s = RunThroughPreProcessor(code);
			Assert.AreEqual(expected, s);
		}

		[Test]
		public void TrimWhitespaceAroundMinusBlocks()
		{
			string code =
				@"Foo
  <% Hi -%>
Bar
  <% Yadda %>
Blah";
			string expected = "output \"\"\"Foo\r\n  \"\"\"\r\n" +
			                  " Hi \r\n" +
			                  "output \"\"\"Bar\r\n  \"\"\"\r\n" +
			                  " Yadda \r\n" +
			                  "output \"\"\"\r\nBlah\"\"\"\r\n";
			string s = RunThroughPreProcessor(code);
			Assert.AreEqual(expected, s);
		}

		private static string RunThroughPreProcessor(string code)
		{
			BrailPreProcessor ppc = new BrailPreProcessor(new BooViewEngine());
			CompilerContext context = new CompilerContext();
			context.Parameters.Input.Add(new StringInput("test", code));
			ppc.Initialize(context);
			ppc.Run();
			return context.Parameters.Input[0].Open().ReadToEnd();
		}
	}
}
