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

namespace Castle.MicroKernel.Resolvers
{
	using System;
	using System.Collections;

	using Castle.Core;

	using Castle.MicroKernel.Util;
	using Castle.MicroKernel.SubSystems.Conversion;

	/// <summary>
	/// Default implementation for <see cref="IDependencyResolver"/>.
	/// This implementation is quite simple, but still should be useful
	/// for 99% of situations. 
	/// </summary>
	[Serializable]
	public class DefaultDependencyResolver : IDependencyResolver
	{
		private readonly IKernel kernel;
		private readonly ITypeConverter converter;
		private DependencyDelegate dependencyResolvingDelegate;
		private IList subResolvers = new ArrayList();

		public DefaultDependencyResolver(IKernel kernel)
		{
			this.kernel = kernel;

			converter = (ITypeConverter) kernel.GetSubSystem(SubSystemConstants.ConversionManagerKey);
		}

		public void Initialize(DependencyDelegate dependencyDelegate)
		{
			dependencyResolvingDelegate = dependencyDelegate;
		}

		public void AddSubResolver(ISubDependencyResolver subResolver)
		{
			if (subResolver == null) throw new ArgumentNullException("subResolver");

			subResolvers.Add(subResolver);
		}

		public void RemoveSubResolver(ISubDependencyResolver subResolver)
		{
			if (subResolver == null) throw new ArgumentNullException("subResolver");

			subResolvers.Remove(subResolver);
		}

		/// <summary>
		/// Returns true if the resolver is able to satisfy the specified dependency.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="parentResolver"></param>
		/// <param name="model"></param>
		/// <param name="dependency"></param>
		/// <returns></returns>
		public bool CanResolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model, DependencyModel dependency)
		{
			// 1 - check for the dependency on CreationContext, if present

			if (context != null)
			{
				if (context.CanResolve(context, parentResolver, model, dependency))
				{
					return true;
				}
			}

			// 2 - check within parent resolver, if present

			if (parentResolver != null)
			{
				if (parentResolver.CanResolve(context, parentResolver, model, dependency))
				{
					return true;
				}
			}

			// 3 - check within subresolvers

			foreach(ISubDependencyResolver subResolver in subResolvers)
			{
				if (subResolver.CanResolve(context, parentResolver, model, dependency))
				{
					return true;
				}
			}

			// 4 - normal flow, checking against the kernel

			if (dependency.DependencyType == DependencyType.Service || 
			    dependency.DependencyType == DependencyType.ServiceOverride)
			{
				return CanResolveServiceDependency(model, dependency);
			}
			else
			{
				return CanResolveParameterDependency(model, dependency);
			}
		}

