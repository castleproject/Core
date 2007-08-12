<%@ Page Language="C#" %>
<script runat="server">
  protected override void OnLoad(EventArgs e)
  {
    Response.Redirect("~/Scaffolding/index.castle");
    base.OnLoad(e);
  }
</script>
