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

namespace Castle.Components.Scheduler
{
	using System;

	/// <summary>
	/// A job scheduler schedules jobs for execution.
	/// It may optionally provide job persistence, clustering and other features.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Each scheduler instance is uniquely identified by a GUID which is used
	/// to track ownership of running jobs.  A running job associated with a
	/// scheduler whose GUID is no longer valid is considered orphaned and is
	/// assigned a failure result for that execution.  The orphaned job will
	/// then be rescheduled according to its trigger.
	/// </para>
	/// </remarks>
	/// <todo>
	/// Optionally track job history.
	/// Provide events for instrumentation.
	/// Support job scheduling rules for mutually exclusive access to contended resources.
	/// Support job behaviors (persistent listeners).
	/// Support job queueing.
	/// </todo>
	public interface IScheduler : IDisposable
	{
		/// <summary>
		/// Returns true if the scheduler has been disposed.
		/// </summary>
		bool IsDisposed { get; }

		/// <summary>
		/// Returns true if the scheduler has been started and is running.
		/// </summary>
		bool IsRunning { get; }

		/// <summary>
		/// Gets the globally unique ID of the scheduler instance.
		/// </summary>
		/// <remarks>
		/// The GUID is used to track ownership of resources that
		/// are transiently owned by a scheduler instance.  The system
		/// ensures that when the GUID associated with a scheduler instance
		/// is invalidated (say by failing to update its record in a persistent
		/// store for a preset time) all of its associated resources should
		/// be released.  In particular, if the scheduler instance terminated
		/// abnormally while it had running jobs, these jobs will eventually
		/// be considered orphaned and will be rescheduled by other scheduler
		/// instances per the job's trigger.
		/// </remarks>
		Guid Guid { get; }

		/// <summary>
		/// Gets the name of the scheduler instance, never null.
		/// The name might not be unique.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Starts scheduling jobs for execution.
		/// </summary>
		/// <exception cref="ObjectDisposedException">Thrown if the scheduler has been disposed</exception>
		void Start();

		/// <summary>
		/// Stops scheduling jobs for execution.
		/// </summary>
		/// <exception cref="ObjectDisposedException">Thrown if the scheduler has been disposed</exception>
		void Stop();

		/// <summary>
		/// Gets the details of the job with the specified name.
		/// </summary>
		/// <param name="jobName">The name of the job</param>
		/// <returns>The job details, or null if not found</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="jobName"/> is null</exception>
		/// <exception cref="SchedulerException">Thrown if an error occurs</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the scheduler has been disposed</exception>
		JobDetails GetJobDetails(string jobName);

		/// <summary>
		/// Creates a job.
		/// If the job already exists, takes the specified alternative conflict action.
		/// </summary>
		/// <param name="jobSpec">The job specification</param>
		/// <param name="conflictAction">The action to take if a job with the
		/// same name already exists</param>
		/// <returns>True if the job was created or updated, false if a conflict occurred
		/// and no changes were made</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="jobSpec"/> is null</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="conflictAction"/> is not a defined value</exception>
		/// <exception cref="SchedulerException">Thrown if an error occurs</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the scheduler has been disposed</exception>
		bool CreateJob(JobSpec jobSpec, CreateJobConflictAction conflictAction);

		/// <summary>
		/// Updates an existing job.
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
		/// <exception cref="ObjectDisposedException">Thrown if the scheduler has been disposed</exception>
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