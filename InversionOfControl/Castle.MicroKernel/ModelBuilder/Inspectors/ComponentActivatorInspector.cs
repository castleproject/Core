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

	/// <summary>
	/// Inspects the component configuration and the type looking for a
	/// definition of component activator type. The configuration preceeds whatever
	/// is defined in the component.
	/// </summary>
	/// <remarks>
	/// This inspector is not guarantee to always set up an component activator type. 
	/// If nothing could be found it wont touch the model. In this case is up to
	/// the kernel to establish a default component activator for components.
	/// </remarks>
	[Serializable]
	public class ComponentActivatorInspector : IContributeComponentModelConstruction
	{
		/// <summary>
		/// Seaches for the component activator in the configuration and, if unsuccessful
		/// look for the component activator attribute in the implementation type.
		/// </summary>
		/// <param name="kernel">The kernel instance</param>
		/// <param name="model">The model instance</param>
		public virtual void ProcessModel(IKernel kernel, ComponentModel model)
		{
			if (!ReadComponentActivatorFromConfiguration(model))
			{
				ReadComponentActivatorFromType(model);
			}
		}

		/// <summary>
		/// Reads the attribute "componentActivatorType" associated with the 
		/// component configuration and verifies it implements the <see cref="IComponentActivator"/> 
		/// interface. 
		/// </summary>
		/// <exception cref="System.Configuration.ConfigurationException">
		/// If the type does not implement the proper interface
		/// </exception>
		/// <param name="model"></param>
		/// <returns></returns>
		protected virtual bool ReadComponentActivatorFromConfiguration(ComponentModel model)
		{
			if (model.Configuration != null)
			{
				string componentActivatorType = model.Configuration.Attributes["componentActivatorType"];

				if (componentActivatorType == null)
				{
					return false;
				}

				try
				{
					Type customComponentActivator = Type.GetType(componentActivatorType, true, false);

					ValidateComponentActivator(customComponentActivator);

					model.CustomComponentActivator = customComponentActivator;
				}
				catch(Exception ex)
				{
					string message =
						String.Format("The Type '{0}' specified  in the componentActivatorType attribute could not be loaded.",
						              componentActivatorType);

					throw new ConfigurationErrorsException(message, ex);
				}
			}

			return false;
		}

		/// <summary>
		/// Check if the type expose one of the component activator attributes
		/// defined in Castle.Core namespace.
		/// </summary>
		/// <param name="model"></param>
		protected virtual void ReadComponentActivatorFromType(ComponentModel model)
		{
			object[] attributes = model.Implementation.GetCustomAttributes(typeof(ComponentActivatorAttribute), true);

			if (attributes.Length != 0)
			{
				ComponentActivatorAttribute attribute = (ComponentActivatorAttribute) attributes[0];

				ValidateComponentActivator(attribute.ComponentActivatorType);

				model.CustomComponentActivator = attribute.ComponentActivatorType;
			}
		}

		/// <summary>
		/// Validates that the provide type implements IComponentActivator
		/// </summary>
		/// <param name="customComponentActivator">The custom component activator.</param>
		protected virtual void ValidateComponentActivator(Type customComponentActivator)
		{
			if (!typeof(IComponentActivator).IsAssignableFrom(customComponentActivator))
			{
				string message =
					String.Format(
						"The Type '{0}' specified  in the componentActivatorType attribute must implement Castle.MicroKernel.IComponentActivator",
						customComponentActivator.FullName);
				throw new InvalidOperationException(message);
			}
		}
	}
}