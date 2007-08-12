<%@ Page Language="C#" %>
<script runat="server">
  protected override void OnLoad(EventArgs e)
  {
    Response.Redirect("~/movies/list.castle");
    base.OnLoad(e);
  }
</script>
