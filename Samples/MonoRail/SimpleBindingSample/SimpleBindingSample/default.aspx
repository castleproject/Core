<%@ Page Language="C#" %>
<script runat="server">
  protected override void OnLoad(EventArgs e)
  {
    Response.Redirect("~/contact/index.castle");
    base.OnLoad(e);
  }
</script>
