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

namespace Castle.MonoRail.Engine.Tests
{
	using System;
	using System.Net;

	using NUnit.Framework;


	[TestFixture]
	public class RescueTestCase : AbstractCassiniTestCase
	{
		[Test]
		public void GeneralRescue()
		{
			string url = "/rescuable/index.rails";
			string expected = "An error happened";

			Execute(url, expected);
		}

		[Test]
		public void RescueForMethod()
		{
			string url = "/rescuable/save.rails";
			string expected = "An error happened during save";

			Execute(url, expected);
		}

		[Test]
		public void RescueForMethodWithLayout()
		{
			string url = "/rescuable/update.rails";
			string expected = "\r\nWelcome!\r\n<p>An error happened during update</p>\r\nFooter";

			Execute(url, expected);
		}

		[Test]
		public void RescueMessage()
		{
			string url = "/rescuable/updateMsg.rails";
			string expected = "custom msg";

			Execute(url, expected);
		}
	}
}
