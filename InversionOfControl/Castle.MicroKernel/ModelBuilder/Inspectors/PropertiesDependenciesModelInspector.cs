// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

	using Castle.Model;

	/// <summary>
	/// This implementation of <see cref="IContributeComponentModelConstruction"/>
	/// collects all potential writable puplic properties exposed by the component 
	/// implementation and populates the model with them.
	/// The Kernel might be able to set some of these properties when the component 
	/// is requested.
	/// </summary>
	public class PropertiesDependenciesModelInspector : IContributeComponentModelConstruction
	{
		/// <summary>
		/// We don't need to have multiple instances
		/// </summary>
		private static readonly PropertiesDependenciesModelInspector instance = new PropertiesDependenciesModelInspector();

		/// <summary>
		/// Singleton instance
		/// </summary>
		public static PropertiesDependenciesModelInspector Instance
		{
			get { return instance; }
		}

		protected PropertiesDependenciesModelInspector()
		{
		}

		/// <summary>
		/// Adds the properties as optional dependencies of this component.
		/// </summary>
		/// <param name="kernel"></param>
		/// <param name="model"></param>
		public virtual void ProcessModel(IKernel kernel, ComponentModel model)
		{
			Type targetType = model.Implementation;

			PropertyInfo[] properties = targetType.GetProperties( 
				BindingFlags.Public|BindingFlags.Instance );

			foreach(PropertyInfo property in properties)
			{
				if (!property.CanWrite)
				{
					continue;
				}

				DependencyModel dependency = null;

				Type propertyType = property.PropertyType;

				// All these dependencies are simple guesses
				// So we make them optional (the 'true' parameter below)

				if (propertyType.IsPrimitive || propertyType == typeof(String) || propertyType == typeof(Type))
				{
					dependency = new DependencyModel(DependencyType.Parameter, property.Name, propertyType, true);
				}
				else if (propertyType.IsInterface || propertyType.IsClass)
				{
					dependency = new DependencyModel(DependencyType.Service, property.Name, propertyType, true);
				}
				else
				{
					continue;
				}

				model.Properties.Add( new PropertySet(property, dependency) );
			}
		}
	}
}
