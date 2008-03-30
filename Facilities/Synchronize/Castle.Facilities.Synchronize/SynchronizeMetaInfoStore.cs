// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Castle.Core;
	using Castle.Core.Configuration;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Proxy;
	using Castle.MicroKernel.SubSystems.Conversion;

	/// <summary>
	/// Maintains the synchronization meta-info for all components.
	/// </summary>
	internal class SynchronizeMetaInfoStore
	{
		private readonly IConversionManager converter;

		private readonly IDictionary<Type, SynchronizeMetaInfo> type2MetaInfo =
			new Dictionary<Type, SynchronizeMetaInfo>();

		private static readonly BindingFlags MethodBindingFlags
			= BindingFlags.Instance
			  | BindingFlags.Public
			  | BindingFlags.NonPublic
			  | BindingFlags.DeclaredOnly;

		/// <summary>
		/// Initializes a new instance of the <see cref="SynchronizeMetaInfoStore"/> class.
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		public SynchronizeMetaInfoStore(IKernel kernel)
		{
			converter = ObtainConversionManager(kernel);
		}

		/// <summary>
		/// Creates the meta-info from the specified type.
		/// </summary>
		/// <param name="model">The component model.</param>
		/// <returns>The corresponding meta-info.</returns>
		public SynchronizeMetaInfo CreateMetaFromModel(ComponentModel model)
		{
			Type implementation = model.Implementation;

			SynchronizeAttribute syncAttrib = (SynchronizeAttribute)
				model.Implementation.GetCustomAttributes(true)[0];

			SynchronizeMetaInfo metaInfo = new SynchronizeMetaInfo(syncAttrib);

			if (model.Service.IsInterface)
			{
			    PopulateMetaInfoFromService(metaInfo, model);
			}
			else
			{
				PopulateMetaInfoFromClass(metaInfo, implementation);
			}

			Register(implementation, metaInfo);

			return metaInfo;
		}

		/// <summary>
		/// Populates the meta-info from the class attributes.
		/// </summary>
		/// <param name="metaInfo">The meta info.</param>
		/// <param name="implementation">The implementation type.</param>
		private static void PopulateMetaInfoFromClass(SynchronizeMetaInfo metaInfo,
		                                              Type implementation)
		{
			if (implementation == typeof(object) ||
			    implementation == typeof(MarshalByRefObject) ||
				implementation.IsGenericTypeDefinition)
			{
				return;
			}

			MethodInfo[] methods = implementation.GetMethods(MethodBindingFlags);

			foreach(MethodInfo method in methods)
			{
				PopulateMetaInfoFromMethod(metaInfo, method, null);
			}

			PopulateMetaInfoFromClass(metaInfo, implementation.BaseType);
		}

		/// <summary>
		/// Populates the meta-info from the service attributes.
		/// </summary>
		/// <param name="metaInfo">The mata info.</param>
		/// <param name="model">The component model.</param>
		private static void PopulateMetaInfoFromService(SynchronizeMetaInfo metaInfo,
												        ComponentModel model)
		{
			Type service = model.Service;

			if (!service.IsGenericTypeDefinition)
			{
				ProxyOptions proxyOptions = ProxyUtil.ObtainProxyOptions(model, true);

				if (proxyOptions.OmitTarget)
				{
					MethodInfo[] methods = service.GetMethods();

					foreach (MethodInfo method in methods)
					{
						PopulateMetaInfoFromMethod(metaInfo, method, null);
					}
				}
				else
				{
					InterfaceMapping mapping = model.Implementation.GetInterfaceMap(service);
					MethodInfo[] interfaceMethods = mapping.InterfaceMethods;
					MethodInfo[] targetMethods = mapping.TargetMethods;

					for (int i = 0; i < interfaceMethods.Length; ++i)
					{
						PopulateMetaInfoFromMethod(metaInfo, targetMethods[i], interfaceMethods[i]);
					}
				}
			}
		}

		/// <summary>
		/// Populates the meta-info from the method attributes.
		/// </summary>
		/// <param name="metaInfo">The meta info.</param>
		/// <param name="method">The method.</param>
		/// <param name="interfaceMethod">The base method.</param>
		private static void PopulateMetaInfoFromMethod(SynchronizeMetaInfo metaInfo,
													   MethodInfo method, MethodInfo interfaceMethod)
		{
			object[] atts = method.GetCustomAttributes(typeof(SynchronizeAttribute), true);

			if (atts.Length == 0 && interfaceMethod != null)
			{
				atts = interfaceMethod.GetCustomAttributes(typeof(SynchronizeAttribute), true);
			}

			if (atts.Length != 0)
			{
				metaInfo.Add(method, atts[0] as SynchronizeAttribute);
			}
		}

		/// <summary>
		/// Creates the meta-info from the configuration.
		/// </summary>
		/// <param name="implementation">The implementation type.</param>
		/// <param name="config">The configuration.</param>
		/// <returns>The corresponding meta-info.</returns>
		public SynchronizeMetaInfo CreateMetaInfoFromConfig(Type implementation, IConfiguration config)
		{
			SynchronizeAttribute syncAttrib = CreateAttributeFromConfig(config);

			SynchronizeMetaInfo metaInfo = new SynchronizeMetaInfo(syncAttrib);

			Register(implementation, metaInfo);

			return metaInfo;
		}

		/// <summary>
		/// Populates the meta-info from the configuration.
		/// </summary>
		/// <param name="implementation">The implementation.</param>
		/// <param name="methods">The methods.</param>
		/// <param name="config">The config.</param>
		public void PopulateMetaFromConfig(Type implementation, MethodInfo[] methods, IConfiguration config)
		{
			SynchronizeMetaInfo metaInfo = GetMetaFor(implementation);

			if (metaInfo != null)
			{
				foreach(MethodInfo method in methods)
				{
					SynchronizeAttribute syncAttrib = CreateAttributeFromConfig(config);
					metaInfo.Add(method, syncAttrib);
				}
			}
		}

		/// <summary>
		/// Gets the meta-info for the specified implementation type.
		/// </summary>
		/// <param name="implementation">The implementation type.</param>
		/// <returns>The corresponding meta-info.</returns>
		public SynchronizeMetaInfo GetMetaFor(Type implementation)
		{
			SynchronizeMetaInfo metaInfo;
			type2MetaInfo.TryGetValue(implementation, out metaInfo);
			return metaInfo;
		}

		/// <summary>
		/// Registers the meta-info for the specified implementation type.
		/// </summary>
		/// <param name="implementation">The implementation type.</param>
		/// <param name="metaInfo">The meta-info.</param>
		private void Register(Type implementation, SynchronizeMetaInfo metaInfo)
		{
			type2MetaInfo[implementation] = metaInfo;
		}

		/// <summary>
		/// Creates the synchronization attribute from configuration.
		/// </summary>
		/// <param name="config">The configuration.</param>
		/// <returns>The corresponding synchronization attribute.</returns>
		private SynchronizeAttribute CreateAttributeFromConfig(IConfiguration config)
		{
			SynchronizeAttribute syncAttrib = null;

			if (config != null)
			{
				String contextRef = config.Attributes[Constants.ContextRefAttribute];

				if (contextRef != null)
				{
					syncAttrib = new SynchronizeAttribute(contextRef);
				}
				else
				{
					String contextType = config.Attributes[Constants.ContextTypeAttribute];

					if (contextType != null)
					{
						Type type = (Type) converter.PerformConversion(contextType, typeof(Type));
						syncAttrib = new SynchronizeAttribute(type);
					}
				}
			}

			return syncAttrib ?? new SynchronizeAttribute();
		}

		/// <summary>
		/// Obtains the <see cref="IConversionManager"/>.
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		/// <returns>The <see cref="IConversionManager"/>.</returns>
		private static IConversionManager ObtainConversionManager(IKernel kernel)
		{
			return (IConversionManager) kernel.GetSubSystem(SubSystemConstants.ConversionManagerKey);
		}
	}
}