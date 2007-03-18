namespace Castle.MonoRail
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Manages the execution of the steps associated with 
	/// a controller (filters/action/disposal).
	/// </summary>
	/// <remarks>
	/// This class is statefull.
	/// </remarks>
	public class ControllerExecutor
	{
		private readonly IController controller;
		private readonly IExecutionContext executionContext;

		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerExecutor"/> class.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="executionContext">The execution context.</param>
		public ControllerExecutor(IController controller, IExecutionContext executionContext)
		{
			if (controller == null) throw new ArgumentNullException("controller");
			if (executionContext == null) throw new ArgumentNullException("executionContext");

			UrlInfo url = executionContext.OriginalUrl;

			controller.SetInitialState(url.Area, url.Controller, url.Action);

			this.controller = controller;
			this.executionContext = executionContext;
		}

		public ActionExecutor SelectAction()
		{
			return SelectAction(executionContext.OriginalUrl.Action);
		}

		public ActionExecutor SelectAction(string actionName)
		{
			// There is room for a clever cache here

			// Right now let's support method action only

			BindingFlags flags = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;

			MethodInfo actionMethod = controller.GetType().GetMethod(actionName, flags);

			if (actionMethod != null)
			{
				return new MethodActionExecutor(actionMethod);
			}

			return null;
		}
	}
}