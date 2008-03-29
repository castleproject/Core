// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Web;

	/// <summary>
	/// Implements <see cref="IHttpAsyncHandler"/> to dispatch the web
	/// requests in async manner
	/// <seealso cref="MonoRailHttpHandlerFactory"/>
	/// </summary>
	public class BaseAsyncHttpHandler : BaseHttpHandler, IHttpAsyncHandler
	{
		private readonly IAsyncController asyncController;
		private HttpContext httpContext;

		/// <summary>
		/// Initializes a new instance of the <see cref="MonoRailHttpHandler"/> class.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The asyncController.</param>
		/// <param name="context">The context.</param>
		/// <param name="sessionLess">Have session?</param>
		public BaseAsyncHttpHandler(IEngineContext engineContext, IAsyncController controller, IControllerContext context,
		                            bool sessionLess)
			: base(engineContext, controller, context, sessionLess)
		{
			this.asyncController = controller;
		}

		/// <summary>
		/// Initiates an asynchronous call to the HTTP handler.
		/// </summary>
		/// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
		/// <param name="cb">The <see cref="T:System.AsyncCallback"/> to call when the asynchronous method call is complete. If <paramref name="cb"/> is null, the delegate is not called.</param>
		/// <param name="extraData">Any extra data needed to process the request.</param>
		/// <returns>
		/// An <see cref="T:System.IAsyncResult"/> that contains information about the status of the process.
		/// </returns>
		public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
		{
			this.httpContext = context;
			BeforeControllerProcess(context);

			try
			{
				controllerContext.Async.Callback = cb;
				controllerContext.Async.State = extraData;

				engineContext.Services.ExtensionManager.RaisePreProcessController(engineContext);

				return asyncController.BeginProcess(engineContext, controllerContext);
			}
			catch(Exception ex)
			{
				HttpResponse response = context.Response;

				if (response.StatusCode == 200)
				{
					response.StatusCode = 500;
				}

				engineContext.LastException = ex;

				engineContext.Services.ExtensionManager.RaiseUnhandledError(engineContext);

				AfterControllerProcess();

				throw new MonoRailException("Error processing MonoRail request. Action " +
				                            controllerContext.Action + " on asyncController " + controllerContext.Name, ex);
			}
		}

		/// <summary>
		/// Provides an asynchronous process End method when the process ends.
		/// </summary>
		/// <param name="result">An <see cref="T:System.IAsyncResult"/> that contains information about the status of the process.</param>
		public void EndProcessRequest(IAsyncResult result)
		{
			try
			{
				controllerContext.Async.Result = result;
				// if we failed on the Begin[Action] and had a rescue take care of rendering the output
				// we won't be executing the End[Action] part
				if (result is FailedToExecuteBeginActionAsyncResult == false)
				{
					asyncController.EndProcess();
				}

				engineContext.Services.ExtensionManager.RaisePostProcessController(engineContext);
			}
			catch(Exception ex)
			{
				HttpResponse response = httpContext.Response;

				if (response.StatusCode == 200)
				{
					response.StatusCode = 500;
				}

				engineContext.LastException = ex;

				engineContext.Services.ExtensionManager.RaiseUnhandledError(engineContext);

				throw new MonoRailException("Error processing MonoRail request. Action " +
				                            controllerContext.Action + " on asyncController " + controllerContext.Name, ex);
			}
			finally
			{
				AfterControllerProcess();
			}
		}
	}
}