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

namespace Castle.Components.Scheduler.WindsorExtension
{
	using System;
	using System.Globalization;
	using Core;
	using MicroKernel;

	/// <summary>
	/// The Windsor job factory constructs instances of <see cref="IJob" /> by
	/// asking the Castle MicroKernel to resolve a component that implements the 
	/// <see cref="IJob" /> service and whose component key equals the requested job key.
	/// </summary>
	[Singleton]
	public class WindsorJobFactory : IJobFactory
	{
		private readonly IKernel kernel;

		/// <summary>
		/// Creates a component job factory based on the specified IoC kernel.
		/// </summary>
		/// <param name="kernel">The IoC kernel to use for component resolution</param>
		public WindsorJobFactory(IKernel kernel)
		{
			this.kernel = kernel;
		}

		/// <inheritdoc />
		public IJob GetJob(string jobKey)
		{
			try
			{
				return (IJob) kernel.Resolve(jobKey, typeof (IJob));
			}
			catch (Exception ex)
			{
				throw new SchedulerException(String.Format(CultureInfo.CurrentCulture,
				                                           "Cannot create IJob component with key '{0}'.", jobKey), ex);
			}
		}

		/// <inheritdoc />
		public void ReleaseJob(IJob job)
		{
			kernel.ReleaseComponent(job);
		}
	}
}