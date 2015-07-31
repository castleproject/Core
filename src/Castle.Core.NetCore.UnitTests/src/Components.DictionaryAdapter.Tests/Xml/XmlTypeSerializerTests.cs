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

#if !SILVERLIGHT && !MONO && !NETCORE // Until support for other platforms is verified
namespace CastleTests.Components.DictionaryAdapter.Xml.Tests
{
	using System;
	using Castle.Components.DictionaryAdapter.Xml;
	using Xunit;

		public class XmlTypeSerializerTests
	{
		[Fact]
		public void String_Roundtrip()
		{
			TestSimpleSerializer("Some String", "Some String");
		}

		[Fact]
		public void Boolean_Roundtrip()
		{
			TestSimpleSerializer(true, "true");
		}

		[Fact]
		public void Char_Roundtrip()
		{
			TestSimpleSerializer('C', "C");
		}

		[Fact]
		public void SByte_Roundtrip()
		{
			TestSimpleSerializer((sbyte) 42, "42");
		}

		[Fact]
		public void Int16_Roundtrip()
		{
			TestSimpleSerializer((short) 42, "42");
		}

		[Fact]
		public void Int32_Roundtrip()
		{
			TestSimpleSerializer(42, "42");
		}

		[Fact]
		public void Int64_Roundtrip()
		{
			TestSimpleSerializer(42L, "42");
		}

		[Fact]
		public void Byte_Roundtrip()
		{
			TestSimpleSerializer((byte) 42, "42");
		}

		[Fact]
		public void UInt16_Roundtrip()
		{
			TestSimpleSerializer((ushort) 42, "42");
		}

		[Fact]
		public void UInt32_Roundtrip()
		{
			TestSimpleSerializer(42U, "42");
		}

		[Fact]
		public void UInt64_Roundtrip()
		{
			TestSimpleSerializer(42UL, "42");
		}

		[Fact]
		public void Single_Roundtrip()
		{
			TestSimpleSerializer(3.1337E+16F, "3.1337E+16");
		}

		[Fact]
		public void Double_Roundtrip()
		{
			TestSimpleSerializer(3.1337E+16D, "3.1337E+16");
		}

		[Fact]
		public void Decimal_Roundtrip()
		{
			TestSimpleSerializer(3.1337M, "3.1337");
		}

		[Fact]
		public void TimeSpan_Roundtrip()
		{
			TestSimpleSerializer(
				new TimeSpan(1, 2, 3, 4, 567),
				"P1DT2H3M4.567S");
		}

		[Fact]
		public void DateTime_Roundtrip()
		{
			TestSimpleSerializer(
				new DateTime(2011, 9, 5, 15, 14, 31, 123, DateTimeKind.Utc),
				"2011-09-05T15:14:31.123Z");
		}

		[Fact]
		public void DateTimeOffset_Roundtrip()
		{
			TestSimpleSerializer(
				new DateTimeOffset(2011, 9, 5, 15, 14, 31, 123, new TimeSpan(-5, 0, 0)),
				"2011-09-05T15:14:31.123-05:00");
		}

		[Fact]
		public void Guid_Roundtrip()
		{
			var text = "04eaaed4-e7e8-433a-93d6-c6ddae957fb5";
			TestSimpleSerializer(new Guid(text), text);
		}

		[Fact]
		public void ByteArray_RoundTrip()
		{
			TestSimpleSerializer(new byte[] { 1, 2, 3 }, "AQID");
		}

		[Fact]
		public void Uri_Roundtrip()
		{
			var text = "http://example/foo/bar";
			TestSimpleSerializer(new Uri(text), text);
		}

		[Fact]
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
			Assert.That(node.Value, Is.EqualTo(text));

			var actual = serializer.GetValue(node, null, null);
			Assert.That(actual, Is.EqualTo(value));
		}
	}
}
#endif
