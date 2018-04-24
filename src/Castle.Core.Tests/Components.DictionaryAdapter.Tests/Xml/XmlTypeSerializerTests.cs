// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.f
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Components.DictionaryAdapter.Xml.Tests
{
	using System;

	using NUnit.Framework;

	[TestFixture]
	public class XmlTypeSerializerTests
	{
		[Test]
		public void String_Roundtrip()
		{
			TestSimpleSerializer("Some String", "Some String");
		}

		[Test]
		public void Boolean_Roundtrip()
		{
			TestSimpleSerializer(true, "true");
		}

		[Test]
		public void Char_Roundtrip()
		{
			TestSimpleSerializer('C', "C");
		}

		[Test]
		public void SByte_Roundtrip()
		{
			TestSimpleSerializer((sbyte) 42, "42");
		}

		[Test]
		public void Int16_Roundtrip()
		{
			TestSimpleSerializer((short) 42, "42");
		}

		[Test]
		public void Int32_Roundtrip()
		{
			TestSimpleSerializer(42, "42");
		}

		[Test]
		public void Int64_Roundtrip()
		{
			TestSimpleSerializer(42L, "42");
		}

		[Test]
		public void Byte_Roundtrip()
		{
			TestSimpleSerializer((byte) 42, "42");
		}

		[Test]
		public void UInt16_Roundtrip()
		{
			TestSimpleSerializer((ushort) 42, "42");
		}

		[Test]
		public void UInt32_Roundtrip()
		{
			TestSimpleSerializer(42U, "42");
		}

		[Test]
		public void UInt64_Roundtrip()
		{
			TestSimpleSerializer(42UL, "42");
		}

		[Test]
		public void Single_Roundtrip()
		{
			TestSimpleSerializer(3.1337E+16F, "3.1337E+16");
		}

		[Test]
		public void Double_Roundtrip()
		{
			TestSimpleSerializer(3.1337E+16D, "3.1337E+16");
		}

		[Test]
		public void Decimal_Roundtrip()
		{
			TestSimpleSerializer(3.1337M, "3.1337");
		}

		[Test]
		public void TimeSpan_Roundtrip()
		{
			TestSimpleSerializer(
				new TimeSpan(1, 2, 3, 4, 567),
				"P1DT2H3M4.567S");
		}

		[Test]
		public void DateTime_Roundtrip()
		{
			TestSimpleSerializer(
				new DateTime(2011, 9, 5, 15, 14, 31, 123, DateTimeKind.Utc),
				"2011-09-05T15:14:31.123Z");
		}

		[Test]
		public void DateTimeOffset_Roundtrip()
		{
			TestSimpleSerializer(
				new DateTimeOffset(2011, 9, 5, 15, 14, 31, 123, new TimeSpan(-5, 0, 0)),
				"2011-09-05T15:14:31.123-05:00");
		}

		[Test]
		public void Guid_Roundtrip()
		{
			var text = "04eaaed4-e7e8-433a-93d6-c6ddae957fb5";
			TestSimpleSerializer(new Guid(text), text);
		}

		[Test]
		public void ByteArray_RoundTrip()
		{
			TestSimpleSerializer(new byte[] { 1, 2, 3 }, "AQID");
		}

		[Test]
		public void Uri_Roundtrip()
		{
			var text = "http://example/foo/bar";
			TestSimpleSerializer(new Uri(text), text);
		}

		[Test]
		public void Dynamic_Roundtrip()
		{
			TestSimpleSerializer(42, "42", typeof(object));
		}

		private void TestSimpleSerializer(object value, string text)
		{
			TestSimpleSerializer(value, text, value.GetType());
		}

		private void TestSimpleSerializer(object value, string text, Type serializerType)
		{
			var serializer = XmlTypeSerializerCache.Instance[serializerType];
			var node = new DummyXmlNode(value.GetType());

			serializer.SetValue(node, null, null, null, ref value);
			Assert.AreEqual(text, node.Value);

			var actual = serializer.GetValue(node, null, null);
			Assert.AreEqual(value, actual);
		}
	}
}
