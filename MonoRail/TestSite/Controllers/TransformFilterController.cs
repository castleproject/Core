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

namespace TestSite.Controllers
{
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.TransformFilters;
	using Castle.MonoRail.TransformFilters;

	public class TransformFilteredController : Controller
	{
		[TransformFilter(typeof(UpperCaseTransformFilter))]
		public void ToUpperCase()
		{
			RenderText("this is not a lowercase string");
		}

		[TransformFilter(typeof(MarkdownTransformFilter))]
		public void Markdown()
		{
			// render the view 
		}

		[TransformFilter(typeof(WikiTransformFilter))]
		public void Wiki()
		{
			// render the view 
		}

		[TransformFilter(typeof(LowerCaseTransformFilter), ExecutionOrder = 0)]
		[TransformFilter(typeof(UpperCaseTransformFilter), ExecutionOrder = 1)]
		public void ExecutingOrder()
		{
			RenderText("this is not a lowercase string");
		}

	}
}
