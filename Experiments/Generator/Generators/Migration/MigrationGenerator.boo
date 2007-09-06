import System.IO

class MigrationGenerator(NamedGeneratorBase):
	def Run():
		MkDir(MigrationsBasePath)
		sVersion = string.Format("{0:000}", Version)
		
		Process('Migration.cs', "${MigrationsBasePath}/${sVersion}_${ClassName}.cs")
	
	def Help():
		return 'Generates a migration'

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
			
	Namespace:
		get:
			return MigrationsNamespace
