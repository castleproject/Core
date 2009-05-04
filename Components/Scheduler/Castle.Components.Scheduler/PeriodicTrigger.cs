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
	using Utilities;

	/// <summary>
	/// Encapsulates an algorithm for generating regular periodic triggers
	/// relative to some fixed start time.  The trigger will fire repeatedly
	/// every recurrence period until either the remainind number of recurrences
	/// drops to zero, the end time is reached, or the associated job is deleted.
	/// </summary>
	[Serializable]
	public class PeriodicTrigger : Trigger
	{
		/// <summary>
		/// The default misfire action.
		/// </summary>
		public const TriggerScheduleAction DefaultMisfireAction = TriggerScheduleAction.Skip;

		private DateTime startTimeUtc;
		private DateTime? endTimeUtc;
		private TimeSpan? period;
		private int? jobExecutionCountRemaining;
		private bool isFirstTime;

		private TimeSpan? misfireThreshold;
		private TriggerScheduleAction misfireAction;

		private DateTime? nextFireTimeUtc;

		/// <summary>
		/// Creates a periodic trigger.
		/// </summary>
		/// <param name="startTimeUtc">The UTC date and time when the trigger will first fire</param>
		/// <param name="endTimeUtc">The UTC date and time when the trigger must stop firing.
		/// If the time is set to null, the trigger may continue firing indefinitely.</param>
		/// <param name="period">The recurrence period of the trigger.
		/// If the period is set to null, the trigger will fire exactly once
		/// and never recur.</param>
		/// <param name="jobExecutionCount">The number of job executions remaining before the trigger
		/// stops firing.  This number is decremented each time the job executes
		/// until it reaches zero.  If the count is set to null, the number of times the job
		/// may execute is unlimited.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="period"/> is negative or zero</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="jobExecutionCount"/> is negative</exception>
		public PeriodicTrigger(DateTime startTimeUtc, DateTime? endTimeUtc, TimeSpan? period, int? jobExecutionCount)
		{
			if (period.HasValue && period.Value.Ticks <= 0)
				throw new ArgumentOutOfRangeException("period", "The recurrence period must not be negative or zero.");
			if (jobExecutionCount.HasValue && jobExecutionCount.Value < 0)
				throw new ArgumentOutOfRangeException("jobExecutionCount", "The job execution count remaining must not be negative.");

			this.startTimeUtc = DateTimeUtils.AssumeUniversalTime(startTimeUtc);
			this.endTimeUtc = DateTimeUtils.AssumeUniversalTime(endTimeUtc);
			this.period = period;
			jobExecutionCountRemaining = jobExecutionCount;
			isFirstTime = true;

			misfireAction = DefaultMisfireAction;
		}

		/// <summary>
		/// Creates a trigger that fires exactly once at the specified time.
		/// </summary>
		/// <param name="fireTimeUtc">The UTC time at which the trigger should fire</param>
		/// <returns>The one-shot trigger</returns>
		public static PeriodicTrigger CreateOneShotTrigger(DateTime fireTimeUtc)
		{
			return new PeriodicTrigger(fireTimeUtc, null, null, 1);
		}


		/// <summary>
		/// Creates a trigger that fires every 24 hours beginning at the specified start time.
		/// </summary>
		/// <remarks>
		/// This method does not take into account local time variations such as Daylight
		/// Saving Time.  Use a more sophisticated calendar-based trigger for that purpose.
		/// </remarks>
		/// <param name="startTimeUtc">The UTC date and time when the trigger will first fire</param>
		public static PeriodicTrigger CreateDailyTrigger(DateTime startTimeUtc)
		{
			return new PeriodicTrigger(startTimeUtc, null, new TimeSpan(24, 0, 0), null);
		}

		/// <summary>
		/// Gets or sets the UTC date and time when the trigger will first fire.
		/// </summary>
		public DateTime StartTimeUtc
		{
			get { return startTimeUtc; }
			set { startTimeUtc = DateTimeUtils.AssumeUniversalTime(value); }
		}

		/// <summary>
		/// Gets or sets the UTC date and time when the trigger must stop firing.
		/// If the time is set to null, the trigger may continue firing indefinitely.
		/// </summary>
		public DateTime? EndTimeUtc
		{
			get { return endTimeUtc; }
			set { endTimeUtc = DateTimeUtils.AssumeUniversalTime(value); }
		}

		/// <summary>
		/// Gets or sets the recurrence period of the trigger.
		/// If the period is set to null, the trigger will fire exactly once
		/// and never recur.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is negative or zero</exception>
		public TimeSpan? Period
		{
			get { return period; }
			set
			{
				if (value.HasValue && value.Value.Ticks <= 0)
					throw new ArgumentOutOfRangeException("value", "The recurrence period must not be negative or zero.");

				period = value;
			}
		}


		/// <summary>
		/// Gets or sets the number of job executions remaining before the trigger
		/// stops firing.  This number is decremented each time the job executes
		/// until it reaches zero.  If the count is set to null, the number of times
		/// the job may execute is unlimited.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is negative</exception>
		public int? JobExecutionCountRemaining
		{
			get { return jobExecutionCountRemaining; }
			set
			{
				if (value.HasValue && value.Value < 0)
					throw new ArgumentOutOfRangeException("value", "The job execution count remaining must not be negative.");

				jobExecutionCountRemaining = value;
			}
		}

		/// <summary>
		/// Gets or sets the amount of time by which the scheduler is permitted to miss
		/// the next scheduled time before a misfire occurs or null if the trigger never misfires.
		/// </summary>
		/// <remarks>
		/// The default is null.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is negative</exception>
		public TimeSpan? MisfireThreshold
		{
			get { return misfireThreshold; }
			set
			{
				if (value.HasValue && value.Value.Ticks < 0)
					throw new ArgumentOutOfRangeException("value", "The misfire threshold must not be negative.");

				misfireThreshold = value;
			}
		}

		/// <summary>
		/// Gets or sets the action to perform when the trigger misses a scheduled recurrence.
		/// </summary>
		/// <remarks>
		/// The default is <see cref="TriggerScheduleAction.Skip"/>.
		/// </remarks>
		public TriggerScheduleAction MisfireAction
		{
			get { return misfireAction; }
			set { misfireAction = value; }
		}

		/// <summary>
		/// Gets or sets whether the next trigger firing should occur at <see cref="StartTimeUtc" />
		/// or at the next recurrence period according to <see cref="Period" />.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This property is initially set to true when the trigger is created so that missing
		/// the time specified by <see cref="StartTimeUtc" /> is considered a misfire.
		/// Once the first time is processed, the property is set to false and the trigger will
		/// skip over as many recurrence periods as needed to catch up with real time.
		/// </para>
		/// <para>
		/// It may be useful to initialize this property to false after creating the trigger
		/// if you do not care whether the trigger fires at the designated start time and
		/// simply wish to ensure that it followed the expected recurrence pattern.  However,
		/// if the <see cref="Period" /> is null, then the trigger will not fire if the time
		/// indicated by <see cref="StartTimeUtc" /> has already passed and will immediately
		/// become inactive.
		/// </para>
		/// </remarks>
		public bool IsFirstTime
		{
			get { return isFirstTime; }
			set { isFirstTime = value; }
		}

		/// <inheritdoc />
		public override DateTime? NextFireTimeUtc
		{
			get { return nextFireTimeUtc; }
		}

		/// <inheritdoc />
		public override TimeSpan? NextMisfireThreshold
		{
			get { return misfireThreshold; }
		}

		/// <inheritdoc />
		public override bool IsActive
		{
			get { return ! jobExecutionCountRemaining.HasValue || jobExecutionCountRemaining.Value > 0; }
		}

		/// <inheritdoc />
		public override Trigger Clone()
		{
			PeriodicTrigger clone = new PeriodicTrigger(startTimeUtc, endTimeUtc, period, jobExecutionCountRemaining);
			clone.nextFireTimeUtc = nextFireTimeUtc;
			clone.misfireThreshold = misfireThreshold;
			clone.misfireAction = misfireAction;
			clone.isFirstTime = isFirstTime;

			return clone;
		}

		/// <inheritdoc />
		public override TriggerScheduleAction Schedule(TriggerScheduleCondition condition, DateTime timeBasisUtc,
		                                               JobExecutionDetails lastJobExecutionDetails)
		{
			timeBasisUtc = DateTimeUtils.AssumeUniversalTime(timeBasisUtc);

			switch (condition)
			{
				case TriggerScheduleCondition.Latch:
					return ScheduleSuggestedAction(TriggerScheduleAction.Skip, timeBasisUtc);

				case TriggerScheduleCondition.Misfire:
					isFirstTime = false;
					return ScheduleSuggestedAction(misfireAction, timeBasisUtc);

				case TriggerScheduleCondition.Fire:
					isFirstTime = false;
					return ScheduleSuggestedAction(TriggerScheduleAction.ExecuteJob, timeBasisUtc);

				default:
					throw new SchedulerException(String.Format(CultureInfo.CurrentCulture,
					                                           "Unrecognized trigger schedule condition '{0}'.", condition));
			}
		}

		private TriggerScheduleAction ScheduleSuggestedAction(TriggerScheduleAction action, DateTime timeBasisUtc)
		{
			switch (action)
			{
				case TriggerScheduleAction.Stop:
					break;

				case TriggerScheduleAction.ExecuteJob:
					// If the job cannot execute again then stop.
					if (jobExecutionCountRemaining.HasValue && jobExecutionCountRemaining.Value <= 0)
						break;

					// If the end time has passed then stop.
					if (endTimeUtc.HasValue && timeBasisUtc > endTimeUtc.Value)
						break;

					// If the start time is still in the future then hold off until then.
					if (timeBasisUtc < startTimeUtc)
					{
						nextFireTimeUtc = startTimeUtc;
						return TriggerScheduleAction.Skip;
					}

					// Otherwise execute the job.
					nextFireTimeUtc = null;
					jobExecutionCountRemaining -= 1;
					return TriggerScheduleAction.ExecuteJob;

				case TriggerScheduleAction.DeleteJob:
					nextFireTimeUtc = null;
					jobExecutionCountRemaining = 0;
					return TriggerScheduleAction.DeleteJob;

				case TriggerScheduleAction.Skip:
					// If the job cannot execute again then stop.
					if (jobExecutionCountRemaining.HasValue && jobExecutionCountRemaining.Value <= 0)
						break;

					// If the start time is still in the future then hold off until then.
					if (isFirstTime || timeBasisUtc < startTimeUtc)
					{
						nextFireTimeUtc = startTimeUtc;
						return TriggerScheduleAction.Skip;
					}

					// If the trigger is not periodic then we must be skipping the only chance the
					// job had to run so stop the trigger.
					if (! period.HasValue)
						break;

					// Compute when the next occurrence should be.
					TimeSpan timeSinceStart = timeBasisUtc - startTimeUtc;
					TimeSpan timeSinceLastPeriod = new TimeSpan(timeSinceStart.Ticks%period.Value.Ticks);
					nextFireTimeUtc = timeBasisUtc + period - timeSinceLastPeriod;

					// If the next occurrence is past the end time then stop.
					if (nextFireTimeUtc > endTimeUtc)
						break;

					// Otherwise we're good.
					return TriggerScheduleAction.Skip;
			}

			// Stop the trigger.
			nextFireTimeUtc = null;
			jobExecutionCountRemaining = 0;
			return TriggerScheduleAction.Stop;
		}
	}
}