using AspectSharp.Lang.AST;
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
	using System.IO;
	using System.Text;

	using AspectSharp.Lang.Steps;

	using NUnit.Framework;

	/// <summary>
	/// Summary description for ParserTestCaseBase.
	/// </summary>
	public abstract class ParserTestCaseBase
	{
		private StringBuilder messages = new StringBuilder();
		protected Context _context;

		[SetUp]
		public void ClearBuffer()
		{
			messages.Length = 0;
		}

		protected AspectParser CreateParser(String content)
		{
			StringReader reader = new StringReader(content);
			return new AspectParser(new AspectLanguageLexer(reader));
		}

		protected void AssertOutput(String expected)
		{
			Assert.AreEqual( expected, messages.ToString() );
		}

		protected EngineConfiguration ProcessContent(string content)
		{
			EngineConfiguration conf = CreateParser( content ).Parse();
	
			StepChainBuilder chain = new StepChainBuilder();
			AddSteps(chain);
	
			_context = new Context();
			_context.Error += new ErrorDelegate(OnError);

			chain.Build().Process(_context, conf);
			return conf;
		}

		protected void OnError(LexicalInfo info, string message)
		{
			messages.Append(message);
			messages.Append(';');
		}

		protected virtual void AddSteps(StepChainBuilder chain)
		{
		}
	}
}
