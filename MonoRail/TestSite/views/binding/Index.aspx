<%@ Page Language="C#" AutoEventWireup="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Controller Binder Examples</title>
</head>
<body>
    <form id="form1" runat="server">
    <h2>Controller Binder Examples</h2>
    <ul>
		<li>
			<asp:HyperLink ID="lnkBasic" NavigateUrl="~/binding/BasicAction.rails" runat="server">Basic Controls</asp:HyperLink>
		</li>
		<li>
			<asp:HyperLink ID="lnkDataBound" NavigateUrl="~/binding/DataBound.rails" runat="server">DataBound Controls</asp:HyperLink>
		</li>
		<li>
			<asp:HyperLink ID="lnkUserControls" NavigateUrl="~/binding/UserControls.rails" runat="server">User Controls</asp:HyperLink>
		</li>	
	</ul>
    </form>
</body>
</html>
