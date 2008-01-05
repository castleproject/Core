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

	/// <summary>
	/// Represents the trace that ASP.Net exposes
	/// </summary>
	public interface ITrace
	{
		/// <summary>
		/// Logs the specified message on the ASP.Net trace
		/// </summary>
		/// <param name="message">The message.</param>
		void Warn(string message);

		/// <summary>
		/// Logs the specified message on the ASP.Net trace
		/// </summary>
		/// <param name="category">The category.</param>
		/// <param name="message">The message.</param>
		void Warn(string category, string message);

		/// <summary>
		/// Logs the specified message on the ASP.Net trace
		/// </summary>
		/// <param name="category">The category.</param>
		/// <param name="message">The message.</param>
		/// <param name="errorInfo">The error info.</param>
		void Warn(string category, string message, Exception errorInfo);

		/// <summary>
		/// Logs the specified message on the ASP.Net trace
		/// </summary>
		/// <param name="message">The message.</param>
		void Write(string message);

		/// <summary>
		/// Logs the specified message on the ASP.Net trace
		/// </summary>
		/// <param name="category">The category.</param>
		/// <param name="message">The message.</param>
		void Write(string category, string message);

		/// <summary>
		/// Logs the specified message on the ASP.Net trace
		/// </summary>
		/// <param name="category">The category.</param>
		/// <param name="message">The message.</param>
		/// <param name="errorInfo">The error info.</param>
		void Write(string category, string message, Exception errorInfo);
	}
}
