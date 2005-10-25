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

namespace Castle.ActiveRecord.Framework.Internal.Tests
{
	using System;

	using NUnit.Framework;

	using Castle.ActiveRecord.Framework.Internal.Tests.Model;
	using System.Reflection;


	[TestFixture]
	public class SemanticCheckTestCase : AbstractActiveRecordTest
	{
		[Test]
		[ExpectedException( typeof(ActiveRecordException), "Unfortunatelly you can't have a discriminator class and a joined subclass at the same time - check type Castle.ActiveRecord.Framework.Internal.Tests.Model.Company" )]
		public void JoinedAndDiscriminatorClass()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Company) );
		}

		[Test]
		[ExpectedException( typeof(ActiveRecordException) )]
		public void VersionedTimestampedClass()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(VersionedTimestampedClass) );
		}

		[Test]
		[ExpectedException( typeof(ActiveRecordException), "A type must declare a primary key. Check type Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassWithoutPrimaryKey" )]
		public void ClassWithoutPrimaryKey()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(ClassWithoutPrimaryKey) );
		}

        [Test]
        [ExpectedException(typeof(ActiveRecordException), "You can't use [Property] on ClassWithBadMapping.ClassA because Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassA is an active record class, did you mean to use BelongTo?")]
        public void ClassThatMapAnotherActiveRecordClassAsAPropertyInsteadOfBelongsTo()
        {
            ActiveRecordStarter.Initialize(GetConfigSource(), typeof(ClassWithBadMapping), typeof(ClassA));
        }
	}
}
