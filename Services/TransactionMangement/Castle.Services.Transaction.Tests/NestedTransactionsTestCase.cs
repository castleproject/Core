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

namespace Castle.Services.Transaction.Tests
{
	using System;

	using NUnit.Framework;

	[TestFixture]
	public class NestedTransactionsTestCase
	{
		DefaultTransactionManager tm;

		[SetUp]
		public void Init()
		{
			tm = new DefaultTransactionManager();
		}

		[Test]
		public void NestedRequiresWithCommits()
		{
			ITransaction root = tm.CreateTransaction( TransactionMode.Requires, IsolationMode.Unspecified );
			Assert.IsTrue( root is StandardTransaction );
			root.Begin();

			ITransaction child1 = tm.CreateTransaction( TransactionMode.Requires, IsolationMode.Unspecified );
			Assert.IsTrue( child1 is ChildTransaction );
			child1.Begin();
			
			ITransaction child2 = tm.CreateTransaction( TransactionMode.Requires, IsolationMode.Unspecified );
			Assert.IsTrue( child2 is ChildTransaction );
			child2.Begin();

			child2.Commit();
			child1.Commit();
			root.Commit();
		}

		[Test]
		public void NestedRequiresAndRequiresNew()
		{
			ITransaction root = tm.CreateTransaction( TransactionMode.Requires, IsolationMode.Unspecified );
			Assert.IsTrue( root is StandardTransaction );
			root.Begin();

			ITransaction child1 = tm.CreateTransaction( TransactionMode.Requires, IsolationMode.Unspecified );
			Assert.IsTrue( child1 is ChildTransaction );
			child1.Begin();
			
			ITransaction innerRoot = tm.CreateTransaction( TransactionMode.RequiresNew, IsolationMode.Unspecified );
			Assert.IsFalse( innerRoot is ChildTransaction );
			innerRoot.Begin();

			ITransaction child2 = tm.CreateTransaction( TransactionMode.Requires, IsolationMode.Unspecified );
			Assert.IsTrue( child2 is ChildTransaction );
			child2.Begin();

			child2.Commit();
			innerRoot.Commit();

			child1.Commit();
			root.Commit();
		}

		[Test]
		public void SameResources()
		{
			ResourceImpl resource = new ResourceImpl();

			ITransaction root = tm.CreateTransaction( TransactionMode.Requires, IsolationMode.Unspecified );
			root.Begin();
			root.Enlist(resource);

			ITransaction child1 = tm.CreateTransaction( TransactionMode.Requires, IsolationMode.Unspecified );
			Assert.IsTrue( child1 is ChildTransaction );
			child1.Enlist(resource);
			child1.Begin();
			
			child1.Commit();
			root.Commit();
		}

		[Test]
		public void NotSupportedAndNoActiveTransaction()
		{
			ITransaction root = tm.CreateTransaction( TransactionMode.NotSupported, IsolationMode.Unspecified );
			Assert.IsNull( root );
		}

		[Test]
		[ExpectedException( typeof(TransactionException) )]
		public void NotSupportedAndActiveTransaction()
		{
			ITransaction root = tm.CreateTransaction( TransactionMode.Requires, IsolationMode.Unspecified );
			root.Begin();

			tm.CreateTransaction( TransactionMode.NotSupported, IsolationMode.Unspecified );
		}

		[Test]
		[ExpectedException( typeof(TransactionException) )]
		public void NestedRollback()
		{
			ITransaction root = tm.CreateTransaction( TransactionMode.Requires, IsolationMode.Unspecified );
			root.Begin();

			ITransaction child1 = tm.CreateTransaction( TransactionMode.Requires, IsolationMode.Unspecified );
			child1.Begin();
			
			ITransaction child2 = tm.CreateTransaction( TransactionMode.Requires, IsolationMode.Unspecified );
			child2.Begin();

			child2.Rollback();
			child1.Commit();
			root.Commit(); // Can't perform
		}
	}
}
