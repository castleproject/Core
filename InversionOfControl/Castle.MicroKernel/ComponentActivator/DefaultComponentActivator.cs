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
	/// Summary description for DefaultComponentActivator
	/// </summary>
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
			// TODO: Put the selection in a strategy 
			// so anyone can override this implementation with a better heuristic

			return Model.Constructors.FewerArgumentsCandidate;
		}

		protected virtual object[] CreateConstructorArguments( 
			ConstructorCandidate constructor )
		{
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
