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

namespace Castle.Windsor
{
	using System;
	using System.Collections;
	using System.Threading;

	using Castle.MicroKernel;
	using Castle.Windsor.Configuration;


	public class AsyncInitializationContainer : WindsorContainer
	{
		private bool initialized = false;
		private ManualResetEvent mrEvent = new ManualResetEvent(false);
		private Exception savedException;
		private Thread installerThread;

		public AsyncInitializationContainer(IConfigurationStore store) : base(store)
		{
		}

		public AsyncInitializationContainer(IConfigurationInterpreter interpreter) : base(interpreter)
		{
		}

		public AsyncInitializationContainer(string xmlFile) : base(xmlFile)
		{
		}

		public AsyncInitializationContainer(IKernel kernel, IComponentsInstaller installer) : base(kernel, installer)
		{
		}

		/// <summary>
		/// Returns the inner instance of the MicroKernel
		/// </summary>
		public override IKernel Kernel
		{
			get
			{
				if (!IsInstallerThread) mrEvent.WaitOne();

				lock(this)
				{
					if (initialized && savedException == null)
					{
						return base.Kernel;
					}
					else
					{
						throw savedException == null ? new InitializationException() : new InitializationException(savedException);
					}
				}
			}
		}

		private bool IsInstallerThread
		{
			get { return Thread.CurrentThread == installerThread; }
		}

		/// <summary>
		/// Gets or sets the parent container if this instance
		/// is a sub container.
		/// </summary>
		public override IWindsorContainer Parent
		{
			get
			{
				if (!IsInstallerThread) mrEvent.WaitOne();

				lock(this)
				{
					if (initialized && savedException == null)
					{
						return base.Parent;
					}
					else
					{
						throw savedException == null ? new InitializationException() : new InitializationException(savedException);
					}
				}
			}
			set
			{
				if (!IsInstallerThread) mrEvent.WaitOne();

				lock(this)
				{
					if (initialized && savedException == null)
					{
						base.Parent = value;
					}
					else
					{
						throw savedException == null ? new InitializationException() : new InitializationException(savedException);
					}
				}
			}
		}

		/// <summary>
		/// Registers a facility within the kernel.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="facility"></param>
		public override void AddFacility(String key, IFacility facility)
		{
			if (!IsInstallerThread) mrEvent.WaitOne();

			lock(this)
			{
				if (initialized && savedException == null)
				{
					base.AddFacility(key, facility);
				}
				else
				{
					throw savedException == null ? new InitializationException() : new InitializationException(savedException);
				}
			}
		}

		/// <summary>
		/// Adds a component to be managed by the container
		/// </summary>
		/// <param name="key"></param>
		/// <param name="classType"></param>
		public override void AddComponent(String key, Type classType)
		{
			if (!IsInstallerThread) mrEvent.WaitOne();

			lock(this)
			{
				if (initialized && savedException == null)
				{
					base.AddComponent(key, classType);
				}
				else
				{
					throw savedException == null ? new InitializationException() : new InitializationException(savedException);
				}
			}
		}

		/// <summary>
		/// Adds a component to be managed by the container
		/// </summary>
		/// <param name="key"></param>
		/// <param name="serviceType"></param>
		/// <param name="classType"></param>
		public override void AddComponent(String key, Type serviceType, Type classType)
		{
			if (!IsInstallerThread) mrEvent.WaitOne();

			lock(this)
			{
				if (initialized && savedException == null)
				{
					base.AddComponent(key, serviceType, classType);
				}
				else
				{
					throw savedException == null ? new InitializationException() : new InitializationException(savedException);
				}
			}
		}

		public override void AddComponentWithProperties(string key, Type classType, IDictionary extendedProperties)
		{
			if (!IsInstallerThread) mrEvent.WaitOne();

			lock(this)
			{
				if (initialized && savedException == null)
				{
					base.AddComponentWithProperties(key, classType, extendedProperties);
				}
				else
				{
					throw savedException == null ? new InitializationException() : new InitializationException(savedException);
				}
			}
		}

