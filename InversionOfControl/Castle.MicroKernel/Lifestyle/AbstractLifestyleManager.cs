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

namespace Castle.MicroKernel.Lifestyle
{
	using System;

	/// <summary>
	/// Summary description for AbstractLifestyleManager.
	/// </summary>
	[Serializable]
	public abstract class AbstractLifestyleManager : ILifestyleManager
	{
		private IKernel _kernel;
		private IComponentActivator _componentActivator;

		public virtual void Init(IComponentActivator componentActivator, IKernel kernel)
		{
			_componentActivator = componentActivator;
			_kernel = kernel;
		}

		public virtual object Resolve()
		{
			return _componentActivator.Create();
		}

		public virtual void Release(object instance)
		{
			_componentActivator.Destroy( instance );
		}	

		public abstract void Dispose();

		protected IKernel Kernel
		{
			get { return _kernel; }
		}

		protected IComponentActivator ComponentActivator
		{
			get { return _componentActivator; }
		}
	}
}
