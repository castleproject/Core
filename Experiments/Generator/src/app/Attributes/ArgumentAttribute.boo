namespace Generator

import System

class ArgumentAttribute(Attribute):
	[Property(Name)] _name as string
	[Property(Optional)] _optional as bool
	[Property(Description)] _description as string
	
	def constructor(description):
		_description = description
