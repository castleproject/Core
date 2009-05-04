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
	/// A job store provides a persistence mechanism for jobs.
	/// </summary>
	public interface IJobStore : IDisposable
	{
		/// <summary>
		/// Returns true if the job store has been disposed.
		/// </summary>
		bool IsDisposed { get; }

		/// <summary>
		/// Registers a scheduler instance with the job store.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The scheduler instance must register itself with the job store before it ever
		/// attempts to run jobs.
		/// </para>
		/// <para>
		/// The scheduler instance may register itself with the job store multiple times
		/// and is not considered an error.  It may even re-register itself after a call
		/// to <see cref="UnregisterScheduler" />.
		/// </para>
		/// </remarks>
		/// <param name="schedulerGuid">The scheduler's GUID</param>
		/// <param name="schedulerName">The scheduler's name</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="schedulerName"/> is null</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the job store has been disposed</exception>
		void RegisterScheduler(Guid schedulerGuid, string schedulerName);

		/// <summary>
		/// Unregisters a scheduler instance from the job store.
		/// </summary>
		/// <remarks>
		/// The scheduler instance should unregister itself from the job store just before
		/// it shuts down to ensure that any resources it owns are reclaimed.  All jobs currently
		/// being executed by the scheduler instance are orphaned.  The job store may also
		/// automatically unregister a scheduler instance if it discovers that it no longer exists
		/// (in some implementation defined manner).
		/// </remarks>
		/// <param name="schedulerGuid">The scheduler's GUID</param>
		void UnregisterScheduler(Guid schedulerGuid);

		/// <summary>
		/// Gets a job watcher for the job store.
		/// </summary>
		/// <returns>The job watcher</returns>
		/// <param name="schedulerGuid">The GUID of the scheduler that is watching the job store</param>
		/// <exception cref="ObjectDisposedException">Thrown if the job store has been disposed</exception>
		IJobWatcher CreateJobWatcher(Guid schedulerGuid);

		/// <summary>
		/// Gets the details of the job with the specified name.
		/// </summary>
		/// <param name="jobName">The name of the job</param>
		/// <returns>The job details, or null if not found</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="jobName"/> is null</exception>
		/// <exception cref="SchedulerException">Thrown if an error occurs</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the job store has been disposed</exception>
		JobDetails GetJobDetails(string jobName);

		/// <summary>
		/// Saves the job details details of the job.
		/// </summary>
		/// <param name="jobDetails">The job details to save</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="jobDetails"/> is null</exception>
		/// <exception cref="ConcurrentModificationException">Thrown if another thread or scheduler instance
		/// has concurrently modified the job in such fashion that the
		/// job details could not be saved (an implementation may track this information by
		/// augmenting its job details with a Version token)</exception>
		/// <exception cref="SchedulerException">Thrown if an error occurs</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the job store has been disposed</exception>
		void SaveJobDetails(JobDetails jobDetails);

		/// <summary>
		/// Creates a job.
		/// If the job already exists, takes the specified alternative conflict action.
		/// </summary>
		/// <param name="jobSpec">The job specification</param>
		/// <param name="creationTimeUtc">The UTC creation time to record.</param>
		/// <param name="conflictAction">The action to take if a job with the
		/// same name already exists</param>
		/// <returns>True if the job was created or updated, false if a conflict occurred
		/// and no changes were made</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="jobSpec"/> is null</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="conflictAction"/> is not a defined value</exception>
		/// <exception cref="SchedulerException">Thrown if an error occurs</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the job store has been disposed</exception>
		bool CreateJob(JobSpec jobSpec, DateTime creationTimeUtc, CreateJobConflictAction conflictAction);

		/// <summary>
		/// Updates an existing job.
		/// If the job's status is <see cref="JobState.Scheduled" /> (but not any other state) it
		/// is reset to <see cref="JobState.Pending" /> so that the trigger can be re-evaluated in case
		/// it was changed by the update.
		/// </summary>
		/// <param name="existingJobName">The name of the existing job to update</param>
		/// <param name="updatedJobSpec">The updated job specification</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="existingJobName"/> or
		/// <paramref name="updatedJobSpec"/> is null</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="existingJobName"/> is an empty string</exception>
		/// <exception cref="SchedulerException">Thrown if an error occurs or if the job does not exist</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the scheduler has been disposed</exception>
		void UpdateJob(string existingJobName, JobSpec updatedJobSpec);

		/// <summary>
		/// Deletes the job with the specified name.
		/// </summary>
		/// <param name="jobName">The name of the job to delete</param>
		/// <returns>True if a job was actually deleted, false if no such job was found</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="jobName"/> is null</exception>
		/// <exception cref="SchedulerException">Thrown if an error occurs</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the job store has been disposed</exception>
		bool DeleteJob(string jobName);

		/// <summary>
		/// Gets the names of all jobs.
		/// </summary>
		/// <returns>The names of all jobs</returns>
		/// <exception cref="SchedulerException">Thrown if an error occurs</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the scheduler has been disposed</exception>
		string[] ListJobNames();
	}
}