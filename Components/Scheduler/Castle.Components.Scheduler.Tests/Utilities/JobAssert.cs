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

namespace Castle.Components.Scheduler.Tests.Utilities
{
	using System;
	using NUnit.Framework;

	/// <summary>
	/// Assertions for job objects.
	/// </summary>
	public static class JobAssert
	{
		/// <summary>
		/// Determines if two dates are equal while compensating for inaccuracies in date representation.
		/// </summary>
		/// <param name="expected"></param>
		/// <param name="actual"></param>
		public static void AreEqualUpToErrorLimit(DateTime expected, DateTime actual)
		{
			actual = actual.ToUniversalTime();
			expected = expected.ToUniversalTime();
			Assert.That(actual, Is.GreaterThanOrEqualTo(expected.AddMilliseconds(-500)).And.LessThanOrEqualTo(expected.AddMilliseconds(500)));
		}

		public static void AreEqualUpToErrorLimit(DateTime? expected, DateTime? actual)
		{
			Assert.AreEqual(expected.HasValue, actual.HasValue, "Are both null or non-null?");
			if (expected.HasValue)
				AreEqualUpToErrorLimit(expected.Value, actual.Value);
		}

		public static void AreEqual(Trigger expected, Trigger actual)
		{
			if (expected == null)
			{
				Assert.IsNull(actual);
				return;
			}

			Assert.IsNotNull(actual);
			Assert.IsInstanceOf(expected.GetType(), actual);
			Assert.AreEqual(expected.IsActive, actual.IsActive);
			AreEqualUpToErrorLimit(expected.NextFireTimeUtc, actual.NextFireTimeUtc);
			Assert.AreEqual(expected.NextMisfireThreshold, actual.NextMisfireThreshold);
		}

		public static void AreEqual(JobExecutionDetails expected, JobExecutionDetails actual)
		{
			if (expected == null)
			{
				Assert.IsNull(actual);
				return;
			}

			Assert.IsNotNull(actual);
			Assert.AreEqual(expected.SchedulerGuid, actual.SchedulerGuid);
			AreEqualUpToErrorLimit(expected.StartTimeUtc, actual.StartTimeUtc);
			AreEqualUpToErrorLimit(expected.EndTimeUtc, actual.EndTimeUtc);
			Assert.AreEqual(expected.StatusMessage, actual.StatusMessage);
			Assert.AreEqual(expected.Succeeded, actual.Succeeded);
		}

		public static void AreEqual(JobData expected, JobData actual)
		{
			if (expected == null)
			{
				Assert.IsNull(actual);
				return;
			}

			Assert.IsNotNull(actual);
			CollectionAssert.AreEqual(expected.State, actual.State);
		}

		public static void AreEqual(JobSpec expected, JobSpec actual)
		{
			if (expected == null)
			{
				Assert.IsNull(actual);
				return;
			}

			Assert.IsNotNull(actual);
			Assert.AreEqual(expected.Name, actual.Name);
			Assert.AreEqual(expected.Description, actual.Description);
			Assert.AreEqual(expected.JobKey, actual.JobKey);
			AreEqual(expected.Trigger, actual.Trigger);
			AreEqual(expected.JobData, actual.JobData);
		}

		public static void AreEqual(JobDetails expected, JobDetails actual)
		{
			if (expected == null)
			{
				Assert.IsNull(actual);
				return;
			}

			Assert.IsNotNull(actual);
			Assert.AreEqual(expected.CreationTimeUtc, actual.CreationTimeUtc);
			AreEqual(expected.JobSpec, actual.JobSpec);
			Assert.AreEqual(expected.JobState, actual.JobState);
			AreEqualUpToErrorLimit(expected.NextTriggerFireTimeUtc, actual.NextTriggerFireTimeUtc);
			Assert.AreEqual(expected.NextTriggerMisfireThreshold, actual.NextTriggerMisfireThreshold);
			AreEqual(expected.LastJobExecutionDetails, actual.LastJobExecutionDetails);
		}
	}
}