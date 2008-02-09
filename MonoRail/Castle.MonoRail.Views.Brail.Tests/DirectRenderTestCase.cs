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

namespace Castle.MonoRail.Views.Brail.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class DirectRenderTestCase : BaseViewOnlyTestFixture
	{
		[Test]
		public void DirectRendering()
		{
			string expected = "Ayende";
            string actual = RenderStaticWithLayout("Ayende");
            Assert.AreEqual(expected, actual);
		}

		[Test]
		public void DirectRenderingWithLayout()
		{
			string expected = "\r\nWelcome!\r\n<p>Ayende</p>\r\nFooter";
		    Layout = "master";
		    string actual = RenderStaticWithLayout("Ayende");
            Assert.AreEqual(expected, actual);
		}
	}
}