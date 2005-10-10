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

namespace Castle.ActiveRecord.Framework.Internal.Tests
{
	using System;

	using NUnit.Framework;

	using Castle.ActiveRecord.Framework.Internal.Tests.Model;

	[TestFixture]
	public class XmlGenerationTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void SimpleCaseWithKeyAndProperties()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create( typeof(ClassA) );
			Assert.IsNotNull( model );

			SemanticVerifierVisitor semanticVisitor = new SemanticVerifierVisitor(builder.Models);
			semanticVisitor.VisitNode( model );

			XmlGenerationVisitor xmlVisitor = new XmlGenerationVisitor();
			xmlVisitor.CreateXml(model);

			String xml = xmlVisitor.Xml;

			String expected = 
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.0\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassA, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"ClassA\"    >\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\"  unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <property name=\"Name1\" access=\"property\" column=\"Name1\" type=\"System.String\"     insert=\"false\" update=\"false\"  />\r\n" + 
				"    <property name=\"Name2\" access=\"property\" column=\"Name2\" type=\"System.String\"  unsaved-value=\"hammett\"      />\r\n" + 
				"    <property name=\"Name3\" access=\"property\" column=\"Name3\" type=\"System.String\"   not-null=\"true\" unique=\"true\"    />\r\n" + 
				"    <property name=\"Text\" access=\"property\" column=\"Text\" type=\"StringClob\"        />\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void AnyAttribute()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create( typeof(ClassWithAnyAttribute) );
			Assert.IsNotNull( model );

			SemanticVerifierVisitor semanticVisitor = new SemanticVerifierVisitor(builder.Models);
			semanticVisitor.VisitNode( model );

			XmlGenerationVisitor xmlVisitor = new XmlGenerationVisitor();
			xmlVisitor.CreateXml(model);

			String xml = xmlVisitor.Xml;

