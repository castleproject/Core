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

namespace Castle.Facilities.NHibernateIntegration
{
	using System;
	using System.Collections;
	using System.Reflection;

	using Castle.Model;
	
	using Castle.MicroKernel;
	using Castle.MicroKernel.ModelBuilder;
	using Castle.MicroKernel.Facilities;

	using Castle.Facilities.NHibernateExtension;
	

	public class AutomaticSessionInspector : IContributeComponentModelConstruction
	{
		public void ProcessModel(IKernel kernel, ComponentModel model)
		{
			if (model.Implementation.IsDefined( 
				typeof(UsesAutomaticSessionCreationAttribute), true ))
			{
				EnsureRelevantMethodsAreVirtual( model.Implementation );

				model.Dependencies.Add( 
					new DependencyModel( DependencyType.Service, null, typeof(AutomaticSessionInterceptor), false ) );

				model.Interceptors.Add( 
					new InterceptorReference( typeof(AutomaticSessionInterceptor) ) );
			}
		}

		private void EnsureRelevantMethodsAreVirtual(Type implementation)
		{
			MethodInfo[] methods = implementation.GetMethods( 
				BindingFlags.Instance|BindingFlags.Public|BindingFlags.DeclaredOnly );

			ArrayList problematicMethods = new ArrayList();

			foreach( MethodInfo method in methods )
			{
				if (!method.IsVirtual && method.IsDefined( typeof(SessionFlushAttribute), true ))
				{
					problematicMethods.Add( method.Name );
				}
			}
			
			if (problematicMethods.Count != 0)
			{
				String[] methodNames = (String[]) problematicMethods.ToArray( typeof(String) );

				String message = String.Format( "The class {0} wants to use transaction interception, " + 
					"however the methods must be marked as virtual in order to do so. Please correct " + 
					"the following methods: {1}", implementation.FullName, String.Join(", ", methodNames) );

				throw new FacilityException(message);
			}
		}
	}
}
