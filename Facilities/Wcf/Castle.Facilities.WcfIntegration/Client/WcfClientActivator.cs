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

namespace Castle.Facilities.WcfIntegration
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.ServiceModel;
	using System.Threading;
	using Castle.Core;
	using Castle.DynamicProxy;
	using Castle.Facilities.WcfIntegration.Internal;
	using Castle.Facilities.WcfIntegration.Proxy;
	using Castle.MicroKernel;
	using Castle.MicroKernel.ComponentActivator;
	using Castle.MicroKernel.Facilities;

	public class WcfClientActivator : DefaultComponentActivator
	{
		private WcfProxyFactory proxyFactory;
		private ChannelCreator createChannel;
		private IWcfBurden channelBurden;

		#region ClientChannelBuilder Delegate Fields

		private delegate ChannelCreator CreateChannelDelegate(
			IKernel kernel, IWcfClientModel clientModel, ComponentModel model, 
			out IWcfBurden burden);

		private static readonly MethodInfo createChannelMethod =
			typeof(WcfClientActivator).GetMethod("CreateChannelCreatorInternal",
				BindingFlags.NonPublic | BindingFlags.Static, null,
					new Type[] { typeof(IKernel), typeof(IWcfClientModel),
						typeof(ComponentModel), typeof(IWcfBurden).MakeByRefType() },
					null
				);

		private static readonly Dictionary<Type, CreateChannelDelegate>
			createChannelCache = new Dictionary<Type, CreateChannelDelegate>();

		private static ReaderWriterLock locker = new ReaderWriterLock();

		#endregion

		public WcfClientActivator(ComponentModel model, IKernel kernel,
			ComponentInstanceDelegate onCreation, ComponentInstanceDelegate onDestruction)
			: base(model, kernel, onCreation, onDestruction)
		{
			var clients = kernel.Resolve<WcfClientExtension>();
			proxyFactory = new WcfProxyFactory(new ProxyGenerator(), clients);
		}

		protected override object InternalCreate(CreationContext context)
		{
			object instance = Instantiate(context);
			ApplyCommissionConcerns(instance);
			return instance;
		}

		protected override object Instantiate(CreationContext context)
		{
			IWcfBurden burden;
			var channelCreator = GetChannelCreator(context, out burden);

			try
			{
				var channelHolder = new WcfChannelHolder(channelCreator, burden);
				return proxyFactory.Create(Kernel, channelHolder, Model, context);
			}
			catch (CommunicationException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new ComponentActivatorException("WcfClientActivator: could not proxy service " +
													  Model.Service.FullName, ex);
			}
		}

		/// <summary>
		/// Creates the channel creator.
		/// </summary>
		/// <param name="context">The context for the creator.</param>
		/// <param name="burden">Receives the channel burden.</param>
		/// <returns>The channel creator.</returns>
		/// <remarks>
		/// Always Open the channel before being used to prevent serialization of requests.
		/// http://blogs.msdn.com/wenlong/archive/2007/10/26/best-practice-always-open-wcf-client-proxy-explicitly-when-it-is-shared.aspx 
		/// </remarks>
		private ChannelCreator GetChannelCreator(CreationContext context, out IWcfBurden burden)
		{
			burden = channelBurden;
			ChannelCreator creator = createChannel;
			IWcfClientModel clientModel = ObtainClientModel(Model, context);

			if (clientModel != null)
			{
				var inner = CreateChannelCreator(Kernel, Model, clientModel, out burden);
				var scopedBurden = burden;

				creator = () =>
				{
					var client = (IClientChannel)inner();
					client.Open();
					client.Extensions.Add(new WcfBurdenExtension<IContextChannel>(scopedBurden));
					return client;
				};
			}
			else if (createChannel == null)
			{
				clientModel = ObtainClientModel(Model);
				var inner = CreateChannelCreator(Kernel, Model, clientModel, out channelBurden);
				creator = createChannel = () =>
				{
					var client = (IClientChannel)inner();
					client.Open();
					return client;
				};
				Model.ExtendedProperties[WcfConstants.ClientBurdenKey] = burden = channelBurden;
			}

			return creator;
		}

		private IWcfClientModel ObtainClientModel(ComponentModel model)
		{
			return (IWcfClientModel)model.ExtendedProperties[WcfConstants.ClientModelKey];
		}

		private IWcfClientModel ObtainClientModel(ComponentModel model, CreationContext context)
		{
			var clientModel = WcfUtils.FindDependencies<IWcfClientModel>(context.AdditionalParameters)
				.FirstOrDefault();

			var endpoint = WcfUtils.FindDependencies<IWcfEndpoint>(context.AdditionalParameters)
				.FirstOrDefault();

			if (endpoint != null)
			{
				if (clientModel == null)
					clientModel = ObtainClientModel(model);

				clientModel = clientModel.ForEndpoint(endpoint);
			}

			return clientModel;
		}

		private void ValidateClientModel(IWcfClientModel clientModel, ComponentModel model)
		{
			Type contract;

			if (model != null)
			{
				contract = model.Service;
			}
			else if (clientModel.Contract != null)
			{
				contract = clientModel.Contract;
			}
			else
			{
				throw new FacilityException(
					"The client endpoint does not specify a contract.");
			}

			if (clientModel.Endpoint == null)
			{
				throw new FacilityException("The client model requires an endpoint.");
			}

			if ((model != null) && (clientModel.Contract != null) &&
				(clientModel.Contract != model.Service))
			{
				throw new FacilityException("The client endpoint contract " +
					clientModel.Contract.FullName + " conflicts with the expected contaxt" +
					model.Service.FullName + ".");
			}
		}

		private ChannelCreator CreateChannelCreator(IKernel kernel, ComponentModel model,
													IWcfClientModel clientModel, out IWcfBurden burden)
		{
			ValidateClientModel(clientModel, model);

			CreateChannelDelegate createChannelDelegate;

			try
			{
				locker.AcquireReaderLock(Timeout.Infinite);

				Type clientModelType = clientModel.GetType();

				if (!createChannelCache.TryGetValue(clientModelType, out createChannelDelegate))
				{
					locker.UpgradeToWriterLock(Timeout.Infinite);

					if (!createChannelCache.TryGetValue(clientModelType, out createChannelDelegate))
					{
						createChannelDelegate = (CreateChannelDelegate)
							Delegate.CreateDelegate(typeof(CreateChannelDelegate),
								createChannelMethod.MakeGenericMethod(clientModelType));
						createChannelCache.Add(clientModelType, createChannelDelegate);
					}
				}
			}
			finally
			{
				locker.ReleaseLock();
			}

			return createChannelDelegate(kernel, clientModel, model, out burden);
		}

		private static ChannelCreator CreateChannelCreatorInternal<M>(
			IKernel kernel, IWcfClientModel clientModel, ComponentModel model, out IWcfBurden burden) 
			where M : IWcfClientModel
		{
			var channelBuilder = kernel.Resolve<IClientChannelBuilder<M>>();
			return channelBuilder.GetChannelCreator((M) clientModel, model.Service, out burden);
		}
	}
}