		/// <summary>
		/// Try to resolve the dependency by checking the parameters in 
		/// the model or checking the Kernel for the requested service.
		/// </summary>
		/// <remarks>
		/// The dependency resolver has the following precedence order:
		/// <list type="bullet">
		/// <item><description>
		/// The dependency is checked within the <see cref="CreationContext"/>
		/// </description></item>
		/// <item><description>
		/// The dependency is checked within the <see cref="IHandler"/> instance for the component
		/// </description></item>
		/// <item><description>
		/// The dependency is checked within the registered <see cref="ISubDependencyResolver"/>s
		/// </description></item>
		/// <item><description>
		/// Finally the resolver tries the normal flow 
		/// which is using the configuration
		/// or other component to satisfy the dependency
		/// </description></item>
		/// </list>
		/// </remarks>
		/// <param name="context"></param>
		/// <param name="parentResolver"></param>
		/// <param name="model"></param>
		/// <param name="dependency"></param>
		/// <returns></returns>
		public object Resolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model, DependencyModel dependency)
		{
			object value = null;

			bool resolved = false;

			// 1 - check for the dependency on CreationContext, if present

			if (context != null)
			{
				if (context.CanResolve(context, parentResolver, model, dependency))
				{
					value = context.Resolve(context, parentResolver, model, dependency);
					resolved = true;
				}
			}

			// 2 - check within parent resolver, if present

			if (!resolved && parentResolver != null)
			{
				if (parentResolver.CanResolve(context, parentResolver, model, dependency))
				{
					value = parentResolver.Resolve(context, parentResolver, model, dependency);
					resolved = true;
				}
			}

			// 3 - check within subresolvers

			if (!resolved)
			{
				foreach(ISubDependencyResolver subResolver in subResolvers)
				{
					if (subResolver.CanResolve(context, parentResolver, model, dependency))
					{
						value = subResolver.Resolve(context, parentResolver, model, dependency);
						resolved = true;
						break;
					}
				}
			}

			// 4 - normal flow, checking against the kernel

			if (!resolved)
			{
				if (dependency.DependencyType == DependencyType.Service ||
					dependency.DependencyType == DependencyType.ServiceOverride)
				{
					value = ResolveServiceDependency(context, model, dependency);
				}
				else
				{
					value = ResolveParameterDependency(context, model, dependency);
				}
			}

			if (value == null && !dependency.IsOptional)
			{
				String implementation = String.Empty;

				if (model.Implementation != null)
				{
					implementation = model.Implementation.FullName;
				}

				String message = String.Format(
					"Could not resolve non-optional dependency for '{0}' ({1}). Parameter '{2}' type '{3}'",
					model.Name, implementation, dependency.DependencyKey, dependency.TargetType.FullName);

				throw new DependencyResolverException(message);
			}

			RaiseDependencyResolving(model, dependency, value);

			return value;
		}

		protected virtual bool CanResolveServiceDependency(ComponentModel model, DependencyModel dependency)
		{
			ParameterModel parameter = ObtainParameterModelMatchingDependency(dependency, model);

			if (parameter != null)
			{
				// User wants to override

				String value = ExtractComponentKey(parameter.Value, parameter.Name);

				return HasComponentInValidState(value);
			}
			else if (dependency.TargetType == typeof(IKernel))
			{
				return true;
			}
			else
			{
				// Default behaviour

				if (dependency.TargetType != null)
				{
					return HasComponentInValidState(dependency.TargetType);
				}
				else
				{
					return HasComponentInValidState(dependency.DependencyKey);
				}
			}
		}

		protected virtual bool CanResolveParameterDependency(ComponentModel model, DependencyModel dependency)
		{
			ParameterModel parameter = ObtainParameterModelMatchingDependency(dependency, model);

			return parameter != null;
		}

		protected virtual object ResolveServiceDependency(CreationContext context, ComponentModel model, DependencyModel dependency)
		{
			IHandler handler;

			if (dependency.DependencyType == DependencyType.Service)
			{
				ParameterModel parameter = ObtainParameterModelMatchingDependency(dependency, model);

				if (parameter != null)
				{
					// User wants to override, we then 
					// change the type to ServiceOverride

					dependency.DependencyKey = ExtractComponentKey(parameter.Value, parameter.Name);
					dependency.DependencyType = DependencyType.ServiceOverride;
				}
			}

			if (dependency.TargetType == typeof(IKernel))
			{
				return kernel;
			}
			else
			{
				if (dependency.DependencyType == DependencyType.ServiceOverride)
				{
					handler = kernel.GetHandler(dependency.DependencyKey);
				}
				else
				{
					if (dependency.TargetType == model.Service)
					{
						throw new DependencyResolverException(
							"Cycle detected in configuration.\r\n" +
							"Component " + model.Name + " has a dependency on " +
							dependency.TargetType + ", but it doesn't provide an override.\r\n" +
							"You must provide an override if a component " +
							"has a dependency on a service that it - itself - provides");
					}

					handler = kernel.GetHandler(dependency.TargetType);
				}
			}

			if (handler == null) return null;
#if DOTNET2
			context = RebuildContextForParameter(context, dependency.TargetType);
#endif
			return handler.Resolve(context);
		}

		protected virtual object ResolveParameterDependency(CreationContext context, ComponentModel model, DependencyModel dependency)
		{
			ParameterModel parameter = ObtainParameterModelMatchingDependency(dependency, model);

			if (parameter != null)
			{
				converter.Context.PushModel(model);

				try
				{
					if (parameter.Value != null)
					{
						return converter.PerformConversion(parameter.Value, dependency.TargetType);
					}
					else
					{
						return converter.PerformConversion(parameter.ConfigValue, dependency.TargetType);
					}
				}
				finally
				{
					converter.Context.PopModel();
				}
			}

			return null;
		}

		protected virtual ParameterModel ObtainParameterModelMatchingDependency(DependencyModel dependency, ComponentModel model)
		{
			String key = dependency.DependencyKey;

			if (key == null) return null;

			return model.Parameters[key];
		}

		/// <summary>
		/// Extracts the component name from the a ref strings which is
		/// ${something}
		/// </summary>
		/// <param name="name"></param>
		/// <param name="keyValue"></param>
		/// <returns></returns>
		protected virtual String ExtractComponentKey(String keyValue, String name)
		{
			if (!ReferenceExpressionUtil.IsReference(keyValue))
			{
				throw new DependencyResolverException(
					String.Format("Key invalid for parameter {0}. " +
					"Thus the kernel was unable to override the service dependency", name));
			}

			return ReferenceExpressionUtil.ExtractComponentKey(keyValue);
		}

		private void RaiseDependencyResolving(ComponentModel model, DependencyModel dependency, object value)
		{
			dependencyResolvingDelegate(model, dependency, value);
		}

		private bool HasComponentInValidState(string key)
		{
			IHandler handler = kernel.GetHandler(key);

			return IsHandlerInValidState(handler);
		}

		private bool HasComponentInValidState(Type service)
		{
			IHandler handler = kernel.GetHandler(service);

			return IsHandlerInValidState(handler);
		}

		private static bool IsHandlerInValidState(IHandler handler)
		{
			if (handler != null)
			{
				return handler.CurrentState == HandlerState.Valid;
			}

			return false;
		}

#if DOTNET2
		
		/// <summary>
		/// This method rebuild the context for the parameter type.
		/// Naive implementation.
		/// </summary>
		private CreationContext RebuildContextForParameter(CreationContext current, Type parameterType)
		{
			if (parameterType.ContainsGenericParameters)
			{
				return current;
			}
			else
			{
				return new CreationContext(current.Handler, parameterType, current);
			}
		}
		
		#endif
	}
}
