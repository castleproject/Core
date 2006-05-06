<div id="userlist">
<table>
<tr>
  <td><strong>Name</strong></td>
  <td><strong>Email</strong></td>
</tr>
$users:{user|
<tr>
  <td>$user.Name$</td>
  <td>$user.Email$</td>
</tr>
}$
</table>
</div>