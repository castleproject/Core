// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Views.AspView.Tests.ViewTests
{
	using System;
	using ViewComponents;
	using Views;
	using NUnit.Framework;

	[TestFixture]	
	public class ViewComponentsTestFixture : AbstractViewComponentsTestFixture
	{
		[Test]
		public void WithRenderText_Works()
		{
			RegisterComponent("MyComponent", typeof(TextRendererViewComponent));

			InitializeView(typeof(WithComponent));

			view.Process();

			expected =
@"Parent
Default text from component's RenderText()
Parent";

			AssertViewOutputEqualsToExpected();
		}

		[Test]
		public void WithRenderView_Works()
		{
			RegisterComponent("MyComponent", typeof(ViewRendererViewComponent));

			InitializeView(typeof(WithComponent));

			AddCompilation("\\components\\MyComponent\\SimpleView", typeof(SimpleView));

			view.Process();

			expected =
@"Parent
Simple
Parent";

			AssertViewOutputEqualsToExpected();
		}

		[Test]
		public void WithRenderTextAndView_ViewIsAlwaysAfterTheText()
		{
			RegisterComponent("MyComponent", typeof(TextAndViewRendererViewComponent));

			InitializeView(typeof(WithComponent));

			AddCompilation("\\components\\MyComponent\\SimpleView", typeof(SimpleView));

			view.Process();

			expected =
@"Parent
Default text from component's RenderText()Simple
Parent";

			AssertViewOutputEqualsToExpected();
		}

		[Test]
		public void WithBody_RendersTheBody()
		{
			RegisterComponent("MyComponent", typeof(BodyRendererViewComponent));

			InitializeView(typeof(WithComponentAndBody));

			view.Process();

			expected =
@"Parent
Component's Body
Parent";

			AssertViewOutputEqualsToExpected();
		}

		[Test]
		public void WithBody_ButNoBody_Throws()
		{
			RegisterComponent("MyComponent", typeof(BodyRendererViewComponent));

			InitializeView(typeof(WithComponent));

			AssertThrows<AspViewException>("This component does not have a body content to be rendered", delegate 
			{
				view.Process();
			});
			
		}

		private delegate void Action();

		static void AssertThrows<T>(string message, Action action)
			where T : Exception
		{
			bool wasTheAssertedException = false;
			string catchedMessage = null;
			try
			{
				action();
			}
			catch(T ex)
			{
				wasTheAssertedException = true;
				catchedMessage = ex.Message;
			}
			if (wasTheAssertedException == false)
				Assert.Fail("Exception {0} should have been throwned", typeof(T));
			else if (message != catchedMessage)
				Assert.Fail("Error message was \"{0}\" instead of \"{1}\"", catchedMessage, message);
		}

		[Test]
		public void WithSections_RendersTheSections()
		{
			RegisterComponent("MyComponent", typeof(SectionsRendererViewComponent));

			InitializeView(typeof(WithComponentAndSections));

			view.Process();

			expected =
@"Parent
section1Textsection2
Parent";

			AssertViewOutputEqualsToExpected();
		}

		
		[Test]
		public void WithViewComponent_ParameterNamesAreCaseInsentive() {

			// if component parameter's names where case sensitive 
			// WithMandatoryParameterViewComponent would throw because 'Text' parameter was not set

			#region test context var
			string text = "insensitive check pass";
			#endregion
			
			RegisterComponent("withmandatoryparameter", typeof(WithMandatoryParameterViewComponent));
		
			InitializeView(typeof(WithComponentWithParameter));
			
			view.Properties["text"] = text;
			
			view.Process();

			expected = text;
			AssertViewOutputEqualsToExpected();

		}
		/*
component with subview in view
component with subview in section
component with subview in body
nested components
		 * */
	}
}
