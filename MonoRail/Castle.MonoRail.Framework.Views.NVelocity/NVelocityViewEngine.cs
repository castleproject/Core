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

using NVelocity;
using NVelocity.App;
using NVelocity.Context;
using NVelocity.Runtime;

namespace Castle.MonoRail.Framework.Views.NVelocity
{
	using System;
	using System.IO;
	using System.Collections;
	using System.Collections.Generic;
	using System.Text;
	using Castle.Core;
	using Commons.Collections;
	using JSGeneration;

	/// <summary>
	/// Implements a view engine using the popular Velocity syntax.
	/// <para>
	/// For details on the syntax, check the VTL Reference Guide
	/// http://jakarta.apache.org/velocity/docs/vtl-reference-guide.html
	/// </para>
	/// </summary>
	public class NVelocityViewEngine : ViewEngineBase, IInitializable
	{
		internal const String TemplateExtension = ".vm";
		internal const String JsTemplateExtension = ".njs";
		internal const String ServiceProvider = "service.provider";

		protected readonly VelocityEngine velocity = new VelocityEngine();

		private IServiceProvider provider;

		#region IInitializable implementation

		public void Initialize()
		{
			ExtendedProperties props = new ExtendedProperties();

			if (ViewSourceLoader.HasSource("nvelocity.properties"))
			{
				using(Stream stream = ViewSourceLoader.GetViewSource("nvelocity.properties").OpenViewStream())
				{
					props.Load(stream);
				}
			}

			// Set up a custom directive manager
			props.SetProperty("directive.manager",
			                  "Castle.MonoRail.Framework.Views.NVelocity.CustomDirectiveManager; Castle.MonoRail.Framework.Views.NVelocity");

			InitializeVelocityProperties(props);

			velocity.SetApplicationAttribute(ServiceProvider, provider);

			velocity.Init(props);
		}

		#endregion

		#region IServiceEnabledComponent implementation

		public override void Service(IServiceProvider provider)
		{
			base.Service(provider);
			this.provider = provider;
		}

		#endregion

		#region IViewEngine implementation

		/// <summary>
		/// Gets a value indicating whether the view engine
		/// support the generation of JS.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if JS generation is supported; otherwise, <c>false</c>.
		/// </value>
		public override bool SupportsJSGeneration
		{
			get { return true; }
		}

		/// <summary>
		/// Gets the view file extension.
		/// </summary>
		/// <value>The view file extension.</value>
		public override string ViewFileExtension
		{
			get { return ".vm"; }
		}

		/// <summary>
		/// Gets the JS generator file extension.
		/// </summary>
		/// <value>The JS generator file extension.</value>
		public override string JSGeneratorFileExtension
		{
			get { return ".njs"; }
		}

		/// <summary>
		/// Pendent.
		/// </summary>
		/// <param name="output">The output.</param>
		/// <param name="context">The context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="viewName">Name of the view.</param>
		public override void Process(String viewName, TextWriter output, IEngineContext context,
		                             IController controller, IControllerContext controllerContext)
		{
			IContext ctx = CreateContext(context, controller, controllerContext);

			try
			{
				AdjustContentType(context);

				bool hasLayout = controllerContext.LayoutNames != null && controllerContext.LayoutNames.Length != 0;

				TextWriter writer;

				if (hasLayout)
				{
					// Because we are rendering within a layout we need to catch it first
					writer = new StringWriter();
				}
				else
				{
					// No layout so render direct to the output
					writer = output;
				}

				String view = ResolveTemplateName(viewName);

				Template template = velocity.GetTemplate(view);

				PreSendView(controller, template);

				BeforeMerge(velocity, template, ctx);
				template.Merge(ctx, writer);

				PostSendView(controller, template);

				if (hasLayout)
				{
					ProcessLayoutRecursively((StringWriter) writer, context, controller, controllerContext, ctx, output);
				}
			}
			catch(Exception ex)
			{
				if (Logger.IsErrorEnabled)
				{
					Logger.Error("Could not render view", ex);
				}

				throw;
			}
		}

		public override void Process(string templateName, string layoutName,
		                             TextWriter output, IDictionary<string, object> parameters)
		{
			IContext ctx = CreateContext(parameters);

			try
			{
				bool hasLayout = layoutName != null;

				TextWriter writer;

				if (hasLayout)
				{
					// Because we are rendering within a layout we need to catch it first
					writer = new StringWriter();
				}
				else
				{
					// No layout so render direct to the output
					writer = output;
				}

				String view = ResolveTemplateName(templateName);

				Template template = velocity.GetTemplate(view);

				BeforeMerge(velocity, template, ctx);
				template.Merge(ctx, writer);

				if (hasLayout)
				{
					String contents = (writer as StringWriter).GetStringBuilder().ToString();
					ProcessLayout(contents, layoutName, ctx, output);
				}
			}
			catch(Exception ex)
			{
				if (Logger.IsErrorEnabled)
				{
					Logger.Error("Could not render view", ex);
				}

				throw;
			}
		}