			String expected = 
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.0\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassWithAnyAttribute, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"ClassWithAnyAttribute\"    >\r\n" +
				"    <id name=\"Id\" access=\"nosetter.camelcase-underscore\" column=\"Id\" type=\"Int32\"  unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <any name=\"PaymentMethod\" access=\"property\" id-type=\"Int64\" meta-type=\"System.String\"    cascade=\"save-update\" >\r\n"+
				// "      <meta-value value=\"CREDIT_CARD\" class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.CreditCard, Castle.ActiveRecord.Framework.Internal.Tests\" />\r\n" +
				"      <meta-value value=\"BANK_ACCOUNT\" class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.BankAccount, Castle.ActiveRecord.Framework.Internal.Tests\" />\r\n" +
				"      <column name=\"BILLING_DETAILS_TYPE\" />\r\n"+
				"      <column name=\"BILLING_DETAILS_ID\" />\r\n"+
                "    </any>\r\n"+
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void HasManyToAnyAttribute()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(ClasssWithHasManyToAny));
			Assert.IsNotNull(model);

			SemanticVerifierVisitor semanticVisitor = new SemanticVerifierVisitor(builder.Models);
			semanticVisitor.VisitNode(model);

			XmlGenerationVisitor xmlVisitor = new XmlGenerationVisitor();
			xmlVisitor.CreateXml(model);

			String xml = xmlVisitor.Xml;
			String expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.0\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClasssWithHasManyToAny, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"ClasssWithHasManyToAny\"    >\r\n" +
				"    <id name=\"Id\" access=\"nosetter.camelcase-underscore\" column=\"Id\" type=\"Int32\"  unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <set name=\"PaymentMethod\" access=\"property\" table=\"payments_table\"       >\r\n" +
				"      <key column=\"pay_id\" />\r\n" +
				"      <many-to-any id-type=\"Int32\" >\r\n" +
				"        <column name=\"payment_type\" />\r\n" +
				"        <column name=\"payment_method_id\" />\r\n" +
				"      </many-to-any>\r\n" +
				"    </set>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void SimpleClassWithMappedFieldAndNonDefaultAccess()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create( typeof(ClassWithMappedField) );
			Assert.IsNotNull( model );

			SemanticVerifierVisitor semanticVisitor = new SemanticVerifierVisitor(builder.Models);
			semanticVisitor.VisitNode( model );

			XmlGenerationVisitor xmlVisitor = new XmlGenerationVisitor();
			xmlVisitor.CreateXml(model);

			String xml = xmlVisitor.Xml;

			String expected = 
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.0\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassWithMappedField, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"ClassWithMappedField\"    >\r\n" +
				"    <id name=\"Id\" access=\"nosetter.camelcase-underscore\" column=\"Id\" type=\"Int32\"  unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <property name=\"name1\" access=\"field\" column=\"MyCustomName\" type=\"System.String\"        />\r\n" + 
				"    <property name=\"Value\" access=\"CustomAccess\" column=\"Value\" type=\"Int32\"        />\r\n"+
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void DiscriminatorUse()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			builder.Create( typeof(ClassDiscriminatorA) );
			ActiveRecordModel model = builder.Create( typeof(ClassDiscriminatorParent) );
			Assert.IsNotNull( model );

			String xml = Process(builder, model);

			String expected = 
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" + 
				"<hibernate-mapping xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.0\">\r\n" + 
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassDiscriminatorParent, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"disctable\" discriminator-value=\"parent\"   >\r\n" + 
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\"  unsaved-value=\"0\">\r\n" + 
				"      <generator class=\"native\">\r\n" + 
				"      </generator>\r\n" + 
				"    </id>\r\n" + 
				"    <discriminator column=\"type\" type=\"String\" />\r\n" + 
				"    <property name=\"Name\" access=\"property\" column=\"Name\" type=\"System.String\"        />\r\n" + 
				"    <subclass name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassDiscriminatorA, Castle.ActiveRecord.Framework.Internal.Tests\"  discriminator-value=\"A\">\r\n" + 
				"      <property name=\"Age\" access=\"property\" column=\"Age\" type=\"Int32\"        />\r\n" + 
				"    </subclass>\r\n" + 
				"  </class>\r\n" + 
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void JoinedSubClassUse()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			builder.Create( typeof(ClassJoinedSubClassA) );
			ActiveRecordModel model = builder.Create( typeof(ClassJoinedSubClassParent) );
			Assert.IsNotNull( model );

			String xml = Process(builder, model);

			String expected = 
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.0\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassJoinedSubClassParent, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"disctable\"    >\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\"  unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <property name=\"Name\" access=\"property\" column=\"Name\" type=\"System.String\"        />\r\n" +
				"    <joined-subclass name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassJoinedSubClassA, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"disctablea\"   >\r\n" +
				"      <key column=\"AId\" />\r\n" +
				"      <property name=\"Age\" access=\"property\" column=\"Age\" type=\"Int32\"        />\r\n" +
				"    </joined-subclass>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void VersionedClass()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create( typeof(VersionedClass) );
			Assert.IsNotNull( model );

			String xml = Process(builder, model);

			String expected = 
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" + 
				"<hibernate-mapping xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.0\">\r\n" + 
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.VersionedClass, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"VersionedClass\"    >\r\n" + 
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\"  unsaved-value=\"0\">\r\n" + 
				"      <generator class=\"native\">\r\n" + 
				"      </generator>\r\n" + 
				"    </id>\r\n" + 
				"    <version name=\"Ver\" access=\"property\" column=\"Ver\" type=\"System.String\" />\r\n" + 
				"    <property name=\"Name\" access=\"property\" column=\"Name\" type=\"System.String\"        />\r\n" + 
				"  </class>\r\n" + 
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void TimestampedClass()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create( typeof(TimestampedClass) );
			Assert.IsNotNull( model );

			String xml = Process(builder, model);

			String expected = 
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" + 
				"<hibernate-mapping xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.0\">\r\n" + 
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.TimestampedClass, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"TimestampedClass\"    >\r\n" + 
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\"  unsaved-value=\"0\">\r\n" + 
				"      <generator class=\"native\">\r\n" + 
				"      </generator>\r\n" + 
				"    </id>\r\n" + 
				"    <timestamp name=\"Ts\" access=\"property\" column=\"Ts\" />\r\n" + 
				"    <property name=\"Name\" access=\"property\" column=\"Name\" type=\"System.String\"        />\r\n" + 
				"  </class>\r\n" + 
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void SequenceParamClass()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create( typeof(SequenceParamClass) );
			Assert.IsNotNull( model );

			String xml = Process(builder, model);

			String expected = 
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.0\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.SequenceParamClass, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"SequenceParamClass\"    >\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\"  unsaved-value=\"0\">\r\n" +
				"      <generator class=\"sequence\">\r\n" +
				"        <param name=\"sequence\">my_seq</param>\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void BelongsTo1()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create( typeof(BelongsToClassA) );
			Assert.IsNotNull( model );

			String xml = Process(builder, model);

			String expected = 
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" + 
				"<hibernate-mapping xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.0\">\r\n" + 
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.BelongsToClassA, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"BelongsToClassA\"    >\r\n" + 
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\"  unsaved-value=\"0\">\r\n" + 
				"      <generator class=\"native\">\r\n" + 
				"      </generator>\r\n" + 
				"    </id>\r\n" + 
				"    <many-to-one name=\"ClassA\" access=\"property\" class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassA, Castle.ActiveRecord.Framework.Internal.Tests\" column=\"classa_id\"      />\r\n" + 
				"  </class>\r\n" + 
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void BelongsTo2()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create( typeof(BelongsToClassA2) );
			Assert.IsNotNull( model );

			String xml = Process(builder, model);

			String expected = 
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" + 
				"<hibernate-mapping xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.0\">\r\n" + 
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.BelongsToClassA2, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"BelongsToClassA2\"    >\r\n" + 
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\"  unsaved-value=\"0\">\r\n" + 
				"      <generator class=\"native\">\r\n" + 
				"      </generator>\r\n" + 
				"    </id>\r\n" + 
				"    <many-to-one name=\"ClassA\" access=\"property\" class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassA, Castle.ActiveRecord.Framework.Internal.Tests\" column=\"classa_id\" insert=\"false\" update=\"false\" not-null=\"true\" unique=\"true\" cascade=\"save-update\" />\r\n" + 
				"  </class>\r\n" + 
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void HasManyWithExplicitInfo()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create( typeof(HasManyClassA) );
			Assert.IsNotNull( model );

			String xml = Process(builder, model);

			String expected = 
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" + 
				"<hibernate-mapping xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.0\">\r\n" + 
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.HasManyClassA, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"HasManyClassA\"    >\r\n" + 
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\"  unsaved-value=\"0\">\r\n" + 
				"      <generator class=\"native\">\r\n" + 
				"      </generator>\r\n" + 
				"    </id>\r\n" + 
				"    <bag name=\"Items\" access=\"property\" table=\"ClassATable\"   inverse=\"true\"    >\r\n" + 
				"      <key column=\"keycol\" />\r\n" + 
				"      <one-to-many class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassA, Castle.ActiveRecord.Framework.Internal.Tests\" />\r\n" + 
				"    </bag>\r\n" + 
				"  </class>\r\n" + 
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void HasManyInfering()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create( typeof(Category) );
			Assert.IsNotNull( model );

			String xml = Process(builder, model);

			String expected = 
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.0\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.Category, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"Category\"    >\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\"  unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <property name=\"Name\" access=\"property\" column=\"Name\" type=\"System.String\"        />\r\n" +
				"    <many-to-one name=\"Parent\" access=\"property\" class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.Category, Castle.ActiveRecord.Framework.Internal.Tests\" column=\"parent_id\"      />\r\n" +
				"    <bag name=\"SubCategories\" access=\"property\" table=\"Category\"       >\r\n" +
				"      <key column=\"parent_id\" />\r\n" +
				"      <one-to-many class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.Category, Castle.ActiveRecord.Framework.Internal.Tests\" />\r\n" +
				"    </bag>\r\n" +
				"  </class>\r\n" + 
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		private string Process(ActiveRecordModelBuilder builder, ActiveRecordModel model)
		{
			GraphConnectorVisitor connectorVisitor = new GraphConnectorVisitor( builder.Models );
			connectorVisitor.VisitNode( model );
	
			SemanticVerifierVisitor semanticVisitor = new SemanticVerifierVisitor(builder.Models);
			semanticVisitor.VisitNode( model );
	
			XmlGenerationVisitor xmlVisitor = new XmlGenerationVisitor();
			xmlVisitor.CreateXml(model);

			return xmlVisitor.Xml;
		}
	}
}
