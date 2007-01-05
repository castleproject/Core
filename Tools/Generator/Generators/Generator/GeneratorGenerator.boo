import System
import System.IO

class GeneratorGenerator(NamedGeneratorBase):

	def Run():
		MkDir("${ScriptBasePath}/${Name}")
		Process('Generator.boo', "${ScriptBasePath}/${Name}/${Name}Generator.boo")
		
		MkDir("${ScriptBasePath}/${Name}/Templates")

	def Help() as string:
		return "Generates a generator skeleton"
