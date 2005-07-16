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
	public class AspNetViewEngine : ViewEngineBase
	{
		private static readonly String ProcessedBeforeKey = "processed_before";
		
		#region IViewEngine

		public override void Init()
		{
		}

		public override bool HasTemplate(String templateName)
		{
			String physicalPath = Path.Combine( ViewRootDir, templateName + ".aspx" );
			FileInfo info = new FileInfo( physicalPath );
			return info.Exists;
		}

		/// <summary>
		/// Obtains the aspx Page from the view name dispatch
		/// its execution using the standard ASP.Net API.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="controller"></param>
		/// <param name="viewName"></param>
		public override void Process(IRailsEngineContext context, Controller controller, String viewName)
		{
			HttpContext httpContext = context.UnderlyingContext as HttpContext;

			// To Do: document this hack for the sake of our users
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

			IHttpHandler childPage = GetCompiledPageInstace(viewName, httpContext);

			ProcessPropertyBag(controller.PropertyBag, childPage);

			ProcessPage(controller, childPage, httpContext);

			ProcessLayoutIfNeeded(controller, httpContext, childPage, masterHandler);		
		}

		public override void ProcessContents(IRailsEngineContext context, Controller controller, String contents)
		{
			HttpContext httpContext = (context.UnderlyingContext as HttpContext);

			Page masterHandler = ObtainMasterPage(httpContext, controller);

			httpContext.Items.Add("rails.contents", contents);

			ProcessPage(controller, masterHandler, httpContext);
		}

		#endregion

		private IHttpHandler GetCompiledPageInstace(string viewName, HttpContext httpContext)
		{
			viewName += ".aspx";

			//TODO: There needs to be a more efficient way to do this than two replace operations
			String physicalPath = Path.Combine(ViewRootDir, viewName).Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);

			return PageParser.GetCompiledPageInstance(viewName, physicalPath, httpContext);
		}

		private bool IsTheSameView(HttpContext httpContext, string viewName)
		{
			string original = ((string) httpContext.Items[Controller.OriginalViewKey]);
			string actual = viewName;

			return String.Compare(original, actual, true) == 0;
		}

		private void ProcessPage(Controller controller, IHttpHandler page, HttpContext httpContext)
		{
			controller.PreSendView(page);
	
			page.ProcessRequest(httpContext);
	
			controller.PostSendView(page);
		}

		private void ProcessLayoutIfNeeded(Controller controller, HttpContext httpContext, IHttpHandler childPage, Page masterHandler)
		{
			if (HasLayout(controller))
			{
				if (httpContext.Response.StatusCode == 200)
				{
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
				}
				else
				{
					WriteBuffered(httpContext.Response);
				}
			}
		}

		private bool HasLayout(Controller controller)
		{
			return controller.LayoutName != null;
		}

		private void WriteBuffered(HttpResponse response)
		{
			response.Flush();

			// Restores the original stream
			DelegateMemoryStream filter = (DelegateMemoryStream) response.Filter;
			response.Filter = filter.OriginalStream;
			
			// Writes the buffered contents
			byte[] buffer = filter.GetBuffer();
			response.OutputStream.Write(buffer, 0, buffer.Length);
		}

		private byte[] RestoreFilter(HttpResponse response)
		{
			response.Flush();

			// Restores the original stream
			DelegateMemoryStream filter = (DelegateMemoryStream) response.Filter;
			response.Filter = filter.OriginalStream;

			return filter.GetBuffer();
		}

		private Page ObtainMasterPage(HttpContext context, Controller controller)
		{
			String layout = "layouts/" + controller.LayoutName + ".aspx";
			String physicalPath = Path.Combine( ViewRootDir, layout );
			physicalPath = physicalPath.Replace('/', '\\');

			return PageParser.GetCompiledPageInstance(layout, physicalPath, context) as Page;
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
