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
	public class DefaultComponentActivator : IComponentActivator
	{
		private IKernel _kernel;
		private ComponentModel _model; 
		private ComponentInstanceDelegate _onCreation;
		private ComponentInstanceDelegate _onDestruction;

		public DefaultComponentActivator(ComponentModel model, IKernel kernel, 
			ComponentInstanceDelegate onCreation, 
			ComponentInstanceDelegate onDestruction)
		{
			_model = model;
			_kernel = kernel;
			_onCreation = onCreation;
			_onDestruction = onDestruction;
		}

		#region IComponentActivator Members

		public object Create()
		{
			ConstructorCandidate candidate = SelectEligibleConstructor();
			
			object[] arguments = CreateConstructorArguments( candidate );

			object instance = null;

			if (_model.Interceptors.HasInterceptors)
			{
				instance = _kernel.ProxyFactory.Create(_model, arguments);
			}
			else
			{
				instance = Activator.CreateInstance(_model.Implementation, arguments);
			}

			SetUpProperties(instance);

			ApplyCommissionConcerns( instance );

			_onCreation(_model, instance);

			return instance;
		}

		public void Destroy(object instance)
		{
			ApplyDecommissionConcerns( instance );

			_onDestruction(_model, instance);
		}

		#endregion

		protected virtual void ApplyCommissionConcerns( object instance )
		{
			object[] steps = _model.LifecycleSteps.GetCommissionSteps();
			ApplyConcerns(steps, instance);
		}

		protected virtual void ApplyDecommissionConcerns( object instance )
		{
			object[] steps = _model.LifecycleSteps.GetDecommissionSteps();
			ApplyConcerns(steps, instance);
		}

		protected virtual void ApplyConcerns( object[] steps, object instance )
		{
			foreach (ILifecycleConcern concern in steps)
			{
				concern.Apply( _model, instance );
			}
		}

		protected virtual ConstructorCandidate SelectEligibleConstructor()
		{
			// TODO: Put the selection in a strategy 
			// so anyone can override this implementation with a better heuristic

			return _model.Constructors.FewerArgumentsCandidate;
		}

		protected virtual object[] CreateConstructorArguments( 
			ConstructorCandidate constructor )
		{
			object[] arguments = new object[constructor.Constructor.GetParameters().Length];

			int index = 0;

			foreach(DependencyModel dependency in constructor.Dependencies)
			{
				object value = _kernel.Resolver.Resolve(_model, dependency);
				arguments[index++] = value;
			}

			return arguments;
		}

		protected virtual void SetUpProperties(object instance)
		{
			foreach(PropertySet property in _model.Properties)
			{
				object value = _kernel.Resolver.Resolve(_model, property.Dependency);
				MethodInfo setMethod = property.Property.GetSetMethod();
				setMethod.Invoke( instance, new object[] { value } );
			}
		}
	}
}
