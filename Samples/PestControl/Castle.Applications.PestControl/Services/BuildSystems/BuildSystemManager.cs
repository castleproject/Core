// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.Applications.PestControl.Services.BuildSystems
{
	using System;
	using System.Collections;

	using Castle.Model;

	using Castle.MicroKernel;

	/// <summary>
	/// Summary description for BuildSystemManager.
	/// </summary>
	public class BuildSystemManager : IInitializable, IDisposable
	{
		private IKernel _kernel;

		public BuildSystemManager(IKernel kernel)
		{
			_kernel = kernel;
		}

		public void Initialize()
		{
		}

		public void Dispose()
		{
//			_kernel = null;
		}

		/// <summary>
		/// Queries the kernel for implementations of IBuildSystem.
		/// This allow new build system to be registered even without
		/// restarting the application. A more efficient approach however
		/// is to issue this query only once, but then you lost the ability
		/// to add/remove buildsystems at runtime
		/// </summary>
		/// <remarks>
		/// This approach invokes Resolve on the handler, 
		/// but do not invoke the counter part release.
		/// </remarks>
		/// <returns></returns>
		public virtual IBuildSystem[] AvailableBuildSystems()
		{
			ArrayList list = new ArrayList();

			IHandler[] handlers = _kernel.GetHandlers( typeof(IBuildSystem) );

			foreach(IHandler handler in handlers)
			{
				if (handler.CurrentState == HandlerState.Valid)
				{
					list.Add( handler.Resolve() );
				}
			}

			return (IBuildSystem[]) list.ToArray( typeof(IBuildSystem) );
		}
	}
}
