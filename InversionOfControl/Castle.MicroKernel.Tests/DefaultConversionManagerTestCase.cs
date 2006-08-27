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

namespace Castle.MicroKernel.Tests
{
	using System;
	using System.Collections;

	using NUnit.Framework;

	using Castle.Core.Configuration;

	using Castle.MicroKernel.SubSystems.Conversion;


	[TestFixture]
	public class DefaultConversionManagerTestCase
	{
		DefaultConversionManager conversionMng = new DefaultConversionManager();

		[Test]
		public void PerformConversionInt()
		{
			Assert.AreEqual(100, conversionMng.PerformConversion("100", typeof(int)));
			Assert.AreEqual(1234, conversionMng.PerformConversion("1234", typeof(int)));
		}

		[Test]
		public void PerformConversionChar()
		{
			Assert.AreEqual('a', conversionMng.PerformConversion("a", typeof(Char)));
		}

		[Test]
		public void PerformConversionBool()
		{
			Assert.AreEqual(true, conversionMng.PerformConversion("true", typeof(bool)));
			Assert.AreEqual(false, conversionMng.PerformConversion("false", typeof(bool)));
		}
		
		[Test]
		public void PerformConversionType()
		{
			Assert.AreEqual(typeof(DefaultConversionManagerTestCase), 
				conversionMng.PerformConversion(
					"Castle.MicroKernel.Tests.DefaultConversionManagerTestCase, Castle.MicroKernel.Tests", 
					typeof(Type)));
		}

		[Test]
		public void PerformConversionList()
		{
			MutableConfiguration config = new MutableConfiguration("list");
			config.Attributes["type"] = "System.String";
			
			config.Children.Add( new MutableConfiguration("item", "first") );
			config.Children.Add( new MutableConfiguration("item", "second") );
			config.Children.Add( new MutableConfiguration("item", "third") );

			Assert.IsTrue(conversionMng.CanHandleType(typeof(IList)));
			Assert.IsTrue(conversionMng.CanHandleType(typeof(ArrayList)));

			IList list = (IList) conversionMng.PerformConversion(config, typeof(IList));
			Assert.IsNotNull(list);
			Assert.AreEqual("first", list[0]);
			Assert.AreEqual("second", list[1]);
			Assert.AreEqual("third", list[2]);
		}

		[Test]
		public void Dictionary()
		{
			MutableConfiguration config = new MutableConfiguration("dictionary");
			config.Attributes["keyType"] = "System.String";
			config.Attributes["valueType"] = "System.String";
			
			config.Children.Add( new MutableConfiguration("item", "first") ).Attributes["key"] = "key1";
			config.Children.Add( new MutableConfiguration("item", "second") ).Attributes["key"] = "key2";
			config.Children.Add( new MutableConfiguration("item", "third") ).Attributes["key"] = "key3";

			MutableConfiguration intItem = new MutableConfiguration("item", "40");
			intItem.Attributes["key"] = "4";
			intItem.Attributes["keyType"] = "System.Int32, mscorlib";
			intItem.Attributes["valueType"] = "System.Int32, mscorlib";

			config.Children.Add(intItem);

			MutableConfiguration dateItem = new MutableConfiguration("item", "2005/12/1");
			dateItem.Attributes["key"] = "2000/1/1";
			dateItem.Attributes["keyType"] = "System.DateTime, mscorlib";
			dateItem.Attributes["valueType"] = "System.DateTime, mscorlib";

			config.Children.Add(dateItem);

			Assert.IsTrue(conversionMng.CanHandleType(typeof(IDictionary)));
			Assert.IsTrue(conversionMng.CanHandleType(typeof(Hashtable)));

			IDictionary dict = (IDictionary) 
				conversionMng.PerformConversion(config, typeof(IDictionary));
			
			Assert.IsNotNull(dict);
			
			Assert.AreEqual("first", dict["key1"]);
			Assert.AreEqual("second", dict["key2"]);
			Assert.AreEqual("third", dict["key3"]);
			Assert.AreEqual(40, dict[4]);
			Assert.AreEqual(new DateTime(2005,12,1), dict[new DateTime(2000,1,1)]);
		}

		[Test]
		public void Array()
		{
			MutableConfiguration config = new MutableConfiguration("array");
			
			config.Children.Add( new MutableConfiguration("item", "first") );
			config.Children.Add( new MutableConfiguration("item", "second") );
			config.Children.Add( new MutableConfiguration("item", "third") );

			Assert.IsTrue(conversionMng.CanHandleType(typeof(String[])));

			String[] array = (String[]) 
				conversionMng.PerformConversion(config, typeof(String[]));
			
			Assert.IsNotNull(array);
			
			Assert.AreEqual("first", array[0]);
			Assert.AreEqual("second", array[1]);
			Assert.AreEqual("third", array[2]);
		}

		[Test]
		public void PerformConversionTimeSpan()
		{
			Assert.AreEqual(TimeSpan.Zero, conversionMng.PerformConversion("0", typeof(TimeSpan)));
			Assert.AreEqual(TimeSpan.FromDays(14), conversionMng.PerformConversion("14", typeof(TimeSpan)));
			Assert.AreEqual(new TimeSpan(0, 1, 2, 3), conversionMng.PerformConversion("1:2:3", typeof(TimeSpan)));
			Assert.AreEqual(new TimeSpan(0, 0, 0, 0, 250), conversionMng.PerformConversion("0:0:0.250", typeof(TimeSpan)));
			Assert.AreEqual(new TimeSpan(10, 20, 30, 40, 500), conversionMng.PerformConversion("10.20:30:40.50", typeof(TimeSpan)));
		}
	}
}
