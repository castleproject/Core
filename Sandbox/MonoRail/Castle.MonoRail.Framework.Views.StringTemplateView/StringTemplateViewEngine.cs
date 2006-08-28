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

namespace Castle.MonoRail.Framework.Views.StringTemplateView
{
	using System;
	using System.IO;
	using System.Collections;
	using System.ComponentModel.Design;
	using Encoding							= System.Text.Encoding;
	using StringBuilder						= System.Text.StringBuilder;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Internal;
	using StringTemplate					= Antlr.StringTemplate.StringTemplate;
	using StringTemplateGroup				= Antlr.StringTemplate.StringTemplateGroup;
	using IStringTemplateErrorListener		= Antlr.StringTemplate.IStringTemplateErrorListener;
	using StringTemplateLoader				= Antlr.StringTemplate.StringTemplateLoader;
	using FileSystemTemplateLoader			= Antlr.StringTemplate.FileSystemTemplateLoader;
	using EmbeddedResourceTemplateLoader	= Antlr.StringTemplate.EmbeddedResourceTemplateLoader;
	using IStringTemplateGroupLoader		= Antlr.StringTemplate.IStringTemplateGroupLoader;
	using CommonGroupLoader					= Antlr.StringTemplate.CommonGroupLoader;
	using CompositeGroupLoader				= Antlr.StringTemplate.CompositeGroupLoader;
	using EmbeddedResourceGroupLoader		= Antlr.StringTemplate.EmbeddedResourceGroupLoader;
	using ConfigConstants					= Castle.MonoRail.Framework.Views.StringTemplateView.Configuration.ConfigConstants;
	using STViewEngineConfiguration			= Castle.MonoRail.Framework.Views.StringTemplateView.Configuration.STViewEngineConfiguration;
	using RendererInfo						= Castle.MonoRail.Framework.Views.StringTemplateView.Configuration.STViewEngineConfiguration.RendererInfo;

	/// <summary>
	/// A MonoRail IViewEngine implementation that uses the StringTemplate 
	/// library as a view template engine.
	/// </summary>
	public class StringTemplateViewEngine : ViewEngineBase
	{
		/// <summary>Maps controllers to [cached] ST groups for locating views templates</summary>
		protected Hashtable area2templateManager = new Hashtable();
		protected StringTemplateGroup templateGroup;
		protected STViewEngineConfiguration config;

		/// <summary>
		/// Creates a new <see cref="StringTemplateViewEngine"/> instance.
		/// </summary>
		public StringTemplateViewEngine()
		{
			
		}

		#region IViewEngine Members

		public override void Init(IServiceContainer serviceContainer)
		{
			base.Init(serviceContainer);

			config = STViewEngineConfiguration.GetConfig(ConfigConstants.ELEMENT_stview_sectionname);

			StringTemplateGroup componentGroup = new ViewComponentStringTemplateGroup(
				ConfigConstants.STGROUP_NAME_PREFIX + "_components", 
				config.TemplateLexerType);

			StringTemplateGroup helpersSTGroup = new BasicStringTemplateGroup(
				ConfigConstants.STGROUP_NAME_PREFIX + "_helpersST", 
				new EmbeddedResourceTemplateLoader(this.GetType().Assembly, ConfigConstants.HELPER_RESOURCE_NAMESPACE, false),
				config.TemplateLexerType);
			helpersSTGroup.SuperGroup = componentGroup;

			templateGroup = new BasicStringTemplateGroup(
				ConfigConstants.STGROUP_NAME_PREFIX, 
				new FileSystemTemplateLoader(ViewSourceLoader.ViewRootDir, false),
				config.TemplateLexerType);
			templateGroup.SuperGroup = helpersSTGroup;

			if (config.TemplateWriterTypeName != null)
			{
				componentGroup.SetTemplateWriterType(config.TemplateWriterType);
				helpersSTGroup.SetTemplateWriterType(config.TemplateWriterType);
				templateGroup.SetTemplateWriterType(config.TemplateWriterType);
			}

			IEnumerator globalRenderersEnumerator = config.GetGlobalAttributeRenderers();
			while (globalRenderersEnumerator.MoveNext())
			{
				RendererInfo renderInfo = (RendererInfo)globalRenderersEnumerator.Current;
				componentGroup.RegisterAttributeRenderer(renderInfo.AttributeType, renderInfo.GetRendererInstance());
				helpersSTGroup.RegisterAttributeRenderer(renderInfo.AttributeType, renderInfo.GetRendererInstance());
				templateGroup.RegisterAttributeRenderer(renderInfo.AttributeType, renderInfo.GetRendererInstance());
			}
		}

