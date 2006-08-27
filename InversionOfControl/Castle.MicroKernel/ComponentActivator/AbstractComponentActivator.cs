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

	using Castle.Core;

	/// <summary>
	/// Abstract implementation of <see cref="IComponentActivator"/>.
	/// The implementors must only override the InternalCreate and 
	/// InternalDestroy methods in order to perform their creation and
	/// destruction logic.
	/// </summary>
	[Serializable]
	public abstract class AbstractComponentActivator : IComponentActivator
	{
		private IKernel kernel;
		private ComponentModel model; 
		private ComponentInstanceDelegate onCreation;
		private ComponentInstanceDelegate onDestruction;

		/// <summary>
		/// Constructs an AbstractComponentActivator
		/// </summary>
		public AbstractComponentActivator(ComponentModel model, IKernel kernel, 
			ComponentInstanceDelegate onCreation, 
			ComponentInstanceDelegate onDestruction)
		{
			this.model = model;
			this.kernel = kernel;
			this.onCreation = onCreation;
			this.onDestruction = onDestruction;
		}

		public IKernel Kernel
		{
			get { return kernel; }
		}

		public ComponentModel Model
		{
			get { return model; }
		}

		public ComponentInstanceDelegate OnCreation
		{
			get { return onCreation; }
		}

		public ComponentInstanceDelegate OnDestruction
		{
			get { return onDestruction; }
		}

		#region IComponentActivator Members

		public virtual object Create(CreationContext context)
		{
			object instance = InternalCreate(context);

			onCreation(model, instance);

			return instance;
		}

		public virtual void Destroy(object instance)
		{
			InternalDestroy(instance);

			onDestruction(model, instance);
		}

		#endregion

		protected abstract object InternalCreate(CreationContext context);

		protected abstract void InternalDestroy(object instance);
	}
}
