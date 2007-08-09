namespace Castle.MonoRail.Framework.Configuration
{
	using System;
	using System.Configuration;
	using System.Xml;

	public class DefaultUrl : ISerializedConfig
	{
		private string url, controller, action, area;

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultUrl"/> class.
		/// </summary>
		public DefaultUrl()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultUrl"/> class.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="action">The action.</param>
		/// <param name="area">The area.</param>
		public DefaultUrl(string url, string controller, string action, string area)
		{
			this.url = url;
			this.controller = controller;
			this.action = action;
			this.area = area;
		}

		#region ISerializedConfig implementation

		public void Deserialize(XmlNode section)
		{
			XmlAttribute urlAtt = section.Attributes["url"];
			XmlAttribute controllerAtt = section.Attributes["controller"];
			XmlAttribute actionAtt = section.Attributes["action"];
			XmlAttribute areaAtt = section.Attributes["area"];

			if ((urlAtt == null || urlAtt.Value == String.Empty) ||
				(controllerAtt == null || controllerAtt.Value == String.Empty) ||
				(actionAtt == null || actionAtt.Value == String.Empty))
			{
				String message = "To add a default url rule, please specify the 'url', 'controller', 'action' and optionally 'area' attributes. " +
					"Check the documentation for more information";
				throw new ConfigurationErrorsException(message);
			}

			url = urlAtt.Value;
			action = actionAtt.Value;
			controller = controllerAtt.Value;
			
			if (areaAtt != null)
			{
				area = areaAtt.Value;
			}
		}

		#endregion

		/// <summary>
		/// Gets the URL.
		/// </summary>
		/// <value>The URL.</value>
		public string Url
		{
			get { return url; }
		}

		/// <summary>
		/// Gets the controller.
		/// </summary>
		/// <value>The controller.</value>
		public string Controller
		{
			get { return controller; }
		}

		/// <summary>
		/// Gets the action.
		/// </summary>
		/// <value>The action.</value>
		public string Action
		{
			get { return action; }
		}

		/// <summary>
		/// Gets the area.
		/// </summary>
		/// <value>The area.</value>
		public string Area
		{
			get { return area; }
		}
	}
}
