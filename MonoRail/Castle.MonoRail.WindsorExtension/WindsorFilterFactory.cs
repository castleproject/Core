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

namespace Castle.MonoRail.WindsorExtension
{
	using System;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Services;
	using Castle.Windsor;

	/// <summary>
	/// Custom implementation of <see cref="IFilterFactory"/>
	/// that uses the WindsorContainer to obtain <see cref="IFilter"/>
	/// instances, and, if not available, uses the default implementation
	/// of <see cref="IFilterFactory"/>.
	/// </summary>
	public class WindsorFilterFactory : DefaultFilterFactory
	{
		public override IFilter Create(Type filterType)
		{
			IWindsorContainer container = ContainerAccessorUtil.ObtainContainer();

			if (container.Kernel.HasComponent(filterType))
			{
				return (IFilter) container[filterType];
			}
			else
			{
				return base.Create(filterType);
			}
		}

		public override void Release(IFilter filter)
		{
			IWindsorContainer container = ContainerAccessorUtil.ObtainContainer();

			container.Release(filter);

			base.Release(filter);
		}
	}
}
