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
	/// A job watcher monitors the status of jobs so that the scheduler
	/// can process them when the trigger elapses.
	/// </summary>
	public interface IJobWatcher : IDisposable
	{
		/// <summary>
		/// <para>
		/// Gets details for any job whose state is <see cref="JobState.Pending" />,
		/// <see cref="JobState.Triggered" />, <see cref="JobState.Completed" />
		/// or <see cref="JobState.Orphaned" />.
		/// </para>
		/// <para>
		/// The watcher also identifies jobs in the <see cref="JobState.Scheduled" /> state whose
		/// <see cref="JobDetails.NextTriggerFireTimeUtc"/> is null or has elapsed, automatically moves
		/// them to the <see cref="JobState.Triggered" /> state, and eventually returns them.
		/// </para>
		/// </summary>
		/// <remarks>
		/// <para>
		/// The same job may be returned repeatedly until it is deleted,
		/// its status is updated or a new trigger schedule time is set.
		/// </para>
		/// <para>
		/// This method blocks until a job is available or the watcher
		/// is disposed, whichever comes first.
		/// </para>
		/// <para>
		/// In a clustered environment, a job watcher might chooses to return jobs
		/// evenly or randomly to different scheduler instances so as to improve load balancing.
		/// </para>
		/// </remarks>
		/// <returns>The next job to process, or null if the watcher was disposed</returns>
		/// <exception cref="ObjectDisposedException">Thrown if the job store has been disposed (but not the watcher)</exception>
		JobDetails GetNextJobToProcess();
	}
}