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

namespace Castle.MonoRail.Framework.Tests.Helpers
{
	using System;
	using System.Collections;
#if DOTNET2
	using System.Collections.Generic;
#endif
	using NUnit.Framework;

	using Castle.MonoRail.Framework.Helpers;

	[TestFixture]
	public class PaginationHelperTestCase
	{
		private PaginationHelper helper;

		[SetUp]
		public void Init()
		{
			helper = new PaginationHelper();
		}

		[Test]
		public void EmptyCollection()
		{
			IList items = new ArrayList();

			IPaginatedPage page = PaginationHelper.CreatePagination(items, 10, 1);
			
			Assert.IsNotNull(page);
			
			Assert.AreEqual(0, page.FirstItem);
			Assert.AreEqual(0, page.LastItem);
			Assert.AreEqual(0, page.TotalItems);
			
			Assert.IsFalse(page.HasFirst);
			Assert.IsFalse(page.HasLast);
			Assert.IsFalse(page.HasNext);
			Assert.IsFalse(page.HasPrevious);
		}

		[Test]
		public void LessItemsThanPageSize()
		{
			IList items = new ArrayList();

			items.Add("1");
			items.Add("2");
			items.Add("3");
			items.Add("4");
			items.Add("5");

			IPaginatedPage page = PaginationHelper.CreatePagination(items, 10, 1);
			
			Assert.IsNotNull(page);
			
			Assert.AreEqual(1, page.FirstItem);
			Assert.AreEqual(5, page.LastItem);
			Assert.AreEqual(5, page.TotalItems);
			
			Assert.IsFalse(page.HasFirst);
			Assert.IsFalse(page.HasLast);
			Assert.IsFalse(page.HasNext);
			Assert.IsFalse(page.HasPrevious);
		}

		[Test]
		public void JustOneItem()
		{
			IList items = new ArrayList();

			items.Add("1");

			IPaginatedPage page = PaginationHelper.CreatePagination(items, 10, 1);
			
			Assert.IsNotNull(page);
			
			Assert.AreEqual(1, page.FirstItem);
			Assert.AreEqual(1, page.LastItem);
			Assert.AreEqual(1, page.TotalItems);
			
			Assert.IsFalse(page.HasFirst);
			Assert.IsFalse(page.HasLast);
			Assert.IsFalse(page.HasNext);
			Assert.IsFalse(page.HasPrevious);
		}

		[Test]
		public void ItemsEqualsToPageSize()
		{
			IList items = new ArrayList();

			items.Add("1");
			items.Add("2");
			items.Add("3");

			IPaginatedPage page = PaginationHelper.CreatePagination(items, 3, 1);
			
			Assert.IsNotNull(page);
			
			Assert.AreEqual(1, page.FirstItem);
			Assert.AreEqual(3, page.LastItem);
			Assert.AreEqual(3, page.TotalItems);
			
			Assert.IsFalse(page.HasFirst);
			Assert.IsFalse(page.HasLast);
			Assert.IsFalse(page.HasNext);
			Assert.IsFalse(page.HasPrevious);
		}

		[Test]
		public void ItemsBiggerThanPageSize()
		{
			IList items = new ArrayList();

			for(int i=1; i <= 15; i++)
			{
				items.Add(i);
			}

			// First page

			IPaginatedPage page = PaginationHelper.CreatePagination(items, 5, 1);
			
			Assert.IsNotNull(page);
			
			Assert.AreEqual(1, page.FirstItem);
			Assert.AreEqual(5, page.LastItem);
			Assert.AreEqual(15, page.TotalItems);
			
			Assert.IsFalse(page.HasFirst);
			Assert.IsTrue(page.HasLast);
			Assert.IsTrue(page.HasNext);
			Assert.IsFalse(page.HasPrevious);

			// Second page

			page = PaginationHelper.CreatePagination( items, 5, 2 );
			
			Assert.IsNotNull(page);
			
			Assert.AreEqual(6, page.FirstItem);
			Assert.AreEqual(10, page.LastItem);
			Assert.AreEqual(15, page.TotalItems);
			
			Assert.IsTrue(page.HasFirst);
			Assert.IsTrue(page.HasLast);
			Assert.IsTrue(page.HasNext);
			Assert.IsTrue(page.HasPrevious);

			// Last page

			page = PaginationHelper.CreatePagination( items, 5, 3 );
			
			Assert.IsNotNull(page);
			
			Assert.AreEqual(11, page.FirstItem);
			Assert.AreEqual(15, page.LastItem);
			Assert.AreEqual(15, page.TotalItems);
			
			Assert.IsTrue(page.HasFirst);
			Assert.IsFalse(page.HasLast);
			Assert.IsFalse(page.HasNext);
			Assert.IsTrue(page.HasPrevious);
		}

#if DOTNET2
		[Test]
		public void UsageWithGenerics()
		{
			IList<int> items = new List<int>();

			for (int i = 1; i <= 15; i++)
			{
				items.Add(i);
			}

			// First page

			IPaginatedPage page = PaginationHelper.CreatePagination(items, 5, 1);

			Assert.IsNotNull(page);

			Assert.AreEqual(1, page.FirstItem);
			Assert.AreEqual(5, page.LastItem);
			Assert.AreEqual(15, page.TotalItems);

			Assert.IsFalse(page.HasFirst);
			Assert.IsTrue(page.HasLast);
			Assert.IsTrue(page.HasNext);
			Assert.IsFalse(page.HasPrevious);

			// Second page

			page = PaginationHelper.CreatePagination(items, 5, 2);

			Assert.IsNotNull(page);

			Assert.AreEqual(6, page.FirstItem);
			Assert.AreEqual(10, page.LastItem);
			Assert.AreEqual(15, page.TotalItems);

			Assert.IsTrue(page.HasFirst);
			Assert.IsTrue(page.HasLast);
			Assert.IsTrue(page.HasNext);
			Assert.IsTrue(page.HasPrevious);

			// Last page

			page = PaginationHelper.CreatePagination(items, 5, 3);

			Assert.IsNotNull(page);

			Assert.AreEqual(11, page.FirstItem);
			Assert.AreEqual(15, page.LastItem);
			Assert.AreEqual(15, page.TotalItems);

			Assert.IsTrue(page.HasFirst);
			Assert.IsFalse(page.HasLast);
			Assert.IsFalse(page.HasNext);
			Assert.IsTrue(page.HasPrevious);
		}
#endif
	}
}
