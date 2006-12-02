// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	using Commons.Collections;

	/// <summary>
	/// Pendent
	/// </summary>
	public class NVelocityViewEngine : ViewEngineBase, IInitializable
	{
		internal const String TemplateExtension = ".vm";

		internal const String ServiceProvider = "service.provider";

		// private const String TemplatePathPattern = "{0}{1}{2}";

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
				using(Stream stream = ViewSourceLoader.GetViewSource("nvelocity.properties").OpenViewStream())
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

		public override bool HasTemplate(String templateName)
		{
			return ViewSourceLoader.HasTemplate(ResolveTemplateName(templateName));
		}

		public override void Process(IRailsEngineContext context, Controller controller, String viewName)
		{
			IContext ctx = CreateContext(context, controller);

			AdjustContentType(context);

			Template template;

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

			try
			{
				template = velocity.GetTemplate(view);

				PreSendView(controller, template);

				BeforeMerge(velocity, template, ctx);
				template.Merge(ctx, writer);

				PostSendView(controller, template);
			}
			catch(Exception)
			{
				if (hasLayout)
				{
					// Restore original writer
					writer = context.Response.Output;
				}

				throw;
			}

			if (hasLayout)
			{
				String contents = (writer as StringWriter).GetStringBuilder().ToString();
				ProcessLayout(contents, controller, ctx, context);
			}
		}

		/// <summary>
		/// Processes the view - using the templateName to obtain the correct template
		/// and writes the results to the System.TextWriter. No layout is applied!
		/// </summary>
		public override void Process(TextWriter output, IRailsEngineContext context, Controller controller, String viewName)
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
			catch(Exception ex)
			{
				throw new RailsException("Could not obtain view: " + view, ex);
			}
		}

		public override void ProcessContents(IRailsEngineContext context, Controller controller, String contents)
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
		/// Resolves the template name into a velocity template file name.
		/// </summary>
		protected virtual string ResolveTemplateName(string templateName)
		{
			return templateName + TemplateExtension;
		}

		/// <summary>
		/// Resolves the template name into a velocity template file name.
		/// </summary>
		protected virtual string ResolveTemplateName(string area, string templateName)
		{
			return String.Format("{0}{1}{2}", area, Path.DirectorySeparatorChar, ResolveTemplateName(templateName));
		}

		protected virtual void BeforeMerge(VelocityEngine velocity, Template template, IContext context)
		{
		}

		private void ProcessLayout(String contents, Controller controller, IContext ctx, IRailsEngineContext context)
		{
			String layout = ResolveTemplateName(TemplateKeys.LayoutPath, controller.LayoutName);

			try
			{
				ctx.Put(TemplateKeys.ChildContent, contents);

				Template template = velocity.GetTemplate(layout);

				BeforeMerge(velocity, template, ctx);
				template.Merge(ctx, context.Response.Output);
			}
			catch(Exception ex)
			{
				if (context.Request.IsLocal)
				{
					SendErrorDetails(ex, context.Response.Output);
					return;
				}
				else
				{
					throw new RailsException("Could not obtain layout: " + layout, ex);
				}
			}
		}

		private IContext CreateContext(IRailsEngineContext context, Controller controller)
		{
#if DOTNET2
			Hashtable innerContext = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
#else
			Hashtable innerContext = new Hashtable(CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default);
#endif

			innerContext.Add(TemplateKeys.Controller, controller);
			innerContext.Add(TemplateKeys.Context, context);
			innerContext.Add(TemplateKeys.Request, context.Request);
			innerContext.Add(TemplateKeys.Response, context.Response);
			innerContext.Add(TemplateKeys.Session, context.Session);

			if (controller.Resources != null)
			{
				foreach(String key in controller.Resources.Keys)
				{
					innerContext.Add(key, controller.Resources[key]);
				}
			}

			foreach(object key in controller.Helpers.Keys)
			{
				innerContext.Add(key, controller.Helpers[key]);
			}

			foreach(String key in context.Params.AllKeys)
			{
				if (key == null) continue; // Nasty bug?
				object value = context.Params[key];
				if (value == null) continue;
				innerContext[key] = value;
			}

			// Adding flash as a collection and each individual item

			innerContext[Flash.FlashKey] = context.Flash;

			foreach(DictionaryEntry entry in context.Flash)
			{
				if (entry.Value == null) continue;
				innerContext[entry.Key] = entry.Value;
			}

			foreach(DictionaryEntry entry in controller.PropertyBag)
			{
				if (entry.Value == null) continue;
				innerContext[entry.Key] = entry.Value;
			}

			innerContext[TemplateKeys.SiteRoot] = context.ApplicationPath;

			return new VelocityContext(innerContext);
		}

		private void SendErrorDetails(Exception ex, TextWriter writer)
		{
			writer.WriteLine("<pre>");
			writer.WriteLine(ex.ToString());
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