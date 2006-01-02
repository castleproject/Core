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

namespace Castle.MonoRail.Framework.Views.NVelocity.Tests
{
	using System;

	using NUnit.Framework;

	using Castle.MonoRail.TestSupport;

	[TestFixture]
	public class ComponentsTestCase : AbstractMRTestCase
	{
		[Test]
		public void InlineComponentUsingRenderText()
		{
			DoGet("usingcomponent/index1.rails");

			AssertSuccess();

			AssertOutput( "static 1\r\nHello from SimpleInlineViewComponent static 2", Output );
		}

		[Test]
		public void InlineComponentUsingRender()
		{
			DoGet("usingcomponent/index6.rails");

			AssertSuccess();

			AssertOutput( "static 1\r\nThis is a view used by a component static 2", Output );
		}

		[Test]
		public void InlineComponentNotOverridingRender()
		{
			DoGet("usingcomponent/index3.rails");

			AssertSuccess();

			AssertOutput( "static 1\r\ndefault component view picked up automatically static 2", Output );
		}

		[Test]
		public void InlineComponentWithParam1()
		{
			DoGet("usingcomponent/index4.rails");

			AssertSuccess();

			AssertOutput( "Done 1", Output );
		}

		[Test]
		public void BlockComp1()
		{
			DoGet("usingcomponent/index5.rails");

			AssertSuccess();

			AssertOutput( "  item 0\r\n  item 1\r\n  item 2\r\n", Output );
		}

		[Test]
		public void BlockWithinForEach()
		{
			DoGet("usingcomponent/index8.rails");

			AssertSuccess();

			AssertOutput( "inner content 1\r\ninner content 2\r\n", Output );
		}

		[Test]
		public void SeveralComponentsInvocation()
		{
			for(int i=0; i < 10; i++)
			{
				DoGet("usingcomponent/index9.rails");

				AssertSuccess();

				AssertOutput( "static 1\r\nContent 1\r\nstatic 2\r\nContent 2\r\nstatic 3\r\nContent 3\r\nstatic 4\r\nContent 4\r\nstatic 5\r\nContent 5\r\n", Output );
			}
		}

		void AssertOutput(String expected, object output)
		{
			Assert.AreEqual(NormalizeWhitespace(expected), NormalizeWhitespace(output.ToString()));
		}

		String NormalizeWhitespace(String s)
		{
			return s.Replace("\r\n", "\n");
		}
	}
}
