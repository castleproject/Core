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

namespace Castle.MonoRail.Framework.Tests.Helpers
{
	using System.Collections;
	using System.Collections.Generic;
	using Castle.Components.Pagination;
	using NUnit.Framework;

	using Castle.MonoRail.Framework.Helpers;

	[TestFixture]
	public class PaginationHelperTestCase
	{
		[Test]
		public void EmptyCollection()
		{
			IList items = new ArrayList();

			IPaginatedPage page = PaginationHelper.CreatePagination(items, 10, 1);
			
			Assert.IsNotNull(page);

			Assert.AreEqual(0, page.FirstItemIndex);
			Assert.AreEqual(0, page.LastItemIndex);
			Assert.AreEqual(0, page.TotalItems);
			
			Assert.IsFalse(page.HasNextPage);
			Assert.IsFalse(page.HasPreviousPage);
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

			Assert.AreEqual(1, page.FirstItemIndex);
			Assert.AreEqual(5, page.LastItemIndex);
			Assert.AreEqual(5, page.TotalItems);
			Assert.IsFalse(page.HasNextPage);
			Assert.IsFalse(page.HasPreviousPage);
		}

		[Test]
		public void JustOneItem()
		{
			IList items = new ArrayList();

			items.Add("1");

			IPaginatedPage page = PaginationHelper.CreatePagination(items, 10, 1);
			
			Assert.IsNotNull(page);

			Assert.AreEqual(1, page.FirstItemIndex);
			Assert.AreEqual(1, page.LastItemIndex);
			Assert.AreEqual(1, page.TotalItems);
			
			Assert.IsFalse(page.HasNextPage);
			Assert.IsFalse(page.HasPreviousPage);
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

			Assert.AreEqual(1, page.FirstItemIndex);
			Assert.AreEqual(3, page.LastItemIndex);
			Assert.AreEqual(3, page.TotalItems);
			
			Assert.IsFalse(page.HasNextPage);
			Assert.IsFalse(page.HasPreviousPage);
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

			Assert.AreEqual(1, page.FirstItemIndex);
			Assert.AreEqual(5, page.LastItemIndex);
			Assert.AreEqual(15, page.TotalItems);
			
			Assert.IsTrue(page.HasNextPage);
			Assert.IsFalse(page.HasPreviousPage);

			// Second page

			page = PaginationHelper.CreatePagination( items, 5, 2 );
			
			Assert.IsNotNull(page);

			Assert.AreEqual(6, page.FirstItemIndex);
			Assert.AreEqual(10, page.LastItemIndex);
			Assert.AreEqual(15, page.TotalItems);
			
			Assert.IsTrue(page.HasNextPage);
			Assert.IsTrue(page.HasPreviousPage);

			// Last page

			page = PaginationHelper.CreatePagination( items, 5, 3 );
			
			Assert.IsNotNull(page);

			Assert.AreEqual(11, page.FirstItemIndex);
			Assert.AreEqual(15, page.LastItemIndex);
			Assert.AreEqual(15, page.TotalItems);
			
			Assert.IsFalse(page.HasNextPage);
			Assert.IsTrue(page.HasPreviousPage);
		}

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

			Assert.AreEqual(1, page.FirstItemIndex);
			Assert.AreEqual(5, page.LastItemIndex);
			Assert.AreEqual(15, page.TotalItems);

			Assert.IsTrue(page.HasNextPage);
			Assert.IsFalse(page.HasPreviousPage);

			// Second page

			page = PaginationHelper.CreatePagination(items, 5, 2);

			Assert.IsNotNull(page);

			Assert.AreEqual(6, page.FirstItemIndex);
			Assert.AreEqual(10, page.LastItemIndex);
			Assert.AreEqual(15, page.TotalItems);

			Assert.IsTrue(page.HasNextPage);
			Assert.IsTrue(page.HasPreviousPage);

			// Last page

			page = PaginationHelper.CreatePagination(items, 5, 3);

			Assert.IsNotNull(page);

			Assert.AreEqual(11, page.FirstItemIndex);
			Assert.AreEqual(15, page.LastItemIndex);
			Assert.AreEqual(15, page.TotalItems);
			Assert.IsFalse(page.HasNextPage);
			Assert.IsTrue(page.HasPreviousPage);
		}


		[Test]
		public void CustomPaginationForObjectCollection()
		{
			IList items = new ArrayList();

			/* First page */
			items.Add("1");
			items.Add("2");
			items.Add("3");

			IPaginatedPage page = PaginationHelper.CreateCustomPage(items, 3, 1, 8);

			Assert.IsNotNull(page);

			Assert.AreEqual(1, page.FirstItemIndex);
			Assert.AreEqual(3, page.LastItemIndex);
			Assert.AreEqual(8, page.TotalItems);

			Assert.IsTrue(page.HasNextPage);
			Assert.IsFalse(page.HasPreviousPage);

			/* Second page */
			items.Clear();
			items.Add("4");
			items.Add("5");
			items.Add("6");

			page = PaginationHelper.CreateCustomPage(items, 3, 2, 8);

			Assert.IsNotNull(page);

			Assert.AreEqual(4, page.FirstItemIndex);
			Assert.AreEqual(6, page.LastItemIndex);
			Assert.AreEqual(8, page.TotalItems);

			Assert.IsTrue(page.HasNextPage);
			Assert.IsTrue(page.HasPreviousPage);

			/* Third page, partial */
			items.Clear();
			items.Add("7");
			items.Add("8");

			page = PaginationHelper.CreateCustomPage(items, 3, 3, 8);

			Assert.IsNotNull(page);

			Assert.AreEqual(7, page.FirstItemIndex);
			Assert.AreEqual(8, page.LastItemIndex);
			Assert.AreEqual(8, page.TotalItems);

			Assert.IsFalse(page.HasNextPage);
			Assert.IsTrue(page.HasPreviousPage);
		}

		[Test]
		public void CustomPaginationForGenericCollection()
		{
			IList<string> items = new List<string>();

			/* First page */
			items.Add("1");
			items.Add("2");
			items.Add("3");

			IPaginatedPage page = PaginationHelper.CreateCustomPage(items, 3, 1, 8);

			Assert.IsNotNull(page);

			Assert.AreEqual(1, page.FirstItemIndex);
			Assert.AreEqual(3, page.LastItemIndex);
			Assert.AreEqual(8, page.TotalItems);

			Assert.IsTrue(page.HasNextPage);
			Assert.IsFalse(page.HasPreviousPage);

			/* Second page */
			items.Clear();
			items.Add("4");
			items.Add("5");
			items.Add("6");

			page = PaginationHelper.CreateCustomPage(items, 3, 2, 8);

			Assert.IsNotNull(page);

			Assert.AreEqual(4, page.FirstItemIndex);
			Assert.AreEqual(6, page.LastItemIndex);
			Assert.AreEqual(8, page.TotalItems);

			Assert.IsTrue(page.HasNextPage);
			Assert.IsTrue(page.HasPreviousPage);

			/* Third page, partial */
			items.Clear();
			items.Add("7");
			items.Add("8");

			page = PaginationHelper.CreateCustomPage(items, 3, 3, 8);

			Assert.IsNotNull(page);

			Assert.AreEqual(7, page.FirstItemIndex);
			Assert.AreEqual(8, page.LastItemIndex);
			Assert.AreEqual(8, page.TotalItems);

			Assert.IsFalse(page.HasNextPage);
			Assert.IsTrue(page.HasPreviousPage);
		}


	}
}
