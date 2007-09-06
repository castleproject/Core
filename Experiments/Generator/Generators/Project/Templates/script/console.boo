#!/usr/bin/env booi
import Boo.Lang.Interpreter
import System.IO

# Default values
config = 'development'

# Replace default values by command line arguments
config = argv[0] if argv.Length > 0

# Switch current dir to find dependencies
Directory.SetCurrentDirectory("public/bin")

interpreter = InteractiveInterpreter(RememberLastValue: true)

# Initialize ActiveRecord
interpreter.load("<%= ClassName %>.dll")
interpreter.Eval("import System")
interpreter.Eval("import <%= ClassName %>")
interpreter.Eval("import <%= ClassName %>.Models")
interpreter.Eval("Boot.InitializeActiveRecord('${config}', false)")

interpreter.Eval("import Castle.ActiveRecord")              
interpreter.Eval("session as SessionScope = SessionScope()")

print "${config} environement loaded"
interpreter.ConsoleLoopEval()

interpreter.Eval("session.Dispose(false)")
