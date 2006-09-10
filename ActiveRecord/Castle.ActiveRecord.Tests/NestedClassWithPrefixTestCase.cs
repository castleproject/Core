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

namespace Castle.ActiveRecord.Tests
{
	using System;
	using System.IO;
	using System.Text.RegularExpressions;
	using NUnit.Framework;

	using Castle.ActiveRecord.Tests.Model.Nested;
	
	[TestFixture]
	public class NestedClassWithPrefixTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void WithPrefix()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(NestedWithPrefix));
			Recreate();

			String fileName = null;
			try
			{
				fileName = Path.GetTempFileName();
				ActiveRecordStarter.GenerateCreationScripts(fileName);
				using (TextReader r = new StreamReader(fileName))
				{
					string schema = r.ReadToEnd();
					Assert.IsTrue(Regex.IsMatch(schema, @"\bfirst\b", RegexOptions.IgnoreCase));
					Assert.IsTrue(Regex.IsMatch(schema, @"\blast\b", RegexOptions.IgnoreCase));
					Assert.IsTrue(Regex.IsMatch(schema, @"\bothername_first\b", RegexOptions.IgnoreCase));
					Assert.IsTrue(Regex.IsMatch(schema, @"\bothername_last\b", RegexOptions.IgnoreCase));
				}
			}
			finally
			{
				if (fileName != null)
					File.Delete(fileName);
			}

			NestedWithPrefix.DeleteAll();
			
			NestedWithPrefix nested = new NestedWithPrefix();
			nested.Save();
			
			nested = NestedWithPrefix.Find(nested.Id);
			Assert.AreEqual(1, nested.Id);
			Assert.IsNull(nested.Self);
			Assert.IsNull(nested.Other);

			nested.Self = new Name();
			nested.Self.First = "John";
			nested.Self.Last = "Doe";
			
			nested.Other = new Name();
			nested.Other.First = "Edward";
			nested.Other.Last = "Norton";
			
			nested.Save();
			
			nested = NestedWithPrefix.Find(nested.Id);
			Assert.IsNotNull(nested.Self);
			Assert.IsNotNull(nested.Other);
			Assert.AreEqual("John", nested.Self.First);
			Assert.AreEqual("Doe", nested.Self.Last);
			Assert.AreEqual("Edward", nested.Other.First);
			Assert.AreEqual("Norton", nested.Other.Last);
		}
	}
}
