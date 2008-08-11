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

	/// <summary>
	/// Represents the tokenized information from an Url.
	/// </summary>
	[Serializable]
	public class UrlInfo
	{
		private readonly int port;
		private readonly string domain, subdomain, appVirtualDir, protocol;
		private readonly string urlRaw;
		private readonly string area, controller, action, extension;
		private readonly string pathInfo;

		/// <summary>
		/// Initializes a new instance of the <see cref="UrlInfo"/> class.
		/// </summary>
		/// <param name="area">The area.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="action">The action.</param>
		public UrlInfo(string area, string controller, string action)
		{
			this.area = area;
			this.controller = controller;
			this.action = action;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UrlInfo"/> class.
		/// </summary>
		/// <param name="area">The area.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="action">The action.</param>
		/// <param name="appVirtualDir">The app virtual dir.</param>
		/// <param name="extension">The extension.</param>
		public UrlInfo(string area, string controller, string action, string appVirtualDir, string extension)
		{
			this.area = area;
			this.controller = controller;
			this.action = action;
			this.appVirtualDir = appVirtualDir;
			this.extension = extension;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UrlInfo"/> class.
		/// </summary>
		/// <param name="domain">The domain (host).</param>
		/// <param name="subdomain">The subdomain (first token on the domain).</param>
		/// <param name="appVirtualDir">The application virtual dir.</param>
		/// <param name="protocol">Protocol (http/https)</param>
		/// <param name="port">The port.</param>
		/// <param name="urlRaw">The raw URL.</param>
		/// <param name="area">The area, or empty.</param>
		/// <param name="controller">The controller name.</param>
		/// <param name="action">The action name.</param>
		/// <param name="extension">The file extension.</param>
		/// <param name="pathInfo">The path info.</param>
		public UrlInfo(string domain, string subdomain, string appVirtualDir, string protocol, int port, string urlRaw,
					   string area, string controller, string action, string extension, string pathInfo)
		{
			this.port = port;
			this.domain = domain;
			this.subdomain = subdomain;
			this.urlRaw = urlRaw;
			this.area = area;
			this.controller = controller;
			this.action = action;
			this.extension = extension;
			this.pathInfo = pathInfo;
			this.appVirtualDir = appVirtualDir;
			this.protocol = protocol;
		}

		/// <summary>
		/// Gets the app virtual dir.
		/// </summary>
		/// <value>The app virtual dir.</value>
		public string AppVirtualDir
		{
			get { return appVirtualDir; }
		}

		/// <summary>
		/// Gets the port.
		/// </summary>
		/// <value>The port.</value>
		public int Port
		{
			get { return port; }
		}

		/// <summary>
		/// Gets the domain.
		/// </summary>
		/// <value>The domain.</value>
		public string Domain
		{
			get { return domain; }
		}

		/// <summary>
		/// Gets the subdomain.
		/// </summary>
		/// <value>The subdomain.</value>
		public string Subdomain
		{
			get { return subdomain; }
		}

		/// <summary>
		/// Gets the subdomain.
		/// </summary>
		/// <value>The subdomain.</value>
		public string FullDomain
		{
			get
			{
				if (!string.IsNullOrEmpty(subdomain))
					return subdomain + "." + domain;
				else
					return domain;
			}
		}

		/// <summary>
		/// Gets the URL raw.
		/// </summary>
		/// <value>The URL raw.</value>
		public string UrlRaw
		{
			get { return urlRaw; }
		}

		/// <summary>
		/// Gets the area.
		/// </summary>
		/// <value>The area.</value>
		public string Area
		{
			get { return area; }
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
		/// Gets the protocol.
		/// </summary>
		/// <value>The protocol.</value>
		public string Protocol
		{
			get { return protocol; }
		}

		/// <summary>
		/// The URL extension, without the leading dot.
		/// </summary>
		public string Extension
		{
			get { return extension; }
		}

		/// <summary>
		/// Gets the path info.
		/// </summary>
		/// <value>The path info.</value>
		public string PathInfo
		{
			get { return pathInfo; }
		}
	}
}
