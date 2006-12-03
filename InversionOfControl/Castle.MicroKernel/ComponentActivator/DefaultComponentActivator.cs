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

namespace Castle.MicroKernel.ComponentActivator
{
	using System;
	using System.Reflection;

	using Castle.Core;
	using Castle.Core.Interceptor;
	using Castle.MicroKernel.LifecycleConcerns;

	/// <summary>
	/// Standard implementation of <see cref="IComponentActivator"/>.
	/// Handles the selection of the best constructor, fills the
	/// writable properties the component exposes, run the commission 
	/// and decommission lifecycles, etc.
	/// </summary>
	/// <remarks>
	/// Custom implementors can just override the <c>CreateInstance</c> method.
	/// Please note however that the activator is responsible for the proxy creation
	/// when needed.
	/// </remarks>
	[Serializable]
	public class DefaultComponentActivator : AbstractComponentActivator
	{
		public DefaultComponentActivator(ComponentModel model, IKernel kernel, 
			ComponentInstanceDelegate onCreation, 
			ComponentInstanceDelegate onDestruction) : base(model, kernel, onCreation, onDestruction)
		{
		}

		#region AbstractComponentActivator Members

		protected override sealed object InternalCreate(CreationContext context)
		{
			object instance = Instantiate(context);

			SetUpProperties(instance, context);

			ApplyCommissionConcerns(instance);

			return instance;
		}

		protected override void InternalDestroy(object instance)
		{
			ApplyDecommissionConcerns( instance );
		}

		#endregion

		protected virtual object Instantiate(CreationContext context)
		{
			ConstructorCandidate candidate = SelectEligibleConstructor(context);
	
			Type[] signature;
			object[] arguments = CreateConstructorArguments(candidate, context, out signature);

			return CreateInstance(context, arguments, signature);
		}

		protected virtual object CreateInstance(CreationContext context, object[] arguments, Type[] signature)
		{
			object instance = null;

			Type implType = Model.Implementation;

			bool createProxy = Model.Interceptors.HasInterceptors;
			bool createInstance = true;
			
			if (createProxy)
			{
				createInstance = Kernel.ProxyFactory.RequiresTargetInstance(Kernel, Model);
			}

			if (createInstance)
			{
				try
				{
					ConstructorInfo cinfo = implType.GetConstructor(
							BindingFlags.Public | BindingFlags.Instance, null, signature, null);

					instance = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(implType);

					cinfo.Invoke(instance, arguments);
				}
				catch (Exception ex)
				{
					throw new ComponentActivatorException("ComponentActivator: could not instantiate " + Model.Implementation.FullName, ex);
				}
			}

			if (createProxy)
			{
				try
				{
					instance = Kernel.ProxyFactory.Create(Kernel, instance, Model, arguments);
				}
				catch(Exception ex)
				{
					throw new ComponentActivatorException("ComponentActivator: could not proxy " + Model.Implementation.FullName, ex);
				}
			}
			
			return instance;
		}

		protected virtual void ApplyCommissionConcerns(object instance)
		{
			instance = GetUnproxiedInstance(instance);
			object[] steps = Model.LifecycleSteps.GetCommissionSteps();
			ApplyConcerns(steps, instance);
		}

		protected virtual void ApplyDecommissionConcerns(object instance)
		{
			instance = GetUnproxiedInstance(instance);
			object[] steps = Model.LifecycleSteps.GetDecommissionSteps();
			ApplyConcerns(steps, instance);
		}

		protected virtual void ApplyConcerns(object[] steps, object instance)
		{
			foreach(ILifecycleConcern concern in steps)
			{
				concern.Apply( Model, instance );
			}
		}

		protected virtual ConstructorCandidate SelectEligibleConstructor(CreationContext context)
		{
			if (Model.Constructors.Count == 0)
			{
				// This is required by some facilities
				return null;
			}

			if (Model.Constructors.Count == 1)
			{
				return Model.Constructors.FewerArgumentsCandidate;
			}

			ConstructorCandidate winnerCandidate = null;

			int winnerPoints = 0;
			
			foreach(ConstructorCandidate candidate in Model.Constructors)
			{
				int candidatePoints = 0;
				
				foreach(DependencyModel dep in candidate.Dependencies)
				{
					if (CanSatisfyDependency(context, dep))
					{
						candidatePoints += 2;
					}
					else
					{
						candidatePoints -= 2;
					}
				}

				if (winnerCandidate == null) winnerCandidate = candidate;

				if (winnerPoints < candidatePoints)
				{
					winnerCandidate = candidate;
					winnerPoints = candidatePoints;
				}
			}

			if (winnerCandidate == null)
			{
				throw new ComponentActivatorException("Could not find eligible constructor for " + Model.Implementation.FullName);
			}

			return winnerCandidate;
		}

		protected virtual bool CanSatisfyDependency(CreationContext context, DependencyModel dep)
		{
			return Kernel.Resolver.CanResolve(context, context.Handler, Model, dep);
		}

		protected virtual object[] CreateConstructorArguments(
			ConstructorCandidate constructor, CreationContext context, out Type[] signature)
		{
			signature = null;

			if (constructor == null) return new object[0];

			object[] arguments = new object[constructor.Constructor.GetParameters().Length];
			signature = new Type[arguments.Length];

			int index = 0;

			foreach(DependencyModel dependency in constructor.Dependencies)
			{
				object value;
				using (new DependencyTrackingScope(context, constructor.Constructor, dependency))
				{
					value = Kernel.Resolver.Resolve(context, context.Handler, Model, dependency);
				}
				arguments[index] = value;
				signature[index++] = dependency.TargetType;
			}

			return arguments;
		}

		protected virtual void SetUpProperties(object instance, CreationContext context)
		{
			instance = GetUnproxiedInstance(instance);

			foreach(PropertySet property in Model.Properties)
			{
				object value;
				using (new DependencyTrackingScope(context, property.Property, property.Dependency))
				{
					value = Kernel.Resolver.Resolve(context, context.Handler, Model, property.Dependency);
				}

				if (value == null) continue;

				MethodInfo setMethod = property.Property.GetSetMethod();

				try
				{
					setMethod.Invoke(instance, new object[] { value });
				}
				catch(Exception ex)
				{
					String message = String.Format("Error setting property {0} on type {1}, Component id is {2}. See inner exception for more information.", 
					                               setMethod.Name, instance.GetType().FullName, Model.Name);
					throw new ComponentActivatorException(message, ex);
				}
			}
		}

		private static object GetUnproxiedInstance(object instance)
		{
			IProxyTargetAccessor accessor = instance as IProxyTargetAccessor;

			if (accessor != null)
			{
				instance = accessor.DynProxyGetTarget();
			}
			
			return instance;
		}
	}

	internal class DependencyTrackingScope : IDisposable
	{
		private readonly CreationContext creationContext;
		private readonly DependencyModel dependencyTrackingKey;

		public DependencyTrackingScope(CreationContext creationContext, MemberInfo memberInfo, DependencyModel dependency)
		{
			this.creationContext = creationContext;

			if (dependency.TargetType != typeof (IKernel))
			{
				// We track dependencies in order to detect cycled graphs
				// This prevents a stack overflow
				dependencyTrackingKey = creationContext.TrackDependency(memberInfo, dependency);
			}
		}

		public void Dispose()
		{
			// The dependency was resolved successfully, we can stop tracking it.
			if (dependencyTrackingKey != null)
				creationContext.UntrackDependency(dependencyTrackingKey);
		}
	}
}