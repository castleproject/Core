// Copyright 2004-2006 Castle Project - http://www.railsproject.org/
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

namespace ActiveRecordIntegrationSample.Components
{
	using System;
	using System.Collections;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Helpers;

	public class ListComponent : ViewComponent
	{
		private IList elements;
		private string name;

		public override void Initialize()
		{
			elements = (IList) ComponentParams["source"];
			name = (String) ComponentParams["controllername"];
			
			base.Initialize();
		}

		public override void Render()
		{
 			IPaginatedPage page = PaginationHelper.CreatePagination(elements, 5);
			
			PropertyBag["page"] = page;
			PropertyBag["name"] = name;
			
			base.Render();
		}

		public override bool SupportsSection(string name)
		{
			return base.SupportsSection(name);
		}
	}
}
