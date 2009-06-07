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

using Castle.ActiveRecord.Framework.Config;
using Castle.ActiveRecord.Tests.Model;
using NHibernate;
using NHibernate.Metadata;

namespace Castle.ActiveRecord.Framework.Internal.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class AssemblyXmlGenerationTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void GenerateHqlQueryAndImportsFromAssembly()
		{
			AssemblyXmlGenerator generator = new AssemblyXmlGenerator();
			string[] xmlConfigurations = generator.CreateXmlConfigurations(typeof(AssemblyXmlGenerationTestCase).Assembly);
			string actual = xmlConfigurations[0];
			string expected = "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
	"<hibernate-mapping  xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"" +
	" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">" +
	"<import class=\"Castle.ActiveRecord.Framework.Internal.Tests.ImportClassRow, Castle.ActiveRecord.Framework.Internal.Tests\" rename=\"ImportClassRow\"/>\r\n" +
	"	<query name='allAdultUsers'>\r\n" +
	"		 <![CDATA[from User user where user.Age > 21]]>\r\n" +
	"	 </query>\r\n" +
	"\r\n" +
	"	<sql-query name='allAdultUsersSql'>\r\n" +
	"		 <![CDATA[select * from Users where Age > 21]]>\r\n"+
	"	 </sql-query>\r\n"+
	"</hibernate-mapping>\r\n" ;
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void CanGetCustomValueFromRawXmlDerivedAttribute()
		{
			string expected =
@"<hibernate-mapping xmlns='urn:nhibernate-mapping-2.2'>
	<import class='Castle.ActiveRecord.Framework.Internal.Tests.ImportClassRow2, Castle.ActiveRecord.Framework.Internal.Tests' rename='ImportClassRow2'/>
</hibernate-mapping>";
			AssemblyXmlGenerator generator = new AssemblyXmlGenerator();
			string[] xmlConfigurations = generator.CreateXmlConfigurations(typeof(AssemblyXmlGenerationTestCase).Assembly);
			string actual = xmlConfigurations[1];
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void WillUseRegisteredAssembliesToLookForRawMappingXmlEvenIfThereAreNoActiveRecordTypesInThatAssembly()
		{
			ActiveRecordStarter.ResetInitializationFlag();
			ActiveRecordStarter.Initialize(
				typeof(RegisterNHibernateClassMapping).Assembly,
				GetConfigSource()
				);
			ISessionFactory factory = ActiveRecordMediator.GetSessionFactoryHolder()
				.GetSessionFactory(typeof(ActiveRecordBase));
			IClassMetadata metadata = factory
				.GetClassMetadata(typeof(NHibernateClass));
			Assert.IsNotNull(metadata);
		}
	}
}
