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

	/// <summary>
	/// The versioned job details subclass maintains a version token
	/// along with job details information for use in determining whether
	/// the job details have been concurrently modified.
	/// </summary>
	[Serializable]
	public class VersionedJobDetails : JobDetails
	{
		private int version;

		/// <summary>
		/// Creates job details for a newly created job.
		/// </summary>
		/// <param name="jobSpec">The job's specification</param>
		/// <param name="creationTime">The time when the job was created</param>
		/// <param name="version">The version number</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="jobSpec"/> is null</exception>
		public VersionedJobDetails(JobSpec jobSpec, DateTime creationTime, int version)
			: base(jobSpec, creationTime)
		{
			this.version = version;
		}

		/// <summary>
		/// Gets or sets the version number.
		/// </summary>
		public int Version
		{
			get { return version; }
			set { version = value; }
		}

		/// <inheritdoc />
		public override JobDetails Clone()
		{
			VersionedJobDetails clone = new VersionedJobDetails(JobSpec.Clone(), CreationTimeUtc, version);
			CopyTo(clone);
			return clone;
		}
	}
}