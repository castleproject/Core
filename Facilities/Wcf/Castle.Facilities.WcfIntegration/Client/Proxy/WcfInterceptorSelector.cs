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
	using System.Collections.Generic;
	using System.Reflection;
	using System.ServiceModel;
	using Castle.Core.Interceptor;

	[Serializable]
	public class WcfInterceptorSelector : IInterceptorSelector
	{
		private readonly Type proxiedType;
		private readonly IInterceptorSelector userProvidedSelector;

		private static readonly IInterceptor[] EmptyInterceptors = new IInterceptor[0];

		public WcfInterceptorSelector(Type proxiedType, IInterceptorSelector userProvidedSelector)
		{
			this.proxiedType = proxiedType;
			this.userProvidedSelector = userProvidedSelector;
		}

		public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
		{
			if (This_should_be_in_ProxyGenerationHook_IsProxyWrapperMethod(method))
			{
				return EmptyInterceptors;
			}

			if (IsProxiedTypeMethod(method))
			{
				return SelectInterceptorsForProxiedType(method, interceptors, type);
			}

			if (IsServiceMethod(method))
			{
				return SelectInterceptorsForServiceType(method, interceptors);
			}

			return interceptors;
		}

		private bool This_should_be_in_ProxyGenerationHook_IsProxyWrapperMethod(MethodInfo methodInfo)
		{
			return methodInfo.DeclaringType.IsAssignableFrom(typeof(IWcfChannelHolder));
		}

		private IInterceptor[] SelectInterceptorsForServiceType(MethodInfo method, IInterceptor[] interceptors)
		{
			return Array.FindAll(interceptors, i => i is IWcfInterceptor);
		}

		private IInterceptor[] SelectInterceptorsForProxiedType(MethodInfo method, IInterceptor[] interceptors, Type type)
		{
			List<IInterceptor> infrastructureInterceptors, userInterceptors;
			SplitInterceptors(interceptors, method, out infrastructureInterceptors, out userInterceptors);

			var selectedInterceptors = AddWcfInterceptors(infrastructureInterceptors,
				SelectUserInterceptors(method, userInterceptors, type));

			return selectedInterceptors;
		}

		private void SplitInterceptors(IInterceptor[] interceptors, MethodInfo method,
		                               out List<IInterceptor> infrastructureInterceptors,
		                               out List<IInterceptor> userInterceptors)
		{
			userInterceptors = new List<IInterceptor>(interceptors.Length);
			infrastructureInterceptors = new List<IInterceptor>(interceptors.Length);

			foreach (var interceptor in interceptors)
			{
				if (interceptor is IWcfInterceptor)
				{
					var infrastructureInterceptor = (IWcfInterceptor)interceptor;
					if (infrastructureInterceptor.Handles(method))
					{
						infrastructureInterceptors.Add(infrastructureInterceptor);
					}
					continue;
				}
				userInterceptors.Add(interceptor);
			}
		}

		private IInterceptor[] SelectUserInterceptors(MethodInfo method, List<IInterceptor> userInterceptors, Type type)
		{
			var selectedInterceptors = userInterceptors.ToArray();

			if (userProvidedSelector != null)
			{
				selectedInterceptors = userProvidedSelector.SelectInterceptors(type, method, selectedInterceptors);
			}

			return selectedInterceptors;
		}

		private IInterceptor[] AddWcfInterceptors(List<IInterceptor> infrastructureInterceptors,
		                                          IInterceptor[] selectedInterceptors)
		{
			if (infrastructureInterceptors.Count > 0)
			{
				int index = selectedInterceptors.Length;
				Array.Resize(ref selectedInterceptors, index + infrastructureInterceptors.Count);
				infrastructureInterceptors.CopyTo(selectedInterceptors, index);
			}

			return selectedInterceptors;
		}

		private bool IsServiceMethod(MethodInfo method)
		{
			Type type = method.DeclaringType;
			return type.IsAssignableFrom(typeof(IClientChannel)) ||
			       type.IsAssignableFrom(typeof(IServiceChannel)) ||
			       type.IsAssignableFrom(typeof(IDuplexContextChannel));
		}

		private bool IsProxiedTypeMethod(MethodInfo method)
		{
			return method.DeclaringType.IsAssignableFrom(proxiedType);
		}
	}
}