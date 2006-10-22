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

namespace FormHelperSample.Controllers
{
	using System;
	using System.Collections;
	using Castle.MonoRail.Framework;
	using FormHelperSample.Models;

	[Layout("default"), Rescue("generalerror")]
	public class SelectSampleController : SmartDispatcherController
	{
		public void SelectIndex()
		{
			int[] primenumbers = new int[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47 };
			Category[] categories = new Category[] { new Category(1, "Music"), new Category(2, "Humor"), new Category(3, "Politics")  };
			IList authors = new Author[] { new Author(1, "hammett"), new Author(2, "john doe"), new Author(3, "someone else"), };
			
			PropertyBag["primenumbers"] = primenumbers;
			PropertyBag["categories"] = categories;
			PropertyBag["authors"] = authors;

			// Pre selection (which is optional)
			
			Blog blog = new Blog();
			blog.Author = new Author(1, "hammett");

			PropertyBag["oneprime"] = 3;
			PropertyBag["multipleprimes"] = new int[] { 3, 5, 7 };
			PropertyBag["blog"] = blog;
		}
		
		public void Process(int oneprime, int[] multipleprimes, [DataBind("blog")] Blog blog)
		{
			PropertyBag["oneprime"] = 3;
			PropertyBag["multipleprimes"] = new int[] { 3, 5, 7 };
			PropertyBag["blog"] = blog;
		}
	}
}
