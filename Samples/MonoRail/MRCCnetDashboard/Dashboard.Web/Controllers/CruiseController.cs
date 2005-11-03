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

namespace Dashboard.Web.Controllers
{
	using System;
	
	using Dashboard.Web.Service;

	using ThoughtWorks.CruiseControl.Remote;


	public class CruiseController : BaseController
	{
		private readonly ContentTransformation contentTransformation;

		public CruiseController(ICruiseManager server, ContentTransformation contentTransformation) : base(server)
		{
			this.contentTransformation = contentTransformation;
		}

		public void Index()
		{
			ProjectStatus[] status = CruiseManager.GetProjectStatus();
			PropertyBag.Add("status", status);
		}

		public void View(String name)
		{
			String[] names = CruiseManager.GetMostRecentBuildNames(name, 5);
			
			PropertyBag.Add("names", names);
		}

		public void ViewSummary(String name, String log)
		{
			PropertyBag.Add("summary", contentTransformation.GetSummary(name, log));

			RenderView("partial_summary");
		}

		public void ViewSpecific(String name, String log, DetailEnum detail)
		{
			PropertyBag.Add("summary", contentTransformation.GetDetail(name, log, detail));

			RenderView("partial_summary");
		}
	}
}
