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

namespace Castle.MonoRail.Framework.Helpers
{
	using System.Collections;

	/// <summary>
	/// Represents a page of a bigger set
	/// </summary>
	/// <remarks>
	/// Indexes are zero based.
	/// </remarks>
	public interface IPaginatedPage : IEnumerable
	{
		/// <summary>
		/// The index this page represents
		/// </summary>
		int CurrentIndex { get; }
		
		/// <summary>
		/// The last index available on the set
		/// </summary>
		int LastIndex { get; }
		
		/// <summary>
		/// The next index (from this page)
		/// </summary>
		int NextIndex { get; }
		
		/// <summary>
		/// The previous index (from this page)
		/// </summary>
		int PreviousIndex { get; }

		/// <summary>
		/// The first index
		/// </summary>
		int FirstIndex { get; }

		/// <summary>
		/// The first element (index + 1)
		/// </summary>
		int FirstItem { get; }

		/// <summary>
		/// The last element in the page (count)
		/// </summary>
		int LastItem { get; }

		/// <summary>
		/// The count of all elements on the set
		/// </summary>
		int TotalItems { get; }

		/// <summary>
		/// Returns true if a previous page 
		/// is accessible from this page
		/// </summary>
		bool HasPrevious { get; }
		
		/// <summary>
		/// Returns true if a next page is
		/// accessible from this page
		/// </summary>
		bool HasNext { get; }

		/// <summary>
		/// Returns true if a first page 
		/// exists
		/// </summary>
		bool HasFirst { get; }

		/// <summary>
		/// Returns true if a last page 
		/// exists
		/// </summary>
		bool HasLast { get; }
	}
}
