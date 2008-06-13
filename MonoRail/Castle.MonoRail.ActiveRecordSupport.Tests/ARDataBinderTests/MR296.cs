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

namespace Castle.MonoRail.ActiveRecordSupport.Tests.ARDataBinderTests
{
	using NUnit.Framework;
	using WatiN.Core;

	[TestFixture, Ignore("can't run successfully under cassini")]
	public class MR296 : BaseAcceptanceTestCase
	{
		[Test]
		public void IfControllerDoesNotInheritFromARSmartDispatcherController_ShouldStillGetValidationSummary()
		{
			ie.GoTo("http://localhost:88/MR296/edit.castle");
			ie.Button(Find.ByValue("Save")).Click();
			Assert.IsTrue(ie.ContainsText("HasValidationErrors"));
		}
	}
}