<%@ Control %>
<script language="C#" runat="server">
  
  public String PageTitle
  {
  	get { return (this as IAttributeAccessor).GetAttribute("title"); }
  }
  
</script>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
	<HEAD>
		<title>PestControl - <%= PageTitle %></title>
        <link href="../../css/def.css" rel="stylesheet" type="text/css">
	</HEAD>
	<body>
		   <div class="titlecontainer">
		     PestControl - <%= PageTitle %>
		   </div>

		   <div class="container" align="center">