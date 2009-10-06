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
// 
namespace Castle.Components.Binder.Tests
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Threading;
	using Models;
	using NUnit.Framework;

	[TestFixture]
	public class ConverterTestCase
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			CultureInfo en = CultureInfo.CreateSpecificCulture("en");

			Thread.CurrentThread.CurrentCulture = en;
			Thread.CurrentThread.CurrentUICulture = en;
		}

		#endregion

		private bool convSucceed;
		private DefaultConverter converter;

		[TestFixtureSetUp]
		public void Init()
		{
			converter = new DefaultConverter();
		}

		private object Convert(Type desiredType, string input)
		{
			return converter.Convert(desiredType, typeof (string), input, out convSucceed);
		}

		private object ConvertFromArray(Type desiredType, string[] input)
		{
			return converter.Convert(desiredType, typeof (string[]), input, out convSucceed);
		}

		[Test]
		public void ArrayConvert()
		{
			Assert.AreEqual(new[] {1, 2, 3}, Convert(typeof (int[]), "1,2,3"));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(null, Convert(typeof (int[]), null));
			Assert.IsFalse(convSucceed);

			Assert.AreEqual(new int[] {}, Convert(typeof (int[]), ""));
			Assert.IsTrue(convSucceed);
		}

		[Test]
		public void BooleanConvert()
		{
			Assert.AreEqual(false, Convert(typeof (bool), ""));
			Assert.IsFalse(convSucceed);

			Assert.AreEqual(false, Convert(typeof (bool), "FalSE"));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(true, Convert(typeof (bool), "1"));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(false, Convert(typeof (bool), "0"));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(true, Convert(typeof (bool), "true"));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(true, Convert(typeof (bool), "on"));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(null, Convert(typeof (bool), null));
			Assert.IsFalse(convSucceed);
		}

		[Test]
		public void BooleanWithArrayAsSourceConvert()
		{
			Assert.AreEqual(true, ConvertFromArray(typeof (bool), new[] {"1", "0"}));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(false, ConvertFromArray(typeof (bool), new[] {"0"}));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(false, ConvertFromArray(typeof (bool), new[] {"0", "0"}));
			Assert.IsTrue(convSucceed);
		}

		[Test]
		public void DateTimeConvert()
		{
			Assert.AreEqual(new DateTime(2005, 1, 31), Convert(typeof (DateTime), "2005-01-31"));
			Assert.IsTrue(convSucceed);

			Convert(typeof (DateTime), null);
			Assert.IsFalse(convSucceed);

			Convert(typeof (DateTime), "      ");
			Assert.IsFalse(convSucceed);
		}

		[Test]
		public void DecimalConvert()
		{
			Assert.AreEqual((decimal) 12.22, Convert(typeof (decimal), "12.22"));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual((decimal) 3000, Convert(typeof (decimal), "3,000.00"));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(null, Convert(typeof (decimal), null));
			Assert.IsFalse(convSucceed);

			Assert.AreEqual(null, Convert(typeof (decimal), "   "));
			Assert.IsTrue(convSucceed);

			try
			{
				Convert(typeof (decimal), "Invalid Value");
				Assert.Fail("DecimalConvert should had throwed an exception");
			}
			catch (BindingException)
			{
				Assert.IsFalse(convSucceed);
			}
		}

		[Test]
		public void EnumConvert()
		{
			Assert.AreEqual(UriPartial.Scheme, Convert(typeof (UriPartial), UriPartial.Scheme.ToString("D")));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(UriPartial.Authority, Convert(typeof (UriPartial), UriPartial.Authority.ToString("D")));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(UriPartial.Authority, Convert(typeof (UriPartial), "Authority"));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(null, Convert(typeof (UriPartial), null));
			Assert.IsFalse(convSucceed);

			Assert.AreEqual(null, Convert(typeof (UriPartial), "   "));
			Assert.IsFalse(convSucceed);

			try
			{
				Convert(typeof (UriPartial), "Invalid Value");
				Assert.Fail("EnumConvert should had throwed an exception");
			}
			catch (BindingException)
			{
				Assert.IsFalse(convSucceed);
			}
		}

		[Test]
		public void GuidConvert()
		{
			Assert.AreEqual(new Guid("6CDEF425-6EEA-42AC-A318-0772B55FF259"),
			                Convert(typeof (Guid), "6CDEF425-6EEA-42AC-A318-0772B55FF259"));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(null, Convert(typeof (Guid), null));
			Assert.IsFalse(convSucceed);

			Assert.AreEqual(null, Convert(typeof (Guid), "   "));
			Assert.IsTrue(convSucceed);

			try
			{
				Convert(typeof (Guid), "Invalid Value");
				Assert.Fail("GuidConvert should had throwed an exception");
			}
			catch (BindingException)
			{
				Assert.IsFalse(convSucceed);
			}
		}

		[Test]
		public void InstanceOfConvert()
		{
			var col = new ArrayList();
			Assert.AreEqual(col, converter.Convert(typeof (ICollection), col, out convSucceed));
			Assert.IsTrue(convSucceed);
		}

		[Test]
		public void Int32Convert()
		{
			Assert.AreEqual(12, Convert(typeof (int), "12"));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(null, Convert(typeof (int), ""));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(null, Convert(typeof (int), null));
			Assert.IsFalse(convSucceed);
		}

		[Test]
		public void InvalidDate1()
		{
			try
			{
				Convert(typeof (DateTime), "Invalid Value");
				Assert.Fail("DateTimeConvert should had throwed an exception");
			}
			catch (BindingException)
			{
				Assert.IsFalse(convSucceed);
			}
		}

		[Test]
		public void InvalidDate2()
		{
			try
			{
				Convert(typeof (DateTime), "2005-02-31");
				Assert.Fail("DateTimeConvert should had throwed an exception");
			}
			catch (BindingException)
			{
				Assert.IsFalse(convSucceed);
			}
		}

		[Test]
		public void ListOfIntsConvert()
		{
			Type desiredType = typeof (List<int>);

			List<int> result;

			result = Convert(desiredType, "1,2,3") as List<int>;

			Assert.IsNotNull(result);
			Assert.AreEqual(3, result.Count);
			Assert.AreEqual(1, result[0]);
			Assert.AreEqual(2, result[1]);
			Assert.AreEqual(3, result[2]);
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(null, Convert(desiredType, null));
			Assert.IsFalse(convSucceed);

			result = Convert(desiredType, "") as List<int>;
			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count);
			Assert.IsTrue(convSucceed);
		}

		[Test]
		public void ListOfStringsConvert()
		{
			Type desiredType = typeof (List<string>);

			List<string> result;

			result = Convert(desiredType, "1,2,3") as List<string>;

			Assert.IsNotNull(result);
			Assert.AreEqual(3, result.Count);
			Assert.AreEqual("1", result[0]);
			Assert.AreEqual("2", result[1]);
			Assert.AreEqual("3", result[2]);
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(null, Convert(desiredType, null));
			Assert.IsFalse(convSucceed);

			result = Convert(desiredType, "") as List<string>;
			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count);
			Assert.IsTrue(convSucceed);
		}

		[Test]
		public void NullableBooleanConversion()
		{
			Assert.AreEqual(new bool?(), Convert(typeof (bool?), ""));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(new bool?(), Convert(typeof (bool?), null));
			Assert.IsFalse(convSucceed);

			Assert.AreEqual(true, Convert(typeof (bool?), "1"));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(false, Convert(typeof (bool?), "0"));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(false, Convert(typeof (bool?), "0"));
			Assert.IsTrue(convSucceed);
		}

		[Test]
		public void NullableBooleanWithArrayAsSourceConvert()
		{
			Assert.AreEqual(true, ConvertFromArray(typeof (bool?), new[] {"1", "0"}));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(false, ConvertFromArray(typeof (bool?), new[] {"0"}));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(false, ConvertFromArray(typeof (bool?), new[] {"0", "0"}));
			Assert.IsTrue(convSucceed);
		}

		[Test]
		public void NullableDateTimeConversion()
		{
			Assert.AreEqual(new DateTime(2005, 1, 31), Convert(typeof (DateTime?), "2005-01-31"));
			Assert.IsTrue(convSucceed);

			Convert(typeof (DateTime?), null);
			Assert.IsFalse(convSucceed);

			Convert(typeof (DateTime?), "      ");
			Assert.IsTrue(convSucceed);
		}

		[Test]
		public void NullableDecimalConversion()
		{
			var val = (decimal?) Convert(typeof (decimal?), "12.22");
			Assert.AreEqual((decimal?) 12.22, val);
			Assert.IsTrue(val.HasValue);
			Assert.IsTrue(convSucceed);

			val = (decimal?) Convert(typeof (decimal?), "");
			Assert.IsFalse(val.HasValue);
			Assert.IsTrue(convSucceed);

			val = (decimal?) Convert(typeof (decimal?), "3,000.00");
			Assert.AreEqual((decimal?) 3000, val);
			Assert.IsTrue(val.HasValue);
			Assert.IsTrue(convSucceed);

			val = (decimal?) Convert(typeof (decimal?), null);
			Assert.IsFalse(val.HasValue);
			Assert.IsFalse(convSucceed);

			val = (decimal?) Convert(typeof (decimal?), "   ");
			Assert.IsFalse(val.HasValue);
			Assert.IsTrue(convSucceed);

			try
			{
				Convert(typeof (decimal?), "Invalid Value");
				Assert.Fail("DecimalConvert should had throwed an exception");
			}
			catch (BindingException)
			{
				Assert.IsFalse(convSucceed);
			}
		}

		[Test]
		public void NullableEnumConversion()
		{
			var val = (UriPartial?) Convert(typeof (UriPartial?), "Path");
			Assert.AreEqual(UriPartial.Path, val);
			Assert.IsTrue(val.HasValue);
			Assert.IsTrue(convSucceed);

			val = (UriPartial?) Convert(typeof (UriPartial?), "");
			Assert.IsFalse(val.HasValue);
			Assert.IsTrue(convSucceed);

			val = (UriPartial?) Convert(typeof (UriPartial?), "  ");
			Assert.IsFalse(val.HasValue);
			Assert.IsTrue(convSucceed);

			val = (UriPartial?) Convert(typeof (UriPartial?), null);
			Assert.IsFalse(val.HasValue);
			Assert.IsFalse(convSucceed);
		}

		[Test]
		public void NullableInt32Conversion()
		{
			var val = (int?) Convert(typeof (int?), "");
			Assert.IsFalse(val.HasValue);
			Assert.IsTrue(convSucceed);

			val = (int?) Convert(typeof (int?), "  ");
			Assert.IsFalse(val.HasValue);
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(null, Convert(typeof (int?), null));
			Assert.IsFalse(convSucceed);
		}

		[Test]
		public void PrimitiveConvert()
		{
			Assert.AreEqual(12.01f, Convert(typeof (float), "12.01"));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(null, Convert(typeof (float), ""));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(null, Convert(typeof (float), null));
			Assert.IsFalse(convSucceed);
		}

		[Test]
		public void StringConvert()
		{
			Assert.AreEqual("hello", Convert(typeof (string), "hello"));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(null, Convert(typeof (string), null));
			Assert.IsFalse(convSucceed);

			Assert.AreEqual("\n  \t", Convert(typeof (string), " \n  \t "));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(null, Convert(typeof (string), ""));
			Assert.IsTrue(convSucceed);
		}

		[Test]
		public void TypeConverterConvert()
		{
			Assert.IsTrue(Convert(typeof (CustomType), "validvalue").GetType() == typeof (CustomType));
			Assert.IsTrue(convSucceed);

			try
			{
				Convert(typeof (CustomType), "invalid value");
				Assert.Fail("TypeConverterConvert should had throwed an exception");
			}
			catch (BindingException)
			{
			}

			try
			{
				Convert(typeof (CustomType2), "validvalue");
				Assert.Fail("TypeConverterConvert should had throwed an exception");
			}
			catch (BindingException)
			{
			}
		}
	}
}