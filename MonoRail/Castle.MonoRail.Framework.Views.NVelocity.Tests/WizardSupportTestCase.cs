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

	using NUnit.Framework;

	using Castle.MonoRail.TestSupport;

	[TestFixture]
	public class WizardSupportTestCase : AbstractMRTestCase
	{
		[Test]
		public void StartRedirectsToFirstPage()
		{
			DoGet("testwizard/start.rails");
			AssertRedirectedTo("/testwizard/Page1.rails");
		}

		[Test]
		public void Page1HasCorrectIndex()
		{
			DoGet("testwizard/Page1.rails");
			AssertSuccess();
			AssertSessionEntryEqualsTo("wizard.testwizardcurrentstepindex", 0);
			AssertSessionEntryEqualsTo("wizard.testwizardcurrentstep", "Page1");

			// Yes, we invoke it twice

			DoGet("testwizard/Page1.rails");
			AssertSuccess();
			AssertSessionEntryEqualsTo("wizard.testwizardcurrentstepindex", 0);
			AssertSessionEntryEqualsTo("wizard.testwizardcurrentstep", "Page1");

			DoGet("testwizard/Page2.rails");
			AssertSuccess();
			AssertSessionEntryEqualsTo("wizard.testwizardcurrentstepindex", 1);
			AssertSessionEntryEqualsTo("wizard.testwizardcurrentstep", "Page2");
		}

		[Test]
		public void ProcessOnlyInvokedUnderRightConditions()
		{
			DoGet("testwizard/Page1.rails");
			AssertSuccess();
			AssertFlashDoesNotContain("ProcessInvoked");

			// Yes, we invoke it twice

			DoGet("testwizard/Page1.rails");
			AssertSuccess();
			AssertFlashDoesNotContain("ProcessInvoked");

			DoGet("testwizard/Page1.rails", "wizard.doprocess=true");
			AssertSuccess();
			AssertFlashContains("ProcessInvoked");
		}
		
//		[Test]
//		public void ProcessWithQueryStringParams()
//		{
//			DoPost("testwizard/Page1.rails", "wizard.doprocess=true", "name=1");
//			AssertSuccess();
//			AssertFlashContains("ProcessInvoked");
//			AssertRedirectedTo("/testwizard/Page2.rails?Id=1");
//		}		
	}
}
