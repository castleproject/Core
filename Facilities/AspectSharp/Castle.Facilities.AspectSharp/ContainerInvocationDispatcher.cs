// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

using AopAlliance.Intercept;
using AspectSharp.Core.Dispatcher;
using AspectSharp.Lang.AST;

namespace Castle.Facilities.AspectSharp
{
	using System;

	using Castle.MicroKernel;

	/// <summary>
	/// This specialization of DefaultInvocationDispatcher only check 
	/// if the AopAlliance interceptor can be obtained from the 
	/// kernel. If not, the standard flow is used.
	/// </summary>
	public class ContainerInvocationDispatcher : DefaultInvocationDispatcher
	{
		private IKernel _kernel;

		public ContainerInvocationDispatcher(AspectDefinition aspect, IKernel kernel) : base(aspect)
		{
			_kernel = kernel;
		}

		protected override IMethodInterceptor ObtainInterceptorInstance(Type adviceType)
		{
			if (_kernel.HasComponent( adviceType ))
			{
				try
				{
					return (IMethodInterceptor) _kernel[adviceType];
				}
				catch(InvalidCastException ex)
				{
					// In this case, the specified interceptor
					// does not implement the IMethodInterceptor from
					// AopAlliance

					String message = String.Format("The interceptor {0} does " + 
						"not implement AopAlliance.Interceptor.IMethodInterceptor", adviceType.FullName); 

					throw new ApplicationException(message, ex);
				}
			}

			return base.ObtainInterceptorInstance(adviceType);
		}
	}
}
