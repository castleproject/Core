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

namespace Castle.Facilities.Startable
{
	using System;
	using System.Collections;

	using Castle.Model;

	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities;


	public class StartableFacility : AbstractFacility
	{
		private ArrayList _waitList = new ArrayList();
		
		protected override void Init()
		{
			Kernel.ComponentModelCreated += 
				new ComponentModelDelegate(OnComponentModelCreated);
			Kernel.ComponentRegistered += 
				new ComponentDataDelegate(OnComponentRegistered);
		}

		private void OnComponentModelCreated(ComponentModel model)
		{
			bool startable = typeof(IStartable).IsAssignableFrom(model.Implementation);

			model.ExtendedProperties["startable"] = startable;

			if (startable)
			{
				model.LifecycleSteps.Add( 
					LifecycleStepType.Commission, StartConcern.Instance );
				model.LifecycleSteps.Add( 
					LifecycleStepType.Decommission, StopConcern.Instance );
			}
		}

		private void OnComponentRegistered(String key, IHandler handler)
		{
			bool startable = (bool) handler.ComponentModel.ExtendedProperties["startable"];
			
			if (startable)
			{
				if (handler.CurrentState == HandlerState.WaitingDependency)
				{
					_waitList.Add( handler );
				}
				else
				{
					Start( key );
				}
			}

			CheckWaitingList();
		}

		/// <summary>
		/// For each new component registered,
		/// some components in the WaitingDependency
		/// state may have became valid, so we check them
		/// </summary>
		private void CheckWaitingList()
		{
			IHandler[] handlers = (IHandler[]) 
				_waitList.ToArray( typeof(IHandler) );

			foreach(IHandler handler in handlers)
			{
				if (handler.CurrentState == HandlerState.Valid)
				{
					Start( handler.ComponentModel.Name );

					_waitList.Remove(handler);
				}
			}
		}

		/// <summary>
		/// Request the component instance
		/// </summary>
		/// <param name="key"></param>
		private void Start(String key)
		{
			object instance = Kernel[key];
		}
	}
}
