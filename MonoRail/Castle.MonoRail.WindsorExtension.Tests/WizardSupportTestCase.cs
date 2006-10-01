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

namespace Castle.MonoRail.WindsorExtension.Tests
{
	using Castle.MonoRail.Framework.Tests;
	using NUnit.Framework;

	using Castle.MonoRail.TestSupport;

	[TestFixture]
	public class WizardSupportTestCase : AbstractTestCase
	{
		[Test]
		public void StartRedirectsToFirstPage()
		{
			DoGet("testwizard/start.rails");
			AssertRedirectedTo("/testwizard/Page1.rails");
		}

		[Test]
		public void UnrelatedWizardActionInvocation()
		{
			DoGet("testwizard/index.rails");
			AssertReplyEqualTo("Hello!");
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
		public void InnerActionsWithNoView()
		{
			DoGet("testwizard/Page1.rails");
			AssertFlashDoesNotContain("InnerActionInvoked");
			AssertSuccess();

			// Yes, we invoke it twice

			DoGet("testwizard/Page1.rails");
			AssertSuccess();
			AssertFlashDoesNotContain("InnerActionInvoked");

			DoGet("testwizard/Page1-InnerAction.rails");
			AssertSuccess();
			AssertFlashContains("InnerActionInvoked");
		}

		[Test]
		public void NavigateToURI()
		{
			DoGet("testwizard/Page4-InnerAction.rails", "navigate.to=uri:testwizard/Page3.rails" );
			AssertRedirectedTo("/testwizard/testwizard/Page3.rails");
			AssertFlashContains("InnerActionInvoked");

			DoGet("testwizard/Page4-InnerAction.rails", "navigate.to=uri:/testwizard/Page3.rails" );
			AssertRedirectedTo("/testwizard/Page3.rails");
			AssertFlashContains("InnerActionInvoked");

			DoGet("testwizard/Page4-InnerAction.rails", "navigate.to=uri:http://google/" );
			AssertRedirectedTo("http://google/");
			AssertFlashContains("InnerActionInvoked");
		}
		
		[Test]
		public void InnerActionsWithView()
		{
			DoGet("testwizard/Page1-InnerAction2.rails");
			AssertSuccess();
			AssertReplyEqualTo("View for Inner action 2");
		}
	
		[Test]
		public void WizardPageWithView()
		{
			DoGet("testwizard/Page3.rails");
			AssertSuccess();
			AssertReplyEqualTo("Wizard page 3");
		}

		[Test]
		public void WizardHelper_HasNextAndPreviousSteps()
		{
			DoGet("testwizard/Page4.rails");
			AssertSuccess();
			AssertReplyEqualTo("hasprevious True\r\nhasnext False");
		}
	}
}
