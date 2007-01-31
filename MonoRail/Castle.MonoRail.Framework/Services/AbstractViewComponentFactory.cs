// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Services
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	using Castle.Core;
	using Castle.Core.Logging;
	using Castle.MonoRail.Framework.ViewComponents;

	/// <summary>
	/// Base implementation for <see cref="IViewComponentFactory"/>
	/// </summary>
	public abstract class AbstractViewComponentFactory : IInitializable, IServiceEnabledComponent, IViewComponentFactory
	{
		/// <summary>
		/// The logger instance
		/// </summary>
		private ILogger logger = NullLogger.Instance;

		public AbstractViewComponentFactory()
		{
		}

		#region IInitializable implementation

		/// <summary>
		/// Invoked by the framework in order to initialize the state
		/// </summary>
		public virtual void Initialize()
		{
			AddBuiltInComponents();
		}

		#endregion

		#region IServiceEnabledComponent implementation

		/// <summary>
		/// Invoked by the framework in order to give a chance to
		/// obtain other services
		/// </summary>
		/// <param name="provider">The service proviver</param>
		public virtual void Service(IServiceProvider provider)
		{
			ILoggerFactory loggerFactory = (ILoggerFactory) provider.GetService(typeof(ILoggerFactory));

			if (loggerFactory != null)
			{
				logger = loggerFactory.Create(typeof(AbstractViewComponentFactory));
			}
		}

		#endregion

		/// <summary>
		/// Creates an instance of the requested <see cref="ViewComponent"/>
		/// </summary>
		/// <param name="name">The view component's name</param>
		/// <returns>The view component instance</returns>
		public virtual ViewComponent Create(String name)
		{
			Type viewCompType = ResolveType(name);
			try
			{
				return (ViewComponent)Activator.CreateInstance(viewCompType);
			}
			catch (Exception ex)
			{
				logger.Error("Could not create ViewComponent instance", ex);

				throw;
			}
		}

		protected Type ResolveType(string name)
		{
			if (logger.IsDebugEnabled)
			{
				logger.DebugFormat("Creating view component '{0}'", name);
			}

			Type viewCompType = GetViewComponentTree().GetViewComponent(name);

			return viewCompType;
		}

		/// <summary>
		/// Releases a ViewComponent instance
		/// </summary>
		/// <remarks>
		/// Not currently used
		/// </remarks>
		/// <param name="instance"></param>
		public virtual void Release(ViewComponent instance)
		{
			if (logger.IsDebugEnabled)
			{
				logger.Debug("Releasing view component instance " + instance);
			}
		}

		/// <summary>
		/// Implementors should return a reference to 
		/// the current view engine.
		/// </summary>
		public abstract IViewEngine ViewEngine { get; set; }

		/// <summary>
		/// Registers viewcomponents provided by default.
		/// <seealso cref="CaptureFor"/>
		/// <seealso cref="SecurityComponent"/>
		/// </summary>
		protected virtual void AddBuiltInComponents()
		{
			RegisterComponent("CaptureFor", typeof(CaptureFor));
			RegisterComponent("SecurityComponent", typeof(SecurityComponent));
			RegisterComponent("ChildContentComponent", typeof(ChildContentComponent));
			RegisterComponent("UpdatePage", typeof(UpdatePage));
			RegisterComponent("UpdatePageTag", typeof(UpdatePageTag));
			RegisterComponent("AuthenticatedContent", typeof(AuthenticatedContent));
			RegisterComponent("DiggStylePagination", typeof(DiggStylePagination));
			RegisterComponent("ColumnRenderer", typeof(ColumnRenderer));
		}

		/// <summary>
		/// Registers a view component type.
		/// </summary>
		/// <param name="name">The view components's name</param>
		/// <param name="type">The view component's which must extend <see cref="ViewComponent"/></param>
		protected void RegisterComponent(String name, Type type)
		{
			if (logger.IsDebugEnabled)
			{
				logger.DebugFormat("Registering ViewComponent {0} Type {1} ", name, type);
			}
			GetViewComponentTree().AddViewComponent(name, type);
		}

		protected virtual IViewComponentTree GetViewComponentTree()
		{
			throw new NotImplementedException();
		}
	}
}