		public override void ProcessPartial(String partialName, TextWriter output, IEngineContext context,
		                                    IController controller, IControllerContext controllerContext)
		{
			IContext ctx = CreateContext(context, controller, controllerContext);
			String view = ResolveTemplateName(partialName);

			try
			{
				Template template = velocity.GetTemplate(view);
				template.Merge(ctx, output);
			}
			catch(Exception ex)
			{
				if (Logger.IsErrorEnabled)
				{
					Logger.Error("Could not render partial", ex);
				}

				throw new MonoRailException("Could not render partial " + view, ex);
			}
		}

		public override object CreateJSGenerator(JSCodeGeneratorInfo generatorInfo,
		                                         IEngineContext context, IController controller,
		                                         IControllerContext controllerContext)
		{
			return new JSGeneratorDuck(generatorInfo.CodeGenerator, generatorInfo.LibraryGenerator,
			                           generatorInfo.Extensions, generatorInfo.ElementExtensions);
		}

		public override void GenerateJS(String templateName, TextWriter output, JSCodeGeneratorInfo generatorInfo,
		                                IEngineContext context, IController controller,
		                                IControllerContext controllerContext)
		{
			IContext ctx = CreateContext(context, controller, controllerContext);

			object generator = CreateJSGenerator(generatorInfo, context, controller, controllerContext);

			ctx.Put("page", generator);

			AdjustJavascriptContentType(context);

			String view = ResolveJSTemplateName(templateName);

			try
			{
				Template template = velocity.GetTemplate(view);

				StringWriter writer = new StringWriter();

				template.Merge(ctx, writer);

				output.WriteLine(generator);
			}
			catch(Exception ex)
			{
				if (Logger.IsErrorEnabled)
				{
					Logger.Error("Could not generate JS", ex);
				}

				throw new MonoRailException("Error generating JS. Template " + templateName, ex);
			}
		}

		public override void RenderStaticWithinLayout(String contents, IEngineContext context, IController controller,
		                                              IControllerContext controllerContext)
		{
			IContext ctx = CreateContext(context, controller, controllerContext);
			AdjustContentType(context);

			bool hasLayout = controllerContext.LayoutNames != null && controllerContext.LayoutNames.Length != 0;

			if (hasLayout)
			{
				ProcessLayout(contents, controller, controllerContext, ctx, context, context.Response.Output);
			}
			else
			{
				context.Response.Output.Write(contents);
			}
		}

		#endregion

		/// <summary>
		/// Initializes basic velocity properties. The main purpose of this method is to
		/// allow this logic to be overrided.
		/// </summary>
		/// <param name="props">The <see cref="ExtendedProperties"/> collection to populate.</param>
		protected virtual void InitializeVelocityProperties(ExtendedProperties props)
		{
			velocity.SetApplicationAttribute(RuntimeConstants.RESOURCE_MANAGER_CLASS,
			                                 new CustomResourceManager(ViewSourceLoader));

			LoadMacros(props);
		}

		/// <summary>
		/// Resolves the layout template name into a velocity template file name.
		/// </summary>
		protected virtual string ResolveLayoutTemplateName(string templateName)
		{
			if (templateName.StartsWith("/"))
			{
				// User is using a folder diferent than the layouts folder

				return ResolveTemplateName(templateName);
			}
			else
			{
				return String.Format("{0}{1}{2}",
				                     TemplateKeys.LayoutPath, Path.DirectorySeparatorChar,
				                     ResolveTemplateName(templateName));
			}
		}

		protected virtual void BeforeMerge(VelocityEngine velocityEngine, Template template, IContext context)
		{
		}

		private void ProcessLayoutRecursively(StringWriter writer, IEngineContext context,
		                                      IController controller, IControllerContext controllerContext,
		                                      IContext ctx, TextWriter finalOutput)
		{
			for(int i = controllerContext.LayoutNames.Length - 1; i >= 0; i--)
			{
				string layoutName = ResolveLayoutTemplateName(controllerContext.LayoutNames[i]);

				string contents = writer.GetStringBuilder().ToString();

				BeforeApplyingLayout(layoutName, ref contents, controller, controllerContext, ctx, context);

				writer.GetStringBuilder().Length = 0;

				RenderLayout(layoutName, contents, ctx, i == 0 ? finalOutput : writer);
			}
		}

		private void ProcessLayout(String contents, string layoutName, IContext ctx, TextWriter output)
		{
			RenderLayout(layoutName, contents, ctx, output);
		}

		private void ProcessLayout(String contents, IController controller, IControllerContext controllerContext, IContext ctx,
		                           IEngineContext context, TextWriter output)
		{
			String layout = ResolveLayoutTemplateName(controllerContext.LayoutNames[0]);

			BeforeApplyingLayout(layout, ref contents, controller, controllerContext, ctx, context);

			RenderLayout(layout, contents, ctx, output);
		}

