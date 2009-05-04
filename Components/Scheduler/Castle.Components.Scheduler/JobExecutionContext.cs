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
	using Core.Logging;

	/// <summary>
	/// The job execution context informs a job about its environment and provides
	/// it with its job data for state management.
	/// </summary>
	/// <remarks>
	/// <para>
	/// An implementation of <see cref="IScheduler" /> may provide specialized
	/// subclasses of <see cref="JobExecutionContext" /> to clients that can use additional
	/// scheduler-specific job execution context information.  However, clients should
	/// be prepared to handle the <see cref="JobExecutionContext" /> base class as a least common
	/// denominator albeit some of the scheduler's advanced capabilities may thus be unavailable.
	/// </para>
	/// <para>
	/// Changes to the <see cref="JobExecutionContext" /> may be persisted across
	/// job executions; particularly <see cref="JobExecutionContext.JobData" />.
	/// </para>
	/// </remarks>
	public class JobExecutionContext
	{
		private readonly IScheduler scheduler;
		private readonly ILogger logger;
		private readonly JobSpec jobSpec;
		private JobData jobData;

		/// <summary>
		/// Creates a job execution context.
		/// </summary>
		/// <param name="scheduler">The scheduler that is managing the job</param>
		/// <param name="logger">The logger to use for logging job progress</param>
		/// <param name="jobSpec">The job's specification</param>
		/// <param name="jobData">The job state data, or null if none</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="scheduler"/>,
		/// <paramref name="logger"/> or <paramref name="jobSpec"/> is null</exception>
		public JobExecutionContext(IScheduler scheduler, ILogger logger, JobSpec jobSpec, JobData jobData)
		{
			if (scheduler == null)
				throw new ArgumentNullException("scheduler");
			if (logger == null)
				throw new ArgumentNullException("logger");
			if (jobSpec == null)
				throw new ArgumentNullException("jobSpec");

			this.scheduler = scheduler;
			this.logger = logger;
			this.jobSpec = jobSpec;
			this.jobData = jobData;
		}

		/// <summary>
		/// Gets the scheduler that is managing the job.
		/// </summary>
		public IScheduler Scheduler
		{
			get { return scheduler; }
		}

		/// <summary>
		/// Gets the logger for logging job progress.
		/// </summary>
		public ILogger Logger
		{
			get { return logger; }
		}

		/// <summary>
		/// Gets the job specification.
		/// </summary>
		public JobSpec JobSpec
		{
			get { return jobSpec; }
		}

		/// <summary>
		/// Gets or sets the job state data, or null if none.
		/// </summary>
		public JobData JobData
		{
			get { return jobData; }
			set { jobData = value; }
		}
	}
}