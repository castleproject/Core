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

using AspectSharp;
using AspectSharp.Core;
using AspectSharp.Core.Dispatcher;
using AspectSharp.Lang.AST;

namespace Castle.Facilities.AspectSharp
{
	using System;

	using Castle.Model;
	using Castle.Model.Interceptor;

	using Castle.MicroKernel;

	using Castle.DynamicProxy;

	/// <summary>
	/// Summary description for AopInterceptor.
	/// </summary>
	[Transient]
	public class AopInterceptor : IMethodInterceptor, IOnBehalfAware
	{
		private IKernel _kernel;
		private AspectEngine _engine;
		private IInvocationDispatcher _dispatcher;

		public AopInterceptor(AspectEngine engine, IKernel kernel)
		{
			_engine = engine;
			_kernel = kernel;
		}

		public void SetInterceptedComponentModel(ComponentModel target)
		{
			AspectDefinition aspectDef = (AspectDefinition) 
				target.ExtendedProperties["aop.aspect"];

			System.Diagnostics.Debug.Assert( aspectDef != null );

			_dispatcher = new ContainerInvocationDispatcher(aspectDef, _kernel);
			_dispatcher.Init(_engine);
		}

		public object Intercept(IMethodInvocation invocation, params object[] args)
		{
			return _dispatcher.Intercept( (IInvocation) invocation, args);
		}
	}
}
