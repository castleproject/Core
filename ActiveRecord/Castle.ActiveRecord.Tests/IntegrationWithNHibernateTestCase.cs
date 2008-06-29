// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

[assembly: Castle.ActiveRecord.Tests.RegisterNHibernateClassMapping]
namespace Castle.ActiveRecord.Tests
{
	using Castle.ActiveRecord.Framework.Internal;
	using NUnit.Framework;

	[TestFixture]
	public class IntegrationWithNHibernateTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void CanIntegrateNHibernateAndActiveRecord()
		{
			ActiveRecordStarter.ModelsValidated+=delegate {
				new ActiveRecordModelBuilder().CreateDummyModelFor(typeof (NHibernateClass));
			};
			ActiveRecordStarter.Initialize(
				GetConfigSource(),
				typeof(ActiveRecordClass));

			Recreate();

			using(TransactionScope tx = new TransactionScope())
			{
				ActiveRecordClass ar = new ActiveRecordClass();
				ar.Friend = new NHibernateClass();
				ActiveRecordMediator.Save(ar.Friend);
				ActiveRecordMediator.Save(ar);
				tx.VoteCommit();
			}

			using (TransactionScope tx = new TransactionScope())
			{
				ActiveRecordClass first = ActiveRecordMediator<ActiveRecordClass>.FindFirst();
				Assert.IsNotNull(first);
				Assert.IsNotNull(first.Friend);
			}
		}
	}

	public class NHibernateClass
	{
		private int id;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}
	}

	[ActiveRecord]
	public class ActiveRecordClass
	{
		private int id;
		private NHibernateClass friend;

		[PrimaryKey]
		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		[BelongsTo]
		public virtual NHibernateClass Friend
		{
			get{ return friend; }
			set{ friend = value;}
		}
	}

	public class RegisterNHibernateClassMapping : RawXmlMappingAttribute
	{
		public override string[] GetMappings()
		{
			return new string[]
			{
				@"<hibernate-mapping  xmlns='urn:nhibernate-mapping-2.2'>
	<class name='Castle.ActiveRecord.Tests.NHibernateClass, Castle.ActiveRecord.Tests'>
		<id name='Id'>
			<generator class='native'/>
		</id>
	</class>
</hibernate-mapping>"
		};
		}
	}
}