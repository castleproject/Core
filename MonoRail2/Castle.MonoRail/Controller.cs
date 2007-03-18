namespace Castle.MonoRail
{
	public class Controller : IController
	{
		private string areaName, controllerName, actionName;

		#region IController

		public void SetInitialState(string area, string controller, string action)
		{
			areaName = area;
			controllerName = controller;
			actionName = action;
		}

		#endregion

		public string AreaName
		{
			get { return areaName; }
		}

		public string Name
		{
			get { return controllerName; }
		}

		public string ActionName
		{
			get { return actionName; }
		}
	}
}
