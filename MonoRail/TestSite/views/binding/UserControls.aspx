<%@ Page Language="C#" %>

<%@ Register Src="EmployeeControl.ascx" TagName="Employee" TagPrefix="uc" %>
<%@ Register Assembly="Castle.MonoRail.Framework" Namespace="Castle.MonoRail.Framework.Views.Aspx" TagPrefix="mr" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">


<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<uc:Employee ID="ucEmployee" runat="server" />
    </div>
    <br />
 	<asp:HyperLink ID="lnkIndex" NavigateUrl="~/binding/Index.rails" runat="server">Index</asp:HyperLink>   
    </form>
   	<mr:ControllerBinder ID="ControllerBinder" runat="server">
		<mr:ControllerBinding controlID="ucEmployee">
			<mr:ActionBinding EventName="AddEmployee" ActionName="AddEmployee">
				<ActionArguments>
					<mr:ActionArgument Expression="$event.Name" Name="name" />
					<mr:ActionArgument Expression="$event.Country" Name="country" />
				</ActionArguments>
			</mr:ActionBinding>
			<mr:ActionBinding EventName="RemoveEmployee" ActionName="RemoveEmployee">
				<ActionArguments>
					<mr:ActionArgument Expression="$event.Id" Name="id" />
				</ActionArguments>
			</mr:ActionBinding>			
		</mr:ControllerBinding>	
   	</mr:ControllerBinder> 
</body>
</html>
