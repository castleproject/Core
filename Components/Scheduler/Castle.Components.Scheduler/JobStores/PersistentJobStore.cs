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
	/// Abstract base class for persistent job store that maintains all job state
	/// in a database.  Jobs are persisted across processes and are shared among all
	/// scheduler instances that belong to the same cluster.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Each active scheduler instance is represented as an entity in the database.
	/// When the scheduler instance shuts down (normally or abnormally), its
	/// information expires.  If the scheduler instance had any running jobs
	/// associated with it, they are transitioned into the Orphaned state and
	/// reclaimed automatically.
	/// </para>
	/// </remarks>
	[Singleton]
	public class PersistentJobStore : BaseJobStore
	{
		private readonly object syncRoot = new object();

		private readonly IJobStoreDao jobStoreDao;

		private string clusterName;
		private int schedulerExpirationTimeInSeconds;
		private int pollIntervalInSeconds;

		private readonly Dictionary<Guid, string> registeredSchedulers;

		private Timer registrationTimer;

		/// <summary>
		/// Creates a persistent job store using the specified DAO.
		/// </summary>
		/// <param name="jobStoreDao">The job store DAO to use for persistence</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="jobStoreDao"/> is null</exception>
		public PersistentJobStore(IJobStoreDao jobStoreDao)
		{
			if (jobStoreDao == null)
				throw new ArgumentNullException("jobStoreDao");
			this.jobStoreDao = jobStoreDao;

			clusterName = "Default";
			schedulerExpirationTimeInSeconds = 120;
			pollIntervalInSeconds = 15;

			registeredSchedulers = new Dictionary<Guid, string>();

			StartRegistrationTimer();
		}

		/// <summary>
		/// Gets the underlying job store Data Access Object implementation.
		/// </summary>
		public IJobStoreDao JobStoreDao
		{
			get { return jobStoreDao; }
		}

		/// <summary>
		/// Gets or sets the unique name of the cluster to which scheduler
		/// instances using this job store should belong.
		/// </summary>
		/// <remarks>
		/// The default value is "Default".
		/// </remarks>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
		public string ClusterName
		{
			get { return clusterName; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");
				clusterName = value;
			}
		}

		/// <summary>
		/// Gets or sets the number of seconds before a scheduler instance that has lost database
		/// connectivity expires.  Any jobs that the scheduler instance was running will be orphaned when
		/// the scheduler instance expires.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The default value is 120 (2 minutes).
		/// </para>
		/// <para>
		/// The same value must be used in each job store instance belonging to the same cluster
		/// to ensure correct behavior.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less
		/// than or equal to 0</exception>
		public int SchedulerExpirationTimeInSeconds
		{
			get { return schedulerExpirationTimeInSeconds; }
			set
			{
				if (value <= 0)
					throw new ArgumentOutOfRangeException("value", "The scheduler expiration time must be greater than 0.");

				lock (syncRoot)
				{
					if (value != schedulerExpirationTimeInSeconds)
					{
						schedulerExpirationTimeInSeconds = value;

						StopRegistrationTimer();
						StartRegistrationTimer();
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the number of seconds to wait between database polls for new jobs to be processed.
		/// </summary>
		/// <remarks>
		/// The default value is 15.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less
		/// than or equal to 0</exception>
		public int PollIntervalInSeconds
		{
			get { return pollIntervalInSeconds; }
			set
			{
				if (value <= 0)
					throw new ArgumentOutOfRangeException("value", "The polling interval must be greater than 0.");

				pollIntervalInSeconds = value;
			}
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			lock (syncRoot)
			{
				StopRegistrationTimer();

				IsDisposed = true;
				Monitor.PulseAll(syncRoot);
			}
		}

		/// <inheritdoc />
		public override void RegisterScheduler(Guid schedulerGuid, string schedulerName)
		{
			if (schedulerName == null)
				throw new ArgumentNullException("schedulerName");

			ThrowIfDisposed();

			lock (registeredSchedulers)
			{
				registeredSchedulers[schedulerGuid] = schedulerName;

				DateTime lastSeen = DateTime.UtcNow;
				jobStoreDao.RegisterScheduler(clusterName, schedulerGuid, schedulerName, lastSeen);
			}
		}

		/// <inheritdoc />
		public override void UnregisterScheduler(Guid schedulerGuid)
		{
			ThrowIfDisposed();

			lock (registeredSchedulers)
			{
				registeredSchedulers.Remove(schedulerGuid);
				jobStoreDao.UnregisterScheduler(clusterName, schedulerGuid);
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

			ThrowIfDisposed();

			return jobStoreDao.GetJobDetails(clusterName, jobName);
		}

		/// <inheritdoc />
		public override void SaveJobDetails(JobDetails jobDetails)
		{
			if (jobDetails == null)
				throw new ArgumentNullException("jobStatus");

			VersionedJobDetails versionedJobDetails = (VersionedJobDetails) jobDetails;

			ThrowIfDisposed();

			jobStoreDao.SaveJobDetails(clusterName, versionedJobDetails);
		}

		/// <inheritdoc />
		public override bool CreateJob(JobSpec jobSpec, DateTime creationTimeUtc, CreateJobConflictAction conflictAction)
		{
			if (jobSpec == null)
				throw new ArgumentNullException("jobSpec");
			if (!Enum.IsDefined(typeof (CreateJobConflictAction), conflictAction))
				throw new ArgumentOutOfRangeException("conflictAction");

			ThrowIfDisposed();

			return jobStoreDao.CreateJob(clusterName, jobSpec, creationTimeUtc, conflictAction);
		}

		/// <inheritdoc />
		public override void UpdateJob(string existingJobName, JobSpec updatedJobSpec)
		{
			if (existingJobName == null)
				throw new ArgumentNullException("existingJobName");
			if (existingJobName.Length == 0)
				throw new ArgumentException("existingJobName");
			if (updatedJobSpec == null)
				throw new ArgumentNullException("jobSpec");

			ThrowIfDisposed();

			jobStoreDao.UpdateJob(clusterName, existingJobName, updatedJobSpec);
		}

		/// <inheritdoc />
		public override bool DeleteJob(string jobName)
		{
			if (jobName == null)
				throw new ArgumentNullException("jobName");

			ThrowIfDisposed();

			return jobStoreDao.DeleteJob(clusterName, jobName);
		}

		/// <inheritdoc />
		public override string[] ListJobNames()
		{
			ThrowIfDisposed();

			return jobStoreDao.ListJobNames(clusterName);
		}

		/// <inheritdoc />
		protected override void SignalBlockedThreads()
		{
			lock (syncRoot)
				Monitor.PulseAll(syncRoot);
		}

		/// <inheritdoc />
		protected override JobDetails GetNextJobToProcessOrWaitUntilSignaled(Guid schedulerGuid)
		{
			lock (syncRoot)
			{
				ThrowIfDisposed();

				DateTime timeBasisUtc = DateTime.UtcNow;
				DateTime? nextTriggerFireTimeUtc;
				try
				{
					VersionedJobDetails nextJob = jobStoreDao.GetNextJobToProcess(clusterName, schedulerGuid, timeBasisUtc,
					                                                              schedulerExpirationTimeInSeconds,
					                                                              out nextTriggerFireTimeUtc);

					if (nextJob != null)
						return nextJob;
				}
				catch (Exception ex)
				{
					Logger.Warn(String.Format("The job store was unable to poll the database for the next job to process.  "
					                          + "It will try again in {0} seconds.", pollIntervalInSeconds), ex);
					nextTriggerFireTimeUtc = null;
				}

				// Wait for a signal or the next fire time whichever comes first.
				if (nextTriggerFireTimeUtc.HasValue)
				{
					// Update the time basis because the SP may have taken a non-trivial
					// amount of time to run.
					timeBasisUtc = DateTime.UtcNow;
					if (nextTriggerFireTimeUtc.Value <= timeBasisUtc)
						return null;

					// Need to ensure that wait time in millis will fit in a 32bit integer, otherwise the
					// Monitor.Wait will throw an ArgumentException (even when using the TimeSpan based overload).
					// This can happen when the next trigger fire time is very far out into the future like DateTime.MaxValue.
					TimeSpan waitTimeSpan = nextTriggerFireTimeUtc.Value - timeBasisUtc;
					int waitMillis = Math.Min((int) Math.Min(int.MaxValue, waitTimeSpan.TotalMilliseconds), pollIntervalInSeconds*1000);
					Monitor.Wait(syncRoot, waitMillis);
				}
				else
				{
					Monitor.Wait(syncRoot, pollIntervalInSeconds*1000);
				}
			}

			return null;
		}

		/// <summary>
		/// Starts a timer to refresh registrations at 1/3 of the scheduler expiration rate.
		/// This ensures that the scheduler instances get 2 chances to be refreshed before
		/// they expire.
		/// </summary>
		private void StartRegistrationTimer()
		{
			lock (syncRoot)
			{
				ThrowIfDisposed();

				if (registrationTimer == null)
				{
					int period = schedulerExpirationTimeInSeconds*333;
					registrationTimer = new Timer(RefreshRegistrations, null, period, period);
				}
			}
		}

		/// <summary>
		/// Stops the registration refresh timer.
		/// </summary>
		private void StopRegistrationTimer()
		{
			lock (syncRoot)
			{
				if (registrationTimer != null)
				{
					registrationTimer.Dispose();
					registrationTimer = null;
				}
			}
		}

		private void RefreshRegistrations(object dummy)
		{
			try
			{
				lock (registeredSchedulers)
				{
					DateTime lastSeen = DateTime.UtcNow;

					foreach (KeyValuePair<Guid, string> entry in registeredSchedulers)
						jobStoreDao.RegisterScheduler(clusterName, entry.Key, entry.Value, lastSeen);
				}
			}
			catch (Exception ex)
			{
				Logger.Warn(String.Format(CultureInfo.CurrentCulture,
				                          "The job store was unable to refresh certain scheduler instance "
				                          + "registrations in the database.  It will try again in {0} seconds.",
				                          schedulerExpirationTimeInSeconds*333), ex);
			}
		}
	}
}