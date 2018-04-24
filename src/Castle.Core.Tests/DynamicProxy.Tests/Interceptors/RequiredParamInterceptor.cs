// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Tests.Interceptors
{
	using System.Linq;
	using System.Reflection;

	using Castle.DynamicProxy.Tests.Classes;

	public class RequiredParamInterceptor : IInterceptor
	{
		public void Intercept(IInvocation invocation)
		{
			ParameterInfo[] parameters = invocation.Method.GetParameters();

			object[] args = invocation.Arguments;

			for (int i = 0; i < parameters.Length; i++)
			{
				if (parameters[i].IsDefined(typeof(RequiredAttribute), false))
				{
					RequiredAttribute required =
						parameters[i].GetCustomAttributes(typeof(RequiredAttribute), false).First() as RequiredAttribute;

					if ((required.BadValue == null && args[i] == null) ||
						(required.BadValue != null && required.BadValue.Equals(args[i])))
					{
						args[i] = required.DefaultValue;
					}
				}
			}

			invocation.Proceed();
		}
	}
}
