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

	/// <summary>
	/// Pendent
	/// </summary>
	[ViewComponentDetails("SelectStylePagination", Sections="startblock,endblock,first,last,next,prev,link,select")]
	public class SelectStylePagination : AbstractPaginationViewComponent
	{
		private const string NextSection = "next";
		private const string PrevSection = "prev";
		private const string FirstSection = "first";
		private const string LastSection = "last";
		private const string LinkSection = "link";
		private const string SelectSection = "select";

		/// <summary>
		/// Called by the framework so the component can 
		/// render its content
		/// </summary>
		public override void Render()
		{
			StringWriter writer = new StringWriter();

			StartBlock(writer);
			WriteFirst(writer);
			WritePrev(writer);

			WriteSelect(writer);

			WriteNext(writer);
			WriteLast(writer);
			EndBlock(writer);

			RenderText(writer.ToString());
		}

		private void WriteSelect(StringWriter writer)
		{
			if (Context.HasSection(SelectSection))
			{
				Context.RenderSection(SelectSection, writer);
			}
			else
			{
				writer.WriteLine("<select onchange=\"window.location.href = this.options[this.selectedIndex].value;\">");

				for (int i = 1; i <= Page.TotalPages; i++)
				{
					string addition = Page.CurrentPageIndex == i ? " selected=\"true\"" : "";
					writer.WriteLine("\t<option value=\"" + CreateUrlForPage(i) + "\"" + addition +">Page " + i + "</option>");
				}
				
				writer.WriteLine("</select>");
			}
		}

		private void WriteFirst(StringWriter writer)
		{
			string caption = "&laquo;&laquo;";

			if (Context.HasSection(FirstSection))
			{
				TextWriter capWriter = new StringWriter();
				Context.RenderSection(FirstSection, capWriter);
				caption = capWriter.ToString().Trim();
			}

			WriteLink(writer, 1, caption, !Page.HasPreviousPage);
		}

		private void WriteLast(StringWriter writer)
		{
			string caption = "&raquo;&raquo;";

			if (Context.HasSection(LastSection))
			{
				TextWriter capWriter = new StringWriter();
				Context.RenderSection(LastSection, capWriter);
				caption = capWriter.ToString().Trim();
			}

			WriteLink(writer, Page.TotalPages, caption, !Page.HasNextPage);
		}

		private void WritePrev(StringWriter writer)
		{
			string caption = "&laquo;";
			
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
			string caption = "&raquo;";
			
			if (Context.HasSection(NextSection))
			{
				TextWriter capWriter = new StringWriter();
				Context.RenderSection(NextSection, capWriter);
				caption = capWriter.ToString().Trim();
			}
			
			WriteLink(writer, Page.NextPageIndex, caption, !Page.HasNextPage);
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
					href = "javascript:" + PaginateFunction + "(" + pageIndex + ");void(0);";
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
