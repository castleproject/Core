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
	using System;
	using System.IO;

	/// <summary>
	/// Support functions for setting view and layout.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class holds the diverse RenderView, RenderSharedView and 
	/// RenderText methods originated from <see cref="Controller"/> along with CancelView and
	/// CancelLayout. This has be done because <see cref="IDynamicAction"/>s do not have
	/// access to the Controller itself, but only to <see cref="IController"/>,
	/// <see cref="IEngineContext"/> and <see cref="IControllerContext"/>.
	/// <see cref="RenderingSupport"/> allows dynamic actions to set the 
	/// relevant properties in a manner consistent to regular actions.
	/// </para>
	/// <example>
	/// Code example:
	/// <code>
	/// public class ExampleDynAction : IDynamicAction
	/// {
	///		public object Execute(
	///			IEngineContext engineContext, 
	///			IController controller, 
	///			IControllerContext controllerContext)
	///		{
	///			// other code
	///			new RenderingSupport(controllerContext, engineContext)
	///				.RenderView("myView");
	///			return null;
	///		}
	/// }
	/// </code>
	/// </example>
	/// <para>
	/// Please note that this class does not implement the following rendering-relating
	/// methods due to complex dependencies of Controller's instance data (mainly 
	/// <see cref="Controller.viewEngineManager"/>, which cannot be accessed by using the
	/// information passed to DynamicActions):
	/// <list type="bullet">
	/// <item><description><see cref="Controller.DirectRender"/></description></item>
	/// <item><description><see cref="Controller.InPlaceRenderView"/></description></item>
	/// <item><description><see cref="Controller.InPlaceRenderSharedView"/></description></item>
	/// <item><description><see cref="Controller.HasTemplate"/></description></item>
	/// </list>
	/// </para>
	/// </remarks>
	public class RenderingSupport
	{
		/// <summary>
		/// Instantiates RenderingSupport for the attached contexts.
		/// </summary>
		/// <param name="context">The controller context</param>
		/// <param name="engineContext">The engine context</param>
		public RenderingSupport(IControllerContext context, IEngineContext engineContext)
		{
			this.context = context;
			this.engineContext = engineContext;
		}

		private readonly IControllerContext context;
		private readonly IEngineContext engineContext;

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		/// <param name="name">view template name (the file extension is optional)</param>
		public void RenderView(string name)
		{
			context.SelectedViewName = Path.Combine(context.ViewFolder, name);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		/// <param name="name">view template name (the file extension is optional)</param>
		/// <param name="skipLayout">If set to <c>true</c>, no layout will be used when rendering the view</param>
		public void RenderView(string name, bool skipLayout)
		{
			if (skipLayout) CancelLayout();

			RenderView(name);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		/// <param name="name">view template name (the file extension is optional)</param>
		/// <param name="skipLayout">If set to <c>true</c>, no layout will be used when rendering the view</param>
		/// <param name="mimeType">The mime type to use on the reply</param>
		public void RenderView(string name, bool skipLayout, string mimeType)
		{
			if (skipLayout) CancelLayout();
			engineContext.Response.ContentType = mimeType;

			RenderView(name);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		/// <param name="controller">Controller name get view from (if you intend to user another controller's view</param>
		/// <param name="name">view template name (the file extension is optional)</param>
		public void RenderView(string controller, string name)
		{
			context.SelectedViewName = Path.Combine(controller, name);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		/// <param name="controller">Controller name get view from (if you intend to user another controller's view</param>
		/// <param name="name">view template name (the file extension is optional)</param>
		/// <param name="skipLayout">If set to <c>true</c>, no layout will be used when rendering the view</param>
		public void RenderView(string controller, string name, bool skipLayout)
		{
			if (skipLayout) CancelLayout();

			RenderView(controller, name);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		/// <param name="controller">Controller name get view from (if you intend to user another controller's view</param>
		/// <param name="name">view template name (the file extension is optional)</param>
		/// <param name="skipLayout">If set to <c>true</c>, no layout will be used when rendering the view</param>
		/// <param name="mimeType">The mime type to use on the reply</param>
		public void RenderView(string controller, string name, bool skipLayout, string mimeType)
		{
			if (skipLayout) CancelLayout();

			engineContext.Response.ContentType = mimeType;
			RenderView(controller, name);
		}

		/// <summary>
		/// Specifies the view to be processed after the action has finished its processing. 
		/// </summary>
		/// <param name="controller">Controller name get view from (if you intend to user another controller's view</param>
		/// <param name="name">view template name (the file extension is optional)</param>
		/// <param name="mimeType">The mime type to use on the reply</param>
		public void RenderView(string controller, string name, string mimeType)
		{
			engineContext.Response.ContentType = mimeType;
			RenderView(controller, name);
		}

		/// <summary>
		/// Specifies the shared view to be processed after the action has finished its
		/// processing. (A partial view shared 
		/// by others views and usually in the root folder
		/// of the view directory).
		/// </summary>
		public void RenderSharedView(string name)
		{
			context.SelectedViewName = name;
		}

		/// <summary>
		/// Specifies the shared view to be processed after the action has finished its
		/// processing. (A partial view shared 
		/// by others views and usually in the root folder
		/// of the view directory).
		/// </summary>
		public void RenderSharedView(string name, bool skipLayout)
		{
			if (skipLayout) CancelLayout();

			RenderSharedView(name);
		}

		/// <summary>
		/// Cancels the view processing.
		/// </summary>
		public void CancelView()
		{
			context.SelectedViewName = null;
		}

		/// <summary>
		/// Cancels the layout processing.
		/// </summary>
		public void CancelLayout()
		{
			context.LayoutNames = null;
		}

		/// <summary>
		/// Cancels the view processing and writes
		/// the specified contents to the browser
		/// </summary>
		public void RenderText(string contents)
		{
			CancelView();

			engineContext.Response.Write(contents);
		}

		/// <summary>
		/// Cancels the view processing and writes
		/// the specified contents to the browser
		/// </summary>
		public void RenderText(string contents, params object[] args)
		{
			RenderText(String.Format(contents, args));
		}

		/// <summary>
		/// Cancels the view processing and writes
		/// the specified contents to the browser
		/// </summary>
		public void RenderText(IFormatProvider formatProvider, string contents, params object[] args)
		{
			RenderText(String.Format(formatProvider, contents, args));
		}

	}

}