		public override bool HasTemplate(string templateName)
		{
			return templateGroup.IsDefinedInThisGroup(templateName);
		}

		public override void Process(IRailsEngineContext context, Controller controller, string viewName)
		{
			TextWriter writer;
			StringBuilder sb = null;
			bool hasLayout = (controller.LayoutName != null);

			AdjustContentType(context);

			if (hasLayout)
			{
				//Because we are rendering within a layout we need to cache the output
				sb = new StringBuilder();
				writer = new StringWriter(sb);
			}
			else
			{
				writer = context.Response.Output;
			}

			try
			{
				StringTemplate template = GetViewTemplate(controller, viewName);
				SetContextAsTemplateAttributes(context, controller, ref template);
				writer.Write(template.ToString());
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
					SendErrorDetails(ex, writer, viewName);
					return;
				}
				else
				{
					throw new RailsException("Could not obtain view", ex);
				}
			}

			if (hasLayout)
			{
				ProcessLayout(context, controller, sb.ToString());
			}
		}

		public override void ProcessContents(IRailsEngineContext context, Controller controller, string contents)
		{
			AdjustContentType(context);

			bool hasLayout = controller.LayoutName != null;

			if (hasLayout)
			{
				ProcessLayout(context, controller, contents);
			}
			else
			{
				context.Response.Output.Write(contents);
			}
		}

		#endregion

		/// <summary>
		/// Retrieves the ST template for specified controller and template name.
		/// </summary>
		protected StringTemplate GetViewTemplate(Controller controller, string viewName)
		{
			StringTemplate st = null;
			StringTemplateGroup templateManager = GetTemplateManager(controller);
			if (viewName.IndexOfAny(ConfigConstants.PATH_SEPARATOR_CHARS) == -1)
			{
				// try locations in order
				bool controllerHasArea = ((controller.AreaName != null) && (controller.AreaName.Trim().Length > 0));
				string templateName;
				// <area>/<controller>/<view>
				if (controllerHasArea)
				{
					templateName = string.Format("{0}/{1}/{2}", controller.AreaName, controller.Name, viewName);
					st = templateManager.GetInstanceOf(templateName);
				}
				if (st == null)
				{
					// <area>/shared/<view>
					if (controllerHasArea)
					{
						templateName = string.Format("{0}/{1}/{2}", controller.AreaName, ConfigConstants.SHARED_DIR, viewName);
						st = templateManager.GetInstanceOf(templateName);
					}
					if (st == null)
					{
						// <controller>/<view>
						templateName = string.Format("{0}/{1}", controller.Name, viewName);
						st = templateManager.GetInstanceOf(templateName);
						if (st == null)
						{
							// shared/<view>
							templateName = string.Format("{0}/{1}", ConfigConstants.SHARED_DIR, viewName);
							st = templateManager.GetInstanceOf(templateName);
						}
					}
				}
			}

			if (st == null)
			{
				st = templateManager.GetInstanceOf(viewName.Replace('\\', '/'));
			}

            if (st == null)
            {
                throw new Castle.MonoRail.Framework.RailsException("Could not find the view: " + viewName);
            }
		    
			return st;
		}
		
		/// <summary>
		/// Retrieves the ST layout template for the specified controller.
		/// </summary>
		protected StringTemplate GetLayoutTemplate(Controller controller)
		{
			string layoutName = controller.LayoutName;
			string templateName = string.Format("{0}/{1}", ConfigConstants.LAYOUTS_DIR, layoutName.Replace('\\', '/'));
			StringTemplateGroup templateManager = GetTemplateManager(controller);
			StringTemplate st = templateManager.GetInstanceOf(templateName);
			return st;
		}
		
