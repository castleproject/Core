<%@ Page Language="C#" %>
<script runat="server">
  protected override void OnLoad(EventArgs e)
  {
    Response.Redirect("~/home/index.castle");
    base.OnLoad(e);
  }
</script>
