// Copyright 2003-2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.ManagementExtensions.Test
{
	using System;
	using System.IO;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;

	using NUnit.Framework;

	using Castle.ManagementExtensions;

	/// <summary>
	/// Summary description for ManagedObjectNameTestCase.
	/// </summary>
	[TestFixture]
	public class ManagedObjectNameTestCase : Assertion
	{
		[Test]
		public void TestCreation()
		{
			ManagedObjectName name1 = new ManagedObjectName("domain.net");
			AssertEquals( "domain.net", name1.Domain );
			AssertEquals( String.Empty, name1.LiteralProperties );

			ManagedObjectName name2 = new ManagedObjectName("domain.org:name=SomeService,type=aware");
			AssertEquals( "domain.org", name2.Domain );
			AssertEquals( "type=aware,name=SomeService", name2.LiteralProperties );
		}

		[Test]
		public void TestEquality1()
		{
			ManagedObjectName name1 = new ManagedObjectName("domain.net");
			ManagedObjectName name2 = new ManagedObjectName("domain.net");

			Assert( name1.GetHashCode() == name2.GetHashCode() );
			Assert( name1.Equals( name2 ) );

			ManagedObjectName name3 = new ManagedObjectName("domain.org");
			Assert( name1.GetHashCode() != name3.GetHashCode() );
			Assert( !name1.Equals( name3 ) );
		}

		[Test]
		public void TestEquality2()
		{
			ManagedObjectName name1 = new ManagedObjectName("domain.net:name=SomeService,type=aware");
			ManagedObjectName name2 = new ManagedObjectName("domain.net:name=SomeService,type=aware");

			Assert( name1.GetHashCode() == name2.GetHashCode() );
			Assert( name1.Equals( name2 ) );

			ManagedObjectName name3 = new ManagedObjectName("domain.net:name=SomeService,type=unaware");
			Assert( name1.GetHashCode() != name3.GetHashCode() );
			Assert( !name1.Equals( name3 ) );
		}

		[Test]
		public void TestSerialization()
		{
			MemoryStream stream = new MemoryStream();

			ManagedObjectName name1 = new ManagedObjectName("domain.net:name=SomeService,type=aware");

			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, name1);

			stream.Position = 0;

			ManagedObjectName name2 = (ManagedObjectName) formatter.Deserialize(stream);

			Assert( name1.GetHashCode() == name2.GetHashCode() );
			Assert( name1.Equals( name2 ) );
		}
	}
}
