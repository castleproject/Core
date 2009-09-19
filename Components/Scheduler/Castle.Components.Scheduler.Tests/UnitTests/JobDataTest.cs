// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.Scheduler.Tests.UnitTests
{
	using System;
	using System.Collections.Generic;
	using NUnit.Framework;
	using Utilities;

	[TestFixture]
	public class JobDataTest : BaseUnitTest
	{
		[Test]
		public void DefaultConstructorCreatesEmptyDictionary()
		{
			JobData jobData = new JobData();
			Assert.AreEqual(0, jobData.State.Count);
		}

		[Test]
		public void ConstructorWithDictionarySetsProperties()
		{
			Dictionary<string, object> dict = new Dictionary<string, object>();
			JobData jobData = new JobData(dict);
			Assert.AreSame(dict, jobData.State);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void ConstructorWithDictionaryThrowsIfDictionaryIsNull()
		{
			new JobData(null);
		}

		[TestCase(false)]
		[TestCase(true)]
		public void ClonePerformsADeepCopy(bool useGenericClonable)
		{
			Dictionary<string, object> dict = new Dictionary<string, object>();
			JobData jobData = new JobData(dict);

			JobData clone = useGenericClonable
			                	? jobData.Clone()
			                	:
			                		(JobData) ((ICloneable) jobData).Clone();

			Assert.AreNotSame(jobData, clone);
			Assert.AreNotSame(dict, clone.State);

			JobAssert.AreEqual(jobData, clone);
		}
	}
}