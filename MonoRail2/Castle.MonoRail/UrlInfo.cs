namespace Castle.MonoRail
{
	public class UrlInfo
	{
		private readonly string area, controller, action;

		public UrlInfo(string area, string controller, string action)
		{
			this.area = area;
			this.controller = controller;
			this.action = action;
		}

		public string Area
		{
			get { return area; }
		}

		public string Controller
		{
			get { return controller; }
		}

		public string Action
		{
			get { return action; }
		}
	}
}
