// Copyright 2004 The Apache Software Foundation
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

namespace Apache.Avalon.DynamicProxy
{
	using System;

	/// <summary>
	/// Provides a standard implementation of <see cref="IInvocationHandler"/>.
	/// Methods PreInvoke, PostInvoke can be overrided to customize its behavior.
	/// </summary>
	public class StandardInvocationHandler : IInvocationHandler
	{
		private object m_target;

		public StandardInvocationHandler(object target)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}

			m_target = target;
		}

		protected virtual void PreInvoke(object proxy, System.Reflection.MethodInfo method, params object[] arguments)
		{
		}

		protected virtual void PostInvoke(object proxy, System.Reflection.MethodInfo method, ref object returnValue, params object[] arguments)
		{
		}

		#region IInvocationHandler Members

		public virtual object Invoke(object proxy, System.Reflection.MethodInfo method, params object[] arguments)
		{
			PreInvoke(proxy, method, arguments);

			object returnValue = method.Invoke( Target, arguments );

			PostInvoke(proxy, method, ref returnValue, arguments);
			
			return returnValue;
		}

		#endregion

		public object Target
		{
			get
			{
				return m_target;
			}
		}
	}
}
