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

namespace ViewComponentSample.Controllers
{
	using System;
	using Castle.MonoRail.Framework;

	[Layout("default")]
	public class HomeController : SmartDispatcherController
	{
		public void Index()
		{
			PropertyBag.Add("items",
			                new String[]
			                	{
			                		"Item 1", "Item 2", "Item 3", "Item 4", "Item 5", 
			                		"Item 6", "Item 7", "Item 8", "Item 9", "Item 10", 
			                	});
		}
	}
}