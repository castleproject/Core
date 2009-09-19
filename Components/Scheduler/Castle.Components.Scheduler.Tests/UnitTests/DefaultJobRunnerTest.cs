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

namespace Castle.Components.Scheduler.Tests.UnitTests
{
	using System;
	using System.Threading;
	using Core.Logging;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class DefaultJobRunnerTest : BaseUnitTest
	{
		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Constructor_ThrowsIfJobFactoryIsNull()
		{
			new DefaultJobRunner(null);
		}

		[Test]
		public void CreateRunnerAndExecuteJob_NoCallback()
		{
			JobExecutionContext context = new JobExecutionContext(
				Mocks.CreateMock<IScheduler>(), Mocks.CreateMock<ILogger>(),
				new JobSpec("job", "description", "key", Mocks.CreateMock<Trigger>()), null);

			IJobFactory jobFactory = Mocks.CreateMock<IJobFactory>();
			IJob job = Mocks.CreateMock<IJob>();

			Expect.Call(jobFactory.GetJob("key")).Return(job);
			Expect.Call(job.Execute(context)).Return(true);
			Expect.Call(delegate { jobFactory.ReleaseJob(null); }).IgnoreArguments();

			Mocks.ReplayAll();

			DefaultJobRunner runner = new DefaultJobRunner(jobFactory);
			IAsyncResult asyncResult = runner.BeginExecute(context, null, "state");
			Assert.AreEqual("state", asyncResult.AsyncState);
			Assert.IsNotNull(asyncResult.AsyncWaitHandle);
			Assert.IsFalse(asyncResult.CompletedSynchronously);
			Assert.IsTrue(runner.EndExecute(asyncResult));
			Assert.IsTrue(asyncResult.IsCompleted);
		}

		[Test]
		public void CreateRunnerAndExecuteJob_WithCallback()
		{
			JobExecutionContext context = new JobExecutionContext(
				Mocks.CreateMock<IScheduler>(), Mocks.CreateMock<ILogger>(),
				new JobSpec("job", "description", "key", Mocks.CreateMock<Trigger>()), null);

			IJobFactory jobFactory = Mocks.CreateMock<IJobFactory>();
			IJob job = Mocks.CreateMock<IJob>();

			Expect.Call(jobFactory.GetJob("key")).Return(job);
			Expect.Call(job.Execute(context)).Return(true);
			Expect.Call(delegate { jobFactory.ReleaseJob(null); }).IgnoreArguments();

			Mocks.ReplayAll();

			DefaultJobRunner runner = new DefaultJobRunner(jobFactory);

			IAsyncResult resultPassedToCallback = null;
			object barrier = new object();

			IAsyncResult asyncResult = runner.BeginExecute(context, delegate(IAsyncResult r)
			{
				resultPassedToCallback = r;

				lock (barrier)
					Monitor.PulseAll(barrier);
			}, "state");

			lock (barrier)
				if (resultPassedToCallback == null)
					Monitor.Wait(barrier, 10000);

			Assert.AreEqual("state", asyncResult.AsyncState);
			Assert.IsNotNull(asyncResult.AsyncWaitHandle);
			Assert.IsFalse(asyncResult.CompletedSynchronously);
			Assert.IsTrue(runner.EndExecute(asyncResult));
			Assert.IsTrue(asyncResult.IsCompleted);

			Assert.AreSame(asyncResult, resultPassedToCallback);
		}

		[Test]
		[ExpectedException(typeof (SchedulerException))]
		public void WrapsJobFactoryException()
		{
			JobExecutionContext context = new JobExecutionContext(
				Mocks.CreateMock<IScheduler>(), Mocks.CreateMock<ILogger>(),
				new JobSpec("job", "description", "key", Mocks.CreateMock<Trigger>()), null);

			IJobFactory jobFactory = Mocks.CreateMock<IJobFactory>();
			Expect.Call(jobFactory.GetJob("key")).Throw(new Exception("Ack!"));
			Mocks.ReplayAll();

			DefaultJobRunner jobRunner = new DefaultJobRunner(jobFactory);

			jobRunner.BeginExecute(context, null, null);
		}
	}
}