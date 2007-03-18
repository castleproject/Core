namespace Castle.MonoRail
{
	using System;

	public class ControllerExecutor
	{
		public ControllerExecutor(IController controller, IExecutionContext executionContext)
		{
			if (controller == null) throw new ArgumentNullException("controller");
			if (executionContext == null) throw new ArgumentNullException("executionContext");

			UrlInfo url = executionContext.OriginalUrl;

			controller.SetInitialState(url.Area, url.Controller, url.Action);
		}
	}
}
