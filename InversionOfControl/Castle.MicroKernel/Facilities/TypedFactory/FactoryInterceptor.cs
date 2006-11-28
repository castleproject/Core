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

namespace Castle.Facilities.TypedFactory
{
	using System;

	using Castle.Core;
	using Castle.Core.Interceptor;

	using Castle.MicroKernel;

	/// <summary>
	/// Summary description for FactoryInterceptor.
	/// </summary>
	[Transient]
	public class FactoryInterceptor : IInterceptor, IOnBehalfAware
	{
		private FactoryEntry _entry;
		private IKernel _kernel;

		public FactoryInterceptor(IKernel kernel)
		{
			_kernel = kernel;
		}

		public void SetInterceptedComponentModel(ComponentModel target)
		{
			_entry = (FactoryEntry) target.ExtendedProperties["typed.fac.entry"];
		}

		public void Intercept(IInvocation invocation)
		{
			String name = invocation.Method.Name;

			object[] args = invocation.Arguments;

			if (name.Equals(_entry.CreationMethod))
			{
				if (args.Length == 0 || args[0] == null)
				{
					invocation.ReturnValue = _kernel[ invocation.Method.ReturnType ];
					return;
				}
				else
				{
					invocation.ReturnValue = _kernel[(String)args[0]];
					return;
				}
			}
			else if (name.Equals(_entry.DestructionMethod))
			{
				if (args.Length == 1)
				{
					_kernel.ReleaseComponent( args[0] );
					invocation.ReturnValue = null;
					return;
				}
			}
			
			invocation.Proceed();
		}
	}
}
