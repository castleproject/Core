// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.CastleOnRails.Framework.Tests.Controllers
{
	using System;

	using NUnit.Framework;

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
		}

		public void Update()
		{
		}
	}

	public class MyFilter : IFilter
	{
		public bool Perform(ExecuteEnum exec, IRailsEngineContext context, Controller controller)
		{
			Assert.IsNotNull(context);
			Assert.IsNotNull(controller);

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
