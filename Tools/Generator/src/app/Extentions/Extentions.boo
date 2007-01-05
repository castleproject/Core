namespace Generator.Extentions

import System.IO
import System.Text.RegularExpressions

# String class extention
def ToCapital(self as string) as string:
	return self[:1].ToUpper() + self[1:].ToLower()
def ToTitle(self as string) as string:
	return join(word.ToCapital() for word in self.Split(char(' ')))
def ToCamel(self as string) as string:
	name = Regex.Replace(self, "(_|\\s)([a-z])", { m as Match | return m.Groups[2].ToString().ToUpper() })
	return name[:1].ToUpper() + name[1:]
def ToClassName(self as string) as string:
	return self.ToCamel()
def ToVarName(self as string) as string:
	name = self.ToCapital()
	return name[:1].ToLower() + name[1:]
def ToHumanName(self as string) as string:
	return /[A-Z]/.Replace(self, ' $0').Trim().ToCapital()
def ToPlural(self as string) as string:
	return Inflector.Instance.ToPlural(self)
def ToSingular(self as string) as string:
	return Inflector.Instance.ToSingular(self)
def ToFileName(self as string) as string:
	# All lower case for compatibility
	return /_/.Replace(self, '').Trim().ToLower()
def ToPath(self as string) as string:
	sep = Path.DirectorySeparatorChar.ToString()
	return self.Replace("/", sep).Replace("\\", sep)

# Integer class extention
def Times(self as int, c as callable(int)):
	for i in range(self):
		c(i+1)
def ToOrdinal(self as int):
	# Adapted from http://api.rubyonrails.com/classes/Inflector.html#M000814
	if self % 100 in range(11, 14):
		return "${self}th"
	else:
		if self % 10 == 1:
			return "${self}st"
		if self % 10 == 2:
			return "${self}nd"
		if self % 10 == 3:
			return "${self}rd"
		else:
			return "${self}th"

# Lists extentions
def Each(self as System.Collections.IEnumerable, action as callable(object)):
	for item in self:
		action(item)
		
# Date and time extentions
def Ago(self as System.TimeSpan) as System.DateTime:
	return System.DateTime.Now.Subtract(self)

def FromNow(self as System.TimeSpan) as System.DateTime:
	return System.DateTime.Now.Add(self)

