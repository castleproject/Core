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

namespace AutoCompletionSample.Controllers
{
	using System;
	using System.Collections;
	using Castle.MonoRail.Framework;

	public class AccountController : SmartDispatcherController
	{
		public void Index()
		{
		}

		public void GetSearchItems(String name)
		{
			IList items = GetRecords();
			IList matchItems = new ArrayList();

			name = name.ToLower();

			foreach (string item in items)
			{
				if (item.ToLower().StartsWith(name))
				{
					matchItems.Add(item);
				}
			}

			PropertyBag.Add("items", matchItems);

			RenderView("partialmatchlist");
		}

		private IList GetRecords()
		{
			ArrayList items = new ArrayList();

			items.Add("Ted");
			items.Add("Teddy");
			items.Add("Mark");
			items.Add("Alfred");
			
			return items;
		}
	}
}