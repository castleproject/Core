<%@ Page CodeBehind="signup.aspx.cs" Language="c#" AutoEventWireup="false" Inherits="Castle.Applications.PestControl.Web.Views.Registration.Index" %>

<form runat="server">
	<p>
		Please fill the fields below to create an account.
	</p>
	<asp:ValidationSummary runat="server" id="ValidationSummary1" />
	<TABLE class="formtable" width="330" BORDER="0" CELLSPACING="6" CELLPADDING="2">
		<TR>
			<TD width="30%">E-mail:</TD>
			<TD><asp:TextBox ID="email" Runat="server" /></TD>
		</TR>
		<TR>
			<TD>Name/Nickname:</TD>
			<TD><asp:TextBox ID="name" Runat="server" /></TD>
		</TR>
		<TR>
			<TD>Password:</TD>
			<TD><asp:TextBox ID="passwd" Runat="server" /></TD>
		</TR>
		<TR>
			<TD>Password confirmation:</TD>
			<TD><asp:TextBox ID="passwd2" Runat="server" /></TD>
		</TR>
		<TR>
			<TD colspan="2" align="center">
				<hr noshade>
				<asp:Button ID="Save" Runat="server" Text="Register"></asp:Button>
			</TD>
		</TR>
	</TABLE>
</form>
