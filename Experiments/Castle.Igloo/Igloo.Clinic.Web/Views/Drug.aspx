<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Drug.aspx.cs" Inherits="Igloo.Clinic.Web.Views.Drug" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Drugs</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:GridView ID="GridView1" Runat="server" DataSourceID="DataSourceControler" 
        AutoGenerateColumns="False"
        AllowPaging="True" 
        AllowSorting="True"
        datakeynames="Id">
        <Columns>
          <asp:CommandField ShowEditButton="True" ShowDeleteButton="True" ShowInsertButton="True" />
          <asp:boundfield datafield="Id" Visible="false" ReadOnly="true" headertext="Id"/>
          <asp:BoundField HeaderText="Name" DataField="Name" SortExpression="Name" />
          <asp:BoundField HeaderText="Description" DataField="Description" SortExpression="Description" />
        </Columns>
    </asp:GridView>
      
    <asp:ObjectDataSource ID="DataSourceControler"
        Runat="server" 
        SelectMethod="GetDrugs"       
        InsertMethod="Create" 
        UpdateMethod="Update" 
        DeleteMethod="Delete"
        TypeName="Igloo.Clinic.Application.DrugController, Igloo.Clinic.Application" 
        OnObjectCreating="DataSourceControler_ObjectCreating">
        <DeleteParameters>
            <asp:Parameter Name="id" Type="Int64" />
        </DeleteParameters>
        <UpdateParameters>
            <asp:Parameter Name="id" Type="Int64" />
            <asp:Parameter Name="name" Type="String" />
            <asp:Parameter Name="description" Type="String" />
        </UpdateParameters>
        <InsertParameters>
            <asp:Parameter Name="id" Type="Int64" />
            <asp:Parameter Name="name" Type="String" />
            <asp:Parameter Name="description" Type="String" />
        </InsertParameters>
    </asp:ObjectDataSource>
    <p />
    <fieldset>
    Name : <asp:TextBox ID="TextBoxNom" runat="server"></asp:TextBox>
    <br />
    Description : <asp:TextBox ID="TextBoxDescription" runat="server"></asp:TextBox>
    <br />
    bb
        <asp:Button ID="ButtonAdd" runat="server" Text="Add" OnClick="ButtonAdd_Click" /></fieldset>
    </div>
    </form>
</body>
</html>
