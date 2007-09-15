// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	using Framework;

	/// <summary>
	/// Default implementation of <see cref="IUrlBuilder"/>	
	/// </summary>
	/// <remarks>
	/// The property <see cref="UseExtensions"/> defines whether the builder should output
	/// file extension. This might be handy to use in combination with a url rewrite strategy
	/// 
	/// <para>
	/// If you want to create a custom urlbuilder, you can extend this one and override 
	/// the <see cref="InternalBuildUrl(string,string,string,string,string,string,string,string,string,bool,bool,string)"/>
	/// </para>
	/// 
	/// </remarks>
	/// <seealso cref="BuildUrl(UrlInfo,IDictionary)"/>
	public class DefaultUrlBuilder : IUrlBuilder, IServiceEnabledComponent
	{
		private bool useExtensions = true;
		private IServerUtility serverUtil;

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultUrlBuilder"/> class.
		/// </summary>
		public DefaultUrlBuilder()
		{
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

		#endregion

		#region IServiceEnabledComponent

		/// <summary>
		/// Services the specified provider.
		/// </summary>
		/// <param name="provider">The provider.</param>
		public void Service(IServiceProvider provider)
		{
			serverUtil = (IServerUtility) provider.GetService(typeof(IServerUtility));
		}

		#endregion

		#region IUrlBuilder

		/// <summary>
		/// Builds the URL using the current url as contextual information and a parameter dictionary.
		/// 
		/// </summary>
		/// 
		/// <remarks>
		/// <para>
		/// Common parameters includes <c>area</c>, <c>controller</c> and <c>action</c>, which outputs
		/// <c>/area/controller/name.extension</c>
		/// </para>
		/// 
		/// <para>
		/// Please note that if you dont specify an area or controller name, they will be inferred from the 
		/// context. If you want to use an empty area, you must specify <c>area=''</c>. 
		/// This is commonly a source of confusion, so understand the following cases:
		/// </para>
		/// 
		/// <example>
		/// <code>
		/// UrlInfo current = ... // Assume that the current is area Admin, controller Products and action List
		/// 
		/// BuildUrl(current, {action: 'view'})
		/// // returns /Admin/Products/view.castle
		/// 
		/// BuildUrl(current, {controller: 'Home', action: 'index'})
		/// // returns /Admin/Home/index.castle
		///  
		/// BuildUrl(current, {area:'', controller: 'Home', action: 'index'})
		/// // returns /Home/index.castle
		/// </code>
		/// </example>
		/// 
		/// <para>
		/// The <c>querystring</c> parameter can be a string or a dictionary. It appends a query string to the url:
		/// <c>/area/controller/name.extension?id=1</c>
		/// </para>
		/// 
		/// <para>
		/// The <c>absolute</c> parameter forces the builder to output a full url like
		/// <c>http://hostname/virtualdir/area/controller/name.extension</c>
		/// </para>
		/// 
		/// <para>
		/// The <c>encode</c> parameter forces the builder to encode the querystring
		/// <c>/controller/name.extension?id=1&amp;name=John</c> which is required to output full xhtml compliant content.
		/// </para>
		/// 
		/// </remarks>
		/// <param name="current">The current Url information.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		public virtual string BuildUrl(UrlInfo current, IDictionary parameters)
		{
			bool applySubdomain = false;
			bool createAbsolutePath = CommonUtils.ObtainEntryAndRemove(parameters, "absolute", "false") == "true";
			bool encode = CommonUtils.ObtainEntryAndRemove(parameters, "encode", "false") == "true";

			string area;

			if (parameters.Contains("area"))
			{
				area = CommonUtils.ObtainEntryAndRemove(parameters, "area");
				
				if (area == null) area = string.Empty;
			}
			else
			{
				area = current.Area;
			}

			string controller = CommonUtils.ObtainEntryAndRemove(parameters, "controller", current.Controller);
			string action = CommonUtils.ObtainEntryAndRemove(parameters, "action", current.Action);
			
			string domain = CommonUtils.ObtainEntryAndRemove(parameters, "domain", current.Domain);
			string subdomain = CommonUtils.ObtainEntryAndRemove(parameters, "subdomain", current.Subdomain);
			string protocol = CommonUtils.ObtainEntryAndRemove(parameters, "protocol", current.Protocol);
			string port = CommonUtils.ObtainEntryAndRemove(parameters, "port", current.Port.ToString());
			string suffix = null;

			object queryString = CommonUtils.ObtainObjectEntryAndRemove(parameters, "querystring");

			string basePath = null;

			if (parameters.Contains("basepath"))
			{
				basePath = CommonUtils.ObtainEntryAndRemove(parameters, "basepath");
			}

			if (queryString != null)
			{
				if (queryString is IDictionary)
				{
					IDictionary qsDictionary = (IDictionary) queryString;
					
					suffix = CommonUtils.BuildQueryString(serverUtil, qsDictionary, encode);
				}
				else if (queryString is NameValueCollection)
				{
					suffix = CommonUtils.BuildQueryString(serverUtil, (NameValueCollection) queryString, encode);	
				}
				else if (queryString is string)
				{
					suffix = queryString.ToString();
				}
			}

			if (subdomain.ToLower() != current.Subdomain.ToLower())
			{
				applySubdomain = true;
			}

			return InternalBuildUrl(area, controller, action, protocol, port, domain, subdomain,
				current.AppVirtualDir, current.Extension, createAbsolutePath, applySubdomain, suffix, basePath);
		}

		/// <summary>
		/// Builds an URL using the controller name and action name.
		/// </summary>
		/// <param name="current">The current Url information.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="action">The action.</param>
		/// <returns></returns>
		public virtual string BuildUrl(UrlInfo current, string controller, string action)
		{
			Hashtable parameters = new Hashtable();
			parameters["controller"] = controller;
			parameters["action"] = action;
			return BuildUrl(current, parameters);
		}

		/// <summary>
		/// Builds an URL using the controller name, action name, and a querystring dictionary.
		/// </summary>
		/// <param name="current">The current Url information.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="action">The action.</param>
		/// <param name="queryStringParams">The query string params.</param>
		/// <returns></returns>
		public virtual string BuildUrl(UrlInfo current, string controller, string action, IDictionary queryStringParams)
		{
			Hashtable parameters = new Hashtable();
			parameters["controller"] = controller;
			parameters["action"] = action;
			parameters["querystring"] = queryStringParams;
			return BuildUrl(current, parameters);
		}

		/// <summary>
		/// Builds an URL using the controller name, action name, and a querystring name value collection.
		/// </summary>
		/// <param name="current">The current Url information.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="action">The action.</param>
		/// <param name="queryStringParams">The query string params.</param>
		/// <returns></returns>
		public virtual string BuildUrl(UrlInfo current, string controller, string action, NameValueCollection queryStringParams)
		{
			Hashtable parameters = new Hashtable();
			parameters["controller"] = controller;
			parameters["action"] = action;
			parameters["querystring"] = queryStringParams;
			return BuildUrl(current, parameters);
		}

		/// <summary>
		/// Builds an URL using the area name, controller name and action name.
		/// </summary>
		/// <param name="current">The current Url information.</param>
		/// <param name="area">The area.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="action">The action.</param>
		/// <returns></returns>
		public virtual string BuildUrl(UrlInfo current, string area, string controller, string action)
		{
			Hashtable parameters = new Hashtable();
			parameters["area"] = area;
			parameters["controller"] = controller;
			parameters["action"] = action;
			return BuildUrl(current, parameters);
		}

		/// <summary>
		/// Builds an URL using the area name, controller name, action name, and a querystring dictionary.
		/// </summary>
		/// <param name="current">The current Url information.</param>
		/// <param name="area">The area.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="action">The action.</param>
		/// <param name="queryStringParams">The query string params.</param>
		/// <returns></returns>
		public virtual string BuildUrl(UrlInfo current, string area, string controller, string action, IDictionary queryStringParams)
		{
			Hashtable parameters = new Hashtable();
			parameters["area"] = area;
			parameters["controller"] = controller;
			parameters["action"] = action;
			parameters["querystring"] = queryStringParams;
			return BuildUrl(current, parameters);
		}

		/// <summary>
		/// Builds an URL using the area name, controller name, action name, and a querystring name value collection.
		/// </summary>
		/// <param name="current">The current Url information.</param>
		/// <param name="area">The area.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="action">The action.</param>
		/// <param name="queryStringParams">The query string params.</param>
		/// <returns></returns>
		public virtual string BuildUrl(UrlInfo current, string area, string controller, string action,
		                       NameValueCollection queryStringParams)
		{
			Hashtable parameters = new Hashtable();
			parameters["area"] = area;
			parameters["controller"] = controller;
			parameters["action"] = action;
			parameters["querystring"] = queryStringParams;
			return BuildUrl(current, parameters);
		}

		#endregion

		/// <summary>
		/// Internals the build URL.
		/// </summary>
		/// <param name="area">The area.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="action">The action.</param>
		/// <param name="protocol">The protocol.</param>
		/// <param name="port">The port.</param>
		/// <param name="domain">The domain.</param>
		/// <param name="subdomain">The subdomain.</param>
		/// <param name="appVirtualDir">The app virtual dir.</param>
		/// <param name="extension">The extension.</param>
		/// <param name="absolutePath">if set to <c>true</c> [absolute path].</param>
		/// <param name="applySubdomain">if set to <c>true</c> [apply subdomain].</param>
		/// <param name="suffix">The suffix.</param>
		/// <returns></returns>
		protected virtual string InternalBuildUrl(string area, string controller, string action, string protocol,
		                                string port, string domain, string subdomain, string appVirtualDir, string extension, 
		                                bool absolutePath, bool applySubdomain, string suffix)
		{
			return
				InternalBuildUrl(area, controller, action, protocol, port, domain, subdomain, appVirtualDir, extension, absolutePath,
				                 applySubdomain, suffix, null);
		}

		/// <summary>
		/// Internals the build URL.
		/// </summary>
		/// <param name="area">The area.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="action">The action.</param>
		/// <param name="protocol">The protocol.</param>
		/// <param name="port">The port.</param>
		/// <param name="domain">The domain.</param>
		/// <param name="subdomain">The subdomain.</param>
		/// <param name="appVirtualDir">The app virtual dir.</param>
		/// <param name="extension">The extension.</param>
		/// <param name="absolutePath">if set to <c>true</c> [absolute path].</param>
		/// <param name="applySubdomain">if set to <c>true</c> [apply subdomain].</param>
		/// <param name="suffix">The suffix.</param>
		/// <param name="basePath">The base path.</param>
		/// <returns></returns>
		protected virtual string InternalBuildUrl(string area, string controller, string action, string protocol,
		                                string port, string domain, string subdomain, string appVirtualDir, string extension,
										bool absolutePath, bool applySubdomain, string suffix, string basePath)
		{
			if (area == null) throw new ArgumentNullException("area");
			if (controller == null) throw new ArgumentNullException("controller");
			if (action == null) throw new ArgumentNullException("action");
			if (appVirtualDir == null) throw new ArgumentNullException("appVirtualDir");

			string path;

			if (basePath != null)
			{
				path = InternalBuildUrlUsingBasePath(action, area, basePath, controller);
			}
			else
			{
				path = InternalBuildUsingAppVirtualDir(absolutePath, action, applySubdomain, appVirtualDir, area, controller, domain, port, protocol, subdomain);
			}

			if (useExtensions)
			{
				path += "." + extension;
			}

			if (suffix != null && suffix != String.Empty)
			{
				path += "?" + suffix;
			}

			return path;
		}

		/// <summary>
		/// Internals the build using app virtual dir.
		/// </summary>
		/// <param name="absolutePath">if set to <c>true</c> [absolute path].</param>
		/// <param name="action">The action.</param>
		/// <param name="applySubdomain">if set to <c>true</c> [apply subdomain].</param>
		/// <param name="appVirtualDir">The app virtual dir.</param>
		/// <param name="area">The area.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="domain">The domain.</param>
		/// <param name="port">The port.</param>
		/// <param name="protocol">The protocol.</param>
		/// <param name="subdomain">The subdomain.</param>
		/// <returns></returns>
		private static string InternalBuildUsingAppVirtualDir(bool absolutePath, string action, bool applySubdomain, string appVirtualDir, string area, string controller, string domain, string port, string protocol, string subdomain)
		{
			string path;

			if (appVirtualDir.Length > 1 && !(appVirtualDir[0] == '/'))
			{
				appVirtualDir = "/" + appVirtualDir;
			}

			if (area != String.Empty)
			{
				path = appVirtualDir + "/" + area + "/" + controller + "/" + action;
			}
			else
			{
				path = appVirtualDir + "/" + controller + "/" + action;
			}

			if (absolutePath)
			{
				string url;

				if (applySubdomain)
				{
					url = protocol + "://" + subdomain + domain;
				}
				else
				{
					url = protocol + "://" + domain;
				}

				if (port != "80")
				{
					url += ":" + port;
				}

				path = url + path;
			}

			return path;
		}

		/// <summary>
		/// Internals the build URL using base path.
		/// </summary>
		/// <param name="action">The action.</param>
		/// <param name="area">The area.</param>
		/// <param name="basePath">The base path.</param>
		/// <param name="controller">The controller.</param>
		/// <returns></returns>
		private static string InternalBuildUrlUsingBasePath(string action, string area, string basePath, string controller)
		{
			string path;

			basePath = basePath[basePath.Length - 1] == '/' ? basePath.Substring(0, basePath.Length - 1) : basePath;

			if (basePath.EndsWith(area))
			{
				path = basePath + "/" + controller + "/" + action;
			}
			else
			{
				path = basePath + "/" + area + "/" + controller + "/" + action;
			}

			return path;
		}
	}
}
