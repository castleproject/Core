class ControllerGenerator(NamedGeneratorBase):
	[Property(Actions)]	_actions as (string)
	[Property(Action)]	_action as string
	
	def Run():
		_actions = Argv[0:]
		
		MkDir(ControllersBasePath)
		Process('Controller.cs', "${ControllersBasePath}/${ClassName}Controller.cs")
		
		MkDir(ControllersTestsBasePath)
		Process('Test.cs', "${ControllersTestsBasePath}/${ClassName}ControllerTest.cs")
		
		MkDir("${ViewsBasePath}/layouts")
		Process("layout.${ViewEngineExtension}", "${ViewsBasePath}/layouts/${FileName}.${ViewEngineExtension}")
		
		MkDir(ViewPath)
		for a in Actions:
			Action = a
			Process("View.${ViewEngineExtension}", "${ViewsBasePath}/${FileName}/${Action.ToFileName()}.${ViewEngineExtension}")
	
	def Usage() as string:
		return 'ControllerName [Action1, Action2, ...]'
	
	def Help() as string:
		return 'Generates a controller'
		
	Namespace:
		get:
			return ControllersNamespace
		
	TestsNamespace:
		get:
			return ControllersTestsNamespace
	
	Extension:
		get:
			return ActionExtension
			
	ViewPath:
		get:
			return "${ViewsBasePath}/${FileName}"
