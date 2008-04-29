// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.WindsorExtension
{
	using System;

	using Castle.MicroKernel;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Services;

	public class WindsorDynamicActionProviderFactory : DefaultDynamicActionProviderFactory
	{
		private readonly IKernel kernel;

		/// <summary>
		/// Initializes a new instance of the <see cref="WindsorDynamicActionProviderFactory"/> class.
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		public WindsorDynamicActionProviderFactory(IKernel kernel)
		{
			this.kernel = kernel;
		}

		/// <summary>
		/// Creates the specified dynamic action provider type.
		/// </summary>
		/// <param name="dynamicActionProviderType">Type of the dynamic action provider.</param>
		/// <returns></returns>
		public override IDynamicActionProvider Create(Type dynamicActionProviderType)
		{
			if (kernel.HasComponent(dynamicActionProviderType))
			{
				return (IDynamicActionProvider) kernel.Resolve(dynamicActionProviderType);
			}
			else
			{
				return base.Create(dynamicActionProviderType);
			}
		}

		/// <summary>
		/// Releases the specified dynamic action provider.
		/// </summary>
		/// <param name="dynamicActionProvider">The dynamic action provider.</param>
		public override void Release(IDynamicActionProvider dynamicActionProvider)
		{
			kernel.ReleaseComponent(dynamicActionProvider);

			base.Release(dynamicActionProvider);
		}
	}
}
