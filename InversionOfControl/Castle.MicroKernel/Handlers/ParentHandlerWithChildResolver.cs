// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel.Handlers
{
	using System;

	using Castle.Core;
	
	/// <summary>
	/// Redirects resolution to the main resolver, and if not found uses
	/// the parent handler.
	/// </summary>
	public class ParentHandlerWithChildResolver : IHandler, IDisposable
	{
		private readonly IHandler parentHandler;
		private readonly ISubDependencyResolver childResolver;

		/// <summary>
		/// Initializes a new instance of the <see cref="ParentHandlerWithChildResolver"/> class.
		/// </summary>
		/// <param name="parentHandler">The parent handler.</param>
		/// <param name="childResolver">The child resolver.</param>
		public ParentHandlerWithChildResolver(IHandler parentHandler, ISubDependencyResolver childResolver) 
		{
			if (parentHandler == null) throw new ArgumentNullException("parentHandler");
			if (childResolver == null) throw new ArgumentNullException("childResolver");

			this.parentHandler = parentHandler;
			parentHandler.OnHandlerStateChanged += new HandlerStateDelegate(RaiseHandlerStateChanged);
			this.childResolver = childResolver;
		}

		#region ISubDependencyResolver Members

		public virtual object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
		{

			object value = childResolver.Resolve(context, null, model, dependency);

			if (value == null)
			{
				value = parentHandler.Resolve(context, contextHandlerResolver, model, dependency);
			}

			return value;
		}

		public virtual bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
		{
			bool canResolve = false;

			if (contextHandlerResolver != null)
			{
				canResolve = childResolver.CanResolve(context, null, model, dependency);
			}

			if (!canResolve)
			{
				canResolve = parentHandler.CanResolve(context, contextHandlerResolver, model, dependency);
			}

			return canResolve;
		}

		#endregion

		#region IHandler Members

		public virtual void Init(IKernel kernel)
		{
			throw new NotImplementedException();
		}

		public virtual object Resolve(CreationContext context)
		{
			return parentHandler.Resolve(context);
		}

		public virtual bool Release(object instance)
		{
			return parentHandler.Release(instance);
		}

		public virtual HandlerState CurrentState
		{
			get { return parentHandler.CurrentState; }
		}

		public virtual ComponentModel ComponentModel
		{
			get { return parentHandler.ComponentModel; }
		}

		public Type Service
		{
			get { return ComponentModel.Service; }
		}

		public event HandlerStateDelegate OnHandlerStateChanged;

		protected virtual void RaiseHandlerStateChanged(object s, EventArgs e)
		{
			if (OnHandlerStateChanged != null)
			{
				OnHandlerStateChanged(s, e);
			}
		}

		public virtual void AddCustomDependencyValue(string key, object value)
		{
			parentHandler.AddCustomDependencyValue(key, value);
		}

		public virtual void RemoveCustomDependencyValue(string key)
		{
			parentHandler.RemoveCustomDependencyValue(key);
		}

		public virtual bool HasCustomParameter(string key)
		{
			return parentHandler.HasCustomParameter(key);
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (parentHandler != null)
				{
					parentHandler.OnHandlerStateChanged -= new HandlerStateDelegate(RaiseHandlerStateChanged);
				}
			}
		}

		#endregion
	}
}
