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

namespace Castle.ActiveRecord.Tests
{
    using System;
    using System.Text;
    using NUnit.Framework;
    using Castle.ActiveRecord.Tests.Model.RulesModel;
    using Castle.ActiveRecord.Framework.Internal;

    [TestFixture]
    public class ActiveRecordWithoutBaseTestCase : AbstractActiveRecordTest
    {
        [SetUp]
        public void Setup()
        {
            ActiveRecordStarter.Initialize(GetConfigSource(),
                typeof(PersistedRule),
                typeof(WorkDaysRules));
            Recreate();
        }

        [Test]
        public void SimpleOperations()
        {
            RuleBase[] rules = (RuleBase[])DomainModel.FindAll(typeof(PersistedRule));

            Assert.IsNotNull(rules);
            Assert.AreEqual(0, rules.Length);

            WorkDaysRules rule = new WorkDaysRules();
            rule.Count = 5;
            rule.Days = 4;
            rule.Name = "blah";

            DomainModel.Save(rule);

            Assert.IsFalse(0 == rule.Id);

            rules = (PersistedRule[])DomainModel.FindAll(typeof(PersistedRule));

            Assert.IsNotNull(rules);
            Assert.AreEqual(1, rules.Length);

            WorkDaysRules retrieved = (WorkDaysRules)DomainModel.FindByPrimaryKey(typeof(WorkDaysRules), rule.Id);
            Assert.IsNotNull(retrieved);

            Assert.AreEqual(rule.Name, retrieved.Name);
            Assert.AreEqual(rule.Count, retrieved.Count);
            Assert.AreEqual(rule.Days, retrieved.Days);
        }

        [Test]
        public void DiscriminatorUse()
        {
            XmlGenerationVisitor xmlVisitor = new XmlGenerationVisitor();
            xmlVisitor.CreateXml(DomainModel.GetModel(typeof(PersistedRule)));
            String xml = xmlVisitor.Xml;

            String expected =
                "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n"+
                "<hibernate-mapping xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.0\">\r\n"+
                "  <class name=\"Castle.ActiveRecord.Tests.Model.RulesModel.PersistedRule, Castle.ActiveRecord.Tests\" table=\"PersistedRule\" discriminator-value=\"0\"   >\r\n"+
                "    <id name=\"Id\" access=\"property\" column=\"Id\" type=\"Int32\"  unsaved-value=\"0\">\r\n"+
                "      <generator class=\"native\">\r\n"+
                "      </generator>\r\n"+
                "    </id>\r\n"+
                "    <discriminator column=\"discriminator\"  />\r\n"+
                "    <property name=\"Count\" access=\"property\" column=\"Count\" type=\"Int32\"        />\r\n"+
                "    <subclass name=\"Castle.ActiveRecord.Tests.Model.RulesModel.WorkDaysRules, Castle.ActiveRecord.Tests\"  discriminator-value=\"2\">\r\n"+
                "      <property name=\"Name\" access=\"property\" column=\"Name\" type=\"System.String\"        />\r\n"+
                "      <property name=\"Days\" access=\"property\" column=\"Days\" type=\"Int32\"        />\r\n"+
                "    </subclass>\r\n"+
                "  </class>\r\n"+
                "</hibernate-mapping>\r\n";;

            Assert.AreEqual(expected, xml);
        }
    }
}
