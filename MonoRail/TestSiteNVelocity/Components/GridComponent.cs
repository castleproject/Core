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

namespace TestSiteNVelocity.Components
{
	using System;
	using System.Collections;

	using Castle.MonoRail.Framework;

	public class GridComponent : ViewComponent
	{
		public override bool SupportsSection(String name)
		{
			return name == "tablestart" || name == "tableend" || 
				name == "header" || name == "item" || name == "footer" || 
				name == "empty";
		}

		public override void Render()
		{
			ICollection source = (ICollection) ComponentParams["source"];

			StartTable();

			Context.RenderSection("header");

			if (source != null && source.Count != 0)
			{
				foreach(object item in source)
				{
					PropertyBag["item"] = item;
					
					RenderText("<tr>");
					
					Context.RenderSection("item");
					
					RenderText("</tr>");
				}
			}
			else
			{
				RenderText("<tr>");
				Context.RenderSection("empty");
				RenderText("</tr>");
			}

			Context.RenderSection("footer");

			EndTable();
		}

		private void StartTable()
		{
			if (Context.HasSection("tablestart"))
			{
				Context.RenderSection("tablestart");
			}
			else
			{
				RenderText("<table>");
			}
		}

		private void EndTable()
		{
			if (Context.HasSection("tableend"))
			{
				Context.RenderSection("tableend");
			}
			else
			{
				RenderText("</table>");
			}
		}
	}
}
