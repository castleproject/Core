<%@ Implements Interface="Castle.MonoRail.Framework.IControllerAware" %>
<%@ Import Namespace="Castle.MonoRail.Framework" %>
<script runat=server>

	private Controller _controller;

	public void SetController(Controller controller)
	{
		_controller = controller;
	}

</script>
Customer is <%= _controller.PropertyBag["CustomerName"] %>
<br>
<% 
   String[] values = (String[]) _controller.PropertyBag["List"];
   foreach(String value in values) 
   {
      Response.Write(value);
   }
%>