<%@ Page %>
<%@ Implements Interface="Castle.MonoRail.Framework.IControllerAware" %>
<%@ Import Namespace="Castle.MonoRail.Framework" %>
<script runat=server>

	private Controller _controller;

	public void SetController(Controller controller)
	{
		_controller = controller;
	}

</script>
My View contents for UploadFile\ShowUploadFileName.rails

<h1><%= _controller.PropertyBag["filename"] %></h1>