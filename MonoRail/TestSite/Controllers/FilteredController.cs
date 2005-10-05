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

namespace TestSite.Controllers
{
	using System;

	using Castle.MonoRail.Framework;

	[Filter( ExecuteEnum.Before|ExecuteEnum.After, typeof(MyFilter) )]
	public class FilteredController : Controller
	{
		public FilteredController()
		{
		}

		[SkipFilter]
		public void Index()
		{
		}

		public void Save()
		{
			RenderText("content");
		}

		public void Update()
		{
		}

        [SkipFilter( typeof(MyFilter))]
        public void SelectiveSkip()
        {
        }
	}

	public class MyFilter : IFilter
	{
		public bool Perform(ExecuteEnum exec, IRailsEngineContext context, Controller controller)
		{
			if (exec == ExecuteEnum.Before)
			{
				context.Response.Write("(before)");
			}
			else
			{
				context.Response.Write("(after)");
			}

			return true; // Continue execution
		}
	}
}
