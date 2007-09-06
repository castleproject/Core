namespace Generator

import System
import System.Reflection
import Generator.Extentions

class ArgumentParser():
	[Property(Target)] _target

	def constructor(target):
		_target = target
		
	def GetUsage() as string:
		return string.Join(' ', GetArgumentsNames()) + " [options]"
		
	def GetHelp() as string:
		usage = "Arguments:"
		for arg in GetArguments():
			usage += "\n    ${arg.Name.PadRight(15)} ${arg.Description}"
		usage += "\nOptions:"
		for arg in GetOptions():
			usage += "\n    -${arg.ShortName}, --${arg.LongName.PadRight(9)} ${arg.Description}"
		return usage
	
	def SetArguments(argvParam as (string)) as (string):
		argv = List(argvParam)
		for field in GetTargetFields():
			arg = Attribute.GetCustomAttribute(field, ArgumentAttribute)
			option = Attribute.GetCustomAttribute(field, OptionAttribute) as OptionAttribute
			if option != null:
				if ("-${option.ShortName}" in argvParam or "--${option.LongName}" in argvParam):
					field.SetValue(_target, true)
					argv.Remove("-${option.ShortName}")
					argv.Remove("--${option.LongName}")
			elif arg != null:
				i = 0
				while i < len(argv)-1 and (argv[i] as string).StartsWith('-'):
					i++
				if i < len(argv):
					field.SetValue(_target, argv[i])
					argv.Remove(argv[i])
			
		return argv.ToArray(string)
	
	def IsValid() as bool:
		for field in GetTargetFields():
			arg = Attribute.GetCustomAttribute(field, ArgumentAttribute) as ArgumentAttribute
			if arg != null and not arg.Optional and (field.GetValue(_target) == null or field.GetValue(_target) == ""):
				return false
		return true
				
	private def GetArguments() as (ArgumentAttribute):
		args = []
		for field in GetTargetFields():
			arg = Attribute.GetCustomAttribute(field, ArgumentAttribute) as ArgumentAttribute
			if arg != null:
				arg.Name = field.Name.ToClassName()
				args.Add(arg) 

		return args.ToArray(ArgumentAttribute)

	private def GetOptions() as (OptionAttribute):
		args = []
		for field in GetTargetFields():
			arg = Attribute.GetCustomAttribute(field, OptionAttribute) as OptionAttribute
			args.Add(arg) if arg != null

		return args.ToArray(OptionAttribute)

	private def GetArgumentsNames() as (string):
		args = []

		for field in GetTargetFields():
			arg = Attribute.GetCustomAttribute(field, ArgumentAttribute) as ArgumentAttribute
			if arg != null:
				if arg.Optional:
					args.Add("[${field.Name.ToClassName()}]")
				else:
					args.Add(field.Name.ToClassName())

		return args.ToArray(string)
		
	private def GetTargetFields():
		return _target.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
		