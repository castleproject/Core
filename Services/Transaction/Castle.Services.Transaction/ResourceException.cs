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

namespace Castle.Services.Transaction
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class ResourceException : TransactionException
	{
		private IResource lastFailedResource = null;
		private IResource[] failedResources = null;


		public ResourceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public ResourceException(string message, IResource lastFailedResource, IResource[] failedResources) : base(message)
		{
			this.lastFailedResource = lastFailedResource;
			this.failedResources = failedResources;
		}

		public ResourceException(string message, Exception innerException, IResource lastFailedResource,
		                         IResource[] failedResources) : base(message, innerException)
		{
			this.lastFailedResource = lastFailedResource;
			this.failedResources = failedResources;
		}

		public ResourceException(SerializationInfo info, StreamingContext context, IResource lastFailedResource,
		                         IResource[] failedResources) : base(info, context)
		{
			this.lastFailedResource = lastFailedResource;
			this.failedResources = failedResources;
		}

		public IResource LastFailedResource
		{
			get { return lastFailedResource; }
		}

		public IResource[] FailedResources
		{
			get { return failedResources; }
		}
	}
}