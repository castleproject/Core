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
	using NUnit.Framework;

	using Castle.ActiveRecord.Tests.Model;

	[TestFixture]
	public class JoinedSubClassTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void Entities()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), 
				typeof(Entity), typeof(CompanyEntity), typeof(PersonEntity) );

			Entity.DeleteAll();
			CompanyEntity.DeleteAll();
			PersonEntity.DeleteAll();

			Entity ent = new Entity();
			ent.Name = "MS";
			ent.Save();

			CompanyEntity ce = new CompanyEntity();
			ce.Name = "Keldor";
			ce.CompanyType = 1;
			ce.Save();

			Entity[] ents = Entity.FindAll();
			Assert.AreEqual( 2, ents.Length );

			CompanyEntity[] ces = CompanyEntity.FindAll();
			Assert.AreEqual( 1, ces.Length );

			PersonEntity[] pes = PersonEntity.FindAll();
			Assert.AreEqual( 0, pes.Length );

			Assert.AreEqual( ce.CompId, ces[0].CompId );
			Assert.AreEqual( ce.Name, ces[0].Name );
			Assert.AreEqual( ce.Id, ces[0].Id );
		}
	}
}
