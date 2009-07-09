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

using System;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Queries;
using Castle.ActiveRecord.Tests.Model;
using NHibernate;
using NUnit.Framework;

namespace Castle.ActiveRecord.Tests
{
	[TestFixture]
	public class SimpleQueryWithParameterTypesTestCase : AbstractActiveRecordTest
	{
		static readonly Guid Key1 = new Guid("1" + Guid.NewGuid().ToString().Remove(0, 1));
		static readonly Guid Key2 = new Guid("2" + Guid.NewGuid().ToString().Remove(0, 1));
		const string Query = "select p from ProductWithGuid p where p.Key LIKE ?"; 

		[SetUp]
		public new void Init()
		{
			ActiveRecordStarter.ResetInitializationFlag();
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(ProductWithGuid));

			Recreate();

			var product1 = new ProductWithGuid() { Key = Key1 };
			product1.Save();

			var product2 = new ProductWithGuid() { Key = Key2 };
			product2.Save();
		}

		[Test]
		public void TestSetup()
		{
			Assert.That(ProductWithGuid.FindAll().Length, Is.EqualTo(2));
		}

		[Test]
		public void TestExceptionThrownWhenNoOverrideProvided()
		{
			var exception = Assert.Throws<ActiveRecordException>(() => new SimpleQuery<ProductWithGuid>(Query, Key1.ToString()[0] + "%").Execute());
			Assert.That(exception.InnerException, Is.Not.Null);
			Assert.That(exception.InnerException, Is.TypeOf<ADOException>());
			Assert.That(exception.InnerException.InnerException, Is.TypeOf<InvalidCastException>());
		}

		[Test]
		public void TestExceptionNotThrownWhenOverrideProvided()
		{
			var results = new SimpleQuery<ProductWithGuid>(Query, new ValueAndTypeTuple(NHibernateUtil.String, Key1.ToString()[0] + "%")).Execute();
			Assert.That(results.Length, Is.EqualTo(1));
		}
	}
}
