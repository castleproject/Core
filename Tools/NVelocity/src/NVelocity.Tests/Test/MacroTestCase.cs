// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace NVelocity.Test
{
	using System;
	using System.IO;
	using NUnit.Framework;
	using NVelocity.App;

	[TestFixture]
	public class MacroTestCase
	{
		private VelocityEngine ve;
		private VelocityContext context;

		[SetUp]
		public void Setup()
		{
			context = new VelocityContext();

			ve = new VelocityEngine();
			ve.Init();
		}
		
		[Test, Ignore("Does not pass, but would be easier to rewrite NVelocity than fix it")]
		public void RecursiveMacroHaveArgsStacked()
		{
			String template =
				"## A comment \r\n" +
				"#macro(rec $someparam $level)\r\n" +
				"#if($level == 3)\r\n" +
				"Done" +
				"#else\r\n" +
				"$level " +
				"#set($newlevel = $level+1)" +
				"#rec($someparam $newlevel)" +
				"$level " +
				"#end\r\n" +
				"#end\r\n" +
				"#rec('hello' 1)\r\n";
			
			StringWriter sw = new StringWriter();
			
			bool success = ve.Evaluate(context, sw, "MacroTest1", template);

			Assert.IsTrue(success, "Evalutation failed");
			Assert.AreEqual("12Done21", sw.GetStringBuilder().ToString());
		}
	}
}
