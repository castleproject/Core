using System;
using Castle.MonoRail.Framework;

namespace <%= Namespace %>
{
	/// <summary>
	/// <%= Name %> controller.
	/// </summary>
	[Layout("<%= FileName %>")]
	public class <%= ClassName %>Controller : ApplicationController
	{
		<% for action in Actions: %>
		public void <%= action %>()
		{
			
		}
		
		<% end %>
	}
}
