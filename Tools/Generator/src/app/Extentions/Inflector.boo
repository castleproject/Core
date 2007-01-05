namespace Generator.Extentions

import System.Collections
import System.Text.RegularExpressions
import Useful.Attributes from "Boo.Lang.Useful"

# Inspired by RoR's inflector
[Singleton]
class Inflector:
	_plurals = []
	_singulars = []
	_uncountables = []
	
	def constructor():
		InitInflections()
		
	def AddPluralInflections(inflections as IEnumerable):
		_plurals.Extend(inflections)
	
	def AddSingularInflections(inflections as IEnumerable):
		_singulars.Extend(inflections)
		
	def AddIrregularInflections(inflections as IEnumerable):
		_plurals.Extend(inflections)
		_singulars.Extend([Inflection(inf.Replace, inf.Pattern) for inf as Inflection in inflections])
		
	def AddUncountables(words as (string)):
		_uncountables.Extend(words)
		
	def IsUncountable(word as string) as bool:
		return _uncountables.Contains(word.ToLower())
		
	def ToPlural(word as string) as string:
		return word if IsUncountable(word)
		
		for inflection as Inflection in _plurals.Reversed:
			return inflection.Apply(word) if inflection.IsMatch(word)
		return word
	
	def ToSingular(word as string) as string:
		return word if IsUncountable(word)
		
		for inflection as Inflection in _singulars.Reversed:
			return inflection.Apply(word) if inflection.IsMatch(word)
		return word
	
	#region Inflections definitions
	private def InitInflections():
		AddPluralInflections(( \
			Inflection(@/$/, 's'),
			Inflection(/(?i)s$/, 's'),
			Inflection(/(?i)(ax|test)is$/, '$1es'),
			Inflection(/(?i)(octop|vir)us$/, '$1i'),
			Inflection(/(?i)(alias|status)$/, '$1es'),
			Inflection(/(?i)(bu)s$/, '$1ses'),
			Inflection(/(?i)(buffal|tomat)o$/, '$1oes'),
			Inflection(/(?i)([ti])um$/, '$1a'),
			Inflection(/(?i)sis$/, 'ses'),
			Inflection(/(?i)(?:([^f])fe|([lr])f)$/, '$1$2ves'),
			Inflection(/(?i)(hive)$/, '$1s'),
			Inflection(/(?i)([^aeiouy]|qu)y$/, '$1ies'),
			Inflection(/(?i)([^aeiouy]|qu)ies$/, '$1y'),
			Inflection(/(?i)(x|ch|ss|sh)$/, '$1es'),
			Inflection(/(?i)(matr|vert|ind)ix|ex$/, '$1ices'),
			Inflection(/(?i)([m|l])ouse$/, '$1ice'),
			Inflection(/(?i)^(ox)$/, '$1en'),
			Inflection(/(?i)(quiz)$/, '$1zes')))
		
		AddSingularInflections(( \
			Inflection(/(?i)s$/, ''),
			Inflection(/(?i)(n)ews$/, '$1ews'),
			Inflection(/(?i)([ti])a$/, '$1um'),
			Inflection(/(?i)((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)ses$/, '$1$2sis'),
			Inflection(/(?i)(^analy)ses$/, '$1sis'),
			Inflection(/(?i)([^f])ves$/, '$1fe'),
			Inflection(/(?i)(hive)s$/, '$1'),
			Inflection(/(?i)(tive)s$/, '$1'),
			Inflection(/(?i)([lr])ves$/, '$1f'),
			Inflection(/(?i)([^aeiouy]|qu)ies$/, '$1y'),
			Inflection(/(?i)(s)eries$/, '$1eries'),
			Inflection(/(?i)(m)ovies$/, '$1ovie'),
			Inflection(/(?i)(x|ch|ss|sh)es$/, '$1'),
			Inflection(/(?i)([m|l])ice$/, '$1ouse'),
			Inflection(/(?i)(bus)es$/, '$1'),
			Inflection(/(?i)(o)es$/, '$1'),
			Inflection(/(?i)(shoe)s$/, '$1'),
			Inflection(/(?i)(cris|ax|test)es$/, '$1is'),
			Inflection(/(?i)([octop|vir])i$/, '$1us'),
			Inflection(/(?i)(alias|status)es$/, '$1'),
			Inflection(/(?i)^(ox)en/, '$1'),
			Inflection(/(?i)(vert|ind)ices$/, '$1ex'),
			Inflection(/(?i)(matr)ices$/, '$1ix'),
			Inflection(/(?i)(quiz)zes$/, '$1')))
		
		AddIrregularInflections(( \
			Inflection('person', 'people'),
			Inflection('man', 'men'),
			Inflection('child', 'children'),
			Inflection('sex', 'sexes'),
			Inflection('move', 'moves')))
		
		AddUncountables(( \
			'equipment', 'information', 'rice', 'money',
			'species', 'series', 'fish', 'sheep'))
	#endregion
	

class Inflection:
	[Property(Pattern)] _pattern as object
	[Property(Replace)] _replace as string
	
	def constructor(pattern, replace):
		_pattern = pattern
		_replace = replace
	
	def IsMatch(str as string) as bool:
		return string.Compare(_pattern, str, true) == 0 if (_pattern isa string)
		return (_pattern as Regex).IsMatch(str)
	
	def Apply(str as string) as string:
		return _replace if (_pattern isa string)
		return (_pattern as Regex).Replace(str, _replace)
