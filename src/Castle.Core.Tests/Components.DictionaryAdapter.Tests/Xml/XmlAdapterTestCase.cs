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
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#if !SILVERLIGHT // Until support for other platforms is verified
namespace Castle.Components.DictionaryAdapter.Xml.Tests
{
	using System;
	using System.Xml;
	using System.Xml.Serialization;
	using NUnit.Framework;

	public abstract class XmlAdapterTestCase
	{
		private DictionaryAdapterFactory factory;

#if FEATURE_XUNITNET
		protected XmlAdapterTestCase()
		{
			SetUp();
		}
#else
		[SetUp]
#endif
		public virtual void SetUp()
		{
			factory = new DictionaryAdapterFactory();
		}

		protected static XmlDocument Xml(params string[] xml)
		{
			var document = new XmlDocument();
			var text = string.Concat(xml)
				.Replace("$xsd", "xmlns:xsd='http://www.w3.org/2001/XMLSchema'")
				.Replace("$xsi", "xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'")
				.Replace("$x",   "xmlns:x='urn:schemas-castle-org:xml-reference'");
			document.LoadXml(text);
			return document;
		}

		protected T Create<T>()
		{
			return Create<T>(new XmlDocument(), null);
		}

		protected T Create<T>(params string[] xml)
		{
			return Create<T>(Xml(xml), null);
		}

		protected T Create<T>(XmlNode storage)
		{
			return Create<T>(storage, null);
		}

		protected virtual T Create<T>(XmlNode storage, Action<PropertyDescriptor> config)
		{
			var xmlAdapter = new XmlAdapter(storage);

			var descriptor = new PropertyDescriptor()
				.AddBehaviors(XmlMetadataBehavior.Default, xmlAdapter);

			if (config != null)
				config(descriptor);

			var dictionary = new System.Collections.Hashtable();

			return (T) factory.GetAdapter(typeof(T), dictionary, descriptor);
		}

		public class FakeStandardXmlSerializable
		{
			public string Text { get; set; }
		}

		public class FakeCustomXmlSerializable : IXmlSerializable
		{
			public string Text { get; set; }

			System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema() { return null; }
			void IXmlSerializable.ReadXml (XmlReader reader) { Text = reader.ReadString(); }
			void IXmlSerializable.WriteXml(XmlWriter writer) { writer.WriteString(Text); }
		}

		protected const string
			Base64String = "VGVzdA==",
			GuidString   = "c7da18ce-aa3f-452d-bf8f-8e3bb9cdec2b";

		protected readonly byte[] Base64Bytes = Convert.FromBase64String(Base64String);
		protected readonly Guid   GuidValue   = new Guid(GuidString);
	}
}
#endif
