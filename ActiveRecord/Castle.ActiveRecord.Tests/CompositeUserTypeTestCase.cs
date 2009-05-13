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
	using System.IO;
	using System.Text.RegularExpressions;
	using Castle.ActiveRecord.Tests.Model.CompositeUserType;
	using NUnit.Framework;

	[TestFixture]
	public class CompositeUserTypeTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void CompositeUserTypeConfig()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Citizen), typeof(SecondCitizen));

			Recreate();

			Citizen c = new Citizen();

			c.Name = new string[] {"Jonh", "Doe"};
			c.ManufacturerName = new string[] {"Acme", "Inc"};
			c.InventorsName = new string[] {"Emmet", "Brown"};
			c.SellersName = new string[] { "Big", "Tex"};

			c.Create();

			Citizen loaded = Citizen.Find(c.Id);

			Assert.IsNotNull(loaded);
			Assert.AreEqual("Jonh", loaded.Name[0]);
			Assert.AreEqual("Doe", loaded.Name[1]);
			Assert.AreEqual("Acme", loaded.ManufacturerName[0]);
			Assert.AreEqual("Emmet", loaded.InventorsName[0]);
			Assert.AreEqual("Brown", loaded.InventorsName[1]);
			Assert.AreEqual("Big", loaded.SellersName[0]);
			Assert.AreEqual("Tex", loaded.SellersName[1]);
		}

		[Test]
		public void CompositeUserTypeNested()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Citizen), typeof(NestedCitizen));
			Recreate();

			String fileName = null;
			try
			{
				fileName = Path.GetTempFileName();
				ActiveRecordStarter.GenerateCreationScripts(fileName);
				using (TextReader r = new StreamReader(fileName))
				{
					string schema = r.ReadToEnd();
					Assert.IsTrue(Regex.IsMatch(schema, @"\bfirstname\b", RegexOptions.IgnoreCase));
					Assert.IsTrue(Regex.IsMatch(schema, @"\blastname\b", RegexOptions.IgnoreCase));
					Assert.IsTrue(Regex.IsMatch(schema, @"\bmanufacturer_firstname\b", RegexOptions.IgnoreCase));
					Assert.IsTrue(Regex.IsMatch(schema, @"\bmanufacturer_lastname\b", RegexOptions.IgnoreCase));
				}
			}
			finally
			{
				if (fileName != null)
					File.Delete(fileName);
			}

			NestedCitizen c = new NestedCitizen();

			c.Name.Name = new string[] { "Jonh", "Doe" };
			c.ManufacturerName.Name = new string[] { "Acme", "Inc" };

			c.Create();

			NestedCitizen loaded = NestedCitizen.Find(c.Id);

			Assert.IsNotNull(loaded);
			Assert.AreEqual("Jonh", loaded.Name.Name[0]);
			Assert.AreEqual("Doe", loaded.Name.Name[1]);
			Assert.AreEqual("Acme", loaded.ManufacturerName.Name[0]);
			Assert.AreEqual("Inc", loaded.ManufacturerName.Name[1]);
		}

		[Test]
		public void CompositeUserTypeWithAccess()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Product));
			Recreate();

			Product product = new Product(new string[]{"John", "Doe"}, new string[]{"Acme", "Inc"});

			product.Create();
			
			Product loaded = Product.Find(product.Id);

			Assert.IsNotNull(loaded);
			Assert.AreEqual("John", loaded.Name[0]);
			Assert.AreEqual("Doe", loaded.Name[1]);
			Assert.AreEqual("Acme", loaded.ManufacturerName[0]);
			Assert.AreEqual("Inc", loaded.ManufacturerName[1]);
		
		}
	}
}
