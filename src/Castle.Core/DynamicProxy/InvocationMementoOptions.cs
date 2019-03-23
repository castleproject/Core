// Copyright 2004-2019 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy
{
	using System;

	/// <summary>
	///   Specifies which parts of an invocation's state to capture.
	/// </summary>
	[Flags]
	public enum InvocationMementoOptions
	{
		/// <summary>
		///   Specifies that arguments should be captured.
		/// </summary>
		Arguments = 1,

		/// <summary>
		///   Specifies that the return value should be captured.
		/// </summary>
		ReturnValue = 2,

		/// <summary>
		///   Specifies that the invocation's position in the interception pipeline should be captured.
		///   <para>
		///     Note that interception of an invocation may have finished by the time a call to
		///     <see cref="IInvocationMemento.Restore"/> is made. While such a "stale" invocation
		///     can be procesed again by interceptors, setting the return value or by-ref arguments
		///     will no longer have any effect on the calling code.
		///   </para>
		/// </summary>
		InterceptionProgress = 4,
	}
}
