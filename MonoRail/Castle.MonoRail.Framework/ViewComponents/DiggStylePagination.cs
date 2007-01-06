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
	/// <c>paginatefunction</c>: a javascript function name to invoke on the page links (instead of a URL)<br/>
	/// </para>
	/// 
	/// <para>
	/// Supported sections: <br/>
	/// <c>start</c>: invoked with <c>page</c> <br/>
	/// <c>end</c>: invoked with <c>page</c> <br/>
	/// <c>link</c>: invoked with <c>pageIndex</c>, <c>url</c> and <c>text</c> 
	/// so you can build a custom link <br/>
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
		private int adjacents = 2;
		private bool useInlineStyle = true;
		private string url;
		private string paginatefunction;
		private IPaginatedPage page;

		/// <summary>
		/// Called by the framework once the component instance
		/// is initialized
		/// </summary>
		public override void Initialize()
		{
			page = (IPaginatedPage) ComponentParams["page"];

			if (page == null)
			{
				throw new ViewComponentException("The DiggStylePagination requires a view component " + 
					"parameter named 'page' which should contain 'IPaginatedPage' instance");
			}

			paginatefunction = (string) ComponentParams["paginatefunction"];

			if (ComponentParams.Contains("adjacents"))
			{
				adjacents = Convert.ToInt32(ComponentParams["adjacents"]);
			}

			if (ComponentParams.Contains("useInlineStyle"))
			{
				useInlineStyle = Convert.ToBoolean(ComponentParams["useInlineStyle"]);
			}

			if (ComponentParams.Contains("url"))
			{
				url = ComponentParams["url"].ToString();
			}
			else
			{
				url = RailsContext.Request.FilePath;
			}

			// So when we render the blocks, the user might access the page
			PropertyBag["page"] = page;
		}

		/// <summary>
		/// Called by the framework so the component can 
		/// render its content
		/// </summary>
		public override void Render()
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

		/// <summary>
		/// Implementor should return true only if the 
		/// <c>name</c> is a known section the view component
		/// supports.
		/// </summary>
		/// <param name="name">section being added</param>
		/// <returns><see langword="true"/> if section is supported</returns>
		public override bool SupportsSection(string name)
		{
			return "start" == name || "end" == name || "link" == name;
		}

		private void WritePrev(StringWriter writer)
		{
			WriteLink(writer, page.PreviousIndex, "« prev", !page.HasPrevious);
		}

		private void WriteNext(StringWriter writer)
		{
			WriteLink(writer, page.NextIndex, "next »", !page.HasNext);
		}

		private void StartBlock(StringWriter writer)
		{
			if (Context.HasSection("start"))
			{
				Context.RenderSection("start", writer);
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
			if (Context.HasSection("end"))
			{
				Context.RenderSection("end", writer);
			}
			else
			{
				writer.Write("\r\n</div>\r\n");
			}
			
		}

		private void WriteElipsis(TextWriter writer)
		{
			writer.Write("...");
		}

		private void WriteNumberedLinks(TextWriter writer, int startIndex, int endIndex)
		{
			for(int i = startIndex; i <= endIndex; i++)
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
			if (Context.HasSection("link"))
			{
				PropertyBag["pageIndex"] = pageIndex;
				PropertyBag["text"] = text;
				PropertyBag["url"] = url;

				Context.RenderSection("link", writer);
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
					href = url + "?page=" + pageIndex;
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
