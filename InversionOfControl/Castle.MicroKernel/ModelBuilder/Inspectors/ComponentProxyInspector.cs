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

namespace Castle.MicroKernel.ModelBuilder.Inspectors
{
	using System;
	using System.Configuration;
	using Castle.Core;
	using Castle.MicroKernel.Proxy;
	using Castle.MicroKernel.SubSystems.Conversion;

	/// <summary>
	/// Inspects the component configuration and type looking for information
	/// that can influence the generation of a proxy for that component.
	/// <para>
	/// We specifically look for <c>useSingleInterfaceProxy</c> and <c>marshalByRefProxy</c> 
	/// on the component configuration or the <see cref="ComponentProxyBehaviorAttribute"/> 
	/// attribute.
	/// </para>
	/// </summary>
	[Serializable]
	public class ComponentProxyInspector : IContributeComponentModelConstruction
	{
		/// <summary>
		/// Seaches for proxy behavior in the configuration and, if unsuccessful
		/// look for the <see cref="ComponentProxyBehaviorAttribute"/> attribute in 
		/// the implementation type.
		/// </summary>
		public virtual void ProcessModel(IKernel kernel, ComponentModel model)
		{
			ReadProxyBehavior(kernel, model);
		}

		/// <summary>
		/// Reads the proxy behavior associated with the 
		/// component configuration/type and applies it to the model.
		/// </summary>
		/// <exception cref="System.Configuration.ConfigurationException">
		/// If the conversion fails
		/// </exception>
		/// <param name="kernel"></param>
		/// <param name="model"></param>
		protected virtual void ReadProxyBehavior(IKernel kernel, ComponentModel model)
		{
			ComponentProxyBehaviorAttribute proxyBehaviorAtt = GetProxyBehaviorFromType(model.Implementation);

			if (proxyBehaviorAtt == null)
			{
				proxyBehaviorAtt = new ComponentProxyBehaviorAttribute();
			}

			string useSingleInterfaceProxyAttrib = model.Configuration != null ? model.Configuration.Attributes["useSingleInterfaceProxy"] : null;
			string marshalByRefProxyAttrib = model.Configuration != null ? model.Configuration.Attributes["marshalByRefProxy"] : null;

			ITypeConverter converter = (ITypeConverter)kernel.GetSubSystem(SubSystemConstants.ConversionManagerKey);

			if (useSingleInterfaceProxyAttrib != null)
			{
				try
				{
					proxyBehaviorAtt.UseSingleInterfaceProxy = (bool)
						converter.PerformConversion(useSingleInterfaceProxyAttrib, typeof(bool));
				}
				catch(ConverterException ex)
				{
					throw new ConfigurationErrorsException("Could not convert attribute " + 
						"'useSingleInterfaceProxy' to bool. Value is " + useSingleInterfaceProxyAttrib, ex);
				}
			}

			if (marshalByRefProxyAttrib != null)
			{
				try
				{
					proxyBehaviorAtt.UseMarshalByRefProxy = (bool)
						converter.PerformConversion(marshalByRefProxyAttrib, typeof(bool));
				}
				catch(ConverterException ex)
				{
					throw new ConfigurationErrorsException("Could not convert attribute " + 
						"'marshalByRefProxy' to bool. Value is " + marshalByRefProxyAttrib, ex);
				}
			}

			ApplyProxyBehavior(proxyBehaviorAtt, model);
		}

		/// <summary>
		/// Returns a <see cref="ComponentProxyBehaviorAttribute"/> instance if the type
		/// uses the attribute. Otherwise returns null.
		/// </summary>
		/// <param name="implementation"></param>
		protected virtual ComponentProxyBehaviorAttribute GetProxyBehaviorFromType(Type implementation)
		{
			object[] attributes = implementation.GetCustomAttributes(
				typeof(ComponentProxyBehaviorAttribute), true);

			if (attributes.Length != 0)
			{
				return (ComponentProxyBehaviorAttribute) attributes[0];
			}

			return null;
		}

		private static void ApplyProxyBehavior(ComponentProxyBehaviorAttribute behavior, ComponentModel model)
		{
			if (behavior.UseSingleInterfaceProxy || behavior.UseMarshalByRefProxy)
			{
				EnsureComponentRegisteredWithInterface(model);
			}

			ProxyOptions options = ProxyUtil.ObtainProxyOptions(model, true);

			options.UseSingleInterfaceProxy = behavior.UseSingleInterfaceProxy;
			options.UseMarshalByRefAsBaseClass = behavior.UseMarshalByRefProxy;
			options.AddAdditionalInterfaces(behavior.AdditionalInterfaces);
		}

		private static void EnsureComponentRegisteredWithInterface(ComponentModel model)
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