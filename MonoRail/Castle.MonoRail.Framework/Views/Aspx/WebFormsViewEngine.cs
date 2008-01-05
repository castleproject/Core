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

namespace Castle.MonoRail.Framework.Views.Aspx
{
	using System;
	using System.Collections.Generic;
	using System.Web;
	using System.IO;
	using Helpers;
	using Page=System.Web.UI.Page;

	/// <summary>
	/// Pendent
	/// </summary>
	public interface IBuildManagerAccessor
	{
		/// <summary>
		/// Gets the build manager.
		/// </summary>
		/// <value>The build manager.</value>
		IBuildManager BuildManager { get; }
	}

	/// <summary>
	/// 
	/// </summary>
	public interface IBuildManager
	{
		/// <summary>
		/// Gets the type of the compiled.
		/// </summary>
		/// <param name="virtualPath">The virtual path.</param>
		/// <returns></returns>
		Type GetCompiledType(string virtualPath);
	}

	/// <summary>
	/// 
	/// </summary>
	public class AspNetBuildManagerAccessor : IBuildManagerAccessor
	{
		/// <summary>
		/// Gets the build manager.
		/// </summary>
		/// <value>The build manager.</value>
		public IBuildManager BuildManager
		{
			get { return BuildManagerAdapter.Instance; }
		}

		/// <summary>
		/// 
		/// </summary>
		public class BuildManagerAdapter : IBuildManager
		{
			private static readonly BuildManagerAdapter instance = new BuildManagerAdapter();

			private BuildManagerAdapter()
			{
			}

			/// <summary>
			/// Gets the  instance.
			/// </summary>
			/// <value>The instance.</value>
			public static BuildManagerAdapter Instance
			{
				get { return instance; }
			}

			/// <summary>
			/// Gets the type of the page.
			/// </summary>
			/// <param name="virtualPath">The virtual path.</param>
			/// <returns></returns>
			public Type GetCompiledType(string virtualPath)
			{
				return System.Web.Compilation.BuildManager.GetCompiledType(virtualPath);
			}
		}
	}

	/// <summary>
	/// Default implementation of a <see cref="IViewEngine"/>.
	/// Uses ASP.Net WebForms as views.
	/// </summary>
	public class WebFormsViewEngine : ViewEngineBase
	{
		private IBuildManagerAccessor buildManagerAccessor = new AspNetBuildManagerAccessor();

		/// <summary>
		/// Gets or sets the build manager accessor.
		/// </summary>
		/// <value>The build manager accessor.</value>
		public IBuildManagerAccessor BuildManagerAccessor
		{
			get { return buildManagerAccessor; }
			set { buildManagerAccessor = value; }
		}

		#region ViewEngineBase overrides

		/// <summary>
		/// Gets a value indicating whether the view engine
		/// support the generation of JS.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if JS generation is supported; otherwise, <c>false</c>.
		/// </value>
		public override bool SupportsJSGeneration
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the view file extension.
		/// </summary>
		/// <value>The view file extension.</value>
		public override string ViewFileExtension
		{
			get { return ".aspx"; }
		}

		/// <summary>
		/// Gets the JS generator file extension.
		/// </summary>
		/// <value>The JS generator file extension.</value>
		public override string JSGeneratorFileExtension
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Evaluates whether the specified template exists.
		/// </summary>
		/// <param name="templateName"></param>
		/// <returns><c>true</c> if it exists</returns>
		public override bool HasTemplate(String templateName)
		{
			return ViewSourceLoader.HasSource(templateName + ".aspx");
		}

