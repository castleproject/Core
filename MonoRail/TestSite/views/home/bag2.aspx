<script runat=server>

	private String _name;
	private String[] _list;

	public String CustomerName
	{
		set { _name = value; }
	}

	public String[] List
	{
		set { _list = value; }
	}

</script>
Customer is <%= _name %>
<br>
<% 
   foreach(String value in _list) 
   {
      Response.Write(value);
   }
%>