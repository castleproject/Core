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
	using Castle.Facilities.WcfIntegration.Internal;

	public class WcfClientResolver : ISubDependencyResolver
	{
		private readonly ICollection<WcfClientModel> clientModels;
		private readonly Dictionary<Type, CreateChannel> channelCreators;
		private readonly ReaderWriterLock locker;

		public WcfClientResolver(WcfClientModel[] clientModels)
		{
			this.clientModels = clientModels;
			channelCreators = new Dictionary<Type, CreateChannel>();
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
			WcfClientModel clientModel;
			Predicate<WcfClientModel> contractMatch = delegate(WcfClientModel candidate)
			{
				return contract == candidate.Contract;
			};

			// First, try the contexrt overrides.
			if (context != null && context.HasAdditionalParameters)
			{
				if (WcfUtils.FindDependency<WcfClientModel>(
						context.AdditionalParameters, contractMatch, 
						out clientModel))
				{
					return clientModel;
				}
			}

			// Then try the component overrides.
			if (model != null)
			{
				if (WcfUtils.FindDependency<WcfClientModel>(
						model.CustomDependencies, contractMatch, 
						out clientModel))
				{
					return clientModel;
				}
			}

			// Finally, try the client models.
			if (WcfUtils.FindDependency<WcfClientModel>(
					clientModels, contractMatch, out clientModel))
			{
				return clientModel;
			}
		
			return null;
		}

		private CreateChannel GetChannelBuilder(WcfClientModel clientModel)
		{
			CreateChannel channelCreator;

			locker.AcquireReaderLock(Timeout.Infinite);

			try
			{
				if (channelCreators.TryGetValue(clientModel.Contract, out channelCreator))
				{
					return channelCreator;
				}

				locker.UpgradeToWriterLock(Timeout.Infinite);

				if (channelCreators.TryGetValue(clientModel.Contract, out channelCreator))
				{
					return channelCreator;
				}

				channelCreator = new ChannelBuilder().GetChannelCreator(clientModel.Endpoint);
				channelCreators[clientModel.Contract] = channelCreator;
				return channelCreator;
			}
			finally
			{
				locker.ReleaseLock();
			}
		}
	}
}

