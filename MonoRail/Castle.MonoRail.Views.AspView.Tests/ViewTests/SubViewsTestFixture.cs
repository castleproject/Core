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
	using Views;
	using NUnit.Framework;

	[TestFixture]
	public class SubViewsTestFixture : AbstractViewTestFixture
	{
		[Test]
		public void SubViewWithNoProperties_Works()
		{
			InitializeView(typeof(WithSubView));

			AddCompilation("subview", typeof(SimpleView));

			view.Process();

			expected =
@"Parent
Simple
Parent";
			AssertViewOutputEqualsToExpected();
		}

		[Test]
		public void SubViewCanSeePropertyBag()
		{
			propertyBag["prop"] = "property";

			InitializeView(typeof(WithSubView));

			AddCompilation("subview", typeof(WithProperty));

			view.Process();

			expected =
@"Parent
ViewWithProperty
property
ViewWithProperty
Parent";
			AssertViewOutputEqualsToExpected();
		}

		[Test]
		public void SubViewCanSeeFlash()
		{
			flash["prop"] = "property";

			InitializeView(typeof(WithSubView));

			AddCompilation("subview", typeof(WithProperty));

			view.Process();

			expected =
@"Parent
ViewWithProperty
property
ViewWithProperty
Parent";
			AssertViewOutputEqualsToExpected();
		}

		[Test]
		public void SubViewCanSeeParameters()
		{
			InitializeView(typeof(WithSubViewWithParameters));

			AddCompilation("subview", typeof(WithProperty));

			view.Process();

			expected =
@"Parent
ViewWithProperty
property
ViewWithProperty
Parent";
			AssertViewOutputEqualsToExpected();
		}

		[Test]
		public void SubViewParametersOverrideViewParameter()
		{
			propertyBag["prop"] = "yoodle";

			InitializeView(typeof(WithSubViewWithParameters));

			AddCompilation("subview", typeof(WithProperty));

			view.Process();

			expected =
@"Parent
ViewWithProperty
property
ViewWithProperty
Parent";
			AssertViewOutputEqualsToExpected();
		}

		[Test]
		public void SubViewParametersAreHiddenFromTheView()
		{
			propertyBag["prop"] = "yoodle";

			InitializeView(typeof(WithPropertyAndWithSubViewWithParameters));

			AddCompilation("subview", typeof(WithProperty));

			view.Process();

			expected =
@"Parent
ViewWithProperty
property
ViewWithProperty
yoodle
Parent";
			AssertViewOutputEqualsToExpected();
		}

		[Test]
		public void SubViewCanBeRenderedFromLayout()
		{
			propertyBag["prop"] = "yoodle";

			InitializeView(typeof(SimpleView));
			SetLayout(typeof(LayoutWithSubView));

			AddCompilation("subview", typeof(WithProperty));

			view.Process();

			expected =
@"Layout - before
Simple
Layout - after
ViewWithProperty
yoodle
ViewWithProperty
Layout - after SubView";
			AssertViewOutputEqualsToExpected();
		}
	}
}
