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

namespace Castle.MonoRail.Framework.Extensions.ExceptionChaining
{
	using System;

	/// <summary>
	/// Represents a processor of an exception.
	/// The processor might choose to register the exception
	/// in some specific way and then delegate the execution
	/// to the next handler
	/// <seealso cref="IConfigurableHandler"/>
	/// </summary>
	public interface IExceptionHandler
	{
		/// <summary>
		/// Implementors should perform any required
		/// initialization
		/// </summary>
		void Initialize();

		/// <summary>
		/// Implementors should perform the action 
		/// on the exception. Note that the exception 
		/// is available in <see cref="IRailsEngineContext.LastException"/>
		/// </summary>
		/// <param name="context"></param>
		void Process(IRailsEngineContext context, IServiceProvider serviceProvider);

		/// <summary>
		/// The next exception in the sink 
		/// or null if none exists.
		/// </summary>
		IExceptionHandler Next { get;set; }
	}
}
