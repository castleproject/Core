<%@ Page CodeBehind="index.aspx.cs" Language="c#" AutoEventWireup="false" Inherits="AspnetSample.views.registration.index" %>
<HTML>
	<body>
		<form runat="server">
			<TABLE WIDTH="300" ALIGN="center" BORDER="1" CELLSPACING="1" CELLPADDING="1">
				<TR>
					<TD>Name</TD>
					<TD><asp:TextBox ID="name" Runat="server" /></TD>
				</TR>
				<TR>
					<TD>Address</TD>
					<TD><asp:TextBox ID="address" Runat="server" /></TD>
				</TR>
				<TR>
					<TD>City</TD>
					<TD><asp:TextBox ID="city" Runat="server" /></TD>
				</TR>
				<TR>
					<TD>Country</TD>
					<TD><asp:TextBox ID="country" Runat="server" /></TD>
				</TR>
				<TR>
					<TD>Age</TD>
					<TD><asp:TextBox ID="age" Runat="server" /></TD>
				</TR>
				<TR>
					<TD colspan="2"><asp:Button ID="Save" Text="Save" Runat="server" OnClick="OnSave" /></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
