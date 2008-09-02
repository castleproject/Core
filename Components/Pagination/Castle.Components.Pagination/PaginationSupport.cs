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

namespace Castle.Components.Pagination
{
	using System;
	using System.Collections;
	using System.Collections.Generic;


	/// <summary>
	/// Common pagination support methods
	/// </summary>
	public static class PaginationSupport
	{
		/// <summary>
		/// Subsititute for System.Linq.Enumerable.ElementAt
		/// </summary>
		/// <param name="enumerable"></param>
		/// <param name="itemIndex"></param>
		/// <returns></returns>
		public static object GetItemAtIndex(IEnumerable enumerable, int itemIndex)
		{
			object item;
			IList list = enumerable as IList;
			if (list != null)
			{
				item = list[itemIndex];
			}
			else if (itemIndex < 0)
			{
				throw new ArgumentException("itemIndex");
			}
			else
			{
				IEnumerator enumerator = enumerable.GetEnumerator();
				int currentIndex = 0;
				do
				{
					enumerator.MoveNext();
					currentIndex++;
				} while(currentIndex < itemIndex);
				item = enumerator.Current;
			}
			return item;
		}

		/// <summary>
		/// Subsititute for System.Linq.Enumerable.ElementAt
		/// </summary>
		/// <param name="enumerable"></param>
		/// <param name="itemIndex"></param>
		/// <returns></returns>
		public static T GetItemAtIndex<T>(IEnumerable<T> enumerable, int itemIndex)
		{
			T item;
			IList<T> list = enumerable as IList<T>;
			if (list != null)
			{
				item = list[itemIndex];
			}
			else if (itemIndex < 0)
			{
				throw new ArgumentException("itemIndex");
			}
			else
			{
				using(IEnumerator<T> enumerator = enumerable.GetEnumerator())
				{
					int currentIndex = 0;
					do
					{
						enumerator.MoveNext();
						currentIndex++;
					} while(currentIndex < itemIndex);
					item = enumerator.Current;
				}
			}
			return item;
		}
	}
}