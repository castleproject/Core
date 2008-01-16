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

namespace Castle.MonoRail.Framework.Services
{
	using System;
	using System.Collections;
	using Internal;
	using Routing;

	#region UrlBuilderParameters

	/// <summary>
	/// Represents an static typed definition of the
	/// <see cref="IUrlBuilder"/> parameters.
	/// </summary>
	public class UrlBuilderParameters
	{
		private string area, controller, action;
		private string domain, subdomain, protocol, basePath, pathInfo;
		private int port;
		private object routeParameters, queryString;
		private IDictionary customParameters;
		private bool encodeForLink, createAbsolutePath, useCurrentRouteParams;
		private RouteMatch routeMatch;

		/// <summary>
		/// Initializes a new instance of the <see cref="UrlBuilderParameters"/> class.
		/// </summary>
		public UrlBuilderParameters(string area, string controller, string action)
		{
			this.area = area;
			this.controller = controller;
			this.action = action;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UrlBuilderParameters"/> class.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="action">The action.</param>
		public UrlBuilderParameters(string controller, string action)
		{
			this.controller = controller;
			this.action = action;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UrlBuilderParameters"/> class.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="action">The action.</param>
		/// <param name="queryString">The query string.</param>
		public UrlBuilderParameters(string controller, string action, object queryString)
		{
			this.controller = controller;
			this.action = action;
			this.queryString = queryString;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UrlBuilderParameters"/> class.
		/// </summary>
		private UrlBuilderParameters(string area, string controller, string action, 
			bool createAbsolutePath, string basePath, string domain, string subdomain, string protocol, int port, 
			string pathInfo, object queryString, bool encodeForLink, IDictionary customParameters, 
			object routeParams) : 
			this(area, controller, action)
		{
			this.createAbsolutePath = createAbsolutePath;
			this.basePath = basePath;
			this.domain = domain;
			this.subdomain = subdomain;
			this.protocol = protocol;
			this.port = port;
			this.pathInfo = pathInfo;
			this.queryString = queryString;
			this.encodeForLink = encodeForLink;
			this.customParameters = customParameters;

			routeParameters = routeParams;
		}

		/// <summary>
		/// Creates an <see cref="UrlBuilderParameters"/> from the 
		/// dictionary
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		public static UrlBuilderParameters From(IDictionary parameters)
		{
			object routeParams = CommonUtils.ObtainObjectEntryAndRemove(parameters, "params");

			return From(parameters, routeParams);
		}

		/// <summary>
		/// Creates an <see cref="UrlBuilderParameters"/> from the
		/// dictionary
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <param name="routeParams">The route params.</param>
		/// <returns></returns>
		public static UrlBuilderParameters From(IDictionary parameters, object routeParams)
		{
			bool encodeForLink = CommonUtils.ObtainEntryAndRemove(parameters, "encode", "false") == "true";
			bool useCurrentRouteParams = CommonUtils.ObtainEntryAndRemove(parameters, "useCurrentRouteParams", "false") == "true";

			string area = CommonUtils.ObtainEntryAndRemove(parameters, "area");
			string controller = CommonUtils.ObtainEntryAndRemove(parameters, "controller");
			string action = CommonUtils.ObtainEntryAndRemove(parameters, "action");

			bool createAbsolutePath = CommonUtils.ObtainEntryAndRemove(parameters, "absolute", "false") == "true";
			string basePath = CommonUtils.ObtainEntryAndRemove(parameters, "basePath");
			string domain = CommonUtils.ObtainEntryAndRemove(parameters, "domain");
			string subdomain = CommonUtils.ObtainEntryAndRemove(parameters, "subdomain");
			string protocol = CommonUtils.ObtainEntryAndRemove(parameters, "protocol");
			int port = Convert.ToInt32(CommonUtils.ObtainEntryAndRemove(parameters, "port", "0"));

			string pathInfo = CommonUtils.ObtainEntryAndRemove(parameters, "pathinfo");
			object queryString = CommonUtils.ObtainObjectEntryAndRemove(parameters, "querystring");
			RouteMatch routeMatch = (RouteMatch) CommonUtils.ObtainObjectEntryAndRemove(parameters, "routeMatch");

			return new UrlBuilderParameters(area, controller, action, 
				createAbsolutePath, basePath, domain, subdomain, protocol, port, 
				pathInfo, queryString, 
				encodeForLink, parameters, routeParams).
				SetRouteMatch(useCurrentRouteParams, routeMatch);
		}

		/// <summary>
		/// Sets the query string.
		/// </summary>
		/// <param name="queryString">The query string.</param>
		/// <returns></returns>
		public UrlBuilderParameters SetQueryString(object queryString)
		{
			this.queryString = queryString;
			return this;
		}

		/// <summary>
		/// Sets the route match.
		/// </summary>
		/// <param name="useCurrentRouteParams">if set to <c>true</c> [use current route params].</param>
		/// <param name="routeMatch">The route match.</param>
		/// <returns></returns>
		public UrlBuilderParameters SetRouteMatch(bool useCurrentRouteParams, RouteMatch routeMatch)
		{
			this.useCurrentRouteParams = useCurrentRouteParams;
			this.routeMatch = routeMatch;
			return this;
		}

		/// <summary>
		/// Sets the route match.
		/// </summary>
		/// <param name="routeMatch">The route match.</param>
		/// <returns></returns>
		public UrlBuilderParameters SetRouteMatch(RouteMatch routeMatch)
		{
			this.routeMatch = routeMatch;
			return this;
		}

		/// <summary>
		/// Gets or sets a value indicating whether [create absolute path].
		/// </summary>
		/// <value><c>true</c> if [create absolute path]; otherwise, <c>false</c>.</value>
		public bool CreateAbsolutePath
		{
			get { return createAbsolutePath; }
			set { createAbsolutePath = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether [encode for link].
		/// </summary>
		/// <value><c>true</c> if [encode for link]; otherwise, <c>false</c>.</value>
		public bool EncodeForLink
		{
			get { return encodeForLink; }
			set { encodeForLink = value; }
		}

		/// <summary>
		/// Gets or sets the area.
		/// </summary>
		/// <value>The area.</value>
		public string Area
		{
			get { return area; }
			set { area = value; }
		}

		/// <summary>
		/// Gets or sets the controller.
		/// </summary>
		/// <value>The controller.</value>
		public string Controller
		{
			get { return controller; }
			set { controller = value; }
		}

		/// <summary>
		/// Gets or sets the action.
		/// </summary>
		/// <value>The action.</value>
		public string Action
		{
			get { return action; }
			set { action = value; }
		}

		/// <summary>
		/// Gets or sets the domain.
		/// </summary>
		/// <value>The domain.</value>
		public string Domain
		{
			get { return domain; }
			set { domain = value; }
		}

		/// <summary>
		/// Gets or sets the subdomain.
		/// </summary>
		/// <value>The subdomain.</value>
		public string Subdomain
		{
			get { return subdomain; }
			set { subdomain = value; }
		}

		/// <summary>
		/// Gets or sets the protocol.
		/// </summary>
		/// <value>The protocol.</value>
		public string Protocol
		{
			get { return protocol; }
			set { protocol = value; }
		}

		/// <summary>
		/// Gets or sets the base path.
		/// </summary>
		/// <value>The base path.</value>
		public string BasePath
		{
			get { return basePath; }
			set { basePath = value; }
		}

		/// <summary>
		/// Gets or sets the port.
		/// </summary>
		/// <value>The port.</value>
		public int Port
		{
			get { return port; }
			set { port = value; }
		}

		/// <summary>
		/// Gets or sets the path info.
		/// </summary>
		/// <value>The path info.</value>
		public string PathInfo
		{
			get { return pathInfo; }
			set { pathInfo = value; }
		}

		/// <summary>
		/// Gets or sets the query string.
		/// </summary>
		/// <value>The query string.</value>
		public object QueryString
		{
			get { return queryString; }
			set { queryString = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether [use current route params].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [use current route params]; otherwise, <c>false</c>.
		/// </value>
		public bool UseCurrentRouteParams
		{
			get { return useCurrentRouteParams; }
			set { useCurrentRouteParams = value; }
		}

		/// <summary>
		/// Gets or sets the route parameters.
		/// </summary>
		/// <value>The route parameters.</value>
		public object RouteParameters
		{
			get { return routeParameters; }
			set { routeParameters = value; }
		}

		/// <summary>
		/// Gets or sets the route match.
		/// </summary>
		/// <value>The route match.</value>
		public RouteMatch RouteMatch
		{
			get { return routeMatch; }
			set { routeMatch = value; }
		}

		/// <summary>
		/// Gets or sets the custom parameters.
		/// </summary>
		/// <value>The custom parameters.</value>
		public IDictionary CustomParameters
		{
			get { return customParameters; }
			set { customParameters = value; }
		}
	}

	#endregion

	/// <summary>
	/// THe UrlBuilder service centralizes the url generation used by the whole 
	/// framework including redirect urls, urls generated by helpers and so on. 
	/// It offers a central place to change MonoRail behavior on how to deal with urls. 
	/// </summary>
	public interface IUrlBuilder
	{
		/// <summary>
		/// Gets or sets a value indicating whether the builder should output an extension.
		/// </summary>
		/// <value><c>true</c> if should use extensions; otherwise, <c>false</c>.</value>
		bool UseExtensions { get; set; }

		/// <summary>
		/// Builds the URL using the current url as contextual information and the specified parameters.
		/// <para>
		/// Common parameters includes area, controller and action. See <see cref="UrlBuilderParameters"/>
		/// for more information regarding the parameters. 
		/// </para>
		/// </summary>
		/// <param name="current">The current Url information.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		string BuildUrl(UrlInfo current, UrlBuilderParameters parameters);

		/// <summary>
		/// Builds the URL using the current url as contextual information and the specified parameters.
		/// <para>
		/// Common parameters includes area, controller and action. See <see cref="UrlBuilderParameters"/>
		/// for more information regarding the parameters.
		/// </para>
		/// </summary>
		/// <param name="current">The current Url information.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="routeParameters">The route parameters.</param>
		/// <returns></returns>
		string BuildUrl(UrlInfo current, UrlBuilderParameters parameters, IDictionary routeParameters);

		/// <summary>
		/// Builds the URL using the current url as contextual information and the specified parameters.
		/// <para>
		/// Common parameters includes area, controller and action. See <see cref="UrlBuilderParameters"/>
		/// for more information regarding the parameters.
		/// </para>
		/// </summary>
		/// <param name="current">The current Url information.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		string BuildUrl(UrlInfo current, IDictionary parameters);

		/// <summary>
		/// Builds the URL using the current url as contextual information and the specified parameters.
		/// <para>
		/// Common parameters includes area, controller and action. See <see cref="UrlBuilderParameters"/>
		/// for more information regarding the parameters.
		/// </para>
		/// </summary>
		/// <param name="current">The current Url information.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="routeParameters">The route parameters.</param>
		/// <returns></returns>
		string BuildUrl(UrlInfo current, IDictionary parameters, IDictionary routeParameters);

		/// <summary>
		/// Builds the URL using the current url as contextual information and the specified parameters.
		/// <para>
		/// Common parameters includes area, controller and action. See <see cref="UrlBuilderParameters"/>
		/// for more information regarding the parameters.
		/// </para>
		/// </summary>
		/// <param name="current">The current Url information.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		UrlParts CreateUrlPartsBuilder(UrlInfo current, UrlBuilderParameters parameters);

		/// <summary>
		/// Builds the URL using the current url as contextual information and the specified parameters.
		/// <para>
		/// Common parameters includes area, controller and action. See <see cref="UrlBuilderParameters"/>
		/// for more information regarding the parameters.
		/// </para>
		/// </summary>
		/// <param name="current">The current Url information.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="routeParameters">The route parameters.</param>
		/// <returns></returns>
		UrlParts CreateUrlPartsBuilder(UrlInfo current, UrlBuilderParameters parameters, IDictionary routeParameters);

		/// <summary>
		/// Builds the URL using the current url as contextual information and the specified parameters.
		/// <para>
		/// Common parameters includes area, controller and action. See <see cref="UrlBuilderParameters"/>
		/// for more information regarding the parameters.
		/// </para>
		/// </summary>
		/// <param name="current">The current Url information.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		UrlParts CreateUrlPartsBuilder(UrlInfo current, IDictionary parameters);

		/// <summary>
		/// Builds the URL using the current url as contextual information and the specified parameters.
		/// <para>
		/// Common parameters includes area, controller and action. See <see cref="UrlBuilderParameters"/>
		/// for more information regarding the parameters.
		/// </para>
		/// </summary>
		/// <param name="current">The current Url information.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="routeParameters">The route parameters.</param>
		/// <returns></returns>
		UrlParts CreateUrlPartsBuilder(UrlInfo current, IDictionary parameters, IDictionary routeParameters);
	}
}
