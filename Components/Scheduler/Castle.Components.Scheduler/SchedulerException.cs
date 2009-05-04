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
	using System.Runtime.Serialization;

	/// <summary>
	/// The type of exception used by the scheduler to report problems.
	/// </summary>
	[Serializable]
	public class SchedulerException : Exception
	{
		/// <summary>
		/// Creates a scheduler exception.
		/// </summary>
		/// <param name="message">The exception message</param>
		public SchedulerException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Creates a scheduler exception.
		/// </summary>
		/// <param name="message">The exception message</param>
		/// <param name="innerException">The inner exception</param>
		public SchedulerException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Creates a scheduler exception from its serialized form.
		/// </summary>
		/// <param name="info">The serialization info</param>
		/// <param name="context">The streaming context</param>
		protected SchedulerException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}