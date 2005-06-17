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
	using System.Collections;

	using Castle.MicroKernel.Lifestyle.Pool;

	/// <summary>
	/// Implements a Poolable Lifestyle Manager. 
	/// </summary>
	[Serializable]
	public class PoolableLifestyleManager : AbstractLifestyleManager
	{
		private IPool _pool;
		private int _initialSize;
		private int _maxSize;

		public PoolableLifestyleManager(int initialSize, int maxSize)
		{
			_initialSize = initialSize;
			_maxSize = maxSize;
		}

		public override void Init(IComponentActivator componentActivator, IKernel kernel)
		{
			base.Init(componentActivator, kernel);

			_pool = InitPool(_initialSize, _maxSize);
		}

		protected IPool InitPool(int initialSize, int maxSize)
		{
			if (!Kernel.HasComponent( typeof(IPoolFactory) ))
			{
				Kernel.AddComponent("castle.internal.poolfactory", 
					typeof(IPoolFactory), typeof(DefaultPoolFactory));
			}

			IPoolFactory factory = Kernel[ typeof(IPoolFactory) ] as IPoolFactory;

			return factory.Create( initialSize, maxSize, ComponentActivator );
		}

		public override object Resolve()
		{
			return _pool.Request();
		}

		public override void Release(object instance)
		{
			_pool.Release(instance);
		}	

		public override void Dispose()
		{
			_pool.Dispose();
		}
	}
}
