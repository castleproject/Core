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

# a class to output the error to the user in a nice way
class OutputError(BrailBase):
	exception as Exception
	
	def constructor(viewEngine as BooViewEngine, output as TextWriter, context as IRailsEngineContext,controller as Controller, exception as Exception):
		super(viewEngine, output, context, controller)
		self.exception = exception
		
	override def Run():
		outputStream.Write("<html><head><title>Brail error!</title></head><body><pre>")
		outputStream.Write(exception.ToString())
		outputStream.Write("</pre></body></html>")
	
	override ScriptDirectory as string:
		get:
			return Environment.CurrentDirectory
