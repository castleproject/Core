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

namespace Castle.ActiveRecord.Framework.Internal.Tests
{
	using Castle.ActiveRecord.Framework.Internal.Tests.Model;

	using NUnit.Framework;

	[TestFixture]
	public class ImportsTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void ImportsGeneration()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(ImportClass));
			Assert.IsNotNull(model);

			string xml = Process(builder, model);

			string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
					"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
					"  <import class=\"Castle.ActiveRecord.Framework.Internal.Tests.ImportClassRow, Castle.ActiveRecord.Framework.Internal.Tests\" rename=\"ImportClassRow\" />\r\n" +
					"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ImportClass, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"ImportClass\" lazy=\"false\">\r\n" +
					"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
					"      <generator class=\"native\">\r\n" +
					"      </generator>\r\n" +
					"    </id>\r\n" +
					"    <property name=\"Name\" access=\"property\" type=\"String\">\r\n" +
					"      <column name=\"Name\"/>\r\n" + 
					"    </property>\r\n" +
					"  </class>\r\n" +
					"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		private string Process(ActiveRecordModelBuilder builder, ActiveRecordModel model)
		{
			GraphConnectorVisitor connectorVisitor = new
				GraphConnectorVisitor(builder.Models);
			connectorVisitor.VisitNode(model);

			SemanticVerifierVisitor semanticVisitor = new
				SemanticVerifierVisitor(builder.Models);
			semanticVisitor.VisitNode(model);

			XmlGenerationVisitor xmlVisitor = new XmlGenerationVisitor();
			xmlVisitor.CreateXml(model);

			return xmlVisitor.Xml;
		}
	}

	public class ImportClassRow
	{
	}
}
