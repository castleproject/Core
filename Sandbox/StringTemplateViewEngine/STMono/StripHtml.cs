using System;

namespace STMono
{
	using antlr.stringtemplate;

	public class StripHtml : AttributeRenderer
	{
		public StripHtml()
		{
		}

		public string ToString(object o)
		{
			if (o != null) return o.ToString().Replace("<", "&lt;").Replace(">", "&gt;");
			else return "";
		}
	}
}
