// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Manages the execution of the steps associated with 
	/// a controller (filters/action/disposal).
	/// </summary>
	/// <remarks>
	/// This class is statefull.
	/// </remarks>
	public class ControllerExecutor
	{
		private readonly object controller;
		private readonly IExecutionContext executionContext;

		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerExecutor"/> class.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="executionContext">The execution context.</param>
		public ControllerExecutor(object controller, IExecutionContext executionContext)
		{
			if (controller == null) throw new ArgumentNullException("controller");
			if (executionContext == null) throw new ArgumentNullException("executionContext");

			UrlInfo url = executionContext.OriginalUrl;

			IController properController = controller as IController;

			if (properController != null)
			{
				properController.SetInitialState(url.Area, url.Controller, url.Action);
			}

			this.controller = controller;
			this.executionContext = executionContext;
		}

		public ActionExecutor SelectAction()
		{
			return SelectAction(executionContext.OriginalUrl.Action);
		}

		public ActionExecutor SelectAction(string actionName)
		{
			// There is room for a clever cache here

			// Right now let's support method action only

			BindingFlags flags = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;

			MethodInfo actionMethod = controller.GetType().GetMethod(actionName, flags);

			if (actionMethod != null)
			{
				return new MethodActionExecutor(actionMethod);
			}

			return null;
		}

		public void Execute(ActionExecutor executor)
		{
			if (executor == null) throw new ArgumentNullException("executor");

			executor.Execute(controller);
		}
	}
}