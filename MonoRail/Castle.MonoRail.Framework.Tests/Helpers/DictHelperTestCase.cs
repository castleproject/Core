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

namespace Castle.MonoRail.Framework.Tests.Helpers
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Threading;

	using Castle.MonoRail.Framework.Helpers;
	
	using NUnit.Framework;


	[TestFixture]
	public class DictHelperTestCase
	{
		private DictHelper helper;

		[SetUp]
		public void Init()
		{
			helper = new DictHelper();
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		}

		[Test]
		public void EmptyDict()
		{
			IDictionary dict = helper.CreateDict();

			Assert.IsNotNull(dict);
			Assert.AreEqual(0, dict.Count);
		}

		[Test]
		public void SimpleDict()
		{
			IDictionary dict = helper.CreateDict("name=value", "other=somethingelse");

			Assert.IsNotNull(dict);
			Assert.AreEqual(2, dict.Count);

			foreach (String key in dict.Keys)
			{
				if (key.Equals("name"))
				{
					Assert.AreEqual("value", dict["name"]);
				}
				else if (key.Equals("other"))
				{
					Assert.AreEqual("somethingelse", dict["other"]);
				}
				else
				{
					Assert.Fail("unexpected key? " + key);
				}
			}
		}

		/// <summary>
		/// Pulled out of [Test] method, so it can be call in timeing time 
		/// w/o the overhead of the Asserts
		/// </summary>
		/// <returns></returns>
		public IDictionary ComplexDict_woChk()
		{
			int h = 1;
			double i = 3.1415;
			int j = 5;
			IDictionary dict = helper.CreateDict("name=value", "other=somethingelse", "A=B",
			                                     "CCC=DDD", "EEE=FFF=GGG",
			                                     "hhh=" + h.ToString(),
			                                     "iii=" + i.ToString(),
			                                     "jjj=" + j.ToString()
				);
			return dict;
		}

		[Test]
		public void ComplexDict()
		{
			IDictionary dict = ComplexDict_woChk();
			ComplexAsserts(dict);
		}

		/// <summary>
		/// Pulled out of [Test] method, so it can be call in timing test 
		/// w/o the overhead of the Asserts
		/// </summary>
		/// <returns></returns>
		public IDictionary ComplexDict2_woChk()
		{
			int h = 1;
			double i = 3.1415;
			int j = 5;
			IDictionary dict = helper.N("name", "value")
				.N("other", "somethingelse")
				.N("A", "B")
				.N("CCC", "DDD")
				.N("EEE", "FFF=GGG")
				.N("hhh", h)
				.N("iii", i)
				.N("jjj", j);

			return dict;
		}

		[Test]
		public void ComplexDict2()
		{
			IDictionary dict = ComplexDict2_woChk();
			ComplexAsserts(dict);
		}

		public IDictionary ComplexDictStatic_woChk()
		{
			int h = 1;
			double i = 3.1415;
			int j = 5;
			return DictHelper.CreateN("name", "value")
				.N("other", "somethingelse")
				.N("A", "B")
				.N("CCC", "DDD")
				.N("EEE", "FFF=GGG")
				.N("hhh", h)
				.N("iii", i)
				.N("jjj", j);
		}

		[Test]
		public void ComplexDictStatic()
		{
			IDictionary dict = ComplexDictStatic_woChk();
			ComplexAsserts(dict);
		}

		public IDictionary ComplexDictMixed_woChk()
		{
			int h = 1;
			double i = 3.1415;
			int j = 5;
			return helper.CreateDict("name=value", "other=somethingelse", "A=B")
				.N("CCC", "DDD").N("EEE", "FFF=GGG").N("hhh", h).N("iii", i).N("jjj", j);
		}

		[Test]
		public void ComplexDictMixed()
		{
			IDictionary dict = ComplexDictMixed_woChk();
			ComplexAsserts(dict);
		}

		private void ComplexAsserts(IDictionary dict)
		{
			Assert.IsNotNull(dict);
			Assert.AreEqual(8, dict.Count);

			ArrayList keys = new ArrayList(dict.Keys);

			Assert.Contains("name", keys);
			Assert.Contains("other", keys);
			Assert.Contains("A", keys);
			Assert.Contains("CCC", keys);
			Assert.Contains("EEE", keys);
			Assert.Contains("hhh", keys);
			Assert.Contains("iii", keys);
			Assert.Contains("jjj", keys);

			Assert.AreEqual("value", dict["name"]);
			Assert.AreEqual("somethingelse", dict["other"]);
			Assert.AreEqual("B", dict["A"]);
			Assert.AreEqual("DDD", dict["CCC"]);
			Assert.AreEqual("FFF=GGG", dict["EEE"]);
			Assert.AreEqual("1", dict["hhh"]);
			Assert.AreEqual("3.1415", dict["iii"]);
			Assert.AreEqual("5", dict["jjj"]);
		}

		[Test]
		public void NameValueDict()
		{
			NameValueCollection nvc = new NameValueCollection(8);
			nvc.Add("name", "value");
			nvc.Add("name", "value2");
			nvc.Add("other", "somethingelse");
			nvc.Add("A", "B");
			nvc.Add("CCC", "DDD");
			nvc.Add("EEE", "FFF=GGG");
			nvc.Add("hhh", "1");
			nvc.Add("iii", "3.1415");
			nvc.Add("jjj", "5");
			IDictionary dict = helper.FromNameValueCollection(nvc);

			Assert.IsNotNull(dict);
			Assert.AreEqual(8, dict.Count);

			ArrayList keys = new ArrayList(dict.Keys);

			Assert.Contains("name", keys);
			Assert.Contains("other", keys);
			Assert.Contains("A", keys);
			Assert.Contains("CCC", keys);
			Assert.Contains("EEE", keys);
			Assert.Contains("hhh", keys);
			Assert.Contains("iii", keys);
			Assert.Contains("jjj", keys);

			Assert.AreEqual("value", ((string[]) dict["name"])[0]);
			Assert.AreEqual("value2", ((string[]) dict["name"])[1]);
			Assert.AreEqual("somethingelse", ((string[]) dict["other"])[0]);
			Assert.AreEqual("B", ((string[]) dict["A"])[0]);
			Assert.AreEqual("DDD", ((string[]) dict["CCC"])[0]);
			Assert.AreEqual("FFF=GGG", ((string[]) dict["EEE"])[0]);
			Assert.AreEqual("1", ((string[]) dict["hhh"])[0]);
			Assert.AreEqual("3.1415", ((string[]) dict["iii"])[0]);
			Assert.AreEqual("5", ((string[]) dict["jjj"])[0]);
		}


		[Test]
		public void PushingParser()
		{
			IDictionary dict = helper.CreateDict("name=value=aa");

			Assert.IsNotNull(dict);
			Assert.AreEqual(1, dict.Count);

			foreach (String key in dict.Keys)
			{
				if (key.Equals("name"))
				{
					Assert.AreEqual("value=aa", dict["name"]);
				}
				else
				{
					Assert.Fail("unexpected key? " + key);
				}
			}
		}

		#region Testing test (set to Ignore presently)

		private const int NumReps = 200000;

		[Test, Ignore]
		public void RepeatTest()
		{
			for (int i = 0; i < NumReps; i++)
			{
				ComplexDict_woChk();
			}
		}

		[Test, Ignore]
		public void RepeatTest2()
		{
			for (int i = 0; i < NumReps; i++)
			{
				ComplexDict2_woChk();
			}
		}

		[Test, Ignore]
		public void RepeatTest3()
		{
			for (int i = 0; i < NumReps; i++)
			{
				ComplexDictStatic();
			}
		}

		#endregion
	}
}