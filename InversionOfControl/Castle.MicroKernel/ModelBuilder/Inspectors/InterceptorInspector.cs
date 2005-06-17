// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

	using Castle.MicroKernel.Util;

	using Castle.Model;
	using Castle.Model.Configuration;

	/// <summary>
	/// Inspect the component for <c>InterceptorAttribute</c> and
	/// the configuration for the interceptors node
	/// </summary>
	[Serializable]
	public class InterceptorInspector : IContributeComponentModelConstruction
	{
		/// <summary>
		/// </summary>
		/// <param name="kernel"></param>
		/// <param name="model"></param>
		public virtual void ProcessModel(IKernel kernel, ComponentModel model)
		{
			CollectFromAttributes(model);
			CollectFromConfiguration(model);
		}

		protected virtual void CollectFromConfiguration(ComponentModel model)
		{
			if (model.Configuration == null) return;

			IConfiguration interceptors = model.Configuration.Children["interceptors"];

			if (interceptors == null) return;

			foreach(IConfiguration interceptor in interceptors.Children)
			{
				String value = interceptor.Value;

				if (!ReferenceExpressionUtil.IsReference(value))
				{
					String message = String.Format(
						"The value for the interceptor must be a reference " + 
						"to a component (Currently {0})", 
						value);

					throw new ConfigurationException(message);
				}

				InterceptorReference interceptorRef = 
					new InterceptorReference( ReferenceExpressionUtil.ExtractComponentKey(value) );
				
				model.Interceptors.Add(interceptorRef);
				model.Dependencies.Add( CreateDependencyModel(interceptorRef) );
			}
		}

		protected virtual void CollectFromAttributes(ComponentModel model)
		{
			if (!model.Implementation.IsDefined( typeof(InterceptorAttribute), true ))
			{
				return;
			}

			object[] attributes = model.Implementation.GetCustomAttributes(true);

			foreach(object attribute in attributes)
			{
				if (attribute is InterceptorAttribute)
				{
					InterceptorAttribute attr = (attribute as InterceptorAttribute);

					AddInterceptor( 
						attr.Interceptor, 
						model.Interceptors );

					model.Dependencies.Add( 
						CreateDependencyModel(attr.Interceptor) );
				}
			}
		}

		protected DependencyModel CreateDependencyModel(InterceptorReference interceptor)
		{
			return new DependencyModel(DependencyType.Service, interceptor.ComponentKey, 
				interceptor.ServiceType, false);
		}

		protected void AddInterceptor(InterceptorReference interceptorRef, InterceptorReferenceCollection interceptors)
		{
			interceptors.Add( interceptorRef );
		}
	}
}
