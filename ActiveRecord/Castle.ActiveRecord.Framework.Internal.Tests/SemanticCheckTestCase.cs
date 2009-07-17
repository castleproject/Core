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

namespace Castle.ActiveRecord.Framework.Internal.Tests
{
	using NUnit.Framework;

	using Castle.ActiveRecord.Framework.Internal.Tests.Model;


	[TestFixture]
	public class SemanticCheckTestCase : AbstractActiveRecordTest
	{
		[Test,Ignore("meta-type is optional")]
		[ExpectedException(typeof(ActiveRecordException), ExpectedMessage = "MetaType is a required attribute of AnyAttribute on Castle.ActiveRecord.Framework.Internal.Tests.Model.BadClassWithAnyAttribute.PaymentMethod.")]
		public void UsingAnyWithoutSpecifyingTheMetaType()
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();
			ActiveRecordModel model = builder.Create(typeof(BadClassWithAnyAttribute));
			Assert.IsNotNull(model);

			Assert.IsNotNull(model);

			SemanticVerifierVisitor semanticVisitor = new SemanticVerifierVisitor(builder.Models);
			semanticVisitor.VisitNode(model);
		}

		[Test]
		[ExpectedException( typeof(ActiveRecordException) )]
		public void VersionedTimestampedClass()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(VersionedTimestampedClass) );
		}

		[Test]
		[ExpectedException(typeof(ActiveRecordException), ExpectedMessage = "A type must declare a primary key. Check type Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassWithoutPrimaryKey")]
		public void ClassWithoutPrimaryKey()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(ClassWithoutPrimaryKey) );
		}

        [Test]
		[ExpectedException(typeof(ActiveRecordException), ExpectedMessage = "Property Clazz must be virtual because class LazyClassWithoutVirtualPropertyOnBelongsTo support lazy loading [ActiveRecord(Lazy=true)]")]
        public void LazyClassWithoutVirtualPropertyOnBelongsTo()
        {
        	ActiveRecordStarter.Initialize(GetConfigSource(), typeof (LazyClassWithoutVirtualPropertyOnBelongsTo));
		}

		[Test]
        [ExpectedException(typeof(ActiveRecordException), ExpectedMessage="You can't use [Property] on ClassWithBadMapping.ClassA because Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassA is an active record class, did you mean to use BelongTo?")]
        public void ClassThatMapAnotherActiveRecordClassAsAPropertyInsteadOfBelongsTo()
        {
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(ClassWithBadMapping), typeof(ClassA));
        }

		[Test]
		[ExpectedException(typeof(ActiveRecordException), ExpectedMessage="To use type 'BadCompositeKey' as a composite id, you must implement Equals and GetHashCode.")]
		public void ClassWithBadCompositeKey()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(ClassWithBadCompositeKey));
		}

		[Test]
		[ExpectedException(typeof(ActiveRecordException), ExpectedMessage="You can't specify a PrimaryKeyAttribute in a joined subclass. Check type Castle.ActiveRecord.Framework.Internal.Tests.Model.JoinedSubClassWithPrimaryKey")]
		public void JoinedClassWithPrimaryKey()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(BaseJoinedClass), typeof(JoinedSubClassWithPrimaryKey));
		}
	
		[Test]
		[ExpectedException(typeof(ActiveRecordException), ExpectedMessage="You can't specify more than one PrimaryKeyAttribute in a class. Check type Castle.ActiveRecord.Framework.Internal.Tests.Model.ClassWithMultiplePrimaryKeys")]
		public void ClassWithMultiplePrimaryKeys()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(BaseJoinedClass), typeof(ClassWithMultiplePrimaryKeys));
		}

		[Test]
		public void ListWithoutIndexColumn()
		{
			Assert.Throws<ActiveRecordException>(() => { ActiveRecordStarter.Initialize(GetConfigSource(), typeof(HasManyWithBadList)); });
		}

		[Test]
		public void ListWithoutIndexColumnManyToMany()
		{
			Assert.Throws<ActiveRecordException>(() => { ActiveRecordStarter.Initialize(GetConfigSource(), typeof(HasAndBelongsToManyWithBadList)); });
		}

		[Test]
		public void ListWithoutIndexColumnManyToAny()
		{
			Assert.Throws<ActiveRecordException>(() => { ActiveRecordStarter.Initialize(GetConfigSource(), typeof(HasManyToAnyWithBadList)); });
		}
	}
}
