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

namespace Castle.Components.Binder.Tests
{
	using System;
	using System.Globalization;
	using System.Threading;
	
	using NUnit.Framework;

	[TestFixture]
	public class ConverterTestCase
	{
		private bool convSucceed;
		private DefaultConverter converter;

		[TestFixtureSetUp]
		public void Init()
		{
			CultureInfo en = CultureInfo.CreateSpecificCulture( "en" );

			Thread.CurrentThread.CurrentCulture	= en;
			Thread.CurrentThread.CurrentUICulture = en;
			
			converter = new DefaultConverter();
		}

		[Test]
		public void StringConvert()
		{
			Assert.AreEqual("hello", Convert(typeof(string), "hello"));
			Assert.IsTrue(convSucceed);
			
			Assert.AreEqual(null, Convert(typeof(string), null));
			Assert.IsFalse(convSucceed);

			Assert.AreEqual("\n  \t", Convert(typeof(string), " \n  \t "));
			Assert.IsTrue(convSucceed);
			
			Assert.AreEqual(null, Convert(typeof(string), ""));
			Assert.IsFalse(convSucceed);
		}

		[Test]
		public void ArrayConvert()
		{
			Assert.AreEqual(new int[] {1, 2, 3}, Convert(typeof(int[]), "1,2,3"));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(null, Convert(typeof(int[]), null));
			Assert.IsFalse(convSucceed);

			Assert.AreEqual(new int[] {}, Convert(typeof(int[]), ""));
			Assert.IsTrue(convSucceed);
		}

		[Test]
		public void EnumConvert()
		{
			Assert.AreEqual(UriPartial.Scheme, Convert(typeof(UriPartial), UriPartial.Scheme.ToString("D")));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(UriPartial.Authority, Convert(typeof(UriPartial), UriPartial.Authority.ToString("D")));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(UriPartial.Authority, Convert(typeof(UriPartial), "Authority"));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(null, Convert(typeof(UriPartial), null));
			Assert.IsFalse(convSucceed);

			Assert.AreEqual(null, Convert(typeof(UriPartial), "   "));
			Assert.IsFalse(convSucceed);

			try
			{
				Convert(typeof(UriPartial), "Invalid Value");
				Assert.Fail("EnumConvert should had throwed an exception");
			}
			catch(BindingException)
			{
				Assert.IsFalse(convSucceed);
			}
		}

		[Test]
		public void DecimalConvert()
		{
			Assert.AreEqual((decimal) 12.22, Convert(typeof(decimal), "12.22"));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual((decimal) 3000, Convert(typeof(decimal), "3,000.00"));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(null, Convert(typeof(decimal), null));
			Assert.IsFalse(convSucceed);

			Assert.AreEqual(null, Convert(typeof(decimal), "   "));
			Assert.IsFalse(convSucceed);

			try
			{
				Convert(typeof(decimal), "Invalid Value");
				Assert.Fail("DecimalConvert should had throwed an exception");
			}
			catch(BindingException)
			{
				Assert.IsFalse(convSucceed);
			}
		}

		[Test]
		public void GuidConvert()
		{
			Assert.AreEqual(new Guid("6CDEF425-6EEA-42AC-A318-0772B55FF259"), Convert(typeof(Guid), "6CDEF425-6EEA-42AC-A318-0772B55FF259"));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(null, Convert(typeof(Guid), null));
			Assert.IsFalse(convSucceed);

			Assert.AreEqual(null, Convert(typeof(Guid), "   "));
			Assert.IsFalse(convSucceed);

			try
			{
				Convert(typeof(Guid), "Invalid Value");
				Assert.Fail("GuidConvert should had throwed an exception");
			}
			catch(BindingException)
			{
				Assert.IsFalse(convSucceed);
			}
		}

		[Test]
		public void DateTimeConvert()
		{
			Assert.AreEqual(new DateTime(2005, 1, 31), Convert(typeof(DateTime), "2005-01-31"));
			Assert.IsTrue(convSucceed);

			Convert(typeof(DateTime), null);
			Assert.IsFalse(convSucceed);

			Convert(typeof(DateTime), "      ");
			Assert.IsFalse(convSucceed);
		}

		[Test]
		public void InvalidDate1()
		{
			try
			{
				Convert(typeof(DateTime), "Invalid Value");
				Assert.Fail("DateTimeConvert should had throwed an exception");
			}
			catch(BindingException)
			{
				Assert.IsFalse(convSucceed);
			}
		}

		[Test]
		public void InvalidDate2()
		{
			try
			{
				Convert(typeof(DateTime), "2005-02-31");
				Assert.Fail("DateTimeConvert should had throwed an exception");
			}
			catch(BindingException)
			{
				Assert.IsFalse(convSucceed);
			}
		}

		[Test]
		public void Int32Convert()
		{
			Assert.AreEqual(12, Convert(typeof(int), "12"));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(null, Convert(typeof(int), ""));
			Assert.IsFalse(convSucceed);

			Assert.AreEqual(null, Convert(typeof(int), null));
			Assert.IsFalse(convSucceed);
		}

		[Test]
		public void PrimitiveConvert()
		{
			Assert.AreEqual(false, Convert(typeof(bool), ""));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(false, Convert(typeof(bool), "FalSE"));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(true, Convert(typeof(bool), "1"));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(false, Convert(typeof(bool), "0"));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(true, Convert(typeof(bool), "true"));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(true, Convert(typeof(bool), "on"));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(null, Convert(typeof(bool), null));
			Assert.IsFalse(convSucceed);

			Assert.AreEqual(12.01, Convert(typeof(float), "12.01"));
			Assert.IsTrue(convSucceed);

			Assert.AreEqual(null, Convert(typeof(float), ""));
			Assert.IsFalse(convSucceed);

			Assert.AreEqual(null, Convert(typeof(float), null));
			Assert.IsFalse(convSucceed);
		}

		[Test]
		public void TypeConverterConvert()
		{
			Assert.IsTrue(Convert(typeof(CustomType), "validvalue").GetType() == typeof(CustomType));
			Assert.IsTrue(convSucceed);

			try
			{
				Convert(typeof(CustomType), "invalid value");
				Assert.Fail("TypeConverterConvert should had throwed an exception");
			}
			catch(BindingException)
			{
			}

			try
			{
				Convert(typeof(CustomType2), "validvalue");
				Assert.Fail("TypeConverterConvert should had throwed an exception");
			}
			catch(BindingException)
			{
			}
		}

		private object Convert(Type desiredType, string input)
		{
			return converter.Convert(desiredType, typeof(string), input, out convSucceed);
		}
	}
}
