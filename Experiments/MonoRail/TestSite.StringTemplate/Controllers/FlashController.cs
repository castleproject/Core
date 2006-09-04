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

namespace TestSite.StringTemplate.Controllers
{
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Filters;

	[Filter(ExecuteEnum.BeforeAction, typeof(RequestValidatorFilter))]
	public class FlashController : SmartDispatcherController
	{
		public FlashController()
		{
		}

		public void FlashIt()
		{
			Flash["flashMessage"] = "It's a flash message";
		}

		public void ValidateRequest()
		{			
		}

		public void FlashContents()
		{	
		}

		public void AddFlashAndRedirect()
		{
			Flash["one"] = 1;
			Flash["two"] = 2;
			Flash["three"] = 3;

			RedirectToAction("FlashContents");
		}
	}
}
