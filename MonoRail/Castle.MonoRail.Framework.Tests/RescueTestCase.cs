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

namespace Castle.MonoRail.Framework.Tests
{
	using System;
	using System.Net;

	using NUnit.Framework;

	using Castle.MonoRail.TestSupport;


	[TestFixture]
	public class RescueTestCase : AbstractMRTestCase
	{
		[Test]
		public void GeneralRescue()
		{
			DoGet("rescuable/index.rails");

			AssertSuccess();

			AssertReplyEqualTo( "An error happened" );
		}

		[Test]
		public void RescueForMethod()
		{
			DoGet("rescuable/save.rails");

			AssertSuccess();

			AssertReplyEqualTo( "An error happened during save" );
		}

		[Test]
		public void RescueForMethodWithLayout()
		{
			DoGet("rescuable/update.rails");

			AssertSuccess();

			AssertReplyEqualTo( "\r\nWelcome!\r\n<p>An error happened during update</p>\r\nFooter" );
		}

		[Test]
		public void RescueMessage()
		{
			DoGet("rescuable/updateMsg.rails");

			AssertSuccess();

			AssertReplyEqualTo( "custom msg" );
		}
	}
}
