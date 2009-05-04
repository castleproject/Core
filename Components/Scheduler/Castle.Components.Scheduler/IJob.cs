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
	/// A job provides an entry point for a task to be performed.
	/// </summary>
	public interface IJob
	{
		/// <summary>
		/// Executes the job.
		/// </summary>
		/// <param name="context">The job's execution context</param>
		/// <returns>True if the job succeeded, false otherwise</returns>
		/// <exception cref="Exception">Any exception thrown by the job is interpreted
		/// as an error by the scheduler.  Changes made to the job's state data
		/// will be discarded when this occurs.</exception>
		bool Execute(JobExecutionContext context);
	}
}