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
#if DOTNET2

namespace Castle.ActiveRecord.Tests
{
	using Castle.ActiveRecord.Tests.Model;
	using NUnit.Framework;
	using CompanyEntity=Castle.ActiveRecord.Tests.Model.GenericModel.CompanyEntity;
	using PersonEntity=Castle.ActiveRecord.Tests.Model.GenericModel.PersonEntity;

	[TestFixture]
	public class GenericJoinedSubClassTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void Entities()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(),
				typeof(Model.GenericModel.Entity<>), 
				typeof(CompanyEntity), 
				typeof(PersonEntity));
			Recreate();

			CompanyEntity.DeleteAll();
			PersonEntity.DeleteAll();

			CompanyEntity ce = new CompanyEntity();
			ce.Name = "Keldor";
			ce.CompanyType = 1;
			ce.Save();

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

#endif