		protected StringTemplateGroup GetTemplateManager(Controller controller)
		{
			if ((controller.AreaName == null) || (controller.AreaName.Trim().Length == 0))
				return templateGroup;

			StringTemplateGroup group = (StringTemplateGroup)area2templateManager[controller.AreaName];
			if (group == null)
			{
				group = new BasicStringTemplateGroup(
					ConfigConstants.STGROUP_NAME_PREFIX + "_area_" + controller.AreaName, 
					(StringTemplateLoader)null,
					config.TemplateLexerType);
				group.SuperGroup = templateGroup;

				if (config.TemplateWriterTypeName != null)
				{
					group.SetTemplateWriterType(config.TemplateWriterType);
				}

				IEnumerator renderersEnumerator = config.GetAttributeRenderersForArea(controller.AreaName);
				while (renderersEnumerator.MoveNext())
				{
					RendererInfo rendererInfo = (RendererInfo)renderersEnumerator.Current;
					group.RegisterAttributeRenderer(rendererInfo.AttributeType, rendererInfo.GetRendererInstance());
				}
				area2templateManager[controller.AreaName] = group;
			}
			return group;
		}
		
		protected void ProcessLayout(IRailsEngineContext context, Controller controller, string contents)
		{
			try
			{
				StringTemplate template = GetLayoutTemplate(controller);
				SetContextAsTemplateAttributes(context, controller, ref template);
				template.SetAttribute(ConfigConstants.CHILD_CONTENT_ATTRIB_KEY, contents);

				context.Response.Output.Write(template.ToString());
			}
			catch (Exception ex)
			{
				if (context.Request.IsLocal)
				{
					SendErrorDetails(ex, context.Response.Output, "n/a");
					return;
				}
				else
				{
					throw new RailsException("Could not obtain layout", ex);
				}
			}
		}

		internal static void SetContextAsTemplateAttributes(IRailsEngineContext context, Controller controller, ref StringTemplate st)
		{
			st.SetAttribute(ConfigConstants.CONTROLLER_ATTRIB_KEY, controller);
			st.SetAttribute(ConfigConstants.CONTEXT_ATTRIB_KEY,  context);
			st.SetAttribute(ConfigConstants.REQUEST_ATTRIB_KEY,  context.Request);
			st.SetAttribute(ConfigConstants.RESPONSE_ATTRIB_KEY, context.Response);
			st.SetAttribute(ConfigConstants.SESSION_ATTRIB_KEY,  context.Session);

		    LoadResources(controller, st);

//			foreach(object helper in controller.Helpers.Values)
//			{
//				st.SetAttribute(helper.GetType().Name, helper);
//			}

		    LoadParams(context, st);

		    LoadFlash(context, st);

		    LoadPropertyBag(controller, st);

		    st.SetAttribute(ConfigConstants.SITEROOT_ATTRIB_KEY, context.ApplicationPath);
		}

	    private static void LoadResources(Controller controller, StringTemplate st) {
	        if (controller.Resources != null)
	        {
	            foreach(string key in controller.Resources.Keys)
	            {
	                st.SetAttribute( key, controller.Resources[ key ] );
	            }
	        }
	    }

	    private static void LoadParams(IRailsEngineContext context, StringTemplate st) {
	        foreach(string key in context.Params.AllKeys)
	        {
	            if (key == null)  continue; // Nasty bug?
	            object value = context.Params[key];
	            if (value == null) continue;
	            st.SetAttribute(key, value);
	            //innerContext[key] = value;
	        }
	    }

	    private static void LoadPropertyBag(Controller controller, StringTemplate st) {
	        foreach(DictionaryEntry entry in controller.PropertyBag)
	        {
	            if (entry.Value == null) continue;
	            //innerContext[entry.Key] = entry.Value;
	            st.SetAttribute((string)entry.Key, entry.Value);
	        }
	    }

	    private static void LoadFlash(IRailsEngineContext context, StringTemplate st) {
	        foreach(DictionaryEntry entry in context.Flash)
	        {
	            if (entry.Value == null) continue;
	            //innerContext[entry.Key] = entry.Value;
	            st.SetAttribute((string)entry.Key, entry.Value);
	        }
	    }

		private void SendErrorDetails(Exception ex, TextWriter writer, string viewName)
		{
			writer.WriteLine("<pre>");
            writer.WriteLine("View Name: " + viewName);
			writer.WriteLine(ex.ToString());
			writer.WriteLine("</pre>");
		}
	}
}
