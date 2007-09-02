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
	using System.Threading;
	using Castle.MonoRail.Framework.Tests;
	using NUnit.Framework;

	[TestFixture]
	public class ResourceTestCase : AbstractTestCase
	{
		[Test]
		public void GetResources()
		{
			Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
			string expected = "testValue";
			DoGet("resourced/getresources.rails");
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void GetIndexedResources()
		{
			Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
			string expected = "testValue";
			DoGet("resourced/indexingResources.rails");
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void IterateOnResources()
		{
			Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
			string expected = "testKey: testValue";
			DoGet("resourced/iterating.rails");
			AssertReplyEqualTo(expected);
		}
	}
}
