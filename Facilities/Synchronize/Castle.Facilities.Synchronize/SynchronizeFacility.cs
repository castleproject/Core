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

namespace Castle.Facilities.Synchronize
{
	using Castle.MicroKernel.Facilities;

	/// <summary>
	/// Augments the kernel to handle synchronized components.
	/// </summary>
	public class SynchronizeFacility : AbstractFacility
	{
		/// <summary>
		/// Registers all components needed by the facility.
		/// </summary>
		protected override void Init()
		{
			RegisterSynchronizationComponents();
			RegisterSynchronizationInspectors();
		}

		/// <summary>
		/// Registers the synchronization components.
		/// </summary>
		private void RegisterSynchronizationComponents()
		{
			Kernel.AddComponent("sync.interceptor", typeof(SynchronizeInterceptor));
			Kernel.AddComponent("sync.metainfo.store", typeof(SynchronizeMetaInfoStore));
		}

		/// <summary>
		/// Registers the synchronization inspectors.
		/// </summary>
		private void RegisterSynchronizationInspectors()
		{
			Kernel.ComponentModelBuilder.AddContributor(new SynchronizeComponentInspector(Kernel));
			Kernel.ComponentModelBuilder.AddContributor(new ControlComponentInspector(Kernel, FacilityConfig));
		}
	}
}