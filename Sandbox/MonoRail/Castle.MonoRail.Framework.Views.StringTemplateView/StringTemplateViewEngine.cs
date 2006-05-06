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
	using System.Text;

	using Antlr.StringTemplate;
	
	using Castle.MonoRail.Framework.Views.StringTemplateView.Configuration;

	/// <summary>
	/// A MonoRail IViewEngine implementation that uses the StringTemplate 
	/// library as a view template engine.
	/// </summary>
	public class StringTemplateViewEngine : ViewEngineBase
	{
		/// <summary>Maps controllers to [cached] ST groups for locating views templates</summary>
		protected Hashtable area2group;
		protected StringTemplateGroup templateGroup;
		protected STViewEngineConfiguration config;

		/// <summary>
		/// Creates a new <see cref="StringTemplateViewEngine"/> instance.
		/// </summary>
		public StringTemplateViewEngine()
		{
			
		}

		#region IViewEngine Members

		public override void Init(IServiceProvider serviceProvider)
		{
			base.Init(serviceProvider);

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
				STViewEngineConfiguration.RendererInfo renderInfo = (STViewEngineConfiguration.RendererInfo)globalRenderersEnumerator.Current;
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
					SendErrorDetails(ex, writer);
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
				// <area>/<controller>/<view>
				string templateName = string.Format("{0}/{1}/{2}", controller.AreaName, controller.Name, viewName);
				st = templateManager.GetInstanceOf(templateName);
				if (st == null)
				{
					// <area>/shared/<view>
					templateName = string.Format("{0}/{1}/{2}", controller.AreaName, ConfigConstants.SHARED_DIR, viewName);
					st = templateManager.GetInstanceOf(templateName);
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

			StringTemplateGroup group = (StringTemplateGroup)area2group[controller.AreaName];
			if (group == null)
			{
				group = new BasicStringTemplateGroup(
					ConfigConstants.STGROUP_NAME_PREFIX + "_area_" + controller.AreaName, 
					new FileSystemTemplateLoader(ViewSourceLoader.ViewRootDir, false),
					config.TemplateLexerType);
				group.SuperGroup = templateGroup;

				if (config.TemplateWriterTypeName != null)
				{
					group.SetTemplateWriterType(config.TemplateWriterType);
				}

				IEnumerator renderersEnumerator = config.GetAttributeRenderersForArea(controller.AreaName);
				while (renderersEnumerator.MoveNext())
				{
					STViewEngineConfiguration.RendererInfo renderInfo = (STViewEngineConfiguration.RendererInfo)renderersEnumerator.Current;
					group.RegisterAttributeRenderer(renderInfo.AttributeType, renderInfo.GetRendererInstance());
				}
				area2group[controller.AreaName] = group;
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
					SendErrorDetails(ex, context.Response.Output);
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

			if (controller.Resources != null)
			{
				foreach(string key in controller.Resources.Keys)
				{
					st.SetAttribute( key, controller.Resources[ key ] );
				}
			}

//			foreach(object helper in controller.Helpers.Values)
//			{
//				st.SetAttribute(helper.GetType().Name, helper);
//			}

			foreach(string key in context.Params.AllKeys)
			{
				if (key == null)  continue; // Nasty bug?
				object value = context.Params[key];
				if (value == null) continue;
				st.SetAttribute(key, value);
				//innerContext[key] = value;
			}

			foreach(DictionaryEntry entry in context.Flash)
			{
				if (entry.Value == null) continue;
				//innerContext[entry.Key] = entry.Value;
				st.SetAttribute((string)entry.Key, entry.Value);
			}

			foreach(DictionaryEntry entry in controller.PropertyBag)
			{
				if (entry.Value == null) continue;
				//innerContext[entry.Key] = entry.Value;
				st.SetAttribute((string)entry.Key, entry.Value);
			}

			st.SetAttribute(ConfigConstants.SITEROOT_ATTRIB_KEY, context.ApplicationPath);
		}

		private void SendErrorDetails(Exception ex, TextWriter writer)
		{
			writer.WriteLine("<pre>");
			writer.WriteLine(ex.ToString());
			writer.WriteLine("</pre>");
		}
	}
}
