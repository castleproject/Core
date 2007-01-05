namespace Generator

import System
import System.IO
import System.Text
import System.Reflection
import Boo.Lang
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Compiler.Steps
import Boo.Lang.Parser
import MGutz.CodeInjection from "MGutz.CodeInjection"
import MGutz.CodeInjection.Antlr from "MGutz.CodeInjection"

class Template:
	[Property(Filename)]
	_filename as string
	
	[Property(Debug)]
	_debug as bool
	
	private def constructor():
		pass
	
	def constructor(filename as string):
		_filename = filename
	
	def Process() as string:
		return Process({})

	def Process(variables as Hash) as string:
		using reader = StreamReader(_filename):
			code = ParseCode(reader, variables)
		
		if _debug:
			print code
		
		compiler = BooCompiler()
		compiler.Parameters.Ducky = true
		compiler.Parameters.Debug = _debug
		compiler.Parameters.Input.Add(StringInput(_filename, code))
		compiler.Parameters.References.Add(typeof(GeneratorBase).Assembly)
		compiler.Parameters.Pipeline = CompileToMemory()
		compiler.Parameters.Pipeline.RemoveAt(0)
		compiler.Parameters.Pipeline.Insert(0, WSABooParsingStep())
		ctx = compiler.Run()
		
		if len(ctx.Errors) > 1:
			print "Compilation errors!"
			for e in ctx.Errors:
				print e unless e.Code == "BCE0028" #No entry point
		
		asm = ctx.GeneratedAssembly
		if asm == null:
			raise Exception("No assembly generated")
		
		templateType = asm.GetType("TemplateCode")
		templateObj = templateType() as duck
		
		return templateObj.RunTemplate(variables)
		
	private def Sanitize(reader as TextReader) as TextReader:
		out = reader.ReadToEnd()
		reader.Close()
		# Ugly fix to allow ${...} in templates
		out = out.Replace('${', '<%= "$"+"{" %>')
		return StringReader(out)
		
	private def ParseCode(reader as TextReader, variables as Hash) as string:	
		lexer = AspLexer(Sanitize(reader))
		parser = AspParser(lexer)
		parser.template()
		code = parser.Builder.ToString()
		
		builder = StringBuilder()
		builder.Append('import Generator.Extentions\n')
		builder.Append('import Generator\n')
		
		builder.Append('class TemplateCode():\n')
		builder.Append('\tpublic def RunTemplate(vars as Hash) as string:\n')
		for v in variables:
			builder.Append("\t\t${v.Key} = vars['${v.Key}']\n")
		builder.Append('\t\tout = System.Text.StringBuilder()\n')
		builder.Append(code)
		builder.Append('\t\treturn out.ToString()\n')
		builder.Append('\tend\n')
		builder.Append('end')
		
		return builder.ToString()

	
