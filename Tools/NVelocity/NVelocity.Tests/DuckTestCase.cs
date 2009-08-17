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

namespace NVelocity
{
	using System;
	using System.IO;
	using System.Text;
	using App;
	using NUnit.Framework;

	#region Ducks

	public class Duck1 : IDuck
	{
		public void SetInvoke(string propName, object value)
		{
			throw new NotImplementedException();
		}

		public object GetInvoke(string propName)
		{
			return "get invoked " + propName;
		}

		public object Invoke(string method, params object[] args)
		{
			String arguments = string.Empty;

			foreach(object arg in args)
			{
				arguments += arg + " ";
			}

			return "invoked " + method + " " + arguments;
		}
	}

	public class Duck2 : IDuck
	{
		private StringBuilder sb = new StringBuilder();

		public void SetInvoke(string propName, object value)
		{
			Log("set invoked " + propName + " " + value);
		}

		public object GetInvoke(string propName)
		{
			Log("get invoked " + propName);
			return this;
		}

		public object Invoke(string method, params object[] args)
		{
			Log("invoked " + method + "_" + args.Length);
			return this;
		}

		private void Log(string call)
		{
			sb.Append(call + Environment.NewLine);
		}

		public override string ToString()
		{
			return sb.ToString();
		}
	}

	// I'm not a duck
	public class Test1
	{
		private string name;
		private decimal amount = (decimal) 0.2200;

		public decimal Amount
		{
			get { return amount; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}
	}

	#endregion

	[TestFixture]
	public class DuckTestCase
	{
		private VelocityEngine velocityEngine;
		private Duck1 duck1;
		private Duck2 duck2;

		[SetUp]
		public void Init()
		{
			velocityEngine = new VelocityEngine();
			velocityEngine.Init();

			duck1 = new Duck1();
			duck2 = new Duck2();
		}

		[Test]
		public void PropertyRead()
		{
			Assert.AreEqual("get invoked name", Evaluate("$duck1.name"));
			Assert.AreEqual("get invoked some", Evaluate("$duck1.some"));

			Evaluate("$duck2.style.border");

			StringBuilder expected = new StringBuilder();
			expected.Append("get invoked style" + Environment.NewLine);
			expected.Append("get invoked border" + Environment.NewLine);

			Assert.AreEqual(expected.ToString(), duck2.ToString());
		}

		[Test]
		public void PropertyWrite()
		{
			Evaluate("#set($duck2.Name = 'aaa')");

			StringBuilder expected = new StringBuilder();
			expected.Append("set invoked Name aaa" + Environment.NewLine);

			Assert.AreEqual(expected.ToString(), duck2.ToString());
		}

		[Test]
		public void MethodInvocations()
		{
			Assert.AreEqual("invoked some arg1 arg2 ", Evaluate("$duck1.some('arg1', 'arg2')"));
			Assert.AreEqual("invoked some arg1 ", Evaluate("$duck1.some('arg1')"));
			Assert.AreEqual("invoked some ", Evaluate("$duck1.some()"));
			Evaluate("$duck2.set()");

			StringBuilder expected = new StringBuilder();
			expected.Append("invoked set_0" + Environment.NewLine);

			Assert.AreEqual(expected.ToString(), duck2.ToString());
		}

		[Test]
		public void ArgumentsAreEvaluated()
		{
			Assert.AreEqual("invoked some message 1 message 2 ", Evaluate("$duck1.some($msg1, $msg2)"));
		}

		[Test]
		public void Quoting()
		{
			decimal amount = (decimal) 0.2200;
			Assert.AreEqual("invoked some '" + amount + "' ", Evaluate("$duck1.some($test.Amount.to_squote)"));
			Assert.AreEqual("invoked some \"" + amount + "\" ", Evaluate("$duck1.some($test.Amount.to_quote)"));
			Assert.AreEqual("invoked some \"" + amount + "\" ", Evaluate("$duck1.some($test.Amount.to_quote)"));
			Assert.AreEqual("invoked some \"message 1\" \"message 2\" ", Evaluate("$duck1.some($msg1.to_quote, $msg2.to_quote)"));
			Assert.AreEqual("invoked some 'message 1' 'message 2' ", Evaluate("$duck1.some($msg1.to_squote, $msg2.to_squote)"));
		}

		private string Evaluate(string toEvaluate)
		{
			StringWriter sw = new StringWriter();

			VelocityContext c = new VelocityContext();

			c.Put("duck1", duck1);
			c.Put("duck2", duck2);
			c.Put("test", new Test1());
			c.Put("msg1", "message 1");
			c.Put("msg2", "message 2");

			Assert.IsTrue(velocityEngine.Evaluate(c, sw, "eval1", toEvaluate));

			return sw.GetStringBuilder().ToString();
		}
	}
}