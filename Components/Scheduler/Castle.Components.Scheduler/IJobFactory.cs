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
	/// A factory that creates <see cref="IJob" /> instances associated
	/// with a given job key prior to job execution.
	/// </summary>
	public interface IJobFactory
	{
		/// <summary>
		/// Gets an instance of a job with the specified key.
		/// </summary>
		/// <param name="jobKey">A key which determines which implementation of
		/// <see cref="IJob" /> is used and perhaps how it is initialized</param>
		/// <returns>The new job</returns>
		/// <exception cref="SchedulerException">Thrown if no such job key is known or if some other error
		/// occurs while initializing the job</exception>
		IJob GetJob(string jobKey);

		/// <summary>
		/// Releases the instance of the job.
		/// </summary>
		/// <param name="job">The <see cref="IJob"/> returned from <see cref="GetJob"/></param>
		void ReleaseJob(IJob job);
	}
}