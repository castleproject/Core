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

namespace Castle.MonoRail.Framework.Views.Xslt
{
	using System;
	using System.IO;
	using System.Xml;
	using System.Xml.Xsl;

	[Obsolete("Far from finished yet")]
	public class XsltViewEngine : ViewEngineBase
	{
		/// <summary>
		/// Initializes the view engine.
		/// </summary>
		public override void Init()
		{}

		/// <summary>
		/// Evaluates whether the specified template exists.
		/// </summary>
		/// <returns><c>true</c> if it exists</returns>
		public override bool HasTemplate(String templateName)
		{
			return File.Exists(Path.Combine(ViewRootDir, ResolveTemplateName(templateName)));
		}

		/// <summary>
		/// Processes the view - using the templateName to obtain the correct template,
		/// and using the context to output the result.
		/// </summary>
		public override void Process(IRailsEngineContext context, Controller controller, String templateName)
		{
			AdjustContentType(context);

			XsltArgumentList args = CreateArguments(context, controller);
			XmlDocument input = CreateInputXml();

			bool hasLayout = (controller.LayoutName != null);

			TextWriter writer = (hasLayout ? new StringWriter() : context.Response.Output);

			try
			{
				XslTransform transform = LoadXslt(context, templateName);

				if (! hasLayout)
				{
					transform.Transform(input, args, writer, null);
				}
				else
				{
					XmlDocument innerContent = new XmlDocument();
					using (StringWriter sw = new StringWriter())
					{
						XmlTextWriter xw = new XmlTextWriter(sw);
						XmlNamespaceManager namespaceManager = new XmlNamespaceManager(innerContent.NameTable);
						namespaceManager.AddNamespace("", "http://www.w3.org/1999/xhtml");
						transform.Transform(input, args, xw);
						xw.Close();
						innerContent.LoadXml("<childContent>" + sw.ToString() + "</childContent>");
					}

					ProcessLayout(innerContent, controller, args, context);
				}
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
					throw new RailsException("Could not obtain view: " + ResolveTemplateName(templateName), ex);
				}
			}
		}

		/// <summary>
		/// Wraps the specified content in the layout using the context to output the result.
		/// </summary>
		public override void ProcessContents(IRailsEngineContext context, Controller controller, String contents)
		{
			throw new NotImplementedException();
		}

		private XslTransform LoadXslt(IRailsEngineContext context, string templateName)
		{
			XslTransform transform = new XslTransform();
			using (Stream templateStream = LoadTemplateFile(context, templateName))
			{
				XmlDocument templateDocument = new XmlDocument();
				templateDocument.Load(templateStream);
				transform.Load(templateDocument.DocumentElement);
			}
			return transform;
		}

		private void ProcessLayout(XmlDocument contents, Controller controller, XsltArgumentList args, IRailsEngineContext context)
		{
			String layout = "layouts/" + controller.LayoutName;

			try
			{
				XslTransform transform = LoadXslt(context, layout);
				transform.Transform(contents, args, context.Response.Output);
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

		protected virtual string ResolveTemplateName(string templateName)
		{
			return templateName + ".xslt";
		}

		protected virtual string ResolveTemplatePath(IRailsEngineContext context, string templateName)
		{
			return Path.Combine(context.ApplicationPath, Path.Combine(ViewRootDir, ResolveTemplateName(templateName)));
		}

		protected virtual Stream LoadTemplateFile(IRailsEngineContext context, string templateName)
		{
			return File.OpenRead(ResolveTemplatePath(context, templateName));
		}

		private XmlDocument CreateInputXml()
		{
			using (StringWriter stringWriter = new StringWriter())
			{
				XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);

				xmlWriter.WriteStartDocument(true);
				xmlWriter.WriteStartElement("input");
				xmlWriter.WriteStartElement("context");
				xmlWriter.WriteEndElement();
				xmlWriter.WriteEndElement();

				xmlWriter.Close();

				XmlDocument document = new XmlDocument();
				document.LoadXml(stringWriter.ToString());
				return document;
			}
		}

		private XsltArgumentList CreateArguments(IRailsEngineContext context, Controller controller)
		{
			XsltArgumentList args = new XsltArgumentList();

			args.AddParam("siteRoot", "", context.ApplicationPath);

			if (controller.Resources != null)
			{
				foreach (String key in controller.Resources.Keys)
				{
					args.AddParam(key, "", controller.Resources[key]);
				}
			}
			
			foreach (string key in context.Flash.Keys)
			{
				if (context.Flash[key] != null)
					args.AddParam(key, "", context.Flash[key]);
			}

			foreach (string key in controller.PropertyBag.Keys)
			{
				if (controller.PropertyBag[key] != null)
					args.AddParam(key, "", controller.PropertyBag[key]);
			}

			foreach (object helper in controller.Helpers.Values)
			{
				args.AddExtensionObject(helper.GetType().Name, helper);
			}

			args.AddExtensionObject("context", context);
			args.AddExtensionObject("request", context.Request);
			args.AddExtensionObject("response", context.Response);
			args.AddExtensionObject("server", context.Server);
			args.AddExtensionObject("session", context.Session);

			return args;
		}

		private void SendErrorDetails(Exception ex, TextWriter writer)
		{
			writer.WriteLine(ex.ToString());
		}
	}
}