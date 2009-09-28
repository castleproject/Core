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
	using System.Diagnostics;
	using System.Threading;
	using Core.Logging;
	using NUnit.Framework;
	using Rhino.Mocks;
	using Rhino.Mocks.Constraints;
	using Scheduler.JobStores;
	using Utilities;

	[TestFixture]
	public class DefaultSchedulerTest : BaseUnitTest
	{
		private delegate void SaveJobDetailsDelegate(JobDetails jobDetails);

		private delegate bool DeleteJobDelegate(string jobName);

		private delegate JobDetails GetNextJobToProcessDelegate();

		private delegate void DisposeDelegate();

		private delegate bool ExecuteDelegate(JobExecutionContext context);

		private delegate IAsyncResult BeingExecuteDelegate(JobExecutionContext context,
		                                                   AsyncCallback asyncCallback, object asyncState);

		private delegate bool EndExecuteDelegate(IAsyncResult asyncResult);

		private DefaultScheduler scheduler;
		private DefaultScheduler uninitializedScheduler;
		private IJobStore mockJobStore;
		private IJobRunner mockJobRunner;
		private ILogger mockLogger;
		private Trigger mockTrigger;

		private JobSpec dummyJobSpec;
		private JobDetails dummyJobDetails;
		private JobData dummyJobData;

		private bool isWoken;

		public override void SetUp()
		{
			base.SetUp();

			mockJobStore = Mocks.CreateMock<IJobStore>();
			mockJobRunner = Mocks.CreateMock<IJobRunner>();
			mockLogger = Mocks.CreateMock<ILogger>();
			mockTrigger = Mocks.PartialMock<Trigger>();
			scheduler = new DefaultScheduler(mockJobStore, mockJobRunner);

			dummyJobData = new JobData();
			dummyJobSpec = new JobSpec("foo", "bar", "key", mockTrigger);
			dummyJobDetails = new JobDetails(dummyJobSpec, DateTime.UtcNow);

			isWoken = false;

			// Ensure the scheduler is initialized.
			mockJobStore.RegisterScheduler(scheduler.Guid, scheduler.Name);
			Mocks.Replay(mockJobStore);
			scheduler.Initialize();
			Mocks.Verify(mockJobStore);
			Mocks.BackToRecord(mockJobStore);

			mockJobStore.UnregisterScheduler(scheduler.Guid);

			// Create a separate uninitialized scheduler for certain tests.
			uninitializedScheduler = new DefaultScheduler(mockJobStore, mockJobRunner);
		}

		public override void TearDown()
		{
			Wake();

			if (!scheduler.IsDisposed)
				scheduler.Dispose();

			base.TearDown();
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Constructor_ThrowsWhenJobStoreIsNull()
		{
			Mocks.ReplayAll();

			new DefaultScheduler(null, mockJobRunner);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Constructor_ThrowsWhenJobRunnerIsNull()
		{
			Mocks.ReplayAll();

			new DefaultScheduler(mockJobStore, null);
		}

		[Test]
		public void HasNonEmptyGuid()
		{
			Mocks.ReplayAll();

			Assert.AreNotEqual(Guid.Empty, scheduler.Guid);
		}

		[Test]
		public void Name_GetterAndSetter()
		{
			Mocks.ReplayAll();

			Assert.IsNotNull(scheduler.Name); // has a default name

			scheduler.Name = "Test";
			Assert.AreEqual("Test", scheduler.Name);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Name_ThrowsIfValueIsNull()
		{
			Mocks.ReplayAll();

			scheduler.Name = null;
		}

		[Test]
		public void Logger_GetterAndSetter()
		{
			Mocks.ReplayAll();

			Assert.AreSame(NullLogger.Instance, scheduler.Logger);

			scheduler.Logger = mockLogger;
			Assert.AreSame(mockLogger, scheduler.Logger);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Logger_ThrowsIfValueIsNull()
		{
			Mocks.ReplayAll();

			scheduler.Logger = null;
		}

		[Test]
		public void ErrorRecoveryDelayInSeconds_GetterAndSetter()
		{
			Mocks.ReplayAll();

			Assert.AreEqual(DefaultScheduler.DefaultErrorRecoveryDelayInSeconds, scheduler.ErrorRecoveryDelayInSeconds);

			scheduler.ErrorRecoveryDelayInSeconds = 25;
			Assert.AreEqual(25, scheduler.ErrorRecoveryDelayInSeconds);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void ErrorRecoveryDelayInSeconds_ThrowsIfValueIsNegative()
		{
			Mocks.ReplayAll();

			scheduler.ErrorRecoveryDelayInSeconds = -1;
		}

		[Test]
		public void DisposeDoesNotThrowWhenCalledMultipleTimes()
		{
			Mocks.ReplayAll();

			Assert.IsFalse(scheduler.IsDisposed);

			scheduler.Dispose();
			Assert.IsTrue(scheduler.IsDisposed);

			scheduler.Dispose();
			Assert.IsTrue(scheduler.IsDisposed);
		}

		[Test]
		public void GetJobDetails_DelegatesToJobStore()
		{
			Expect.Call(mockJobStore.GetJobDetails("testJob")).Return(dummyJobDetails);

			Mocks.ReplayAll();

			Assert.AreSame(dummyJobDetails, scheduler.GetJobDetails("testJob"));
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void GetJobDetails_ThrowsIfNameIsNull()
		{
			Mocks.ReplayAll();

			scheduler.GetJobDetails(null);
		}

		[Test]
		[ExpectedException(typeof (ObjectDisposedException))]
		public void GetJobDetails_ThrowsIfDisposed()
		{
			Mocks.ReplayAll();

			scheduler.Dispose();
			scheduler.GetJobDetails("test");
		}

		[Test]
		[ExpectedException(typeof (SchedulerException))]
		public void GetJobDetails_ThrowsIfNotInitialized()
		{
			Mocks.ReplayAll();

			uninitializedScheduler.GetJobDetails("test");
		}

		[Test]
		public void DeleteJob_DelegatesToJobStore()
		{
			Expect.Call(mockJobStore.DeleteJob("testJob")).Return(true);

			Mocks.ReplayAll();

			Assert.IsTrue(scheduler.DeleteJob("testJob"));
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void DeleteJob_ThrowsIfNameIsNull()
		{
			Mocks.ReplayAll();

			scheduler.DeleteJob(null);
		}

		[Test]
		[ExpectedException(typeof (ObjectDisposedException))]
		public void DeleteJob_ThrowsIfDisposed()
		{
			Mocks.ReplayAll();

			scheduler.Dispose();
			scheduler.DeleteJob("test");
		}

		[Test]
		[ExpectedException(typeof (SchedulerException))]
		public void DeleteJob_ThrowsIfNotInitialized()
		{
			Mocks.ReplayAll();

			uninitializedScheduler.DeleteJob("test");
		}

		[Test]
		public void UpdateJob_DelegatesToJobStore()
		{
			mockJobStore.UpdateJob("testJob", dummyJobSpec);

			Mocks.ReplayAll();

			scheduler.UpdateJob("testJob", dummyJobSpec);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void UpdateJob_ThrowsIfNameIsNull()
		{
			Mocks.ReplayAll();

			scheduler.UpdateJob(null, dummyJobSpec);
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void UpdateJob_ThrowsIfNameIsEmpty()
		{
			Mocks.ReplayAll();

			scheduler.UpdateJob("", dummyJobSpec);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void UpdateJob_ThrowsIfJobSpecIsNull()
		{
			Mocks.ReplayAll();

			scheduler.UpdateJob("job", null);
		}

		[Test]
		[ExpectedException(typeof (ObjectDisposedException))]
		public void UpdateJob_ThrowsIfDisposed()
		{
			Mocks.ReplayAll();

			scheduler.Dispose();
			scheduler.UpdateJob("test", dummyJobSpec);
		}

		[Test]
		[ExpectedException(typeof (SchedulerException))]
		public void UpdateJob_ThrowsIfNotInitialized()
		{
			Mocks.ReplayAll();

			uninitializedScheduler.UpdateJob("test", dummyJobSpec);
		}

		[Test]
		public void ListJobNames_DelegatesToJobStore()
		{
			Expect.Call(mockJobStore.ListJobNames()).Return(new string[] {"a", "b"});

			Mocks.ReplayAll();

			CollectionAssert.AreEqual(new [] { "a", "b" }, scheduler.ListJobNames());
		}

		[Test]
		[ExpectedException(typeof (ObjectDisposedException))]
		public void ListJobNames_ThrowsIfDisposed()
		{
			Mocks.ReplayAll();

			scheduler.Dispose();
			scheduler.ListJobNames();
		}

		[Test]
		[ExpectedException(typeof (SchedulerException))]
		public void ListJobNames_ThrowsIfNotInitialized()
		{
			Mocks.ReplayAll();

			uninitializedScheduler.ListJobNames();
		}

		[Test]
		public void CreateJob_DelegatesToJobStore()
		{
			mockJobStore.CreateJob(dummyJobSpec, DateTime.UtcNow, CreateJobConflictAction.Update);
			LastCall.Constraints(Rhino.Mocks.Constraints.Is.Same(dummyJobSpec), Rhino.Mocks.Constraints.Is.Anything(), Rhino.Mocks.Constraints.Is.Equal(CreateJobConflictAction.Update)).Return(true);

			Mocks.ReplayAll();

			Assert.IsTrue(scheduler.CreateJob(dummyJobSpec, CreateJobConflictAction.Update));
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void CreateJob_ThrowsIfSpecIsNull()
		{
			Mocks.ReplayAll();

			scheduler.CreateJob(null, CreateJobConflictAction.Update);
		}

		[Test]
		[ExpectedException(typeof (ObjectDisposedException))]
		public void CreateJob_ThrowsIfDisposed()
		{
			Mocks.ReplayAll();

			scheduler.Dispose();
			scheduler.CreateJob(dummyJobSpec, CreateJobConflictAction.Update);
		}

		[Test]
		[ExpectedException(typeof (SchedulerException))]
		public void CreateJob_ThrowsIfNotInitialized()
		{
			Mocks.ReplayAll();

			uninitializedScheduler.CreateJob(dummyJobSpec, CreateJobConflictAction.Update);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void CreateJob_ThrowsIfConflictActionIsInvalid()
		{
			Mocks.ReplayAll();

			scheduler.CreateJob(dummyJobSpec, (CreateJobConflictAction) 9999);
		}

		[Test]
		public void StartTwiceDoesNotCrash()
		{
			PrepareMockJobWatcher(null);
			Mocks.ReplayAll();

			scheduler.Start();
			Assert.IsTrue(scheduler.IsRunning);

			scheduler.Start();
			Assert.IsTrue(scheduler.IsRunning);
		}

		[Test]
		public void StopWithoutStartDoesNotCrash()
		{
			Mocks.ReplayAll();

			scheduler.Stop();
			Assert.IsFalse(scheduler.IsRunning);
		}

		[Test]
		public void StoppingCompletesSynchronously()
		{
			PrepareMockJobWatcher(null);
			Mocks.ReplayAll();

			scheduler.Start();
			Assert.IsTrue(scheduler.IsRunning);

			scheduler.Stop();
			Assert.IsFalse(scheduler.IsRunning); // synchronously stopped
		}

		[Test]
		[ExpectedException(typeof (ObjectDisposedException))]
		public void Start_ThrowsIfDisposed()
		{
			Mocks.ReplayAll();

			scheduler.Dispose();
			scheduler.Start();
		}

		[Test]
		[ExpectedException(typeof (ObjectDisposedException))]
		public void Stop_ThrowsIfDisposed()
		{
			Mocks.ReplayAll();

			scheduler.Dispose();
			scheduler.Stop();
		}

		[Test]
		[ExpectedException(typeof (SchedulerException))]
		public void Start_ThrowsIfNotInitialized()
		{
			Mocks.ReplayAll();

			uninitializedScheduler.Start();
		}

		[Test]
		[ExpectedException(typeof (SchedulerException))]
		public void Stop_ThrowsIfNotInitialized()
		{
			Mocks.ReplayAll();

			uninitializedScheduler.Stop();
		}

		[Test]
		public void SchedulePendingJob_WithSkipAction()
		{
			JobDetails jobDetails = new JobDetails(dummyJobSpec, DateTime.UtcNow);
			jobDetails.JobState = JobState.Pending;

			PrepareMockJobWatcher(jobDetails);

			Expect.Call(mockTrigger.Schedule(TriggerScheduleCondition.Latch, DateTime.UtcNow, null))
				.Constraints(Rhino.Mocks.Constraints.Is.Equal(TriggerScheduleCondition.Latch), Rhino.Mocks.Constraints.Is.Anything(), Rhino.Mocks.Constraints.Is.Null())
				.Return(TriggerScheduleAction.Skip);
			Expect.Call(mockTrigger.NextFireTimeUtc).Return(new DateTime(1970, 1, 5, 0, 0, 0, DateTimeKind.Utc));
			Expect.Call(mockTrigger.NextMisfireThreshold).Return(new TimeSpan(0, 1, 0));

			mockJobStore.SaveJobDetails(jobDetails);
			LastCall.Do((SaveJobDetailsDelegate) WakeOnSaveJobDetails);

			Mocks.ReplayAll();

			RunSchedulerUntilWake();

			Assert.AreEqual(JobState.Scheduled, jobDetails.JobState);
			DateTimeAssert.AreEqualIncludingKind(new DateTime(1970, 1, 5, 0, 0, 0, DateTimeKind.Utc),
			                                     jobDetails.NextTriggerFireTimeUtc);
			Assert.AreEqual(new TimeSpan(0, 1, 0), jobDetails.NextTriggerMisfireThreshold);
			Assert.IsNull(jobDetails.JobSpec.JobData);
			Assert.IsNull(jobDetails.LastJobExecutionDetails);
		}

		[TestCase(false, false, false)]
		[TestCase(true, false, false)]
		[TestCase(true, true, true)]
		public void ScheduleOrphanJob_WithStopAction(bool lastExecutionDetailsNotNull,
		                                             bool lastExecutionSucceeded, bool lastEndTimeNotNull)
		{
			JobDetails jobDetails = new JobDetails(dummyJobSpec, DateTime.UtcNow);
			jobDetails.JobState = JobState.Orphaned;

			if (lastExecutionDetailsNotNull)
			{
				jobDetails.LastJobExecutionDetails = new JobExecutionDetails(scheduler.Guid, DateTime.UtcNow);
				jobDetails.LastJobExecutionDetails.Succeeded = lastExecutionSucceeded;

				if (lastEndTimeNotNull)
					jobDetails.LastJobExecutionDetails.EndTimeUtc = DateTime.UtcNow;
			}

			PrepareMockJobWatcher(jobDetails);

			Expect.Call(mockTrigger.Schedule(TriggerScheduleCondition.Latch, DateTime.UtcNow, null))
				.Constraints(Rhino.Mocks.Constraints.Is.Equal(TriggerScheduleCondition.Latch), Rhino.Mocks.Constraints.Is.Anything(), Property.Value("Succeeded", false))
				.Return(TriggerScheduleAction.DeleteJob);
			Expect.Call(mockTrigger.NextFireTimeUtc).Return(null);
			Expect.Call(mockTrigger.NextMisfireThreshold).Return(null);

			mockJobStore.DeleteJob(jobDetails.JobSpec.Name);
			LastCall.Do((DeleteJobDelegate) WakeOnDeleteJob);

			Mocks.ReplayAll();

			RunSchedulerUntilWake();

			Assert.AreEqual(JobState.Orphaned, jobDetails.JobState);
			Assert.IsNull(jobDetails.NextTriggerFireTimeUtc);
			Assert.IsNull(jobDetails.NextTriggerMisfireThreshold);
			Assert.IsNull(jobDetails.JobSpec.JobData);
			Assert.IsNotNull(jobDetails.LastJobExecutionDetails);
			Assert.AreEqual(scheduler.Guid, jobDetails.LastJobExecutionDetails.SchedulerGuid);
			Assert.AreEqual(false, jobDetails.LastJobExecutionDetails.Succeeded);
			Assert.IsNotNull(jobDetails.LastJobExecutionDetails.EndTimeUtc);
		}

		[TestCase(false, false, false)]
		[TestCase(true, false, false)]
		[TestCase(true, true, true)]
		public void ScheduleCompletedJob_WithStopAction(bool lastExecutionDetailsNotNull,
		                                                bool lastExecutionSucceeded, bool lastEndTimeNotNull)
		{
			JobDetails jobDetails = new JobDetails(dummyJobSpec, DateTime.UtcNow);
			jobDetails.JobState = JobState.Completed;

			if (lastExecutionDetailsNotNull)
			{
				jobDetails.LastJobExecutionDetails = new JobExecutionDetails(scheduler.Guid, DateTime.UtcNow);
				jobDetails.LastJobExecutionDetails.Succeeded = lastExecutionSucceeded;

				if (lastEndTimeNotNull)
					jobDetails.LastJobExecutionDetails.EndTimeUtc = DateTime.UtcNow;
			}

			PrepareMockJobWatcher(jobDetails);

			Expect.Call(mockTrigger.Schedule(TriggerScheduleCondition.Latch, DateTime.UtcNow, null))
				.Constraints(Rhino.Mocks.Constraints.Is.Equal(TriggerScheduleCondition.Latch), Rhino.Mocks.Constraints.Is.Anything(),
				             Property.Value("Succeeded", lastExecutionSucceeded))
				.Return(TriggerScheduleAction.Stop);
			Expect.Call(mockTrigger.NextFireTimeUtc).Return(null);
			Expect.Call(mockTrigger.NextMisfireThreshold).Return(null);

			mockJobStore.SaveJobDetails(jobDetails);
			LastCall.Do((SaveJobDetailsDelegate) WakeOnSaveJobDetails);

			Mocks.ReplayAll();

			RunSchedulerUntilWake();

			Assert.AreEqual(JobState.Stopped, jobDetails.JobState);
			Assert.IsNull(jobDetails.NextTriggerFireTimeUtc);
			Assert.IsNull(jobDetails.NextTriggerMisfireThreshold);
			Assert.IsNull(jobDetails.JobSpec.JobData);
			Assert.IsNotNull(jobDetails.LastJobExecutionDetails);
			Assert.AreEqual(scheduler.Guid, jobDetails.LastJobExecutionDetails.SchedulerGuid);
			Assert.AreEqual(lastExecutionSucceeded, jobDetails.LastJobExecutionDetails.Succeeded);
			Assert.IsNotNull(jobDetails.LastJobExecutionDetails.EndTimeUtc);
		}

		[TestCase(false, true, true, Description = "Fire with trigger fire time & misfire threshold.")]
		[TestCase(false, true, false, Description = "Fire with trigger fire time but not misfire threshold.")]
		[TestCase(true, true, true, Description = "Misfire with trigger fire time & misfire threshold.")]
		[TestCase(true, false, true, Description = "Misfire because of missing trigger fire time but have misfire threshold.")]
		[TestCase(true, false, false,
			Description = "Misfire because of missing trigger fire time but also missing misfire threshold.")]
		[TestCase(true, true, true, Description = "Misfire assumed because last execution details missing.")]
		public void SchedulerTriggeredJob_WithSkipAction(bool misfire,
		                                                 bool nextTriggerFireTimeNotNull,
		                                                 bool nextTriggerMisfireThresholdNotNull)
		{
			// Create a job scheduled to fire 3 minutes in the past.
			// We cause a misfire by setting a threshold for 2 seconds which clearly is
			// in the past.  Otherwise we set the threshold to 1 minute which clearly is satisfiable. 
			DateTime schedTime = DateTime.UtcNow.AddSeconds(-3);
			JobDetails jobDetails = new JobDetails(dummyJobSpec, schedTime.AddSeconds(-5));
			jobDetails.JobState = JobState.Triggered;

			if (nextTriggerFireTimeNotNull)
				jobDetails.NextTriggerFireTimeUtc = schedTime;
			if (nextTriggerMisfireThresholdNotNull)
				jobDetails.NextTriggerMisfireThreshold = misfire ? new TimeSpan(0, 0, 2) : new TimeSpan(0, 1, 0);

			PrepareMockJobWatcher(jobDetails);

			TriggerScheduleCondition expectedCondition = misfire
			                                             	? TriggerScheduleCondition.Misfire
			                                             	: TriggerScheduleCondition.Fire;

			Expect.Call(mockTrigger.Schedule(expectedCondition, DateTime.MinValue, null))
				.Constraints(Rhino.Mocks.Constraints.Is.Equal(expectedCondition), Rhino.Mocks.Constraints.Is.Anything(), Rhino.Mocks.Constraints.Is.Null())
				.Return(TriggerScheduleAction.Skip);
			Expect.Call(mockTrigger.NextFireTimeUtc).Return(new DateTime(1970, 1, 5, 0, 0, 0, DateTimeKind.Utc));
			Expect.Call(mockTrigger.NextMisfireThreshold).Return(new TimeSpan(0, 2, 0));

			mockJobStore.SaveJobDetails(jobDetails);
			LastCall.Do((SaveJobDetailsDelegate) WakeOnSaveJobDetails);

			Mocks.ReplayAll();

			RunSchedulerUntilWake();

			Assert.AreEqual(JobState.Scheduled, jobDetails.JobState);
			DateTimeAssert.AreEqualIncludingKind(new DateTime(1970, 1, 5, 0, 0, 0, DateTimeKind.Utc),
			                                     jobDetails.NextTriggerFireTimeUtc);
			Assert.AreEqual(new TimeSpan(0, 2, 0), jobDetails.NextTriggerMisfireThreshold);
			Assert.IsNull(jobDetails.JobSpec.JobData);
			Assert.IsNull(jobDetails.LastJobExecutionDetails);
		}

		[TestCase(true, false, false, Description = "Successful job execution.")]
		[TestCase(false, false, false, Description = "Failed job execution with no exception.")]
		[TestCase(false, true, false, Description = "Failed job executed without an exception.")]
		public void SchedulerExecutesJobsAndHandlesSuccessFailureOrException(
			bool jobSucceeds, bool jobThrows, bool savingCompletedJobThrows)
		{
			JobData newJobData = new JobData();

			ExecuteDelegate execute = delegate(JobExecutionContext context)
			{
				Assert.IsNotNull(context);
				Assert.AreSame(scheduler, context.Scheduler);
				Assert.IsNotNull(context.Logger);
				Assert.IsNotNull(context.JobSpec);

				context.JobData = newJobData;

				if (jobThrows)
					throw new Exception("Oh no!");

				return jobSucceeds;
			};

			PrepareJobForExecution(execute.BeginInvoke, execute.EndInvoke);

			/* Note: We used to drop back into ScheduleJob again immediately after a job completed.
             *       That's a cheap optimization but it makes it more difficult to ensure that
             *       the scheduler will shut down cleanly since it could just keep re-executing the job.
            TriggerScheduleCondition expectedCondition = jobSucceeds ? TriggerScheduleCondition.JobSucceeded : TriggerScheduleCondition.JobFailed;
            Expect.Call(mockTrigger.Schedule(expectedCondition, DateTime.UtcNow))
                .Constraints(Is.Equal(expectedCondition), Is.Anything())
                .Return(TriggerScheduleAction.Stop);
            Expect.Call(mockTrigger.NextFireTime).Return(null);
            Expect.Call(mockTrigger.NextMisfireThreshold).Return(null);
             */

			mockJobStore.SaveJobDetails(null);
			LastCall.IgnoreArguments().Do((SaveJobDetailsDelegate) delegate(JobDetails completedJobDetails)
			{
				Assert.IsNotNull(completedJobDetails);
				Assert.AreEqual(dummyJobSpec.Name, completedJobDetails.JobSpec.Name);

				Assert.IsNotNull(completedJobDetails.LastJobExecutionDetails);
				Assert.AreEqual(scheduler.Guid, completedJobDetails.LastJobExecutionDetails.SchedulerGuid);
				Assert.GreaterOrEqual(completedJobDetails.LastJobExecutionDetails.StartTimeUtc,
				                        completedJobDetails.CreationTimeUtc);
				Assert.IsNotNull(completedJobDetails.LastJobExecutionDetails.EndTimeUtc);
				Assert.GreaterOrEqual(completedJobDetails.LastJobExecutionDetails.EndTimeUtc,
				                        completedJobDetails.LastJobExecutionDetails.StartTimeUtc);
				Assert.AreEqual(jobSucceeds, completedJobDetails.LastJobExecutionDetails.Succeeded);

				if (!jobThrows)
					JobAssert.AreEqual(newJobData, completedJobDetails.JobSpec.JobData);
				else
					Assert.IsNull(completedJobDetails.JobSpec.JobData);

				Wake();
			});

			Mocks.ReplayAll();

			RunSchedulerUntilWake();
		}

		[Test]
		public void SchedulerExecutesJobsAndHandlesBeginJobFailure()
		{
			PrepareJobForExecution(delegate { throw new Exception("Eeep!"); }, delegate { return true; });

			/* Note: We used to drop back into ScheduleJob again immediately after a job completed.
             *       That's a cheap optimization but it makes it more difficult to ensure that
             *       the scheduler will shut down cleanly since it could just keep re-executing the job.
            Expect.Call(mockTrigger.Schedule(TriggerScheduleCondition.JobFailed, DateTime.UtcNow))
                .Constraints(Is.Equal(TriggerScheduleCondition.JobFailed), Is.Anything())
                .Return(TriggerScheduleAction.Stop);
            Expect.Call(mockTrigger.NextFireTime).Return(null);
            Expect.Call(mockTrigger.NextMisfireThreshold).Return(null);
             */

			mockJobStore.SaveJobDetails(null);
			LastCall.IgnoreArguments().Do((SaveJobDetailsDelegate) delegate(JobDetails completedJobDetails)
			{
				Assert.IsNotNull(completedJobDetails);
				Assert.AreEqual(dummyJobSpec.Name, completedJobDetails.JobSpec.Name);

				Assert.IsNotNull(completedJobDetails.LastJobExecutionDetails);
				Assert.AreEqual(scheduler.Guid, completedJobDetails.LastJobExecutionDetails.SchedulerGuid);
				Assert.GreaterOrEqual(completedJobDetails.LastJobExecutionDetails.StartTimeUtc,
				                        completedJobDetails.CreationTimeUtc);
				Assert.IsNotNull(completedJobDetails.LastJobExecutionDetails.EndTimeUtc);
				Assert.GreaterOrEqual(completedJobDetails.LastJobExecutionDetails.EndTimeUtc,
				                        completedJobDetails.LastJobExecutionDetails.StartTimeUtc);
				Assert.AreEqual(false, completedJobDetails.LastJobExecutionDetails.Succeeded);
				Assert.IsNull(completedJobDetails.JobSpec.JobData);

				Wake();
			});

			Mocks.ReplayAll();

			RunSchedulerUntilWake();
		}

		/// <summary>
		/// This case is a bit hard to verify automatically because everything is asynchronous.
		/// TODO: Look for the message that was logged by the exception.
		/// </summary>
		[Test]
		public void SchedulerExecutesJobsAndToleratesExceptionDuringFinalSave()
		{
			ExecuteDelegate execute = delegate { return true; };

			PrepareJobForExecution(execute.BeginInvoke, execute.EndInvoke);

			/* Note: We used to drop back into ScheduleJob again immediately after a job completed.
             *       That's a cheap optimization but it makes it more difficult to ensure that
             *       the scheduler will shut down cleanly since it could just keep re-executing the job.
            Expect.Call(mockTrigger.Schedule(TriggerScheduleCondition.JobSucceeded, DateTime.UtcNow))
                .Constraints(Is.Equal(TriggerScheduleCondition.JobSucceeded), Is.Anything())
                .Return(TriggerScheduleAction.Stop);
            Expect.Call(mockTrigger.NextFireTime).Return(null);
            Expect.Call(mockTrigger.NextMisfireThreshold).Return(null);
             */

			mockJobStore.SaveJobDetails(null);
			LastCall.IgnoreArguments().Do((SaveJobDetailsDelegate) delegate
			{
				Wake();
				throw new Exception("Oops!");
			});

			Mocks.ReplayAll();

			RunSchedulerUntilWake();
		}

		[TestCase(JobState.Scheduled)]
		[TestCase(JobState.Running)]
		[TestCase(JobState.Stopped)]
		[TestCase((JobState) 9999)] // invalid job state
		public void SchedulerHandlesUnexpectedJobStateReceivedFromWatcherByStoppingTheTrigger(JobState jobState)
		{
			JobDetails jobDetails = new JobDetails(dummyJobSpec, DateTime.UtcNow);
			jobDetails.JobState = jobState;

			PrepareMockJobWatcher(jobDetails);

			mockJobStore.SaveJobDetails(jobDetails);
			LastCall.Do((SaveJobDetailsDelegate) WakeOnSaveJobDetails);

			Mocks.ReplayAll();

			RunSchedulerUntilWake();

			Assert.AreEqual(JobState.Stopped, jobDetails.JobState);
			Assert.IsNull(jobDetails.NextTriggerFireTimeUtc);
			Assert.IsNull(jobDetails.NextTriggerMisfireThreshold);
			Assert.IsNull(jobDetails.LastJobExecutionDetails);
			Assert.IsNull(jobDetails.JobSpec.JobData);
		}

		[Test, Ignore("Disabled because it looks like Stopwatch is not accurate in a VM.")]
		public void SchedulerHandlesJobWatcherExceptionByInsertingAnErrorRecoveryDelay()
		{
			// The mock job watcher will throw on the first GetNextJobToProcess.
			// On the second one it will cause us to wake up.
			// There should be a total delay at least as big as the error recovery delay.
			IJobWatcher mockJobWatcher = Mocks.CreateMock<IJobWatcher>();

			Expect.Call(mockJobWatcher.GetNextJobToProcess()).Throw(new Exception("Uh oh!"));
			Expect.Call(mockJobWatcher.GetNextJobToProcess()).Do((GetNextJobToProcessDelegate) delegate
			{
				Wake();
				return null;
			});

			mockJobWatcher.Dispose();
			LastCall.Repeat.AtLeastOnce();

			Expect.Call(mockJobStore.CreateJobWatcher(scheduler.Guid)).Return(mockJobWatcher);

			Mocks.ReplayAll();

			scheduler.ErrorRecoveryDelayInSeconds = 2;

			Stopwatch stopWatch = Stopwatch.StartNew();
			RunSchedulerUntilWake();
			Assert.That(stopWatch.ElapsedMilliseconds, NUnit.Framework.Is.GreaterThanOrEqualTo(2000).And.LessThanOrEqualTo(10000));
		}

		[TestCase(false)]
		[TestCase(true)]
		public void SchedulerHandlesJobWatcherObjectDisposedExceptionByStopping(bool throwSecondExceptionInDispose)
		{
			// The mock job watcher will throw ObjectDisposedException on the first
			// call to GetNextJobToProcess.  This should cause the scheduler's job watching
			// thread to begin shutting down and eventually call the job watcher's Dispose.
			// That will wake us up from sleep so we can verify that the scheduler has stopped
			// on its own.
			// We also check what happens if a second exception occurs in Dispose during shutdown.
			IJobWatcher mockJobWatcher = Mocks.CreateMock<IJobWatcher>();

			Expect.Call(mockJobWatcher.GetNextJobToProcess()).Throw(new ObjectDisposedException("Uh oh!"));

			mockJobWatcher.Dispose();
			LastCall.Do((DisposeDelegate) delegate
			{
				Wake();

				if (throwSecondExceptionInDispose)
					throw new Exception("Yikes!  We're trying to shut down here!");
			});

			Expect.Call(mockJobStore.CreateJobWatcher(scheduler.Guid)).Return(mockJobWatcher);

			Mocks.ReplayAll();

			scheduler.Start();
			WaitUntilWake();

			Assert.IsFalse(scheduler.IsRunning, "The scheduler should have stopped itself automatically.");
		}

		[Test, Ignore("Disabled because it looks like Stopwatch is not accurate in a VM.")]
		public void SchedulerHandlesJobStoreExceptionByInsertingAnErrorRecoveryDelay()
		{
			// The mock job watcher will return job detail on the first GetNextJobToProcess
			// but the mock job store will throw Expection during SaveJobDetails.
			// On the second call to GetNextJobToProcess the mock job watcher will cause us to wake up.
			// There should be a total delay at least as big as the error recovery delay.
			JobDetails jobDetails = new JobDetails(dummyJobSpec, DateTime.UtcNow);
			jobDetails.JobState = JobState.Pending;

			Expect.Call(mockTrigger.Schedule(TriggerScheduleCondition.Latch, DateTime.UtcNow, null))
				.Constraints(Rhino.Mocks.Constraints.Is.Equal(TriggerScheduleCondition.Latch), Rhino.Mocks.Constraints.Is.Anything(), Rhino.Mocks.Constraints.Is.Null())
				.Return(TriggerScheduleAction.Stop);
			Expect.Call(mockTrigger.NextFireTimeUtc).Return(new DateTime(1970, 1, 5, 0, 0, 0, DateTimeKind.Utc));
			Expect.Call(mockTrigger.NextMisfireThreshold).Return(new TimeSpan(0, 1, 0));

			mockJobStore.SaveJobDetails(jobDetails);
			LastCall.Throw(new Exception("Uh oh!"));

			IJobWatcher mockJobWatcher = Mocks.CreateMock<IJobWatcher>();

			Expect.Call(mockJobWatcher.GetNextJobToProcess()).Return(jobDetails);
			Expect.Call(mockJobWatcher.GetNextJobToProcess()).Do((GetNextJobToProcessDelegate) delegate
			{
				Wake();
				return null;
			});

			mockJobWatcher.Dispose();
			LastCall.Repeat.AtLeastOnce();

			Expect.Call(mockJobStore.CreateJobWatcher(scheduler.Guid)).Return(mockJobWatcher);

			Mocks.ReplayAll();

			scheduler.ErrorRecoveryDelayInSeconds = 2;

			Stopwatch stopWatch = Stopwatch.StartNew();
			RunSchedulerUntilWake();
			Assert.That(stopWatch.ElapsedMilliseconds, NUnit.Framework.Is.GreaterThanOrEqualTo(2000).And.LessThanOrEqualTo(10000));
		}

		[Test]
		public void SchedulerHandlesJobStoreConcurrentModificationExceptionByIgnoringTheJob()
		{
			// The mock job watcher will return job detail on the first GetNextJobToProcess
			// but the mock job store will throw ConcurrentModificationException during SaveJobDetails.
			// On the second call to GetNextJobToProcess the mock job watcher will cause us to wake up.
			// There should be no noticeable delay, particularly not one as big as the error recovery delay.
			JobDetails jobDetails = new JobDetails(dummyJobSpec, DateTime.UtcNow);
			jobDetails.JobState = JobState.Pending;

			Expect.Call(mockTrigger.Schedule(TriggerScheduleCondition.Latch, DateTime.UtcNow, null))
				.Constraints(Rhino.Mocks.Constraints.Is.Equal(TriggerScheduleCondition.Latch), Rhino.Mocks.Constraints.Is.Anything(), Rhino.Mocks.Constraints.Is.Null())
				.Return(TriggerScheduleAction.Stop);
			Expect.Call(mockTrigger.NextFireTimeUtc).Return(new DateTime(1970, 1, 5, 0, 0, 0, DateTimeKind.Utc));
			Expect.Call(mockTrigger.NextMisfireThreshold).Return(new TimeSpan(0, 1, 0));

			mockJobStore.SaveJobDetails(jobDetails);
			LastCall.Throw(new ConcurrentModificationException("Another scheduler grabbed it."));

			IJobWatcher mockJobWatcher = Mocks.CreateMock<IJobWatcher>();

			Expect.Call(mockJobWatcher.GetNextJobToProcess()).Return(jobDetails);
			Expect.Call(mockJobWatcher.GetNextJobToProcess()).Do((GetNextJobToProcessDelegate) delegate
			{
				Wake();
				return null;
			});

			mockJobWatcher.Dispose();
			LastCall.Repeat.AtLeastOnce();

			Expect.Call(mockJobStore.CreateJobWatcher(scheduler.Guid)).Return(mockJobWatcher);

			Mocks.ReplayAll();

			scheduler.ErrorRecoveryDelayInSeconds = 2;

			Stopwatch stopWatch = Stopwatch.StartNew();
			RunSchedulerUntilWake();
			Assert.Less(stopWatch.ElapsedMilliseconds, 2000);
		}

		[TestCase((TriggerScheduleAction) 9999)] // invalid trigger action
		public void ScheduleHandlesUnexpectedActionReceivedFromTriggerByStoppingTheTrigger(TriggerScheduleAction action)
		{
			JobDetails jobDetails = new JobDetails(dummyJobSpec, DateTime.UtcNow);
			jobDetails.JobState = JobState.Pending;

			PrepareMockJobWatcher(jobDetails);

			Expect.Call(mockTrigger.Schedule(TriggerScheduleCondition.Latch, DateTime.UtcNow, null))
				.Constraints(Rhino.Mocks.Constraints.Is.Equal(TriggerScheduleCondition.Latch), Rhino.Mocks.Constraints.Is.Anything(), Rhino.Mocks.Constraints.Is.Null())
				.Return(action);
			Expect.Call(mockTrigger.NextFireTimeUtc).Return(new DateTime(1970, 1, 5, 0, 0, 0, DateTimeKind.Utc));
			Expect.Call(mockTrigger.NextMisfireThreshold).Return(new TimeSpan(0, 1, 0));

			mockJobStore.SaveJobDetails(jobDetails);
			LastCall.Do((SaveJobDetailsDelegate) WakeOnSaveJobDetails);

			Mocks.ReplayAll();

			RunSchedulerUntilWake();

			Assert.AreEqual(JobState.Stopped, jobDetails.JobState);
			Assert.IsNull(jobDetails.NextTriggerFireTimeUtc);
			Assert.IsNull(jobDetails.NextTriggerMisfireThreshold);
			Assert.IsNull(jobDetails.LastJobExecutionDetails);
			Assert.IsNull(jobDetails.JobSpec.JobData);
		}

		[Test]
		public void ScheduleHandlesTriggerExceptionByStoppingTheTrigger()
		{
			JobDetails jobDetails = new JobDetails(dummyJobSpec, DateTime.UtcNow);
			jobDetails.JobState = JobState.Pending;

			PrepareMockJobWatcher(jobDetails);

			Expect.Call(mockTrigger.Schedule(TriggerScheduleCondition.Latch, DateTime.UtcNow, null))
				.Constraints(Rhino.Mocks.Constraints.Is.Equal(TriggerScheduleCondition.Latch), Rhino.Mocks.Constraints.Is.Anything(), Rhino.Mocks.Constraints.Is.Null())
				.Throw(new Exception("Oh no!")); // throw an exception from the trigger

			mockJobStore.SaveJobDetails(jobDetails);
			LastCall.Do((SaveJobDetailsDelegate) WakeOnSaveJobDetails);

			Mocks.ReplayAll();

			RunSchedulerUntilWake();

			Assert.AreEqual(JobState.Stopped, jobDetails.JobState);
			Assert.IsNull(jobDetails.NextTriggerFireTimeUtc);
			Assert.IsNull(jobDetails.NextTriggerMisfireThreshold);
			Assert.IsNull(jobDetails.LastJobExecutionDetails);
			Assert.IsNull(jobDetails.JobSpec.JobData);
		}

		/// <summary>
		/// Schedules a job that is guaranteed to be executed.
		/// </summary>
		private void PrepareJobForExecution(BeingExecuteDelegate beginExecute,
		                                    EndExecuteDelegate endExecute)
		{
			JobDetails jobDetails = new JobDetails(dummyJobSpec, DateTime.UtcNow);
			jobDetails.JobState = JobState.Pending;

			PrepareMockJobWatcher(jobDetails);

			Expect.Call(mockTrigger.Schedule(TriggerScheduleCondition.Latch, DateTime.MinValue, null))
				.Constraints(Rhino.Mocks.Constraints.Is.Equal(TriggerScheduleCondition.Latch), Rhino.Mocks.Constraints.Is.Anything(), Rhino.Mocks.Constraints.Is.Null())
				.Return(TriggerScheduleAction.ExecuteJob);
			Expect.Call(mockTrigger.NextFireTimeUtc).Return(null);
			Expect.Call(mockTrigger.NextMisfireThreshold).Return(null);

			mockJobStore.SaveJobDetails(jobDetails);
			LastCall.Do((SaveJobDetailsDelegate) delegate
			{
				Assert.AreEqual(JobState.Running, jobDetails.JobState);
				Assert.IsNotNull(jobDetails.LastJobExecutionDetails);
				Assert.AreEqual(scheduler.Guid, jobDetails.LastJobExecutionDetails.SchedulerGuid);
				Assert.GreaterOrEqual(jobDetails.LastJobExecutionDetails.StartTimeUtc, jobDetails.CreationTimeUtc);
				Assert.IsNull(jobDetails.LastJobExecutionDetails.EndTimeUtc);
				Assert.IsFalse(jobDetails.LastJobExecutionDetails.Succeeded);
			});

			Expect.Call(mockJobRunner.BeginExecute(null, null, null))
				.IgnoreArguments().Repeat.Any().Do(beginExecute);
			Expect.Call(mockJobRunner.EndExecute(null))
				.IgnoreArguments().Repeat.Any().Do(endExecute);
		}

		/// <summary>
		/// Sets the mock job store to provide a watcher that yields the specified job details
		/// on its first access then waits to be disposed.
		/// </summary>
		/// <param name="jobDetails">The job details to yield</param>
		private void PrepareMockJobWatcher(JobDetails jobDetails)
		{
			Expect.Call(mockJobStore.CreateJobWatcher(scheduler.Guid)).
				Return(new MockJobWatcher(jobDetails));
		}

		/// <summary>
		/// Runs the scheduler until <see cref="Wake" /> is called then Stops it.
		/// </summary>
		private void RunSchedulerUntilWake()
		{
			lock (scheduler)
			{
				scheduler.Logger = new ConsoleLogger();
				scheduler.Start();
			}

			WaitUntilWake();
			scheduler.Stop();
		}

		/// <summary>
		/// Waits for <see cref="Wake" /> to be called.
		/// </summary>
		private void WaitUntilWake()
		{
			lock (scheduler)
			{
				if (! isWoken)
					Assert.IsTrue(Monitor.Wait(scheduler, 10000), "Wake signal not received before timeout elapsed.");
			}
		}

		/// <summary>
		/// Wakes the threads blocked in <see cref="WaitUntilWake" />.
		/// </summary>
		private void Wake()
		{
			lock (scheduler)
			{
				isWoken = true;
				Monitor.PulseAll(scheduler);
			}
		}

		private void WakeOnSaveJobDetails(JobDetails jobDetails)
		{
			Wake();
		}

		private bool WakeOnDeleteJob(string jobName)
		{
			Wake();
			return true;
		}

		private class MockJobWatcher : IJobWatcher
		{
			private readonly JobDetails jobDetails;
			private bool firstTime;
			private bool isDisposed;

			public MockJobWatcher(JobDetails jobDetails)
			{
				this.jobDetails = jobDetails;

				firstTime = true;
			}

			public JobDetails GetNextJobToProcess()
			{
				lock (this)
				{
					while (!isDisposed)
					{
						if (jobDetails != null && firstTime)
						{
							firstTime = false;
							return jobDetails;
						}

						Monitor.Wait(this);
					}
				}

				return null;
			}

			public void Dispose()
			{
				lock (this)
				{
					isDisposed = true;
					Monitor.PulseAll(this);
				}
			}
		}
	}
}