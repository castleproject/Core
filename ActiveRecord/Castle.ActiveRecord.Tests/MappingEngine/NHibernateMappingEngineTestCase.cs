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

	using NUnit.Framework;

	using Castle.ActiveRecord.Tests.Model;


	[TestFixture]
	public class NHibernateMappingEngineTestCase
	{
		[Test]
		public void SimpleModelNoRelations()
		{
			String contents = "\r\n<hibernate-mapping xmlns=\"urn:nhibernate-mapping-2.0\" >\r\n" + 
				"<class name=\"Castle.ActiveRecord.Tests.Model.SimpleModel, Castle.ActiveRecord.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\" table=\"TableNameHere\" >\r\n" + 
				"<id name=\"id\" type=\"Int32\" column=\"t_id\" >\r\n" + 
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
				"<class name=\"Castle.ActiveRecord.Tests.Model.SimpleModel2, Castle.ActiveRecord.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\" table=\"TableNameHere\" >\r\n" + 
				"<id name=\"id\" type=\"Int32\" column=\"t_id\" >\r\n" + 
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
				"<class name=\"Castle.ActiveRecord.Tests.Model.SimpleModel3, Castle.ActiveRecord.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\" table=\"TableNameHere\" >\r\n" + 
				"<id name=\"id\" type=\"Int32\" column=\"t_id\" >\r\n" + 
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
	}
}
