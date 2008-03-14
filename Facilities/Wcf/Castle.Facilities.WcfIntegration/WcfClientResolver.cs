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

namespace Castle.Facilities.WcfIntegration
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using Castle.Core;
	using Castle.MicroKernel;

	public class WcfClientResolver : ISubDependencyResolver
	{
		private readonly Dictionary<Type, CreateChannel> channelBuilders;
		private readonly ICollection<WcfClientModel> clientModels;
		private readonly ReaderWriterLock locker;

		public WcfClientResolver(WcfClientModel[] clientModels)
		{
			this.clientModels = clientModels;
			channelBuilders = new Dictionary<Type, CreateChannel>();
			locker = new ReaderWriterLock();
		}

		public bool CanResolve(CreationContext context, ISubDependencyResolver parentResolver,
							   ComponentModel model, DependencyModel dependency)
		{
			return ResolveClientModel(dependency.TargetType, model, context) != null;
		}

		public object Resolve(CreationContext context, ISubDependencyResolver parentResolver,
							  ComponentModel model, DependencyModel dependency)
		{
			WcfClientModel clientModel =
				ResolveClientModel(dependency.TargetType, model, context);

			if (clientModel != null)
			{
				return GetChannelBuilder(clientModel)();
			}

			throw new InvalidOperationException(string.Format(
				   "Could not find a client supporting contract {0}", dependency.TargetType));
		}

		private WcfClientModel ResolveClientModel(Type contract, ComponentModel model, CreationContext context)
		{
			// First, try the contexrt overrides.
			if (context.HasAdditionalParameters)
			{
				foreach (object dependency in context.AdditionalParameters.Values)
				{
					if (dependency is WcfClientModel)
					{
						WcfClientModel clientModel = (WcfClientModel)dependency;
						if (clientModel.Contract == contract)
						{
							return clientModel;
						}
					}
				}
			}

			// Then try the component overrides.
			if (model != null)
			{
				foreach (object dependency in model.CustomDependencies.Values)
				{
					if (dependency is WcfClientModel)
					{
						WcfClientModel clientModel = (WcfClientModel)dependency;
						if (clientModel.Contract == contract)
						{
							return clientModel;
						}
					}
				}
			}

			// Finally, try the client models.
			foreach (WcfClientModel clientModel in clientModels)
			{
				if (clientModel.Contract == contract)
				{
					return clientModel;
				}
			}

			return null;
		}

		private CreateChannel GetChannelBuilder(WcfClientModel clientModel)
		{
			CreateChannel channelBuilder;

			locker.AcquireReaderLock(Timeout.Infinite);

			try
			{
				if (channelBuilders.TryGetValue(clientModel.Contract, out channelBuilder))
				{
					return channelBuilder;
				}

				locker.UpgradeToWriterLock(Timeout.Infinite);

				if (channelBuilders.TryGetValue(clientModel.Contract, out channelBuilder))
				{
					return channelBuilder;
				}

				channelBuilder = clientModel.GetChannelBuilder();
				channelBuilders[clientModel.Contract] = channelBuilder;
				return channelBuilder;
			}
			finally
			{
				locker.ReleaseLock();
			}
		}
	}
}

