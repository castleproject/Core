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

	/// <summary>
	/// A job store Data Access Object provides data access for <see cref="PersistentJobStore" />
	/// implementations.  The DAO may assume that all of its inputs have been pre-validated
	/// by the calling code.
	/// </summary>
	public interface IJobStoreDao
	{
		/// <summary>
		/// Registers a scheduler.
		/// </summary>
		/// <param name="clusterName">The cluster name, never null</param>
		/// <param name="schedulerGuid">The scheduler GUID</param>
		/// <param name="schedulerName">The scheduler name, never null</param>
		/// <param name="lastSeenUtc">The time the scheduler was last seen</param>
		/// <exception cref="SchedulerException">Thrown if an error occurs</exception>
		void RegisterScheduler(string clusterName, Guid schedulerGuid, string schedulerName, DateTime lastSeenUtc);

		/// <summary>
		/// Unregisters a scheduler and orphans all of its jobs.
		/// </summary>
		/// <param name="clusterName">The cluster name, never null</param>
		/// <param name="schedulerGuid">The scheduler GUID</param>
		/// <exception cref="SchedulerException">Thrown if an error occurs</exception>
		void UnregisterScheduler(string clusterName, Guid schedulerGuid);

		/// <summary>
		/// Creates a job in the database.
		/// </summary>
		/// <param name="clusterName">The cluster name, never null</param>
		/// <param name="jobSpec">The job specification, never null</param>
		/// <param name="creationTimeUtc">The job creation time</param>
		/// <param name="conflictAction">The action to take if a conflict occurs</param>
		/// <returns>True if the job was created or updated, false if a conflict occurred
		/// and no changes were made</returns>
		/// <exception cref="SchedulerException">Thrown if an error occurs</exception>
		bool CreateJob(string clusterName, JobSpec jobSpec, DateTime creationTimeUtc, CreateJobConflictAction conflictAction);

		/// <summary>
		/// Updates an existing job.
		/// </summary>
		/// <param name="clusterName">The cluster name, never null</param>
		/// <param name="existingJobName">The name of the existing job to update</param>
		/// <param name="updatedJobSpec">The updated job specification</param>
		/// <exception cref="SchedulerException">Thrown if an error occurs or if the job does not exist</exception>
		void UpdateJob(string clusterName, string existingJobName, JobSpec updatedJobSpec);

		/// <summary>
		/// Deletes the job with the specified name.
		/// </summary>
		/// <param name="clusterName">The cluster name, never null</param>
		/// <param name="jobName">The job name, never null</param>
		/// <returns>True if a job was actually deleted</returns>
		/// <exception cref="SchedulerException">Thrown if an error occurs</exception>
		bool DeleteJob(string clusterName, string jobName);

		/// <summary>
		/// Gets details for the named job.
		/// </summary>
		/// <param name="clusterName">The cluster name, never null</param>
		/// <param name="jobName">The job name, never null</param>
		/// <returns>The job details, or null if none was found</returns>
		/// <exception cref="SchedulerException">Thrown if an error occurs</exception>
		VersionedJobDetails GetJobDetails(string clusterName, string jobName);

		/// <summary>
		/// Saves details for the job.
		/// </summary>
		/// <param name="clusterName">The cluster name, never null</param>
		/// <param name="jobDetails">The job details, never null</param>
		/// <exception cref="SchedulerException">Thrown if an error occurs</exception>
		void SaveJobDetails(string clusterName, VersionedJobDetails jobDetails);

		/// <summary>
		/// Gets the next job to process for the specified scheduler.
		/// </summary>
		/// <param name="clusterName">The cluster name, never null</param>
		/// <param name="schedulerGuid">The scheduler GUID</param>
		/// <param name="timeBasisUtc">The UTC time to consider as "now"</param>
		/// <param name="nextTriggerFireTimeUtc">Set to the UTC next trigger fire time, or null if there are
		/// no triggers currently scheduled to fire</param>
		/// <param name="schedulerExpirationTimeInSeconds">The scheduler expiration time in seconds, always greater than zero</param>
		/// <returns>The details of job to process or null if none</returns>
		/// <exception cref="SchedulerException">Thrown if an error occurs</exception>
		VersionedJobDetails GetNextJobToProcess(string clusterName, Guid schedulerGuid, DateTime timeBasisUtc,
		                                        int schedulerExpirationTimeInSeconds, out DateTime? nextTriggerFireTimeUtc);

		/// <summary>
		/// Gets the names of all jobs.
		/// </summary>
		/// <param name="clusterName">The cluster name, never null</param>
		/// <returns>The names of all jobs</returns>
		/// <exception cref="SchedulerException">Thrown if an error occurs</exception>
		string[] ListJobNames(string clusterName);
	}
}