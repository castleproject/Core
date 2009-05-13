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
	using System.Text.RegularExpressions;
	using App;
	using NUnit.Framework;

	/// <summary>
	/// Tests to make sure that the VelocityContext is functioning correctly
	/// </summary>
	[TestFixture]
	public class ForeachTest
	{
		private ArrayList items;
		private VelocityContext c;
		private StringWriter sw;
		private VelocityEngine velocityEngine;
		private Boolean ok;
		private string template;

		[SetUp]
		public void Setup()
		{
			items = new ArrayList();

			items.Add("a");
			items.Add("b");
			items.Add("c");
			items.Add("d");

			c = new VelocityContext();
			c.Put("x", new Something());
			c.Put("items", items);

			sw = new StringWriter();

			velocityEngine = new VelocityEngine();
			velocityEngine.Init();

			template =
				@"
						#foreach( $item in $items )
						#before
							(
						#each
							$item
						#after
							)
						#between  
							,
						#odd  
							+
						#even  
							-
						#nodata
							nothing
						#beforeall
							<
						#afterall
							>
						#end
			";
		}

		[Test]
		public void SimpleForeach()
		{
			ok = velocityEngine.Evaluate(c, sw,
			                             "ContextTest.CaseInsensitive",
			                             @"
					#foreach( $item in $items )
						$item,
					#end
				");

			Assert.IsTrue(ok, "Evaluation returned failure");
			Assert.AreEqual("a,b,c,d,", Normalize(sw));
		}

		[Test]
		public void BeforeAfterForeach()
		{
			ok = velocityEngine.Evaluate(c, sw,
			                             "ContextTest.CaseInsensitive",
			                             @"
						#foreach( $item in $items )
							$item,
						#beforeall
							<
						#afterall
							>
						#end
					");

			Assert.IsTrue(ok, "Evaluation returned failure");
			Assert.AreEqual("<a,b,c,d,>", Normalize(sw));
		}

		[Test]
		public void TemplateForeachAllSections()
		{
			ok = velocityEngine.Evaluate(c, sw,
			                             "ContextTest.CaseInsensitive", template);

			Assert.IsTrue(ok, "Evaluation returned failure");
			Assert.AreEqual("<(+a),(-b),(+c),(-d)>", Normalize(sw));
		}

		[Test]
		public void TemplateForeachNoDataSection()
		{
			items.Clear();
			ok = velocityEngine.Evaluate(c, sw,
			                             "ContextTest.CaseInsensitive", template);

			Assert.IsTrue(ok, "Evaluation returned failure");
			Assert.AreEqual("nothing", Normalize(sw));
		}

		[Test]
		public void TemplateForeachNoDataSection2()
		{
			c.Put("items", null);

			ok = velocityEngine.Evaluate(c, sw,
			                             "ContextTest.CaseInsensitive", template);

			Assert.IsTrue(ok, "Evaluation returned failure");
			Assert.AreEqual("nothing", Normalize(sw));
		}

		[Test]
		public void TemplateForeachTwoItems()
		{
			items.Clear();
			items.Add("a");
			items.Add("b");

			ok = velocityEngine.Evaluate(c, sw,
			                             "ContextTest.CaseInsensitive", template);

			Assert.IsTrue(ok, "Evaluation returned failure");
			Assert.AreEqual("<(+a),(-b)>", Normalize(sw));
		}

		[Test]
		public void TemplateForeachOneItem()
		{
			items.Clear();
			items.Add("a");

			ok = velocityEngine.Evaluate(c, sw,
			                             "ContextTest.CaseInsensitive", template);

			Assert.IsTrue(ok, "Evaluation returned failure");
			Assert.AreEqual("<(+a)>", Normalize(sw));
		}

		[Test]
		public void ParamArraySupportAndForEach2()
		{
			ArrayList items = new ArrayList();

			items.Add("a");
			items.Add("b");
			items.Add("c");

			VelocityContext c = new VelocityContext();
			c.Put("x", new Something());
			c.Put("items", items);

			StringWriter sw = new StringWriter();

			VelocityEngine ve = new VelocityEngine();
			ve.Init();

			Boolean ok = false;

			ok = ve.Evaluate(c, sw,
			                 "ContextTest.CaseInsensitive",
			                 "#foreach( $item in $items )\r\n" +
			                 "#if($item == \"a\")\r\n $x.Contents( \"x\", \"y\" )#end\r\n" +
			                 "#if($item == \"b\")\r\n $x.Contents( \"x\" )#end\r\n" +
			                 "#if($item == \"c\")\r\n $x.Contents( \"c\", \"d\", \"e\" )#end\r\n" +
			                 "#end\r\n");

			Assert.IsTrue(ok, "Evaluation returned failure");
			Assert.AreEqual(" x,y x c,d,e", sw.ToString());
		}

		[Test]
		public void ForEachSimpleCase()
		{
			ArrayList items = new ArrayList();

			items.Add("a");
			items.Add("b");
			items.Add("c");

			VelocityContext c = new VelocityContext();
			c.Put("x", new Something2());
			c.Put("items", items);
			c.Put("d1", new DateTime(2005, 7, 16));
			c.Put("d2", new DateTime(2005, 7, 17));
			c.Put("d3", new DateTime(2005, 7, 18));

			StringWriter sw = new StringWriter();

			VelocityEngine ve = new VelocityEngine();
			ve.Init();

			Boolean ok = false;

			ok = ve.Evaluate(c, sw,
			                 "ContextTest.CaseInsensitive",
			                 "#foreach( $item in $items )\r\n" +
			                 "#if($item == \"a\")\r\n $x.FormatDate( $d1 )#end\r\n" +
			                 "#if($item == \"b\")\r\n $x.FormatDate( $d2 )#end\r\n" +
			                 "#if($item == \"c\")\r\n $x.FormatDate( $d3 )#end\r\n" +
			                 "#end\r\n");

			Assert.IsTrue(ok, "Evaluation returned failure");
			Assert.AreEqual(" 16 17 18", sw.ToString());
		}

		[Test]
		public void Hashtable1()
		{
			VelocityContext c = new VelocityContext();

			Hashtable x = new Hashtable();
			x.Add("item", "value1");

			c.Put("x", x);

			StringWriter sw = new StringWriter();

			VelocityEngine ve = new VelocityEngine();
			ve.Init();

			Boolean ok = false;

			ok = ve.Evaluate(c, sw,
			                 "ContextTest.CaseInsensitive",
			                 "$x.get_Item( \"item\" )");

			Assert.IsTrue(ok, "Evaluation returned failure");
			Assert.AreEqual("value1", sw.ToString());
		}

		private string Normalize(StringWriter sw)
		{
			return Regex.Replace(sw.ToString(), "\\s+", string.Empty);
		}
	}
}