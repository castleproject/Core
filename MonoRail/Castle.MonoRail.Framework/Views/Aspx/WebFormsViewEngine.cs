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

namespace Castle.MonoRail.Framework.Views.Aspx
{
	using System;
	using System.Web;
	using System.Web.UI;
	using System.IO;
	using System.Collections;
	using System.Reflection;

	/// <summary>
	/// Default implementation of a <see cref="IViewEngine"/>.
	/// Uses ASP.Net WebForms as views.
	/// </summary>
	public class WebFormsViewEngine : ViewEngineBase
	{
		private static readonly String ProcessedBeforeKey = "processed_before";

		public WebFormsViewEngine()
		{
		}

		public override bool HasTemplate(String templateName)
		{
			return ViewSourceLoader.HasTemplate(templateName + ".aspx");
		}

		/// <summary>
		/// Obtains the aspx Page from the view name dispatch
		/// its execution using the standard ASP.Net API.
		/// </summary>
		public override void Process(IRailsEngineContext context, Controller controller, String viewName)
		{
			AdjustContentType(context);

			HttpContext httpContext = context.UnderlyingContext as HttpContext;

			//TODO: document this hack for the sake of our users
			if (httpContext != null)
			{
				if (!httpContext.Items.Contains(ProcessedBeforeKey))
				{
					httpContext.Items[ProcessedBeforeKey] = true;
				}
				else
				{
					if (IsTheSameView(httpContext, viewName)) return;
				}
			}

			Page masterHandler = null;

			if (HasLayout(controller))
			{
				StartFiltering(httpContext.Response);
				masterHandler = ObtainMasterPage(httpContext, controller);
			}

			IHttpHandler childPage = GetCompiledPageInstance(viewName, httpContext);

			ProcessPropertyBag(controller.PropertyBag, childPage);

			ProcessPage(controller, childPage, httpContext);

			ProcessLayoutIfNeeded(controller, httpContext, childPage, masterHandler);
		}

		public override void ProcessContents(IRailsEngineContext context, Controller controller, String contents)
		{
			AdjustContentType(context);

			HttpContext httpContext = (context.UnderlyingContext as HttpContext);

			Page masterHandler = ObtainMasterPage(httpContext, controller);

			httpContext.Items.Add("rails.contents", contents);

			ProcessPage(controller, masterHandler, httpContext);
		}

		private IHttpHandler GetCompiledPageInstance( String viewName, HttpContext httpContext )
		{	 
			viewName += ".aspx";

			//TODO: There needs to be a more efficient way to do this than two replace operations
			String physicalPath = 
				Path.Combine(ViewSourceLoader.ViewRootDir, viewName).Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
		    
#if dotNet2		    
			// This is a hack until I can understand the different behavior exhibited by
			// PageParser.GetCompiledPageInstance(..) when running ASP.NET 2.0.  It appears
			// that the virtualPath (first argument) to this method must represent a valid
			// virtual directory with respect to the web application.   As a result, the
			// viewRootDir must be relative to the web application project.  If it is not,
			// this will certainly fail.  If this is indeed the case, it may make sense to
			// be able to obtain an absolute virtual path to the views directory from the
			// ViewSourceLoader.

			String physicalAppPath = httpContext.Request.PhysicalApplicationPath;
			if (physicalPath.StartsWith(physicalAppPath))
			{
				viewName = "~/" + physicalPath.Substring(physicalAppPath.Length);
			}		    
#endif

			return PageParser.GetCompiledPageInstance(viewName, physicalPath, httpContext);
		}

		private bool IsTheSameView(HttpContext httpContext, String viewName)
		{
			String original = ((String) httpContext.Items[Constants.OriginalViewKey]);
			String actual = viewName;

			return String.Compare(original, actual, true) == 0;
		}

		private void ProcessPage(Controller controller, IHttpHandler page, HttpContext httpContext)
		{
			PreSendView(controller, page);
	
			page.ProcessRequest(httpContext);
	
			PostSendView(controller, page);
		}

		private void ProcessLayoutIfNeeded(Controller controller, HttpContext httpContext, IHttpHandler childPage, Page masterHandler)
		{
			if (HasLayout(controller))
			{
//				if (httpContext.Response.StatusCode == 200)
//				{
					byte[] contents = RestoreFilter(httpContext.Response);

					// Checks if its only returning from a inner process invocation
					if (!Convert.ToBoolean(httpContext.Items["rails.layout.processed"]))
					{
						httpContext.Items.Add("rails.contents", contents);
						httpContext.Items.Add("rails.child", childPage);

						httpContext.Items["rails.layout.processed"] = true;

						httpContext.Response.RedirectLocation = "foo";
					}

					ProcessPage(controller, masterHandler, httpContext);
//				}
//				else
//				{
//					WriteBuffered(httpContext.Response);
//				}
			}
		}

		private bool HasLayout(Controller controller)
		{
			return controller.LayoutName != null;
		}

//		private void WriteBuffered(HttpResponse response)
//		{
//			// response.Flush();
//
//			// Restores the original stream
//			DelegateMemoryStream filter = (DelegateMemoryStream) response.Filter;
//			response.Filter = filter.OriginalStream;
//			
//			// Writes the buffered contents
//			byte[] buffer = filter.GetBuffer();
//			response.OutputStream.Write(buffer, 0, buffer.Length);
//		}

		private byte[] RestoreFilter(HttpResponse response)
		{
			response.Flush();
			response.Filter.Flush();

			// Restores the original stream
			DelegateMemoryStream filter = (DelegateMemoryStream) response.Filter;
			response.Filter = filter.OriginalStream;

			return filter.GetBuffer();
		}

		private Page ObtainMasterPage(HttpContext context, Controller controller)
		{
			String layout = "layouts/" + controller.LayoutName;
            return GetCompiledPageInstance(layout, context) as Page;
		}

		private void StartFiltering(HttpResponse response)
		{
			Stream filter = response.Filter;
			response.Filter = new DelegateMemoryStream(filter);
			response.Buffer = true;
		}

		protected void ProcessPropertyBag(IDictionary bag, IHttpHandler handler)
		{
			foreach(DictionaryEntry entry in bag)
			{
				SetPropertyValue( handler, entry.Key, entry.Value );
			}
		}

		protected void SetPropertyValue(IHttpHandler handler, object key, object value)
		{
			if (value == null) return;

			Type type = handler.GetType();

			PropertyInfo info = 
				type.GetProperty(key.ToString(), 
				BindingFlags.Public|BindingFlags.Instance|BindingFlags.IgnoreCase);

			if (info == null || !info.CanWrite) return;

			if (!value.GetType().IsAssignableFrom(info.PropertyType)) return;

			info.GetSetMethod().Invoke( handler, new object[] {value} );
		}
	}

	internal class DelegateMemoryStream : MemoryStream
	{
		public Stream _original;

		public DelegateMemoryStream(Stream original)
		{
			_original = original;
		}

		public Stream OriginalStream
		{
			get { return _original; }
		}
	}
}
