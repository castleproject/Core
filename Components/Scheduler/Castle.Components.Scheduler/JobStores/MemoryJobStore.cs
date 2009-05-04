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

namespace Castle.Components.Scheduler.JobStores
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Threading;
	using Core;

	/// <summary>
	/// The memory job store maintains all job state in-process in memory.
	/// It does not support persistence or clustering.
	/// </summary>
	[Singleton]
	public class MemoryJobStore : BaseJobStore
	{
		private readonly Dictionary<string, VersionedJobDetails> jobs;

		/// <summary>
		/// Creates an in-process memory job store initially without any jobs.
		/// </summary>
		public MemoryJobStore()
		{
			jobs = new Dictionary<string, VersionedJobDetails>();
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			lock (jobs)
			{
				IsDisposed = true;
				jobs.Clear();

				Monitor.PulseAll(jobs);
			}
		}

		/// <inheritdoc />
		public override void RegisterScheduler(Guid schedulerGuid, string schedulerName)
		{
			if (schedulerName == null)
				throw new ArgumentNullException("schedulerName");

			ThrowIfDisposed();

			// The memory job store has no use for the registration information itself.
		}

		/// <inheritdoc />
		public override void UnregisterScheduler(Guid schedulerGuid)
		{
			lock (jobs)
			{
				ThrowIfDisposed();

				// Orphan any jobs that are still running.
				bool jobsWereOrphaned = false;
				foreach (VersionedJobDetails job in jobs.Values)
				{
					JobExecutionDetails execution = job.LastJobExecutionDetails;
					if (job.JobState == JobState.Running && execution.SchedulerGuid == schedulerGuid)
					{
						job.JobState = JobState.Orphaned;
						jobsWereOrphaned = true;
					}
				}

				if (jobsWereOrphaned)
					SignalBlockedThreads();
			}
		}

		/// <inheritdoc />
		public override IJobWatcher CreateJobWatcher(Guid schedulerGuid)
		{
			ThrowIfDisposed();

			return new JobWatcher(this, schedulerGuid);
		}

		/// <inheritdoc />
		public override JobDetails GetJobDetails(string jobName)
		{
			if (jobName == null)
				throw new ArgumentNullException("jobName");

			lock (jobs)
			{
				ThrowIfDisposed();

				VersionedJobDetails jobDetails;
				if (jobs.TryGetValue(jobName, out jobDetails))
					return jobDetails.Clone();

				return null;
			}
		}

		/// <inheritdoc />
		public override void SaveJobDetails(JobDetails jobDetails)
		{
			if (jobDetails == null)
				throw new ArgumentNullException("jobDetails");

			VersionedJobDetails versionedJobDetails = (VersionedJobDetails) jobDetails;

			lock (jobs)
			{
				ThrowIfDisposed();

				string jobName = jobDetails.JobSpec.Name;

				VersionedJobDetails existingJobDetails;
				if (! jobs.TryGetValue(jobName, out existingJobDetails))
					throw new ConcurrentModificationException(
						"The job details could not be saved because the job was concurrently deleted.");

				if (existingJobDetails.Version != versionedJobDetails.Version)
					throw new ConcurrentModificationException(
						"The job details could not be saved because the job was concurrently modified.");

				versionedJobDetails.Version += 1;
				jobs[jobName] = (VersionedJobDetails) versionedJobDetails.Clone();

				Monitor.PulseAll(jobs);
			}
		}

		/// <inheritdoc />
		public override bool CreateJob(JobSpec jobSpec, DateTime creationTimeUtc, CreateJobConflictAction conflictAction)
		{
			if (jobSpec == null)
				throw new ArgumentNullException("jobSpec");
			if (!Enum.IsDefined(typeof (CreateJobConflictAction), conflictAction))
				throw new ArgumentOutOfRangeException("conflictAction");

			lock (jobs)
			{
				ThrowIfDisposed();

				VersionedJobDetails existingJobDetails;
				if (jobs.TryGetValue(jobSpec.Name, out existingJobDetails))
				{
					switch (conflictAction)
					{
						case CreateJobConflictAction.Ignore:
							return false;

						case CreateJobConflictAction.Update:
							InternalUpdateJob(existingJobDetails, jobSpec);
							return true;

						case CreateJobConflictAction.Replace:
							break;

						case CreateJobConflictAction.Throw:
							throw new SchedulerException(String.Format(CultureInfo.CurrentCulture,
							                                           "There is already a job with name '{0}'.", jobSpec.Name));
					}
				}

				VersionedJobDetails jobDetails = new VersionedJobDetails(jobSpec.Clone(), creationTimeUtc, 0);

				jobs[jobSpec.Name] = jobDetails;
				Monitor.PulseAll(jobs);
				return true;
			}
		}

		/// <inheritdoc />
		public override void UpdateJob(string existingJobName, JobSpec updatedJobSpec)
		{
			if (existingJobName == null)
				throw new ArgumentNullException("existingJobName");
			if (existingJobName.Length == 0)
				throw new ArgumentException("existingJobName");
			if (updatedJobSpec == null)
				throw new ArgumentNullException("updatedJobSpec");

			lock (jobs)
			{
				ThrowIfDisposed();

				VersionedJobDetails existingJobDetails;
				if (! jobs.TryGetValue(existingJobName, out existingJobDetails))
					throw new SchedulerException(String.Format(CultureInfo.CurrentCulture,
					                                           "There is no existing job named '{0}'.", existingJobName));

				InternalUpdateJob(existingJobDetails, updatedJobSpec);
			}
		}

		private void InternalUpdateJob(VersionedJobDetails existingJobDetails, JobSpec updatedJobSpec)
		{
			if (existingJobDetails.JobSpec.Name != updatedJobSpec.Name)
			{
				if (jobs.ContainsKey(updatedJobSpec.Name))
					throw new SchedulerException(String.Format(CultureInfo.CurrentCulture,
					                                           "Cannot rename job '{0}' to '{1}' because there already exists another job with the new name.",
					                                           existingJobDetails.JobSpec.Name, updatedJobSpec.Name));

				jobs.Remove(existingJobDetails.JobSpec.Name);
				jobs.Add(updatedJobSpec.Name, existingJobDetails);
			}

			existingJobDetails.Version += 1;
			existingJobDetails.JobSpec = updatedJobSpec.Clone();

			if (existingJobDetails.JobState == JobState.Scheduled)
				existingJobDetails.JobState = JobState.Pending;

			Monitor.PulseAll(jobs);
		}

		/// <inheritdoc />
		public override bool DeleteJob(string jobName)
		{
			if (jobName == null)
				throw new ArgumentNullException("jobName");

			lock (jobs)
			{
				ThrowIfDisposed();

				if (!jobs.Remove(jobName))
					return false;

				Monitor.PulseAll(jobs);
				return true;
			}
		}

		/// <inheritdoc />
		public override string[] ListJobNames()
		{
			lock (jobs)
			{
				ThrowIfDisposed();

				string[] jobNames = new string[jobs.Count];
				jobs.Keys.CopyTo(jobNames, 0);
				return jobNames;
			}
		}

		/// <inheritdoc />
		protected override void SignalBlockedThreads()
		{
			lock (jobs)
				Monitor.PulseAll(jobs);
		}

		/// <inheritdoc />
		protected override JobDetails GetNextJobToProcessOrWaitUntilSignaled(Guid schedulerGuid)
		{
			lock (jobs)
			{
				ThrowIfDisposed();

				DateTime timeBasis = DateTime.UtcNow;
				DateTime? waitNextTriggerFireTime = null;

				foreach (VersionedJobDetails jobDetails in jobs.Values)
				{
					switch (jobDetails.JobState)
					{
						case JobState.Scheduled:
							if (jobDetails.NextTriggerFireTimeUtc.HasValue)
							{
								DateTime jobNextTriggerFireTimeUtc = jobDetails.NextTriggerFireTimeUtc.Value;

								if (jobNextTriggerFireTimeUtc > timeBasis)
								{
									if (!waitNextTriggerFireTime.HasValue || jobNextTriggerFireTimeUtc < waitNextTriggerFireTime.Value)
										waitNextTriggerFireTime = jobNextTriggerFireTimeUtc;
									break;
								}
							}

							jobDetails.JobState = JobState.Triggered;
							return jobDetails.Clone();

						case JobState.Pending:
						case JobState.Triggered:
						case JobState.Orphaned:
						case JobState.Completed:
							return jobDetails.Clone();
					}
				}

				// Otherwise wait for a signal or the next fire time whichever comes first.
				if (waitNextTriggerFireTime.HasValue)
				{
					// Need to ensure that wait time in millis will fit in a 32bit integer, otherwise the
					// Monitor.Wait will throw an ArgumentException (even when using the TimeSpan based overload).
					// This can happen when the next trigger fire time is very far out into the future like DateTime.MaxValue.
					TimeSpan waitTimeSpan = waitNextTriggerFireTime.Value - timeBasis;
					int waitMillis = (int) Math.Min(int.MaxValue, waitTimeSpan.TotalMilliseconds);

					Monitor.Wait(jobs, waitMillis);
				}
				else
				{
					Monitor.Wait(jobs);
				}
			}

			return null;
		}
	}
}