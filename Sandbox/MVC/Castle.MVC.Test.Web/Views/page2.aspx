<%@ Page language="c#" Codebehind="page2.aspx.cs" AutoEventWireup="false" Inherits="Castle.MVC.Test.Web.Views.page2" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>page2</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</HEAD>
	<body MS_POSITIONING="FlowLayout">
		<form id="Form1" method="post" runat="server">
			<P>
				Ok
				<asp:Button id="Button" runat="server" CommandName="GoToIndex" Text="Goto index"></asp:Button></P>
			<P>Previous view :
				<asp:Label id="LabelPreviousView" runat="server">LabelPreviousView</asp:Label></P>
		</form>
	</body>
</HTML>
