<%@ Page language="c#" Codebehind="index.aspx.cs" AutoEventWireup="false" Inherits="TestSite.views.ajax.index" %>
<%@ Register tagprefix="rails" namespace="Castle.CastleOnRails.Framework.Views.Aspx" assembly="Castle.CastleOnRails.Framework" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>index</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<rails:invokehelper id="GetJavascriptFunctions" runat="server" Method="GetJavascriptFunctions" Name="AjaxHelper"></rails:invokehelper>
			<h2>implemented</h2>
			
			<!-- ObserveField --> Observer field example: Please enter the zip code:<br>
			<input id="zip" type="text" name="zip">
			<br>
			<div id="address"></div>
			<rails:invokehelper id=ObserveField_Zip runat="server" Method="ObserveField" Name="AjaxHelper" Args='<%# new object[]{"zip", 2, "inferaddress.rails", "address", "\"aa new content\"" } %>' />
			<hr>
			<h2>not implemented yet</h2>
			
			<!-- ObserveForm -->
			<p>Observer form example: Fill the field below to create an account:<br>
				Name:
				<asp:textbox id="name" runat="server"></asp:textbox><br>
				Address:
				<asp:textbox id="addressf" runat="server"></asp:textbox><br>
				<div id="message"></div>
			<P></P>
			<rails:invokehelper id=ObserveForm runat="server" Method="ObserveForm" Name="AjaxHelper" Args='<%# new object[]{ "Form1", 2, "accountformvalidate.rails", "message", "" } %>' />
			<hr>
			
			<!-- Remote Form -->
			<h4 id="status">Status</h4>
			<p><asp:datagrid id="DataGrid1" runat="server"></asp:datagrid><br>
				<b>Add New User:</b> 
				<!-- $AjaxHelper.BuildFormRemoteTag("AddUserWithAjax.rails", 
			"userlist", null, "$('status').innerHTML = 'Saving...'", null, null, 
			"$('status').innerHTML = 'Done!'") -->
				<table>
					<tr>
						<td>Name:</td>
						<td><input type="text" name="name"></td>
					</tr>
					<tr>
						<td>EMail:</td>
						<td><input type="text" name="email"></td>
					</tr>
					<tr>
						<td colspan="2" align="center">
							<input type="submit" value="Add">
						</td>
					</tr>
				</table>
				<!--</form>--></p>
		</form>
	</body>
</HTML>
