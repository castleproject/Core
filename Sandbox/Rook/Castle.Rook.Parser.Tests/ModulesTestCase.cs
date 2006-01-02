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


	public class ModulesTestCase : AbstractParserTestCase
	{
		public void Mixins()
		{
			String contents = 
				"module Debug   \r\n" +
				"  def whoAmI?   \r\n" +
				"    return \"#{self.type.name} (##{self.id}): #{self.to_s}\"   \r\n" +
				"  end   \r\n" +
				"end   \r\n" +
				"class Phonograph   \r\n" +
				"  include Debug   \r\n" +
				"  # ...   \r\n" +
				"end   \r\n" +
				"class EightTrack   \r\n" +
				"  include Debug   \r\n" +
				"  # ...   \r\n" +
				"end   \r\n" +
				"ph = Phonograph.new(\"West End Blues\")   \r\n" +
				"et = EightTrack.new(\"Surrealistic Pillow\")   \r\n" +
				"  \r\n" +
				"ph.whoAmI?  # \"Phonograph (#537762134): West End Blues \r\n" +
				"et.whoAmI?  # \"EightTrack (#537761824): Surrealistic Pillow\"  ";
		}
	}
}
