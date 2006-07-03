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

namespace Castle.Facilities.DynamicLoader
{
	using System;
	using System.Runtime.Remoting.Lifetime;

	using Castle.MicroKernel;
	using Castle.MicroKernel.ComponentActivator;
	using Castle.MicroKernel.Facilities;
	using Castle.Model;

	/// <summary>
	/// Delegates the creation of components to a <see cref="RemoteLoader"/>,
	/// which creates the component on a different <see cref="AppDomain"/>.
	/// </summary>
	public class DynamicLoaderActivator : DefaultComponentActivator, IDisposable
	{
		readonly RemoteLoader loader;
		ClientSponsor keepAliveSponsor;

		/// <summary>
		/// Creates a new <see cref="DynamicLoaderActivator"/>.
		/// </summary>
		public DynamicLoaderActivator(ComponentModel model, IKernel kernel, ComponentInstanceDelegate onCreation, ComponentInstanceDelegate onDestruction)
			: base(model, kernel, onCreation, onDestruction)
		{
			if (!model.Implementation.IsSubclassOf(typeof(MarshalByRefObject)))
				throw new FacilityException(String.Format("The implementation for the component '{0}' must inherit from System.MarshalByRefObject in order to be created in an isolated AppDomain.", model.Name));

			this.loader = (RemoteLoader) model.ExtendedProperties["dynamicLoader.loader"];
			
			if (this.loader == null)
				throw new FacilityException(String.Format("A remote loader was not created for component '{0}'.", model.Name));
		}

		/// <summary>
		/// Creates the component instance by calling the <see cref="RemoteLoader.CreateRemoteInstance"/>
		/// method. The component is then registered with the <see cref="ClientSponsor"/>
		/// with a renewal time of 2 minutes, in order to stay alive forever.
		/// </summary>
		protected override object CreateInstance(CreationContext context, object[] arguments, Type[] signature)
		{
			object instance = loader.CreateRemoteInstance(Model, context, arguments, signature);

			if (keepAliveSponsor == null)
				keepAliveSponsor = new ClientSponsor(TimeSpan.FromMinutes(2));

			keepAliveSponsor.Register((MarshalByRefObject) instance);

			return instance;
		}

		/// <summary>
		/// Disposes an object, and unregisters it from the <see cref="ClientSponsor"/>.
		/// </summary>
		/// <param name="instance">The object being destroyed</param>
		public override void Destroy(object instance)
		{
			if (instance is IDisposable)
				((IDisposable) instance).Dispose();

			if (keepAliveSponsor != null)
				keepAliveSponsor.Unregister((MarshalByRefObject) instance);
		}

		/// <summary>
		/// Closes the <see cref="ClientSponsor"/> used to keep remote objects alive.
		/// </summary>
		public void Dispose()
		{
			if (keepAliveSponsor != null)
				keepAliveSponsor.Close();
		}
	}
}