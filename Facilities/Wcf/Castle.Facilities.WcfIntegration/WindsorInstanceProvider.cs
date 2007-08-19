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

using Castle.MicroKernel;

namespace Castle.Facilities.WcfIntegration
{
	using System;
	using System.ServiceModel;
	using System.ServiceModel.Channels;
	using System.ServiceModel.Dispatcher;
	using Windsor;

	/// <summary>
	/// Initialize a service using Windsor
	/// </summary>
	public class WindsorInstanceProvider : IInstanceProvider
	{
		private readonly IKernel kernel;
		private readonly Type type;

		/// <summary>
		/// Initializes a new instance of the <see cref="WindsorInstanceProvider"/> class.
		/// </summary>
		public WindsorInstanceProvider(IKernel kernel, Type type)
		{
			this.kernel = kernel;
			this.type = type;
		}

		#region IInstanceProvider Members

		///<summary>
		///Returns a service object given the specified <see cref="T:System.ServiceModel.InstanceContext"></see> object.
		///</summary>
		///
		///<returns>
		///A user-defined service object.
		///</returns>
		///
		///<param name="instanceContext">The current <see cref="T:System.ServiceModel.InstanceContext"></see> object.</param>
		public object GetInstance(InstanceContext instanceContext)
		{
			return GetInstance(instanceContext, null);
		}

		///<summary>
		///Returns a service object given the specified <see cref="T:System.ServiceModel.InstanceContext"></see> object.
		///</summary>
		///
		///<returns>
		///The service object.
		///</returns>
		///
		///<param name="message">The message that triggered the creation of a service object.</param>
		///<param name="instanceContext">The current <see cref="T:System.ServiceModel.InstanceContext"></see> object.</param>
		public object GetInstance(InstanceContext instanceContext, Message message)
		{
			return kernel.Resolve(type);
		}

		///<summary>
		///Called when an <see cref="T:System.ServiceModel.InstanceContext"></see> object recycles a service object.
		///</summary>
		///
		///<param name="instanceContext">The service's instance context.</param>
		///<param name="instance">The service object to be recycled.</param>
		public void ReleaseInstance(InstanceContext instanceContext, object instance)
		{
			kernel.ReleaseComponent(instance);
		}

		#endregion
	}
}