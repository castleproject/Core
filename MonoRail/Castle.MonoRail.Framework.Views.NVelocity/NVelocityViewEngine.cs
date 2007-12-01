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

using NVelocity;
using NVelocity.App;
using NVelocity.Context;
using NVelocity.Runtime;

namespace Castle.MonoRail.Framework.Views.NVelocity
{
	using System;
	using System.IO;
	using System.Collections;
	using Castle.Core;
	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Views.NVelocity.JSGeneration;
	using Commons.Collections;

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

		private IServiceProvider provider;

		protected VelocityEngine velocity = new VelocityEngine();

		/// <summary>
		/// Creates a new <see cref="NVelocityViewEngine"/> instance.
		/// </summary>
		public NVelocityViewEngine()
		{
		}

		#region IInitializable implementation

		public void Initialize()
		{
			ExtendedProperties props = new ExtendedProperties();

			if (ViewSourceLoader.HasTemplate("nvelocity.properties"))
			{
				using (Stream stream = ViewSourceLoader.GetViewSource("nvelocity.properties").OpenViewStream())
				{
					props.Load(stream);
				}
			}

			// Set up a custom directive manager
			props.SetProperty("directive.manager",
							  "Castle.MonoRail.Framework.Views.NVelocity.CustomDirectiveManager; Castle.MonoRail.Framework.Views.NVelocity");

			InitializeVelocityProperties(props);

			velocity.SetApplicationAttribute("service.provider", provider);

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
		/// Processes the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="viewName">Name of the view.</param>
		public override void Process(IRailsEngineContext context, IController controller, String viewName)
		{
			IContext ctx = CreateContext(context, controller);

			try
			{
				AdjustContentType(context);

				bool hasLayout = controller.LayoutName != null;

				TextWriter writer;

				if (hasLayout)
				{
					// Because we are rendering within a layout we need to catch it first
					writer = new StringWriter();
				}
				else
				{
					// No layout so render direct to the output
					writer = context.Response.Output;
				}

				String view = ResolveTemplateName(viewName);

				Template template = velocity.GetTemplate(view);

				PreSendView(controller, template);

				BeforeMerge(velocity, template, ctx);
				template.Merge(ctx, writer);

				PostSendView(controller, template);

				if (hasLayout)
				{
					String contents = (writer as StringWriter).GetStringBuilder().ToString();
					ProcessLayout(contents, controller, ctx, context);
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

		/// <summary>
		/// Processes the view - using the templateName to obtain the correct template
		/// and writes the results to the System.TextWriter. No layout is applied!
		/// </summary>
		public override void Process(TextWriter output, IRailsEngineContext context, IController controller, String viewName)
		{
			IContext ctx = CreateContext(context, controller);

			AdjustContentType(context);

			String view = ResolveTemplateName(viewName);

			try
			{
				Template template = velocity.GetTemplate(view);

				BeforeMerge(velocity, template, ctx);
				template.Merge(ctx, output);
			}
			catch (Exception ex)
			{
				if (Logger.IsErrorEnabled)
				{
					Logger.Error("Could not render view", ex);
				}

				throw new MonoRailException("Could not render view: " + view, ex);
			}
		}

		public override void ProcessPartial(TextWriter output, IRailsEngineContext context,
											IController controller, string partialName)
		{
			IContext ctx = CreateContext(context, controller);
			String view = ResolveTemplateName(partialName);

			try
			{
				Template template = velocity.GetTemplate(view);
				template.Merge(ctx, output);
			}
			catch (Exception ex)
			{
				if (Logger.IsErrorEnabled)
				{
					Logger.Error("Could not render partial", ex);
				}

				throw new MonoRailException("Could not render partial " + view, ex);
			}
		}

		public override object CreateJSGenerator(IRailsEngineContext context)
		{
			return new JSGeneratorDuck(new PrototypeHelper.JSGenerator(context));
		}

		public override void GenerateJS(TextWriter output, IRailsEngineContext context, IController controller,
										string templateName)
		{
			IContext ctx = CreateContext(context, controller);

			object generator = CreateJSGenerator(context);

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
			catch (Exception ex)
			{
				if (Logger.IsErrorEnabled)
				{
					Logger.Error("Could not generate JS", ex);
				}

				throw new MonoRailException("Error generating JS. Template " + templateName, ex);
			}
		}

		public override void ProcessContents(IRailsEngineContext context, IController controller, String contents)
		{
			IContext ctx = CreateContext(context, controller);
			AdjustContentType(context);

			bool hasLayout = controller.LayoutName != null;

			if (hasLayout)
			{
				ProcessLayout(contents, controller, ctx, context);
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
		protected string ResolveLayoutTemplateName(string templateName)
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

		private void ProcessLayout(String contents, IController controller, IContext ctx, IRailsEngineContext context)
		{
			String layout = ResolveLayoutTemplateName(controller.LayoutName);

			BeforeApplyingLayout(layout, ref contents, controller, ctx, context);

			RenderLayout(layout, contents, ctx, context, context.Response.Output);
		}

		protected virtual void BeforeApplyingLayout(string layout, ref string contents,
			IController controller, IContext ctx, IRailsEngineContext context)
		{
		}

		protected void RenderLayout(string layoutName, string contents, IContext ctx, IRailsEngineContext context, TextWriter output)
		{
			try
			{
				ctx.Put(TemplateKeys.ChildContent, contents);

				Template template = velocity.GetTemplate(layoutName);

				BeforeMerge(velocity, template, ctx);

				template.Merge(ctx, output);
			}
			catch (Exception ex)
			{
				if (context.Request.IsLocal)
				{
					SendErrorDetails(ex, context.Response.Output);
				}
				else
				{
					throw new MonoRailException("Error processing layout. Maybe the layout file does not exists? File: " + layoutName, ex);
				}
			}
		}

		private IContext CreateContext(IRailsEngineContext context, IController controller)
		{
			Hashtable innerContext = new Hashtable(StringComparer.InvariantCultureIgnoreCase);

			innerContext.Add(TemplateKeys.Controller, controller);
			innerContext.Add(TemplateKeys.Context, context);
			innerContext.Add(TemplateKeys.Request, context.Request);
			innerContext.Add(TemplateKeys.Response, context.Response);
			innerContext.Add(TemplateKeys.Session, context.Session);

			if (controller.Resources != null)
			{
				foreach (String key in controller.Resources.Keys)
				{
					innerContext[key] = controller.Resources[key];
				}
			}

			foreach(String key in context.Request.QueryString.AllKeys)
			{
				if (key == null) continue; // Nasty bug?
				object value = context.Params[key];
				if (value == null) continue;
				innerContext[key] = value;
			}
			foreach(String key in context.Request.Form.AllKeys)
			{
				if (key == null) continue; // Nasty bug?
				object value = context.Params[key];
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

			if (controller.Helpers != null)
			{
				foreach (object key in controller.Helpers.Keys)
				{
					innerContext[key] = controller.Helpers[key];
				}
			}

			// Adding flash as a collection and each individual item

			if (context.Flash != null)
			{
				innerContext[Flash.FlashKey] = context.Flash;

				foreach (DictionaryEntry entry in context.Flash)
				{
					if (entry.Value == null) continue;
					innerContext[entry.Key] = entry.Value;
				}
			}

			if (controller.PropertyBag != null)
			{
				foreach (DictionaryEntry entry in controller.PropertyBag)
				{
					if (entry.Value == null) continue;
					innerContext[entry.Key] = entry.Value;
				}
			}

			innerContext[TemplateKeys.SiteRoot] = context.ApplicationPath;

			return new VelocityContext(innerContext);
		}

		private void SendErrorDetails(Exception ex, TextWriter writer)
		{
			writer.WriteLine("<pre>");
			writer.WriteLine(ex);
			writer.WriteLine("</pre>");
		}

		private void LoadMacros(ExtendedProperties props)
		{
			String[] macros = ViewSourceLoader.ListViews("macros");

			ArrayList macroList = new ArrayList(macros);

			if (macroList.Count > 0)
			{
				object libPropValue = props.GetProperty(RuntimeConstants.VM_LIBRARY);

				if (libPropValue is ICollection)
				{
					macroList.AddRange((ICollection)libPropValue);
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
