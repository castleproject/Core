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

namespace Castle.MicroKernel.ComponentFactory
{
	using System;
	using System.Reflection;

	using Castle.Model;

	using Castle.MicroKernel.LifecycleConcerns;

	/// <summary>
	/// Summary description for DefaultComponentFactory.
	/// </summary>
	public class DefaultComponentFactory : IComponentFactory
	{
		private IKernel _kernel;
		private ComponentModel _model; 

		public DefaultComponentFactory(ComponentModel model, IKernel kernel)
		{
			_model = model;
			_kernel = kernel;
		}

		#region IComponentFactory Members

		public object Create()
		{
			ConstructorCandidate candidate = SelectEligibleConstructor();
			
			object[] arguments = CreateConstructorArguments( candidate );

			object instance = Activator.CreateInstance(_model.Implementation, arguments);

			SetUpProperties(instance);

			ApplyCommissionConcerns( instance );

			return instance;
		}

		public void Destroy(object instance)
		{
			ApplyDecommissionConcerns( instance );
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
