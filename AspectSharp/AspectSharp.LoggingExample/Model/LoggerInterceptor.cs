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

namespace AspectSharp.LoggingExample.Model
{
	using System;
	using System.Reflection;
	using AopAlliance.Intercept;


	public class LoggerInterceptor : IMethodInterceptor
	{
		public LoggerInterceptor()
		{
		}

		public object Invoke(IMethodInvocation invocation)
		{
			LogMethod(invocation.Method);
			LogArguments(invocation.Arguments);

			object result = invocation.Proceed();

			LogReturn(result);

			return result;
		}

		private void LogMethod(MethodBase method)
		{
			Console.WriteLine("[Aspect# Logger Interceptor][method]Name: {0}", method.Name);
		}

		private void LogReturn(object result)
		{
			Console.WriteLine("[Aspect# Logger Interceptor][return]Value: {0}", result);
		}

		private void LogArguments(object[] arguments)
		{
			for (int i = 0; i < arguments.Length; i++)
			{
				Console.WriteLine("[Aspect# Logger Interceptor][argument]Index: {0}. Value: {1}", i, arguments[i]);
			}
		}
	}
}
