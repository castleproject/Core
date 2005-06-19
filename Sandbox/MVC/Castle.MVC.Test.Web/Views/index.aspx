<%@ Page language="c#" Codebehind="index.aspx.cs" AutoEventWireup="false" Inherits="Castle.MVC.Test.Web.Views.index" %>
<%@ Register TagPrefix="uc1" TagName="MyUserControl" Src="MyUserControl.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>index</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</HEAD>
	<body MS_POSITIONING="FlowLayout">
		<form id="Form1" method="post" runat="server">
			<TABLE class="formtable" BORDER="0" CELLSPACING="6" CELLPADDING="2">
				<TR>
					<TD>E-mail:</TD>
					<TD><asp:TextBox ID="email" Runat="server" /></TD>
				</TR>
				<TR>
					<TD>Password:</TD>
					<TD><asp:TextBox ID="passwd" Runat="server" /></TD>
				</TR>
				<TR>
					<TD colspan="2" align="center">
						<hr noshade>
						<asp:Button ID="ButtonLogin" CommandName="GoToPage2" CommandArgument="UnknownArgument" Runat="server"
							Text="Log In"></asp:Button>
					</TD>
				</TR>
			</TABLE>
			<P>
				<asp:LinkButton id="LinkToPage2" runat="server" CommandArgument="arg1" CommandName="GoToPage2">LinkButton</asp:LinkButton></P>
			<P>
				<uc1:MyUserControl id="MyUserControl" runat="server"></uc1:MyUserControl></P>
		</form>
	</body>
</HTML>
