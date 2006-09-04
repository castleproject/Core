<html>
<head>
$AjaxHelperAlias/GetJavascriptFunctions()$
</head>

<body>
Observer field example:

<form>
  Please enter the zip code:<br>
  <input type="text" name="zip" id="zip"> 
  
  <br>
  <div id="address">
  </div>

</form>

$AjaxHelperAlias/ObserveField(fieldId="zip", frequency=2, url="inferaddress.rails", idOfElementToBeUpdated="address", with="'aa new content'")$

<hr>

<p>
Observer form example:

<form id="myform">
  Fill the field below to create an account:<br>
  Name: <input type="text" name="name" id="name"> <br>
  Address: <input type="text" name="addressf" id="addressf"> 
  
  <br>
  <div id="message">
  </div>

</form>
</p>

$AjaxHelperAlias/ObserveForm(formId="myform", frequency=2, url="accountformvalidate.rails", idOfElementToBeUpdated="message")$

<hr>

<h4 id="status">Status</h4>

<p>

userlist()

<br>
<b>Add New User:</b>
$AjaxHelper/BuildFormRemoteTag("AddUserWithAjax.rails", "userlist", null, "$('status').innerHTML = 'Saving...'", null, null, "$('status').innerHTML = 'Done!'")
<table>  
	<tr>
		<td>Name:</td>
		<td><input type="text" name="name"></td>
	</tr>
	<tr>
		<td>EMail:</td>
		<td><input type="text" name="email"></td>
	</tr>
	<tr>
		<td colspan="2" align="center">
		<input type="submit" value="Add">
		</td>
	</tr>
</table>  
</form>

</p>

</body>

</html>
