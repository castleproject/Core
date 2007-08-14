// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.ViewComponents
{
	using System;
	using System.Collections;

	/// <summary>
	/// Renders a table where each (repeated) item
	/// is rendered to a column. 
	/// TODO: Document it properly!
	/// </summary>
	public class ColumnRenderer : ViewComponent
	{
		private int cols = 3;
		private IEnumerable enumerable;
		private bool dontRenderUneededTableForEmptyLists = false;

		/// <summary>
		/// Called by the framework once the component instance
		/// is initialized
		/// </summary>
		public override void Initialize()
		{
			if (ComponentParams.Contains("cols"))
			{
				cols = Convert.ToInt32(ComponentParams["cols"]);

				if (cols <= 0)
				{
					throw new ViewComponentException("ColumnRenderer: 'cols' parameter must be greater than zero");
				}
			}
			if (ComponentParams.Contains("emptyness")) {
				dontRenderUneededTableForEmptyLists = Convert.ToBoolean(ComponentParams["emptyness"]);
			}

			enumerable = (IEnumerable) ComponentParams["items"];
		}

		/// <summary>
		/// Called by the framework so the component can 
		/// render its content
		/// </summary>
		public override void Render()
		{
			if (IsEmptyAndShouldNotRenderBrokenTableTags())
			{
				NoElements();
				return;
			}
			StartTable();

			bool hasElements = false;
			int itemIndex = 0;

			if (enumerable != null)
			{
				int toFinishRow = 0;

				if (Context.HasSection("firstelement"))
				{
					StartRow();

					StartColumn();

					Context.RenderSection("firstelement");

					EndColumn();

					toFinishRow = itemIndex++ + cols;
				}

				foreach(object item in enumerable)
				{
					hasElements = true;

					bool writeRow = itemIndex++ % cols == 0;

					if (toFinishRow == itemIndex) EndRow();

					if (writeRow)
					{
						StartRow();
					}

					StartColumn();

					WriteElement(item);

					EndColumn();

					if (writeRow)
					{
						toFinishRow = itemIndex + cols;
					}
				}

				if (itemIndex < toFinishRow)
				{
					for(; itemIndex < toFinishRow - 1; itemIndex++)
					{
						StartColumn();
						RenderText("&nbsp;");
						EndColumn();
					}
					EndRow();
				}
			}

			if (!hasElements)
			{
				NoElements();
			}

			EndTable();
		}


		private bool IsEmptyAndShouldNotRenderBrokenTableTags()
		{
			if (dontRenderUneededTableForEmptyLists)
			{
				if (enumerable == null || !enumerable.GetEnumerator().MoveNext())
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Implementor should return true only if the 
		/// <c>name</c> is a known section the view component
		/// supports.
		/// </summary>
		/// <param name="name">section being added</param>
		/// <returns><see langword="true"/> if section is supported</returns>
		public override bool SupportsSection(string name)
		{
			return name == "start" || name == "endblock" ||
			       name == "startcolumn" || name == "endcolumn" ||
			       name == "startrow" || name == "endrow" ||
			       name == "item" || name == "firstelement" || 
				   name == "empty";
		}

		private void WriteElement(object item)
		{
			if (Context.HasSection("item"))
			{
				PropertyBag["item"] = item;
				Context.RenderSection("item");
			}
			else
			{
				RenderText(item.ToString());
			}
		}

		private void StartColumn()
		{
			if (Context.HasSection("startcolumn"))
			{
				Context.RenderSection("startcolumn");
			}
			else
			{
				RenderText("<td>");
			}
		}

		private void EndColumn()
		{
			if (Context.HasSection("endcolumn"))
			{
				Context.RenderSection("endcolumn");
			}
			else
			{
				RenderText("</td>");
			}
		}

		private void StartRow()
		{
			if (Context.HasSection("startrow"))
			{
				Context.RenderSection("startrow");
			}
			else
			{
				RenderText("<tr>");
			}
		}

		private void EndRow()
		{
			if (Context.HasSection("endrow"))
			{
				Context.RenderSection("endrow");
			}
			else
			{
				RenderText("</tr>");
			}
		}

		private void StartTable()
		{
			if (Context.HasSection("start"))
			{
				Context.RenderSection("start");
			}
			else
			{
				RenderText("<table cellpadding='5'>");
			}
		}

		private void NoElements()
		{
			if (Context.HasSection("empty"))
			{
				Context.RenderSection("empty");
			}
			else
			{
				if (dontRenderUneededTableForEmptyLists)
				{
					RenderText("Empty");
				}
				else
				{
					StartRow();
					StartColumn();
					RenderText("Empty");
					EndColumn();
					EndRow();
				}
			}
		}

		private void EndTable()
		{
			if (Context.HasSection("endblock"))
			{
				Context.RenderSection("endblock");
			}
			else
			{
				RenderText("</table>");
			}
		}
	}
}
