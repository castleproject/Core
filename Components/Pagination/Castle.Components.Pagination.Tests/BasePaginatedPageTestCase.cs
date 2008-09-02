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

namespace Castle.Components.Pagination.Tests
{
	using System.Collections;
	using System.Collections.Generic;
	using NUnit.Framework;

	public abstract class BasePaginatedPageTestCase<TClassToTest> where TClassToTest : IPaginatedPage
	{
		[Test]
		public void TestExpectedResultForFirstPageWithSingleItemPage()
		{
			IList sampleData = getSampleListData();
			int currentPageIndex = 1;
			int pageSize = 1;
			IPaginatedPage page = CreateAbstractPageWithSampleData(sampleData, currentPageIndex, pageSize);
			assertIsOnFirstPageHavingMultiplePages(page);
			Assert.AreEqual(sampleData.Count, page.TotalPages);
			Assert.AreEqual(currentPageIndex + 1, page.NextPageIndex);
			Assert.AreEqual(currentPageIndex - 1, page.PreviousPageIndex);
			Assert.AreEqual(1, page.FirstItemIndex);
			Assert.AreEqual(1, page.LastItemIndex);
			Assert.AreEqual(sampleData.Count, page.TotalItems);
			Assert.AreEqual(pageSize, page.PageSize);
			Assert.AreEqual(page.FirstItem, page.LastItem);
			Assert.AreEqual("one", page.FirstItem);
			Assert.AreEqual(1, page.CurrentPageSize);
			assertHasPageIsTrueBetween(page, 1, sampleData.Count, -1, sampleData.Count + 2);
			assertGetEnumeratorItterateOverExpectedValues(page, "one");
		}

		[Test]
		public void TestExpectedResultForMiddlePageWithSingleItemPage()
		{
			IList sampleData = getSampleListData();
			int currentPageIndex = 3;
			int pageSize = 1;
			IPaginatedPage page = CreateAbstractPageWithSampleData(sampleData, currentPageIndex, pageSize);
			Assert.AreEqual(currentPageIndex, page.CurrentPageIndex);
			Assert.AreEqual(sampleData.Count, page.TotalPages);
			Assert.IsTrue(page.HasPreviousPage);
			Assert.IsTrue(page.HasNextPage);
			Assert.AreEqual(currentPageIndex + 1, page.NextPageIndex);
			Assert.AreEqual(currentPageIndex - 1, page.PreviousPageIndex);
			Assert.AreEqual(3, page.FirstItemIndex);
			Assert.AreEqual(3, page.LastItemIndex);
			Assert.AreEqual(sampleData.Count, page.TotalItems);
			Assert.AreEqual(pageSize, page.PageSize);
			Assert.AreEqual(page.FirstItem, page.LastItem);
			Assert.AreEqual("three", page.FirstItem);
			Assert.AreEqual(1, page.CurrentPageSize);
			assertHasPageIsTrueBetween(page, 1, sampleData.Count, -1, sampleData.Count + 2);
			assertGetEnumeratorItterateOverExpectedValues(page, "three");
		}

		[Test]
		public void TestExpectedResultForLastPageWithSingleItemPage()
		{
			IList sampleData = getSampleListData();
			int currentPageIndex = sampleData.Count;
			int pageSize = 1;
			IPaginatedPage page = CreateAbstractPageWithSampleData(getSampleListData(), currentPageIndex, pageSize);
			assertIsOnLastPageHavingMultiplePages(page);
			Assert.AreEqual(currentPageIndex, page.CurrentPageIndex);
			Assert.AreEqual(currentPageIndex + 1, page.NextPageIndex);
			Assert.AreEqual(currentPageIndex - 1, page.PreviousPageIndex);
			Assert.AreEqual(5, page.FirstItemIndex);
			Assert.AreEqual(5, page.LastItemIndex);
			Assert.AreEqual(sampleData.Count, page.TotalItems);
			Assert.AreEqual(pageSize, page.PageSize);
			Assert.AreEqual(page.FirstItem, page.LastItem);
			Assert.AreEqual("five", page.FirstItem);
			Assert.AreEqual(1, page.CurrentPageSize);
			assertHasPageIsTrueBetween(page, 1, sampleData.Count, -1, sampleData.Count + 2);
			assertGetEnumeratorItterateOverExpectedValues(page, "five");
		}

		[Test]
		public void TestExpectedResultForFirstPageWithTwoItemPerPage()
		{
			IList sampleData = getSampleListData();
			int currentPageIndex = 1;
			int pageSize = 2;
			IPaginatedPage page = CreateAbstractPageWithSampleData(sampleData, currentPageIndex, pageSize);
			assertIsOnFirstPageHavingMultiplePages(page);
			Assert.AreEqual(currentPageIndex + 1, page.NextPageIndex);
			Assert.AreEqual(currentPageIndex - 1, page.PreviousPageIndex);
			Assert.AreEqual(2, page.CurrentPageSize);
			assertGetEnumeratorItterateOverExpectedValues(page, "one", "two");
		}

