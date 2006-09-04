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

namespace Castle.MonoRail.Framework.Views.StringTemplateView.Tests
{
	using System;
	using System.IO;

	using NUnit.Framework;

	using Castle.MonoRail.Framework.Tests;

	[TestFixture]
	public class StringTemplateRescueTestCase : RescueTestCase
	{
		[Test]
		public void RescueWithSkipRescue()
		{
			DoGet("rescuable/MethodWithSkipRescue.rails");
			AssertStatusCode(500);
		}
		
		[Test]
		public void RescueWithAppExceptionType()
		{
			DoGet("rescuable/RescueWithExceptionsByType.rails", "exceptionType=appException");

			AssertSuccess();

			AssertReplyEqualTo( "appException" );
		}		
		
		[Test]
		public void RescueWithArgExceptionType()
		{
			DoGet("rescuable/RescueWithExceptionsByType.rails", "exceptionType=argException");

			AssertSuccess();

			AssertReplyEqualTo( "argException" );
		}		
		
		[Test]
		public void RescueWithControllerDefaultExceptionType()
		{
			DoGet("rescuable/RescueWithExceptionsByType.rails");

			AssertSuccess();

			AssertReplyEqualTo( "An error happened" );
		}		

		[Test]
		public void RescueWithMethodDefaultArgExceptionType()
		{
			DoGet("rescuable/RescueWithExceptionsByTypeWithDefaultException.rails", "exceptionType=argException");

			AssertSuccess();

			AssertReplyEqualTo( "argException" );
		}
		
		[Test]
		public void RescueWithMethodDefaultExceptionType()
		{
			DoGet("rescuable/RescueWithExceptionsByTypeWithDefaultException.rails");

			AssertSuccess();

			AssertReplyEqualTo( "methodDefaultException" );
		}		
	}
}
