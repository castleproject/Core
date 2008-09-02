// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using System.IO;
	using Castle.Components.Pagination;
	using Helpers;

	/// <summary>
	/// Creates a digg style pagination.
	/// <para>
	/// Based on Alex Henderson work. See 
	/// (Monorail Pagination with Base4.Net)
	/// http://blog.bittercoder.com/PermaLink,guid,579711a8-0b16-481b-b52b-ebdfa1a7e225.aspx
	/// </para>
	/// </summary>
	/// 
	/// <remarks>
	/// <para>
	/// Parameters: <br/>
	/// <c>adjacents</c>: number of links to show around the current page <br/>
	/// <c>page</c> (required): <see cref="IPaginatedPage"/> instance (<see cref="PaginationHelper"/>) <br/>
	/// <c>url</c>: url to link to<br/>
	/// <c>useInlineStyle</c>: defines if the outputted content will use inline style or css classnames (defaults to true)<br/>
	/// <c>renderIfOnlyOnePage</c>: should the pagination render if there's only one page (defaults to true)<br/>
	/// <c>paginatefunction</c>: a javascript function name to invoke on the page links (instead of a URL)<br/>
	/// </para>
	/// 
	/// <para>
	/// Supported sections: <br/>
	/// <c>startblock</c>: invoked with <c>page</c> <br/>
	/// <c>endblock</c>: invoked with <c>page</c> <br/>
	/// <c>link</c>: invoked with <c>pageIndex</c>, <c>url</c> and <c>text</c>
	/// so you can build a custom link <br/>
	/// <c>prev</c>: text displayed instead of "&lt;%lt;prev"  <br/>
	/// <c>next</c>: text displayed instead of "next&gt;%gt;"  <br/>
	/// </para>
	/// 
	/// </remarks>
	[ViewComponentDetails("DiggStylePagination", Sections = "startblock,endblock,next,prev,link")]
	public class DiggStylePagination : AbstractPaginationViewComponent
	{
		private const string LinkSection = "link";
		private const string NextSection = "next";
		private const string PrevSection = "prev";

		private int adjacents = 2;
		private bool renderIfOnlyOnePage = true;

		/// <summary>
		/// Gets or sets the adjacents (number of links to show).
		/// </summary>
		/// <value>The adjacents.</value>
		[ViewComponentParam]
		public int Adjacents
		{
			get { return adjacents; }
			set { adjacents = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the component should render links even if there is only one page.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if it should render; otherwise, <c>false</c>.
		/// </value>
		[ViewComponentParam]
		public bool RenderIfOnlyOnePage
		{
			get { return renderIfOnlyOnePage; }
			set { renderIfOnlyOnePage = value; }
		}

		/// <summary>
		/// Called by the framework so the component can 
		/// render its content
		/// </summary>
		public override void Render()
		{
			if (renderIfOnlyOnePage || Page.TotalPages > 1)
			{
				StringWriter writer = new StringWriter();

				StartBlock(writer);
				WritePrev(writer);

				if (Page.TotalPages < (4 + (adjacents * 2))) // not enough links to make it worth breaking up
				{
					WriteNumberedLinks(writer, 1, Page.TotalPages);
				}
				else
				{
					if ((Page.TotalPages - (adjacents * 2) > Page.CurrentPageIndex) && // in the middle
						(Page.CurrentPageIndex > (adjacents * 2)))
					{
						WriteNumberedLinks(writer, 1, 2);
						WriteElipsis(writer);
						WriteNumberedLinks(writer, Page.CurrentPageIndex - adjacents, Page.CurrentPageIndex + adjacents);
						WriteElipsis(writer);
						WriteNumberedLinks(writer, Page.TotalPages - 1, Page.TotalPages);
					}
					else if (Page.CurrentPageIndex < (Page.TotalPages / 2))
					{
						WriteNumberedLinks(writer, 1, 2 + (adjacents * 2));
						WriteElipsis(writer);
						WriteNumberedLinks(writer, Page.TotalPages - 1, Page.TotalPages);
					}
					else // at the end
					{
						WriteNumberedLinks(writer, 1, 2);
						WriteElipsis(writer);
						WriteNumberedLinks(writer, Page.TotalPages - (2 + (adjacents * 2)), Page.TotalPages);
					}
				}

				WriteNext(writer);
				EndBlock(writer);
				RenderText(writer.ToString());
			}
		}

		private void WritePrev(StringWriter writer)
		{
			string caption = "&laquo; prev";
			if (Context.HasSection(PrevSection))
			{
				TextWriter capWriter = new StringWriter();
				Context.RenderSection(PrevSection, capWriter);
				caption = capWriter.ToString().Trim();
			}
			WriteLink(writer, Page.PreviousPageIndex, caption, !Page.HasPreviousPage);
		}

		private void WriteNext(StringWriter writer)
		{
			string caption = "next &raquo;";
			if (Context.HasSection(NextSection))
			{
				TextWriter capWriter = new StringWriter();
				Context.RenderSection(NextSection, capWriter);
				caption = capWriter.ToString().Trim();
			}
			WriteLink(writer, Page.NextPageIndex, caption, !Page.HasNextPage);
		}

		private void WriteElipsis(TextWriter writer)
		{
			writer.Write("&#8230;");
		}

		private void WriteNumberedLinks(TextWriter writer, int startIndex, int endIndex)
		{
			for (int i = startIndex; i <= endIndex; i++)
			{
				WriteNumberedLink(writer, i);
			}
		}

		private void WriteLink(TextWriter writer, int index, string text, bool disabled)
		{
			if (disabled)
			{
				if (UseInlineStyle)
				{
					writer.Write(String.Format("<span style=\"color: #DDD; padding: 2px 5px 2px 5px; margin: 2px; border: 1px solid #EEE;\">{0}</span>", text));
				}
				else
				{
					writer.Write(String.Format("<span class=\"disabled\">{0}</span>", text));
				}
			}
			else
			{
				WritePageLink(writer, index, text, null);
			}
		}

		private void WriteNumberedLink(TextWriter writer, int index)
		{
			if (index == Page.CurrentPageIndex)
			{
				if (UseInlineStyle)
				{
					writer.Write(String.Format("\r\n<span class=\"font-weight: bold; background-color: #000099; color: #FFF; padding: 2px 5px 2px 5px; margin: 2px; border: 1px solid #000099;\">{0}</span>\r\n", index));
				}
				else
				{
					writer.Write(String.Format("\r\n<span class=\"current\">{0}</span>\r\n", index));
				}
			}
			else
			{
				WritePageLink(writer, index, index.ToString(), null);
			}
		}

		/// <summary>
		/// Writes the Page link.
		/// </summary>
		/// <param name="writer">The writer.</param>
		/// <param name="pageIndex">Index of the page.</param>
		/// <param name="text">The text.</param>
		/// <param name="htmlAttributes">The HTML attributes.</param>
		protected void WritePageLink(TextWriter writer, int pageIndex, String text, IDictionary htmlAttributes)
		{
			string url = CreateUrlForPage(pageIndex);

			if (Context.HasSection(LinkSection))
			{
				PropertyBag["pageIndex"] = pageIndex;
				PropertyBag["text"] = text;
				PropertyBag["url"] = url;

				Context.RenderSection(LinkSection, writer);
			}
			else
			{
				String href;

				if (PaginateFunction != null)
				{
					href = "javascript:" + PaginateFunction + "(" + pageIndex + 
						(PaginatefunctionFixedArgs != null ? "," + PaginatefunctionFixedArgs : "") + 
						");void(0);";
				}
				else
				{
					href = url;
				}

				if (UseInlineStyle)
				{
					writer.Write(String.Format("<a style=\"color: #000099;text-decoration: none;padding: 2px 5px 2px 5px;margin: 2px;border: 1px solid #AAAFEE;\" href=\"{0}\">{1}</a>\r\n", href, text));
				}
				else
				{
					writer.Write(String.Format("<a href=\"{0}\">{1}</a>\r\n", href, text));
				}
			}
		}
	}
}
