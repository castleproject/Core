// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
namespace Castle.MonoRail.Views.Brail

import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Steps
import log4net
import Castle.MonoRail.Framework

class BrailPreProcessor(AbstractCompilerStep):
	
	static Seperators = {"<?brail":"?>", "<%":"%>"}
	static logger = LogManager.GetLogger(BrailPreProcessor)
	
	override def Run():
		new = []
		for input in self.Parameters.Input:
			#if input.Name.Contains("empty"):
			#	System.Diagnostics.Debugger.Break()
			using reader=input.Open():
				code = Booify(reader.ReadToEnd())
				if logger.IsDebugEnabled:
					logger.DebugFormat("Transform result for {0}:\n{1}\n-----",input.Name, code)
				new.Add(StringInput(input.Name, code))
		self.Parameters.Input.Clear()
		for input in new:
			self.Parameters.Input.Add(input)		
		
	def Booify(code as string):
		return "output string.Empty\r\n" if code.Length == 0
		buffer = System.IO.StringWriter()
		index, lastIndex = 0,0
		start,end = GetSeperators(code)
			
		while index != -1:
			index = code.IndexOf(start, lastIndex) 
			break if index == -1
			Output(buffer, code[lastIndex:index])
			lastIndex = code.IndexOf(end, index + start.Length)
			raise RailsException('expected ' + end) if lastIndex == -1
			buffer.Write(code[index+start.Length:lastIndex])
			lastIndex += end.Length 
				
		Output(buffer, code[lastIndex:]) 
		return buffer.ToString()

	def GetSeperators(code as string):
		start as string
		end as string	
		for seperator in Seperators:
			if code.IndexOf( seperator.Key as string,0)!= -1:
				if start is not null:
					raise RailsException("Can't mix seperators in one file. Found both ${start} and ${seperator.Key}")
				start = seperator.Key
				end = seperator.Value
		if start is null: #default, doesn't really matter, since it won't be used.
			for seperator in Seperators:#for good manners, we'll return the first one.
				return seperator.Key.ToString(), seperator.Value.ToString()
		return start,end
		
	def Output(buffer as System.IO.StringWriter, code as string):
		return if len(code) == 0
		buffer.Write('output """')
		buffer.Write(code)
		buffer.WriteLine('"""')