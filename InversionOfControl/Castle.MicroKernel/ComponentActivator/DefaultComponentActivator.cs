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

namespace Castle.MicroKernel.ComponentActivator
{
	using System;
	using System.Reflection;

	using Castle.Model;

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
	public class DefaultComponentActivator : AbstractComponentActivator
	{
		public DefaultComponentActivator(ComponentModel model, IKernel kernel, 
			ComponentInstanceDelegate onCreation, 
			ComponentInstanceDelegate onDestruction) : base(model, kernel, onCreation, onDestruction)
		{
		}

		#region AbstractComponentActivator Members

		protected override sealed object InternalCreate()
		{
			object instance = Instantiate();

			SetUpProperties(instance);

			ApplyCommissionConcerns( instance );

			return instance;
		}

		protected override void InternalDestroy(object instance)
		{
			ApplyDecommissionConcerns( instance );
		}

		#endregion

		protected virtual object Instantiate()
		{
			ConstructorCandidate candidate = SelectEligibleConstructor();
	
			object[] arguments = CreateConstructorArguments( candidate );
	
			return CreateInstance(arguments);
		}

		protected virtual object CreateInstance(object[] arguments)
		{
			object instance;

			if (Model.Interceptors.HasInterceptors)
			{
				instance = Kernel.ProxyFactory.Create(Kernel, Model, arguments);
			}
			else
			{
				instance = Activator.CreateInstance(Model.Implementation, arguments);
			}
			
			return instance;
		}

		protected virtual void ApplyCommissionConcerns( object instance )
		{
			object[] steps = Model.LifecycleSteps.GetCommissionSteps();
			ApplyConcerns(steps, instance);
		}

		protected virtual void ApplyDecommissionConcerns( object instance )
		{
			object[] steps = Model.LifecycleSteps.GetDecommissionSteps();
			ApplyConcerns(steps, instance);
		}

		protected virtual void ApplyConcerns( object[] steps, object instance )
		{
			foreach (ILifecycleConcern concern in steps)
			{
				concern.Apply( Model, instance );
			}
		}

		protected virtual ConstructorCandidate SelectEligibleConstructor()
		{
			if (Model.Constructors.Count == 0)
			{
				// This is required by some facilities
				return null;
			}

			if (Model.Constructors.BestCandidate != null)
			{
				return Model.Constructors.BestCandidate;
			}

			if (Model.Constructors.Count == 1)
			{
				return Model.Constructors.FewerArgumentsCandidate;
			}

			ConstructorCandidate winnerCandidate = null; 

			foreach(ConstructorCandidate candidate in Model.Constructors)
			{
				foreach(DependencyModel dep in candidate.Dependencies)
				{
					if (CanSatisfyDependency(dep))
					{
						candidate.Points += 2;
					}
					else
					{
						candidate.Points -= 2;
					}
				}

				if (winnerCandidate == null) winnerCandidate = candidate;

				if (winnerCandidate.Points < candidate.Points)
				{
					winnerCandidate = candidate;
				}
			}

			if (winnerCandidate == null)
			{
				// What?
				throw new ComponentActivatorException("Could not find eligible constructor.");
			}

			Model.Constructors.BestCandidate = winnerCandidate;

			return winnerCandidate;
		}

		protected virtual bool CanSatisfyDependency(DependencyModel dep)
		{
			return Kernel.Resolver.CanResolve(Model, dep);
		}

		protected virtual object[] CreateConstructorArguments( 
			ConstructorCandidate constructor )
		{
			if (constructor == null) return new object[0];

			object[] arguments = new object[constructor.Constructor.GetParameters().Length];

			int index = 0;

			foreach(DependencyModel dependency in constructor.Dependencies)
			{
				object value = Kernel.Resolver.Resolve(Model, dependency);
				arguments[index++] = value;
			}

			return arguments;
		}

		protected virtual void SetUpProperties(object instance)
		{
			foreach(PropertySet property in Model.Properties)
			{
				object value = Kernel.Resolver.Resolve(Model, property.Dependency);
				if (value == null) continue;
				MethodInfo setMethod = property.Property.GetSetMethod();
				setMethod.Invoke( instance, new object[] { value } );
			}
		}
	}
}
