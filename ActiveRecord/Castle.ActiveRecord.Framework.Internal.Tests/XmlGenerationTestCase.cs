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

namespace Castle.ActiveRecord.Framework.Internal.Tests
{
	using System;
	using NUnit.Framework;
	using Castle.ActiveRecord.Framework.Internal.Tests.Model;

	[TestFixture]
	public class XmlGenerationTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void SimpleCaseWithPrimaryAssigned()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(ClassKeyAssigned));
			Assert.IsNotNull(model);

			SemanticVerifierVisitor semanticVisitor = new SemanticVerifierVisitor(builder.Models);
			semanticVisitor.VisitNode(model);

			XmlGenerationVisitor xmlVisitor = new XmlGenerationVisitor();
			xmlVisitor.CreateXml(model);

			String xml = xmlVisitor.Xml;

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassKeyAssigned, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"ClassKeyAssigned\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"assigned\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <property name=\"Name1\" access=\"property\" type=\"String\">\r\n" +
				"      <column name=\"Name1\"/>\r\n" +
				"    </property>\r\n" +
				"    <property name=\"Insertion\" access=\"property\" type=\"System.DateTime\">\r\n" +
				"      <column name=\"Insertion\" default=\"getdate()\"/>\r\n" +
				"    </property>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void SimpleCaseWithNestedComponent()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(SimpleNestedComponent));
			Assert.IsNotNull(model);

			SemanticVerifierVisitor semanticVisitor = new SemanticVerifierVisitor(builder.Models);
			semanticVisitor.VisitNode(model);

			XmlGenerationVisitor xmlVisitor = new XmlGenerationVisitor();
			xmlVisitor.CreateXml(model);

			String xml = xmlVisitor.Xml;

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.SimpleNestedComponent, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"SimpleNestedComponent\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <component name=\"Nested\" class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.NestedComponent, Castle.ActiveRecord.Framework.Internal.Tests\" access=\"property\">\r\n" +
				"      <parent name=\"Parent\"/>\r\n" +
				"      <property name=\"NestedProperty\" access=\"property\" type=\"String\">\r\n" +
				"        <column name=\"NestedProperty\"/>\r\n" +
				"      </property>\r\n" +
				"    </component>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}


		[Test]
		public void SimpleCaseWithNestedComponentWithFieldAccess()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(SimpleNestedComponentWithFieldAccess));
			Assert.IsNotNull(model);

			SemanticVerifierVisitor semanticVisitor = new SemanticVerifierVisitor(builder.Models);
			semanticVisitor.VisitNode(model);

			XmlGenerationVisitor xmlVisitor = new XmlGenerationVisitor();
			xmlVisitor.CreateXml(model);

			String xml = xmlVisitor.Xml;

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.SimpleNestedComponentWithFieldAccess, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"SimpleNestedComponentWithFieldAccess\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <component name=\"Nested\" class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.NestedComponent, Castle.ActiveRecord.Framework.Internal.Tests\" access=\"nosetter.camelcase\">\r\n" +
				"      <parent name=\"Parent\"/>\r\n" +
				"      <property name=\"NestedProperty\" access=\"property\" type=\"String\">\r\n" +
				"        <column name=\"NestedProperty\"/>\r\n" +
				"      </property>\r\n" +
				"    </component>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}


		[Test]
		public void SimpleCaseWithKeyAndProperties()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(ClassA));
			Assert.IsNotNull(model);

			SemanticVerifierVisitor semanticVisitor = new SemanticVerifierVisitor(builder.Models);
			semanticVisitor.VisitNode(model);

			XmlGenerationVisitor xmlVisitor = new XmlGenerationVisitor();
			xmlVisitor.CreateXml(model);

			String xml = xmlVisitor.Xml;

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassA, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"ClassA\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <property name=\"Name1\" access=\"property\" type=\"String\" insert=\"false\" update=\"false\">\r\n" +
				"      <column name=\"Name1\"/>\r\n" +
				"    </property>\r\n" +
				"    <property name=\"Name2\" access=\"property\" type=\"String\">\r\n" +
				"      <column name=\"Name2\"/>\r\n" +
				"    </property>\r\n" +
				"    <property name=\"Name3\" access=\"property\" type=\"String\">\r\n" +
				"      <column name=\"Name3\" not-null=\"true\" unique=\"true\"/>\r\n" +
				"    </property>\r\n" +
				"    <property name=\"Text\" access=\"property\" type=\"StringClob\">\r\n" +
				"      <column name=\"Text\"/>\r\n" +
				"    </property>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void SimpleCaseWithKeyPropertiesAndDynamicInsertUpdate()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(ClassADynamicInsertUpdate));
			Assert.IsNotNull(model);

			SemanticVerifierVisitor semanticVisitor = new SemanticVerifierVisitor(builder.Models);
			semanticVisitor.VisitNode(model);

			XmlGenerationVisitor xmlVisitor = new XmlGenerationVisitor();
			xmlVisitor.CreateXml(model);

			String xml = xmlVisitor.Xml;

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassADynamicInsertUpdate, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"ClassA\" dynamic-update=\"true\" dynamic-insert=\"true\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <property name=\"Name1\" access=\"property\" type=\"String\" insert=\"false\" update=\"false\">\r\n" +
				"      <column name=\"Name1\"/>\r\n" +
				"    </property>\r\n" +
				"    <property name=\"Name2\" access=\"property\" type=\"String\">\r\n" +
				"      <column name=\"Name2\"/>\r\n" +
				"    </property>\r\n" +
				"    <property name=\"Name3\" access=\"property\" type=\"String\">\r\n" +
				"      <column name=\"Name3\" not-null=\"true\" unique=\"true\"/>\r\n" +
				"    </property>\r\n" +
				"    <property name=\"Text\" access=\"property\" type=\"StringClob\">\r\n" +
				"      <column name=\"Text\"/>\r\n" +
				"    </property>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void SimpleCaseWithKeyPropertiesAndCustomPersister()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(ClassWithCustomPersister));
			Assert.IsNotNull(model);

			SemanticVerifierVisitor semanticVisitor = new SemanticVerifierVisitor(builder.Models);
			semanticVisitor.VisitNode(model);

			XmlGenerationVisitor xmlVisitor = new XmlGenerationVisitor();
			xmlVisitor.CreateXml(model);

			String xml = xmlVisitor.Xml;

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassWithCustomPersister, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"ClassA\" persister=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.CustomPersister, Castle.ActiveRecord.Framework.Internal.Tests\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <property name=\"Name1\" access=\"property\" type=\"String\" insert=\"false\" update=\"false\">\r\n" +
				"      <column name=\"Name1\"/>\r\n" +
				"    </property>\r\n" +
				"    <property name=\"Name2\" access=\"property\" type=\"String\">\r\n" +
				"      <column name=\"Name2\"/>\r\n" +
				"    </property>\r\n" +
				"    <property name=\"Name3\" access=\"property\" type=\"String\">\r\n" +
				"      <column name=\"Name3\" not-null=\"true\" unique=\"true\"/>\r\n" +
				"    </property>\r\n" +
				"    <property name=\"Text\" access=\"property\" type=\"StringClob\">\r\n" +
				"      <column name=\"Text\"/>\r\n" +
				"    </property>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void SimpleCaseWithBatch_SelectBeforeUpdate_Locking_Polimorphism()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(ClassWithSomeCustomOptions));
			Assert.IsNotNull(model);

			SemanticVerifierVisitor semanticVisitor = new SemanticVerifierVisitor(builder.Models);
			semanticVisitor.VisitNode(model);

			XmlGenerationVisitor xmlVisitor = new XmlGenerationVisitor();
			xmlVisitor.CreateXml(model);

			String xml = xmlVisitor.Xml;

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassWithSomeCustomOptions, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"ClassA\" select-before-update=\"true\" polymorphism=\"explicit\" batch-size=\"10\" optimistic-lock=\"dirty\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <property name=\"Name1\" access=\"property\" type=\"String\" insert=\"false\" update=\"false\">\r\n" +
				"      <column name=\"Name1\"/>\r\n" +
				"    </property>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void AnyAttribute()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(ClassWithAnyAttribute));
			Assert.IsNotNull(model);

			SemanticVerifierVisitor semanticVisitor = new SemanticVerifierVisitor(builder.Models);
			semanticVisitor.VisitNode(model);

			XmlGenerationVisitor xmlVisitor = new XmlGenerationVisitor();
			xmlVisitor.CreateXml(model);

			String xml = xmlVisitor.Xml;

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassWithAnyAttribute, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"ClassWithAnyAttribute\">\r\n" +
				"    <id name=\"Id\" access=\"nosetter.camelcase-underscore\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <any name=\"PaymentMethod\" access=\"property\" id-type=\"Int64\" meta-type=\"System.String\" cascade=\"save-update\" not-null=\"true\">\r\n" +
				"      <meta-value value=\"BANK_ACCOUNT\" class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.BankAccount, Castle.ActiveRecord.Framework.Internal.Tests\" />\r\n" +
				"      <column name=\"BILLING_DETAILS_TYPE\" />\r\n" +
				"      <column name=\"BILLING_DETAILS_ID\" />\r\n" +
				"    </any>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void AnyAttributeUsingGenericId()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(ClassWithAnyAttributeUsingGenericId));
			Assert.IsNotNull(model);

			SemanticVerifierVisitor semanticVisitor = new SemanticVerifierVisitor(builder.Models);
			semanticVisitor.VisitNode(model);

			XmlGenerationVisitor xmlVisitor = new XmlGenerationVisitor();
			xmlVisitor.CreateXml(model);

			String xml = xmlVisitor.Xml;

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassWithAnyAttributeUsingGenericId, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"ClassWithAnyAttributeUsingGenericId\">\r\n" +
				"    <id name=\"Id\" access=\"nosetter.camelcase-underscore\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <any name=\"PaymentMethod\" access=\"property\" id-type=\"System.Nullable`1[[System.Int64, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]\" meta-type=\"System.String\" cascade=\"save-update\" not-null=\"true\">\r\n" +
				"      <meta-value value=\"BANK_ACCOUNT\" class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.BankAccount, Castle.ActiveRecord.Framework.Internal.Tests\" />\r\n" +
				"      <column name=\"BILLING_DETAILS_TYPE\" />\r\n" +
				"      <column name=\"BILLING_DETAILS_ID\" />\r\n" +
				"    </any>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void LazyForClass()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			builder.Create(typeof(ClassWithAnyAttribute));
			ActiveRecordModel model = builder.Create(typeof(LazyClass));
			Assert.IsNotNull(model);

			SemanticVerifierVisitor semanticVisitor = new SemanticVerifierVisitor(builder.Models);
			semanticVisitor.VisitNode(model);

			XmlGenerationVisitor xmlVisitor = new XmlGenerationVisitor();
			xmlVisitor.CreateXml(model);

			String xml = xmlVisitor.Xml;

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.LazyClass, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"LazyClass\" lazy=\"true\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <many-to-one name=\"Clazz\" access=\"property\" class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassWithAnyAttribute, Castle.ActiveRecord.Framework.Internal.Tests\" column=\"Clazz\" lazy=\"proxy\" />\r\n" +
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
			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClasssWithHasManyToAny, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"ClasssWithHasManyToAny\">\r\n" +
				"    <id name=\"Id\" access=\"nosetter.camelcase-underscore\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <set name=\"PaymentMethod\" access=\"property\" table=\"payments_table\" lazy=\"false\">\r\n" +
				"      <key column=\"pay_id\" />\r\n" +
				"      <many-to-any id-type=\"Int32\" meta-type=\"Int32\">\r\n" +
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
			ActiveRecordModel model = builder.Create(typeof(ClassWithMappedField));
			Assert.IsNotNull(model);

			SemanticVerifierVisitor semanticVisitor = new SemanticVerifierVisitor(builder.Models);
			semanticVisitor.VisitNode(model);

			XmlGenerationVisitor xmlVisitor = new XmlGenerationVisitor();
			xmlVisitor.CreateXml(model);

			String xml = xmlVisitor.Xml;

			const string expected = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
									"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
									"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassWithMappedField, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"ClassWithMappedField\">\r\n" +
									"    <id name=\"Id\" access=\"nosetter.camelcase-underscore\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
									"      <generator class=\"native\">\r\n" +
									"      </generator>\r\n" +
									"    </id>\r\n" +
									"    <property name=\"name1\" access=\"field\" type=\"String\">\r\n" +
									"      <column name=\"MyCustomName\"/>\r\n" +
									"    </property>\r\n" +
									"    <property name=\"Value\" access=\"CustomAccess\" type=\"Int32\">\r\n" +
									"      <column name=\"Value\"/>\r\n" +
									"    </property>\r\n" +
									"  </class>\r\n" +
									"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void SimpleCompositeClass()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(ClassWithCompositeKey2));
			Assert.IsNotNull(model);

			String xml = Process(builder, model);
			const string expected = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
									"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
									"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassWithCompositeKey2, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"ClassWithCompositeKey2\" lazy=\"false\">\r\n" +
									"    <composite-id name=\"Key\" class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.CompositeKey2, Castle.ActiveRecord.Framework.Internal.Tests\" unsaved-value=\"none\" access=\"property\">\r\n" +
									"      <key-property name=\"Key1\" access=\"property\" column=\"Key1\" type=\"Int32\" />\r\n" +
									"      <key-property name=\"Key2\" access=\"property\" column=\"Key2\" type=\"String\" />\r\n" +
									"    </composite-id>\r\n" +
									"  </class>\r\n" +
									"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test, Ignore("This is not implemented yet")]
		public void CompositeClassWithBelongsTo()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(ClassWithCompositeKey3));
			Assert.IsNotNull(model);

			String xml = Process(builder, model);
			const string expected = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
									"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
									"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassWithCompositeKey3, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"ClassWithCompositeKey3\" lazy=\"false\">\r\n" +
									"    <composite-id name=\"Key\" class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.CompositeKey2, Castle.ActiveRecord.Framework.Internal.Tests\" unsaved-value=\"none\" access=\"property\">\r\n" +
									"      <key-property name=\"Key1\" access=\"property\" column=\"Key1\" type=\"Int32\" />\r\n" +
									"      <key-many-to-one name=\"Key2\" access=\"property\" column=\"Key2\" type=\"String\" />\r\n" +
									"    </composite-id>\r\n" +
									"  </class>\r\n" +
									"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void SimpleClassWithElementList()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(ClassWithElementList));
			Assert.IsNotNull(model);

			String xml = Process(builder, model);
			const string expected = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
									"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
									"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassWithElementList, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"ClassWithElementList\">\r\n" +
									"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
									"      <generator class=\"native\">\r\n" +
									"      </generator>\r\n" +
									"    </id>\r\n" +
									"    <bag name=\"Elements\" access=\"property\" table=\"Elements\" lazy=\"false\">\r\n" +
									"      <key column=\"ClassId\" />\r\n" +
									"      <element  column=\"Name\"  type=\"System.String, mscorlib\"/>\r\n" +
									"    </bag>\r\n" +
									"  </class>\r\n" +
									"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void SimpleClassWithGuessedEnumElementList()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(EnumModel));
			Assert.IsNotNull(model);

			String xml = Process(builder, model);
			const string expected = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
									"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
									"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.EnumModel, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"EnumModel\">\r\n" +
									"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
									"      <generator class=\"native\">\r\n" +
									"      </generator>\r\n" +
									"    </id>\r\n" +
									"    <bag name=\"Roles\" access=\"property\" table=\"Roles\" lazy=\"false\">\r\n" +
									"      <key column=\"EnumModelId\" />\r\n" +
									"      <element  column=\"RoleId\"  type=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.Role, Castle.ActiveRecord.Framework.Internal.Tests\"/>\r\n" +
									"    </bag>\r\n" +
									"  </class>\r\n" +
									"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void DiscriminatorUse()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			builder.Create(typeof(ClassDiscriminatorA));
			ActiveRecordModel model = builder.Create(typeof(ClassDiscriminatorParent));
			Assert.IsNotNull(model);

			String xml = Process(builder, model);

			const string expected = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
									"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
									"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassDiscriminatorParent, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"disctable\" discriminator-value=\"parent\">\r\n" +
									"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
									"      <generator class=\"native\">\r\n" +
									"      </generator>\r\n" +
									"    </id>\r\n" +
									"    <discriminator column=\"type\" type=\"String\" />\r\n" +
									"    <property name=\"Name\" access=\"property\" type=\"String\">\r\n" +
									"      <column name=\"Name\"/>\r\n" +
									"    </property>\r\n" +
									"    <subclass name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassDiscriminatorA, Castle.ActiveRecord.Framework.Internal.Tests\" discriminator-value=\"A\">\r\n" +
									"      <property name=\"Age\" access=\"property\" type=\"Int32\">\r\n" +
									"        <column name=\"Age\"/>\r\n" +
									"      </property>\r\n" +
									"    </subclass>\r\n" +
									"  </class>\r\n" +
									"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void DiscriminatorLengthUse()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			builder.Create(typeof(ClassDiscriminatorB));
			ActiveRecordModel model = builder.Create(typeof(ClassDiscriminatorLengthParent));
			Assert.IsNotNull(model);

			String xml = Process(builder, model);

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassDiscriminatorLengthParent, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"disctable\" discriminator-value=\"parent\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <discriminator column=\"type\" type=\"String\" length=\"10\" />\r\n" +
				"    <property name=\"Name\" access=\"property\" type=\"String\">\r\n" +
				"      <column name=\"Name\"/>\r\n" +
				"    </property>\r\n" +
				"    <subclass name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassDiscriminatorB, Castle.ActiveRecord.Framework.Internal.Tests\" discriminator-value=\"B\">\r\n" +
				"      <property name=\"Age\" access=\"property\" type=\"Int32\">\r\n" +
				"        <column name=\"Age\"/>\r\n" +
				"      </property>\r\n" +
				"    </subclass>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void JoinedSubClassUseWithGenericType()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			builder.Create(typeof(GenClassJoinedSubClassA));
			ActiveRecordModel model = builder.Create(typeof(GenClassJoinedSubClassParent));
			Assert.IsNotNull(model);

			String xml = Process(builder, model);

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.GenClassJoinedSubClassParent, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"disctable\" lazy=\"false\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <property name=\"Name\" access=\"property\" type=\"String\">\r\n" +
				"      <column name=\"Name\"/>\r\n" +
				"    </property>\r\n" +
				"    <joined-subclass name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.GenClassJoinedSubClassA, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"disctablea\" lazy=\"false\">\r\n" +
				"      <key column=\"AId\" />\r\n" +
				"      <property name=\"Age\" access=\"property\" type=\"Int32\">\r\n" +
				"        <column name=\"Age\"/>\r\n" +
				"      </property>\r\n" +
				"    </joined-subclass>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void JoinedSubClassUseWithGenericTypeAndAbstractBase()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			builder.Create(typeof(SubClassJoinedClass));
			ActiveRecordModel model = builder.Create(typeof(BaseJoinedClass));
			Assert.IsNotNull(model);

			String xml = Process(builder, model);

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.BaseJoinedClass, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"disctable\" lazy=\"false\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <property name=\"Name\" access=\"property\" type=\"String\">\r\n" +
				"      <column name=\"Name\"/>\r\n" +
				"    </property>\r\n" +
				"    <joined-subclass name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.SubClassJoinedClass, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"disctablea\" lazy=\"false\">\r\n" +
				"      <key column=\"AId\" />\r\n" +
				"      <property name=\"Age\" access=\"property\" type=\"Int32\">\r\n" +
				"        <column name=\"Age\"/>\r\n" +
				"      </property>\r\n" +
				"    </joined-subclass>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void JoinedSubClassUseWithGenericTypeAndGenericAbstractBase()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			builder.Create(typeof(GenSubClassJoinedClass));
			ActiveRecordModel model = builder.Create(typeof(GenBaseJoinedClass<GenSubClassJoinedClass>));
			Assert.IsNotNull(model);

			String xml = Process(builder, model);

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.GenBaseJoinedClass`1[[Castle.ActiveRecord.Framework.Internal.Tests.Model.GenSubClassJoinedClass, Castle.ActiveRecord.Framework.Internal.Tests, Version=2.0.1000.0, Culture=neutral, PublicKeyToken=null]], Castle.ActiveRecord.Framework.Internal.Tests\" table=\"disctable\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <property name=\"Name\" access=\"property\" type=\"String\">\r\n" +
				"      <column name=\"Name\"/>\r\n" +
				"    </property>\r\n" +
				"    <joined-subclass name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.GenSubClassJoinedClass, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"disctablea\" lazy=\"false\">\r\n" +
				"      <key column=\"AId\" />\r\n" +
				"      <property name=\"Age\" access=\"property\" type=\"Int32\">\r\n" +
				"        <column name=\"Age\"/>\r\n" +
				"      </property>\r\n" +
				"    </joined-subclass>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void JoinedSubClassUse()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			builder.Create(typeof(ClassJoinedSubClassA));
			ActiveRecordModel model = builder.Create(typeof(ClassJoinedSubClassParent));
			Assert.IsNotNull(model);

			String xml = Process(builder, model);

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassJoinedSubClassParent, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"disctable\" lazy=\"false\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <property name=\"Name\" access=\"property\" type=\"String\">\r\n" +
				"      <column name=\"Name\"/>\r\n" +
				"    </property>\r\n" +
				"    <joined-subclass name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassJoinedSubClassA, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"disctablea\" lazy=\"false\">\r\n" +
				"      <key column=\"AId\" />\r\n" +
				"      <property name=\"Age\" access=\"property\" type=\"Int32\">\r\n" +
				"        <column name=\"Age\"/>\r\n" +
				"      </property>\r\n" +
				"    </joined-subclass>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void VersionedClass()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(VersionedClass));
			Assert.IsNotNull(model);

			String xml = Process(builder, model);

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.VersionedClass, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"VersionedClass\" lazy=\"false\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <version name=\"Ver\" access=\"property\" column=\"Ver\" type=\"String\" />\r\n" +
				"    <property name=\"Name\" access=\"property\" type=\"String\">\r\n" +
				"      <column name=\"Name\"/>\r\n" +
				"    </property>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void TimestampedClass()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(TimestampedClass));
			Assert.IsNotNull(model);

			String xml = Process(builder, model);

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.TimestampedClass, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"TimestampedClass\" lazy=\"false\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <timestamp name=\"Ts\" access=\"property\" column=\"Ts\" />\r\n" +
				"    <property name=\"Name\" access=\"property\" type=\"String\">\r\n" +
				"      <column name=\"Name\"/>\r\n" +
				"    </property>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void SequenceParamClass()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(SequenceParamClass));
			Assert.IsNotNull(model);

			String xml = Process(builder, model);

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.SequenceParamClass, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"SequenceParamClass\" lazy=\"false\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
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
			ActiveRecordModel model = builder.Create(typeof(BelongsToClassA));
			Assert.IsNotNull(model);

			String xml = Process(builder, model);

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.BelongsToClassA, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"BelongsToClassA\" lazy=\"false\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <many-to-one name=\"ClassA\" access=\"property\" class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassA, Castle.ActiveRecord.Framework.Internal.Tests\" column=\"classa_id\" foreign-key=\"FK_FOREIGN_KEY_A\" lazy=\"proxy\" />\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void BelongsTo2()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(BelongsToClassA2));
			Assert.IsNotNull(model);

			String xml = Process(builder, model);

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.BelongsToClassA2, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"BelongsToClassA2\" lazy=\"false\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <many-to-one name=\"ClassA\" access=\"property\" class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassA, Castle.ActiveRecord.Framework.Internal.Tests\" column=\"classa_id\" insert=\"false\" update=\"false\" not-null=\"true\" unique=\"true\" foreign-key=\"FK_FOREIGN_KEY_B\" cascade=\"save-update\" lazy=\"proxy\" />\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void BelongsToWithFetch()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(BelongsToClassAFetch));
			Assert.IsNotNull(model);

			String xml = Process(builder, model);

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.BelongsToClassAFetch, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"BelongsToClassAFetch\" lazy=\"false\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <many-to-one name=\"ClassA\" access=\"property\" class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassA, Castle.ActiveRecord.Framework.Internal.Tests\" column=\"classa_id\" fetch=\"join\" lazy=\"proxy\" />\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void BelongsToWithLazy()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(BelongsToWithLazy));
			Assert.IsNotNull(model);

			String xml = Process(builder, model);

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.BelongsToWithLazy, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"BelongsToWithLazy\" lazy=\"false\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <many-to-one name=\"ClassA\" access=\"property\" class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassA, Castle.ActiveRecord.Framework.Internal.Tests\" column=\"classa_id\" lazy=\"false\" />\r\n" +
				"    <many-to-one name=\"ClassA2\" access=\"property\" class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassA, Castle.ActiveRecord.Framework.Internal.Tests\" column=\"classa2_id\" lazy=\"proxy\" />\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void HasManyWithExplicitInfo()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(HasManyClassA));
			Assert.IsNotNull(model);

			String xml = Process(builder, model);

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.HasManyClassA, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"HasManyClassA\" lazy=\"false\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <bag name=\"Items\" access=\"property\" table=\"ClassATable\" lazy=\"false\" inverse=\"true\">\r\n" +
				"      <key column=\"keycol\" />\r\n" +
				"      <one-to-many class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassA, Castle.ActiveRecord.Framework.Internal.Tests\" />\r\n" +
				"    </bag>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void HasManyWithFetch()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(HasManyWithFetch));
			Assert.IsNotNull(model);

			String xml = Process(builder, model);

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.HasManyWithFetch, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"HasManyWithFetch\" lazy=\"false\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <bag name=\"Items\" access=\"property\" table=\"ClassATable\" lazy=\"false\" inverse=\"true\" fetch=\"join\">\r\n" +
				"      <key column=\"keycol\" />\r\n" +
				"      <one-to-many class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassA, Castle.ActiveRecord.Framework.Internal.Tests\" />\r\n" +
				"    </bag>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void HasManyWithBatch()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(HasManyWithBatch));
			Assert.IsNotNull(model);

			String xml = Process(builder, model);

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.HasManyWithBatch, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"HasManyWithBatch\" lazy=\"false\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <bag name=\"Items\" access=\"property\" table=\"ClassATable\" lazy=\"false\" inverse=\"true\" batch-size=\"3\">\r\n" +
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
			ActiveRecordModel model = builder.Create(typeof(Category));
			Assert.IsNotNull(model);

			String xml = Process(builder, model);

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.Category, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"Category\" lazy=\"false\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <property name=\"Name\" access=\"property\" type=\"String\">\r\n" +
				"      <column name=\"Name\"/>\r\n" +
				"    </property>\r\n" +
				"    <many-to-one name=\"Parent\" access=\"property\" class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.Category, Castle.ActiveRecord.Framework.Internal.Tests\" column=\"parent_id\" lazy=\"proxy\" />\r\n" +
				"    <bag name=\"SubCategories\" access=\"property\" table=\"Category\" lazy=\"false\">\r\n" +
				"      <key column=\"parent_id\" />\r\n" +
				"      <one-to-many class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.Category, Castle.ActiveRecord.Framework.Internal.Tests\" />\r\n" +
				"    </bag>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void CompositeKey()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(ClassWithCompositeKey));
			Assert.IsNotNull(model);

			string xml = Process(builder, model);

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassWithCompositeKey, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"ClassWithCompositeKey\" lazy=\"false\">\r\n" +
				"    <composite-id name=\"Key\" class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.CompositeKeyForClassWithCompositeKey, Castle.ActiveRecord.Framework.Internal.Tests\" unsaved-value=\"none\" access=\"property\">\r\n" +
				"      <key-property name=\"KeyA\" access=\"property\" column=\"KeyA\" type=\"String\" />\r\n" +
				"      <key-property name=\"KeyB\" access=\"property\" column=\"KeyB\" type=\"String\" />\r\n" +
				"    </composite-id>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void One2OneKey()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel empModel = builder.Create(typeof(Employee));
			ActiveRecordModel awardModel = builder.Create(typeof(Award));
			Assert.IsNotNull(empModel);
			Assert.IsNotNull(awardModel);

			string xml = Process(builder, empModel);

			string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.Employee, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"Employee\" lazy=\"false\">\r\n" +
				"    <id name=\"ID\" access=\"property\" column=\"EmployeeID\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <property name=\"FirstName\" access=\"property\" type=\"String\">\r\n" +
				"      <column name=\"FirstName\"/>\r\n" +
				"    </property>\r\n" +
				"    <property name=\"LastName\" access=\"property\" type=\"String\">\r\n" +
				"      <column name=\"LastName\"/>\r\n" +
				"    </property>\r\n" +
				"    <one-to-one name=\"Award\" access=\"property\" class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.Award, Castle.ActiveRecord.Framework.Internal.Tests\" foreign-key=\"FK_FOREIGN_KEY\" fetch=\"join\" constrained=\"true\" />\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);

			xml = Process(builder, awardModel);

			expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.Award, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"Award\" lazy=\"false\">\r\n" +
				"    <id name=\"ID\" access=\"property\" column=\"EmployeeID\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"foreign\">\r\n" +
				"        <param name=\"property\">Employee</param>\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <property name=\"Description\" access=\"property\" type=\"String\">\r\n" +
				"      <column name=\"Description\"/>\r\n" +
				"    </property>\r\n" +
				"    <one-to-one name=\"Employee\" access=\"property\" class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.Employee, Castle.ActiveRecord.Framework.Internal.Tests\" />\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void CachedClass()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(CacheClass));
			Assert.IsNotNull(model);

			String xml = Process(builder, model);

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.CacheClass, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"CacheClass\" lazy=\"false\">\r\n" +
				"    <cache region=\"Region1\" usage=\"read-write\" />\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <property name=\"Name\" access=\"property\" type=\"String\">\r\n" +
				"      <column name=\"Name\"/>\r\n" +
				"    </property>\r\n" +
				"    <many-to-one name=\"Parent\" access=\"property\" class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.CacheClass, Castle.ActiveRecord.Framework.Internal.Tests\" column=\"parent_id\" lazy=\"proxy\" />\r\n" +
				"    <bag name=\"SubClasses\" access=\"property\" table=\"CacheClass\" lazy=\"false\">\r\n" +
				"      <cache usage=\"read-write\" />\r\n" +
				"      <key column=\"parent_id\" />\r\n" +
				"      <one-to-many class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.CacheClass, Castle.ActiveRecord.Framework.Internal.Tests\" />\r\n" +
				"    </bag>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void NotFoundBehaviourClass()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel NotFoundBehaviourClassModel = builder.Create(typeof(NotFoundBehaviourClass));
			ActiveRecordModel RelationalFoobarModel = builder.Create(typeof(RelationalFoobar));
			Assert.IsNotNull(NotFoundBehaviourClassModel);
			Assert.IsNotNull(RelationalFoobarModel);

			String xml = Process(builder, NotFoundBehaviourClassModel);

			string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.NotFoundBehaviourClass, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"NotFoundBehaviourClass\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <bag name=\"SubClasses\" access=\"property\" table=\"RelationalFoobarTable\" lazy=\"false\">\r\n" +
				"      <key column=\"keycol\" />\r\n" +
				"      <one-to-many class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.RelationalFoobar, Castle.ActiveRecord.Framework.Internal.Tests\" not-found=\"ignore\" />\r\n" +
				"    </bag>\r\n" +
				"    <bag name=\"ManySubClasses\" access=\"property\" table=\"ManySubClasses\" lazy=\"false\">\r\n" +
				"      <key column=\"id\" />\r\n" +
				"      <many-to-many class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.RelationalFoobar, Castle.ActiveRecord.Framework.Internal.Tests\" column=\"ref_id\" not-found=\"ignore\"/>\r\n" +
				"    </bag>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);

			xml = Process(builder, RelationalFoobarModel);

			expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.RelationalFoobar, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"RelationalFoobar\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <many-to-one name=\"NotFoundBehaviourClass\" access=\"property\" class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.NotFoundBehaviourClass, Castle.ActiveRecord.Framework.Internal.Tests\" column=\"NotFoundBehaviourClass\" lazy=\"proxy\" not-found=\"ignore\" />\r\n" +
				"    <bag name=\"NotFoundBehaviourClassList\" access=\"property\" table=\"ManySubClasses\" lazy=\"false\">\r\n" +
				"      <key column=\"id\" />\r\n" +
				"      <many-to-many class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.NotFoundBehaviourClass, Castle.ActiveRecord.Framework.Internal.Tests\" column=\"ref_id\" not-found=\"ignore\"/>\r\n" +
				"    </bag>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void EnumWithColumnType()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(EnumTestClass));
			Assert.IsNotNull(model);

			string xml = Process(builder, model);

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.EnumTestClass, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"EnumTestClass\" lazy=\"false\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <property name=\"NoColumnType\" access=\"property\">\r\n" +
				"      <column name=\"NoColumnType\"/>\r\n" +
				"    </property>\r\n" +
				"    <property name=\"WithColumnType\" access=\"property\" type=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.GenericEnumStringType`1[[Castle.ActiveRecord.Framework.Internal.Tests.Model.EnumVal, Castle.ActiveRecord.Framework.Internal.Tests]], Castle.ActiveRecord.Framework.Internal.Tests\">\r\n" +
				"      <column name=\"WithColumnType\"/>\r\n" +
				"    </property>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		private string Process(ActiveRecordModelBuilder builder, ActiveRecordModel model)
		{
			GraphConnectorVisitor connectorVisitor = new GraphConnectorVisitor(builder.Models);
			connectorVisitor.VisitNode(model);

			SemanticVerifierVisitor semanticVisitor = new SemanticVerifierVisitor(builder.Models);
			semanticVisitor.VisitNode(model);

			XmlGenerationVisitor xmlVisitor = new XmlGenerationVisitor();
			xmlVisitor.CreateXml(model);

			return xmlVisitor.Xml;
		}

		[Test]
		public void HasManyWithDictionary()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(DictionaryModel));
			Assert.IsNotNull(model);

			string xml = Process(builder, model);

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.DictionaryModel, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"DictionaryModel\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <map name=\"Snippet\" access=\"property\" table=\"DictionaryModel_Snippet\" lazy=\"false\">\r\n" +
				"      <key column=\"id\" />\r\n" +
				"      <index column=\"LangCode\" type=\"String\" />\r\n" +
				"      <element  column=\"Text\"  type=\"System.String, mscorlib\"/>\r\n" +
				"    </map>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void SimpleListOfComponents()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(HasManyDependentObjects));
			Assert.IsNotNull(model);

			String xml = Process(builder, model);

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.HasManyDependentObjects, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"HasManyDependentObjects\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <list name=\"Components\" access=\"property\" table=\"dependent_objects\" lazy=\"false\">\r\n" +
				"      <key column=\"id\" />\r\n" +
				"      <index column=\"pos\" />\r\n" +
				"      <composite-element class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.Component, Castle.ActiveRecord.Framework.Internal.Tests\">\r\n" +
				"        <property name=\"Value\" access=\"property\" type=\"String\">\r\n" +
				"          <column name=\"Value\"/>\r\n" +
				"        </property>\r\n" +
				"      </composite-element>\r\n" +
				"    </list>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void SimpleListOfComponentsWithNestedComponents()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(HasManyDependentObjectsWithNested));
			Assert.IsNotNull(model);

			String xml = Process(builder, model);

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.HasManyDependentObjectsWithNested, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"HasManyDependentObjectsWithNested\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <list name=\"ComponentsWithNested\" access=\"property\" table=\"dependent_objects\" lazy=\"false\">\r\n" +
				"      <key column=\"id\" />\r\n" +
				"      <index column=\"pos\" />\r\n" +
				"      <composite-element class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ComponentWithNested, Castle.ActiveRecord.Framework.Internal.Tests\">\r\n" +
				"        <nested-composite-element name=\"Component\" class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.Component, Castle.ActiveRecord.Framework.Internal.Tests\" access=\"property\">\r\n" +
				"          <property name=\"Value\" access=\"property\" type=\"String\">\r\n" +
				"            <column name=\"Value\"/>\r\n" +
				"          </property>\r\n" +
				"        </nested-composite-element>\r\n" +
				"      </composite-element>\r\n" +
				"    </list>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void SimpleListOfComponentsWithNestedComponents2LevelsDeep()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(HasManyDependentObjectsWithNested2LevelsDeep));
			Assert.IsNotNull(model);

			String xml = Process(builder, model);

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.HasManyDependentObjectsWithNested2LevelsDeep, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"HasManyDependentObjectsWithNested2LevelsDeep\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <list name=\"ComponentsWithNested2LevelsDeep\" access=\"property\" table=\"dependent_objects\" lazy=\"false\">\r\n" +
				"      <key column=\"id\" />\r\n" +
				"      <index column=\"pos\" />\r\n" +
				"      <composite-element class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ComponentWithNested2LevelsDeep, Castle.ActiveRecord.Framework.Internal.Tests\">\r\n" +
				"        <nested-composite-element name=\"ComponentWithNested\" class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ComponentWithNested, Castle.ActiveRecord.Framework.Internal.Tests\" access=\"property\">\r\n" +
				"          <nested-composite-element name=\"Component\" class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.Component, Castle.ActiveRecord.Framework.Internal.Tests\" access=\"property\">\r\n" +
				"            <property name=\"Value\" access=\"property\" type=\"String\">\r\n" +
				"              <column name=\"Value\"/>\r\n" +
				"            </property>\r\n" +
				"          </nested-composite-element>\r\n" +
				"        </nested-composite-element>\r\n" +
				"      </composite-element>\r\n" +
				"    </list>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void ManyToMayViaComponents()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(HasManyToManyViaComponents));
			Assert.IsNotNull(model);

			String xml = Process(builder, model);
			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.HasManyToManyViaComponents, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"HasManyToManyViaComponents\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <list name=\"Components\" access=\"property\" table=\"components_to_a\" lazy=\"false\">\r\n" +
				"      <key column=\"id\" />\r\n" +
				"      <index column=\"pos\" />\r\n" +
				"      <composite-element class=\"Castle.ActiveRecord.Framework.Internal.Tests.ComponentManyToClassA, Castle.ActiveRecord.Framework.Internal.Tests\">\r\n" +
				"        <property name=\"Value\" access=\"property\" type=\"String\">\r\n" +
				"          <column name=\"Value\"/>\r\n" +
				"        </property>\r\n" +
				"        <many-to-one name=\"A\" access=\"property\" class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassA, Castle.ActiveRecord.Framework.Internal.Tests\" column=\"aId\" lazy=\"proxy\" />\r\n" +
				"      </composite-element>\r\n" +
				"    </list>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";
			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void IdBagPrimitive()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(IdBagPrimitive));
			Assert.IsNotNull(model);

			String xml = Process(builder, model);
			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.IdBagPrimitive, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"IdBagPrimitive\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <idbag name=\"Items\" access=\"property\" table=\"IdToItems\" lazy=\"false\">\r\n" +
				"      <collection-id type=\"Int32\" column=\"col\">\r\n" +
				"        <generator class=\"sequence\">\r\n" + 
				"        </generator>\r\n" +
				"      </collection-id>\r\n" +
				"      <key column=\"keyid\" />\r\n" +
				"      <element  type=\"System.String\" />\r\n" + 
				"    </idbag>\r\n" + 
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";
			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void SimpleClassWithOverride()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(SimpleClass));
			Assert.IsNotNull(model);

			String xml = Process(builder, model);

			string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.SimpleClass, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"SimpleClass\" lazy=\"false\">\r\n" +
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

			model = builder.Create(typeof(SimpleClassOverride));
			Assert.IsNotNull(model);

			xml = Process(builder, model);

			expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.SimpleClassOverride, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"SimpleClassOverride\" lazy=\"false\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"assigned\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"    <property name=\"Name\" access=\"property\" type=\"String\">\r\n" +
				"      <column name=\"Name\" length=\"50\"/>\r\n" +
				"    </property>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void SimpleCaseWithTuplizer()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(ClassWithTuplizer));
			Assert.IsNotNull(model);

			SemanticVerifierVisitor semanticVisitor = new SemanticVerifierVisitor(builder.Models);
			semanticVisitor.VisitNode(model);

			XmlGenerationVisitor xmlVisitor = new XmlGenerationVisitor();
			xmlVisitor.CreateXml(model);

			String xml = xmlVisitor.Xml;

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassWithTuplizer, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"ClassWithTuplizer\">\r\n" +
				"    <tuplizer class=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.Tuplizer, Castle.ActiveRecord.Framework.Internal.Tests\" entity-mode=\"poco\" />\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}

		[Test]
		public void SimpleCaseSchemaAction()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(ClassWithSchemaAction));
			Assert.IsNotNull(model);

			SemanticVerifierVisitor semanticVisitor = new SemanticVerifierVisitor(builder.Models);
			semanticVisitor.VisitNode(model);

			XmlGenerationVisitor xmlVisitor = new XmlGenerationVisitor();
			xmlVisitor.CreateXml(model);

			String xml = xmlVisitor.Xml;

			const string expected =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
				"<hibernate-mapping  auto-import=\"true\" default-lazy=\"false\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">\r\n" +
				"  <class name=\"Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassWithSchemaAction, Castle.ActiveRecord.Framework.Internal.Tests\" table=\"ClassWithSchemaAction\" schema-action=\"none\">\r\n" +
				"    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\" unsaved-value=\"0\">\r\n" +
				"      <generator class=\"native\">\r\n" +
				"      </generator>\r\n" +
				"    </id>\r\n" +
				"  </class>\r\n" +
				"</hibernate-mapping>\r\n";

			Assert.AreEqual(expected, xml);
		}
	}
}
