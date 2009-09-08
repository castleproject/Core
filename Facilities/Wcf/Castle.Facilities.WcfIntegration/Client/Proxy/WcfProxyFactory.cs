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

namespace Castle.Facilities.WcfIntegration.Proxy
{
	using System;
	using System.Runtime.Remoting;
	using System.ServiceModel;
	using Castle.Core;
	using Castle.Core.Interceptor;
	using Castle.DynamicProxy;
	using Castle.Facilities.WcfIntegration.Async;
	using Castle.Facilities.WcfIntegration.Async.TypeSystem;
	using Castle.Facilities.WcfIntegration.Internal;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Proxy;
	using Castle.Windsor.Proxy;

	public class WcfProxyFactory : AbstractProxyFactory
	{
		private readonly ProxyGenerator generator;
		private readonly WcfClientExtension clients;
		private AsyncType asyncType;

		public WcfProxyFactory(ProxyGenerator generator, WcfClientExtension clients)
		{
			this.generator = generator;
			this.clients = clients;
		}

		public override object Create(IKernel kernel, object instance, ComponentModel model, 
									  CreationContext context, params object[] constructorArguments)
		{
			var channelHolder = instance as IWcfChannelHolder;

			if (channelHolder == null)
			{
				throw new ArgumentException("Given instance is not an IWcfChannelHolder", "instance");
			}

			var isDuplex = IsDuplex(channelHolder.RealProxy);
			var proxyOptions = ProxyUtil.ObtainProxyOptions(model, true);
			var generationOptions = CreateProxyGenerationOptions(model.Service, proxyOptions);
			var additionalInterfaces = GetInterfaces(model.Service, proxyOptions, isDuplex);
			var interceptors = GetInterceptors(kernel, model, context);

			return generator.CreateInterfaceProxyWithTarget(typeof(IWcfChannelHolder),
				additionalInterfaces, channelHolder, generationOptions, interceptors);
		}

		public override bool RequiresTargetInstance(IKernel kernel, ComponentModel model)
		{
			return true;
		}

		protected bool IsDuplex(object realProxy)
		{
			var typeInfo = (IRemotingTypeInfo)realProxy;
			return typeInfo.CanCastTo(typeof(IDuplexContextChannel), null);
		}

		protected virtual Type[] GetInterfaces(Type service, ProxyOptions proxyOptions, bool isDuplex)
		{
			var additionalInterfaces = proxyOptions.AdditionalInterfaces ?? Type.EmptyTypes;
			Array.Resize(ref additionalInterfaces, additionalInterfaces.Length + (isDuplex ? 4 : 3));
			int index = additionalInterfaces.Length;
			additionalInterfaces[--index] = service;
			additionalInterfaces[--index] = typeof(IServiceChannel);
			additionalInterfaces[--index] = typeof(IClientChannel);

			if (isDuplex)
			{
				additionalInterfaces[--index] = typeof(IDuplexContextChannel);
			}

			return additionalInterfaces;
		}

		private IInterceptor[] GetInterceptors(IKernel kernel, ComponentModel model, CreationContext context)
		{
			var interceptors = ObtainInterceptors(kernel, model, context);
			var clientModel = (IWcfClientModel)model.ExtendedProperties[WcfConstants.ClientModelKey];
			Array.Resize(ref interceptors, interceptors.Length + (clientModel.WantsAsyncCapability ? 2 : 1));
			int index = interceptors.Length;

			interceptors[--index] = new WcfRemotingInterceptor(clients);

			if (clientModel.WantsAsyncCapability)
			{
				var getAsyncType = WcfUtils.SafeInitialize(ref asyncType,
					() => AsyncType.GetAsyncType(model.Service));
				interceptors[--index] = new WcfRemotingAsyncInterceptor(getAsyncType, clients);
			}

			return interceptors;
		}

		private ProxyGenerationOptions CreateProxyGenerationOptions(Type service, ProxyOptions proxyOptions)
		{
			if (proxyOptions.MixIns != null && proxyOptions.MixIns.Length > 0)
			{
				throw new NotImplementedException(
					"Support for mixins is not yet implemented. How about contributing a patch?");
			}

			var proxyGenOptions = new ProxyGenerationOptions(new WcfProxyGenerationHook(null))
			{
				Selector = new WcfInterceptorSelector(service, proxyOptions.Selector)
			};

			return proxyGenOptions;
		}
	}
}