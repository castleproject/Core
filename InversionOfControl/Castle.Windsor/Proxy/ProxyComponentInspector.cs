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

namespace Castle.Windsor.Proxy
{
	using System;
	using System.Configuration;
	using Castle.Core;
	using Castle.DynamicProxy;
	using Castle.MicroKernel;
	using Castle.MicroKernel.ModelBuilder;
	using Castle.MicroKernel.SubSystems.Conversion;

	/// <summary>
	/// Inspects the component configuration looking for information
	/// that can influence the generation of a proxy for that component.
	/// </summary>
	[Serializable]
	public class ProxyComponentInspector : IContributeComponentModelConstruction
	{
		/// <summary>
		/// Seaches for proxy behavior in the configuration and, if unsuccessful
		/// look for the <see cref="ComponentProxyBehaviorAttribute"/> attribute in 
		/// the implementation type.
		/// </summary>
		/// <param name="kernel"></param>
		/// <param name="model"></param>
		public virtual void ProcessModel(IKernel kernel, ComponentModel model)
		{
			if (!ReadProxyBehaviorFromConfiguration(kernel, model))
			{
				ReadProxyBehaviorFromType(model);
			}
		}

		/// <summary>
		/// Reads the proxy behavior associated with the 
		/// component configuration and applies it to the model.
		/// </summary>
		/// <exception cref="System.Configuration.ConfigurationException">
		/// If the conversion fails
		/// </exception>
		/// <param name="kernel"></param>
		/// <param name="model"></param>
		/// <returns></returns>
		protected virtual bool ReadProxyBehaviorFromConfiguration(IKernel kernel, ComponentModel model)
		{
			if (model.Configuration != null)
			{
				String useSingleInterfaceProxyAttrib = model.Configuration.Attributes["useSingleInterfaceProxy"];

				if (useSingleInterfaceProxyAttrib != null)
				{
					try
					{
						ITypeConverter converter = (ITypeConverter) kernel.GetSubSystem(SubSystemConstants.ConversionManagerKey);

						bool useSingleInterfaceproxy = (bool) converter.PerformConversion(useSingleInterfaceProxyAttrib, typeof(bool));

						ApplyComponentProxyBehavior(useSingleInterfaceproxy, model);
					}
					catch
					{
						String message = String.Format(
							"Could not convert the specified attribute value " +
							"'{0}' to a boolean value", useSingleInterfaceProxyAttrib);

#if DOTNET2
						throw new ConfigurationErrorsException(message);
#else
						throw new ConfigurationException(message);
#endif
					}

					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Check if the type exposes the <see cref="ComponentProxyBehaviorAttribute"/>.
		/// </summary>
		/// <param name="model"></param>
		protected virtual void ReadProxyBehaviorFromType(ComponentModel model)
		{
			object[] attributes = model.Implementation.GetCustomAttributes(
				typeof(ComponentProxyBehaviorAttribute), true);

			if (attributes.Length != 0)
			{
				ComponentProxyBehaviorAttribute attribute = (ComponentProxyBehaviorAttribute) attributes[0];

				ApplyComponentProxyBehavior(attribute.UseSingleInterfaceProxy, model);
			}
		}

		private static void ApplyComponentProxyBehavior(bool useSingleInterfaceProxy, ComponentModel model)
		{
			if (useSingleInterfaceProxy)
			{
				EnsureComponentRegisteredAsService(model);
			}

			ProxyGenerationOptions options = ProxyUtil.ObtainProxyGenerationOptions(model, true);
			options.UseSingleInterfaceProxy = useSingleInterfaceProxy;
		}

		private static void EnsureComponentRegisteredAsService(ComponentModel model)
		{
			if (!model.Service.IsInterface)
			{
				String message = String.Format("The class {0} requested a single interface proxy, " +
				                               "however the service {1} does not represent an interface",
											   model.Implementation.FullName, model.Service.FullName);

				throw new ComponentRegistrationException(message);
			}
		}
	}
}