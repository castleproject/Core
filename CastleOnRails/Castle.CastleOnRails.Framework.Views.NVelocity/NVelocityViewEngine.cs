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

namespace Castle.CastleOnRails.Framework.Views.NVelocity
{
	using System;
	using System.IO;
	using System.Collections;

	using Commons.Collections;

	public class NVelocityViewEngine : IViewEngine
	{
		private String _viewRootDir;

		public NVelocityViewEngine()
		{
		}

		private void InitVelocity()
		{
			ExtendedProperties props = new ExtendedProperties();

			props.SetProperty( RuntimeConstants_Fields.RESOURCE_MANAGER_CLASS, "NVelocity.Runtime.Resource.ResourceManagerImpl\\,NVelocity" );
			props.SetProperty( RuntimeConstants_Fields.FILE_RESOURCE_LOADER_PATH, ViewRootDir );

			Velocity.Init(props);
		}

		#region IViewEngine Members

		public void Process(IRailsEngineContext context, Controller controller, String viewName)
		{
			IContext ctx = CreateContext(context, controller);
			AdjustContentType(ctx, context, controller, viewName);

			Template template = null;

			bool hasLayout = controller.LayoutName != null;

			TextWriter writer = context.Response.Output;
			
			if (hasLayout)
			{
				writer = new StringWriter();
			}

			try
			{
				template = RuntimeSingleton.getTemplate(viewName + ".vm");

				template.Merge(ctx, writer);
			}
			catch(Exception ex)
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
					throw new RailsException("Could not obtain view");
				}
			}

			if (hasLayout)
			{
				ProcessLayout( (writer as StringWriter).GetStringBuilder().ToString(), controller, ctx, context );
			}

		}

		public String ViewRootDir
		{
			get { return _viewRootDir; }
			set { _viewRootDir = value; InitVelocity(); }
		}

		#endregion

		private void ProcessLayout(String contents, Controller controller, IContext ctx, IRailsEngineContext context)
		{
			String layout = String.Format("layouts/{0}.vm", controller.LayoutName);

			try
			{
				ctx.Put("childContent", contents);

				Template template = RuntimeSingleton.getTemplate(layout);

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
					throw new RailsException("Could not obtain layout");
				}
			}
		}

		private void AdjustContentType(IContext ctx, IRailsEngineContext context, Controller controller, string name)
		{
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

			foreach(DictionaryEntry entry in controller.PropertyBag)
			{
				innerContext[entry.Key] = entry.Value;
			}
			foreach(String key in context.Params.AllKeys)
			{
				innerContext[key] = context.Params[key];
			}
			foreach(DictionaryEntry entry in context.Flash)
			{
				innerContext[entry.Key] = entry.Value;
			}

			return new VelocityContext( innerContext );
		}

		private void SendErrorDetails(Exception ex, TextWriter writer)
		{
			writer.WriteLine(ex.ToString());
		}
	}
}