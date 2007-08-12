<%@ Page Language="C#" %>
<script runat="server">
  protected override void OnLoad(EventArgs e)
  {
    Response.Redirect("~/account/index.castle");
    base.OnLoad(e);
  }
</script>
