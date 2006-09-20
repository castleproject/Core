<?brail
import Boo.Lang 
?>
<html>
<body>
<title>${title.ToUpper()}</title>
<body>
<ul>
<?brail
for i in Builtins.range(3):
    output "<li>${i}</li>"
end
?>
</ul>
</body>
</html>
