<%@ Page Language="C#" %>
<script runat="server">
  protected override void OnLoad(EventArgs e)
  {
    Response.Redirect("~/contact/index.rails");
    base.OnLoad(e);
  }
</script>

<!DOCTYPE html PUBLIC 
  "-//W3C//DTD XHTML 1.0 Strict//EN"
  "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
  <title>Welcome</title>
</head>
<body>
  <p>
    If you were not redirected, please <a href="contact/index.rails">click here</a>.
  </p>
</body>
</html>