// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace AspectSharp.Tests.Classes
{
	using System;
	using System.Collections;
	using System.Reflection;

	using AopAlliance.Intercept;


	public class LogInvocationInterceptor : IMethodInterceptor
	{
		private static readonly ArrayList _list = new ArrayList();

		public LogInvocationInterceptor()
		{
		}

		public static void Clear()
		{
			_list.Clear();
		}

		public static String[] Messages
		{
			get { return (String[]) _list.ToArray( typeof(String) ); }
		}

		public object Invoke(IMethodInvocation invocation)
		{
			MethodBase method = invocation.Method;
			
			if (method.IsSpecialName)
			{
				_list.Add("property " + method.Name);
			}
			else
			{
				Type[] args = Type.GetTypeArray(invocation.Arguments);

				String argMessage = "(";
				foreach(Type arg in args)
				{
					argMessage += " " + arg.Name;
				}
				argMessage += ")";
				
				_list.Add("method " + method.Name + argMessage);
			}

			return invocation.Proceed();
		}
	}
}
