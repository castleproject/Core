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
	/// Summary description for PropertiesDependenciesModelInspection.
	/// </summary>
	public class PropertiesDependenciesModelInspector : IContributeComponentModelConstruction
	{
		private static readonly PropertiesDependenciesModelInspector instance = new PropertiesDependenciesModelInspector();

		public static PropertiesDependenciesModelInspector Instance
		{
			get { return instance; }
		}

		public PropertiesDependenciesModelInspector()
		{
		}

		#region IContributeComponentModelConstruction Members

		public void ProcessModel(IKernel kernel, ComponentModel model)
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

				Type propertyType = property.PropertyType;

				// Values types can be valid in some
				// contexts, but we'd better wait for the real need
				// before allowing them
				if (propertyType.IsPrimitive)
				{
					continue;
				}

				// All properties dependencies are simple
				// guesses. So we make them optional

				DependencyModel dependency = new DependencyModel(DependencyType.Service, property.Name, propertyType, true);
				model.Properties.Add( new PropertySet(property, dependency) );
			}
		}

		#endregion
	}
}
