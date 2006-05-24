using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Castle.MicroKernel;
using NShop.Services;
using System.Collections.Generic;
using NShop.Views;

namespace NShop
{
	public class FrontController : IHttpHandlerFactory
	{
		Dictionary<IHttpHandler, IController> handler2Controller = new Dictionary<IHttpHandler, IController>();

		#region IHttpHandlerFactory Members

		public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
		{			
			string controllerName = PathToControllerName(context);
			Container current = Container.Current;
		    IController controller = current.Resolve<IController>(controllerName);
			controller.Process(context);
			if (controller.View == null)
				throw new InvalidOperationException("Controller doesn't have an associated view.");
			string fullView = controller.View + ".aspx";
			IHttpHandler instance = PageParser.GetCompiledPageInstance(url, context.Server.MapPath(fullView), context);
			((IView) instance).Items = controller.Items;
			lock (handler2Controller)
			{
				handler2Controller[instance] = controller;
			}
			return instance;
		}

		private static string PathToControllerName(HttpContext context)
		{
		    string path = context.Request.Path;
            // get rid of the '.aspx' post-fix		    
            path = path.Substring(0, path.Length - 5);
            return path.ToLower();
		}

		public void ReleaseHandler(IHttpHandler handler)
		{
			IController controller;
			lock (handler2Controller)
			{
				if (handler2Controller.TryGetValue(handler, out controller))
					handler2Controller.Remove(handler);
			}
			if (controller == null)
				return;
			controller.End();
			Container.Current.Release(controller);
		}

		#endregion

	}

}

