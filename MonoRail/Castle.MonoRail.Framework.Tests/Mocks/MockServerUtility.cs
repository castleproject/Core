namespace Castle.MonoRail.Framework.Tests.Mocks
{
	using System;

	public class MockServerUtility : IServerUtility
	{
		public string MapPath(string virtualPath)
		{
			throw new NotImplementedException();
		}

		public string HtmlEncode(string content)
		{
			throw new NotImplementedException();
		}

		public string UrlEncode(string content)
		{
			return content.Replace("&", "&amp;");
		}

		public string UrlPathEncode(string content)
		{
			throw new NotImplementedException();
		}

		public string JavaScriptEscape(string content)
		{
			throw new NotImplementedException();
		}
	}
}
