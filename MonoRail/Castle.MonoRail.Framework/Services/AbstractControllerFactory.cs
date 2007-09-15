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

	using Castle.Core;
	using Castle.Core.Logging;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Controllers;

	/// <summary>
	/// Base implementation of <see cref="IControllerFactory"/>
	/// using the <see cref="DefaultControllerTree"/> to build an hierarchy
	/// of controllers and the areas they belong to.
	/// <seealso cref="DefaultControllerTree"/>
	/// </summary>
	public abstract class AbstractControllerFactory : IServiceEnabledComponent, IInitializable, IControllerFactory
	{
		/// <summary>
		/// The controller tree. A binary tree that contains
		/// all controllers registered
		/// </summary>
		private IControllerTree tree;
		
		/// <summary>
		/// The logger instance
		/// </summary>
		private ILogger logger = NullLogger.Instance;

		/// <summary>
		/// Initializes an <c>AbstractControllerFactory</c> instance
		/// </summary>
		public AbstractControllerFactory()
		{
		}

		#region IInitializable implementation
		
		/// <summary>
		/// Invoked by the framework in order to initialize the state
		/// </summary>
		public virtual void Initialize()
		{
			AddBuiltInControllers();
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
				logger = loggerFactory.Create(typeof(AbstractControllerFactory));
			}

			tree = (IControllerTree) provider.GetService(typeof(IControllerTree));
		}
		
		#endregion

		/// <summary>
		/// Implementors should perform their logic to
		/// return a instance of <see cref="Controller"/>.
		/// If the <see cref="Controller"/> can not be found,
		/// it should return <c>null</c>.
		/// </summary>
		/// <param name="urlInfo"></param>
		/// <returns></returns>
		public virtual Controller CreateController(UrlInfo urlInfo)
		{
			String area = urlInfo.Area ?? String.Empty;
			String name = urlInfo.Controller;
			
			return CreateControllerInstance(area, name);
		}

		/// <summary>
		/// Implementors should perform their logic
		/// to release the <see cref="Controller"/> instance
		/// and its resources.
		/// </summary>
		/// <param name="controller"></param>
		public virtual void Release(Controller controller)
		{
			if (logger.IsDebugEnabled)
			{
				logger.Debug("Controller released: " + controller.GetType());
			}
		}

		/// <summary>
		/// Gets the tree.
		/// </summary>
		/// <value>The tree.</value>
		public IControllerTree Tree
		{
			get { return tree; }
		}

		/// <summary>
		/// Register built-in controller that serve static files
		/// </summary>
		protected void AddBuiltInControllers()
		{
			if (logger.IsDebugEnabled)
			{
				logger.Debug("Registering built-in controllers");
			}
			
			Tree.AddController("MonoRail", "Files", typeof(FilesController));
		}

		/// <summary>
		/// Creates the controller instance.
		/// </summary>
		/// <param name="area">The area.</param>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		protected virtual Controller CreateControllerInstance(String area, String name)
		{
			if (logger.IsDebugEnabled)
			{
				logger.DebugFormat("Creating controller instance. Area '{0}' Name '{1}'", area, name);
			}

			Type type = Tree.GetController(area, name);

			if (type == null)
			{
				logger.ErrorFormat("Controller not found. Area '{0}' Name '{1}'", area, name);
				
				throw new ControllerNotFoundException(area, name);
			}

			try
			{
				return (Controller) Activator.CreateInstance(type);
			}
			catch(Exception ex)
			{
				logger.Error("Could not create controller instance. Activation failed.", ex);
				
				throw;
			}
		}
	}
}
