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
// 
namespace Castle.Components.Binder
{
	using System;
	using System.Collections;

	public sealed class IndexComparer : IComparer
	{
		private static readonly IndexComparer comparer = new IndexComparer();

		public static IndexComparer Instance
		{
			get { return comparer; }
		}

		public int Compare(object x, object y)
		{
			String left, right;

			left = (String) x;
			right = (String) y;

			return Convert.ToInt32(left) - Convert.ToInt32(right);
		}
	}
}