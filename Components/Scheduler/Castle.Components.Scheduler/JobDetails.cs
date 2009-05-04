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
	using Utilities;

	/// <summary>
	/// Provides details about a job including its current status.
	/// </summary>
	/// <remarks>
	/// <para>
	/// An implementation of <see cref="IScheduler" /> may provide specialized
	/// subclasses of <see cref="JobDetails" /> to clients that can use additional
	/// scheduler-specific job details.  However, clients should
	/// be prepared to handle the <see cref="JobDetails" /> base class as a least common
	/// denominator albeit some of the scheduler's advanced capabilities may thus be unavailable.
	/// </para>
	/// <para>
	/// The <see cref="JobDetails" /> object returned to client code should
	/// always be a clone of the master copy, if applicable.
	/// </para>
	/// </remarks>
	[Serializable]
	public class JobDetails : ICloneable<JobDetails>
	{
		private JobSpec jobSpec;
		private DateTime creationTimeUtc;

		private JobState jobState;
		private DateTime? nextTriggerFireTimeUtc;
		private TimeSpan? nextTriggerMisfireThreshold;
		private JobExecutionDetails lastJobExecutionDetails;

		/// <summary>
		/// Creates job details for a newly created job.
		/// </summary>
		/// <param name="jobSpec">The job's specification</param>
		/// <param name="creationTimeUtc">The UTC time when the job was created</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="jobSpec"/> is null</exception>
		public JobDetails(JobSpec jobSpec, DateTime creationTimeUtc)
		{
			if (jobSpec == null)
				throw new ArgumentNullException("jobSpec");

			this.jobSpec = jobSpec;
			this.creationTimeUtc = DateTimeUtils.AssumeUniversalTime(creationTimeUtc);
		}

		/// <summary>
		/// Gets or sets the job's specification.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
		public JobSpec JobSpec
		{
			get { return jobSpec; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");
				jobSpec = value;
			}
		}

		/// <summary>
		/// Gets or sets the UTC time when the job was created.
		/// </summary>
		public DateTime CreationTimeUtc
		{
			get { return creationTimeUtc; }
			set { creationTimeUtc = DateTimeUtils.AssumeUniversalTime(value); }
		}

		/// <summary>
		/// Gets or sets the current state of the job.
		/// </summary>
		/// <remarks>
		/// Initially <see cref="Scheduler.JobState.Pending" />.
		/// </remarks>
		public JobState JobState
		{
			get { return jobState; }
			set { jobState = value; }
		}

		/// <summary>
		/// Gets or sets the UTC time when the trigger is next scheduled to fire or null if the
		/// trigger is not scheduled to fire again based on a time signal.
		/// </summary>
		/// <remarks>
		/// Initially null.
		/// </remarks>
		public DateTime? NextTriggerFireTimeUtc
		{
			get { return nextTriggerFireTimeUtc; }
			set { nextTriggerFireTimeUtc = DateTimeUtils.AssumeUniversalTime(value); }
		}

		/// <summary>
		/// Gets or sets the amount of time by which the trigger is permitted to miss the next
		/// scheduled time before a misfire occurs or null to consider the schedule on
		/// time no matter how late it fires.
		/// </summary>
		/// <remarks>
		/// Initially null.
		/// </remarks>
		public TimeSpan? NextTriggerMisfireThreshold
		{
			get { return nextTriggerMisfireThreshold; }
			set { nextTriggerMisfireThreshold = value; }
		}

		/// <summary>
		/// Gets or sets the execution details for the most recent (possibly in-progress)
		/// job execution or null if the job has never been executed.
		/// </summary>
		public JobExecutionDetails LastJobExecutionDetails
		{
			get { return lastJobExecutionDetails; }
			set { lastJobExecutionDetails = value; }
		}

		/// <summary>
		/// Clones the job details including a deep copy of all properties.
		/// </summary>
		/// <returns>The cloned job details</returns>
		public virtual JobDetails Clone()
		{
			JobDetails clone = new JobDetails(jobSpec.Clone(), creationTimeUtc);
			CopyTo(clone);
			return clone;
		}

		/// <summary>
		/// Copies the properties of the job details to the specified target object.
		/// </summary>
		/// <remarks>
		/// This method may be used to simplify implementing <see cref="Clone" /> in subclasses.
		/// </remarks>
		/// <param name="target">The target</param>
		protected virtual void CopyTo(JobDetails target)
		{
			target.JobSpec = jobSpec.Clone();
			target.jobState = jobState;
			target.nextTriggerFireTimeUtc = nextTriggerFireTimeUtc;
			target.nextTriggerMisfireThreshold = nextTriggerMisfireThreshold;

			if (lastJobExecutionDetails != null)
				target.lastJobExecutionDetails = lastJobExecutionDetails.Clone();
			else
				target.lastJobExecutionDetails = null;
		}

		object ICloneable.Clone()
		{
			return Clone();
		}
	}
}