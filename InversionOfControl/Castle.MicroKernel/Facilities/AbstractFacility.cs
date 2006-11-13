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

namespace Castle.MicroKernel.Facilities
{
	using System;

	using Castle.Core.Configuration;

	/// <summary>
	/// Base class for facilities. 
	/// </summary>
	public abstract class AbstractFacility : IFacility, IDisposable
	{
		private IKernel kernel;
		private IConfiguration facilityConfig;

		/// <summary>
		/// Gets the <see cref="IKernel"/> where the facility is registered.
		/// </summary>
		/// <value>The <see cref="IKernel"/>.</value>
		public IKernel Kernel
		{
			get { return kernel; }
		}

		/// <summary>
		/// Gets the facility configuration.
		/// </summary>
		/// <value>The <see cref="IConfiguration"/> representing 
		/// the facility configuration.</value>
		public IConfiguration FacilityConfig
		{
			get { return facilityConfig; }
		}

		/// <summary>
		/// The custom initialization for the Facility.
		/// </summary>
		/// <remarks>It must be overriden.</remarks>
		protected abstract void Init();

		#region IFacility Members

		/// <summary>
		/// Initializes the facility. First it performs the initialization common for all 
		/// facilities, setting the <see cref="Kernel"/> and the 
		/// <see cref="FacilityConfig"/>. After it, the <c>Init</c> method is invoked
		/// and the custom initilization is perfomed.
		/// </summary>
		/// <param name="kernel"></param>
		/// <param name="facilityConfig"></param>
		public void Init(IKernel kernel, IConfiguration facilityConfig)
		{
			this.kernel = kernel;
			this.facilityConfig = facilityConfig;

			Init();
		}

		/// <summary>
		/// Terminates the Facility, invokind the <see cref="Dispose"/> and setting 
		/// the Kernel to a null reference.
		/// </summary>
		public void Terminate()
		{
			Dispose();
			
			kernel = null;
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Performs the tasks associated with freeing, releasing, or resetting 
		/// the facility resources.
		/// </summary>
		/// <remarks>It can be overriden.</remarks>
		public virtual void Dispose()
		{
		}

		#endregion
	}
}
