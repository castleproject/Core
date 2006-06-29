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
	using System.Diagnostics;
	using System.IO;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Text.RegularExpressions;

	using Castle.MicroKernel;
	using Castle.MicroKernel.ComponentActivator;
	using Castle.Model;

	/// <summary>
	/// Loads components on an isolated <see cref="AppDomain"/>.
	/// </summary>
	public class RemoteLoader : MarshalByRefObject, IDisposable
	{
		private static readonly Regex rxComponentIdMask = new Regex("([*]|$)", RegexOptions.Compiled);

		private readonly IKernel kernel;
		private readonly object syncRegister = new object();

		private int seqComponent;
		private string componentIdMask;

		/// <summary>
		/// Creates a new <see cref="RemoteLoader"/>. This constructor should not be called
		/// directly in the code, but via <see cref="AppDomain.CreateInstance(string,string)"/>.
		/// </summary>
		public RemoteLoader()
		{
			this.kernel = new DefaultKernel();

			// forces the loading of every library on the AppDomain directory
			LoadAllAssemblies();
		}
		
		/// <summary>
		/// Searches for implementations of the given services in the current <see cref="AppDomain"/>
		/// and add as components. Used by <see cref="DynamicLoaderFacility.InitializeBatchRegistration"/>.
		/// </summary>
		/// <param name="componentIdMask">The component id mask. Any <c>*</c> (asterisk) character will be replaced by a sequential number, starting by 1 (one).</param>
		/// <param name="services">The services in which to test</param>
		public void RegisterByServiceProvided(string componentIdMask, params Type[] services)
		{
			lock (syncRegister)
			{
				this.componentIdMask = componentIdMask;
				this.seqComponent = 0;

				foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
				{
					Debug.WriteLine("Assembly: " + asm);
					foreach (Type t in asm.GetExportedTypes())
					{
						foreach (Type serviceType in services)
						{
							if (!IsValidServiceImplementation(serviceType, t))
								continue;

							string newId = GenerateComponentId();
							Debug.WriteLine(String.Format("Adding component: '{0}': {1} ({2})", newId, t, serviceType));
							kernel.AddComponent(newId, serviceType, t);
						}
					}
				}
			}
		}

		/// <summary>
		/// Loads all assemblies in the current <see cref="AppDomain"/>.
		/// </summary>
		private void LoadAllAssemblies()
		{
			DirectoryInfo baseDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
			foreach (FileInfo file in baseDir.GetFiles("*.dll"))
				Assembly.Load(Path.GetFileNameWithoutExtension(file.Name));
		}

		/// <summary>
		/// The <see cref="IKernel"/> in which the components are registered.
		/// </summary>
		public IKernel Kernel
		{
			get { return kernel; }
		}

		/// <summary>
		/// Disposes the <see cref="Kernel"/>.
		/// </summary>
		public void Dispose()
		{
			kernel.Dispose();
		}

		/// <summary>
		/// Checks whether a type <paramref name="t"/> is a valid implementation of a
		/// given service <paramref name="serviceType"/>.
		/// </summary>
		/// <param name="serviceType">The service type</param>
		/// <param name="t">The component type</param>
		/// <returns>
		/// <c>true</c> if <paramref name="t"/> is a valid implementation of the
		/// service specified by <paramref name="serviceType"/>, <c>false</c> otherwise.
		/// </returns>
		private bool IsValidServiceImplementation(Type serviceType, Type t)
		{
			return
				!t.IsAbstract &&
				serviceType.IsAssignableFrom(t) &&
				t != serviceType;
		}

		/// <summary>
		/// Generates an unique component id, given the <see cref="componentIdMask"/>.
		/// </summary>
		/// <returns>The unique component id</returns>
		private string GenerateComponentId()
		{
			if (!rxComponentIdMask.IsMatch(componentIdMask))
			{
				if (!kernel.HasComponent(componentIdMask))
					return componentIdMask;
				else
					componentIdMask += ".*";
			}

			string newId;
			do
			{
				newId = rxComponentIdMask.Replace(componentIdMask, (++seqComponent).ToString(), 1);
			} while (kernel.HasComponent(newId));
			return newId;
		}

		/// <summary>
		/// Creates a component on an isolated <see cref="AppDomain"/>.
		/// </summary>
		public object CreateRemoteInstance(ComponentModel model, CreationContext context, object[] arguments, Type[] signature)
		{
			object instance;

			Type implType = model.Implementation;

			if (model.Interceptors.HasInterceptors)
			{
				try
				{
					instance = Kernel.ProxyFactory.Create(Kernel, model, arguments);
				}
				catch (Exception ex)
				{
					throw new ComponentActivatorException("RemoteLoader: could not proxy " + model.Implementation.FullName, ex);
				}
			}
			else
			{
				try
				{
					ConstructorInfo cinfo = implType.GetConstructor(
							BindingFlags.Public | BindingFlags.Instance, null, signature, null);

					instance = FormatterServices.GetUninitializedObject(implType);

					cinfo.Invoke(instance, arguments);
				}
				catch (Exception ex)
				{
					throw new ComponentActivatorException("RemoteLoader: could not instantiate " + model.Implementation.FullName, ex);
				}
			}

			return instance;
		}
	}
}