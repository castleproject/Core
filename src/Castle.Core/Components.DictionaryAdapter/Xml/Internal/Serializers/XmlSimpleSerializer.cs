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

#if FEATURE_DICTIONARYADAPTER_XML
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Xml;

	public class XmlSimpleSerializer<T> : XmlTypeSerializer
	{
		private readonly Func<T, string> getString;
		private readonly Func<string, T> getObject;

		public XmlSimpleSerializer(
			Func<T, string> getString,
			Func<string, T> getObject)
		{
			this.getString = getString;
			this.getObject = getObject;
		}

		public override XmlTypeKind Kind
		{
			get { return XmlTypeKind.Simple; }
		}

		public override object GetValue(IXmlNode node, IDictionaryAdapter parent, IXmlAccessor accessor)
		{
			return getObject(node.Value);
		}

		public override void SetValue(IXmlNode node, IDictionaryAdapter parent, IXmlAccessor accessor, object oldValue, ref object value)
		{
			node.Value = getString((T) value);
		}
	}

	public static class XmlSimpleSerializer
	{
		public static readonly XmlTypeSerializer
			ForBoolean        = new XmlSimpleSerializer<Boolean>        (XmlConvert.ToString, XmlConvert.ToBoolean),
			ForChar           = new XmlSimpleSerializer<Char>           (XmlConvert.ToString, XmlConvert.ToChar),
			ForSByte          = new XmlSimpleSerializer<SByte>          (XmlConvert.ToString, XmlConvert.ToSByte),
			ForInt16          = new XmlSimpleSerializer<Int16>          (XmlConvert.ToString, XmlConvert.ToInt16),
			ForInt32          = new XmlSimpleSerializer<Int32>          (XmlConvert.ToString, XmlConvert.ToInt32),
			ForInt64          = new XmlSimpleSerializer<Int64>          (XmlConvert.ToString, XmlConvert.ToInt64),
			ForByte           = new XmlSimpleSerializer<Byte>           (XmlConvert.ToString, XmlConvert.ToByte),
			ForUInt16         = new XmlSimpleSerializer<UInt16>         (XmlConvert.ToString, XmlConvert.ToUInt16),
			ForUInt32         = new XmlSimpleSerializer<UInt32>         (XmlConvert.ToString, XmlConvert.ToUInt32),
			ForUInt64         = new XmlSimpleSerializer<UInt64>         (XmlConvert.ToString, XmlConvert.ToUInt64),
			ForSingle         = new XmlSimpleSerializer<Single>         (XmlConvert.ToString, XmlConvert.ToSingle),
			ForDouble         = new XmlSimpleSerializer<Double>         (XmlConvert.ToString, XmlConvert.ToDouble),
			ForDecimal        = new XmlSimpleSerializer<Decimal>        (XmlConvert.ToString, XmlConvert.ToDecimal),
			ForTimeSpan       = new XmlSimpleSerializer<TimeSpan>       (XmlConvert.ToString, XmlConvert.ToTimeSpan),
			ForDateTime       = new XmlSimpleSerializer<DateTime>       (XmlConvert_ToString, XmlConvert_ToDateTime),
			ForDateTimeOffset = new XmlSimpleSerializer<DateTimeOffset> (XmlConvert.ToString, XmlConvert.ToDateTimeOffset),
			ForGuid           = new XmlSimpleSerializer<Guid>           (XmlConvert.ToString, XmlConvert.ToGuid),
			ForByteArray      = new XmlSimpleSerializer<Byte[]>         (Convert.ToBase64String, Convert.FromBase64String),
			ForUri            = new XmlSimpleSerializer<Uri>            (u => u.ToString(), s => new Uri(s, UriKind.RelativeOrAbsolute));

		private static string XmlConvert_ToString(DateTime value)
		{
			return XmlConvert.ToString(value, XmlDateTimeSerializationMode.RoundtripKind);
		}

		private static DateTime XmlConvert_ToDateTime(string value)
		{
			return XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.RoundtripKind);
		}
	}
}
#endif
