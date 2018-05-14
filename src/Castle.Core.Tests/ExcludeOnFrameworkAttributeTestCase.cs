// Copyright 2004-2018 Castle Project - http://www.castleproject.org/
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

namespace Castle
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using NUnit.Framework;
	using NUnit.Framework.Interfaces;
	using NUnit.Framework.Internal;

	[TestFixture]
	public class ExcludeOnFrameworkAttributeTestCase
	{
		[Test]
		public void ExcludeOnFramework_marks_test_as_skipped_when_specified_framework_equal_to_current_framework()
		{
			var couldIdentifyRunningFramework = FrameworkUtil.TryGetRunningFramework(out var runningFramework);
			Assume.That(couldIdentifyRunningFramework);

			var attribute = new ExcludeOnFrameworkAttribute(runningFramework, "Blah.");
			var test = new MockTest();

			attribute.ApplyToTest(test);

			Assert.AreEqual(RunState.Skipped, test.RunState);
			Assert.True(test.Properties.ContainsKey(PropertyNames.SkipReason));
			Assert.True(test.Properties[PropertyNames.SkipReason].OfType<string>().Any(sr => sr.Contains("Blah.")));
		}

		[Test]
		public void ExcludeOnFramework_does_not_mark_test_as_skipped_when_specified_framework_not_equal_to_current_framework()
		{
			const Framework noFramework = 0;

			var attribute = new ExcludeOnFrameworkAttribute(noFramework, "Blah.");
			var test = new MockTest();

			attribute.ApplyToTest(test);

			Assert.AreNotEqual(RunState.Skipped, test.RunState);
			Assert.False(test.Properties.ContainsKey(PropertyNames.SkipReason));
		}

		private sealed class MockTest : Test
		{
			public MockTest() : base("TestName") { }
			public override string XmlElementName => throw new NotImplementedException();
			public override bool HasChildren => throw new NotImplementedException();
			public override IList<ITest> Tests => throw new NotImplementedException();
			public override TNode AddToXml(TNode parentNode, bool recursive) => throw new NotImplementedException();
			public override TestResult MakeTestResult() => throw new NotImplementedException();
		}
	}
}
