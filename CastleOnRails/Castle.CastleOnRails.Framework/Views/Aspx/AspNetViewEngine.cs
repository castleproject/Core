// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.CastleOnRails.Framework.Views.Aspx
{
	using System;
	using System.Web;
	using System.Web.UI;
	using System.IO;
	using System.Collections;
	using System.Reflection;

	public class AspNetViewEngine : IViewEngine
	{
		private String _viewRootDir;

		public AspNetViewEngine()
		{
		}

		#region IViewEngine

		public String ViewRootDir
		{
			get { return _viewRootDir; }
			set { _viewRootDir = value; }
		}

		public void Process(IRailsEngineContext context, Controller controller, String viewName)
		{
			if (!Path.IsPathRooted(_viewRootDir))
			{
				_viewRootDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _viewRootDir);
			}

			HttpContext httpContext = context.UnderlyingContext as HttpContext;
			Page masterHandler = null;

			if (controller.LayoutName != null)
			{
				StartFiltering(httpContext.Response);
				masterHandler = ObtainMasterPage(httpContext, controller);
			}

			String physicalPath = Path.Combine( _viewRootDir, viewName + ".aspx" );
			physicalPath = physicalPath.Replace('/', '\\');

			IHttpHandler childPage = 
				PageParser.GetCompiledPageInstance(viewName, physicalPath, httpContext);

			ProcessPropertyBag(controller.PropertyBag, childPage);

			controller.PreSendView(childPage);

			childPage.ProcessRequest(httpContext);

			controller.PostSendView(childPage);

			if (controller.LayoutName != null)
			{
				if (httpContext.Response.StatusCode == 200)
				{
					byte[] contents = RestoreFilter(httpContext.Response);

					httpContext.Items.Add("rails.contents", contents);
					httpContext.Items.Add("rails.child", childPage);

					masterHandler.ProcessRequest(httpContext);
				}
				else
				{
					WriteBuffered(httpContext.Response);
				}
			}
		}

		#endregion

		private void WriteBuffered(HttpResponse response)
		{
//			DelegateMemoryStream filter = (DelegateMemoryStream) response.Filter;
//			byte[] buffer = filter.GetBuffer();
//			response.OutputStream.Write( buffer, 0, buffer.Length );
		}

		private byte[] RestoreFilter(HttpResponse response)
		{
			response.Flush();
			DelegateMemoryStream filter = (DelegateMemoryStream) response.Filter;
			response.Filter = filter.OriginalStream;
			return filter.GetBuffer();
		}

		private Page ObtainMasterPage(HttpContext context, Controller controller)
		{
			String layout = "layouts/" + controller.LayoutName;
			String physicalPath = Path.Combine( _viewRootDir, layout + ".aspx" );
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
