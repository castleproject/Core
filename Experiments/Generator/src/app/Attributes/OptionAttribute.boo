namespace Generator

import System

class OptionAttribute(Attribute):
	[Property(LongName)] _longName as string
	[Property(ShortName)] _shortName as string
	[Property(Description)] _description as string
	
	
	def constructor(longName, shortName, description):
		_longName = longName
		_shortName = shortName
		_description = description
	