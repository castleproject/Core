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

namespace Castle.Components.Scheduler.Tests.UnitTests.JobStores
{
	using System;
	using System.Diagnostics;
	using System.Threading;
	using NUnit.Framework;
	using Scheduler.JobStores;

	[TestFixture]
	public abstract class PersistentJobStoreTest : BaseJobStoreTest
	{
		public new PersistentJobStore JobStore
		{
			get { return (PersistentJobStore) base.JobStore; }
		}

		protected override BaseJobStore CreateJobStore()
		{
			PersistentJobStore jobStore = CreatePersistentJobStore();
			Assert.AreEqual(15, jobStore.PollIntervalInSeconds);
			Assert.AreEqual(120, jobStore.SchedulerExpirationTimeInSeconds);

			jobStore.PollIntervalInSeconds = 1;
			jobStore.SchedulerExpirationTimeInSeconds = 5;
			return jobStore;
		}

		protected abstract PersistentJobStore CreatePersistentJobStore();

		/// <summary>
		/// Sets whether subsequent Db connection requests for the specified job store
		/// should be caused to fail.
		/// </summary>
		protected abstract void SetBrokenConnectionMocking(PersistentJobStore jobStore, bool brokenConnections);

		[Test]
		public void ClusterName_GetterAndSetter()
		{
			Assert.AreEqual("Default", JobStore.ClusterName);

			JobStore.ClusterName = "Cluster";
			Assert.AreEqual("Cluster", JobStore.ClusterName);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void ClusterName_ThrowsIfValueIsNull()
		{
			JobStore.ClusterName = null;
		}

		[Test]
		public void PollIntervalInSeconds_GetterAndSetter()
		{
			JobStore.PollIntervalInSeconds = 15;
			Assert.AreEqual(15, JobStore.PollIntervalInSeconds);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void PollIntervalInSeconds_ThrowsIfValueIsZero()
		{
			JobStore.PollIntervalInSeconds = 0;
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void PollIntervalInSeconds_ThrowsIfValueIsNegative()
		{
			JobStore.PollIntervalInSeconds = -1;
		}

		[Test]
		public void SchedulerExpirationTimeInSeconds_GetterAndSetter()
		{
			JobStore.SchedulerExpirationTimeInSeconds = 15;
			Assert.AreEqual(15, JobStore.SchedulerExpirationTimeInSeconds);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void SchedulerExpirationTimeInSeconds_ThrowsIfValueIsZero()
		{
			JobStore.SchedulerExpirationTimeInSeconds = 0;
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void SchedulerExpirationTimeInSeconds_ThrowsIfValueIsNegative()
		{
			JobStore.SchedulerExpirationTimeInSeconds = -1;
		}

		[Test]
		[ExpectedException(typeof (SchedulerException))]
		public void RegisterScheduler_WrapsExceptionIfDbConnectionFailureOccurs()
		{
			Mocks.ReplayAll();

			SetBrokenConnectionMocking(JobStore, true);
			JobStore.RegisterScheduler(SchedulerGuid, SchedulerName);
		}

		[Test]
		[ExpectedException(typeof (SchedulerException))]
		public void UnregisterScheduler_WrapsExceptionIfDbConnectionFailureOccurs()
		{
			Mocks.ReplayAll();

			SetBrokenConnectionMocking(JobStore, true);
			JobStore.UnregisterScheduler(SchedulerGuid);
		}

		[Test]
		[ExpectedException(typeof (SchedulerException))]
		public void CreateJob_WrapsExceptionIfDbConnectionFailureOccurs()
		{
			Mocks.ReplayAll();

			SetBrokenConnectionMocking(JobStore, true);
			JobStore.CreateJob(dummyJobSpec, DateTime.UtcNow, CreateJobConflictAction.Ignore);
		}

		[Test]
		[ExpectedException(typeof (SchedulerException))]
		public void UpdateJob_WrapsExceptionIfDbConnectionFailureOccurs()
		{
			Mocks.ReplayAll();

			SetBrokenConnectionMocking(JobStore, true);
			JobStore.UpdateJob("foo", dummyJobSpec);
		}

		[Test]
		[ExpectedException(typeof (SchedulerException))]
		public void DeleteJob_WrapsExceptionIfDbConnectionFailureOccurs()
		{
			Mocks.ReplayAll();

			SetBrokenConnectionMocking(JobStore, true);
			JobStore.DeleteJob("foo");
		}

		[Test]
		[ExpectedException(typeof (SchedulerException))]
		public void GetJobDetails_WrapsExceptionIfDbConnectionFailureOccurs()
		{
			Mocks.ReplayAll();

			SetBrokenConnectionMocking(JobStore, true);
			JobStore.GetJobDetails("foo");
		}

		[Test]
		public void UpdateJob_IncrementsVersionNumber()
		{
			Mocks.ReplayAll();

			VersionedJobDetails jobDetails = (VersionedJobDetails) CreatePendingJob("job", DateTime.UtcNow);
			int originalVersion = jobDetails.Version;

			jobDetails.JobSpec.Name = "renamedJob";
			JobStore.UpdateJob("job", jobDetails.JobSpec);

			VersionedJobDetails updatedJobDetails = (VersionedJobDetails) JobStore.GetJobDetails("renamedJob");
			Assert.AreEqual(originalVersion + 1, updatedJobDetails.Version,
			                "Version number of saved object should be incremented in database.");
		}

		[Test]
		public void SaveJobDetails_IncrementsVersionNumber()
		{
			Mocks.ReplayAll();

			VersionedJobDetails jobDetails = (VersionedJobDetails) CreatePendingJob("job", DateTime.UtcNow);
			int originalVersion = jobDetails.Version;

			jobDetails.JobState = JobState.Stopped;
			JobStore.SaveJobDetails(jobDetails);

			Assert.AreEqual(originalVersion + 1, jobDetails.Version,
			                "Version number of original object should be incremented in place.");

			VersionedJobDetails updatedJobDetails = (VersionedJobDetails) JobStore.GetJobDetails("job");
			Assert.AreEqual(originalVersion + 1, updatedJobDetails.Version,
			                "Version number of saved object should be incremented in database.");
		}

		[Test]
		[ExpectedException(typeof (SchedulerException))]
		public void SaveJobDetails_WrapsExceptionIfDbConnectionFailureOccurs()
		{
			Mocks.ReplayAll();

			JobDetails jobDetails = CreatePendingJob("job", DateTime.UtcNow);

			SetBrokenConnectionMocking(JobStore, true);
			JobStore.SaveJobDetails(jobDetails);
		}

		[Test]
		[ExpectedException(typeof (SchedulerException))]
		public void ListJobNames_WrapsExceptionIfDbConnectionFailureOccurs()
		{
			Mocks.ReplayAll();

			SetBrokenConnectionMocking(JobStore, true);
			JobStore.ListJobNames();
		}

		[Test]
		public void JobWatcher_IgnoresExceptionIfDbConnectionFailureOccurs()
		{
			Mocks.ReplayAll();

			SetBrokenConnectionMocking(JobStore, true);

			IJobWatcher jobWatcher = JobStore.CreateJobWatcher(SchedulerGuid);

			// This could throw an exception but instead we catch and log it then keep going
			// until we are disposed.
			Stopwatch stopwatch = Stopwatch.StartNew();

			ThreadPool.QueueUserWorkItem(delegate
			{
				Thread.Sleep(2000);
				jobWatcher.Dispose();
			});

			Assert.IsNull(jobWatcher.GetNextJobToProcess());
			Assert.That(stopwatch.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(2000).And.LessThanOrEqualTo(4000), "Check that the thread was blocked the whole time.");
		}

		[Test]
		public void RunningJobsAreOrphanedWhenSchedulerRegistrationRefreshFails()
		{
			Mocks.ReplayAll();

			CreateRunningJob("running-job");

			SetBrokenConnectionMocking(JobStore, true);
			JobStore.SchedulerExpirationTimeInSeconds = 1;

			// Allow some time for the expiration time to expire.
			Thread.Sleep(3000);

			// Now get a new scheduler.
			// Its next job up for processing should be the one that we created earlier
			// but now it will be Orphaned.
			PersistentJobStore newJobStore = CreatePersistentJobStore();
			newJobStore.SchedulerExpirationTimeInSeconds = 1;
			newJobStore.PollIntervalInSeconds = 1;

			IJobWatcher jobWatcher = newJobStore.CreateJobWatcher(Guid.NewGuid());
			JobDetails orphanedJob = jobWatcher.GetNextJobToProcess();

			Assert.AreEqual("running-job", orphanedJob.JobSpec.Name);
			Assert.AreEqual(JobState.Orphaned, orphanedJob.JobState);
			Assert.AreEqual(false, orphanedJob.LastJobExecutionDetails.Succeeded);
			Assert.IsNotNull(orphanedJob.LastJobExecutionDetails.EndTimeUtc);
		}
	}
}