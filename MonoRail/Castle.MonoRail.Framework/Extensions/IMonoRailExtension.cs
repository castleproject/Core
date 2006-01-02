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

namespace Castle.MonoRail.Framework
{
	using System;

	using Castle.MonoRail.Framework.Configuration;

	/// <summary>
	/// Contract for extensions that want to hook 
	/// on MonoRail's events
	/// </summary>
	/// <remarks>
	/// Extensions implementations must be thread safe and stateless.
	/// </remarks>
	public interface IMonoRailExtension
	{
		/// <summary>
		/// Implementors have a chance to read custom 
		/// elements from <see cref="MonoRailConfiguration"/>
		/// </summary>
		/// <param name="configuration"></param>
		void Init(MonoRailConfiguration configuration);

		/// <summary>
		/// Invoked when an instance of the implementation 
		/// of <see cref="IRailsEngineContext"/> is created
		/// </summary>
		/// <remarks>
		/// The extension instance is shared and should not 
		/// hold a reference to the context
		/// </remarks>
		/// <param name="context"></param>
		void OnRailsContextCreated(IRailsEngineContext context, IServiceProvider serviceProvider);

		/// <summary>
		/// Invoked when an instance of the implementation 
		/// of <see cref="IRailsEngineContext"/> is about
		/// to be released.
		/// </summary>
		/// <remarks>
		/// The extension instance is shared and should not 
		/// hold a reference to the context
		/// </remarks>
		/// <param name="context"></param>
		void OnRailsContextDiscarded(IRailsEngineContext context, IServiceProvider serviceProvider);

		void OnActionException(IRailsEngineContext context, IServiceProvider serviceProvider);
	}
}
