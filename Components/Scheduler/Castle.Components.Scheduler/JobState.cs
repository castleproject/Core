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
	/// <summary>
	/// Describes the state of a job.
	/// </summary>
	public enum JobState
	{
		/// <summary>
		/// The job has just been created or updated and is waiting to be scheduled.
		/// </summary>
		Pending = 0,

		/// <summary>
		/// The job has been scheduled and is waiting for its trigger to fire.
		/// </summary>
		Scheduled = 1,

		/// <summary>
		/// The job's trigger has fired.
		/// </summary>
		Triggered = 2,

		/// <summary>
		/// The job is running.
		/// </summary>
		Running = 3,

		/// <summary>
		/// The job completed.  Whether it suceeded or failed can be determined by examining
		/// the last job execution details.
		/// </summary>
		Completed = 4,

		/// <summary>
		/// The job was previously running but has been orphaned because the scheduler instance
		/// that was previously managing its execution no longer exists.
		/// </summary>
		Orphaned = 5,

		/// <summary>
		/// The job has been stopped.  Its trigger will not fire again.
		/// </summary>
		Stopped = 6
	}
}