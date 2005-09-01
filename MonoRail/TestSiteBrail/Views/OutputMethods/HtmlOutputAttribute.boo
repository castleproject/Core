<%
[Html]
def HtmlMethod():
	return "Some text that will be <html> encoded"
%>
${HtmlMethod()}