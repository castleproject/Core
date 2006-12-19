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
	/// </para>
	/// 
	/// <para>
	/// Supported sections: <br/>
	/// <c>link</c>: invoked with <c>pageIndex</c>, <c>url</c> and <c>text</c>
	/// so you can build a custom link
	/// </para>
	/// </summary>
	/// 
	/// <remarks>
	/// Based on Alex Henderson work. See 
	/// http://blog.bittercoder.com/CategoryView,category,castle.aspx
	/// </remarks>
	public class DiggStylePagination : ViewComponent
	{
		private int adjacents = 2;
		private string url;
		private IPaginatedPage page;
		private IDictionary queryString = null;

		/// <summary>
		/// Called by the framework once the component instance
		/// is initialized
		/// </summary>
		public override void Initialize()
		{
			if (ComponentParams.Contains("adjacents"))
			{
				adjacents = Convert.ToInt32(ComponentParams["adjacents"]);
			}

			if (ComponentParams.Contains("url"))
			{
				url = ComponentParams["url"].ToString();
			}
			else
			{
				url = RailsContext.Request.FilePath;
			}

			page = (IPaginatedPage) ComponentParams["page"];

			if (page == null)
			{
				throw new ViewComponentException("The DiggStylePagination requires a view component " + 
					"parameter named 'page' which should contain 'IPaginatedPage' instance");
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
				WriteNumberedLinks(writer, page, 1, page.LastIndex, queryString);
			}
			else
			{
				if ((page.LastIndex - (adjacents * 2) > page.CurrentIndex) && // in the middle
				    (page.CurrentIndex > (adjacents * 2)))
				{
					WriteNumberedLinks(writer, page, 1, 2, queryString);

					WriteElipsis(writer);

					WriteNumberedLinks(writer, page, page.CurrentIndex - adjacents, page.CurrentIndex + adjacents, queryString);

					WriteElipsis(writer);

					WriteNumberedLinks(writer, page, page.LastIndex - 1, page.LastIndex, queryString);
				}

				else if (page.CurrentIndex < (page.LastIndex / 2))
				{
					WriteNumberedLinks(writer, page, 1, 2 + (adjacents * 2), queryString);

					WriteElipsis(writer);

					WriteNumberedLinks(writer, page, page.LastIndex - 1, page.LastIndex, queryString);
				}

				else // at the end
				{
					WriteNumberedLinks(writer, page, 1, 2, queryString);

					WriteElipsis(writer);

					WriteNumberedLinks(writer, page, page.LastIndex - (2 + (adjacents * 2)), page.LastIndex, queryString);
				}
			}

			WriteNext(writer);

			EndBlock(writer);

			RenderText(writer.ToString());
		}

		private void WritePrev(StringWriter writer)
		{
			WriteLink(writer, page.PreviousIndex, "« prev", !page.HasPrevious, queryString);
		}

		private void WriteNext(StringWriter writer)
		{
			WriteLink(writer, page.NextIndex, "next »", !page.HasNext, queryString);
		}

		private void StartBlock(StringWriter writer)
		{
			writer.Write("<div class=\"pagination\">\r\n");
		}

		private void EndBlock(StringWriter writer)
		{
			writer.Write("\r\n</div>\r\n");
		}

		private void WriteElipsis(TextWriter writer)
		{
			writer.Write("...");
		}

		private void WriteNumberedLinks(TextWriter writer, IPaginatedPage page, int startIndex, int endIndex,
		                                IDictionary queryString)
		{
			for(int i = startIndex; i <= endIndex; i++)
			{
				WriteNumberedLink(writer, page, i, queryString);
			}
		}

		private void WriteLink(TextWriter writer, int index, string text, bool disabled, IDictionary queryString)
		{
			if (disabled)
			{
				writer.Write(String.Format("<span class=\"disabled\">{0}</span>", text));
			}

			else
			{
				WritePageLink(writer, index, text, null, queryString);
			}
		}

		private void WriteNumberedLink(TextWriter writer, IPaginatedPage page, int index, IDictionary queryString)
		{
			if (index == page.CurrentIndex)
			{
				writer.Write(String.Format("\r\n<span class=\"current\">{0}</span>\r\n", index));
			}

			else
			{
				WritePageLink(writer, index, index.ToString(), null, queryString);
			}
		}

		protected void WritePageLink(TextWriter writer, int pageIndex, String text,
		                             IDictionary htmlAttributes, IDictionary queryString)
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
				writer.Write(String.Format("<a href=\"{0}?page={1}\">{2}</a>\r\n", url, pageIndex, text));
			}
		}
	}
}