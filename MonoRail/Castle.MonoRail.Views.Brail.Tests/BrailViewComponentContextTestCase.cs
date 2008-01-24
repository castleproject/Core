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
	using System.Collections;

	using NUnit.Framework;

	[TestFixture]
	public class BrailViewComponentContextTestCase
	{
		[Test]
		public void ShouldConvertIgnoreNulls_InViewComponentParameters_ToTargetTypes()
		{
			IDictionary parameters = new Hashtable(1);
			parameters["Message"] = new ExpandDucktype_WorkaroundForDuplicateVirtualMethodsTestCase.IgnoreNullForTest("Hello");
			BrailViewComponentContext context = new BrailViewComponentContext(null, null, "", null, parameters);
			Assert.AreEqual("Hello", context.ComponentParameters["Message"]);
		}
	}
}