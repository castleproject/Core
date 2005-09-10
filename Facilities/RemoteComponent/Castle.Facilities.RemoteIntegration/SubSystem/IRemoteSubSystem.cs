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

namespace Castle.Facilities.RemoteIntegration
{
	using System;
	using Castle.MicroKernel;
	using Castle.MicroKernel.SubSystems.Naming;

	public interface IRemoteSubSystem: ISubSystem
	{
		/// <summary>
		/// Remote uri.
		/// </summary>
		String RemoteUri{get;set;}
		/// <summary>
		/// Connect to remote machine.
		/// </summary>
		void Connect();
		/// <summary>
		/// Connect to remote machine from uri.
		/// </summary>
		/// <param name="remoteUri">Remote uri</param>
		void Connect(string remoteUri);
		/// <summary>
		/// Remote SubSystem.
		/// </summary>
		IRemoteSubSystem Remote{get;}
		/// <summary>
		/// Return SubSystem from remote machine.
		/// </summary>
		/// <param name="key">Name of SubSystem</param>
		ISubSystem GetSubSystem(String key);  
	}
}
