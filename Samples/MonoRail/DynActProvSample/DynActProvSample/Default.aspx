<%@ Page Language="C#" %>
<script runat="server">
  protected override void OnLoad(EventArgs e)
  {
    Response.Redirect("~/category2/list.rails");
    base.OnLoad(e);
  }
</script>

