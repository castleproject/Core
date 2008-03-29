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
	using Test;

	/// <summary>
	/// Pendent
	/// </summary>
	public delegate void ControllerHandler(IExecutableAction action, IEngineContext engineContext,
	                                       IController controller, IControllerContext controllerContext);

	/// <summary>
	/// Represent the core functionality required out of a controller
	/// </summary>
	public interface IController : IDisposable
	{
		/// <summary>
		/// Occurs just before the action execution.
		/// </summary>
		event ControllerHandler BeforeAction;

		/// <summary>
		/// Occurs just after the action execution.
		/// </summary>
		event ControllerHandler AfterAction;

		/// <summary>
		/// Performs the specified action, which means:
		/// <br/>
		/// 1. Define the default view name<br/>
		/// 2. Run the before filters<br/>
		/// 3. Select the method related to the action name and invoke it<br/>
		/// 4. On error, execute the rescues if available<br/>
		/// 5. Run the after filters<br/>
		/// 6. Invoke the view engine<br/>
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="context">The controller context.</param>
		void Process(IEngineContext engineContext, IControllerContext context);

		/// <summary>
		/// Invoked by the view engine to perform
		/// any logic before the view is sent to the client.
		/// </summary>
		/// <param name="view"></param>
		void PreSendView(object view);

		/// <summary>
		/// Invoked by the view engine to perform
		/// any logic after the view had been sent to the client.
		/// </summary>
		/// <param name="view"></param>
		void PostSendView(object view);
	}
}