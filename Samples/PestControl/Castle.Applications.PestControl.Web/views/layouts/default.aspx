<%@ Page Inherits="Castle.MonoRail.Framework.Views.Aspx.MasterPageBase" %>
<%@ Register tagprefix="rails" namespace="Castle.MonoRail.Framework.Views.Aspx" assembly="Castle.MonoRail.Framework" %>
<script language="C#" runat="server">
  
  public String PageTitle
  {
  	// get { return (this as IAttributeAccessor).GetAttribute("title"); }
  	get { return "Home"; }
  }
  
</script>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
	<HEAD>
		<title>PestControl - <%= PageTitle %></title>
        <link href="../../css/def.css" rel="stylesheet" type="text/css">
	</HEAD>
	<body>
		   <div class="titlecontainer">
		     <table border=0 width="98%">
				<tr class="titlecontainer">
					<td>
				     PestControl - <%= PageTitle %>
					</td>
					<td align=right>
					    <% 
						   if (Context.User != null && Context.User.Identity.IsAuthenticated)
					       {
					    %>
						<font size=-1>Logged as <%= Context.User.Identity.Name %></font>
					    <% }
					       else if (!Context.User.Identity.IsAuthenticated) 
						   {
					    %>
						<font size=-1>Not logged on</font>
					    <% } %>
					</td>
				</tr>
		     </table>
		   </div>

		   <div class="container" align="center">
		   <rails:Contents id="contents" runat="server" />
		   </div>
			
	</body>
</HTML>
