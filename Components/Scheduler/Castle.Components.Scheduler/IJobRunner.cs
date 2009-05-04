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
	/// A job runner takes care of reliably running jobs and reporting
	/// back when they complete.
	/// </summary>
	public interface IJobRunner
	{
		/// <summary>
		/// Begins executing a job asynchronously.
		/// </summary>
		/// <param name="context">The job's execution context, never null</param>
		/// <param name="asyncCallback">The callback to invoke when the job completes</param>
		/// <param name="asyncState">The state information for the asynchronous result token</param>
		/// <returns>The asynchronous result token, never null</returns>
		/// <exception cref="Exception">Any exception thrown by the job runner is interpreted
		/// as job failure by the scheduler.  Changes made to the job's state data
		/// will be discarded when this occurs.  The job may be executed again later
		/// if it is scheduled to do so.</exception>
		IAsyncResult BeginExecute(JobExecutionContext context,
		                          AsyncCallback asyncCallback, object asyncState);

		/// <summary>
		/// Gets the result of having executed a job.
		/// </summary>
		/// <param name="asyncResult">The asynchronous result token</param>
		/// <returns>True if the job succeeded, false otherwise</returns>
		/// <exception cref="Exception">Any exception thrown by the job runner is interpreted
		/// as job failure by the scheduler.  Changes made to the job's state data
		/// will be discarded when this occurs.  The job may be executed again later
		/// if it is scheduled to do so.</exception>
		bool EndExecute(IAsyncResult asyncResult);
	}
}