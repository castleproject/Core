<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Igloo.Clinic.Web.Views.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Login</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    			<TABLE class="formtable" BORDER="0" CELLSPACING="6" CELLPADDING="2">
				<TR>
					<TD>Login :</TD>
					<TD><asp:TextBox ID="login" Text="No" Runat="server" /></TD>
				</TR>
				<TR>
					<TD>Password :</TD>
					<TD><asp:TextBox ID="password" Text="No" Runat="server" /></TD>
				</TR>
				<TR>
					<TD colspan="2" align="center">
						<hr noshade>
						<asp:Button ID="ButtonLogin" CommandName="login" Runat="server" Text="Log In" OnClick="ButtonLogin_Click"/>
					</TD>
				</TR>
			</TABLE>
    </div>
        <asp:Literal ID="LiteralMessage" runat="server"></asp:Literal>
    </form>
</body>
</html>
