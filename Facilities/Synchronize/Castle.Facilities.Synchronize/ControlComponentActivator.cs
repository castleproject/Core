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
	using System.Runtime.Remoting;
	using System.Windows.Forms;
	using Castle.Core;
	using Castle.Core.Interceptor;
	using Castle.MicroKernel;
	using Castle.MicroKernel.ComponentActivator;

	internal class ControlComponentActivator : DefaultComponentActivator
	{
		private readonly InstantiationDelegate instantiateInMainThread;

		private delegate object InstantiationDelegate(CreationContext context);

		/// <summary>
		/// Initializes a new instance of the <see cref="ControlComponentActivator"/> class.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="kernel">The kernel.</param>
		/// <param name="onCreation">Delegate called on construction.</param>
		/// <param name="onDestruction">Delegate called on destruction.</param>
		public ControlComponentActivator(ComponentModel model, IKernel kernel,
		                                 ComponentInstanceDelegate onCreation,
		                                 ComponentInstanceDelegate onDestruction)
			: base(model, kernel, onCreation, onDestruction)
		{
			instantiateInMainThread = InstantiateInMainThread;
		}

		protected override object Instantiate(CreationContext context)
		{
			// The marshal control is used to ensure that all controls
			// are created in the main UI thread context.

			Control marshalingControl = Model.ExtendedProperties[Constants.MarshalControl] as Control;

			if (marshalingControl != null && marshalingControl.InvokeRequired)
			{
				return marshalingControl.Invoke(instantiateInMainThread, new object[] {context});
			}

			return InstantiateInMainThread(context);
		}

		private object InstantiateInMainThread(CreationContext context)
		{
			// The thread associated with the control is the thread in
			// which its windows handle was created, so ensure that the
			// handle is initialized in the context of the main UI thread.

			object component = base.Instantiate(context);
			EnsureHandleCreated(component);
			return component;
		}

		private static void EnsureHandleCreated(object component)
		{
			Control control = (Control)GetUnproxiedInstance(component);
			ControlUtils.EnsureHandleCreated(control);			
		}

		private static object GetUnproxiedInstance(object instance)
		{
			if (!RemotingServices.IsTransparentProxy(instance))
			{
				IProxyTargetAccessor accessor = instance as IProxyTargetAccessor;

				if (accessor != null)
				{
					instance = accessor.DynProxyGetTarget();
				}
			}

			return instance;
		}
	}
}