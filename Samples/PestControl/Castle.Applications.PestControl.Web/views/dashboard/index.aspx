<%@ Page CodeBehind="index.aspx.cs" Language="c#" AutoEventWireup="false" Inherits="Castle.Applications.PestControl.Web.Views.Dashboard.Index" %>
<%@ Register TagPrefix="pcontrol" TagName="Header" Src="../../views/header.ascx" %>
<%@ Register TagPrefix="pcontrol" TagName="Footer" Src="../../views/footer.ascx" %>

<pcontrol:Header runat=server title="Dashboard" />

<p>
<font size="+1">Welcome to Dashboard</font>
</p>

<p align=left>
<a href="../project/new.rails">Add new Project</a>
</p>


<pcontrol:Footer runat=server />
