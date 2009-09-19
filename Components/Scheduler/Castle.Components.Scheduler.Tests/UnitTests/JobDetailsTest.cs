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
	public class JobDetailsTest : BaseUnitTest
	{
		private static readonly Guid SchedulerGuid = Guid.NewGuid();

		private readonly JobSpec jobSpec = new JobSpec("abc", "some job", "with.this.key",
		                                               PeriodicTrigger.CreateDailyTrigger(DateTime.UtcNow));

		[Test]
		public void ConstructorSetsProperties()
		{
			JobDetails jobDetails = new JobDetails(jobSpec, new DateTime(2000, 4, 2));
			Assert.AreSame(jobSpec, jobDetails.JobSpec);
			DateTimeAssert.AreEqualIncludingKind(new DateTime(2000, 4, 2, 0, 0, 0, DateTimeKind.Utc), jobDetails.CreationTimeUtc);
			Assert.AreEqual(JobState.Pending, jobDetails.JobState);
			Assert.IsNull(jobDetails.NextTriggerFireTimeUtc);
			Assert.IsNull(jobDetails.NextTriggerMisfireThreshold);
			Assert.IsNull(jobDetails.LastJobExecutionDetails);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void ConstructorThrowsWhenJobSpecIsNull()
		{
			new JobDetails(null, DateTime.UtcNow);
		}

		[Test]
		public void JobSpec_GetterAndSetter()
		{
			JobDetails jobDetails = new JobDetails(jobSpec, DateTime.UtcNow);

			JobSpec newJobSpec = jobSpec.Clone();
			jobDetails.JobSpec = newJobSpec;
			Assert.AreSame(newJobSpec, jobDetails.JobSpec);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void JobSpec_ThrowsIfValueIsNull()
		{
			JobDetails jobDetails = new JobDetails(jobSpec, DateTime.UtcNow);
			jobDetails.JobSpec = null;
		}

		[Test]
		public void JobState_GetterAndSetter()
		{
			JobDetails jobDetails = new JobDetails(jobSpec, DateTime.UtcNow);

			jobDetails.JobState = JobState.Scheduled;
			Assert.AreEqual(JobState.Scheduled, jobDetails.JobState);
		}

		[Test]
		public void CreationTimeUtc_GetterAndSetter()
		{
			JobDetails jobDetails = new JobDetails(jobSpec, DateTime.UtcNow);

			jobDetails.CreationTimeUtc = new DateTime(1970, 1, 1);
			DateTimeAssert.AreEqualIncludingKind(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), jobDetails.CreationTimeUtc);
		}

		[Test]
		public void NextTriggerFireTimeUtc_GetterAndSetter()
		{
			JobDetails jobDetails = new JobDetails(jobSpec, DateTime.UtcNow);

			jobDetails.NextTriggerFireTimeUtc = new DateTime(1970, 1, 1);
			DateTimeAssert.AreEqualIncludingKind(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc),
			                                     jobDetails.NextTriggerFireTimeUtc);
		}

		[Test]
		public void NextTriggerMisfireThreshold_GetterAndSetter()
		{
			JobDetails jobDetails = new JobDetails(jobSpec, DateTime.UtcNow);

			jobDetails.NextTriggerMisfireThreshold = new TimeSpan(0, 1, 0);
			Assert.AreEqual(new TimeSpan(0, 1, 0), jobDetails.NextTriggerMisfireThreshold);
		}

		[Test]
		public void LastJobExecutionDetails_GetterAndSetter()
		{
			JobDetails jobDetails = new JobDetails(jobSpec, DateTime.UtcNow);

			JobExecutionDetails jobExecutionDetails = new JobExecutionDetails(SchedulerGuid, DateTime.UtcNow);
			jobDetails.LastJobExecutionDetails = jobExecutionDetails;
			Assert.AreSame(jobExecutionDetails, jobDetails.LastJobExecutionDetails);
		}

		[TestCase(false)]
		[TestCase(true)]
		public void ClonePerformsADeepCopy(bool useGenericClonable)
		{
			JobDetails jobDetails = new JobDetails(jobSpec, DateTime.UtcNow);
			jobDetails.LastJobExecutionDetails = new JobExecutionDetails(SchedulerGuid, DateTime.UtcNow);
			jobDetails.JobState = JobState.Scheduled;
			jobDetails.NextTriggerFireTimeUtc = DateTime.UtcNow;
			jobDetails.NextTriggerMisfireThreshold = TimeSpan.MaxValue;

			JobDetails clone = useGenericClonable
			                   	? jobDetails.Clone()
			                   	: (JobDetails) ((ICloneable) jobDetails).Clone();

			Assert.AreNotSame(jobDetails, clone);
			Assert.AreNotSame(jobDetails.JobSpec, clone.JobSpec);
			Assert.AreNotSame(jobDetails.LastJobExecutionDetails, clone.LastJobExecutionDetails);

			JobAssert.AreEqual(jobDetails, clone);
		}
	}
}