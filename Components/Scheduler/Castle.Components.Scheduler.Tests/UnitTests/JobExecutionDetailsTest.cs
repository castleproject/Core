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
	using NUnit.Framework;
	using Utilities;

	[TestFixture]
	public class JobExecutionDetailsTest : BaseUnitTest
	{
		private static readonly Guid SchedulerGuid = Guid.NewGuid();

		[Test]
		public void ConstructorSetsProperties()
		{
			JobExecutionDetails details = new JobExecutionDetails(SchedulerGuid, new DateTime(2000, 3, 4));

			Assert.AreEqual(SchedulerGuid, details.SchedulerGuid);
			DateTimeAssert.AreEqualIncludingKind(new DateTime(2000, 3, 4, 0, 0, 0, DateTimeKind.Utc), details.StartTimeUtc);
			Assert.IsNull(details.EndTimeUtc);
			Assert.IsFalse(details.Succeeded);
			Assert.AreEqual("Unknown", details.StatusMessage);
		}

		[Test]
		public void EndTimeUtc_GetterAndSetter()
		{
			JobExecutionDetails details = new JobExecutionDetails(SchedulerGuid, DateTime.UtcNow);

			details.EndTimeUtc = new DateTime(1970, 1, 2);
			DateTimeAssert.AreEqualIncludingKind(new DateTime(1970, 1, 2, 0, 0, 0, DateTimeKind.Utc), details.EndTimeUtc);
		}

		[Test]
		public void Succeeded_GetterAndSetter()
		{
			JobExecutionDetails details = new JobExecutionDetails(SchedulerGuid, DateTime.UtcNow);

			details.Succeeded = true;
			Assert.IsTrue(details.Succeeded);
		}

		[Test]
		public void StatusMessage_GetterAndSetter()
		{
			JobExecutionDetails details = new JobExecutionDetails(SchedulerGuid, DateTime.UtcNow);

			details.StatusMessage = "Test test test";
			Assert.AreEqual("Test test test", details.StatusMessage);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void StatusMessage_ThrowsIfValueIsNull()
		{
			JobExecutionDetails details = new JobExecutionDetails(SchedulerGuid, DateTime.UtcNow);
			details.StatusMessage = null;
		}

		[TestCase(false)]
		[TestCase(true)]
		public void ClonePerformsADeepCopy(bool useGenericClonable)
		{
			DateTime now = DateTime.UtcNow;
			JobExecutionDetails details = new JobExecutionDetails(SchedulerGuid, now);
			details.EndTimeUtc = new DateTime(2000, 3, 4);
			details.Succeeded = true;
			details.StatusMessage = "Blah";

			JobExecutionDetails clone = useGenericClonable
			                            	? details.Clone()
			                            	: (JobExecutionDetails) ((ICloneable) details).Clone();

			Assert.AreNotSame(details, clone);

			JobAssert.AreEqual(details, clone);
		}
	}
}