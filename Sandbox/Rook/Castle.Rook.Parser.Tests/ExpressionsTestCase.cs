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

namespace Castle.Rook.Parse.Tests
{
	using System;

	using NUnit.Framework;

	[TestFixture]
	public class ExpressionsTestCase
	{
		[Test]
		public void SimpleAssignment()
		{
			String contents = "x = 10;";

			RookParser.ParseContents(contents);
		}

		[Test]
		public void MultipleAssignment()
		{
			String contents = "x, y = 10;";

			RookParser.ParseContents(contents);
		}

		[Test]
		public void MultipleAssignment2()
		{
			String contents = "x, y = 10, 11;";

			RookParser.ParseContents(contents);
		}

		[Test]
		public void AssignmentAndInvocation()
		{
			String contents = "x = 10 \r\n" +
					"puts (x) \r\n";

			RookParser.ParseContents(contents);
		}

		[Test]
		public void AssignmentAndInvocation2()
		{
			String contents = "x = 10 \r\n" +
				"System.Console.WriteLine x \r\n";

			RookParser.ParseContents(contents);
		}
	}
}
