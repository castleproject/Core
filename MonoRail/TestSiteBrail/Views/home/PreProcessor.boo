<? 
import Boo.Lang 
?>
<html>
<body>
<title>${title.ToUpper()}</title>
<body>
<ul>
<?
for i in Builtins.range(3):
    output "<li>${i}</li>"
end
?>
</ul>
</body>
</html>
