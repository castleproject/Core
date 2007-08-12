<%@ Page Language="C#" %>
<script runat="server">
  protected override void OnLoad(EventArgs e)
  {
    Response.Redirect("~/product/index.castle");
    base.OnLoad(e);
  }
</script>
