<%@ Page language="c#" Codebehind="postback.aspx.cs" AutoEventWireup="false" Inherits="TestSite.views.registration.postback" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>postback</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			Post Count: 
			<asp:Label ID="postCount" Runat="server"></asp:Label><br>
			<asp:Button ID="Save" Runat="server" Text="Save" OnClick="DoAPostBack" />
		</form>
	</body>
</HTML>
