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

namespace Castle.DynamicProxy.Test.Interceptors
{
	using System;
	using System.Text;
	using Castle.DynamicProxy.Test.Classes;


	public class RefAndOutInterceptor : LogInvokeInterceptor
	{
		public RefAndOutInterceptor()
		{

		}

		protected override void PostProceed(IInvocation invocation, ref object returnValue, params object[] args)
		{
			base.PostProceed(invocation, ref returnValue, args);

			switch (invocation.Method.Name)
			{
				case "RefInt":
				case "OutInt":
					args[0] = ((int) args[0]) + 100;
					break;
				case "RefString":
				case "OutString":
					args[0] = ((string) args[0]) + "_xxx";
					break;
				case "RefDateTime":
				case "OutDateTime":
					args[0] = ((DateTime) args[0]).AddMonths(1);
					break;
				case "RefSByteEnum":
				case "OutSByteEnum":
					args[0] = ((SByteEnum) args[0]) == SByteEnum.One ? SByteEnum.Two : SByteEnum.One;
					break;
				default:
					throw new NotSupportedException(invocation.Method.Name);
			}
		}
	}
}
