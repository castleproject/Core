// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace TestSiteNVelocity.Controllers.ActionProviders
{
	using System;

	using Castle.MonoRail.Framework;


	public class DynamicActionProvider1 : IDynamicActionProvider
	{
		/// <summary>
		/// Implementors should register their dynamics 
		/// actions into the controller
		/// </summary>
		public void IncludeActions(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
		{
			controllerContext.DynamicActions["index"] = new IndexDynamicAction();
		}
	}

	public class DynamicActionProvider2 : IDynamicActionProvider
	{
		/// <summary>
		/// Implementors should register their dynamics 
		/// actions into the controller
		/// </summary>
		public void IncludeActions(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
		{
			controllerContext.DynamicActions["save"] = new SaveDynamicAction();
		}
	}

	internal class IndexDynamicAction : IDynamicAction
	{
		/// <summary>
		/// Implementors should perform the action 
		/// upon this invocation
		/// </summary>
		public object Execute(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
		{
			controllerContext.PropertyBag.Add("message", "hello!");

			return null;
		}
	}

	internal class SaveDynamicAction : IDynamicAction
	{
		/// <summary>
		/// Implementors should perform the action 
		/// upon this invocation
		/// </summary>
		public object Execute(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
		{
			((Controller)controller).RenderText("Hello from save dynamic action");

			return null;
		}
	}
}
