namespace Castle.MonoRail.Framework
{
	using System;

	/// <summary>
	/// Represent the core functionality required out of a controller
	/// that wants to support async actions
	/// </summary>
	public interface IAsyncController : IController
	{
		/// <summary>
		/// Begin to perform the specified async action, which means:
		/// <br/>
		/// 1. Define the default view name<br/>
		/// 2. Run the before filters<br/>
		/// 3. Select the begin method related to the action name and invoke it<br/>
		/// 4. Return the result of the async method start and let ASP.Net wait on it
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="context">The controller context.</param>
		/// <remarks>
		/// The async infomrmation about this call is pass using the controller context Async property
		/// </remarks>
		IAsyncResult BeginProcess(IEngineContext engineContext, IControllerContext context);

		/// <summary>
		/// Complete processing of the request:<br/>
		/// 1. Execute end method related to the action name<br/>
		/// 2. On error, execute the rescues if available<br/>
		/// 3. Run the after filters<br/>
		/// 4. Invoke the view engine<br/>
		/// </summary>
		/// <remarks>
		/// The async infomrmation about this call is pass using the controller context Async property
		/// </remarks>
		void EndProcess();
	}
}