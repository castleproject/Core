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

namespace Castle.MonoRail.Framework.Views.NVelocity.Tests
{
	using System;
	using System.IO;

	using NUnit.Framework;
	
	using Castle.MonoRail.Engine.Tests;


	[TestFixture]
	public class NVelocityComponentsTestCase : AbstractNVelocityTestCase
	{
		[Test]
		public void BlockComponent1()
		{
			string url = "/usingcomponent/index.rails";
			string expected = "hello from body\r\n";

			Execute(url, expected);
		}

		[Test]
		public void SmartComponent1()
		{
			string url = "/usingcomponent/index2.rails";
			string expected = "0 ";

			Execute(url, expected);
		}

		[Test]
		public void SmartComponent2()
		{
			string url = "/usingcomponent/index2.rails?port=1&host=xxx";
			string expected = "1 xxx";

			Execute(url, expected);
		}

		[Test]
		public void BlockComponentWithinForeach()
		{
			string url = "/usingcomponent/index3.rails";
			string expected = "inner content 1\r\ninner content 2\r\n";

			Execute(url, expected);
		}
	}
}
