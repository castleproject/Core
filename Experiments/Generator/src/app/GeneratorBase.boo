namespace Generator

import System
import System.IO
import System.Reflection
import Generator.Extentions

abstract class GeneratorBase:
	[Property(Argv)]			_argv as (string)
	[Property(ScriptPath)]		_scriptPath as string
	[Property(TemplatesPath)]	_templatesPath as string
	
	[Option('replace', 'r', 'Overwrite existing files')]
	_overwiteAll as bool = false
	[Option('force', 'f', 'Force overwrite of files that look the same')]
	_force as bool
	[Option('silent', 's', 'Display not output')]
	_silent as bool
	[Option('debug', 'd', 'Set debug mode')]
	_debug as bool
		
	_parser as ArgumentParser
	
	def constructor():
		_scriptPath = Path.Combine(ScriptBasePath, GeneratorName)
		_templatesPath = "${ScriptPath}/Templates"
		_parser = ArgumentParser(self)
	
	# This method is called when the generator is created
	# it can be used to initialize arguments
	virtual def Init(argv as (string)):
		_argv = _parser.SetArguments(argv)
	
	# Main method to run the generator
	abstract def Run():
		pass
	
	virtual def Usage() as string:
		return _parser.GetUsage()
		
	virtual def Help() as string:
		return 'No help yet, maybe tomorrow!'
	
	#region Template properties
	GeneratorName as string:
		get:
			n = GetType().Name
			return n.Substring(0, n.Length - 'generator'.Length)
	
	ScriptBasePath as string:
		get:
			return GeneratorFactory.Instance.ScriptBasePath
	#endregion
	
	#region Template processing methods
	def Process(template as string, tofile as string):
		Process(template, tofile, false)
		
	def Process(template as string, tofile as string, keep as bool):
		cleanToFile = tofile.ToPath()
		t = Template(Path.Combine(_templatesPath, template.ToPath()))
		result = t.Process(CollectVariables())
		file = FileInfo(cleanToFile)
		
		if file.Exists:
			if keep:
				Log('exists', cleanToFile)
				return
				
			if _force:
				Log('replace', cleanToFile)
			elif file.Length == result.Length:
				Log('same', cleanToFile)
				return
			else:
				if LogAndAskForOverwrite(cleanToFile):
					Log('replace', cleanToFile)
				else:
					return
		else:
			Log('create', cleanToFile)
		
		using w = StreamWriter(cleanToFile):
			w.Write(result)
	
	def Copy(file as string, topath as string):
		cleanFile = Path.Combine(_templatesPath, file.ToPath())
		cleanToPath = topath.ToPath()
		cleanToFile = Path.Combine(cleanToPath, FileInfo(cleanFile).Name)
		
		if File.Exists(cleanToFile):
			if _force or LogAndAskForOverwrite(cleanToFile):
				Log('replace', cleanToFile)
			else:
				return
		else:
			Log('create', cleanToFile)
		
		File.Copy(cleanFile, cleanToFile, true)
		
	def CopyDir(dir as string, topath as string):
		MkDir(topath)
		source = Path.Combine(_templatesPath, dir.ToPath())
		for file in Directory.GetFiles(source):
			name = FileInfo(file).Name
			Copy(file, topath) if not name.StartsWith('.')
		for dir in Directory.GetDirectories(source):
			name = DirectoryInfo(dir).Name
			CopyDir(dir, Path.Combine(topath, name)) if not name.StartsWith('.')
		
	def MkDir(path as string):
		cleanPath = path.ToPath()
		if Directory.Exists(cleanPath):
			Log('exists', cleanPath)
		else:
			Log('create', cleanPath)
			Directory.CreateDirectory(cleanPath)
	#endregion
	
	#region Helper methods
	virtual def PrintUsage():
		print "usage: generate ${GeneratorName} ${Usage()}"
		print
		print _parser.GetHelp()
		print
		print Help()

	protected def Log(action as string, path as string):
		if not _silent:
			print action.PadLeft(10), path
			
	protected def Debug(msg as string):
		if _debug and not _silent:
			print msg
	
	private def CollectVariables() as Hash:
		props = {}
		for p as PropertyInfo in GetType().GetProperties():
			props.Add(p.Name, p.GetValue(self, null))
		return props
	
	private def LogAndAskForOverwrite(path as string) as bool:
		return true if _overwiteAll
		
		question = "${'exists'.PadLeft(10)} ${path} Overwrite? [Ynaq] "
		answer = prompt(question).ToLower()
		if answer == 'y' or answer == '':
			return true
		elif answer == 'a':
			_overwiteAll = true
			return true
		elif answer == 'q':
			Environment.Exit(0)
			return false
	#endregion
