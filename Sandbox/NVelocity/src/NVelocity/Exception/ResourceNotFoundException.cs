// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace NVelocity.Exception
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>  
	/// Application-level exception thrown when a resource of any type
	/// isn't found by the Velocity engine.
	/// <br>
	/// When this exception is thrown, a best effort will be made to have
	/// useful information in the exception's message.  For complete
	/// information, consult the runtime log.
	/// </summary>
	[Serializable]
	public class ResourceNotFoundException : VelocityException
	{
		public ResourceNotFoundException(String exceptionMessage) : base(exceptionMessage)
		{
		}

		public ResourceNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public ResourceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}