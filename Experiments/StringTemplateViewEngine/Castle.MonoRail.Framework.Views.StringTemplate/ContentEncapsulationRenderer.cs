using System;

namespace Castle.MonoRail.Framework.Views.StringTemplate
{
	using antlr.stringtemplate;

	public class ContentEncapsulationRenderer : AttributeRenderer
	{
		public ContentEncapsulationRenderer()
		{
		}

		public string ToString(object o)
		{
			ContentEncapsulation content = (ContentEncapsulation) o;
			return content.Content;
		}
	}
}
