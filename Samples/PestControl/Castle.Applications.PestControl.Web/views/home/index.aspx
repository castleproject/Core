<%@ Page CodeBehind="index.aspx.cs" Language="c#" AutoEventWireup="false" Inherits="Castle.Applications.PestControl.Web.Views.Home.Index" %>

	    	
    <p>If you'd like to administer projects, please log on.</p>
    <p>Of course, you have to <a href="../registration/signup.rails">register</a> first.</p>
    
    <% if (ErrorMessage != null) { %>
       <p>
		<font color="red"><%= ErrorMessage %></font>
	   </p>
    <% } %>
    
		<form runat="server">

			<TABLE class="formtable"  BORDER="0" CELLSPACING="6" CELLPADDING="2">
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
						<asp:Button ID="LoginIn" OnClick="OnLogin" Runat="server" Text="Log In"></asp:Button>
					</TD>
				</TR>
			</TABLE>
			<p>
			If you'd like to check the status of projects, check the <a href="../dashboard/index.rails">
				Dashboard</a>.
				</p>
		</form>
				

