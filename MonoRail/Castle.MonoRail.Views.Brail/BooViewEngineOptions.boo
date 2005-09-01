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

# Options class for the BooViewEngine
class BooViewEngineOptions:
	[Property(BatchCompile)]
	batch as bool = true
	
	[Property(CommonScriptsDirectory)]
	commonScriptsDirectory as string = "CommonScripts"
	
	[Property(SaveToDisk)]
	saveToDisk as bool = false
	
	[Property(Debug)]
	debug as bool = false
		
	[Property(SaveDirectory)]  
	saveDirectory as string = "Brail_Generated_Code"

	[Property(AssembliesToReference)]
	assembliesToReference = []
	
	def constructor():
		assembliesToReference.Add(typeof(BrailBase).Assembly)
		assembliesToReference.Add(typeof(Castle.MonoRail.Framework.Controller).Assembly)
