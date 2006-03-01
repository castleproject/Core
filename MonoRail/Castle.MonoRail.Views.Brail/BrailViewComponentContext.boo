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

class BrailViewComponentContext(IViewComponentContext):
	
	[Getter(ComponentName)]
	componentName as string
	
	[Getter(Writer)]
	writer as TextWriter
	
	[Getter(ContextVars)]
	contextVars as IDictionary = Hashtable(CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default)
	
	[Getter(ComponentParameters)]
	componentParameters as IDictionary
	
	[Property(ViewToRender)]
	viewToRender as string
	
	[Property(Body)]
	body as callable
	
	def constructor(body as callable, name as string, 
		writer as TextWriter, params as IDictionary):
		
		self.body = body
		self.componentName = name
		self.writer = writer
		self.componentParameters = params
	
	def RenderBody():
		RenderBody(writer)
		
	def RenderBody(writer as TextWriter):
		if body is null:
			raise RailsException("This component does not have a body content to be rendered")
		body(writer)
