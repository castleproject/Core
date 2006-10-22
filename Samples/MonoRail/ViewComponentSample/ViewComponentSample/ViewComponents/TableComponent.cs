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

namespace ViewComponentSample.ViewComponents
{
	using System;
	using System.Collections;
	using Castle.MonoRail.Framework;

	public class TableComponent : ViewComponent
	{
		private ICollection elements;
		
		private object border;
		private string style;
		private object cellpadding;
		private object cellspacing;

		public override void Initialize()
		{
			elements = (ICollection) ComponentParams["elements"];
			
			border = ComponentParams["border"];
			style = (String) ComponentParams["style"];
			cellpadding = ComponentParams["cellpadding"];
			cellspacing = ComponentParams["cellspacing"];
			
			base.Initialize();
		}

		public override void Render()
		{
			RenderText(
				String.Format("<table border=\"{0}\" style=\"{1}\" cellpadding=\"{2}\" cellspacing=\"{3}\">", 
				              border, style, cellpadding, cellspacing));
			
			if (Context.HasSection("colheaders"))
			{
				Context.RenderSection("colheaders");
			}
			
			if (elements != null)
			{
				int index = 0;
				
				foreach(object item in elements)
				{
					PropertyBag["index"] = ++index;
					PropertyBag["item"] = item;
					
					if (Context.HasSection("altitem") && index % 2 != 0)
					{
						Context.RenderSection("altitem");
					}
					else
					{
						Context.RenderSection("item");
					}
				}
			}
			
			RenderText("</table>");
		}

		public override bool SupportsSection(string name)
		{
			return name == "colheaders" || name == "item" || name == "altitem";
		}
	}
}
