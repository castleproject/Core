<%@ Page language="c#" Codebehind="helpme.aspx.cs" AutoEventWireup="false" Inherits="TestSite.views.helper.helpme" %>
<%@ Register tagprefix="rails" namespace="Castle.CastleOnRails.Framework.Views.Aspx" assembly="Castle.CastleOnRails.Framework" %>
<%@ Import namespace="Castle.CastleOnRails.Framework.Helpers" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>helpme</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
		
			<rails:InvokeHelper Name="DateFormatHelper" Method="FormatDate" Arg="<%# DateTime.Now %>" runat="server"/>
			
		<%=
			((DateFormatHelper)_controller.Helpers["DateFormatHelper"]).FormatDate( DateTime.Now )
		%>
		</form>
	</body>
</HTML>
