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
	using Castle.MonoRail.Framework;
	using FormHelperSample.Models;

	[Layout("default"), Rescue("generalerror")]
	public class CheckboxSampleController : SmartDispatcherController
	{
		public void CheckboxIndex()
		{
			
		}
		
		public void ProcessCheckbox(bool enabled1, int[] enabled2)
		{
			PropertyBag["enabled1"] = enabled1;
			PropertyBag["enabled2"] = enabled2;
		}
		
		public void CheckboxListIndex()
		{
			int[] primenumbers = new int[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97 };
			Category[] categories = new Category[] { new Category(1, "Music"), new Category(2, "Humor"), new Category(3, "Politics")  };
		
			PropertyBag["primenumbers"] = primenumbers;
			PropertyBag["categories"] = categories;
			
			// Pre selection (optional)
			int[] selectedPrimes = new int[] { 11, 19, 29 };
			
			Blog blog = new Blog();
			blog.Categories = new Category[] { new Category(2, "Humor") };
			
			PropertyBag["selectedPrimes"] = selectedPrimes;
			PropertyBag["blog"] = blog;
		}

		public void ProcessCheckboxList(int[] selectedPrimes, [DataBind("blog")] Blog blog)
		{
			PropertyBag["selectedPrimes"] = selectedPrimes;
			PropertyBag["blog"] = blog;
		}
	}
}
