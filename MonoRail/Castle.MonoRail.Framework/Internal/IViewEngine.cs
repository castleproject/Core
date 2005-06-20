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

namespace Castle.MonoRail.Framework
{
	using System;

	/// <summary>
	/// Depicts the contract used by the engine
	/// to process views, in an independent manner.
	/// </summary>
	public interface IViewEngine
	{
		/// <summary>
		/// Initializes the View Engine.
		/// </summary>
		void Init();

		/// <summary>
		/// The root directory of views, obtained
		/// from the configuration.
		/// </summary>
		String ViewRootDir { get; set; }
		
		/// <summary>
		/// Implementors should process the view (using the viewName to
		/// obtain the correct template) and use the context to output
		/// the result.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="controller"></param>
		/// <param name="viewName"></param>
		void Process(IRailsEngineContext context, Controller controller, String viewName);
	}
}