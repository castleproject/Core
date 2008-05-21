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
	using System.Collections.Specialized;
	using Castle.Core;
	using Castle.MonoRail.Framework.Internal;
	using Configuration;
	using Framework;

	/// <summary>
	/// Default implementation of <see cref="IUrlBuilder"/>	
	/// </summary>
	/// <remarks>
	/// The property <see cref="UseExtensions"/> defines whether the builder should output
	/// file extension. This might be handy to use in combination with a url rewrite strategy
	/// </remarks>
	/// <seealso cref="BuildUrl(UrlInfo,IDictionary)"/>
	public class DefaultUrlBuilder : IUrlBuilder, IServiceEnabledComponent
	{
		private bool useExtensions = true;
		private IServerUtility serverUtil;
		private IRoutingEngine routingEng;

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultUrlBuilder"/> class.
		/// </summary>
		public DefaultUrlBuilder()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultUrlBuilder"/> class.
		/// </summary>
		/// <param name="serverUtil">The server util.</param>
		/// <param name="routingEng">The routing eng.</param>
		public DefaultUrlBuilder(IServerUtility serverUtil, IRoutingEngine routingEng)
		{
			this.serverUtil = serverUtil;
			this.routingEng = routingEng;
		}

		#region Properties

		/// <summary>
		/// Gets or sets a value indicating whether the builder should output an extension.
		/// </summary>
		/// <value><c>true</c> if should use extensions; otherwise, <c>false</c>.</value>
		public bool UseExtensions
		{
			get { return useExtensions; }
			set { useExtensions = value; }
		}

		/// <summary>
		/// Gets or sets the server utility instance.
		/// </summary>
		/// <value>The server util.</value>
		public IServerUtility ServerUtil
		{
			get { return serverUtil; }
			set { serverUtil = value; }
		}

		/// <summary>
		/// Gets or sets the routing engine.
		/// </summary>
		/// <value>The routing engine.</value>
		public IRoutingEngine RoutingEngine
		{
			get { return routingEng; }
			set { routingEng = value; }
		}

		#endregion

		#region IServiceEnabledComponent

		/// <summary>
		/// Services the specified provider.
		/// </summary>
		/// <param name="provider">The provider.</param>
		public void Service(IServiceProvider provider)
		{
			IMonoRailConfiguration config = (IMonoRailConfiguration) provider.GetService(typeof(IMonoRailConfiguration));
			useExtensions = config.UrlConfig.UseExtensions;

			serverUtil = (IServerUtility) provider.GetService(typeof(IServerUtility));
			routingEng = (IRoutingEngine) provider.GetService(typeof(IRoutingEngine));
		}

		#endregion

		#region IUrlBuilder

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
		public virtual string BuildUrl(UrlInfo current, IDictionary parameters)
		{
			AssertArguments(current, parameters);

			UrlBuilderParameters typedParams = UrlBuilderParameters.From(parameters);

			return BuildUrl(current, typedParams, ConvertRouteParams(typedParams.RouteParameters));
		}

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
		public virtual string BuildUrl(UrlInfo current, IDictionary parameters, IDictionary routeParameters)
		{
			AssertArguments(current, parameters);

			UrlBuilderParameters typedParams = UrlBuilderParameters.From(parameters, routeParameters);

			return BuildUrl(current, typedParams, ConvertRouteParams(typedParams.RouteParameters));
		}

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
		public virtual string BuildUrl(UrlInfo current, UrlBuilderParameters parameters)
		{
			AssertArguments(current, parameters);

			return BuildUrl(current, parameters, ConvertRouteParams(parameters.RouteParameters));
		}

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
		public virtual string BuildUrl(UrlInfo current, UrlBuilderParameters parameters, IDictionary routeParameters)
		{
			AssertArguments(current, parameters);

			bool encodeForLink = parameters.EncodeForLink;

			UrlParts url = CreateUrlPartsBuilder(current, parameters, routeParameters);

			if (encodeForLink)
			{
				return url.BuildPathForLink(serverUtil);
			}

			return url.BuildPath();
		}

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
		public virtual UrlParts CreateUrlPartsBuilder(UrlInfo current, IDictionary parameters)
		{
			AssertArguments(current, parameters);

			UrlBuilderParameters typedParams = UrlBuilderParameters.From(parameters);

			return CreateUrlPartsBuilder(current, parameters, ConvertRouteParams(typedParams.RouteParameters));
		}

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
		public virtual UrlParts CreateUrlPartsBuilder(UrlInfo current, IDictionary parameters, IDictionary routeParameters)
		{
			AssertArguments(current, parameters);

			UrlBuilderParameters typedParams = UrlBuilderParameters.From(parameters, routeParameters);

			return CreateUrlPartsBuilder(current, parameters, ConvertRouteParams(typedParams.RouteParameters));
		}

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
		public virtual UrlParts CreateUrlPartsBuilder(UrlInfo current, UrlBuilderParameters parameters)
		{
			AssertArguments(current, parameters);

			return CreateUrlPartsBuilder(current, parameters, ConvertRouteParams(parameters.RouteParameters));
		}

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
		public virtual UrlParts CreateUrlPartsBuilder(UrlInfo current, UrlBuilderParameters parameters, IDictionary routeParameters)
		{
			AssertArguments(current, parameters);

			string appVirtualDir = current.AppVirtualDir;

			string area = parameters.Area ?? current.Area;
			string controller = parameters.Controller ?? current.Controller;
			string action = parameters.Action ?? current.Action;
			
			if (appVirtualDir.Length > 1 && !(appVirtualDir[0] == '/'))
			{
				appVirtualDir = "/" + appVirtualDir;
			}

			string path = ComputeStandardBasePath(appVirtualDir, area);
			path = ApplyBasePathOrAbsolutePathIfNecessary(appVirtualDir, area, current, parameters, path);

			UrlParts parts = TryCreateUrlUsingRegisteredRoutes(current.Domain, parameters, appVirtualDir, routeParameters);

			if (parts == null)
			{
				parts = new UrlParts(path, controller, action + (useExtensions ? SafeExt(current.Extension) : ""));
				AppendPathInfo(parts, parameters);
			}
			else if (parameters.CreateAbsolutePath)
			{
				parts.InsertFrontPath(path);
			}
			
			AppendQueryString(parts, parameters);

			return parts;
		}

		#endregion

		/// <summary>
		/// Tries the create URL using registered routes.
		/// </summary>
		/// <param name="domain">The domain.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="appVirtualDir">The app virtual dir.</param>
		/// <param name="routeParameters">The route parameters.</param>
		/// <returns></returns>
		protected UrlParts TryCreateUrlUsingRegisteredRoutes(string domain, UrlBuilderParameters parameters,
		                                                     string appVirtualDir,
		                                                     IDictionary routeParameters)
		{
			if (routingEng != null && !routingEng.IsEmpty)
			{
				// We take was was explicitly set (we do not inherit those from the context)
				routeParameters["area"] = parameters.Area;
				routeParameters["controller"] = parameters.Controller;
				routeParameters["action"] = parameters.Action;

				if (parameters.UseCurrentRouteParams)
				{
					if (parameters.RouteMatch == null)
					{
						throw new InvalidOperationException("Error creating URL. 'UseCurrentRouteParams' was set, but routematch is null.");
					}

					CommonUtils.MergeOptions(routeParameters, parameters.RouteMatch.Parameters);
				}

				string url;

				if (parameters.RouteName != null)
				{
					url = routingEng.CreateUrl(parameters.RouteName, domain, appVirtualDir, routeParameters);
				}
				else
				{
					url = routingEng.CreateUrl(domain, appVirtualDir, routeParameters);
				}

				if (url != null)
				{
					return new UrlParts(url);
				}
			}

			return null;
		}

		/// <summary>
		/// Appends the path info.
		/// </summary>
		/// <param name="parts">The parts.</param>
		/// <param name="parameters">The parameters.</param>
		protected virtual void AppendPathInfo(UrlParts parts, UrlBuilderParameters parameters)
		{
			if (!string.IsNullOrEmpty(parameters.PathInfo))
			{
				parts.PathInfoDict.Parse(parameters.PathInfo);
			}
		}

		/// <summary>
		/// Converts the route params.
		/// </summary>
		/// <param name="routeParams">The route params.</param>
		/// <returns></returns>
		protected virtual IDictionary ConvertRouteParams(object routeParams)
		{
			IDictionary parameters;

			if (routeParams != null)
			{
				if (typeof(IDictionary).IsAssignableFrom(routeParams.GetType()))
				{
					parameters = (IDictionary) routeParams;
				}
				else
				{
					parameters = new ReflectionBasedDictionaryAdapter(routeParams);
				}

				// Forces copying entries to a non readonly dictionary, preserving the original one
				parameters = new Hashtable(parameters, StringComparer.InvariantCultureIgnoreCase);
			}
			else
			{
				parameters = new HybridDictionary(true);
			}

			return parameters;
		}

		/// <summary>
		/// Computes the standard base path.
		/// </summary>
		/// <param name="appVirtualDir">The app virtual dir.</param>
		/// <param name="area">The area.</param>
		/// <returns></returns>
		protected virtual string ComputeStandardBasePath(string appVirtualDir, string area)
		{
			string path;

			if (area != String.Empty)
			{
				path = appVirtualDir + "/" + area + "/";
			}
			else
			{
				path = appVirtualDir + "/";
			}

			return path;
		}

		/// <summary>
		/// Applies the base path or absolute path if necessary.
		/// </summary>
		/// <param name="appVirtualDir">The app virtual dir.</param>
		/// <param name="area">The area.</param>
		/// <param name="current">The current.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="path">The path.</param>
		/// <returns></returns>
		protected virtual string ApplyBasePathOrAbsolutePathIfNecessary(string appVirtualDir, string area, UrlInfo current,
															  UrlBuilderParameters parameters, string path)
		{
			bool createAbsolutePath = parameters.CreateAbsolutePath;
			string basePath = parameters.BasePath;

			if (!string.IsNullOrEmpty(basePath))
			{
				basePath = basePath[basePath.Length - 1] == '/' ? basePath.Substring(0, basePath.Length - 1) : basePath;

				if (basePath.EndsWith(area, StringComparison.InvariantCultureIgnoreCase))
				{
					path = basePath;
				}
				else
				{
					path = basePath + "/" + area;
				}
			}
			else if (createAbsolutePath)
			{
				string domain = parameters.Domain ?? current.Domain;
				string subdomain = parameters.Subdomain ?? current.Subdomain;
				string protocol = parameters.Protocol ?? current.Protocol;
				int port = parameters.Port == 0 ? current.Port : 0;

				bool includePort =
					(protocol == "http" && port != 80) ||
					(protocol == "https" && port != 443);

				path = protocol + "://";

				if (!string.IsNullOrEmpty(subdomain))
				{
					path += subdomain + "." + domain;
				}
				else
				{
					path += domain;
				}

				if (includePort)
				{
					path += ":" + port;
				}

				path += ComputeStandardBasePath(appVirtualDir, area);
			}

			return path;
		}

		/// <summary>
		/// Appends the query string.
		/// </summary>
		/// <param name="parts">The parts.</param>
		/// <param name="parameters">The parameters.</param>
		protected virtual void AppendQueryString(UrlParts parts, UrlBuilderParameters parameters)
		{
			object queryString = parameters.QueryString;

			string suffix = string.Empty;

			if (queryString != null)
			{
				if (queryString is IDictionary)
				{
					IDictionary qsDictionary = (IDictionary) queryString;

					suffix = CommonUtils.BuildQueryString(serverUtil, qsDictionary, false);
				}
				else if (queryString is NameValueCollection)
				{
					suffix = CommonUtils.BuildQueryString(serverUtil, (NameValueCollection) queryString, false);
				}
				else if (queryString is string && ((string)queryString).Length > 0)
				{
					string[] pairs = queryString.ToString().Split('&');

					suffix = string.Empty;

					foreach(string pair in pairs)
					{
						string[] keyvalues = pair.Split(new char[] { '=' }, 2);
						
						if (keyvalues.Length < 2) continue;

						if (suffix.Length != 0)
						{
							suffix += "&";
						}

						suffix += serverUtil.UrlEncode(keyvalues[0]) + "=" + serverUtil.UrlEncode(keyvalues[1]);
					}
				}
			}

			if (suffix != string.Empty)
			{
				parts.SetQueryString(suffix);
			}
		}

		private string SafeExt(string extension)
		{
			if (extension.StartsWith("."))
			{
				return extension;
			}

			return "." + extension;
		}

		private static void AssertArguments<T>(UrlInfo current, T parameters) where T : class
		{
			if (current == null)
			{
				throw new ArgumentNullException("current");
			}
			if (parameters == null)
			{
				throw new ArgumentNullException("parameters");
			}
		}
	}
}