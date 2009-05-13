// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel.Lifestyle
{
	using System;
	using Castle.Core;

	/// <summary>
	/// Summary description for AbstractLifestyleManager.
	/// </summary>
	[Serializable]
	public abstract class AbstractLifestyleManager : ILifestyleManager
	{
		private IKernel kernel;
		private IComponentActivator componentActivator;
		private ComponentModel model;

		public virtual void Init(IComponentActivator componentActivator, IKernel kernel, ComponentModel model)
		{
			this.componentActivator = componentActivator;
			this.kernel = kernel;
			this.model = model;
		}

		public virtual object Resolve(CreationContext context)
		{
			return componentActivator.Create(context);
		}

		public virtual bool Release(object instance)
		{
			componentActivator.Destroy(instance);
			return true;
		}

		public abstract void Dispose();

		protected IKernel Kernel
		{
			get { return kernel; }
		}

		protected IComponentActivator ComponentActivator
		{
			get { return componentActivator; }
		}

		protected ComponentModel Model
		{
			get { return model; }
		}
	}
}