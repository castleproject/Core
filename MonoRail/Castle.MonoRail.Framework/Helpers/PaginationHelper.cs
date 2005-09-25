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

namespace Castle.MonoRail.Framework.Helpers
{
	using System;
	using System.Collections;
	using System.Web;


	public class PaginationHelper : AbstractHelper
	{
		public PaginationHelper()
		{
		}

		public String CreatePageLink(int page, String text)
		{
			return CreatePageLink(page, text, null, null);
		}

		public String CreatePageLink(int page, String text, IDictionary htmlAttributes)
		{
			return CreatePageLink(page, text, htmlAttributes, null);
		}

		public String CreatePageLink(int page, String text, IDictionary htmlAttributes, IDictionary queryStringParams)
		{
			String filePath = HttpContext.Current.Request.FilePath;

			if (queryStringParams == null)
			{
				queryStringParams = new Hashtable();
			}

			queryStringParams["page"] = page.ToString();

			return String.Format("<a href=\"{0}?{1}\" {2}>{3}</a>", 
				filePath, BuildQueryString(queryStringParams), GetAttributes(htmlAttributes), text);
		}

		public static Page CreatePagination(IList list, int pageSize, Controller controller)
		{
			String currentPage = controller.Request.Params["page"];

			int curPage = 1;

			if (currentPage != null)
			{
				curPage = Int32.Parse(currentPage);
			}

			return new Page( list, curPage, pageSize );
		}
	}

	[Serializable]
	public class Page : IEnumerable
	{
		private int firstItem, lastItem, totalItems;
		private int previousIndex, nextIndex, lastIndex;
		private bool hasPrev, hasNext, hasFirst, hasLast;

		[NonSerialized]
		private readonly IList slice = new ArrayList();

		public Page(IList list, int curPage, int pageSize)
		{
			// Calculate slice indexes
			int startIndex = (pageSize * curPage) - pageSize;
			int endIndex =  Math.Min(startIndex + pageSize, list.Count);

			CreateSlicedCollection(startIndex, endIndex, list);

			ProcessViewData(startIndex, endIndex, list, pageSize, curPage);
		}

		private void CreateSlicedCollection(int startIndex, int endIndex, IList list)
		{
			for(int index = startIndex; index < endIndex; index++)
			{
				slice.Add( list[index] );
			}
		}

		private void ProcessViewData(int startIndex, int endIndex, IList list, int pageSize, int curPage)
		{
			firstItem = startIndex + 1;
			lastItem = endIndex;
			totalItems = list.Count;
	
			hasPrev = startIndex != 0;
			hasNext = (startIndex + pageSize) < list.Count;
			hasFirst = curPage != 1;
			hasLast = list.Count > curPage * pageSize;
	
			previousIndex = curPage - 1;
			nextIndex = curPage + 1;
			lastIndex = list.Count / pageSize;
	
			if (list.Count / (float) pageSize > lastIndex)
			{
				lastIndex++;
			}
		}

		public int LastIndex
		{
			get { return lastIndex; }
		}

		public int PreviousIndex
		{
			get { return previousIndex; }
		}

		public int NextIndex
		{
			get { return nextIndex; }
		}

		public bool HasPrevious
		{
			get { return hasPrev; }
		}

		public bool HasNext
		{
			get { return hasNext; }
		}

		public bool HasFirst
		{
			get { return hasFirst; }
		}

		public bool HasLast
		{
			get { return hasLast; }
		}

		public int FirstItem
		{
			get { return firstItem; }
		}

		public int LastItem
		{
			get { return lastItem; }
		}

		public int TotalItems
		{
			get { return totalItems; }
		}

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return slice.GetEnumerator();
		}

		#endregion
	}
}
