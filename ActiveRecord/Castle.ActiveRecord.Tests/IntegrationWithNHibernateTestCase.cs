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

using System.Reflection;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Tests.Model;
using NHibernate.Cfg;

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
			ActiveRecordStarter.ModelsValidated += delegate
			{
				new ActiveRecordModelBuilder().CreateDummyModelFor(typeof(NHibernateClass));
			};
			ActiveRecordStarter.Initialize(
				GetConfigSource(),
				typeof(ActiveRecordClass),
				typeof(NHibernateClass));

			Recreate();

			using (TransactionScope tx = new TransactionScope())
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

		[Test]
		public void WhenMappingRegisteredInConfigurationCalledTheConfigurationHasClasses()
		{
			ActiveRecordStarter.ModelsValidated += delegate
			   {
				   new ActiveRecordModelBuilder().CreateDummyModelFor(typeof(NHibernateClass));
			   };
			Configuration configuration = null;
			ActiveRecordStarter.MappingRegisteredInConfiguration += delegate(ISessionFactoryHolder holder)
			{
				configuration = holder.GetAllConfigurations()[0];
			};
			ActiveRecordStarter.Initialize(
				GetConfigSource(),
				typeof(ActiveRecordClass),
				typeof(NHibernateClass));
			Assert.AreNotEqual(0, configuration.ClassMappings.Count);
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
			get { return friend; }
			set { friend = value; }
		}
	}
}