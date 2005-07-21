<%@ Page language="C#" %>
<%@ Implements interface="Castle.MonoRail.Framework.IControllerAware" %>
<script runat="server" language="C#">
	protected Castle.MonoRail.Framework.Controller _controller;
	
	public void SetController(Castle.MonoRail.Framework.Controller controller)
	{
		_controller = controller;
	}
</script>
<%= _controller.Context.LastException.Message %>