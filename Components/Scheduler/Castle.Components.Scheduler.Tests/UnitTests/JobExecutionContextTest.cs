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
	using Core.Logging;
	using NUnit.Framework;

	[TestFixture]
	public class JobExecutionContextTest : BaseUnitTest
	{
		private IScheduler scheduler;
		private ILogger logger;
		private JobSpec jobSpec;
		private JobData jobData;

		public override void SetUp()
		{
			base.SetUp();

			scheduler = Mocks.CreateMock<IScheduler>();
			logger = Mocks.CreateMock<ILogger>();
			jobSpec = new JobSpec("abc", "some job", "with.this.key", PeriodicTrigger.CreateDailyTrigger(DateTime.UtcNow));
			jobData = new JobData();
			Mocks.ReplayAll();
		}

		[Test]
		public void ConstructorSetsProperties()
		{
			JobExecutionContext context = new JobExecutionContext(scheduler, logger, jobSpec, jobData);
			Assert.AreSame(scheduler, context.Scheduler);
			Assert.AreSame(logger, context.Logger);
			Assert.AreSame(jobSpec, context.JobSpec);
			Assert.AreSame(jobData, context.JobData);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void ConstructorThrowsWhenSchedulerIsNull()
		{
			new JobExecutionContext(null, logger, jobSpec, jobData);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void ConstructorThrowsWhenLoggerIsNull()
		{
			new JobExecutionContext(scheduler, null, jobSpec, jobData);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void ConstructorThrowsWhenJobSpecIsNull()
		{
			new JobExecutionContext(scheduler, logger, null, jobData);
		}

		[Test]
		public void JobData_GetterAndSetter()
		{
			JobExecutionContext context = new JobExecutionContext(scheduler, logger, jobSpec, jobData);
			context.JobData = null;
			Assert.IsNull(context.JobData);
		}
	}
}