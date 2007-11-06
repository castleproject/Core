namespace Castle.MonoRail.Framework
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.IO;
	using Internal;

	/// <summary>
	/// Represent the core functionality required out of a controller
	/// </summary>
	public interface IController : IDisposable
	{
		/// <summary>
		/// Gets the view folder -- (areaname + 
		/// controllername) or just controller name -- that this controller 
		/// will use by default.
		/// </summary>
		string ViewFolder { get; }

		/// <summary>
		/// Gets a dicitionary of name/<see cref="IResource"/>
		/// </summary>
		/// <remarks>It is supposed to be used by MonoRail infrastructure only</remarks>
		/// <value>The resources.</value>
		ResourceDictionary Resources { get; }

		/// <summary>
		/// Gets a dictionary of name/helper instance
		/// </summary>
		/// <value>The helpers.</value>
		IDictionary Helpers { get; }

		/// <summary>
		/// Gets the controller's name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the controller's area name.
		/// </summary>
		string AreaName { get; }

		/// <summary>
		/// Gets or set the layout being used.
		/// </summary>
		string LayoutName { get; set; }

		/// <summary>
		/// Gets the name of the action being processed.
		/// </summary>
		string Action { get; }

		/// <summary>
		/// Gets or sets the view which will be rendered by this action.
		/// </summary>
		string SelectedViewName { get; set; }

		/// <summary>
		/// Gets the property bag, which is used
		/// to pass variables to the view.
		/// </summary>
		IDictionary PropertyBag { get; set; }

		/// <summary>
		/// Gets a dictionary of volative items.
		/// Ideal for showing success and failures messages.
		/// </summary>
		Flash Flash { get; }

		/// <summary>
		/// Gets the request object.
		/// </summary>
		IRequest Request { get; }

		/// <summary>
		/// Gets the response object.
		/// </summary>
		IResponse Response { get; }

		/// <summary>
		/// Shortcut to <see cref="IRequest.Params"/> 
		/// </summary>
		NameValueCollection Params { get; }

		/// <summary>
		/// Shortcut to <see cref="IRequest.Form"/> 
		/// </summary>
		NameValueCollection Form { get; }

		/// <summary>
		/// Shortcut to <see cref="IRequest.QueryString"></see>
		/// </summary>
		NameValueCollection Query { get; }

		/// <summary>
		/// Performs the specified action, which means:
		/// <br/>
		/// 1. Define the default view name<br/>
		/// 2. Run the before filters<br/>
		/// 3. Select the method related to the action name and invoke it<br/>
		/// 4. On error, execute the rescues if available<br/>
		/// 5. Run the after filters<br/>
		/// 6. Invoke the view engine<br/>
		/// </summary>
		/// <param name="action">Action name</param>
		void Send(string action);

		/// <summary>
		/// Performs the specified action with arguments.
		/// </summary>
		/// <param name="action">Action name</param>
		/// <param name="actionArgs">Action arguments</param>
		void Send(string action, IDictionary actionArgs);

		/// <summary>
		/// Invoked by the view engine to perform
		/// any logic before the view is sent to the client.
		/// </summary>
		/// <param name="view"></param>
		void PreSendView(object view);

		/// <summary>
		/// Invoked by the view engine to perform
		/// any logic after the view had been sent to the client.
		/// </summary>
		/// <param name="view"></param>
		void PostSendView(object view);

		/// <summary>
		/// Specifies the shared view to be processed and results are written to System.IO.TextWriter.
		/// (A partial view shared by others views and usually in the root folder
		/// of the view directory).
		/// </summary>
		/// <param name="output"></param>
		/// <param name="name">The name of the view to process.</param>
		void InPlaceRenderSharedView(TextWriter output, string name);
	}
}