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

namespace Castle.MicroKernel.Resolvers
{
	using System;

	using Castle.Model;

	using Castle.MicroKernel.SubSystems.Conversion;
	using Castle.MicroKernel.Util;

	/// <summary>
	/// Default implementation for <see cref="IDependencyResolver"/>.
	/// This implementation is quite simple, but still should be useful
	/// for 99% of situations. 
	/// </summary>
	public class DefaultDependecyResolver : IDependencyResolver
	{
		private readonly IKernel _kernel;
		private readonly ITypeConverter _converter;

		public DefaultDependecyResolver(IKernel kernel)
		{
			_kernel = kernel;

			_converter = (ITypeConverter) 
				_kernel.GetSubSystem( SubSystemConstants.ConversionManagerKey );
		}

		/// <summary>
		/// Try to resolve the dependency by checking the parameters in 
		/// the model or checking the Kernel for the requested service.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="dependency"></param>
		/// <returns></returns>
		public object Resolve(ComponentModel model, DependencyModel dependency)
		{
			object value = null;

			if(dependency.DependencyType == DependencyType.Service)
			{
				value = ResolveServiceDependency( model, dependency );
			}
			else
			{
				value = ResolveParameterDependency( model, dependency );
			}

			if (value == null && !dependency.IsOptional)
			{
				String message = String.Format(
					"Could not resolve non-optional dependency for {0}. Parameter '{1}' type {2}", 
					model.Name, dependency.DependencyKey, dependency.TargetType.Name);

				throw new DependecyResolverException(message);	
			}

			return value;
		}

		/// <summary>
		/// Returns true if the resolver is able to satisfy this dependency.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="dependency"></param>
		/// <returns></returns>
		public bool CanResolve(ComponentModel model, DependencyModel dependency)
		{
			if(dependency.DependencyType == DependencyType.Service)
			{
				return CanResolveServiceDependency( model, dependency );
			}
			else
			{
				return CanResolveParameterDependency( model, dependency );
			}
		}

		protected virtual bool CanResolveServiceDependency(ComponentModel model, DependencyModel dependency)
		{
			ParameterModel parameter = ObtainParameterModelMatchingDependency(dependency, model);

			if (parameter != null)
			{
				// User wants to override

				String value = ExtractComponentKey( parameter.Value, parameter.Name );

				return _kernel.HasComponent( value );
			}
			else if (dependency.TargetType == typeof(IKernel))
			{
				return true;
			}
			else
			{
				// Default behaviour

				return _kernel.HasComponent( dependency.TargetType );
			}
		}

		protected virtual bool CanResolveParameterDependency(ComponentModel model, DependencyModel dependency)
		{
			ParameterModel parameter = ObtainParameterModelMatchingDependency(dependency, model);

			return parameter != null;
		}

		protected virtual object ResolveServiceDependency(ComponentModel model, DependencyModel dependency)
		{
			IHandler handler = null;

			ParameterModel parameter = ObtainParameterModelMatchingDependency(dependency, model);

			if (parameter != null)
			{
				// User wants to override

				String value = ExtractComponentKey( parameter.Value, parameter.Name );

				handler = _kernel.GetHandler( value );
			}
			else if (dependency.TargetType == typeof(IKernel))
			{
				return _kernel;
			}
			else
			{
				// Default behaviour

				handler = _kernel.GetHandler( dependency.TargetType );
			}

			if (handler == null) return null;

			return handler.Resolve();
		}

		protected virtual object ResolveParameterDependency(ComponentModel model, DependencyModel dependency)
		{
			ParameterModel parameter = ObtainParameterModelMatchingDependency(dependency, model);

			if (parameter != null)
			{
				if (parameter.Value != null)
				{
					return _converter.PerformConversion( parameter.Value, dependency.TargetType );
				}
				else
				{
					return _converter.PerformConversion( parameter.ConfigValue, dependency.TargetType );
				}
			}

			return null;
		}

		protected virtual ParameterModel ObtainParameterModelMatchingDependency(DependencyModel dependency, ComponentModel model)
		{
			String key = dependency.DependencyKey;

			return model.Parameters[ key ];
		}

		/// <summary>
		/// Extracts the component name from the a ref strings which is
		/// #{something}
		/// </summary>
		/// <param name="keyValue"></param>
		/// <returns></returns>
		protected virtual String ExtractComponentKey(String keyValue, String name)
		{
			if (!ReferenceExpressionUtil.IsReference(keyValue))
			{
				throw new DependecyResolverException( 
					String.Format("Key invalid for parameter {0}. " + 
					"Thus the kernel was unable to override the service dependency", name) );
			}

			return ReferenceExpressionUtil.ExtractComponentKey(keyValue);
		}
	}
}
