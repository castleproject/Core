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

namespace Castle.ActiveRecord.Tests
{
	using System;
	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Framework.Internal;
	using Castle.ActiveRecord.Tests.Model.AnyModel;
	using NUnit.Framework;

	[TestFixture]
	public class AnyRelationTestCase : AbstractActiveRecordTest
	{
		[Test]
		[ExpectedException(typeof(ActiveRecordException))]
		public void TestInvalidAnyMapping()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(ModelWithInCorrectMapping));
		}
		
		[Test]
		public void XmlConfigTest()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(Order));
			Assert.IsNotNull(model);

			SemanticVerifierVisitor semanticVisitor = new SemanticVerifierVisitor(builder.Models);
			semanticVisitor.VisitNode(model);

			XmlGenerationVisitor xmlVisitor = new XmlGenerationVisitor();
			xmlVisitor.CreateXml(model);

			String xml = xmlVisitor.Xml;

			String expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Tests.Model.AnyModel.Order, Castle.ActiveRecord.Tests\" table=\"Orders\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <bag name=\"Payments\" access=\"property\" table=\"payments_table\" lazy=\"false\">\r\n" +
				"      <key column=\"pay_id\" />\r\n" +
				"      <many-to-any id-type=\"Int32\" meta-type=\"System.String\">\r\n" +
				"        <meta-value value=\"BANK_ACCOUNT\" class=\"Castle.ActiveRecord.Tests.Model.AnyModel.BankAccounts, Castle.ActiveRecord.Tests\" />\r\n" +
				"        <meta-value value=\"CREDIT_CARD\" class=\"Castle.ActiveRecord.Tests.Model.AnyModel.CreditCards, Castle.ActiveRecord.Tests\" />\r\n" +
				"        <column name=\"Billing_Details_Type\" />\r\n" +
				"        <column name=\"Billing_Details_Id\" />\r\n" +
				"      </many-to-any>\r\n" +
				"    </bag>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void SchemaTest()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(BankAccounts), typeof(Order), typeof(CreditCards));
			Recreate();
		}
	}
}
