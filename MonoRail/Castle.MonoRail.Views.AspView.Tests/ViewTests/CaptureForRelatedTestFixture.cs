// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
	using Framework.ViewComponents;
	using Views;
	using NUnit.Framework;

	[TestFixture]	
	public class CaptureForRelatedTestFixture : AbstractViewComponentsTestFixture
	{
		protected override void  ExtraSetUp()
		{
			base.ExtraSetUp();
			RegisterComponent("CaptureFor", typeof(CaptureFor));
		}

		[Test]
		public void CaptureForInView_VisibleInLayout()
		{
			InitializeView(typeof(WithCaptureFor));
			SetLayout(typeof(LayoutUsesCaptureFor));
			view.Process();

			expected = @"View: Parent

Parent
From CaptureFor: The captured content";

			AssertViewOutputEqualsToExpected();
		}

		[Test]
		public void CaptureForInSubView_VisibleInLayout()
		{
			AddCompilation("subview", typeof(WithCaptureFor));
			InitializeView(typeof(WithSubView));
			SetLayout(typeof(LayoutUsesCaptureFor));
			view.Process();

			expected = @"View: Parent
Parent

Parent
Parent
From CaptureFor: The captured content";

			AssertViewOutputEqualsToExpected();
		}

		[Test]
		public void CaptureForInSubView_VisibleInView()
		{
			AddCompilation("subview", typeof(WithCaptureFor));
			InitializeView(typeof(UsingBubbledCaptureFromSubView));
			view.Process();

			expected = @"Parnt View
From subview: Parent

Parent
From CaptureFor: The captured content";

			AssertViewOutputEqualsToExpected();
		}
		
		[Test]
		public void TwoCaptureForInSubViewsWithAppend_AllGetsAppended()
		{
			AddCompilation("subview", typeof(WithCaptureForAppendAfter));
			InitializeView(typeof(UsingTwoBubbledCaptureFromSubViews));
			view.Process();

			expected = @"Starting UsingTwoBubbledCaptureFromSubViews
From subview: Starting WithCaptureForAppendAfter
End of WithCaptureForAppendAfter
From subview, again: Starting WithCaptureForAppendAfter
End of WithCaptureForAppendAfter
From CaptureFor: The captured contentThe captured content
End of UsingTwoBubbledCaptureFromSubViews";
			AssertViewOutputEqualsToExpected();
		}
	}
}
