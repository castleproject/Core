// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy
{
	using System;
	using System.Runtime.Remoting;

	public class ProxyUtil
	{
		public static object GetUnproxiedInstance(object instance)
		{
#if (!SILVERLIGHT)
			if (!RemotingServices.IsTransparentProxy(instance))
#endif
			{
				var accessor = instance as IProxyTargetAccessor;
				if (accessor != null)
				{
					instance = accessor.DynProxyGetTarget();
				}
			}

			return instance;
		}

		public static Type GetUnproxiedType(object instance)
		{
#if (!SILVERLIGHT)
			if (!RemotingServices.IsTransparentProxy(instance))
#endif
			{
				var accessor = instance as IProxyTargetAccessor;

				if (accessor != null)
				{
					var target = accessor.DynProxyGetTarget();

					if (target != null)
					{
						if (ReferenceEquals(target, instance))
						{
							return instance.GetType().BaseType;
						}

						instance = target;
					}
				}
			}

			return instance.GetType();
		}

		public static bool IsProxy(object instance)
		{
#if (!SILVERLIGHT)
			if (RemotingServices.IsTransparentProxy(instance))
			{
				return true;
			}
#endif
			return instance is IProxyTargetAccessor;
		}

		public static bool IsProxyType(Type type)
		{
			return typeof(IProxyTargetAccessor).IsAssignableFrom(type);
		}
	}
}