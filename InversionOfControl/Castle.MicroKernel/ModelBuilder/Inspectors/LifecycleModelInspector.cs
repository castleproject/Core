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

	using Castle.Model;

	using Castle.MicroKernel.LifecycleConcerns;

	/// <summary>
	/// Inspects the type looking for interfaces that constitutes
	/// lifecycle interfaces, defined in the Castle.Model namespace.
	/// </summary>
	public class LifecycleModelInspector : IContributeComponentModelConstruction
	{
		public LifecycleModelInspector()
		{
		}

		/// <summary>
		/// Checks if the type implements <see cref="IInitialize"/> and or
		/// <see cref="IDisposable"/> interfaces.
		/// </summary>
		/// <param name="kernel"></param>
		/// <param name="model"></param>
		public virtual void ProcessModel(IKernel kernel, ComponentModel model)
		{
			if (typeof (IInitializable).IsAssignableFrom(model.Implementation))
			{
				model.LifecycleSteps.Add( LifecycleStepType.Commission, InitializationConcern.Instance );
			}
			if (typeof (IDisposable).IsAssignableFrom(model.Implementation))
			{
				model.LifecycleSteps.Add( LifecycleStepType.Decommission, DisposalConcern.Instance );
			}
		}
	}
}