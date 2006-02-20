// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
namespace Castle.MonoRail.Views.Brail


import System
import System.IO
import System.Collections
import Castle.MonoRail.Framework
import Boo.Lang.Parser
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Steps
import Boo.Lang.Compiler.Pipelines

# This is the class that is responsible to take a template name and turn it into an
# executable code that then is piped to the user.
public class BooViewEngine (ViewEngineBase):
	compilations = Hashtable.Synchronized(Hashtable(CaseInsensitiveHashCodeProvider.Default, 
			CaseInsensitiveComparer.Default))
	
	# This is used to add a reference to the common scripts for each compiled scripts
	common as System.Reflection.Assembly
	# Watch the common script files directory
	commonScriptWatcher as FileSystemWatcher
	# Watch the Views directory for changes
	viewsScriptWatcher as FileSystemWatcher
	
	static options as BooViewEngineOptions
	baseSavePath as string
	commonScriptPath as string
	fullPathToViewDir as string
	
	static def InitializeConfig():
		InitializeConfig("brail")
		InitializeConfig("Brail") if options is null
		options = BooViewEngineOptions() if options is null
	
	static def InitializeConfig(name as string):
		options = System.Configuration.ConfigurationSettings.GetConfig(name)
	
	# Get configuration options if they exists, if they do not exist, load the default ones
	# Create directory to save the compiled assemblies if required.
	# pre-compile the common scripts
	override def Init():
		InitializeConfig() if options is null
		baseDir = Path.GetDirectoryName(typeof(BooViewEngine).Assembly.Location)
		self.baseSavePath = Path.Combine(baseDir,options.SaveDirectory)
		self.commonScriptPath = Path.Combine(ViewRootDir, options.CommonScriptsDirectory)
		self.fullPathToViewDir = Path.GetFullPath(ViewRootDir)
		if options.SaveToDisk and not Directory.Exists(baseSavePath):
			Directory.CreateDirectory(baseSavePath)
		
		viewsScriptWatcher = FileSystemWatcher(ViewRootDir,
			IncludeSubdirectories: true, Filter: "*.boo",
			NotifyFilter: NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.DirectoryName)
		viewsScriptWatcher.Changed += def (source, e as FileSystemEventArgs):
			file = Path.GetFullPath(e.FullPath)
			# Will cause a recompilation
			compilations[file] = null
		
		viewsScriptWatcher.EnableRaisingEvents = true
		
		if not CompileCommonScripts():
			return
			
		commonScriptWatcher = FileSystemWatcher(commonScriptPath,
			IncludeSubdirectories: false, Filter:"*.boo",
			NotifyFilter: NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.DirectoryName)
		commonScriptWatcher.Changed += def (source, e as FileSystemEventArgs):
			CompileCommonScripts.BeginInvoke( null,
				{ ar as IAsyncResult | CompileCommonScripts.EndInvoke(ar) } )
		
		commonScriptWatcher.EnableRaisingEvents = true
		
		
	# Just check if the filename exists, I'm not sure when it's called
	override def HasTemplate(templateName as string):
		file = GetFileName(templateName)
		exists = File.Exists(file)
		return exists
		
	# Process a template name and output the results to the user
	# This may throw if an error occured and the user is not local (which would 
	# cause the yellow screen of death) if the user is local, then a detailed stack
	# is shown
	override def Process(context as IRailsEngineContext, controller as Controller, templateName as string):
		try:
			file = GetFileName(templateName)
			view as BrailBase
			# Output may be the layout's child output if a layout exists
			# or the context.Response.Output if the layout is null
			output as TextWriter, layout as BrailBase = GetOutput(context, controller)
			# Will compile on first time, then save the assembly on the cache.
			view = GetCompiledScriptInstance(file, output, context, controller)
			controller.PreSendView(view)
			view.Run()
			layout.Run() if layout is not null
			controller.PostSendView(view)
		except e:
			HandleError(context, controller, e, "Cannot load view ${templateName}")
	
	# Send the contents text directly to the user, only adding the layout if neccecary
	override def ProcessContents(context as IRailsEngineContext, controller as Controller, contents as string):
		try:
			output as TextWriter, layout as BrailBase = GetOutput(context, controller)
			output.Write(contents)
			layout.Run() if layout is not null
		except e:
			HandleError(context, controller, e, "")
	
	# Throw if the user is not local, otherwise send detailed error message
	def HandleError(context as IRailsEngineContext, controller as Controller, e as Exception, msg as string):
		if not context.Request.IsLocal:
				raise RailsException(msg)
		error = OutputError(self, context.Response.Output,context, controller, e)
		error.Run()
	
	# Check if a layout has been defined. If it was, then the layout would be created
	# and will take over the output, otherwise, the context.Reposne.Output is used, 
	# and layout is null
	def GetOutput(context as IRailsEngineContext, controller as Controller):
		layout as BrailBase
		output as TextWriter = context.Response.Output
		if controller.LayoutName is not null:
			layoutTemplate = "layouts/${controller.LayoutName}"
			layoutFilename = GetFileName(layoutTemplate)
			layout = GetCompiledScriptInstance(layoutFilename,output, 
				context, controller)
			output = layout.ChildOutput = StringWriter()	
		return output, layout
				
	def GetFileName(templateName as string):
		filename = Path.Combine(self.ViewRootDir,templateName)+".boo"
		return Path.GetFullPath(filename)
		
	# This takes a filename and return an instance of the view ready to be used.
	# If the file does not exist, an exception is raised
	# The cache is checked to see if the file has already been compiled, and it had been
	# a check is made to see that the compiled instance is newer then the file's modification date.
	# If the file has not been compiled, or the version on disk is newer than the one in memory, a new
	# version is compiled.
	# Finally, an instance is created and returned	
	def GetCompiledScriptInstance(file as string, 
		output as TextWriter,
		context as IRailsEngineContext,
		controller as Controller) as BrailBase:
			
		batch as bool = options.BatchCompile
		if  compilations.ContainsKey(file):
			type as Type = compilations[file]
			if type is not null:
				return type(self, output, context, controller)
			# if file is in compilations and the type is null,
			# this means that we need to recompile. Since this usually means that 
			# the file was changed, we'll set batch to false and procceed to compile just
			# this file.
			batch = false
			
		type = CompileScript(file, batch)
		return type(self, output,context, controller)
	
	# Compile a script (or all scripts in a directory), save the compiled result
	# to the cache and return the compiled type.
	# If an error occurs in batch compilation, then an attempt is made to compile just the single
	# request file.
	def CompileScript(filename as string, batch as bool) as Type:
		inputs = GetInput(filename, batch)
		name = NormalizeName(filename, batch)
		result = DoCompile(inputs,name)
		if result.Errors.Count:
			if not batch:
				#This can be very useful to figure out things, but it's also pretty bad
				#security wise.
				#code = BrailPreProcessor.Booify(System.IO.File.OpenText(filename).ReadToEnd())
				#code = System.Web.HttpUtility.HtmlEncode(code)
				raise RailsException("Error during compile:\r\n${result.Errors.ToString()}\r\nCode:\r\n")
			#error compiling a batch, let's try a single file
			return CompileScript(filename,false)
		
		type as Type
		for input in inputs:
			type = result.GeneratedAssembly.GetType(Path.GetFileNameWithoutExtension(input.Name))
			compilations[input.Name] = type
		
		type = compilations[filename]
		return type
	
	# If batch compilation is set to true, this would return all the view scripts
	# in the director (not recursive!)
	# Otherwise, it would return just the single file
	def GetInput(filename as string, batch as bool) as (ICompilerInput):
		return (FileInput(filename),) if not batch
		inputs = []
		directory = Path.GetDirectoryName(filename)
		for file in Directory.GetFiles(directory,"*.boo"):
			inputs.Add(FileInput(file))
		return inputs.ToArray(ICompilerInput)
	
	# Perform the actual compilation of the scripts
	# Things to note here:
	# * The generated assembly reference the Castle.MonoRail.MonoRailBrail and Castle.MonoRail.Framework assemblies
	# * If a common scripts assembly exist, it is also referenced
	# * The AddBrailBaseClassStep compiler step is added - to create a class from the view's code
	# * The ProcessMethodBodiesWithDuckTyping is replaced with ReplaceUknownWithParameters
	#   this allows to use naked parameters such as (output context.IsLocal) without using 
	#   any special syntax
	# * The IntroduceGlobalNamespaces step is removed, to allow to use common variables such as 
	#   date & list without accidently using the Boo.Lang.BuiltIn versions
	def DoCompile(files as (ICompilerInput), name as string):
		compiler = SetupCompiler(files)
		filename = Path.Combine(self.baseSavePath,name)
		compiler.Parameters.OutputAssembly = filename
		# this is here and not in SetupCompiler since CompileCommon is also
		# using SetupCompiler, and we don't want reference to the old common from the new one
		compiler.Parameters.References.Add(common) if common is not null
		# pre procsssor needs to run before the parser
		compiler.Parameters.Pipeline.Insert(0, BrailPreProcessor())
		# inserting the add class step after the parser
		compiler.Parameters.Pipeline.Insert(2, TransformToBrailStep())
		compiler.Parameters.Pipeline.Replace(Steps.ProcessMethodBodiesWithDuckTyping,ReplaceUknownWithParameters())
		compiler.Parameters.Pipeline.Replace(InitializeTypeSystemServices, InitializeCustomTypeSystem())
		compiler.Parameters.Pipeline.RemoveAt(compiler.Parameters.Pipeline.Find(IntroduceGlobalNamespaces))
		
		return compiler.Run()
	
	# Return the output filename for the generated assembly
	# The filename is dependant on whatever we are doing a batch
	# compile or not, if it's a batch compile, then the directory name
	# is used, if it's just a single file, we're using the file's name.
	# '/' and '\' are replaced with '_', I'm not handling ':' since the path
	# should never include it since I'm converting this to a relative path
	def NormalizeName(filename as string, batch as bool):
		#Get relative filename,
		name = Path.GetFullPath(filename)[fullPathToViewDir.Length+1:]
		name = Path.GetDirectoryName(name) if batch
		for ch as char in (Path.AltDirectorySeparatorChar,
			Path.DirectorySeparatorChar):
			name = name.Replace(ch.ToString(),"_")
		return name + ".dll"
	
	# Compile all the common scripts to a common assemblies
	# an error in the common scripts would raise an exception.
	def CompileCommonScripts():
		if options.CommonScriptsDirectory is null:
			return false
		if not Directory.Exists(commonScriptPath):
			return false
		
		# the demi.boo is stripped, but GetInput require it.
		demiFile = Path.Combine(commonScriptPath,"demi.boo")
		inputs = GetInput(demiFile,true)
		compiler = SetupCompiler(inputs)
		outputFile = Path.Combine(self.baseSavePath, "CommonScripts.dll")
		compiler.Parameters.OutputAssembly = outputFile
		result = compiler.Run()
		if result.Errors.Count:
			raise RailsException(result.Errors.ToString(true))
		common = result.GeneratedAssembly
		compilations.Clear()
		return true
		
	# common setup for the compiler
	def SetupCompiler(files as (ICompilerInput)):
		
		compiler = BooCompiler()
		compiler.Parameters.Ducky = true
		compiler.Parameters.Debug = options.Debug
		if options.SaveToDisk:
			compiler.Parameters.Pipeline = CompileToFile()
		else:
			compiler.Parameters.Pipeline = CompileToMemory()
		# replace the normal parser with white space agnostic one.
		compiler.Parameters.Pipeline.RemoveAt(0)
		compiler.Parameters.Pipeline.Insert(0, WSABooParsingStep() )
		for file in files:
			compiler.Parameters.Input.Add(file)
		for assembly as System.Reflection.Assembly in options.AssembliesToReference:
			compiler.Parameters.References.Add(assembly)
		compiler.Parameters.OutputType = CompilerOutputType.Library
		return compiler
		

