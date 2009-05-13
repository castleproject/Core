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
	using System.Globalization;
	using System.IO;
	using System.Threading;
	using NUnit.Framework;
	using NVelocity.App;

	[TestFixture]
	public class AmbiguousExceptionTestCase
	{
		[SetUp]
		public void Init()
		{
			Thread.CurrentThread.CurrentCulture =
				CultureInfo.CreateSpecificCulture("en-us");
		}

		[Test]
		public void DecimalToString()
		{
			StringWriter sw = new StringWriter();

			VelocityContext c = new VelocityContext();
			c.Put("x", (decimal) 1.2);
			c.Put("model", new ModelClass());

			VelocityEngine velocityEngine = new VelocityEngine();
			velocityEngine.Init();

			bool ok = velocityEngine.Evaluate(c, sw,
			                                  "ContextTest.CaseInsensitive",
			                                  "$model.Amount.ToString() \r\n" +
			                                  "$model.Amount.ToString('#0.00') \r\n" +
			                                  "$x.ToString() \r\n" +
			                                  "$x.ToString('#0.00') \r\n");

			Assert.IsTrue(ok, "Evaluation returned failure");
			Assert.AreEqual("1.2 \r\n1.20 \r\n1.2 \r\n1.20 \r\n", sw.ToString());
		}

		[Test]
		public void DuplicateMethodNames()
		{
			StringWriter sw = new StringWriter();

			VelocityContext c = new VelocityContext();
			c.Put("model", new ModelClass());

			VelocityEngine velocityEngine = new VelocityEngine();
			velocityEngine.Init();

			bool ok = velocityEngine.Evaluate(c, sw,
			                                  "ContextTest.CaseInsensitive",
			                                  "$model.DoSome('y') $model.DoSome(2) ");

			Assert.IsTrue(ok, "Evaluation returned failure");
			Assert.AreEqual("x:y 4 ", sw.ToString());
		}
	}

	public interface ISomething
	{
		String DoSome(String x);

		String DoSome(int x);
	}

	public class ModelClass : ISomething
	{
		private decimal amount = (decimal) 1.2;

		public decimal Amount
		{
			get { return amount; }
			set { amount = value; }
		}

		public String DoSome(string x)
		{
			return "x:" + x;
		}

		string ISomething.DoSome(string x)
		{
			return x;
		}

		string ISomething.DoSome(int i)
		{
			return (i * i).ToString();
		}
	}
}