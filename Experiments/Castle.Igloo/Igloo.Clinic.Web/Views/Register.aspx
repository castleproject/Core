<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="Igloo.Clinic.Web.Views.Register" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Register</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<TABLE class="formtable" BORDER="0" CELLSPACING="6" CELLPADDING="2">
			<TR>
				<TD>Name :</TD>
				<TD><asp:TextBox ID="TextBoxName" Text="No" Runat="server" /></TD>
			</TR>
			<TR>
				<TD>Login :</TD>
				<TD><asp:TextBox ID="TextBoxLogin" Text="No" Runat="server" /></TD>
			</TR>
			<TR>
				<TD>Password :</TD>
				<TD><asp:TextBox ID="TextBoxPassword" Text="No" Runat="server" /></TD>
			</TR>
			<TR>
				<TD colspan="2" align="center">
					<hr noshade>
					<asp:Button ID="ButtonRegister" CommandName="register" Runat="server" Text="Register" OnClick="ButtonRegister_Click" />
				</TD>
			</TR>
		</TABLE>
        <asp:Literal ID="LiteralMessage" runat="server"></asp:Literal>
    </div>
    </form>
</body>
</html>
