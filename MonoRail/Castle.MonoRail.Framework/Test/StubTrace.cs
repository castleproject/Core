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

namespace Castle.MonoRail.Framework.Test
{
	using System;

	/// <summary>
	/// Represents a mock implementation of <see cref="ITrace"/> for unit test purposes.
	/// </summary>
	public class StubTrace : ITrace
	{
		/// <summary>
		/// Logs the specified message on the ASP.Net trace
		/// </summary>
		/// <param name="message">The message.</param>
		public virtual void Warn(string message)
		{
		}

		/// <summary>
		/// Logs the specified message on the ASP.Net trace
		/// </summary>
		/// <param name="category">The category.</param>
		/// <param name="message">The message.</param>
		public virtual void Warn(string category, string message)
		{
		}

		/// <summary>
		/// Logs the specified message on the ASP.Net trace
		/// </summary>
		/// <param name="category">The category.</param>
		/// <param name="message">The message.</param>
		/// <param name="errorInfo">The error info.</param>
		public virtual void Warn(string category, string message, Exception errorInfo)
		{
		}

		/// <summary>
		/// Logs the specified message on the ASP.Net trace
		/// </summary>
		/// <param name="message">The message.</param>
		public virtual void Write(string message)
		{
		}

		/// <summary>
		/// Logs the specified message on the ASP.Net trace
		/// </summary>
		/// <param name="category">The category.</param>
		/// <param name="message">The message.</param>
		public virtual void Write(string category, string message)
		{
		}

		/// <summary>
		/// Logs the specified message on the ASP.Net trace
		/// </summary>
		/// <param name="category">The category.</param>
		/// <param name="message">The message.</param>
		/// <param name="errorInfo">The error info.</param>
		public virtual void Write(string category, string message, Exception errorInfo)
		{
		}
	}
}