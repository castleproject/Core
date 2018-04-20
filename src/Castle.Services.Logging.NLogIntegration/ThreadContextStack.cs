// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.Services.Logging.NLogIntegration
{
	using System;

	using Castle.Core.Logging;

	using NLog;

	/// <summary>
	///   Used to access <see cref = "NestedDiagnosticsContext" />
	/// </summary>
	public class ThreadContextStack : IContextStack
	{
		/// <summary>
		///   Not implemented.
		/// </summary>
		/// <exception cref = "NotImplementedException" />
		public int Count
		{
			get { return NestedDiagnosticsContext.GetAllObjects().Length; }
		}

		/// <summary>
		///   Clears current thread NDC stack.
		/// </summary>
		public void Clear()
		{
			NestedDiagnosticsContext.Clear();
		}

		/// <summary>
		///   Pops the top message off the NDC stack.
		/// </summary>
		/// <returns>The top message which is no longer on the stack.</returns>
		public string Pop()
		{
			return NestedDiagnosticsContext.Pop();
		}

		/// <summary>
		///   Pushes the specified text on current thread NDC.
		/// </summary>
		/// <param name = "message">The message to be pushed.</param>
		/// <returns>An instance of the object that implements IDisposable that returns the stack to the previous level when IDisposable.Dispose() is called. To be used with C# using() statement.</returns>
		public IDisposable Push(string message)
		{
			return NestedDiagnosticsContext.Push(message);
		}
	}
}