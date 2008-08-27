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
	public class LayoutExtensionOptions : IntegrationViewTestFixture
	{
		[Test]
		public void LayoutWithAspxExtension_WouldWork()
		{
			const string expected = @"regular layout
I am sane
regular layout again";
			Layout = "regular";
			ProcessView("/sanity/sanity");
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void LayoutWithMasterExtension_WouldWork()
		{
			const string expected = @"master layout
I am sane
master layout again";
			Layout = "master";
			ProcessView("/sanity/sanity");
			AssertReplyEqualTo(expected);
		}


		[Test]
		public void NestingLayoutsWithDifferentExtensions_WouldWork()
		{
			const string expected = @"regular layout
master layout
I am sane
master layout again
regular layout again";
			Layout = "regular,master";
			ProcessView("/sanity/sanity");
			AssertReplyEqualTo(expected);
		}
	}
}
