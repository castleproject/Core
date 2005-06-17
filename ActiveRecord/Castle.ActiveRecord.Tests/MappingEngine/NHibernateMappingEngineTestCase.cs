// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Tests.MappingEngine
{
	using System;
	using System.Collections;
	using NUnit.Framework;
	using Castle.ActiveRecord.Tests.Model;
	using System.IO;
	using System.Reflection;
	using System.Xml;

	[TestFixture]
	public class NHibernateMappingEngineTestCase
	{		
		private Hashtable mappings = new Hashtable();

		[SetUp]
		public void init()
		{
			string resourceName = Assembly.GetExecutingAssembly().GetName().Name + ".TestXmlMappings.Mappings.xml";
			Stream xmlStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
			Assert.IsNotNull(xmlStream);
			
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(xmlStream);
			XmlNodeList nodes = xmlDoc.SelectNodes("/xml_samples/xml_mapping");
			Assert.IsTrue(nodes.Count > 0);
			foreach (XmlNode node in nodes)
				mappings[node.Attributes["name"].Value] = node.FirstChild.InnerText;
			xmlStream.Close();
		}


		[Test]
		public void SimpleModelNoRelations()
		{
			String contents = "\r\n<hibernate-mapping xmlns=\"urn:nhibernate-mapping-2.0\" >\r\n" + 
				"<class name=\"Castle.ActiveRecord.Tests.Model.SimpleModel, Castle.ActiveRecord.Tests\" table=\"TableNameHere\" >\r\n" + 
				"<id name=\"id\" type=\"Int32\" column=\"t_id\" unsaved-value=\"0\" >\r\n" + 
				"<generator class=\"assigned\">\r\n" + 
				"</generator>\r\n" + 
				"</id>\r\n" + 
				"<property name=\"name\"  type=\"String\" column=\"t_name\" >\r\n" + 
				"</property>\r\n" + 
				"</class>\r\n" + 
				"</hibernate-mapping>";

			NHibernateMappingEngine engine = new NHibernateMappingEngine();
			String xml = engine.CreateMapping( typeof(SimpleModel), new Type[] { typeof(SimpleModel) } );
			Assert.AreEqual(contents, xml);
		}

		[Test]
		public void SimpleModel2NoRelations()
		{
			String contents = "\r\n<hibernate-mapping xmlns=\"urn:nhibernate-mapping-2.0\" >\r\n" + 
				"<class name=\"Castle.ActiveRecord.Tests.Model.SimpleModel2, Castle.ActiveRecord.Tests\" table=\"TableNameHere\" >\r\n" + 
				"<id name=\"id\" type=\"Int32\" column=\"t_id\" unsaved-value=\"0\" >\r\n" + 
				"<generator class=\"native\">\r\n" + 
				"</generator>\r\n" + 
				"</id>\r\n" + 
				"<property name=\"name\"  type=\"StringClob\" column=\"t_name\" >\r\n" + 
				"</property>\r\n" + 
				"</class>\r\n" + 
				"</hibernate-mapping>";

			NHibernateMappingEngine engine = new NHibernateMappingEngine();
			String xml = engine.CreateMapping( typeof(SimpleModel2), new Type[] { typeof(SimpleModel2) } );
			Assert.AreEqual(contents, xml);
		}

		[Test]
		public void SimpleModel3NoRelations()
		{
			String contents = "\r\n<hibernate-mapping xmlns=\"urn:nhibernate-mapping-2.0\" >\r\n" + 
				"<class name=\"Castle.ActiveRecord.Tests.Model.SimpleModel3, Castle.ActiveRecord.Tests\" table=\"TableNameHere\" >\r\n" + 
				"<id name=\"id\" type=\"Int32\" column=\"t_id\" unsaved-value=\"0\" >\r\n" + 
				"<generator class=\"guid\">\r\n" + 
				"</generator>\r\n" + 
				"</id>\r\n" + 
				"<property name=\"name\"  type=\"StringClob\" column=\"t_name\" length=\"20\" not-null=\"true\" >\r\n" + 
				"</property>\r\n" + 
				"</class>\r\n" + 
				"</hibernate-mapping>";

			NHibernateMappingEngine engine = new NHibernateMappingEngine();
			String xml = engine.CreateMapping( typeof(SimpleModel3), new Type[] { typeof(SimpleModel3) } );
			Assert.AreEqual(contents, xml);
		}
		
		[Test]
		public void ManyToManySetMapping()
		{
			string expected = (string) mappings["ManyToManyWithSet"];
			Assert.IsTrue(expected.Length > 0);
			
			NHibernateMappingEngine engine = new NHibernateMappingEngine();
			string xml = engine.CreateMapping(typeof(Order), new Type[] { typeof(Order) });
			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void ManyToManyIDMapping()
		{
			string expected = (string) mappings["ManyToManyWithIDBag"];
			Assert.IsTrue(expected.Length > 0);
			
			NHibernateMappingEngine engine = new NHibernateMappingEngine();
			string xml = engine.CreateMapping(typeof(OrderWithIDBag), new Type[] { typeof(OrderWithIDBag) });
			Assert.AreEqual(expected, xml);
		}
	}
}
