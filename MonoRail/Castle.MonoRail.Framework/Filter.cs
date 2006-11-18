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
	/// <summary>
	/// Base class for filters which dispatches to virtual methods
	/// based on the <see cref="ExecuteEnum"/> value.
	/// </summary>
	public abstract class Filter : IFilter
	{
		/// <summary>
		/// Implementors should perform they filter logic and
		/// return <c>true</c> if the action should be processed.
		/// </summary>
		/// <param name="exec">When this filter is being invoked</param>
		/// <param name="context">Current context</param>
		/// <param name="controller">The controller instance</param>
		/// <returns><c>true</c> if the action 
		/// should be invoked, otherwise <c>false</c></returns>
		public bool Perform(ExecuteEnum exec, IRailsEngineContext context, Controller controller)
		{
			if (exec == ExecuteEnum.AfterAction)
			{
				OnAfterAction(context, controller);
				return true;
			}
			else if (exec == ExecuteEnum.AfterRendering)
			{
				OnAfterRendering(context, controller);
				return true;
			}
			else if (exec == ExecuteEnum.BeforeAction)
			{
				return OnBeforeAction(context, controller);
			}
			else // if (exec == ExecuteEnum.StartRequest)
			{
				return OnStartRequest(context, controller);
			}
		}

		/// <summary>
		/// Override this method if the filter was set to
		/// handle <see cref="ExecuteEnum.AfterAction"/>
		/// </summary>
		/// <param name="context">The MonoRail request context</param>
		/// <param name="controller">The controller instance</param>
		protected virtual void OnAfterAction(IRailsEngineContext context, Controller controller)
		{
		}

		/// <summary>
		/// Override this method if the filter was set to
		/// handle <see cref="ExecuteEnum.AfterRendering"/>
		/// </summary>
		/// <param name="context">The MonoRail request context</param>
		/// <param name="controller">The controller instance</param>
		protected virtual void OnAfterRendering(IRailsEngineContext context, Controller controller)
		{
		}

		/// <summary>
		/// Override this method if the filter was set to
		/// handle <see cref="ExecuteEnum.BeforeAction"/>
		/// </summary>
		/// <param name="context">The MonoRail request context</param>
		/// <param name="controller">The controller instance</param>
		/// <returns><c>true</c> if the request should proceed, otherwise <c>false</c></returns>
		protected virtual bool OnBeforeAction(IRailsEngineContext context, Controller controller)
		{
			return true;
		}

		/// <summary>
		/// Override this method if the filter was set to
		/// handle <see cref="ExecuteEnum.StartRequest"/>
		/// </summary>
		/// <param name="context">The MonoRail request context</param>
		/// <param name="controller">The controller instance</param>
		/// <returns><c>true</c> if the request should proceed, otherwise <c>false</c></returns>
		protected virtual bool OnStartRequest(IRailsEngineContext context, Controller controller)
		{
			return true;
		}
	}
}
