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
	using System.Collections.Generic;
	using System.Threading;
	using Core.Logging;
	using NUnit.Framework;
	using Scheduler.JobStores;
	using Utilities;

	/// <summary>
	/// Base tests for a job store.
	/// </summary>
	[TestFixture]
	public abstract class BaseJobStoreTest : BaseUnitTest
	{
		protected static readonly Guid SchedulerGuid = Guid.NewGuid();
		protected const string SchedulerName = "test";

		private BaseJobStore jobStore;

		protected JobSpec dummyJobSpec;
		protected JobData dummyJobData;

		/// <summary>
		/// Gets the job store to be tested.
		/// </summary>
		protected BaseJobStore JobStore
		{
			get { return jobStore; }
			set { jobStore = value; }
		}

		public override void SetUp()
		{
			base.SetUp();

			dummyJobSpec = new JobSpec("job", "test", "dummy", PeriodicTrigger.CreateOneShotTrigger(DateTime.UtcNow));
			dummyJobData = new JobData();
			dummyJobData.State["key"] = "value";

			jobStore = CreateJobStore();
			jobStore.RegisterScheduler(SchedulerGuid, SchedulerName);
		}

		public override void TearDown()
		{
			if (! jobStore.IsDisposed)
				jobStore.Dispose();

			base.TearDown();
		}

		/// <summary>
		/// Creates the job store to be tested.
		/// </summary>
		/// <returns>The job store</returns>
		protected abstract BaseJobStore CreateJobStore();

		[Test]
		public void Logger_GetterAndSetter()
		{
			Mocks.ReplayAll();

			Assert.AreSame(NullLogger.Instance, jobStore.Logger);

			ILogger mockLogger = Mocks.CreateMock<ILogger>();
			jobStore.Logger = mockLogger;
			Assert.AreSame(mockLogger, jobStore.Logger);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Logger_ThrowsIfValueIsNull()
		{
			Mocks.ReplayAll();

			jobStore.Logger = null;
		}

		[Test]
		public void IsDisposed_ChangesToTrueWhenDisposed()
		{
			Mocks.ReplayAll();

			Assert.IsFalse(jobStore.IsDisposed);

			jobStore.Dispose();
			Assert.IsTrue(jobStore.IsDisposed);
		}

		/// <summary>
		/// There are no strong contracts to be checked regarding scheduler registration
		/// with a job store, it is just supposed to happen.  So there isn't much that
		/// we can check generically across all job store implementations.
		/// </summary>
		[Test]
		public void RegisterScheduler_DoesNotCrashWhenCalledRedundantly()
		{
			Mocks.ReplayAll();

			jobStore.RegisterScheduler(SchedulerGuid, SchedulerName);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void RegisterScheduler_ThrowsIfSchedulerNameIsNull()
		{
			Mocks.ReplayAll();

			jobStore.RegisterScheduler(SchedulerGuid, null);
		}

		[Test]
		public void UnregisterScheduler_OrphansRunningJobs()
		{
			Mocks.ReplayAll();

			JobDetails jobDetails = CreatePendingJob("job", DateTime.UtcNow);
			jobDetails.LastJobExecutionDetails = new JobExecutionDetails(SchedulerGuid, DateTime.UtcNow);
			jobDetails.JobState = JobState.Running;
			jobStore.SaveJobDetails(jobDetails);

			jobStore.UnregisterScheduler(SchedulerGuid);

			jobDetails = jobStore.GetJobDetails("job");
			Assert.AreEqual(JobState.Orphaned, jobDetails.JobState);
		}

		[TestCase(CreateJobConflictAction.Ignore)]
		[TestCase(CreateJobConflictAction.Update)]
		[TestCase(CreateJobConflictAction.Replace)]
		[TestCase(CreateJobConflictAction.Throw)]
		public void CreateJob_ReturnsTrueIfCreated(CreateJobConflictAction conflictAction)
		{
			Assert.IsTrue(jobStore.CreateJob(dummyJobSpec, DateTime.UtcNow, conflictAction));
		}

		[TestCase(CreateJobConflictAction.Ignore, false)]
		[TestCase(CreateJobConflictAction.Update, true)]
		[TestCase(CreateJobConflictAction.Replace, true)]
		[TestCase(CreateJobConflictAction.Throw, false, ExpectedException = typeof (SchedulerException))]
		public void CreateJob_HandlesDuplicatesAccordingToConflictAction(CreateJobConflictAction conflictAction,
		                                                                 bool expectedResult)
		{
			Assert.IsTrue(jobStore.CreateJob(dummyJobSpec, new DateTime(2007, 5, 31), CreateJobConflictAction.Ignore));

			Assert.AreEqual(expectedResult, jobStore.CreateJob(dummyJobSpec, new DateTime(2007, 6, 1), conflictAction));

			if (expectedResult)
			{
				JobDetails jobDetails = jobStore.GetJobDetails(dummyJobSpec.Name);

				// In Update mode, the creation time should remain the same as the original job.
				// We should also preserve its history (though we don't check this right now.  -- TODO)
				if (conflictAction == CreateJobConflictAction.Update)
					DateTimeAssert.AreEqualIncludingKind(new DateTime(2007, 5, 31, 0, 0, 0, DateTimeKind.Utc),
					                                     jobDetails.CreationTimeUtc);

				// In Replace mode, the creation time should be that of the newly created job.
				if (conflictAction == CreateJobConflictAction.Replace)
					DateTimeAssert.AreEqualIncludingKind(new DateTime(2007, 6, 1, 0, 0, 0, DateTimeKind.Utc),
					                                     jobDetails.CreationTimeUtc);
			}
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void CreateJob_ThrowsIfJobSpecIsNull()
		{
			jobStore.CreateJob(null, DateTime.UtcNow, CreateJobConflictAction.Ignore);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void CreateJob_ThrowsIfConflictActionIsInvalid()
		{
			jobStore.CreateJob(dummyJobSpec, DateTime.UtcNow, (CreateJobConflictAction) 9999);
		}

		[Test]
		[ExpectedException(typeof (ObjectDisposedException))]
		public void CreateJob_ThrowsIfDisposed()
		{
			jobStore.Dispose();
			jobStore.CreateJob(dummyJobSpec, DateTime.UtcNow, CreateJobConflictAction.Ignore);
		}

		[Test]
		public void UpdateJob_CanRenameJobsAndAlterOtherProperties()
		{
			Mocks.ReplayAll();

			JobDetails originalJob = CreateRunningJob("originalJob");

			JobSpec updatedJobSpec = new JobSpec("updatedJob", "This is updated.", "The new key",
			                                     PeriodicTrigger.CreateOneShotTrigger(new DateTime(2000, 3, 4)));

			jobStore.UpdateJob("originalJob", updatedJobSpec);

			Assert.IsNull(jobStore.GetJobDetails("originalJob"), "The job should not be accessible under its original name.");

			// The job gets a new job spec, but all other properties should be preserved including
			// the job's execution history.
			JobDetails updatedJob = jobStore.GetJobDetails("updatedJob");

			JobDetails expectedUpdatedJob = originalJob.Clone();
			expectedUpdatedJob.JobSpec = updatedJobSpec;

			JobAssert.AreEqual(expectedUpdatedJob, updatedJob);
		}

		[TestCase(JobState.Pending, JobState.Pending)]
		[TestCase(JobState.Scheduled, JobState.Pending)]
		[TestCase(JobState.Triggered, JobState.Triggered)]
		[TestCase(JobState.Running, JobState.Running)]
		[TestCase(JobState.Completed, JobState.Completed)]
		[TestCase(JobState.Orphaned, JobState.Orphaned)]
		[TestCase(JobState.Stopped, JobState.Stopped)]
		public void UpdateJob_ResetsTheJobStateAppropriately(JobState initialJobState, JobState expectedJobState)
		{
			Mocks.ReplayAll();

			// Create a job in the specified initial state.
			JobDetails job = CreatePendingJob("job", DateTime.UtcNow);
			job.JobState = initialJobState;
			jobStore.SaveJobDetails(job);

			// Update it.
			jobStore.UpdateJob("job", job.JobSpec);

			// Ensure its state has been updated appropriately.
			JobDetails updatedJob = jobStore.GetJobDetails("job");
			Assert.AreEqual(expectedJobState, updatedJob.JobState);
		}

		[Test]
		[ExpectedException(typeof (SchedulerException))]
		public void UpdateJob_ThrowsIfNoJobExistsWithTheOriginalName()
		{
			Mocks.ReplayAll();

			jobStore.UpdateJob("noSuchJob", dummyJobSpec);
		}

		[Test]
		[ExpectedException(typeof (SchedulerException))]
		public void UpdateJob_ThrowsIfJobWithNewNameAlreadyExists()
		{
			Mocks.ReplayAll();

			JobDetails thisJob = CreatePendingJob("thisJob", DateTime.UtcNow);
			CreatePendingJob("thatJob", DateTime.UtcNow);

			thisJob.JobSpec.Name = "thatJob";
			jobStore.UpdateJob("thisJob", thisJob.JobSpec);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void UpdateJob_ThrowsIfNameIsNull()
		{
			Mocks.ReplayAll();

			jobStore.UpdateJob(null, dummyJobSpec);
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void UpdateJob_ThrowsIfNameIsEmpty()
		{
			Mocks.ReplayAll();

			jobStore.UpdateJob("", dummyJobSpec);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void UpdateJob_ThrowsIfJobSpecIsNull()
		{
			Mocks.ReplayAll();

			jobStore.UpdateJob("job", null);
		}

		[Test]
		[ExpectedException(typeof (ObjectDisposedException))]
		public void UpdateJob_ThrowsIfDisposed()
		{
			Mocks.ReplayAll();

			jobStore.Dispose();
			jobStore.UpdateJob("test", dummyJobSpec);
		}

		[Test]
		public void ListJobNames_ReturnsAllActiveJobNamesAndExcludesAnyDeletedJobs()
		{
			Mocks.ReplayAll();

			CreatePendingJob("thisJob", DateTime.UtcNow);

			JobDetails thatJob = CreatePendingJob("thatJob", DateTime.UtcNow);
			thatJob.JobSpec.Name = "thatJobRenamed";
			jobStore.UpdateJob("thatJob", thatJob.JobSpec);

			CreatePendingJob("deletedJob", DateTime.UtcNow);
			jobStore.DeleteJob("deletedJob");

			string[] jobNames = jobStore.ListJobNames();
			Assert.AreEqual(2, jobNames.Length);
			CollectionAssert.Contains(jobNames, "thisJob");
			CollectionAssert.Contains(jobNames, "thatJobRenamed");
		}

		[Test]
		[ExpectedException(typeof (ObjectDisposedException))]
		public void ListJobNames_ThrowsIfDisposed()
		{
			Mocks.ReplayAll();

			jobStore.Dispose();
			jobStore.ListJobNames();
		}

		[Test]
		public void GetJobDetails_ReturnsCorrectDetailsAfterCreateJob()
		{
			DateTime creationTime = DateTime.UtcNow;
			Assert.IsTrue(jobStore.CreateJob(dummyJobSpec, creationTime, CreateJobConflictAction.Throw));

			JobDetails jobDetails = jobStore.GetJobDetails(dummyJobSpec.Name);
			Assert.IsNotNull(jobDetails);

			JobAssert.AreEqual(dummyJobSpec, jobDetails.JobSpec);

			JobAssert.AreEqualUpToErrorLimit(creationTime, jobDetails.CreationTimeUtc);
			Assert.AreEqual(JobState.Pending, jobDetails.JobState);
			Assert.IsNull(jobDetails.LastJobExecutionDetails);
			Assert.IsNull(jobDetails.NextTriggerFireTimeUtc);
			Assert.IsNull(jobDetails.NextTriggerMisfireThreshold);
		}

		[Test]
		public void GetJobDetails_ReturnsNullIfJobDoesNotExists()
		{
			Assert.IsNull(jobStore.GetJobDetails("non-existant"));
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void GetJobDetails_ThrowsIfJobNameIsNull()
		{
			jobStore.GetJobDetails(null);
		}

		[Test]
		[ExpectedException(typeof (ObjectDisposedException))]
		public void GetJobDetails_ThrowsIfDisposed()
		{
			jobStore.Dispose();
			jobStore.GetJobDetails("job");
		}

		[Test]
		public void SaveJobDetails_RoundTripWithGetJobDetails()
		{
			// Create job and set details.
			Assert.IsTrue(jobStore.CreateJob(dummyJobSpec, DateTime.UtcNow, CreateJobConflictAction.Throw));
			JobDetails savedJobDetails = jobStore.GetJobDetails(dummyJobSpec.Name);

			savedJobDetails.JobState = JobState.Running;
			savedJobDetails.LastJobExecutionDetails = new JobExecutionDetails(SchedulerGuid, DateTime.UtcNow);
			savedJobDetails.LastJobExecutionDetails.StatusMessage = "Status";
			savedJobDetails.LastJobExecutionDetails.Succeeded = true;
			savedJobDetails.LastJobExecutionDetails.EndTimeUtc = new DateTime(1969, 12, 31);
			savedJobDetails.NextTriggerFireTimeUtc = new DateTime(1970, 1, 1);
			savedJobDetails.NextTriggerMisfireThreshold = new TimeSpan(0, 1, 0);
			savedJobDetails.JobSpec.JobData = new JobData();
			savedJobDetails.JobSpec.JobData.State["key"] = "new value";
			savedJobDetails.JobSpec.Trigger.Schedule(TriggerScheduleCondition.Latch, DateTime.MaxValue, null);
			dummyJobSpec.Trigger.Schedule(TriggerScheduleCondition.Latch, DateTime.MaxValue, null);

			jobStore.SaveJobDetails(savedJobDetails);

			// Check job details.
			JobDetails loadedJobDetails = jobStore.GetJobDetails(dummyJobSpec.Name);
			JobAssert.AreEqual(savedJobDetails, loadedJobDetails);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void SaveJobDetails_ThrowsIfJobStatusIsNull()
		{
			jobStore.SaveJobDetails(null);
		}

		[Test]
		[ExpectedException(typeof (ObjectDisposedException))]
		public void SaveJobDetails_ThrowsIfDisposed()
		{
			jobStore.CreateJob(dummyJobSpec, DateTime.UtcNow, CreateJobConflictAction.Throw);
			JobDetails savedJobDetails = jobStore.GetJobDetails(dummyJobSpec.Name);

			jobStore.Dispose();
			jobStore.SaveJobDetails(savedJobDetails);
		}

		[Test]
		[ExpectedException(typeof (ConcurrentModificationException))]
		public void SaveJobDetails_ThrowsOnConcurrentDeletion()
		{
			JobDetails job = CreatePendingJob("test", DateTime.UtcNow);
			jobStore.DeleteJob("test");

			// Now try to save the job details back again.
			job.JobState = JobState.Stopped;
			jobStore.SaveJobDetails(job);
		}

		[Test]
		[ExpectedException(typeof (ConcurrentModificationException))]
		public void SaveJobDetails_ThrowsOnConcurrentSave()
		{
			JobDetails job = CreatePendingJob("test", DateTime.UtcNow);
			JobDetails modifiedJob = job.Clone();
			modifiedJob.JobState = JobState.Stopped;
			jobStore.SaveJobDetails(modifiedJob);

			// Now try to save the original job details back again.
			job.JobState = JobState.Orphaned;
			jobStore.SaveJobDetails(job);
		}

		[Test]
		public void DeleteJobMakesTheJobInaccessibleToSubsequentGetJobDetails()
		{
			Assert.IsTrue(jobStore.CreateJob(dummyJobSpec, DateTime.UtcNow, CreateJobConflictAction.Throw));
			Assert.IsTrue(jobStore.DeleteJob(dummyJobSpec.Name));
			Assert.IsNull(jobStore.GetJobDetails(dummyJobSpec.Name));
		}

		[Test]
		public void DeleteJobReturnsFalseIfJobDoesNotExist()
		{
			Assert.IsFalse(jobStore.DeleteJob("does not exist"));
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void DeleteJob_ThrowsIfJobNameIsNull()
		{
			jobStore.DeleteJob(null);
		}

		[Test]
		[ExpectedException(typeof (ObjectDisposedException))]
		public void DeleteJob_ThrowsIfDisposed()
		{
			jobStore.Dispose();
			jobStore.DeleteJob("job");
		}

		[Test]
		[ExpectedException(typeof (ObjectDisposedException))]
		public void CreateJobWatcher_ThrowsIfDisposed()
		{
			jobStore.Dispose();
			jobStore.CreateJobWatcher(SchedulerGuid);
		}

		[Test]
		[ExpectedException(typeof (ObjectDisposedException))]
		public void JobWatcher_UnblocksThreadAndThrowsIfJobStoreIsDisposedAsynchronously()
		{
			IJobWatcher watcher = jobStore.CreateJobWatcher(SchedulerGuid);

			ThreadPool.QueueUserWorkItem(delegate
			{
				Thread.Sleep(2000);
				jobStore.Dispose();
			});

			// This call blocks until the dispose runs.
			watcher.GetNextJobToProcess();
		}

		[Test]
		public void JobWatcher_UnblocksWhenScheduledJobBecomesTriggered()
		{
			IJobWatcher watcher = jobStore.CreateJobWatcher(SchedulerGuid);

			DateTime fireTime = DateTime.UtcNow.AddSeconds(3);
			CreateScheduledJob("scheduled", fireTime);

			// Wait for job to become ready.
			Assert.Less(DateTime.UtcNow, fireTime);
			JobDetails triggered = watcher.GetNextJobToProcess();
			Assert.GreaterOrEqual(DateTime.UtcNow, fireTime.Subtract(new TimeSpan(0, 0, 0, 0, 500)));
				// allow a little imprecision

			// Job should come back triggered.
			Assert.AreEqual("scheduled", triggered.JobSpec.Name);
			Assert.AreEqual(JobState.Triggered, triggered.JobState);

			watcher.Dispose();
		}

		[Test]
		public void JobWatcher_UnblocksWhenPendingJobAdded()
		{
			IJobWatcher watcher = jobStore.CreateJobWatcher(SchedulerGuid);

			ThreadPool.QueueUserWorkItem(delegate
			{
				Thread.Sleep(2000);
				CreatePendingJob("pending", DateTime.UtcNow);
			});

			// Wait for job to become ready.
			JobDetails triggered = watcher.GetNextJobToProcess();

			// Job should come back pending.
			Assert.AreEqual("pending", triggered.JobSpec.Name);
			Assert.AreEqual(JobState.Pending, triggered.JobState);

			watcher.Dispose();
		}

		[Test]
		public void JobWatcher_YieldsJobsInExpectedSequence()
		{
			IJobWatcher watcher = jobStore.CreateJobWatcher(SchedulerGuid);

			JobDetails orphaned = CreateOrphanedJob("orphaned", new DateTime(1970, 1, 3));
			JobDetails pending = CreatePendingJob("pending", new DateTime(1970, 1, 2));
			JobDetails triggered = CreateTriggeredJob("triggered", new DateTime(1970, 1, 6));
			JobDetails completed = CreateCompletedJob("completed", new DateTime(1970, 1, 1));
			JobDetails scheduled = CreateScheduledJob("scheduled", new DateTime(1970, 1, 4));

			// Ensure we tolerate a few odd cases where data may not be available like it should.
			JobDetails scheduled2 = CreateScheduledJob("scheduled2", new DateTime(1970, 1, 2));
			scheduled2.NextTriggerFireTimeUtc = null;
			jobStore.SaveJobDetails(scheduled2);

			JobDetails completed2 = CreateCompletedJob("completed2", new DateTime(1970, 1, 1));
			completed2.LastJobExecutionDetails = null;
			jobStore.SaveJobDetails(completed2);

			JobDetails orphaned2 = CreateOrphanedJob("orphaned2", new DateTime(1970, 1, 3));
			orphaned2.LastJobExecutionDetails.EndTimeUtc = null;
			jobStore.SaveJobDetails(orphaned2);

			// Populate a table of expected jobs.
			List<JobDetails> expectedJobs = new List<JobDetails>(new JobDetails[]
			                                                     	{
			                                                     		orphaned, pending, triggered, completed, scheduled, scheduled2
			                                                     		, completed2, orphaned2
			                                                     	});

			// Add in some extra jobs in other states that will not be returned.
			CreateRunningJob("running1");
			CreateStoppedJob("stopped1");
			CreateScheduledJob("scheduled-in-the-future", DateTime.MaxValue);

			// Ensure expected jobs are retrieved.
			while (expectedJobs.Count != 0)
			{
				JobDetails actualJob = watcher.GetNextJobToProcess();
				JobDetails expectedJob =
					expectedJobs.Find(delegate(JobDetails candidate) { return candidate.JobSpec.Name == actualJob.JobSpec.Name; });
				Assert.IsNotNull(expectedJob, "Did expect job {0}", actualJob.JobSpec.Name);

				// All expected scheduled jobs will have been triggered.
				if (expectedJob.JobState == JobState.Scheduled)
					expectedJob.JobState = JobState.Triggered;

				JobAssert.AreEqual(expectedJob, actualJob);

				if (expectedJobs.Count == 1)
				{
					// Ensure same job is returned a second time until its status is changed.
					// We wait for Count == 1 because that's the easiest case for which to verify
					// this behavior.
					JobDetails actualJob2 = watcher.GetNextJobToProcess();
					JobAssert.AreEqual(expectedJob, actualJob2);
				}

				// Change the status to progress.
				actualJob.JobState = JobState.Stopped;
				jobStore.SaveJobDetails(actualJob);

				expectedJobs.Remove(expectedJob);
			}

			// Ensure next request blocks but is released by the call to dispose.
			ThreadPool.QueueUserWorkItem(delegate
			{
				Thread.Sleep(2);
				watcher.Dispose();
			});

			// This call blocks until the dispose runs.
			Assert.IsNull(watcher.GetNextJobToProcess());
		}

		protected JobDetails CreatePendingJob(string jobName, DateTime creationTime)
		{
			jobStore.CreateJob(new JobSpec(jobName, "", "key", PeriodicTrigger.CreateOneShotTrigger(creationTime)),
			                   creationTime, CreateJobConflictAction.Throw);
			return jobStore.GetJobDetails(jobName);
		}

		protected JobDetails CreateScheduledJob(string jobName, DateTime triggerFireTime)
		{
			JobDetails job = CreatePendingJob(jobName, new DateTime(1970, 1, 1));
			job.JobState = JobState.Scheduled;
			job.NextTriggerFireTimeUtc = triggerFireTime;
			jobStore.SaveJobDetails(job);
			return job;
		}

		protected JobDetails CreateTriggeredJob(string jobName, DateTime triggerFireTime)
		{
			JobDetails job = CreatePendingJob(jobName, new DateTime(1970, 1, 1));
			job.JobState = JobState.Triggered;
			job.NextTriggerFireTimeUtc = triggerFireTime;
			jobStore.SaveJobDetails(job);
			return job;
		}

		protected JobDetails CreateOrphanedJob(string jobName, DateTime executionEndTime)
		{
			JobDetails job = CreatePendingJob(jobName, new DateTime(1970, 1, 1));
			job.JobState = JobState.Orphaned;
			job.LastJobExecutionDetails = new JobExecutionDetails(SchedulerGuid, new DateTime(1970, 1, 1));
			job.LastJobExecutionDetails.EndTimeUtc = executionEndTime;
			jobStore.SaveJobDetails(job);
			return job;
		}

		protected JobDetails CreateCompletedJob(string jobName, DateTime executionEndTime)
		{
			JobDetails job = CreatePendingJob(jobName, new DateTime(1970, 1, 1));
			job.JobState = JobState.Completed;
			job.LastJobExecutionDetails = new JobExecutionDetails(SchedulerGuid, new DateTime(1970, 1, 1));
			job.LastJobExecutionDetails.EndTimeUtc = executionEndTime;
			jobStore.SaveJobDetails(job);
			return job;
		}

		protected JobDetails CreateRunningJob(string jobName)
		{
			JobDetails job = CreatePendingJob(jobName, new DateTime(1970, 1, 1));
			job.JobState = JobState.Running;
			job.LastJobExecutionDetails = new JobExecutionDetails(SchedulerGuid, new DateTime(1970, 1, 1));
			jobStore.SaveJobDetails(job);
			return job;
		}

		protected JobDetails CreateStoppedJob(string jobName)
		{
			JobDetails job = CreatePendingJob(jobName, new DateTime(1970, 1, 1));
			job.JobState = JobState.Stopped;
			jobStore.SaveJobDetails(job);
			return job;
		}
	}
}