<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="EmployeeControl.ascx.cs" Inherits="AspnetSample.views.EmployeeControl" %>

<table border="1" cellpadding="1" cellspacing="1">
	<tr style="background-color: orange">
		<th align="center">Id</th>
		<th align="center">Name</th>
		<td align="center">Country</td>
	</tr>
	<tr>
		<td><asp:TextBox ID="txtID" Width="50px" runat="server"/></td>
		<td><asp:TextBox ID="txtName" Width="150px" runat="server"/></td>
		<td><asp:TextBox ID="txtCountry" Width="100px" runat="server"/></td>
	</tr>
	<tr>
		<td colspan="3"></td>
	</tr>
	<tr>
		<td colspan="3" align="center">
			<asp:Button ID="btnAdd" runat="server" Text="Add" OnClick="btnAdd_Click" />&nbsp;&nbsp;
			<asp:Button ID="btnRemove" runat="server" Text="Remove" OnClick="btnRemove_Click" />
		</td>
	</tr>
</table>