		public override void AddComponentWithProperties(string key, Type serviceType, Type classType, IDictionary extendedProperties)
		{
			if (!IsInstallerThread) mrEvent.WaitOne();

			lock(this)
			{
				if (initialized && savedException == null)
				{
					base.AddComponentWithProperties(key, serviceType, classType, extendedProperties);
				}
				else
				{
					throw savedException == null ? new InitializationException() : new InitializationException(savedException);
				}
			}
		}

		/// <summary>
		/// Returns a component instance by the key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public override object Resolve(String key)
		{
			if (!IsInstallerThread) mrEvent.WaitOne();

			lock(this)
			{
				if (initialized && savedException == null)
				{
					return base.Resolve(key);
				}
				else
				{
					throw savedException == null ? new InitializationException() : new InitializationException(savedException);
				}
			}
		}

		/// <summary>
		/// Returns a component instance by the service
		/// </summary>
		/// <param name="service"></param>
		/// <returns></returns>
		public override object Resolve(Type service)
		{
			if (!IsInstallerThread) mrEvent.WaitOne();

			lock(this)
			{
				if (initialized && savedException == null)
				{
					return base.Resolve(service);
				}
				else
				{
					throw savedException == null ? new InitializationException() : new InitializationException(savedException);
				}
			}
		}

		/// <summary>
		/// Shortcut to the method <see cref="Resolve"/>
		/// </summary>
		public override object this[String key]
		{
			get
			{
				if (!IsInstallerThread) mrEvent.WaitOne();

				lock(this)
				{
					if (initialized && savedException == null)
					{
						return base[key];
					}
					else
					{
						throw savedException == null ? new InitializationException() : new InitializationException(savedException);
					}
				}
			}
		}

		/// <summary>
		/// Shortcut to the method <see cref="Resolve"/>
		/// </summary>
		public override object this[Type service]
		{
			get
			{
				if (!IsInstallerThread) mrEvent.WaitOne();

				lock(this)
				{
					if (initialized && savedException == null)
					{
						return base[service];
					}
					else
					{
						throw savedException == null ? new InitializationException() : new InitializationException(savedException);
					}
				}
			}
		}

		/// <summary>
		/// Releases a component instance
		/// </summary>
		/// <param name="instance"></param>
		public override void Release(object instance)
		{
			if (!IsInstallerThread) mrEvent.WaitOne();

			lock(this)
			{
				if (initialized && savedException == null)
				{
					base.Release(instance);
				}
				else
				{
					throw savedException == null ? new InitializationException() : new InitializationException(savedException);
				}
			}
		}

		/// <summary>
		/// Registers a subcontainer. The components exposed
		/// by this container will be accessible from subcontainers.
		/// </summary>
		/// <param name="childContainer"></param>
		public override void AddChildContainer(IWindsorContainer childContainer)
		{
			if (!IsInstallerThread) mrEvent.WaitOne();

			lock(this)
			{
				if (initialized && savedException == null)
				{
					base.AddChildContainer(childContainer);
				}
				else
				{
					throw savedException == null ? new InitializationException() : new InitializationException(savedException);
				}
			}
		}

		/// <summary>
		/// Executes Dispose on underlying <see cref="IKernel"/>
		/// </summary>
		public override void Dispose()
		{
			if (!IsInstallerThread) mrEvent.WaitOne();

			lock(this)
			{
				if (initialized && savedException == null)
				{
					base.Dispose();
				}
				else
				{
					throw savedException == null ? new InitializationException() : new InitializationException(savedException);
				}
			}
		}

		protected override void RunInstaller()
		{
			installerThread = new Thread(new ThreadStart(InstallerThreadExec));
			installerThread.Start();
		}

		private void InstallerThreadExec()
		{
			lock(this)
			{
				initialized = true;

				try
				{
					Installer.SetUp(this, Kernel.ConfigurationStore);
				}
				catch(Exception ex)
				{
					savedException = ex;
				}
				finally
				{
					mrEvent.Set();
				}
			}
		}
	}
}
