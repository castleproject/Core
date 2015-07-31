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
	using System;
	using System.Collections;
	using System.Text;
	using System.Collections.Generic;
	using System.Reflection;

	public class LogInvocationInterceptor : StandardInterceptor
	{
		private StringBuilder sb = new StringBuilder();
		private List<string> invocations = new List<string>();

		public bool Proceed = true;

		protected override void PreProceed(IInvocation invocation)
		{
			invocations.Add(invocation.Method.Name);

			sb.Append(String.Format("{0} ", invocation.Method.Name));
		}

		protected override void PerformProceed(IInvocation invocation)
		{
			if (Proceed)
			{
				base.PerformProceed(invocation);
			}
			else if (invocation.Method.ReturnType.GetTypeInfo().IsValueType && invocation.Method.ReturnType != typeof(void))
			{
				invocation.ReturnValue = Activator.CreateInstance(invocation.Method.ReturnType); // set default return value
			}
		}

		public String LogContents
		{
			get { return sb.ToString(); }
		}

		public IList Invocations
		{
			get { return invocations; }
		}
	}
}