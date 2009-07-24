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

namespace Castle.Facilities.WcfIntegration.Async
{
	using System;
	using System.Reflection;
	using System.ServiceModel;
	using System.ServiceModel.Description;
	using Castle.Core.Interceptor;
	using Castle.DynamicProxy;
	using Castle.Facilities.WcfIntegration.Async.TypeSystem;

	public class AsynChannelFactoryBuilder<M> : DefaultChannelFactoryBuilder<M>
		where M : IWcfClientModel
	{
		private readonly ProxyGenerator _generator;

		public AsynChannelFactoryBuilder()
		{
			_generator = new ProxyGenerator();
		}

		public override ChannelFactory CreateChannelFactory(Type channelFactoryType, M clientModel, 
															params object[] constructorArgs)
		{
			if (!clientModel.WantsAsyncCapability)
			{
				return base.CreateChannelFactory(channelFactoryType, clientModel, constructorArgs);
			}

			EnsureValidChannelFactoryType(channelFactoryType);

			ReplaceServiceEndpointAsyncContracts(constructorArgs);

			var interceptor = new CreateDescriptionInterceptor();
			var proxyOptions = new ProxyGenerationOptions(new AsyncChannelFactoryProxyHook());
			return (ChannelFactory)_generator.CreateClassProxy(
				channelFactoryType, Type.EmptyTypes, proxyOptions, constructorArgs, interceptor);
		}

		private void ReplaceServiceEndpointAsyncContracts(object[] constructorArgs)
		{
			for (int i = 0; i < constructorArgs.Length; ++i)
			{
				var endpoint = constructorArgs[i] as ServiceEndpoint;
				if (endpoint != null)
				{
					var asyncEndpoint = new ServiceEndpoint(ContractDescription.GetContract(
						AsyncType.GetAsyncType(endpoint.Contract.ContractType)))
					{
						Name = endpoint.Name,
						Address = endpoint.Address,
						Binding = endpoint.Binding,
						ListenUri = endpoint.ListenUri,
						ListenUriMode = endpoint.ListenUriMode
					};

					asyncEndpoint.Behaviors.Clear();
					foreach (var behavior in endpoint.Behaviors)
					{
						asyncEndpoint.Behaviors.Add(behavior);	
					}

					constructorArgs[i] = asyncEndpoint;
				}
			}
		}
	}

	#region Class AsyncChannelFactoryProxyHook
	
	class AsyncChannelFactoryProxyHook : IProxyGenerationHook
	{
		public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
		{
			if (methodInfo.Name == "CreateDescription")
			{
				return true;
			}
			return false;
		}

		public void NonVirtualMemberNotification(Type type, MemberInfo memberInfo)
		{
		}

		public void MethodsInspected()
		{
		}
	}

	#endregion

	#region Class CreateDescriptionInterceptor

	class CreateDescriptionInterceptor : IInterceptor
	{
		private bool applied;

		public void Intercept(IInvocation invocation)
		{
			if (!applied)
			{
				var target = invocation.InvocationTarget;
				var channelFactoryType = FindTypeContainingChannelTypeField(target.GetType());

				if (channelFactoryType != null)
				{
					var channelTypeField = channelFactoryType.GetField("channelType",
						BindingFlags.Instance | BindingFlags.NonPublic);
					var channelType = (Type)channelTypeField.GetValue(target);
					channelTypeField.SetValue(target, AsyncType.GetAsyncType(channelType));
					applied = true;
				}
			}

			invocation.Proceed();
		}

		private Type FindTypeContainingChannelTypeField(Type channelFactoryType)
		{
			do
			{
				if (channelFactoryType.BaseType == typeof(ChannelFactory))
					return channelFactoryType;
				channelFactoryType = channelFactoryType.BaseType;
			} while (channelFactoryType != null);

			return null;
		}
	}

	#endregion
}
