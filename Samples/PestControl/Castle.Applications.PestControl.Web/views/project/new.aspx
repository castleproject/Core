<%@ Page CodeBehind="new.aspx.cs" Language="c#" AutoEventWireup="false" Inherits="Castle.Applications.PestControl.Web.views.project.New" %>

<form runat="server" ID="Form1" method=post>
	<p>
		Please fill the fields below to create a new project
	</p>
	<asp:ValidationSummary runat="server" id="ValidationSummary1" />
	<TABLE class="formtable" width="420" BORDER="0" CELLSPACING="6" CELLPADDING="2">
		<TR>
			<TD width="40%">Project Name:</TD>
			<TD><asp:TextBox ID="name" Runat="server" /> <asp:CheckBox ID="isPublic" Runat="server" Text="Public" /></TD>
		</TR>
		<TR>
			<TD>Build System:</TD>
			<TD><asp:DropDownList ID="bs" Runat="server" /></TD>
		</TR>
		<TR>
			<TD colspan="2">
			<fieldset>
			  <legend>Source Control:</legend>
			    <p>Some source controls may require aditional configurations.</p>
			    Source control: <asp:DropDownList ID="sc" Runat="server" />
				<p>
					<table border="0" width="100%">
						<asp:Repeater id="dynRep" runat="server">
							<ItemTemplate>
								<tr>
									<td>
									<%# DataBinder.Eval(Container, "DataItem.Name") %>:
									<td>
									<td>
										<input type="hidden" id="propKey" runat="server" value='<%# DataBinder.Eval(Container, "DataItem.Name") %>' />
										<asp:TextBox id="propValue" runat="server" />
									<td>
								</tr>
							</ItemTemplate>
						</asp:Repeater>
					</table>
					</p>
			</fieldset>
			</TD>
		</TR>
		<TR>
			<TD colspan="2" align="center">
				<hr noshade>
				<asp:Button ID="Save" Runat="server" Text="Register"></asp:Button>
			</TD>
		</TR>
	</TABLE>
</form>

