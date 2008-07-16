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

namespace Castle.MonoRail.Views.AspView.Tests.Compiler.MarkupTransformers
{
	using NUnit.Framework;
	using AspView.Compiler.MarkupTransformers;
	
	[TestFixture]	
	public class SiteRootMarkupTransformerTestFixture : AbstractMarkupTransformerTestFixture
	{
		protected override void CreateTransformer()
		{
			transformer = new SiteRootMarkupTransformer();
		}

		[Test]
		public void Transform_WhenNoSiteRoot_DoNothing()
		{
			input = "Some stuff";
			expected = input;
		
			AssertTranfromerOutput();
		}

		[Test]
		public void Transform_WhenHasSingleSiteRoot_Transforms()
		{
			input = "~foo";
			expected = "<% Output(siteRoot); %>foo";
			AssertTranfromerOutput();
		}

		[Test]
		public void Transform_WhenHasMoreThanOneSiteRoot_Transforms()
		{
			input = "~foo ~bar";
			expected = "<% Output(siteRoot); %>foo <% Output(siteRoot); %>bar";
			AssertTranfromerOutput();
		}
	}
}
