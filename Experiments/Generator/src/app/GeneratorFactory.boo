namespace Generator

import System
import System.IO
import System.Text
import System.Reflection
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Useful.Attributes
import Generator.Extentions

[Singleton]
class GeneratorFactory:
	[Property(ScriptBasePath)]
	_scriptBasePath as string
	
	def constructor():
		asmpath = Path.GetDirectoryName(typeof(GeneratorBase).Assembly.Location)
		_scriptBasePath = Path.Combine(asmpath, "../Generators/".ToPath())
	
	def CreateAndRun(argv as (string)) as int:
		if argv.Length == 0:
			ListGenerators()
			return -1
		
		name = argv[0].ToClassName()
		args = argv[1:]
		
		try:
			generator = GetGenerator(name)
		except ex as Exception:
			print ex.Message
			return -1
		
		if argv.Length == 1:
			generator.PrintUsage()
			return -1
		try:
			generator.Init(args)
			generator.Run()
		except ex as Exception:
			print 'An error occured while running the generator:'
			print ex.Message
			return -1
		return 0
		
	# Creates a new generator by its name
	def GetGenerator(name as string) as GeneratorBase:
		script = GetGeneratorScriptFile(name)
		
		raise GeneratorException("Generator ${name} not found") if not File.Exists(script)
		
		return Compile(script)
	
	# Returns all generators
	def GetGenerators() as (GeneratorBase):
		generators = []
		for d in Directory.GetDirectories(ScriptBasePath):
			name = DirectoryInfo(d).Name
			try:
				generators.Add(GetGenerator(name))
			except Exception:
				pass
		return generators.ToArray(GeneratorBase)
	
	private def ListGenerators():
		print 'usage: generate GeneratorName [Arguments...]'
		print
		print 'Available generators:'
		for gen in GetGenerators():
			print gen.GeneratorName.PadLeft(10), ':',	 gen.Help()
		
	private def GetGeneratorScriptFile(name as string):
		return Path.Combine(ScriptBasePath, "${name}/${name}Generator.boo")
			
	private def Compile(script as string) as GeneratorBase:
		code = StringBuilder()
		
		# Adds default imports
		code.Append('import Generator;')
		code.Append('import Generator.Extentions;')
		code.Append('import Config;')
		
		using reader = StreamReader(script):
			code.Append(reader.ReadToEnd())
		
		compiler = BooCompiler()
		compiler.Parameters.Input.Add(FileInput("${ScriptBasePath}/Config.boo"))
		compiler.Parameters.Input.Add(StringInput(script, code.ToString()))
		compiler.Parameters.References.Add(typeof(GeneratorBase).Assembly)
		compiler.Parameters.Pipeline = CompileToMemory()
		
		ctx = compiler.Run()
		
		if len(ctx.Errors) > 1:
			print "Compilation errors!"
			for e in ctx.Errors:
				print e unless e.Code == "BCE0028" #No entry point
		
		if ctx.GeneratedAssembly is null:
			raise GeneratorException("Can't compile generator")
		
		genType = ctx.GeneratedAssembly.GetTypes()[1]
		generator as GeneratorBase = genType()
		
		return generator

