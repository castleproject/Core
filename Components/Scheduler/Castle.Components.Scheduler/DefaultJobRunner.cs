// Copyright 2007 Castle Project - http://www.castleproject.org/
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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace Castle.Components.Scheduler
{
    /// <summary>
    /// The default implementation of the <see cref="IJobRunner" />
    /// uses a <see cref="IJobFactory" /> to create instances of
    /// <see cref="IJob" /> based on the job's key.  The job runs
    /// asynchronously in the threadpool.
    /// </summary>
    public class DefaultJobRunner : IJobRunner
    {
        private IJobFactory jobFactory;
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

        /// <inheritdoc />
        public IAsyncResult BeginExecute(JobExecutionContext context, AsyncCallback asyncCallback, object asyncState)
        {
            IJob job;
            try
            {
                job = jobFactory.GetJob(context.JobSpec.JobKey);
            }
            catch (Exception ex)
            {
                throw new SchedulerException(String.Format(CultureInfo.CurrentCulture,
                    "The job factory failed to construct a job instance for job key '{0}' "
                    + "associated with job '{1}'.",
                    context.JobSpec.JobKey, context.JobSpec.Name), ex);
            }

            ExecuteDelegate executeDelegate = job.Execute;
            CompositeAsyncResult result = new CompositeAsyncResult(executeDelegate, asyncCallback);
            result.Inner = executeDelegate.BeginInvoke(context, result.Callback, asyncState);
            return result;
        }

        /// <inheritdoc />
        public bool EndExecute(IAsyncResult asyncResult)
        {
            CompositeAsyncResult compositeResult = (CompositeAsyncResult)asyncResult;
            return compositeResult.ExecuteDelegate.EndInvoke(compositeResult.Inner);
        }

        private class CompositeAsyncResult : IAsyncResult
        {
            private IAsyncResult inner;
            private ExecuteDelegate executeDelegate;
            private AsyncCallback asyncCallback;

            public CompositeAsyncResult(ExecuteDelegate executeDelegate, AsyncCallback asyncCallback)
            {
                this.executeDelegate = executeDelegate;
                this.asyncCallback = asyncCallback;
            }

            public IAsyncResult Inner
            {
                get { return inner; }
                set { inner = value; }
            }

            public ExecuteDelegate ExecuteDelegate
            {
                get { return executeDelegate; }
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
                get { return inner.AsyncState; }
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
        }
    }
}