		/// <summary>
		/// Obtains the aspx Page from the view name dispatch
		/// its execution using the standard ASP.Net API.
		/// </summary>
		public override void Process(String viewName, TextWriter output, IEngineContext context, IController controller,
		                             IControllerContext controllerContext)
		{
			AdjustContentType(context);

			string fullVirtualPathToTemplate = ResolveTemplateOnViewFolder(viewName);

			IBuildManager buildManager = buildManagerAccessor.BuildManager;

			Type pageType = buildManager.GetCompiledType(fullVirtualPathToTemplate);

			Page page = (Page) Activator.CreateInstance(pageType);

			if (controllerContext.LayoutNames != null && controllerContext.LayoutNames.Length != 0)
			{
				page.MasterPageFile = ResolveMasterOnViewFolder(controllerContext.LayoutNames[0]);
			}

			PageBase pageBase = page as PageBase;

			if (pageBase != null)
			{
				pageBase.ControllerContext = controllerContext;
				pageBase.UrlHelper = (UrlHelper) controllerContext.Helpers["UrlHelper"];
				pageBase.FormHelper = (FormHelper) controllerContext.Helpers["FormHelper"];
				pageBase.TextHelper = (TextHelper) controllerContext.Helpers["TextHelper"];
			}

			ProcessPage(controller, page, context.UnderlyingContext);
		}

		/// <summary>
		/// Processes the view - using the templateName
		/// to obtain the correct template
		/// and writes the results to the <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="templateName"></param>
		/// <param name="layoutName"></param>
		/// <param name="output"></param>
		/// <param name="parameters"></param>
		public override void Process(string templateName, string layoutName, TextWriter output,
		                             IDictionary<string, object> parameters)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Processes the partial.
		/// </summary>
		/// <param name="output">The output.</param>
		/// <param name="context">The context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="partialName">The partial name.</param>
		public override void ProcessPartial(string partialName, TextWriter output,
		                                    IEngineContext context, IController controller,
		                                    IControllerContext controllerContext)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Creates the JS generator.
		/// </summary>
		/// <param name="generatorInfo">The generator info.</param>
		/// <param name="context">The context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <returns>A JS generator instance</returns>
		public override object CreateJSGenerator(JSCodeGeneratorInfo generatorInfo, IEngineContext context,
		                                         IController controller, IControllerContext controllerContext)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Generates the JS.
		/// </summary>
		/// <param name="templateName">Name of the template.</param>
		/// <param name="output">The output.</param>
		/// <param name="generatorInfo">The generator info.</param>
		/// <param name="context">The context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		public override void GenerateJS(string templateName, TextWriter output, JSCodeGeneratorInfo generatorInfo,
		                                IEngineContext context, IController controller, IControllerContext controllerContext)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Wraps the specified content in the layout using the
		/// context to output the result.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="contents">The contents.</param>
		public override void RenderStaticWithinLayout(String contents, IEngineContext context, IController controller,
		                                              IControllerContext controllerContext)
		{
//			AdjustContentType(context);
//
//			HttpContext httpContext = context.UnderlyingContext;
//
//			Page masterHandler = ObtainMasterPage(httpContext, controllerContext);
//
//			httpContext.Items.Add("rails.contents", contents);
//
//			ProcessPage(controller, masterHandler, httpContext);
		}

		#endregion

		private string ResolveTemplateOnViewFolder(string viewName)
		{
			if (!Path.HasExtension(viewName))
			{
				viewName += ViewFileExtension;
			}

			if (viewName.StartsWith("~"))
			{
				// Nothing to do then
				return viewName;
			}

			return Path.Combine("~/", Path.Combine(ViewSourceLoader.VirtualViewDir, viewName));
		}

		private string ResolveMasterOnViewFolder(string viewName)
		{
			if (!Path.HasExtension(viewName))
			{
				viewName += ".master";
			}

			if (viewName.StartsWith("~"))
			{
				// Nothing to do then
				return viewName;
			}

			return Path.Combine("~/", Path.Combine(ViewSourceLoader.VirtualViewDir, Path.Combine("layouts", viewName)));
		}

		private void ProcessPage(IController controller, IHttpHandler page, HttpContext httpContext)
		{
			PreSendView(controller, page);

			page.ProcessRequest(httpContext);

			PostSendView(controller, page);
		}
	}
}
