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

namespace Castle.MicroKernel.ComponentActivator
{
	using System;

	using Castle.Model;

	/// <summary>
	/// Abstract implementation of <see cref="IComponentActivator"/>.
	/// The implementors must only override the InternalCreate and 
	/// InternalDestroy methods in order to perform their creation and
	/// destruction logic.
	/// </summary>
	[Serializable]
	public abstract class AbstractComponentActivator : IComponentActivator
	{
		private IKernel _kernel;
		private ComponentModel _model; 
		private ComponentInstanceDelegate _onCreation;
		private ComponentInstanceDelegate _onDestruction;

		/// <summary>
		/// Constructs an AbstractComponentActivator
		/// </summary>
		/// <param name="model"></param>
		/// <param name="kernel"></param>
		/// <param name="onCreation"></param>
		/// <param name="onDestruction"></param>
		public AbstractComponentActivator(ComponentModel model, IKernel kernel, 
			ComponentInstanceDelegate onCreation, 
			ComponentInstanceDelegate onDestruction)
		{
			_model = model;
			_kernel = kernel;
			_onCreation = onCreation;
			_onDestruction = onDestruction;
		}

		public IKernel Kernel
		{
			get { return _kernel; }
		}

		public ComponentModel Model
		{
			get { return _model; }
		}

		public ComponentInstanceDelegate OnCreation
		{
			get { return _onCreation; }
		}

		public ComponentInstanceDelegate OnDestruction
		{
			get { return _onDestruction; }
		}

		protected abstract object InternalCreate();

		protected abstract void InternalDestroy(object instance);

		#region IComponentActivator Members

		public virtual object Create()
		{
			object instance = InternalCreate();

			_onCreation(_model, instance);

			return instance;
		}

		public virtual void Destroy(object instance)
		{
			InternalDestroy(instance);

			_onDestruction(_model, instance);
		}

		#endregion
	}
}
