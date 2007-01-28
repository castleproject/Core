<%@ Page Language="C#" MasterPageFile="~/Views/Site.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Igloo.Clinic.Web.Views.Index" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   -<asp:GridView ID="GridViewBlog" runat="server" BackColor="White" BorderColor="#E7E7FF" 
   BorderStyle="None" BorderWidth="1px" CellPadding="3"  GridLines="Horizontal">
       <FooterStyle BackColor="#B5C7DE" ForeColor="#4A3C8C" />
       <RowStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" />
       <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="#F7F7F7" />
       <PagerStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" HorizontalAlign="Right" />
       <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#F7F7F7" />
       <AlternatingRowStyle BackColor="#F7F7F7" />
    </asp:GridView>
    <asp:Button ID="Button1" runat="server" CommandName="searchDrug" OnClick="Button1_Click" Text="View drug" />
</asp:Content>
