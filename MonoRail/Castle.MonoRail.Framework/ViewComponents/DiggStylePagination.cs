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
	using System.IO;
	using Castle.MonoRail.Framework.Helpers;

	/// <summary>
	/// Creates a digg style pagination.
	/// 
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
	/// <c>prev</c>: text displayed instead of "&lt;%lt;prev"
	/// <c>next</c>: text displayed instead of "next&gt;%gt;"
	/// </para>
	/// </summary>
	/// 
	/// <remarks>
	/// Based on Alex Henderson work. See 
	/// (Monorail Pagination with Base4.Net)
	/// http://blog.bittercoder.com/PermaLink,guid,579711a8-0b16-481b-b52b-ebdfa1a7e225.aspx
	/// </remarks>
	public class DiggStylePagination : ViewComponent
	{
		private const string StartSection = "startblock";
		private const string EndSection = "endblock";
		private const string LinkSection = "link";
		private const string NextSection = "next";
		private const string PrevSection = "prev";

		private int adjacents = 2;
		private bool useInlineStyle = true;
		private bool renderIfOnlyOnePage = true;
		private string url;
		private string paginatefunction;
		private IPaginatedPage page;

		[ViewComponentParam(Required = true)]
		public IPaginatedPage Page
		{
			get { return page; }
			set { page = value; }
		}

		[ViewComponentParam]
		public string PaginateFunction
		{
			get { return paginatefunction; }
			set { paginatefunction = value; }
		}

		[ViewComponentParam]
		public int Adjacents
		{
			get { return adjacents; }
			set { adjacents = value; }
		}

		[ViewComponentParam]
		public bool UseInlineStyle
		{
			get { return useInlineStyle; }
			set { useInlineStyle = value; }
		}

		[ViewComponentParam]
		public bool RenderIfOnlyOnePage
		{
			get { return renderIfOnlyOnePage; }
			set { renderIfOnlyOnePage = value; }
		}

		[ViewComponentParam]
		public string Url
		{
			get { return url; }
			set { url = value; }
		}

		/// <summary>
		/// Called by the framework once the component instance
		/// is initialized
		/// </summary>
		public override void Initialize()
		{
			if (page == null)
			{
				throw new ViewComponentException("The DiggStylePagination requires a view component " +
					"parameter named 'page' which should contain 'IPaginatedPage' instance");
			}

			if (url == null)
			{
				url = RailsContext.Request.FilePath;
			}

			// So when we render the blocks, the user might access the page
			PropertyBag["page"] = page;
		}

		public override bool SupportsSection(string name)
		{
			return name == StartSection || name == EndSection || name == LinkSection || name == PrevSection || name == NextSection;
		}

		/// <summary>
		/// Called by the framework so the component can 
		/// render its content
		/// </summary>
		public override void Render()
		{
			if (renderIfOnlyOnePage || page.LastIndex > 1)
			{
				StringWriter writer = new StringWriter();

				StartBlock(writer);
				WritePrev(writer);

				if (page.LastIndex < (4 + (adjacents * 2))) // not enough links to make it worth breaking up
				{
					WriteNumberedLinks(writer, 1, page.LastIndex);
				}
				else
				{
					if ((page.LastIndex - (adjacents * 2) > page.CurrentIndex) && // in the middle
						(page.CurrentIndex > (adjacents * 2)))
					{
						WriteNumberedLinks(writer, 1, 2);
						WriteElipsis(writer);
						WriteNumberedLinks(writer, page.CurrentIndex - adjacents, page.CurrentIndex + adjacents);
						WriteElipsis(writer);
						WriteNumberedLinks(writer, page.LastIndex - 1, page.LastIndex);
					}
					else if (page.CurrentIndex < (page.LastIndex / 2))
					{
						WriteNumberedLinks(writer, 1, 2 + (adjacents * 2));
						WriteElipsis(writer);
						WriteNumberedLinks(writer, page.LastIndex - 1, page.LastIndex);
					}
					else // at the end
					{
						WriteNumberedLinks(writer, 1, 2);
						WriteElipsis(writer);
						WriteNumberedLinks(writer, page.LastIndex - (2 + (adjacents * 2)), page.LastIndex);
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
			WriteLink(writer, page.PreviousIndex, caption, !page.HasPrevious);
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
			WriteLink(writer, page.NextIndex, caption, !page.HasNext);
		}

		private void StartBlock(StringWriter writer)
		{
			if (Context.HasSection(StartSection))
			{
				Context.RenderSection(StartSection, writer);
			}
			else
			{
				if (useInlineStyle)
				{
					writer.Write("<div style=\"padding: 3px; margin: 3px; text-align: right; \">\r\n");
				}
				else
				{
					writer.Write("<div class=\"pagination\">\r\n");
				}
			}
		}

		private void EndBlock(StringWriter writer)
		{
			if (Context.HasSection(EndSection))
			{
				Context.RenderSection(EndSection, writer);
			}
			else
			{
				writer.Write("\r\n</div>\r\n");
			}

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
				if (useInlineStyle)
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
			if (index == page.CurrentIndex)
			{
				if (useInlineStyle)
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

		protected void WritePageLink(TextWriter writer, int pageIndex, String text, IDictionary htmlAttributes)
		{
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

				if (paginatefunction != null)
				{
					href = "javascript:" + paginatefunction + "(" + pageIndex + ");void(0);";
				}
				else
				{
					string separator = "?";
					if (url.IndexOf('?') > 0)
						separator = "&";

					href = url + separator + "page=" + pageIndex;
				}

				if (useInlineStyle)
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
