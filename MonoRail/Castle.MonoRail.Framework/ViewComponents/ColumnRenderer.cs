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
	/// Renders a table where each nested content is rendered on a cell. 
	/// <para>
	/// For example, suppose you have a dynamic list of items and what to display
	/// them side by side, in four columns. As the number of elements in unknown 
	/// in development time, you can use the ColumnRenderer to 
	/// create the table and cells.
	/// </para>
	/// </summary>
	/// 
	/// <example>
	/// The following example uses nvelocity view engine syntax.
	/// <code>
	/// <![CDATA[
	/// #blockcomponent(ColumnRenderer with "items=$interests")
	/// 
	/// #firstelement
	///   Custom first element
	/// #end
	/// 
	/// #item
	///   Content is $item
	/// #end
	/// 
	/// #end
	/// ]]>
	/// </code>
	/// <para>
	/// Which should render something like:
	/// </para>
	/// <code>
	/// <![CDATA[
	/// <table> 
	///   <tr>
	///     <td>
	///       Custom first element
	///     </td>
	///     <td>
	///       Content is Tennis
	///     </td>
	///     <td>
	///       Content is Soccer
	///     </td>
	///   </tr>
	///   <tr>
	///     <td>
	///       Content is Voleyball
	///     </td>
	///   </tr>
	/// </table>
	/// ]]>
	/// </code>
	/// </example>
	/// 
	/// <remarks>
	/// The following sections are supported. Only the <c>item</c> section must be always provided. <br/>
	/// 
	/// <para>
	/// <c>start</c>: override it in order to create the table yourself <br/>
	/// <c>endblock</c>: override it in order to end the table <br/>
	/// 
	/// <c>startrow</c>: override it in order to start the column <br/>
	/// <c>endrow</c>: override it in order to end the column <br/>
	/// 
	/// <c>startcolumn</c>: override it in order to start the cell <br/>
	/// <c>endcolumn</c>: override it in order to end the cell <br/>
	/// 
	/// <c>item</c>: must be overriden in order to display the item content (unless it's something trivial like a primitive) <br/>
	/// <c>empty</c>: section used when the <see cref="Items"/> is empty  <br/>
	/// <c>firstelement</c>: if provided, will be rendered before any cells  <br/>
	/// </para>
	/// 
	/// <para>
	/// The number of columns defaults to three. 
	/// </para>
	/// </remarks>
	public class ColumnRenderer : ViewComponent
	{
		private int cols = 3;
		private IEnumerable enumerable;
		private bool dontRenderUneededTableForEmptyLists = false;

		/// <summary>
		/// Gets or sets the number of columns to display.
		/// </summary>
		/// <value>The cols.</value>
		[ViewComponentParam]
		public int Cols
		{
			get { return cols; }
			set { cols = value; }
		}

		/// <summary>
		/// Gets or sets the items to show.
		/// </summary>
		/// <value>The items.</value>
		[ViewComponentParam(Required = true)]
		public IEnumerable Items
		{
			get { return enumerable; }
			set { enumerable = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the component should render a table
		/// even if there are no elements on the <see cref="Items"/>.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if it should not render; otherwise, <c>false</c>.
		/// </value>
		[ViewComponentParam("emptyness")]
		public bool DontRenderUneededTableForEmptyLists
		{
			get { return dontRenderUneededTableForEmptyLists; }
			set { dontRenderUneededTableForEmptyLists = value; }
		}

		/// <summary>
		/// Called by the framework once the component instance
		/// is initialized
		/// </summary>
		public override void Initialize()
		{
			if (cols <= 0)
			{
				throw new ViewComponentException("ColumnRenderer: 'cols' parameter must be greater than zero");
			}
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
