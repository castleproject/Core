<%@ Page Inherits="Castle.MonoRail.Framework.Views.Aspx.MasterPageBase" %>
<%@ Register tagprefix="castle" namespace="Castle.MonoRail.Framework.Views.Aspx" assembly="Castle.MonoRail.Framework" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html>
	<head>
		<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
		<title>Layout</title>
		<style type="text/css" media="screen">
body
{
	background-color: white;
	font-family: Verdana, Geneva, Arial, Helvetica, sans-serif;
	font-size: small;
}		
		</style>
	</head>

	<body>

<castle:Contents id="contents" runat="server" />

	</body>
</html>
