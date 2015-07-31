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

#if !NETCORE
namespace Castle.DynamicProxy.Tests.Interceptors
{
	using Castle.Core.Interceptor;

	public class ResultModifierInterceptor : StandardInterceptor
	{
		protected override void PostProceed(IInvocation invocation)
		{
			object returnValue = invocation.ReturnValue;

			if (returnValue != null && returnValue.GetType() == typeof (int))
			{
				int value = (int) returnValue;

				invocation.ReturnValue = --value;
			}
			if (returnValue != null && returnValue.GetType() == typeof (bool))
			{
				invocation.ReturnValue = true;
			}
		}
	}
}
#endif