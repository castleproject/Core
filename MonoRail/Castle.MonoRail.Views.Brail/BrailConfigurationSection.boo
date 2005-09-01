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
import System.Reflection
import System.Configuration
import System.Xml

# Very simple configuration section
class BrailConfigurationSection(System.Configuration.IConfigurationSectionHandler):
	
	def Create(parent as object, configContext as object, section as XmlNode):
		options = BooViewEngineOptions()
		if section.Attributes["batch"]:
			options.BatchCompile = bool.Parse(section.Attributes["batch"].Value)
		if section.Attributes["saveToDisk"]:
			options.SaveToDisk =  bool.Parse(section.Attributes["saveToDisk"].Value)
		if section.Attributes["debug"]:
			options.Debug = bool.Parse(section.Attributes["debug"].Value)
		if section.Attributes["saveDirectory"]:
			options.SaveDirectory = section.Attributes["saveDirectory"].Value
		if section.Attributes["commonScriptsDirectory"]:
			options.CommonScriptsDirectory = section.Attributes["commonScriptsDirectory"].Value
		for reference as XmlNode in section.SelectNodes("reference"):
			asm = Assembly.Load(reference.Attributes["assembly"].Value) 
			options.AssembliesToReference.Add(asm)
		return options
