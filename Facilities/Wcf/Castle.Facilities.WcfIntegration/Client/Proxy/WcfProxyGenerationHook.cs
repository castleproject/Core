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
	using System.Reflection;
	using Castle.DynamicProxy;

	[Serializable]
	public class WcfProxyGenerationHook : IProxyGenerationHook
	{
		private readonly IProxyGenerationHook hook;

		public WcfProxyGenerationHook(IProxyGenerationHook hook)
		{
			this.hook = hook;
		}

		public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
		{
			// BUG: Due to... illogical behavior of DP this will produce illformed proxy types.
			// We have to move this piece of logic to InterceptorSelector, and move it back when the bug gets fixed.
			//if (IsChannelHolderMethod(methodInfo))
			//{
			//    return false;
			//}

			if (hook != null)
			{
				return hook.ShouldInterceptMethod(type, methodInfo);
			}

			return true;
		}

		private bool IsChannelHolderMethod(MethodInfo methodInfo)
		{
			return typeof(IWcfChannelHolder).IsAssignableFrom(methodInfo.DeclaringType);
		}

		public void NonVirtualMemberNotification(Type type, MemberInfo memberInfo)
		{
			if (hook != null)
			{
				//give the inner hook a chance to throw its own exception
				hook.NonVirtualMemberNotification(type, memberInfo);
			}

			// actually we should never get this, since we're doing an interface proxy
			// so if we do, this may mean it's a bug.
			throw new NotSupportedException(
				string.Format("Member {0}.{1} is non virtual hence can not be proxied. If you think it's a bug, please report it.",
				              type.FullName, memberInfo.Name));
		}

		public void MethodsInspected()
		{
			if(hook!=null)
			{
				hook.MethodsInspected();
			}
		}
	}
}