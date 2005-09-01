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
import System.Diagnostics
import System.Collections
import Castle.MonoRail.Framework

# Base class for all the view scripts, this is the class that is responsible for
# support all the behind the scenes magic such as variable to PropertyBag trasnlation, 
# resources usage, etc.
abstract class BrailBase:
	
	[Property(OutputStream)]
	outputStream as TextWriter

	# This is used by layout scripts only, for outputing the
	# child's content
	[Property(ChildOutput)]
	childOutput as TextWriter
	
	properties as Hashtable
	
	context as IRailsEngineContext
	
	controller as Controller
	
	viewEngine as BooViewEngine

	def constructor(viewEngine as BooViewEngine, output as TextWriter, context as IRailsEngineContext, controller as Controller):
		self.viewEngine = viewEngine
		self.outputStream = output
		self.context = context
		self.controller = controller
		InitProperties(context, controller)
		
	# This is the method that will contain all the global code in the script and 
	# is run to get the output of the script
	abstract def Run():
		pass
		
	# The path of the script, this is filled by AddBrailBaseClassStep
	# and is used for sub views
	virtual ScriptDirectory as string:
		get:
			return viewEngine.ViewRootDir
	
	# Output the subview to the client, this is either a relative path "SubView" which
	# is relative to the current /script/ or an "absolute" path "/home/menu" which is
	# actually relative to ViewDirRoot
	def OutputSubView(subviewName as string):
		OutputSubView(subviewName, {})
	
	# Similiar to the above function, but with a bunch of parameters that are used
	# just for this subview. This parameters are /not/ inheritable.
	def OutputSubView(subviewName as string, parameters as IDictionary):
		subViewFileName = GetSubViewFilename(subviewName)
		subView = viewEngine.GetCompiledScriptInstance(subViewFileName,outputStream,context, controller)
		for param as DictionaryEntry in parameters:
			subView.properties[param.Key] = param.Value if param.Key != null
		subView.Run()
	
	# Get the sub view file name, if the subview starts with a '/' 
	# then the filename is considered relative to ViewDirRoot
	# otherwise, it's relative to the current script directory
	def GetSubViewFilename(subviewName as string):
		#absolute path from Views directory
		if subviewName[0] == char('/'):
			return viewEngine.GetFileName(subviewName[1:])
		return Path.Combine(ScriptDirectory,subviewName)+".boo"
	
	# this is called by ReplaceUnknownWithParameters step to create a more dynamic experiance
	# any uknon identifier will be translate into a call for GetParameter('identifier name').
	# This mean that when an uknonwn identifier is in the script, it will only be found on runtime.
	def GetParameter(name as string) as object:
		raise RailsException("Parameter '${name}' was not found!") if not properties.Contains(name)
		return properties[name]
	
	# Allows to check that a parameter was defined
	def IsDefined(name as string) as bool:
		return properties.Contains(name)
	
	# Initialize all the properties that a script may need
	# One thing to note here is that resources are wrapped in ResourceToDuck wrapper
	# to enable easy use by the script
	def InitProperties(context as IRailsEngineContext, controller as Controller):
		properties = Hashtable(CaseInsensitiveHashCodeProvider.Default, 
			CaseInsensitiveComparer.Default)
		
		properties.Add("context", context)
		properties.Add("request", context.Request)
		properties.Add("response", context.Response)
		properties.Add("session", context.Session)
		
		if controller.Resources is not null:
			for key in controller.Resources.Keys:
				properties.Add(key, ResourceToDuck(controller.Resources[key]))
		
		for helper in controller.Helpers.Values:
			properties.Add(helper.GetType().Name, helper)
		
		for key in context.Params.AllKeys:
			continue if key is null
			properties[key] = context.Params[key]
		
		for entry as DictionaryEntry in context.Flash:
			properties[entry.Key] = entry.Value
		
		for entry as DictionaryEntry in controller.PropertyBag:
			properties[entry.Key] = entry.Value
		
		properties["siteRoot"] = context.ApplicationPath
