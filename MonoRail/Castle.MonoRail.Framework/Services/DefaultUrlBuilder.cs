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

	public class DefaultUrlBuilder : IUrlBuilder, IServiceEnabledComponent
	{
		private bool useExtensions = true;
		private IServerUtility serverUtil;

		public DefaultUrlBuilder()
		{
		}

		#region Properties

		public bool UseExtensions
		{
			get { return useExtensions; }
			set { useExtensions = value; }
		}

		public IServerUtility ServerUtil
		{
			get { return serverUtil; }
			set { serverUtil = value; }
		}

		#endregion

		#region IServiceEnabledComponent

		public void Service(IServiceProvider provider)
		{
			serverUtil = (IServerUtility) provider.GetService(typeof(IServerUtility));
		}

		#endregion

		#region IUrlBuilder

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

			if (queryString == null && parameters.Count != 0)
			{
				suffix = CommonUtils.BuildQueryString(serverUtil, parameters, encode);
			}
			else if (queryString != null)
			{
				if (queryString is IDictionary)
				{
					IDictionary qsDictionary = (IDictionary) queryString;
					
					// Copy all existing entries on parameters to querystring dictionary
					CommonUtils.MergeOptions(qsDictionary, parameters);

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

		public virtual string BuildUrl(UrlInfo current, string controller, string action)
		{
			Hashtable parameters = new Hashtable();
			parameters["controller"] = controller;
			parameters["action"] = action;
			return BuildUrl(current, parameters);
		}

		public virtual string BuildUrl(UrlInfo current, string controller, string action, IDictionary queryStringParams)
		{
			Hashtable parameters = new Hashtable();
			parameters["controller"] = controller;
			parameters["action"] = action;
			parameters["querystring"] = queryStringParams;
			return BuildUrl(current, parameters);
		}

		public virtual string BuildUrl(UrlInfo current, string controller, string action, NameValueCollection queryStringParams)
		{
			Hashtable parameters = new Hashtable();
			parameters["controller"] = controller;
			parameters["action"] = action;
			parameters["querystring"] = queryStringParams;
			return BuildUrl(current, parameters);
		}

		public virtual string BuildUrl(UrlInfo current, string area, string controller, string action)
		{
			Hashtable parameters = new Hashtable();
			parameters["area"] = area;
			parameters["controller"] = controller;
			parameters["action"] = action;
			return BuildUrl(current, parameters);
		}

		public virtual string BuildUrl(UrlInfo current, string area, string controller, string action, IDictionary queryStringParams)
		{
			Hashtable parameters = new Hashtable();
			parameters["area"] = area;
			parameters["controller"] = controller;
			parameters["action"] = action;
			parameters["querystring"] = queryStringParams;
			return BuildUrl(current, parameters);
		}

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

		protected virtual string InternalBuildUrl(string area, string controller, string action, string protocol,
		                                string port, string domain, string subdomain, string appVirtualDir, string extension, 
		                                bool absolutePath, bool applySubdomain, string suffix)
		{
			return
				InternalBuildUrl(area, controller, action, protocol, port, domain, subdomain, appVirtualDir, extension, absolutePath,
				                 applySubdomain, suffix, null);
		}

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
