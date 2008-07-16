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

namespace Castle.MonoRail.Views.AspView.Tests.RenderingTests
{
	using NUnit.Framework;

	[TestFixture]
	public class SubViewTests : IntegrationViewTestFixture
	{
		[Test]
		public void WhenCallingASubviewOnTheSameDirectoryWithoutArguments_ItRendersProperly()
		{
			const string expected = @"I am view1
I am view2
I am view1 again";
			ProcessView("/subviews/view1");
			AssertReplyEqualTo(expected);
		}

	}
}
