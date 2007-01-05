import System.IO

class ModelGenerator(NamedGeneratorBase):
	[Property(Fields)] _fields as (string)
	[Property(Properties)] _properties as (string)
	[Option('no-migration', 'm', "Don't generate migration")]
	_noMigration as bool
	
	def Run():
		_fields = [arg.ToVarName() for arg in Argv].ToArray(string)
		_properties = [arg.ToClassName() for arg in Argv].ToArray(string)
		
		MkDir(ModelsBasePath)
		Process('Model.cs', "${ModelsBasePath}/${ClassName}.cs")
		MkDir(ModelsTestsBasePath)
		Process('Test.cs', "${ModelsTestsBasePath}/${ClassName}Test.cs")
		
		if not _noMigration:
			MkDir(MigrationsBasePath)
			migrationVersion = string.Format("{0:000}", Version)
		
			Process('Migration.cs', "${MigrationsBasePath}/${migrationVersion}_Add${ClassName}Table.cs")
	
	def Usage():
		return 'ModelName [Property1, Property2, ...]'
	
	def Help():
		return 'Generates an ActiveRecord model class'
				
	Namespace:
		get:
			return ModelsNamespace
			
	TestsNamespace:
		get:
			return ModelsTestsNamespace

	UseGeneric as bool:
		get:
			return Framework == "net-2.0"
			
	Version as int:
		get:
			return LastVersion+1
			
	LastVersion as int:
		get:
			return 1 unless Directory.Exists("${MigrationsBasePath}")
			max = 0
			for file in Directory.GetFiles("${MigrationsBasePath}"):
				info = FileInfo(file)
				if info.Name.Substring(3, 1) == '_':
					v = int.Parse(info.Name.Substring(0, 3))
					max = v if v > max
			return max
			
	MigrationNamespace:
		get:
			return MigrationsNamespace