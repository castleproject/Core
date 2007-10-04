// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	public class JustViewFixture : BaseViewOnlyTestFixture
	{
		[Test]
		public void CanRenderViewWithoutUsingFullMonoRailPipeline()
		{
			string outpot = ProcessView("home/index");
			Assert.AreEqual("Brail is wonderful", outpot);
		}

		[Test]
		public void UsingExtentionMethods()
		{
			IDictionary dictionary = new Hashtable();
			dictionary["items"] = new int[] {};
			string templatePath = "home/extensionMethods";

			string actual = ProcessView(dictionary, templatePath);
			Assert.AreEqual("No Data", actual);
		}


		[Test]
		public void WithParameters()
		{
			IDictionary dictionary = new Hashtable();
			dictionary["list"] = new int[] {2, 5, 7, 8};
			dictionary["name"] = "test";
			string templatePath = "home/bag";

			string actual = ProcessView(dictionary, templatePath);
			string expected = @"test is the name
 2
 5
 7
 8
";
			Assert.AreEqual(expected, actual);
		}
	}
}