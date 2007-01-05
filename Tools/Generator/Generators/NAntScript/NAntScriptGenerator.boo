import System.Xml from "System.Xml"

class NAntScriptGenerator(GeneratorBase):
	[Property(ProjectFile), Argument('Path to the .csproj file')]
	_projectFile as string
	[Property(Project), Argument('Project name')]
	_project as string
	
	[Property(References)] _references as (string)
	[Property(Sources)] _sources as (string)

	def Run():
		doc = XmlDocument()
		doc.Load(_projectFile)
		
		root = "//VisualStudioProject/CSHARP"
		
		_references = [node.Attributes['HintPath'].Value for node as XmlElement in doc.SelectNodes("${root}/Build/References/Reference")].ToArray(string)
		_sources = [node.Attributes['RelPath'].Value for node as XmlElement in doc.SelectNodes("${root}//Files/Include/File")].ToArray(string)
		
		Process('default.build', "${Project}.build")
		
	def Help():
		return 'Generates a NAnt build script from a Visual Studio 2005 project file (.csproj)'
