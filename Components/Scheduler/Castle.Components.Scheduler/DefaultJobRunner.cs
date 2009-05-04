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
	using System.Globalization;
	using System.Threading;
	using Core.Logging;

	/// <summary>
	/// The default implementation of the <see cref="IJobRunner" />
	/// uses a <see cref="IJobFactory" /> to create instances of
	/// <see cref="IJob" /> based on the job's key.  The job runs
	/// asynchronously in the threadpool.
	/// </summary>
	public class DefaultJobRunner : IJobRunner
	{
		private ILogger logger = NullLogger.Instance;

		private readonly IJobFactory jobFactory;

		private delegate bool ExecuteDelegate(JobExecutionContext context);

		/// <summary>
		/// Creates a job runner with the specified job factory
		/// </summary>
		/// <param name="jobFactory">The job factory</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="jobFactory"/> is null</exception>
		public DefaultJobRunner(IJobFactory jobFactory)
		{
			if (jobFactory == null)
				throw new ArgumentNullException("jobFactory");

			this.jobFactory = jobFactory;
		}

		/// <summary>
		/// Logger.
		/// </summary>
		public ILogger Logger
		{
			get { return logger; }
			set { logger = value; }
		}

		/// <inheritdoc />
		public IAsyncResult BeginExecute(JobExecutionContext context, AsyncCallback asyncCallback, object asyncState)
		{
			Logger.Debug("Beginning execute");

			IJob job;
			try
			{
				job = jobFactory.GetJob(context.JobSpec.JobKey);

				Logger.Debug("Getting the job " + context.JobSpec.Name);
			}
			catch (Exception ex)
			{
				throw new SchedulerException(String.Format(CultureInfo.CurrentCulture,
				                                           "The job factory failed to construct a job instance for job key '{0}' "
				                                           + "associated with job '{1}'.",
				                                           context.JobSpec.JobKey, context.JobSpec.Name), ex);
			}
			
			return new JobAsyncResult(job, asyncCallback, context, asyncState);			
		}

		/// <inheritdoc />
		public bool EndExecute(IAsyncResult asyncResult)
		{
			JobAsyncResult jobResult = (JobAsyncResult)asyncResult;
			bool executed = jobResult.End();
			jobFactory.ReleaseJob(jobResult.Job);
			return executed;

			//TODO: Fix it.
			//Something is not right. Sometimes (dunno why) it throws:
			//System.Runtime.Remoting.RemotingException: The async result object is null or of an unexpected type.
		}

		private class JobAsyncResult : IAsyncResult
		{
			private readonly IJob job;
			private IAsyncResult inner;
			private readonly ExecuteDelegate executeDelegate;
			private readonly AsyncCallback asyncCallback;
			private readonly object asyncState;

			public JobAsyncResult(IJob job, AsyncCallback asyncCallback, JobExecutionContext context,
			                            object asyncState)
			{
				this.job = job;
				this.executeDelegate = job.Execute;
				this.asyncCallback = asyncCallback;
				this.asyncState = asyncState;
				inner = executeDelegate.BeginInvoke(context, Callback, asyncState);
			}

			public IJob Job
			{
				get { return job; }
			}

			public IAsyncResult Inner
			{
				get { return inner; }
				set { inner = value; }
			}

			public bool IsCompleted
			{
				get { return inner.IsCompleted; }
			}

			public WaitHandle AsyncWaitHandle
			{
				get { return inner.AsyncWaitHandle; }
			}

			public object AsyncState
			{
				get { return asyncState; }
			}

			public bool CompletedSynchronously
			{
				get { return inner.CompletedSynchronously; }
			}

			public void Callback(IAsyncResult asyncResult)
			{
				if (asyncCallback != null)
					asyncCallback(this);
			}

			public bool End()
			{
				if (inner != null)
					return executeDelegate.EndInvoke(inner);

				return false;
			}
		}
	}
}