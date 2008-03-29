// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Threading;

	/// <summary>
	/// This is used to mark that an operation has failed during the begin phase
	/// and that the end method of the async action should not be called.
	/// </summary>
	public class FailedToExecuteBeginActionAsyncResult : IAsyncResult
	{
		/// <summary>
		/// Gets an indication whether the asynchronous operation has completed.
		/// </summary>
		/// <value></value>
		/// <returns>true if the operation is complete; otherwise, false.</returns>
		public bool IsCompleted
		{
			get { return true; }
		}

		/// <summary>
		/// Gets a <see cref="T:System.Threading.WaitHandle"/> that is used to wait for an asynchronous operation to complete.
		/// </summary>
		/// <value></value>
		/// <returns>A <see cref="T:System.Threading.WaitHandle"/> that is used to wait for an asynchronous operation to complete.</returns>
		public WaitHandle AsyncWaitHandle
		{
			get { return null; }
		}

		/// <summary>
		/// Gets a user-defined object that qualifies or contains information about an asynchronous operation.
		/// </summary>
		/// <value></value>
		/// <returns>A user-defined object that qualifies or contains information about an asynchronous operation.</returns>
		public object AsyncState
		{
			get { return null; }
		}

		/// <summary>
		/// Gets an indication of whether the asynchronous operation completed synchronously.
		/// </summary>
		/// <value></value>
		/// <returns>true if the asynchronous operation completed synchronously; otherwise, false.</returns>
		public bool CompletedSynchronously
		{
			get { return true; }
		}
	}
}