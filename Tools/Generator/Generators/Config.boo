"""
Global generators config file
Static variables (and methods) defined here are direcly
accessible from the generators.
"""
class Config:
	public static BaseNamespace = "TestProject"
	
	public static Framework = "net-2.0"
	
	# Paths to your models (your ActiveRecord classes)
	public static ModelsBasePath = "app/models"
	public static ModelsNamespace = "${BaseNamespace}.Models"
	public static ModelsTestsBasePath = "test/models"
	public static ModelsTestsNamespace = "${BaseNamespace}.Tests.Models"
	
	# Paths to your controllers stuff
	public static ControllersBasePath = "app/controllers"
	public static ControllersNamespace = "${BaseNamespace}.Controllers"
	public static ControllersTestsBasePath = "test/controllers"
	public static ControllersTestsNamespace = "${BaseNamespace}.Tests.Controllers"
	public static ActionExtension = "aspx" # change to rails if you like
	public static ViewsBasePath = "app/views"
	public static ViewEngineExtension = "vm" # options: brail, vm
	public static StaticContentBasePath = "public/content"
	public static StaticContentBaseUrl = "content"
	public static HelpersBasePath = "app/helpers"
	public static HelpersNamespace = "${BaseNamespace}.Helpers"
	
	# Paths to your migration stuff
	public static MigrationsBasePath = "db/migrations"
	public static MigrationsNamespace = "${BaseNamespace}.Migrations"
	