		protected virtual void BeforeApplyingLayout(string layout, ref string contents,
		                                            IController controller, IControllerContext controllerContext,
		                                            IContext ctx, IEngineContext context)
		{
		}

		protected void RenderLayout(string layoutName, string contents, IContext ctx, TextWriter output)
		{
			ctx.Put(TemplateKeys.ChildContent, contents);

			Template template = velocity.GetTemplate(layoutName);

			BeforeMerge(velocity, template, ctx);

			template.Merge(ctx, output);
		}

		private IContext CreateContext(IDictionary<string, object> parameters)
		{
			Hashtable innerContext = new Hashtable(StringComparer.InvariantCultureIgnoreCase);

			foreach(KeyValuePair<string, object> pair in parameters)
			{
				innerContext[pair.Key] = pair.Value;
			}

			return new VelocityContext(innerContext);
		}

		private IContext CreateContext(IEngineContext context, IController controller, IControllerContext controllerContext)
		{
			IRequest request = context.Request;

			Hashtable innerContext = new Hashtable(StringComparer.InvariantCultureIgnoreCase);

			innerContext.Add(TemplateKeys.Controller, controller);
			innerContext.Add(TemplateKeys.Context, context);
			innerContext.Add(TemplateKeys.Request, context.Request);
			innerContext.Add(TemplateKeys.Response, context.Response);
			innerContext.Add(TemplateKeys.Session, context.Session);

			if (controllerContext.Resources != null)
			{
				foreach(String key in controllerContext.Resources.Keys)
				{
					innerContext[key] = controllerContext.Resources[key];
				}
			}

			foreach(String key in request.QueryString.AllKeys)
			{
				if (key == null) continue;
				object value = request.QueryString[key];
				if (value == null) continue;
				innerContext[key] = value;
			}
			foreach(String key in request.Form.AllKeys)
			{
				if (key == null) continue;
				object value = request.Form[key];
				if (value == null) continue;
				innerContext[key] = value;
			}

			// list from : http://msdn2.microsoft.com/en-us/library/hfa3fa08.aspx
			object[] builtInHelpers =
				new object[]
					{
						new StaticAccessorHelper<Byte>(),
						new StaticAccessorHelper<SByte>(),
						new StaticAccessorHelper<Int16>(),
						new StaticAccessorHelper<Int32>(),
						new StaticAccessorHelper<Int64>(),
						new StaticAccessorHelper<UInt16>(),
						new StaticAccessorHelper<UInt32>(),
						new StaticAccessorHelper<UInt64>(),
						new StaticAccessorHelper<Single>(),
						new StaticAccessorHelper<Double>(),
						new StaticAccessorHelper<Boolean>(),
						new StaticAccessorHelper<Char>(),
						new StaticAccessorHelper<Decimal>(),
						new StaticAccessorHelper<String>(),
						new StaticAccessorHelper<Guid>(),
						new StaticAccessorHelper<DateTime>()
					};

			foreach(object helper in builtInHelpers)
			{
				innerContext[helper.GetType().GetGenericArguments()[0].Name] = helper;
			}

			if (controllerContext.Helpers != null)
			{
				foreach(object key in controllerContext.Helpers.Keys)
				{
					innerContext[key] = controllerContext.Helpers[key];
				}
			}

			// Adding flash as a collection and each individual item

			if (context.Flash != null)
			{
				innerContext[Flash.FlashKey] = context.Flash;

				foreach(DictionaryEntry entry in context.Flash)
				{
					if (entry.Value == null) continue;
					innerContext[entry.Key] = entry.Value;
				}
			}

			if (controllerContext.PropertyBag != null)
			{
				foreach(DictionaryEntry entry in controllerContext.PropertyBag)
				{
					if (entry.Value == null) continue;
					innerContext[entry.Key] = entry.Value;
				}
			}

			innerContext[TemplateKeys.SiteRoot] = context.ApplicationPath;

			return new VelocityContext(innerContext);
		}

		private void LoadMacros(ExtendedProperties props)
		{
			String[] macros = ViewSourceLoader.ListViews("macros",this.ViewFileExtension,this.JSGeneratorFileExtension);

			ArrayList macroList = new ArrayList(macros);

			if (macroList.Count > 0)
			{
				object libPropValue = props.GetProperty(RuntimeConstants.VM_LIBRARY);

				if (libPropValue is ICollection)
				{
					macroList.AddRange((ICollection) libPropValue);
				}
				else if (libPropValue is string)
				{
					macroList.Add(libPropValue);
				}

				props.AddProperty(RuntimeConstants.VM_LIBRARY, macroList);
			}

			props.AddProperty(RuntimeConstants.VM_LIBRARY_AUTORELOAD, true);
		}
	}
}