		[Test]
		public void TestExpectedResultForMiddlePageWithTwoItemPerPage()
		{
			IList sampleData = getSampleListData();
			int currentPageIndex = 2;
			int pageSize = 2;
			IPaginatedPage page = CreateAbstractPageWithSampleData(sampleData, currentPageIndex, pageSize);
			Assert.AreEqual(4, page.LastItemIndex);
			Assert.AreEqual(currentPageIndex + 1, page.NextPageIndex);
			Assert.AreEqual(currentPageIndex - 1, page.PreviousPageIndex);
			Assert.AreEqual(2, page.CurrentPageSize);
			assertGetEnumeratorItterateOverExpectedValues(page, "three", "four");
		}

		[Test]
		public void TestExpectedResultForLastPageWithTwoItemPerPage()
		{
			IList sampleData = getSampleListData();
			int currentPageIndex = 3;
			int pageSize = 2;
			IPaginatedPage page = CreateAbstractPageWithSampleData(sampleData, currentPageIndex, pageSize);
			assertIsOnLastPageHavingMultiplePages(page);
			Assert.AreEqual(sampleData.Count, page.LastItemIndex);
			Assert.AreEqual(currentPageIndex + 1, page.NextPageIndex);
			Assert.AreEqual(currentPageIndex - 1, page.PreviousPageIndex);
			Assert.AreEqual(1, page.CurrentPageSize);
			assertGetEnumeratorItterateOverExpectedValues(page, "five");
		}

		private void assertGetEnumeratorItterateOverExpectedValues(IPaginatedPage page, params string[] values)
		{
			int CurrentPageIndex = 0;
			foreach(string value in page)
			{
				Assert.AreEqual(values[CurrentPageIndex], value);
				++CurrentPageIndex;
			}
		}

		private void assertHasPageIsTrueBetween(IPaginatedPage page, int firstPageIndex, int lastPageIndex,
		                                        int rangeLowerBound, int rangeUpperBound)
		{
			for(int i = rangeLowerBound, max = rangeUpperBound; i < max; ++i)
			{
				if (i < firstPageIndex || i > lastPageIndex)
				{
					Assert.IsFalse(page.HasPage(i), "expecting false for HasPage({0})", i);
				}
				else
				{
					Assert.IsTrue(page.HasPage(i), "expecting true for HasPage({0})", i);
				}
			}
		}

		private void assertIsOnFirstPageHavingMultiplePages(IPaginatedPage page)
		{
			Assert.IsFalse(page.HasPreviousPage);
			Assert.IsTrue(page.HasNextPage);
			Assert.AreEqual(1, page.CurrentPageIndex);
		}

		private void assertIsOnLastPageHavingMultiplePages(IPaginatedPage page)
		{
			Assert.IsTrue(page.HasPreviousPage);
			Assert.IsFalse(page.HasNextPage);
			Assert.AreEqual(page.TotalPages, page.CurrentPageIndex);
		}

		private IList getSampleListData()
		{
			return new ArrayList(new string[] {"one", "two", "three", "four", "five"});
		}

		protected abstract IPaginatedPage CreateAbstractPageWithSampleData(IList sampleData, int currentPage, int pageSize);
	}

	[TestFixture]
	public class PageTestCase : BasePaginatedPageTestCase<Page>
	{
		protected override IPaginatedPage CreateAbstractPageWithSampleData(IList sampleData, int currentPage, int pageSize)
		{
			return new Page(sampleData, currentPage, pageSize);
		}
	}

	[TestFixture]
	public class GenericPageTestCase : BasePaginatedPageTestCase<GenericPage<string>>
	{
		protected override IPaginatedPage CreateAbstractPageWithSampleData(IList sampleData, int currentPage, int pageSize)
		{
			IList<string> genericData = new List<string>(sampleData.Count);
			foreach(string value in sampleData)
			{
				genericData.Add(value);
			}
			return new GenericPage<string>(genericData, currentPage, pageSize);
		}
	}

	[TestFixture]
	public class GenericCustomPageTestCase : BasePaginatedPageTestCase<GenericPage<string>>
	{
		protected override IPaginatedPage CreateAbstractPageWithSampleData(IList sampleData, int currentPage, int pageSize)
		{
			IList<string> genericData = new List<string>(pageSize);
			int firstindex, lastindex;
			firstindex = (currentPage - 1) * pageSize;
			lastindex = firstindex + pageSize - 1;
			for(int i = 0; i < sampleData.Count; ++i)
			{
				if (i < firstindex)
				{
					continue;
				}
				if (i > lastindex)
				{
					break;
				}
				genericData.Add(sampleData[i] as string);
			}
			return new GenericCustomPage<string>(genericData, currentPage, pageSize, sampleData.Count);
		}
	}
}