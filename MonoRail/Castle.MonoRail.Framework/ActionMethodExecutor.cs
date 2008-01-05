// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework
{
	using System.Collections.Generic;
	using System.Reflection;
	using Castle.MonoRail.Framework.Descriptors;

	/// <summary>
	/// Pendent
	/// </summary>
	public class ActionMethodExecutor : BaseExecutableAction
	{
		/// <summary>
		/// Pendent
		/// </summary>
		protected readonly MethodInfo actionMethod;

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionMethodExecutor"/> class.
		/// </summary>
		/// <param name="actionMethod">The action method.</param>
		/// <param name="metaDescriptor">The meta descriptor.</param>
		public ActionMethodExecutor(MethodInfo actionMethod, ActionMetaDescriptor metaDescriptor) : base(metaDescriptor)
		{
			this.actionMethod = actionMethod;
		}

		/// <summary>
		/// Executes the action this instance represents.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="context">The context.</param>
		public override object Execute(IEngineContext engineContext, Controller controller, IControllerContext context)
		{
			return actionMethod.Invoke(controller, null);
		}
	}

	/// <summary>
	/// Pendent
	/// </summary>
	public class ActionMethodExecutorCompatible : ActionMethodExecutor
	{
		private readonly InvokeOnController invoke;

		/// <summary>
		/// Pendent
		/// </summary>
		public delegate object InvokeOnController(MethodInfo method, IRequest request, IDictionary<string, object> methodArgs);

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionMethodExecutorCompatible"/> class.
		/// </summary>
		/// <param name="actionMethod">The action method.</param>
		/// <param name="metaDescriptor">The meta descriptor.</param>
		/// <param name="invoke">The invoke.</param>
		public ActionMethodExecutorCompatible(MethodInfo actionMethod, ActionMetaDescriptor metaDescriptor, InvokeOnController invoke) : 
			base(actionMethod, metaDescriptor)
		{
			this.invoke = invoke;
		}

		/// <summary>
		/// Executes the action this instance represents.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="context">The context.</param>
		public override object Execute(IEngineContext engineContext, Controller controller, IControllerContext context)
		{
			return invoke(actionMethod, engineContext.Request, context.CustomActionParameters);
		}
	}
}
