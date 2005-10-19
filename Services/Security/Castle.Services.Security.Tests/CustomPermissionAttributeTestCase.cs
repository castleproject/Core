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

namespace Castle.Services.Security.Tests
{
	using System;
	using System.Security;
	using System.Threading;
	using System.Security.Principal;

	using Castle.Services.Security.Tests.Model;
	
	using NUnit.Framework;


	[TestFixture]
	public class CustomPermissionAttributeTestCase
	{
		[Test]
		public void UserCanAccessEverything()
		{
			Thread.CurrentPrincipal = new GenericExtendedPrincipal(
				new GenericIdentity("johndoe", "NTLS"), 
				new string[] { "admin" } , 
				new string[] { "can_access_private_info", "can_do_something_critical" });

			MySecurityClass instance = new MySecurityClass();
			instance.DoSomethingCritical();
		}

		[Test]
		//[ExpectedException( typeof(SecurityException) )]
		public void UserCannotAccessMethod()
		{
			Thread.CurrentPrincipal = new GenericExtendedPrincipal(
				new GenericIdentity("johndoe", "NTLS"), 
				new string[] { "admin" } , 
				new string[] { "can_access_private_info" });

			MySecurityClass instance = new MySecurityClass();
			instance.DoSomethingCritical();
		}

		[Test]
		//[ExpectedException( typeof(SecurityException) )]
		public void UserCannotAccessAnything()
		{
			Thread.CurrentPrincipal = new GenericExtendedPrincipal(
				new GenericIdentity("johndoe", "NTLS"), 
				new string[] { "admin" } , 
				new string[] {  });

			new MySecurityClass();
		}
	}
}
