// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.Rook.Parse.Tests
{
	using System;


	public class EntryPointCodeTestCase : AbstractParserTestCase
	{
		public EntryPointCodeTestCase()
		{
		}

		public void NoCodeAtAll()
		{
			String contents = "\r\n";
		}

		public void HelloWorld()
		{
			String contents = 
				"puts 'Hello' \r\n " + 
				"puts \"World\"\r\n";
		}

		public void HelloWorldWithComments()
		{
			String contents = 
				"# some comments here \r\n " +
				"puts 'Hello' # more comments 1 \r\n " + 
				"puts \"World\" # more comments 2 \r\n";
		}

		public void HelloWorldWithModuleAndMethod()
		{
			String contents = 
				"module HelloApp \r\n " + 
				"  include System.Console \r\n " + 
				"  def self.say_hello     \r\n " + 
				"    puts 'hello world'   \r\n " + 
				"  end \r\n " + 
				"end \r\n " + 
				"HelloApp.say_hello \r\n ";
		}
	}
}
