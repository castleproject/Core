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

namespace NVelocity.Test
{
	using System;
	using System.Collections;
	using System.Globalization;
	using System.IO;
	using App;
	using NVelocity.Exception;
	using NUnit.Framework;

	[TestFixture]
	public class MethodCallTestCase
	{
		private VelocityEngine velocityEngine;
		private VelocityContext c;

		[TestFixtureSetUp]
		public void StartNVelocity()
		{
			velocityEngine = new VelocityEngine();
			velocityEngine.Init();
		}

		[SetUp]
		public void ResetContext()
		{
			c = new VelocityContext();
			c.Put("test", new TestClass());
		}

		[Test]
		public void HasExactSignature()
		{
			double num = 55.0;
			c.Put("num", num);
			Assert.AreEqual("55", Eval("$test.justDoIt($num)"));
		}

		[Test]
		public void HasExactSignatureWithCorrectCase()
		{
			double num = 55.0;
			c.Put("num", num);
			Assert.AreEqual("55", Eval("$test.JustDoIt($num)"));
		}

		[Test]
		public void HasExactSignatureWithMessedUpCase()
		{
			double num = 55.0;
			c.Put("num", num);
			Assert.AreEqual("55", Eval("$test.jUStDoIt($num)"));
		}

		[Test]
		public void HasCompatibleSignature()
		{
			int num = 99;
			c.Put("num", num);
			Assert.AreEqual("99", Eval("$test.justDoIt($num)"));
		}

		[Test]
		public void NoAmbiguityTest()
		{
			int num = 99;
			c.Put("num", num);
			Assert.AreEqual("Int32", Eval("$test.Amb($num)"));
			Assert.AreEqual("HybridDictionary", Eval("$test.Amb(\"%{id=1}\")"));
		}

		[Test]
		public void CorrectExceptionThrownOnInvocationException()
		{
			try
			{
				Eval("$test.ThrowException");
				Assert.Fail();
			}
			catch (MethodInvocationException miex)
			{
				Assert.AreEqual("From ThrowException", miex.InnerException.Message);
			}
		}

		[Test]
		[Ignore("mono issues")]
		public void HasRelaxedSignature()
		{
			string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
			DirectoryInfo di = new DirectoryInfo(path);
			c.Put("di", di);
			Assert.AreEqual(path, Eval("$test.justDoIt($di)"));
		}

		[Test]
		[Ignore("mono issues")]
		public void HasRelaxedSignatureWithCorrectCase()
		{
			string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
			DirectoryInfo di = new DirectoryInfo(path);
			c.Put("di", di);
			Assert.AreEqual(path, Eval("$test.JustDoIt($di)"));
		}

		[Test]
		[Ignore("mono issues")]
		public void HasRelaxedSignatureWithMessedUpCase()
		{
			string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
			DirectoryInfo di = new DirectoryInfo(path);
			c.Put("di", di);
			Assert.AreEqual(path, Eval("$test.juSTDOIt($di)"));
		}

		public class TestClass
		{
			public string JustDoIt(double obj)
			{
				return Convert.ToString(obj, CultureInfo.InvariantCulture);
			}

			public string JustDoIt(object obj)
			{
				return Convert.ToString(obj, CultureInfo.InvariantCulture);
			}

			public string Amb(object obj)
			{
				return obj.GetType().Name;
			}

			public string Amb(IDictionary obj)
			{
				return obj.GetType().Name;
			}

			public string ThrowException()
			{
				throw new Exception("From ThrowException");
			}
		}

		private string Eval(string template)
		{
			using(StringWriter sw = new StringWriter())
			{
				bool ok = velocityEngine.Evaluate(c, sw, "ContextTest.CaseInsensitive", template);

				Assert.IsTrue(ok, "Evaluation returned failure");

				return sw.ToString();
			}
		}
	}
}