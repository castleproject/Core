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

using NVelocity;
using NVelocity.App;
using NVelocity.Context;
using NVelocity.Runtime;
using Commons.Collections;

namespace Castle.MonoRail.Framework.Views.NVelocity
{
	using System;
	using System.IO;
	using System.Collections;
	
	using Castle.MonoRail.Framework.Internal;
	
	public class NVelocityViewEngine : ViewEngineBase
	{
		private const String TemplateExtension = ".vm";
		private const String TemplatePathPattern = "{0}{1}{2}";

		private static IViewComponentFactory staticViewComponentFactory;

		protected VelocityEngine velocity = new VelocityEngine();

		/// <summary>
		/// Creates a new <see cref="NVelocityViewEngine"/> instance.
		/// </summary>
		public NVelocityViewEngine()
		{
			
		}

		#region IViewEngine Members

		public override void Init()
		{
			ExtendedProperties props = new ExtendedProperties();

			String externalProperties = Path.Combine( ViewRootDir, "nvelocity.properties" );

			if (File.Exists( externalProperties ))
			{
				using(FileStream fs = File.OpenRead( externalProperties ))
				{
 					props.Load( fs );
				}
			}

			// Set up a custom directive manager
			props.SetProperty("directive.manager", "Castle.MonoRail.Framework.Views.NVelocity.CustomDirectiveManager; Castle.MonoRail.Framework.Views.NVelocity");

			InitializeVelocityProperties(props);

			velocity.Init(props);
		}

		/// <summary>
		/// Gets/sets the factory for <see cref="ViewComponent"/>s
		/// </summary>
		public override IViewComponentFactory ViewComponentFactory
		{
			get { return base.ViewComponentFactory; }
			set { base.ViewComponentFactory = value; staticViewComponentFactory = value; }
		}

		public override bool HasTemplate(String templateName)
		{
			try
			{
				return velocity.GetTemplate(ResolveTemplateName(templateName)) != null;
			}
			catch(Exception)
			{
				return false;
			}
		}

		public override void Process(IRailsEngineContext context, Controller controller, String viewName)
		{
			IContext ctx = CreateContext(context, controller);
			
			AdjustContentType(context);

			Template template = null;

			bool hasLayout = controller.LayoutName != null;

			TextWriter writer;

			if (hasLayout)
			{
				writer = new StringWriter();		//Because we are rendering within a layout we need to catch it first
			}
			else
			{
				writer = context.Response.Output;	//No layout so render direct to the output
			}

			String view = ResolveTemplateName(viewName);
			try
			{
				template = velocity.GetTemplate(view);

				template.Merge(ctx, writer);
			}
			catch (Exception ex)
			{
				if (hasLayout)
				{
					// Restore original writer
					writer = context.Response.Output;
				}

				if (context.Request.IsLocal)
				{
					SendErrorDetails(ex, writer);
					return;
				}
				else
				{
					throw new RailsException("Could not obtain view: " + view, ex);
				}
			}

			if (hasLayout)
			{
				ProcessLayout((writer as StringWriter).GetStringBuilder().ToString(), controller, ctx, context);
			}
		}

		///<summary>
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

				template.Merge(ctx, output);
			}
			catch (Exception ex)
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

		public static IViewComponentFactory StaticViewComponentFactory
		{
			get { return staticViewComponentFactory; }
		}

		/// <summary>
		/// Initializes basic velocity properties. The main purpose of this method is to
		/// allow this logic to be overrided.
		/// </summary>
		/// <param name="props">The <see cref="ExtendedProperties"/> collection to populate.</param>
		protected virtual void InitializeVelocityProperties(ExtendedProperties props)
		{
			props.SetProperty(RuntimeConstants_Fields.RESOURCE_MANAGER_CLASS, "NVelocity.Runtime.Resource.ResourceManagerImpl\\,NVelocity");
			props.SetProperty(RuntimeConstants_Fields.FILE_RESOURCE_LOADER_PATH, ViewRootDir);
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
			return String.Format(TemplatePathPattern, area, Path.DirectorySeparatorChar, ResolveTemplateName(templateName));
		}
		
		private void ProcessLayout(String contents, Controller controller, IContext ctx, IRailsEngineContext context)
		{
			String layout = ResolveTemplateName(TemplateKeys.LayoutPath, controller.LayoutName);

			try
			{
				ctx.Put(TemplateKeys.ChildContent, contents);

				Template template = velocity.GetTemplate(layout);

				template.Merge(ctx, context.Response.Output);
			}
			catch (Exception ex)
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
			Hashtable innerContext = new Hashtable(CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default);

			innerContext.Add(TemplateKeys.Context,	context);
			innerContext.Add(TemplateKeys.Request,	context.Request);
			innerContext.Add(TemplateKeys.Response, context.Response);
			innerContext.Add(TemplateKeys.Session,  context.Session);

			if (controller.Resources != null)
			{
				foreach(String key in controller.Resources.Keys)
				{
					innerContext.Add( key, controller.Resources[ key ] );
				}
			}

			foreach(object key in controller.Helpers.Keys)
			{
				innerContext.Add( key, controller.Helpers[ key ] );
			}

			foreach(String key in context.Params.AllKeys)
			{
				if (key == null)  continue; // Nasty bug?
				object value = context.Params[key];
				if (value == null) continue;
				innerContext[key] = value;
			}

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
	}
}
