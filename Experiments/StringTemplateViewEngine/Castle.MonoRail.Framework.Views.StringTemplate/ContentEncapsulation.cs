using System;

namespace Castle.MonoRail.Framework.Views.StringTemplate
{
	public class ContentEncapsulation
	{
		private readonly string content;

		public ContentEncapsulation(string content)
		{
			this.content = content;
		}

		public string Content
		{
			get { return content; }
		}
	}
}
