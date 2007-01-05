class ScaffoldGenerator(NamedGeneratorBase):
	[Property(Area)] _area as string
	[Property(Controller)] _controller as string
	[Property(Model)] _model as string
	
	def Run():
		_model = Name
		if Argv.Length > 0:
			# controller name is specified
			matches = /((\w+)\/|)(\w+)/.Match(Argv[0])
			_area = matches.Groups[2].Value
			_controller = matches.Groups[3].Value
		else:
			# controller name is not specified so
			# it is plural of model name
			_controller = _model.ToPlural()
		
		if Area == "" or Area == null:
			Area = null
			controllerPath = ControllersBasePath
			controllerTestPath = ControllersTestsBasePath
			viewPath = "${ViewsBasePath}/${ControllerFileName}"
		else:
			controllerPath = "${ControllersBasePath}/${Area.ToLower()}"
			controllerTestPath = "${ControllersTestsBasePath}/${Area.ToLower()}"
			viewPath = "${ViewsBasePath}/${Area.ToLower()}/${ControllerFileName}"
			
		MkDir(ControllersBasePath)
		Process('ApplicationController.cs', "${ControllersBasePath}/ApplicationController.cs", true)	

		MkDir(controllerPath)
		Process('Controller.cs', "${controllerPath}/${ControllerName}Controller.cs")
				
		MkDir(controllerTestPath)
		Process('Test.cs', "${controllerTestPath}/${ControllerName}ControllerTest.cs")
				
		MkDir(HelpersBasePath)
		Process('ScaffoldHelper.cs', "${HelpersBasePath}/ScaffoldHelper.cs", true)
				
		MkDir(viewPath)
		Process('views/list.vm', "${viewPath}/list.vm")
		Process('views/edit.vm', "${viewPath}/edit.vm")
		Process('views/view.vm', "${viewPath}/view.vm")
		Process('views/new.vm', "${viewPath}/new.vm")
		Process('views/_form.vm', "${viewPath}/_form.vm")
				
		MkDir("${ViewsBasePath}/layouts")
		Process('views/layout.vm', "${ViewsBasePath}/layouts/${ControllerFileName}.vm")
				
		MkDir("${StaticContentBasePath}/stylesheets")
		Process('style.css', "${StaticContentBasePath}/stylesheets/scaffold.css", true)
	
	def Usage():
		return 'ModelName [[area/]controller]'
	
	def Help():
		return 'Generates a CRUD actions controller for a specified model.'
	
	ControllerName:
		get:
			return Controller.ToClassName()
	
	ControllerFileName:
		get:
			return Controller.ToFileName()
	
	ControllerLink:
		get:
			if Area != null:
				return "${Area}/${ControllerFileName}"
			return ControllerFileName
			
	ModelClassName:
		get:
			return _model.ToClassName()
				
	Namespace:
		get:
			if Area != null:
				return "${ControllersNamespace}.${Area.ToClassName()}"
			return ControllersNamespace
		
	TestsNamespace:
		get:
			if Area != null:
				return "${ControllersTestsNamespace}.${Area.ToClassName()}"
			return ControllersTestsNamespace
	
	ApplicationControllerNamespace:
		get:
			return ControllersNamespace
	
	ModelsNamespace:
		get:
			return Config.ModelsNamespace

	HelpersNamespace:
		get:
			return Config.HelpersNamespace
	
	ContentBasePath:
		get:
			return StaticContentBaseUrl

	ActionExtension:
		get:
			return Config.ActionExtension
