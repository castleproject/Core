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
	/// collects all available constructors and populates them in the model
	/// as candidates. The Kernel will pick up one of the candidates
	/// according to a heuristic.
	/// </summary>
	public class ConstructorDependenciesModelInspector : IContributeComponentModelConstruction
	{
		/// <summary>
		/// We don't need to have multiple instances
		/// </summary>
		private static readonly ConstructorDependenciesModelInspector instance = new ConstructorDependenciesModelInspector();

		/// <summary>
		/// Singleton instance
		/// </summary>
		public static ConstructorDependenciesModelInspector Instance
		{
			get { return instance; }
		}

		protected ConstructorDependenciesModelInspector()
		{
		}

		public virtual void ProcessModel(IKernel kernel, ComponentModel model)
		{
			Type targetType = model.Implementation;

			ConstructorInfo[] constructors = 
				targetType.GetConstructors(BindingFlags.Public|BindingFlags.Instance);

			foreach(ConstructorInfo constructor in constructors)
			{
				// We register each public constructor
				// and let the ComponentFactory select an 
				// eligible amongst the candidates later

				model.Constructors.Add( CreateConstructorCandidate(constructor) );
			}
		}

		protected virtual ConstructorCandidate CreateConstructorCandidate( ConstructorInfo constructor )
		{
			ParameterInfo[] parameters = constructor.GetParameters();

			DependencyModel[] dependencies = new DependencyModel[parameters.Length];

			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo parameter = parameters[i];

				Type paramType = parameter.ParameterType;

				if (paramType.IsPrimitive || paramType == typeof(String))
				{
					dependencies[i] = new DependencyModel( 
						DependencyType.Parameter, parameter.Name, null, false );
				}
				else
				{
					dependencies[i] = new DependencyModel( 
						DependencyType.Service, parameter.Name, paramType, false );
				}
			}

			return new ConstructorCandidate( constructor, dependencies );
		}
	}
}
