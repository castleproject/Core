<%@ Page language="c#" Codebehind="actionto.aspx.cs" AutoEventWireup="false" Inherits="TestSite.views.registration.actionto" %>
<%@ Register tagprefix="rails" namespace="Castle.CastleOnRails.Framework.Views.Aspx" assembly="Castle.CastleOnRails.Framework" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>actionto</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<rails:ActionTo id="actto" Controller="Registration" Action="ActionTo" runat="server" FormID="Form1" />
			<asp:Button ID="Save" Runat="server" Text="Save" OnClick="DoAPostBack" />
			<%= Context.Items["errormessage"] %>
		</form>
	</body>
</HTML>
