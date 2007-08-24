// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework
{
	using System.Collections.Generic;
	using System.Web;
	using Castle.Core;

	/// <summary>
	/// Uses the HttpContext and the <see cref="IServiceProviderExAccessor"/> 
	/// to access the container instance.
	/// </summary>
	public class ServiceProviderLocator
	{
		private static readonly ServiceProviderLocator instance = new ServiceProviderLocator();
		
		private readonly IList<IAccessorStrategy> locatorStrategies = new List<IAccessorStrategy>();

		public ServiceProviderLocator()
		{
			AddLocatorStrategy(new ServiceProviderAccessorStrategy());
		}

		public static ServiceProviderLocator Instance
		{
			get { return instance; }
		}

		public IServiceProviderEx LocateProvider()
		{
			foreach(IAccessorStrategy strategy in locatorStrategies)
			{
				IServiceProviderEx serviceProvider = strategy.LocateProvider();
			
				if (serviceProvider != null)
				{
					return serviceProvider;
				}
			}

			return null;
		}

		public void AddLocatorStrategy(IAccessorStrategy strategy)
		{
			locatorStrategies.Add(strategy);
		}

		public interface IAccessorStrategy
		{
			IServiceProviderEx LocateProvider();
		}

		public class ServiceProviderAccessorStrategy : IAccessorStrategy
		{
			public IServiceProviderEx LocateProvider()
			{
				IServiceProviderExAccessor containerAccessor =
					HttpContext.Current.ApplicationInstance as IServiceProviderExAccessor;

				if (containerAccessor == null)
				{
					return null;
				}

				return containerAccessor.ServiceProvider;
			}
		}
	}
}
