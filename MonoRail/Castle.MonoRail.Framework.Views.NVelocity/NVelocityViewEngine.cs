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

namespace Castle.MonoRail.Framework.Views.NVelocity
{
	using System;
	using System.IO;
	using System.Collections;
	using Commons.Collections;

	public class NVelocityViewEngine : ViewEngineBase
	{
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

			InitializeVelocityProperties(props);

			velocity.Init(props);
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
			AdjustContentType(ctx, context, controller);

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

			try
			{
				template = velocity.GetTemplate(ResolveTemplateName(viewName));

				template.Merge(ctx, writer);
			}
			catch (Exception ex)
			{
				if (hasLayout)
				{
					//Restore original writer
					writer = context.Response.Output;
				}

				if (context.Request.IsLocal)
				{
					SendErrorDetails(ex, writer);
					return;
				}
				else
				{
					throw new RailsException("Could not obtain view");
				}
			}

			if (hasLayout)
			{
				ProcessLayout((writer as StringWriter).GetStringBuilder().ToString(), controller, ctx, context);
			}
		}

		public override void ProcessContents(IRailsEngineContext context, Controller controller, String contents)
		{
			IContext ctx = CreateContext(context, controller);
			AdjustContentType(ctx, context, controller);

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
			props.SetProperty(RuntimeConstants_Fields.RESOURCE_MANAGER_CLASS, "NVelocity.Runtime.Resource.ResourceManagerImpl\\,NVelocity");
			props.SetProperty(RuntimeConstants_Fields.FILE_RESOURCE_LOADER_PATH, ViewRootDir);
		}
	
		/// <summary>
		/// Resolves the template name into a velocity template file name.
		/// </summary>
		protected virtual string ResolveTemplateName(string templateName)
		{
			return templateName + ".vm";
		}
		
		/// <summary>
		/// Resolves the template name into a velocity template file name.
		/// </summary>
		protected virtual string ResolveTemplateName(string area, string templateName)
		{
			return String.Format("{0}{1}{2}", area, Path.AltDirectorySeparatorChar, ResolveTemplateName(templateName));
		}
		
		private void ProcessLayout(String contents, Controller controller, IContext ctx, IRailsEngineContext context)
		{
			String layout = ResolveTemplateName("layouts", controller.LayoutName);

			try
			{
				ctx.Put("childContent", contents);

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
					throw new RailsException("Could not obtain layout");
				}
			}
		}

		private void AdjustContentType(IContext ctx, IRailsEngineContext context, Controller controller)
		{
			//TODO: Allow users to turn on XHTML support, then send the correct mime type here
			//		Tatham has a method that does the mime type negotiation already
			context.Response.ContentType = "text/html";
		}

		private IContext CreateContext(IRailsEngineContext context, Controller controller)
		{
			Hashtable innerContext = new Hashtable(
				CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default);

			innerContext.Add("context", context);
			innerContext.Add("request", context.Request);
			innerContext.Add("response", context.Response);
			innerContext.Add("session", context.Session);

			if (controller.Resources != null)
			{
				foreach (String key in controller.Resources.Keys)
				{
					innerContext.Add( key, controller.Resources[ key ] );
				}
			}

			foreach( object helper in controller.Helpers.Values)
			{
				innerContext.Add(helper.GetType().Name, helper);
			}

			foreach (String key in context.Params.AllKeys)
			{
				if (key == null)  continue; // Nasty bug?
				object value = context.Params[key];
				if (value == null) continue;
				innerContext[key] = value;
			}

			foreach (DictionaryEntry entry in context.Flash)
			{
				if (entry.Value == null) continue;
				innerContext[entry.Key] = entry.Value;
			}

			foreach (DictionaryEntry entry in controller.PropertyBag)
			{
				if (entry.Value == null) continue;
				innerContext[entry.Key] = entry.Value;
			}

			innerContext["siteRoot"] = context.ApplicationPath;

			return new VelocityContext(innerContext);
		}

		private void SendErrorDetails(Exception ex, TextWriter writer)
		{
			writer.WriteLine(ex.ToString());
		}
	}
}