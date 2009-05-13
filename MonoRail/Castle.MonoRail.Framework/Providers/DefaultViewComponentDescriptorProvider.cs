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

namespace Castle.MonoRail.Framework.Providers
{
	using System;
	using System.Threading;
	using System.Collections.Generic;
	using Castle.MonoRail.Framework;
	using Descriptors;
	using ViewComponents;

	/// <summary>
	/// Creates <see cref="ViewComponentDescriptor"/> from attributes 
	/// associated with the <see cref="ViewComponent"/>
	/// </summary>
	public class DefaultViewComponentDescriptorProvider : IViewComponentDescriptorProvider
	{
		private readonly IDictionary<Type, ViewComponentDescriptor> type2Desc =
			new Dictionary<Type, ViewComponentDescriptor>();

		private static readonly ReaderWriterLock locker = new ReaderWriterLock();

		/// <summary>
		/// Services the specified provider.
		/// </summary>
		/// <param name="provider">The provider.</param>
		public void Service(IMonoRailServices provider)
		{
		}

		/// <summary>
		/// Creates a <see cref="ViewComponentDescriptor"/> by inspecting the
		/// specified view component type.
		/// </summary>
		/// <param name="viewComponentType">Type of the view component.</param>
		/// <returns></returns>
		public ViewComponentDescriptor Collect(Type viewComponentType)
		{
			if (viewComponentType == null) throw new ArgumentNullException("viewComponentType");

			locker.AcquireReaderLock(-1);
			try
			{
				if (type2Desc.ContainsKey(viewComponentType))
				{
					return type2Desc[viewComponentType];
				}
			}
			finally
			{
				locker.ReleaseReaderLock();
			}

			object[] attrs = viewComponentType.GetCustomAttributes(typeof (ViewComponentDetailsAttribute), true);

			ViewComponentDescriptor descriptor;

			if (attrs.Length == 0)
			{
				descriptor = ViewComponentDescriptor.Empty;
			}
			else
			{
				ViewComponentDetailsAttribute details = (ViewComponentDetailsAttribute) attrs[0];

				IViewComponentCacheKeyGenerator generator = null;

				if (details.Cache == ViewComponentCache.Always)
				{
					generator = new AlwaysCacheKeyGenerator();
				}
				else if (details.CacheKeyFactory != null)
				{
					try
					{
						generator = (IViewComponentCacheKeyGenerator)
						            Activator.CreateInstance(details.CacheKeyFactory);
					}
					catch (Exception ex)
					{
						throw new MonoRailException(
							"Could not instantiate IViewComponentCacheKeyGenerator implementation or " +
							"it does not implement this interface. Type: " + details.CacheKeyFactory.FullName, ex);
					}
				}

				descriptor = new ViewComponentDescriptor(details.Cache != ViewComponentCache.Disabled, details.Cache, generator);
			}

			locker.UpgradeToWriterLock(-1);
			try
			{
				type2Desc[viewComponentType] = descriptor;
			}
			finally
			{
				locker.ReleaseWriterLock();
			}

			return descriptor;
		}
	}
}