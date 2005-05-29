// ${Copyrigth}

namespace Castle.MonoRail.Framework.Internal.Test
{
	using System;

	
	public class ServerUtilityImpl : IServerUtility
	{
		public ServerUtilityImpl()
		{
		}

		public string HtmlEncode(string content)
		{
			return content.Replace("<", "&lt;").Replace(">", "&gt;");
		}
	}
}
