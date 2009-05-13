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
	using Castle.ActiveRecord.Tests.Model;
	using NUnit.Framework;

	[TestFixture]
	public class JoinedSubClassWithDiscriminatorTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void Animals()
		{
			Assert.IsTrue(NHibernate.Cfg.Environment.UseReflectionOptimizer);
			ActiveRecordStarter.Initialize(GetConfigSource(),
				typeof(Animal<>),
				typeof(Cat),
				typeof(Dog));
			Assert.IsFalse(NHibernate.Cfg.Environment.UseReflectionOptimizer);
			Recreate();

			Cat.DeleteAll();
			Dog.DeleteAll();

			Cat cat = new Cat();
			cat.Name = "Alfred";
			cat.Breed = "Lion";
			cat.Save();

			Cat[] cats = Cat.FindAll();
			Assert.AreEqual(1, cats.Length);

			Dog[] dogs = Dog.FindAll();
			Assert.AreEqual(0, dogs.Length);

			Assert.AreEqual(cat.AnimalId, cats[0].AnimalId);
			Assert.AreEqual(cat.Name, cats[0].Name);
			Assert.AreEqual(cat.Breed, cats[0].Breed);
		}
	}
}
