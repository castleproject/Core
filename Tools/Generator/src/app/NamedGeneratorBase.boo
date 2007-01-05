namespace Generator

import Generator.Extentions

abstract class NamedGeneratorBase(GeneratorBase):
	[Property(Name), Argument('The name of the thing to generate')]
	_name as string
	
	virtual VarName:
		get:
			return _name.ToVarName()
	
	virtual ClassName:
		get:
			return _name.ToClassName()
	
	virtual FileName:
		get:
			return _name.ToFileName()

	virtual TableName:
		get:
			return PluralClassName
		
	virtual PluralClassName:
		get:
			return ClassName.ToPlural()
	
	virtual CrossPlatformPluralClassName:
		get:
			return ClassName.ToPlural().ToLower()
			
	virtual PluralVarName:
		get:
			return VarName.ToPlural()
			
	virtual SingularVarName:
		get:
			return VarName.ToSingular()
	
	virtual HumanName:
		get:
			return _name.ToHumanName()
	
	virtual PluralHumanName:
		get:
			return HumanName.ToPlural()
	
	virtual SingularHumanName:
		get:
			return HumanName.ToSingular()
