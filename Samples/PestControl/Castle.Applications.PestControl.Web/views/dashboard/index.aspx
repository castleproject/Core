<%@ Page CodeBehind="index.aspx.cs" Language="c#" AutoEventWireup="false" Inherits="Castle.Applications.PestControl.Web.Views.Dashboard.Index" %>
<%@ Register TagPrefix="pcontrol" TagName="Header" Src="../../views/header.ascx" %>
<%@ Register TagPrefix="pcontrol" TagName="Footer" Src="../../views/footer.ascx" %>

<pcontrol:Header runat=server title="Dashboard" />

<p>
<font size="+1">Welcome to Dashboard</font>
</p>

<p align=left>
<a href="../project/new.rails">Add new Project</a>
</p>

<asp:Repeater id="projectsRepeater" runat="server">
<ItemTemplate>
  <p align=left>
  <table width="100%" border=0 cellpadding=3 cellspacing=0>
	<tr>
	  <td><strong>Project <%# DataBinder.Eval(Container, "DataItem.Name") %></strong></td>
	</tr>
	<tr>
	  <td>Current state: Good!</td>
	</tr>
	<tr>
	  <td>Last 5 builds (View all):</td>
	</tr>
	<tr>
	  <td>
		<table width="100%" border=1 cellpadding=3 cellspacing=0>
			<tr>
			<td><b>At</b></td>
			<td><b>Result</b></td>
			</tr>
			<tr>
			<td>12/12/2004 - 12:30</td>
			<td>Successful</td>
			</tr>
		</table>
	  </td>
	</tr>
  </table>
  <hr noshade>
  </p>
</ItemTemplate>
</asp:Repeater>


<pcontrol:Footer runat=server />
