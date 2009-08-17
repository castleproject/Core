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
	using System.IO;
	using System.Text;
	using App;
	using NUnit.Framework;

	[TestFixture]
	public class StringInterpolationTestCase
	{
		[Test]
		public void SingleParamDict()
		{
			Assert.AreEqual("1:key1=<value1>", Eval("%{      key1     =  'value1' }"));
			Assert.AreEqual("1:key1=<value1>", Eval("%{      key1='value1' }"));
			Assert.AreEqual("1:key1=<value1>", Eval("%{key1='value1'}"));
		}

		[Test]
		public void MultiParamDict()
		{
			Assert.AreEqual("2:key1=<value1> key2=<10>", Eval("%{key1='value1', key2=10}"));
			Assert.AreEqual("2:key1=<value1> key2=<10>", Eval("%{key1='value1' ,  key2=10}"));
			Assert.AreEqual("2:key1=<value1> key2=<10>", Eval("%{key1='value1'   key2=10}"));
		}

		[Test]
		public void MultiParamDictUsingInterpolation()
		{
			Assert.AreEqual("2:key1=<> key2=<1:id=<123>>", Eval("%{key1=${siteRoot}, key2=$params}"));
			Assert.AreEqual("3:key1=<value1> key2=<value2> key3=<value3>",
			                Eval("%{ key1='value${survey}', key2='value$id', key3='value3' }"));

			Assert.AreEqual("2:key1=<value1> key2=<value2>",
			                Eval("%{ key1='value${survey}', key2='value$id' }"));

			Assert.AreEqual("2:key1=<> key2=<2>", Eval("%{key1=${siteRoot}, key2=$id}"));
		}

		[Test]
		public void NestedDicts()
		{
			Assert.AreEqual("3:action=<index> controller=<area> params=<0>",
			                Eval("%{controller='area', action='index', params={}}"));

			Assert.AreEqual("3:action=<index> controller=<area> params=<2:id=<1> lastpage=<2>>",
			                Eval("%{controller='area', action='index', params={id=1, lastpage=$id} }"));

			Assert.AreEqual("3:action=<index> controller=<area> params=<0>",
			                Eval("%{params={}, action='index', controller='area'}"));

			Assert.AreEqual("3:action=<1> controller=<area> params=<0>",
			                Eval("%{params={}, action=$survey, controller='area'}"));

			Assert.AreEqual("3:action=<index> controller=<area> params=<2:id=<'1'> lastpage=<2>>",
			                Eval("%{params={id=$survey.to_squote, lastpage=$id}, controller='area', action='index'}"));

			Assert.AreEqual("1:url=<3:action=<viewpage> pathinfo=<> querystring=<1:id=<1>>>",
			                Eval("%{url={action='viewpage',pathinfo=$context.info,querystring={id=1}}}"));
		}

		[Test]
		public void EscapeChars()
		{
			Assert.AreEqual("1:action=<'abc'>", Eval(@"%{action='\'abc\''}"));
		}

		[Test]
		public void ZeroParamDictInterpolation()
		{
			Assert.AreEqual("0", Eval("%{       }"));
			Assert.AreEqual("0", Eval("%{}"));
		}

		[Test]
		public void InnerStringBug()
		{
			Assert.AreEqual("1:class=<loader {department: {url: 'something' } }>",
				Eval("%{class='loader {department: {url: \\'something\\' } }'}"));
		}

		public string Eval(string text)
		{
			return Eval(text, true);
		}

		public string Eval(string text, bool wrapBetweenQuote)
		{
			VelocityContext c = new VelocityContext();
			Hashtable hash2 = new Hashtable();
			hash2["id"] = "123";
			c.Put("params", hash2);
			c.Put("style", "style='color:red'");
			c.Put("survey", 1);
			c.Put("id", 2);
			c.Put("siteRoot", String.Empty);
			c.Put("Helper", new Helper());
			c.Put("DictHelper", new DictHelper());

			StringWriter sw = new StringWriter();

			VelocityEngine velocityEngine = new VelocityEngine();
			velocityEngine.Init();

			string templatePrefix = "$Helper.Dump(";
			string templateSuffix = ")";
			string templateContent = (wrapBetweenQuote) ? '"' + text + '"' : text;

			string template = templatePrefix + templateContent + templateSuffix;

			bool ok = velocityEngine.Evaluate(c, sw, "ContextTest.CaseInsensitive", template);

			Assert.IsTrue(ok, "Evaluation returned failure");

			string result = sw.ToString();

			return result.StartsWith(templatePrefix) ? text : result;
		}
	}

	public class Helper
	{
		public String Dump(IDictionary options)
		{
			if (options == null) throw new ArgumentNullException("options");

			StringBuilder stringBuilder = new StringBuilder();

			Array keysSorted = (new ArrayList(options.Keys)).ToArray(typeof(string)) as string[];

			Array.Sort(keysSorted);

			stringBuilder.Append(options.Count).Append(':');

			foreach(string key in keysSorted)
			{
				object val = options[key];

				IDictionary dictionary = val as IDictionary;

				if (dictionary != null)
				{
					stringBuilder.Append(key).Append("=<").Append(Dump(dictionary)).Append("> ");
				}
				else
				{
					stringBuilder.Append(key).Append("=<").Append(val).Append("> ");
				}
			}

			if (stringBuilder.Length > 0) stringBuilder.Length--;

			return stringBuilder.ToString();
		}
	}
}