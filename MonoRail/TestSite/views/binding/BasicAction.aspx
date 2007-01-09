<%@ Page Language="C#" AutoEventWireup="false" %>

<%@ Register Assembly="Castle.MonoRail.Framework" Namespace="Castle.MonoRail.Framework.Views.Aspx" TagPrefix="mr" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Binding Actions to Basic Controls</title>
    <style type="text/css">
		li
		{
			margin-top: 10px;
		}
    </style>
</head>
<body>
    <form id="form1" runat="server">
		<h2>Basic Controls</h2>
		<ul>
			<li>Button:
				<asp:Button ID="btnClickMe" Width="100px" runat="server" Text="Click Me" />&nbsp;
				<asp:LinkButton ID="lbtnClickMe" Width="100px" runat="server" Text="Click Me" />
				</li><li>DropDownList:
				<asp:DropDownList ID="ddlSelectMe" Width="102px" runat="server" AutoPostBack="True">
				<asp:ListItem>Brenda</asp:ListItem>
   				<asp:ListItem>Kaitlyn</asp:ListItem> 			
   				<asp:ListItem>Lauren</asp:ListItem> 			       			
				</asp:DropDownList>	
			</li>
			<li>TextBox:
				<asp:TextBox ID="txtChangeMe" Width="96px" runat="server" AutoPostBack="True"></asp:TextBox>
			</li>
			<li>CheckBox:
				<asp:CheckBox ID="cbCheckMe" Text="Check Me" runat="server" AutoPostBack="True" />
			</li>
			<li>Radio:
				<asp:RadioButton ID="rbPick1" Text="Pick 1" GroupName="Radio" runat="server" AutoPostBack="True" />
				<asp:RadioButton ID="rbPick2" Text="Pick 2" GroupName="Radio" runat="server" AutoPostBack="True" />
			</li>			
		</ul>
		<asp:HyperLink ID="lnkIndex" NavigateUrl="~/binding/Index.rails" runat="server">Index</asp:HyperLink>
		</form>
		<mr:ControllerBinder ID="ControllerBinder" runat="server">
			<ControllerBindings>
				<mr:ControllerBinding controlID="btnClickMe">
					<mr:ActionBinding EventName="Click" ActionName="ClickMe">
						<ActionArguments>
							<mr:ActionArgument Expression="$ddlSelectMe.Text" Name="selection" />
							<mr:ActionArgument Expression="$txtChangeMe.Text" Name="text" />
						</ActionArguments>
					</mr:ActionBinding>
				</mr:ControllerBinding>
				<mr:ControllerBinding controlID="ddlSelectMe">
					<mr:ActionBinding EventName="SelectedIndexChanged" ActionName="SelectMe">
						<ActionArguments>
							<mr:ActionArgument Expression="$this.Text" Name="selection" />
						</ActionArguments>
					</mr:ActionBinding>
				</mr:ControllerBinding>
				<mr:ControllerBinding controlID="txtChangeMe">
					<mr:ActionBinding ActionName="ChangeMe" EventName="TextChanged">
						<ActionArguments>
							<mr:ActionArgument Expression="$this.Text" Name="text" />
						</ActionArguments>
					</mr:ActionBinding>
				</mr:ControllerBinding>
				<mr:ControllerBinding controlID="lbtnClickMe">
					<mr:ActionBinding ActionName="ClickMe" EventName="Click">
					</mr:ActionBinding>
				</mr:ControllerBinding>
				<mr:ControllerBinding controlID="cbCheckMe">
					<mr:ActionBinding ActionName="CheckMe" EventName="CheckedChanged">
						<ActionArguments>
							<mr:ActionArgument Expression="$this.Checked" Name="isChecked" />
						</ActionArguments>
					</mr:ActionBinding>
				</mr:ControllerBinding>
				<mr:ControllerBinding controlID="rbPick2">
					<mr:ActionBinding ActionName="PickMe" EventName="CheckedChanged">
						<ActionArguments>
							<mr:ActionArgument Expression="$this.Text" Name="name" />
						</ActionArguments>
					</mr:ActionBinding>
				</mr:ControllerBinding>
				<mr:ControllerBinding controlID="rbPick1">
					<mr:ActionBinding ActionName="PickMe" EventName="CheckedChanged">
						<ActionArguments>
							<mr:ActionArgument Expression="$this.Text" Name="name" />
						</ActionArguments>
					</mr:ActionBinding>
				</mr:ControllerBinding>
			</ControllerBindings>
		</mr:ControllerBinder>
</body>
</html>
