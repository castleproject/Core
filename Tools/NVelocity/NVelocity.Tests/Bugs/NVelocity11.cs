// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace NVelocity.Bugs
{
	using System.Collections;
	using System.IO;
	using NUnit.Framework;
	using NVelocity.App;

	[TestFixture]
	public class NVelocity11
	{
		[Test]
		public void DoesNotGiveNullReferenceExceptionWhenAmbiguousMatchBecauseOfNullArg()
		{
			VelocityEngine engine = new VelocityEngine();
			engine.Init();

			VelocityContext ctx = new VelocityContext();
			ctx.Put("test", new TestClass());
			ctx.Put("nullObj", null);
			StringWriter writer = new StringWriter();

			Assert.IsTrue(engine.Evaluate(ctx, writer, "testEval", "$test.DoSomething($nullObj)"));
			Assert.AreEqual("$test.DoSomething($nullObj)", writer.GetStringBuilder().ToString());
		}

		public class TestClass
		{
			public string DoSomething(IList l)
			{
				return "Some List";
			}

			public string DoSomething(IDictionary parameters)
			{
				return "Some Parameters";
			}
		}
	}
}
