// Copyright 2003-2004 The Apache Software Foundation
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

using System;

namespace Apache.Avalon.Framework
{
	/// <summary>
	///	Utility class that makes it easier to transfer
	/// a component through it's lifecycle stages.
	/// </summary>
	public class ContainerUtil
	{
		/// <summary>
		/// Runs specified object through shutdown lifecycle stages
		/// (Stop and Dispose).
		/// </summary>
		/// <param name="component">The Component to shutdown.</param>
		/// <exception cref="Exception">If there is a problem stoppping object.</exception>
		public static void Shutdown(object component)
		{
			Stop(component);
			Dispose(component);
		}

		/// <summary>
		/// Supplies specified object with Logger if it implements the
		/// <see cref="ILogEnabled"/> interface.
		/// </summary>
		/// <param name="component">The component instance</param>
		/// <param name="logger">The Logger to enable component with.</param>
		/// <exception cref="ArgumentException">
		/// If the component is <see cref="ILogEnabled"/> but <see cref="ILogger"/> is null.
		/// </exception>
		/// <remarks>
		/// The Logger may be null in which case
		/// the specified component must not implement <see cref="ILogEnabled"/>.
		/// </remarks>
		public static void EnableLogging(object component, ILogger logger)
		{
			if (component is ILogEnabled)
			{
				if (logger == null)
				{
					throw new ArgumentNullException("logger is null");
				}

				((ILogEnabled) component).EnableLogging(logger);
			}
		}

		/// <summary>
		/// Supplies specified object with Context if it implements the
		/// <see cref="IContextualizable"/> interface.
		/// </summary>
		/// <param name="component">The component instance</param>
		/// <param name="context">The context.</param>
		/// <exception cref="ArgumentException">
		/// If the component is <see cref="IContextualizable"/> but <see cref="IContext"/> is null.
		/// </exception>
		/// <remarks>
		/// 
		/// </remarks>
		public static void Contextualize(object component, IContext context)
		{
			if (component is IContextualizable)
			{
				if (context == null)
				{
					throw new ArgumentNullException("context");
				}

				((IContextualizable) component).Contextualize(context);
			}
		}

		/// <summary>
		/// Checks if the specified components supports IStartable
		/// or IDisposable interfaces - meaning that it cares about Stop
		/// or disposable phases.
		/// </summary>
		/// <param name="component">The component instance</param>
		/// <returns>true if the component wants the shutdown phase.</returns>
		public static bool ExpectsDispose(object component)
		{
			return (component is IStartable) || (component is IDisposable);
		}

		/// <summary>
		/// Supplies specified component with <see cref="ILookupManager"/>
		/// if it implements the <see cref="ILookupEnabled"/> interface.
		/// </summary>
		/// <param name="component">The Component to service.</param>
		/// <param name="lookupManager">
		/// The <see cref="ILookupManager"/> object to use for component.
		/// </param>
		/// <exception cref="ArgumentException">
		/// If the object is <see cref="ILookupEnabled"/> but
		/// <see cref="ILookupManager"/> is null.
		/// </exception>
		/// <exception cref="LookupException">
		/// If there is a problem servicing component.
		/// </exception>
		/// <remarks>
		/// The Service manager may be null in
		/// which case the specified component must not
		/// implement <see cref="ILookupEnabled"/>.
		/// </remarks>
		public static void Service(object component, ILookupManager lookupManager)
		{
			if (component is ILookupEnabled)
			{
				if (lookupManager == null)
				{
					throw new ArgumentNullException("LookupManager is null");
				}

				((ILookupEnabled) component).EnableLookups(lookupManager);
			}
		}


		/// <summary>
		/// Configures specified component if it implements the
		/// <see cref="IConfigurable"/> interface.
		/// </summary>
		/// <param name="component">The Component to configure.</param>
		/// <param name="configuration">
		/// The Configuration object to use during the configuration.
		/// </param>
		/// <exception cref="ArgumentException">
		/// If the component is <see cref="IConfigurable"/> but
		/// configuration is null.
		/// </exception>
		/// <exception cref="ConfigurationException">
		/// If there is a problem configuring component,
		/// or the component is <see cref="IConfigurable"/> but configuration is null.
		/// </exception>
		/// <remarks>
		/// The Configuration may be null in which case
		///  the specified component must not implement <see cref="IConfigurable"/>.
		/// </remarks>
		public static void Configure(object component, IConfiguration configuration)
		{
			if (component is IConfigurable)
			{
				if (configuration == null)
				{
					throw new ArgumentNullException("configuration is null");
				}

				((IConfigurable) component).Configure(configuration);
			}
		}

		/// <summary>
		/// Initializes specified component if it implements the
		/// <see cref="IInitializable"/> interface.
		/// </summary>
		/// <param name="component">
		/// The Component to initialize.
		/// </param>
		/// <exception cref="Exception">
		/// If there is a problem initializing component.
		/// </exception>
		public static void Initialize(object component)
		{
			if (component is IInitializable)
			{
				( (IInitializable) component).Initialize();
			}
		}

		/// <summary>
		/// Starts specified component if it implements the
		/// <see cref="IStartable"/> interface.
		/// </summary>
		/// <param name="component">The Component to start.</param>
		/// <exception cref="Exception">
		/// If there is a problem starting component.
		/// </exception>
		public static void Start(object component)
		{
			if (component is IStartable)
			{
				( (IStartable) component).Start();
			}
		}

		/// <summary>
		/// Stops specified components if it implements the
		/// <see cref="IStartable"/> interface.
		/// </summary>
		/// <param name="component">The Component to stop.</param>
		/// <exception cref="Exception">
		/// If there is a problem stoppping component.
		/// </exception>
		public static void Stop(object component)
		{
			if (component is IStartable)
			{
				( (IStartable) component).Stop();
			}
		}

		/// <summary>
		/// Disposes specified component if it implements the
		/// <see cref="IDisposable"/> interface.
		/// </summary>
		/// <param name="component">The Component to dispose.</param>
		/// <exception cref="Exception">
		/// If there is a problem disposing component.
		/// </exception>
		public static void Dispose(object component)
		{
			if (component is IDisposable)
			{
				((IDisposable) component).Dispose();
			}
		}
	}
}
