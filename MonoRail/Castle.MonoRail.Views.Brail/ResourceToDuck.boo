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
import Castle.MonoRail.Framework
import Boo.Lang

# This allows to treat resources in a natural way:
#	text.Hello
# Instead of:
# 	text["Hello"]
class ResourceToDuck(IQuackFu):
	
	resource as IResource
	
	def constructor(resource as IResource):
		self.resource = resource
		
	def QuackGet(name as string):
		raise RailsException("Resource ${name} does not exists") if resource.GetObject(name) is null
		return resource.GetObject(name)
	
	def QuackSet(name as string, obj as object):
		raise RailsException("You cannnot set resource ${name}")
	
	def QuackInvoke(name as string, args as (object)):
		raise RailsException("You cannnot invoke resource ${name}")
