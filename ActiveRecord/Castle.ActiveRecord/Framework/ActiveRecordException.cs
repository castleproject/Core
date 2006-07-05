// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Framework
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class ActiveRecordException : Exception
	{
		public ActiveRecordException(string message) : base(message)
		{
		}

		public ActiveRecordException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected ActiveRecordException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
	
	/// <summary>
	/// Exception thrown when an error is detected on the ActiveRecord initialization phase.
	/// </summary>
	[Serializable]
	public class ActiveRecordInitializationException : ActiveRecordException
	{
		public ActiveRecordInitializationException(string message) : base(message)
		{
		}

		public ActiveRecordInitializationException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected ActiveRecordInitializationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
