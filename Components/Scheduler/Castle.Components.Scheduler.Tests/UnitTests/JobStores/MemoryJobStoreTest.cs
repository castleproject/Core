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

namespace Castle.Components.Scheduler.Tests.UnitTests.JobStores
{
	using System;
	using NUnit.Framework;
	using Scheduler.JobStores;

	[TestFixture]
	public class MemoryJobStoreTest : BaseJobStoreTest
	{
		protected override BaseJobStore CreateJobStore()
		{
			return new MemoryJobStore();
		}

		[Test]
		public void UpdateJob_IncrementsVersionNumber()
		{
			Mocks.ReplayAll();

			VersionedJobDetails jobDetails = (VersionedJobDetails) CreatePendingJob("job", DateTime.UtcNow);
			int originalVersion = jobDetails.Version;

			jobDetails.JobSpec.Name = "renamedJob";
			JobStore.UpdateJob("job", jobDetails.JobSpec);

			VersionedJobDetails updatedJobDetails = (VersionedJobDetails) JobStore.GetJobDetails("renamedJob");
			Assert.AreEqual(originalVersion + 1, updatedJobDetails.Version,
			                "Version number of saved object should be incremented in database.");
		}

		[Test]
		public void SaveJobDetails_IncrementsVersionNumber()
		{
			Mocks.ReplayAll();

			VersionedJobDetails jobDetails = (VersionedJobDetails) CreatePendingJob("job", DateTime.UtcNow);
			int originalVersion = jobDetails.Version;

			jobDetails.JobState = JobState.Stopped;
			JobStore.SaveJobDetails(jobDetails);

			Assert.AreEqual(originalVersion + 1, jobDetails.Version,
			                "Version number of original object should be incremented in place.");

			VersionedJobDetails updatedJobDetails = (VersionedJobDetails) JobStore.GetJobDetails("job");
			Assert.AreEqual(originalVersion + 1, updatedJobDetails.Version,
			                "Version number of saved object should be incremented in memory too.");
		}
	}
}