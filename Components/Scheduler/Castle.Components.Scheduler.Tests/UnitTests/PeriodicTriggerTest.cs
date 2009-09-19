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
	using NUnit.Framework;
	using Utilities;

	[TestFixture]
	public class PeriodicTriggerTest : BaseUnitTest
	{
		[Test]
		public void ConstructorSetsProperties()
		{
			TimeSpan interval = new TimeSpan(0, 1, 30);
			PeriodicTrigger trigger = new PeriodicTrigger(new DateTime(2000, 3, 4), new DateTime(1999, 1, 2), interval, 33);

			DateTimeAssert.AreEqualIncludingKind(new DateTime(2000, 3, 4, 0, 0, 0, DateTimeKind.Utc), trigger.StartTimeUtc);
			Assert.AreEqual(interval, trigger.Period);
			Assert.AreEqual(PeriodicTrigger.DefaultMisfireAction, trigger.MisfireAction);
			Assert.AreEqual(null, trigger.MisfireThreshold);
			DateTimeAssert.AreEqualIncludingKind(new DateTime(1999, 1, 2, 0, 0, 0, DateTimeKind.Utc), trigger.EndTimeUtc);
			Assert.AreEqual(33, trigger.JobExecutionCountRemaining);
			Assert.IsTrue(trigger.IsFirstTime);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void Constructor_ThrowsIfPeriodIsZero()
		{
			new PeriodicTrigger(DateTime.UtcNow, null, TimeSpan.Zero, null);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void Constructor_ThrowsIfPeriodIsNegative()
		{
			new PeriodicTrigger(DateTime.UtcNow, null, TimeSpan.MinValue, null);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void Constructor_ThrowsIfJobExecutionCountIsNegative()
		{
			new PeriodicTrigger(DateTime.UtcNow, null, null, -1);
		}

		[Test]
		public void StartTimeUtc_GetterAndSetter()
		{
			PeriodicTrigger trigger = new PeriodicTrigger(DateTime.MaxValue, null, null, null);

			trigger.StartTimeUtc = new DateTime(2000, 3, 4);
			DateTimeAssert.AreEqualIncludingKind(new DateTime(2000, 3, 4, 0, 0, 0, DateTimeKind.Utc), trigger.StartTimeUtc);
		}

		[Test]
		public void EndTimeUtc_GetterAndSetter()
		{
			PeriodicTrigger trigger = new PeriodicTrigger(DateTime.MaxValue, null, null, null);

			trigger.EndTimeUtc = new DateTime(2000, 3, 4);
			DateTimeAssert.AreEqualIncludingKind(new DateTime(2000, 3, 4, 0, 0, 0, DateTimeKind.Utc), trigger.EndTimeUtc);
		}

		[Test]
		public void Period_GetterAndSetter()
		{
			PeriodicTrigger trigger = new PeriodicTrigger(DateTime.UtcNow, null, TimeSpan.MaxValue, null);

			TimeSpan value = new TimeSpan(0, 1, 0);
			trigger.Period = value;
			Assert.AreEqual(value, trigger.Period);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void Period_ThrowsIfValueIsZero()
		{
			PeriodicTrigger trigger = new PeriodicTrigger(DateTime.UtcNow, null, null, null);
			trigger.Period = TimeSpan.Zero;
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void Period_ThrowsIfValueIsNegative()
		{
			PeriodicTrigger trigger = new PeriodicTrigger(DateTime.UtcNow, null, null, null);
			trigger.Period = TimeSpan.MinValue;
		}

		[Test]
		public void JobExecutionCountRemaining_GetterAndSetter()
		{
			PeriodicTrigger trigger = new PeriodicTrigger(DateTime.UtcNow, null, null, 33);

			trigger.JobExecutionCountRemaining = 42;
			Assert.AreEqual(42, trigger.JobExecutionCountRemaining);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void JobExecutionCountRemaining_ThrowsIfValueIsNegative()
		{
			PeriodicTrigger trigger = new PeriodicTrigger(DateTime.UtcNow, null, null, null);
			trigger.JobExecutionCountRemaining = -1;
		}

		[Test]
		public void MisfireAction_GetterAndSetter()
		{
			PeriodicTrigger trigger = new PeriodicTrigger(DateTime.UtcNow, null, null, null);

			trigger.MisfireAction = TriggerScheduleAction.ExecuteJob;
			Assert.AreEqual(TriggerScheduleAction.ExecuteJob, trigger.MisfireAction);
		}

		[Test]
		public void MisfireThreshold_GetterAndSetter()
		{
			PeriodicTrigger trigger = new PeriodicTrigger(DateTime.UtcNow, null, null, null);

			trigger.MisfireThreshold = new TimeSpan(1, 0, 0);
			Assert.AreEqual(new TimeSpan(1, 0, 0), trigger.MisfireThreshold);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void MisfireThreshold_ThrowsIfValueIsNegative()
		{
			PeriodicTrigger trigger = new PeriodicTrigger(DateTime.UtcNow, null, null, null);
			trigger.MisfireThreshold = TimeSpan.MinValue;
		}

		[Test]
		public void IsFirstTime_GetterAndSetter()
		{
			PeriodicTrigger trigger = new PeriodicTrigger(DateTime.UtcNow, null, TimeSpan.MaxValue, null);

			trigger.IsFirstTime = false;
			Assert.IsFalse(trigger.IsFirstTime);
		}

		[Test]
		public void CreateDailyTrigger()
		{
			DateTime now = DateTime.UtcNow;
			PeriodicTrigger trigger = PeriodicTrigger.CreateDailyTrigger(DateTime.UtcNow);

			Assert.AreEqual(now, trigger.StartTimeUtc);
			Assert.AreEqual(new TimeSpan(24, 0, 0), trigger.Period);
			Assert.IsNull(trigger.EndTimeUtc);
			Assert.IsNull(trigger.JobExecutionCountRemaining);
			Assert.IsTrue(trigger.IsFirstTime);
		}

		[Test]
		public void CreateOneShotTrigger()
		{
			DateTime now = DateTime.UtcNow;
			PeriodicTrigger trigger = PeriodicTrigger.CreateOneShotTrigger(DateTime.UtcNow);

			Assert.AreEqual(now, trigger.StartTimeUtc);
			Assert.IsNull(trigger.Period);
			Assert.IsNull(trigger.EndTimeUtc);
			Assert.AreEqual(1, trigger.JobExecutionCountRemaining);
			Assert.IsTrue(trigger.IsFirstTime);
		}

		[TestCase(false)]
		[TestCase(true)]
		public void ClonePerformsADeepCopy(bool useGenericClonable)
		{
			DateTime now = DateTime.UtcNow;
			PeriodicTrigger trigger = new PeriodicTrigger(now, DateTime.MaxValue, TimeSpan.MaxValue, 42);
			trigger.MisfireAction = TriggerScheduleAction.DeleteJob;
			trigger.MisfireThreshold = TimeSpan.MaxValue;
			trigger.Schedule(TriggerScheduleCondition.Latch, now, null);

			PeriodicTrigger clone = useGenericClonable
			                        	? (PeriodicTrigger) trigger.Clone()
			                        	: (PeriodicTrigger) ((ICloneable) trigger).Clone();

			Assert.AreNotSame(trigger, clone);

			Assert.AreEqual(trigger.StartTimeUtc, clone.StartTimeUtc);
			Assert.AreEqual(trigger.EndTimeUtc, clone.EndTimeUtc);
			Assert.AreEqual(trigger.Period, clone.Period);
			Assert.AreEqual(trigger.JobExecutionCountRemaining, clone.JobExecutionCountRemaining);
			Assert.AreEqual(trigger.MisfireAction, clone.MisfireAction);
			Assert.AreEqual(trigger.MisfireThreshold, clone.MisfireThreshold);
			Assert.AreEqual(trigger.IsFirstTime, clone.IsFirstTime);

			Assert.AreEqual(trigger.IsActive, clone.IsActive);
			Assert.AreEqual(trigger.NextFireTimeUtc, clone.NextFireTimeUtc);
			Assert.AreEqual(trigger.NextMisfireThreshold, clone.NextMisfireThreshold);
		}

		// One-shot triggers
		[TestCase(1, 0, 0, 1, TriggerScheduleAction.Skip, TriggerScheduleCondition.Latch, 1, true,
			TriggerScheduleAction.Skip, 1, 1, true, true, Description = "One-shot latch first time.")]
		[TestCase(1, 0, 0, 0, TriggerScheduleAction.Skip, TriggerScheduleCondition.Latch, 1, false,
			TriggerScheduleAction.Stop, 0, 0, false, false, Description = "One-shot latch second time succeeded yields stop.")]
		[TestCase(1, 0, 0, 1, TriggerScheduleAction.Skip, TriggerScheduleCondition.Fire, 1, true,
			TriggerScheduleAction.ExecuteJob, 0, 0, false, true, Description = "One-shot fire.")]
		[TestCase(1, 0, 0, 1, TriggerScheduleAction.Skip, TriggerScheduleCondition.Misfire, 2, true,
			TriggerScheduleAction.Stop, 0, 0, false, true, Description = "One-shot misfire skip yields stop.")]
		[TestCase(1, 0, 0, 1, TriggerScheduleAction.ExecuteJob, TriggerScheduleCondition.Misfire, 2, true,
			TriggerScheduleAction.ExecuteJob, 0, 0, false, true, Description = "One-shot misfire execute yields execute.")]
		[TestCase(1, 0, 0, 1, TriggerScheduleAction.DeleteJob, TriggerScheduleCondition.Misfire, 2, true,
			TriggerScheduleAction.DeleteJob, 0, 0, false, true, Description = "One-shot misfire deletejob yields deletejob.")]
		[TestCase(1, 0, 0, 1, TriggerScheduleAction.Stop, TriggerScheduleCondition.Misfire, 2, true,
			TriggerScheduleAction.Stop, 0, 0, false, true, Description = "One-shot misfire stop yields stop.")]
		// One-shot triggers, fired before start time
		[TestCase(3, 0, 0, 1, TriggerScheduleAction.Skip, TriggerScheduleCondition.Fire, 1, true,
			TriggerScheduleAction.Skip, 3, 1, true, true, Description = "One-shot fire before start time.")]
		// Periodic unlimited triggers
		[TestCase(1, 0, 1, -1, TriggerScheduleAction.Skip, TriggerScheduleCondition.Latch, 1, true,
			TriggerScheduleAction.Skip, 1, -1, true, true, Description = "Periodic latch first time.")]
		[TestCase(1, 0, 1, -1, TriggerScheduleAction.Skip, TriggerScheduleCondition.Latch, 1, false,
			TriggerScheduleAction.Skip, 2, -1, true, false, Description = "Periodic latch second time yields skip.")]
		[TestCase(1, 0, 1, -1, TriggerScheduleAction.Skip, TriggerScheduleCondition.Fire, 1, true,
			TriggerScheduleAction.ExecuteJob, 0, -1, true, true, Description = "Periodic fire.")]
		[TestCase(1, 0, 1, -1, TriggerScheduleAction.Skip, TriggerScheduleCondition.Misfire, 2, true,
			TriggerScheduleAction.Skip, 3, -1, true, true, Description = "Periodic misfire skip yields skip.")]
		[TestCase(1, 0, 1, -1, TriggerScheduleAction.ExecuteJob, TriggerScheduleCondition.Misfire, 2, true,
			TriggerScheduleAction.ExecuteJob, 0, -1, true, true, Description = "Periodic misfire execute yields execute.")]
		[TestCase(1, 0, 1, -1, TriggerScheduleAction.DeleteJob, TriggerScheduleCondition.Misfire, 2, true,
			TriggerScheduleAction.DeleteJob, 0, 0, false, true, Description = "Periodic misfire deletejob yields deletejob.")]
		[TestCase(1, 0, 1, -1, TriggerScheduleAction.Stop, TriggerScheduleCondition.Misfire, 2, true,
			TriggerScheduleAction.Stop, 0, 0, false, true, Description = "Periodic misfire stop yields stop.")]
		// Periodic execution count limited triggers, non-zero count
		[TestCase(1, 0, 1, 10, TriggerScheduleAction.Skip, TriggerScheduleCondition.Latch, 1, true,
			TriggerScheduleAction.Skip, 1, 10, true, true, Description = "Periodic non-zero execution count latch first time.")]
		[TestCase(1, 0, 1, 10, TriggerScheduleAction.Skip, TriggerScheduleCondition.Latch, 1, false,
			TriggerScheduleAction.Skip, 2, 10, true, false,
			Description = "Periodic non-zero execution count latch second time yields skip.")]
		[TestCase(1, 0, 1, 10, TriggerScheduleAction.Skip, TriggerScheduleCondition.Fire, 1, true,
			TriggerScheduleAction.ExecuteJob, 0, 9, true, true, Description = "Periodic non-zero execution count fire.")]
		[TestCase(1, 0, 1, 10, TriggerScheduleAction.Skip, TriggerScheduleCondition.Misfire, 2, true,
			TriggerScheduleAction.Skip, 3, 10, true, true,
			Description = "Periodic non-zero execution count misfire skip yields skip.")]
		[TestCase(1, 0, 1, 10, TriggerScheduleAction.ExecuteJob, TriggerScheduleCondition.Misfire, 2, true,
			TriggerScheduleAction.ExecuteJob, 0, 9, true, true,
			Description = "Periodic non-zero execution count misfire execute yields execute.")]
		[TestCase(1, 0, 1, 10, TriggerScheduleAction.DeleteJob, TriggerScheduleCondition.Misfire, 2, true,
			TriggerScheduleAction.DeleteJob, 0, 0, false, true,
			Description = "Periodic non-zero execution count misfire deletejob yields deletejob.")]
		[TestCase(1, 0, 1, 10, TriggerScheduleAction.Stop, TriggerScheduleCondition.Misfire, 2, true,
			TriggerScheduleAction.Stop, 0, 0, false, true,
			Description = "Periodic non-zero execution count misfire stop yields stop.")]
		// Periodic execution count limited triggers, zero count
		[TestCase(1, 0, 1, 0, TriggerScheduleAction.Skip, TriggerScheduleCondition.Latch, 1, true,
			TriggerScheduleAction.Stop, 0, 0, false, true,
			Description = "Periodic zero execution count latch first time yields stop.")]
		[TestCase(1, 0, 1, 0, TriggerScheduleAction.Skip, TriggerScheduleCondition.Latch, 1, false,
			TriggerScheduleAction.Stop, 0, 0, false, false,
			Description = "Periodic zero execution count latch second time yields stop.")]
		[TestCase(1, 0, 1, 0, TriggerScheduleAction.Skip, TriggerScheduleCondition.Fire, 1, true,
			TriggerScheduleAction.Stop, 0, 0, false, true, Description = "Periodic zero execution count fire yields stop.")]
		[TestCase(1, 0, 1, 0, TriggerScheduleAction.Skip, TriggerScheduleCondition.Misfire, 2, true,
			TriggerScheduleAction.Stop, 0, 0, false, true,
			Description = "Periodic zero execution count misfire skip yields stop.")]
		[TestCase(1, 0, 1, 0, TriggerScheduleAction.ExecuteJob, TriggerScheduleCondition.Misfire, 2, true,
			TriggerScheduleAction.Stop, 0, 0, false, true,
			Description = "Periodic zero execution count misfire execute yields stop.")]
		[TestCase(1, 0, 1, 0, TriggerScheduleAction.DeleteJob, TriggerScheduleCondition.Misfire, 2, true,
			TriggerScheduleAction.DeleteJob, 0, 0, false, true,
			Description = "Periodic zero execution count misfire deletejob yields deletejob.")]
		[TestCase(1, 0, 1, 0, TriggerScheduleAction.Stop, TriggerScheduleCondition.Misfire, 2, true,
			TriggerScheduleAction.Stop, 0, 0, false, true,
			Description = "Periodic zero execution count misfire stop yields stop.")]
		// Periodic time limited triggers, over time
		[TestCase(1, 3, 1, -1, TriggerScheduleAction.Skip, TriggerScheduleCondition.Latch, 4, true,
			TriggerScheduleAction.Skip, 1, -1, true, true,
			Description = "Periodic over-time latch first time yields skip (because we want to detect the misfire).")]
		[TestCase(1, 3, 1, -1, TriggerScheduleAction.Skip, TriggerScheduleCondition.Latch, 4, false,
			TriggerScheduleAction.Stop, 0, 0, false, false, Description = "Periodic over-time latch second time yields stop.")]
		[TestCase(1, 3, 1, -1, TriggerScheduleAction.Skip, TriggerScheduleCondition.Fire, 4, true,
			TriggerScheduleAction.Stop, 0, 0, false, true, Description = "Periodic over-time fire yields stop.")]
		[TestCase(1, 3, 1, -1, TriggerScheduleAction.Skip, TriggerScheduleCondition.Misfire, 4, true,
			TriggerScheduleAction.Stop, 0, 0, false, true, Description = "Periodic over-time misfire skip yields stop.")]
		[TestCase(1, 3, 1, -1, TriggerScheduleAction.ExecuteJob, TriggerScheduleCondition.Misfire, 4, true,
			TriggerScheduleAction.Stop, 0, 0, false, true, Description = "Periodic over-time misfire execute yields stop.")]
		[TestCase(1, 3, 1, -1, TriggerScheduleAction.DeleteJob, TriggerScheduleCondition.Misfire, 4, true,
			TriggerScheduleAction.DeleteJob, 0, 0, false, true,
			Description = "Periodic over-time misfire deletejob yields deletejob.")]
		[TestCase(1, 3, 1, -1, TriggerScheduleAction.Stop, TriggerScheduleCondition.Misfire, 4, true,
			TriggerScheduleAction.Stop, 0, 0, false, true, Description = "Periodic over-time misfire stop yields stop.")]
		// Periodic time limited triggers, exactly at end time
		[TestCase(1, 3, 1, -1, TriggerScheduleAction.Skip, TriggerScheduleCondition.Latch, 3, true,
			TriggerScheduleAction.Skip, 1, -1, true, true,
			Description = "Periodic at end-time latch first time yields skip (because we want to detect the misfire).")]
		[TestCase(1, 3, 1, -1, TriggerScheduleAction.Skip, TriggerScheduleCondition.Latch, 3, false,
			TriggerScheduleAction.Stop, 0, 0, false, false, Description = "Periodic at end-time latch second time yields stop.")]
		[TestCase(1, 3, 1, -1, TriggerScheduleAction.Skip, TriggerScheduleCondition.Fire, 3, true,
			TriggerScheduleAction.ExecuteJob, 0, -1, true, true,
			Description = "Periodic at end-time fire yields execute (because we're exactly on it).")]
		[TestCase(1, 3, 1, -1, TriggerScheduleAction.Skip, TriggerScheduleCondition.Misfire, 3, true,
			TriggerScheduleAction.Stop, 0, 0, false, true, Description = "Periodic at end-time misfire skip yields stop.")]
		[TestCase(1, 3, 1, -1, TriggerScheduleAction.ExecuteJob, TriggerScheduleCondition.Misfire, 3, true,
			TriggerScheduleAction.ExecuteJob, 0, -1, true, true,
			Description = "Periodic at end-time misfire execute yields stop.")]
		[TestCase(1, 3, 1, -1, TriggerScheduleAction.DeleteJob, TriggerScheduleCondition.Misfire, 3, true,
			TriggerScheduleAction.DeleteJob, 0, 0, false, true,
			Description = "Periodic at end-time misfire deletejob yields deletejob.")]
		[TestCase(1, 3, 1, -1, TriggerScheduleAction.Stop, TriggerScheduleCondition.Misfire, 3, true,
			TriggerScheduleAction.Stop, 0, 0, false, true, Description = "Periodic at end-time misfire stop yields stop.")]
		public void Schedule(int startDay, int endDay, int periodDays, int jobExecutionCount,
		                     TriggerScheduleAction misfireAction,
		                     TriggerScheduleCondition condition, int timeBasisDay, bool isFirstTime,
		                     TriggerScheduleAction expectedAction, int expectedFireDay,
		                     int expectedJobExecutionCountRemaining, bool expectedIsActive, bool expectedIsFirstTime)
		{
			DateTime startTime = new DateTime(1970, 1, startDay);
			DateTime? endTime = endDay != 0 ? new DateTime(1970, 1, endDay) : (DateTime?) null;
			TimeSpan? period = periodDays != 0 ? new TimeSpan(periodDays, 0, 0, 0) : (TimeSpan?) null;
			int? jobExecutionCountArg = jobExecutionCount >= 0 ? jobExecutionCount : (int?) null;
			DateTime timeBasis = new DateTime(1970, 1, timeBasisDay);
			DateTime? expectedFireTime = expectedFireDay != 0
			                             	? new DateTime(1970, 1, expectedFireDay, 0, 0, 0, DateTimeKind.Utc)
			                             	: (DateTime?) null;
			int? expectedJobExecutionCountRemainingArg = expectedJobExecutionCountRemaining >= 0
			                                             	? expectedJobExecutionCountRemaining
			                                             	: (int?) null;

			PeriodicTrigger trigger = new PeriodicTrigger(startTime, endTime, period, jobExecutionCountArg);
			trigger.MisfireAction = misfireAction;
			trigger.MisfireThreshold = TimeSpan.Zero;
			trigger.IsFirstTime = expectedIsFirstTime;

			TriggerScheduleAction action = trigger.Schedule(condition, timeBasis, null);
			Assert.AreEqual(expectedAction, action);
			DateTimeAssert.AreEqualIncludingKind(expectedFireTime, trigger.NextFireTimeUtc);
			Assert.AreEqual(expectedJobExecutionCountRemainingArg, trigger.JobExecutionCountRemaining);
			Assert.AreEqual(expectedIsActive, trigger.IsActive);
		}

		[Test]
		public void SchedulePeriodicTriggerCorrectlyComputesNextCycle()
		{
			// Constructs a 3-day period trigger on an odd date.
			PeriodicTrigger trigger = new PeriodicTrigger(new DateTime(1970, 1, 5), null, new TimeSpan(3, 0, 0, 0), null);
			trigger.MisfireAction = TriggerScheduleAction.Skip;
			trigger.MisfireThreshold = TimeSpan.Zero;

			// Ensure returns next period if skipping with time basis on an even boundary.
			trigger.Schedule(TriggerScheduleCondition.Misfire, new DateTime(1970, 1, 8), null);
			DateTimeAssert.AreEqualIncludingKind(new DateTime(1970, 1, 11, 0, 0, 0, DateTimeKind.Utc), trigger.NextFireTimeUtc);

			// Ensure returns next period if skipping with time basis at 1/3 beyond even boundary.
			trigger.Schedule(TriggerScheduleCondition.Misfire, new DateTime(1970, 1, 9), null);
			DateTimeAssert.AreEqualIncludingKind(new DateTime(1970, 1, 11, 0, 0, 0, DateTimeKind.Utc), trigger.NextFireTimeUtc);

			// Ensure returns next period if skipping with time basis at 2/3 beyond even boundary.
			trigger.Schedule(TriggerScheduleCondition.Misfire, new DateTime(1970, 1, 10), null);
			DateTimeAssert.AreEqualIncludingKind(new DateTime(1970, 1, 11, 0, 0, 0, DateTimeKind.Utc), trigger.NextFireTimeUtc);
		}

		[Test]
		[ExpectedException(typeof (SchedulerException))]
		public void ScheduleThrowsIfConditionIsUnrecognized()
		{
			PeriodicTrigger trigger = new PeriodicTrigger(new DateTime(1970, 1, 5), null, new TimeSpan(3, 0, 0, 0), null);
			trigger.Schedule((TriggerScheduleCondition) 9999, DateTime.UtcNow, null);
		}
	}
}