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

namespace Castle.MicroKernel.ModelBuilder.Inspectors
{
	using System;
	using System.Reflection;

	using Castle.Core;
	using Castle.MicroKernel.SubSystems.Conversion;
	using Castle.Core.Configuration;

	/// <summary>
	/// This implementation of <see cref="IContributeComponentModelConstruction"/>
	/// collects all potential writable puplic properties exposed by the component 
	/// implementation and populates the model with them.
	/// The Kernel might be able to set some of these properties when the component 
	/// is requested.
	/// </summary>
	[Serializable]
	public class PropertiesDependenciesModelInspector : IContributeComponentModelConstruction
	{
		[NonSerialized]
		private IConversionManager converter;

		public PropertiesDependenciesModelInspector()
		{
		}

		/// <summary>
		/// Adds the properties as optional dependencies of this component.
		/// </summary>
		/// <param name="kernel"></param>
		/// <param name="model"></param>
		public virtual void ProcessModel(IKernel kernel, ComponentModel model)
		{
			if (converter == null)
			{
				converter = (IConversionManager) 
					kernel.GetSubSystem( SubSystemConstants.ConversionManagerKey );
			}

			InspectProperties(model);
		}

		protected virtual void InspectProperties(ComponentModel model)
		{
			if (model.InspectionBehavior == PropertiesInspectionBehavior.Undefined)
			{
				model.InspectionBehavior = GetInspectionBehaviorFromTheConfiguration(model.Configuration);
			}
			
			if (model.InspectionBehavior == PropertiesInspectionBehavior.None)
			{
				// Nothing to be inspected
				return;
			}
			
			BindingFlags bindingFlags;
			
			if (model.InspectionBehavior == PropertiesInspectionBehavior.DeclaredOnly)
			{
				bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
			}
			else // if (model.InspectionBehavior == PropertiesInspectionBehavior.All) or Undefined
			{
				bindingFlags = BindingFlags.Public | BindingFlags.Instance;
			}
			
			Type targetType = model.Implementation;
	
			PropertyInfo[] properties = targetType.GetProperties(bindingFlags);
	
			foreach(PropertyInfo property in properties)
			{
				if (!property.CanWrite)
				{
					continue;
				}

				ParameterInfo[] indexerParams = property.GetIndexParameters();

				if (indexerParams != null && indexerParams.Length != 0)
				{
					continue;
				}
				
				if (property.IsDefined(typeof(DoNotWireAttribute), true))
				{
					continue;
				}

				DependencyModel dependency;

				Type propertyType = property.PropertyType;

				// All these dependencies are simple guesses
				// So we make them optional (the 'true' parameter below)

				if ( converter.IsSupportedAndPrimitiveType(propertyType) )
				{
					dependency = new DependencyModel(DependencyType.Parameter, property.Name, propertyType, true);
				}
				else if (propertyType.IsInterface || propertyType.IsClass)
				{
					dependency = new DependencyModel(DependencyType.Service, property.Name, propertyType, true);
				}
				else
				{
					// What is it?!
					// Awkward type, probably.

					continue;
				}

				model.Properties.Add( new PropertySet(property, dependency) );
			}
		}

		private PropertiesInspectionBehavior GetInspectionBehaviorFromTheConfiguration(IConfiguration config)
		{
			if (config == null || config.Attributes["inspectionBehavior"] == null)
			{
				// return default behavior
				return PropertiesInspectionBehavior.All;
			}

			String enumStringVal = config.Attributes["inspectionBehavior"];

			try
			{
				return (PropertiesInspectionBehavior) 
					Enum.Parse(typeof(PropertiesInspectionBehavior), enumStringVal, true);
			}
			catch(Exception)
			{
				String[] enumNames = Enum.GetNames(typeof(PropertiesInspectionBehavior));
				
				String message = String.Format("Error on properties inspection. " + 
					"Could not convert the inspectionBehavior attribute value into an expected enum value. " + 
					"Value found is '{0}' while possible values are '{1}'", 
						enumStringVal, String.Join(",", enumNames));
				
				throw new KernelException(message);
			}
		}
	}
}