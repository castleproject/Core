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

	/// <summary>
	/// Summary description for AspNetViewEngine.
	/// </summary>
	public class AspNetViewEngine : IViewEngine
	{
		public AspNetViewEngine()
		{
		}

		public virtual void Process(Controller controller, String url, String viewPath, 
			String viewName, HttpContext context)
		{
			// String viewUrl = viewName.Replace('\\', '/');

			if (!Path.IsPathRooted(viewPath))
			{
				viewPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, viewPath);
			}

			String physicalPath = Path.Combine( viewPath, viewName + ".aspx" );
			physicalPath = physicalPath.Replace('/', '\\');

			IHttpHandler handler = 
				PageParser.GetCompiledPageInstance( viewName, physicalPath, context );

			ProcessPropertyBag( controller.PropertyBag, handler );
			
			controller.PreSendView( handler );

			handler.ProcessRequest(context);

			controller.PostSendView( handler );
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
}
