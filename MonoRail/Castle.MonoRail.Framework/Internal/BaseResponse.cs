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

namespace Castle.MonoRail.Framework.Internal
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.IO;
	using System.Web;
	using Core;
	using Helpers;
	using Routing;
	using Services;
	using Test;

	/// <summary>
	/// 
	/// </summary>
	public abstract class BaseResponse : IMockResponse
	{
		private readonly UrlInfo currentUrl;
		private readonly IServerUtility serverUtility;
		private readonly RouteMatch routeMatch;
		private readonly string referrer;
		private IUrlBuilder urlBuilder;
		/// <summary>
		/// Indicates if a redirected has been issued
		/// </summary>
		protected bool redirected;
		private UrlInfo urlInfo;

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseResponse"/> class.
		/// </summary>
		/// <param name="currentUrl">The current URL.</param>
		/// <param name="urlBuilder">The URL builder.</param>
		/// <param name="serverUtility">The server utility.</param>
		/// <param name="routeMatch">The route match.</param>
		/// <param name="referrer">The referrer.</param>
		protected BaseResponse(UrlInfo currentUrl, IUrlBuilder urlBuilder, IServerUtility serverUtility, RouteMatch routeMatch, string referrer)
		{
			this.currentUrl = currentUrl;
			this.urlBuilder = urlBuilder;
			this.serverUtility = serverUtility;
			this.routeMatch = routeMatch;
			this.referrer = referrer;
		}

		#region abstracts

		/// <summary>
		/// Gets or sets the status code.
		/// </summary>
		/// <value>The status code.</value>
		public abstract int StatusCode { get; set; }

		/// <summary>
		/// Gets or sets the status code.
		/// </summary>
		/// <value>The status code.</value>
		public abstract string StatusDescription { get; set; }

		/// <summary>
		/// Gets or sets the type of the content.
		/// </summary>
		/// <value>The type of the content.</value>
		public abstract string ContentType { get; set; }

		/// <summary>
		/// Gets the caching policy (expiration time, privacy,
		/// vary clauses) of a Web page.
		/// </summary>
		/// <value></value>
		public abstract HttpCachePolicy CachePolicy { get; set; }

		/// <summary>
		/// Sets the Cache-Control HTTP header to Public or Private.
		/// </summary>
		/// <value></value>
		public abstract string CacheControlHeader { get; set; }

		/// <summary>
		/// Gets or sets the HTTP character set of the output stream.
		/// </summary>
		/// <value></value>
		public abstract string Charset { get; set; }

		/// <summary>
		/// Gets the output.
		/// </summary>
		/// <value>The output.</value>
		public abstract TextWriter Output { get; set; }

		/// <summary>
		/// Gets the output stream.
		/// </summary>
		/// <value>The output stream.</value>
		public abstract Stream OutputStream { get; }

		/// <summary>
		/// Appends the header.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public abstract void AppendHeader(string name, string value);
		/// <summary>
		/// Writes the buffer to the browser
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		public abstract void BinaryWrite(byte[] buffer);
		/// <summary>
		/// Writes the stream to the browser
		/// </summary>
		/// <param name="stream">The stream.</param>
		public abstract void BinaryWrite(Stream stream);
		/// <summary>
		/// Clears the response (only works if buffered)
		/// </summary>
		public abstract void Clear();
		/// <summary>
		/// Clears the response content (only works if buffered).
		/// </summary>
		public abstract void ClearContent();
		/// <summary>
		/// Writes the specified string.
		/// </summary>
		/// <param name="s">The string.</param>
		public abstract void Write(string s);
		/// <summary>
		/// Writes the specified obj.
		/// </summary>
		/// <param name="obj">The obj.</param>
		public abstract void Write(object obj);
		/// <summary>
		/// Writes the specified char.
		/// </summary>
		/// <param name="ch">The char.</param>
		public abstract void Write(char ch);
		/// <summary>
		/// Writes the specified buffer.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <param name="index">The index.</param>
		/// <param name="count">The count.</param>
		public abstract void Write(char[] buffer, int index, int count);
		/// <summary>
		/// Writes the file.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		public abstract void WriteFile(string fileName);
		/// <summary>
		/// Gets a value indicating whether the response sent a redirect.
		/// </summary>
		/// <value><c>true</c> if was redirected; otherwise, <c>false</c>.</value>
		public abstract bool WasRedirected { get; }

		/// <summary>
		/// Gets a value indicating whether this instance is client connected.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is client connected; otherwise, <c>false</c>.
		/// </value>
		public abstract bool IsClientConnected { get; }

		/// <summary>
		/// Redirects to the specified url
		/// </summary>
		/// <param name="url">An relative or absolute URL to redirect the client to</param>
		/// <param name="endProcess">if set to <c>true</c>, sends the redirect and
		/// kills the current request process.</param>
		public abstract void RedirectToUrl(string url, bool endProcess);

		#endregion

		/// <summary>
		/// Redirects to url using referrer.
		/// </summary>
		public void RedirectToReferrer()
		{
			if (referrer == null)
			{
				throw new InvalidOperationException("No referrer available");
			}
			RedirectToUrl(referrer);
		}

		/// <summary>
		/// Redirects to the site root directory (<c>Context.ApplicationPath + "/"</c>).
		/// </summary>
		public void RedirectToSiteRoot()
		{
			RedirectToUrl(currentUrl.AppVirtualDir + "/");
		}

		/// <summary>
		/// Redirects to the specified url
		/// </summary>
		/// <param name="url">An relative or absolute URL to redirect the client to</param>
		public void RedirectToUrl(string url)
		{
			RedirectToUrl(url, false);
		}

		/// <summary>
		/// Redirects to the specified url
		/// </summary>
		/// <param name="url">An relative or absolute URL to redirect the client to</param>
		/// <param name="queryStringParameters">The querystring entries</param>
		public void RedirectToUrl(string url, IDictionary queryStringParameters)
		{
			if (queryStringParameters != null && queryStringParameters.Count != 0)
			{
				if (url.IndexOf('?') != -1)
				{
					url += '&';
				}
				else
				{
					url += '?';
				}
				url += CommonUtils.BuildQueryString(serverUtility, queryStringParameters, false);
			}
			RedirectToUrl(url, false);
		}

		/// <summary>
		/// Redirects to the specified url
		/// </summary>
		/// <param name="url">An relative or absolute URL to redirect the client to</param>
		/// <param name="queryStringParameters">The querystring entries</param>
		public void RedirectToUrl(string url, NameValueCollection queryStringParameters)
		{
			if (queryStringParameters != null && queryStringParameters.Count != 0)
			{
				if (url.IndexOf('?') != -1)
				{
					url += '&';
				}
				else
				{
					url += '?';
				}
				url += CommonUtils.BuildQueryString(serverUtility, queryStringParameters, false);
			}
			RedirectToUrl(url, false);
		}

		/// <summary>
		/// Redirects to the specified url
		/// </summary>
		/// <param name="url">An relative or absolute URL to redirect the client to</param>
		/// <param name="queryStringParameters">The querystring entries</param>
		public void RedirectToUrl(string url, params string[] queryStringParameters)
		{
			RedirectToUrl(url, DictHelper.Create(queryStringParameters));
		}

		/// <summary>
		/// Redirects to the specified url
		/// </summary>
		/// <param name="url">An relative or absolute URL to redirect the client to</param>
		/// <param name="queryStringAnonymousDictionary">The querystring entries as an anonymous dictionary</param>
		public void RedirectToUrl(string url, object queryStringAnonymousDictionary)
		{
			RedirectToUrl(url, new ReflectionBasedDictionaryAdapter(queryStringAnonymousDictionary));
		}

		/// <summary>
		/// Redirects to another controller's action.
		/// </summary>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		public void Redirect(string controller, string action)
		{
			RedirectToUrl(urlBuilder.BuildUrl(currentUrl, new UrlBuilderParameters(controller, action)), false);
		}

		/// <summary>
		/// Redirects to another controller's action with the specified parameters.
		/// </summary>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="queryStringParameters">The querystring entries</param>
		public void Redirect(string controller, string action, NameValueCollection queryStringParameters)
		{
			RedirectToUrl(urlBuilder.BuildUrl(currentUrl, new UrlBuilderParameters(controller, action).
				SetQueryString(queryStringParameters)), false);
		}

		/// <summary>
		/// Redirects to another controller's action with the specified parameters.
		/// </summary>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="queryStringParameters">The querystring entries</param>
		public void Redirect(string controller, string action, IDictionary queryStringParameters)
		{
			RedirectToUrl(urlBuilder.BuildUrl(currentUrl, new UrlBuilderParameters(controller, action).
				SetQueryString(queryStringParameters)), false);
		}

		/// <summary>
		/// Redirects to another controller's action with the specified parameters.
		/// </summary>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="queryStringAnonymousDictionary">The querystring entries as an anonymous dictionary</param>
		public void Redirect(string controller, string action, object queryStringAnonymousDictionary)
		{
			RedirectToUrl(urlBuilder.BuildUrl(currentUrl, new UrlBuilderParameters(controller, action).
				SetQueryString(new ReflectionBasedDictionaryAdapter(queryStringAnonymousDictionary))), false);
		}

		/// <summary>
		/// Redirects to another controller's action in a different area.
		/// </summary>
		/// <param name="area">The area the target controller belongs to.</param>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		public void Redirect(string area, string controller, string action)
		{
			RedirectToUrl(urlBuilder.BuildUrl(currentUrl, new UrlBuilderParameters(area, controller, action)), false);
		}

		/// <summary>
		/// Redirects to another controller's action in a different area with the specified parameters.
		/// </summary>
		/// <param name="area">The area the target controller belongs to.</param>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="queryStringParameters">The querystring entries</param>
		public void Redirect(string area, string controller, string action, IDictionary queryStringParameters)
		{
			RedirectToUrl(urlBuilder.BuildUrl(currentUrl, new UrlBuilderParameters(area, controller, action).
				SetQueryString(queryStringParameters)), false);
		}

		/// <summary>
		/// Redirects to another controller's action in a different area with the specified parameters.
		/// </summary>
		/// <param name="area">The area the target controller belongs to.</param>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="queryStringParameters">The querystring entries</param>
		public void Redirect(string area, string controller, string action, NameValueCollection queryStringParameters)
		{
			RedirectToUrl(urlBuilder.BuildUrl(currentUrl, new UrlBuilderParameters(area, controller, action).
				SetQueryString(queryStringParameters)), false);
		}

		/// <summary>
		/// Redirects to another controller's action in a different area with the specified parameters.
		/// </summary>
		/// <param name="area">The area the target controller belongs to.</param>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="queryStringAnonymousDictionary">The querystring entries as an anonymous dictionary</param>
		public void Redirect(string area, string controller, string action, object queryStringAnonymousDictionary)
		{
			RedirectToUrl(urlBuilder.BuildUrl(currentUrl, new UrlBuilderParameters(area, controller, action).
				SetQueryString(new ReflectionBasedDictionaryAdapter(queryStringAnonymousDictionary))), false);
		}

		/// <summary>
		/// Redirects using a named route.
		/// The name must exists otherwise a <see cref="MonoRailException"/> will be thrown.
		/// </summary>
		/// <param name="routeName">Route name.</param>
		public void RedirectUsingNamedRoute(string routeName)
		{
			UrlBuilderParameters @params = new UrlBuilderParameters();
			@params.RouteName = routeName;
			RedirectToUrl(urlBuilder.BuildUrl(currentUrl, @params), false);
		}

		/// <summary>
		/// Redirects using a named route.
		/// The name must exists otherwise a <see cref="MonoRailException"/> will be thrown.
		/// </summary>
		/// <param name="routeName">Route name.</param>
		/// <param name="routeParameters">The route parameters.</param>
		public void RedirectUsingNamedRoute(string routeName, object routeParameters)
		{
			UrlBuilderParameters @params = new UrlBuilderParameters();
			@params.RouteName = routeName;
			@params.RouteParameters = routeParameters;
			RedirectToUrl(urlBuilder.BuildUrl(currentUrl, @params), false);
		}

		/// <summary>
		/// Redirects using a named route.
		/// The name must exists otherwise a <see cref="MonoRailException"/> will be thrown.
		/// </summary>
		/// <param name="routeName">Route name.</param>
		/// <param name="routeParameters">The route parameters.</param>
		public void RedirectUsingNamedRoute(string routeName, IDictionary routeParameters)
		{
			UrlBuilderParameters @params = new UrlBuilderParameters();
			@params.RouteName = routeName;
			@params.RouteParameters = routeParameters;
			RedirectToUrl(urlBuilder.BuildUrl(currentUrl, @params), false);
		}

		/// <summary>
		/// Tries to resolve the target redirect url by using the routing rules registered.
		/// </summary>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="useCurrentRouteParams">if set to <c>true</c> the current request matching route rules will be used.</param>
		public void RedirectUsingRoute(string controller, string action, bool useCurrentRouteParams)
		{
			UrlBuilderParameters @params = new UrlBuilderParameters(controller, action).
				SetRouteMatch(useCurrentRouteParams, routeMatch);
			RedirectToUrl(urlBuilder.BuildUrl(currentUrl, @params), false);
		}

		/// <summary>
		/// Tries to resolve the target redirect url by using the routing rules registered.
		/// </summary>
		/// <param name="area">The area the target controller belongs to.</param>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="useCurrentRouteParams">if set to <c>true</c> the current request matching route rules will be used.</param>
		public void RedirectUsingRoute(string area, string controller, string action, bool useCurrentRouteParams)
		{
			UrlBuilderParameters @params = new UrlBuilderParameters(area, controller, action).
				SetRouteMatch(useCurrentRouteParams, routeMatch);
			RedirectToUrl(urlBuilder.BuildUrl(currentUrl, @params), false);
		}

		/// <summary>
		/// Tries to resolve the target redirect url by using the routing rules registered.
		/// </summary>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="routeParameters">The routing rule parameters.</param>
		public void RedirectUsingRoute(string controller, string action, IDictionary routeParameters)
		{
			UrlBuilderParameters @params = new UrlBuilderParameters(controller, action);
			@params.RouteParameters = routeParameters;
			RedirectToUrl(urlBuilder.BuildUrl(currentUrl, @params), false);
		}

		/// <summary>
		/// Tries to resolve the target redirect url by using the routing rules registered.
		/// </summary>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="routeParameters">The routing rule parameters.</param>
		public void RedirectUsingRoute(string controller, string action, object routeParameters)
		{
			UrlBuilderParameters @params = new UrlBuilderParameters(controller, action);
			@params.RouteParameters = routeParameters;
			RedirectToUrl(urlBuilder.BuildUrl(currentUrl, @params), false);
		}

		/// <summary>
		/// Tries to resolve the target redirect url by using the routing rules registered.
		/// </summary>
		/// <param name="area">The area the target controller belongs to.</param>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="routeParameters">The routing rule parameters.</param>
		public void RedirectUsingRoute(string area, string controller, string action, IDictionary routeParameters)
		{
			UrlBuilderParameters @params = new UrlBuilderParameters(area, controller, action);
			@params.RouteParameters = routeParameters;
			RedirectToUrl(urlBuilder.BuildUrl(currentUrl, @params), false);
		}

		/// <summary>
		/// Tries to resolve the target redirect url by using the routing rules registered.
		/// </summary>
		/// <param name="area">The area the target controller belongs to.</param>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="routeParameters">The routing rule parameters.</param>
		public void RedirectUsingRoute(string area, string controller, string action, object routeParameters)
		{
			UrlBuilderParameters @params = new UrlBuilderParameters(area, controller, action);
			@params.RouteParameters = routeParameters;
			RedirectToUrl(urlBuilder.BuildUrl(currentUrl, @params), false);
		}

		/// <summary>
		/// Creates the cookie.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="cookieValue">The cookie value.</param>
		public virtual void CreateCookie(string name, string cookieValue)
		{
			HttpCookie cookie = new HttpCookie(name, cookieValue);
			cookie.Path = SafeAppPath();
			CreateCookie(cookie);
		}

		/// <summary>
		/// Creates the cookie.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="cookieValue">The cookie value.</param>
		/// <param name="expiration">The expiration.</param>
		public virtual void CreateCookie(string name, string cookieValue, DateTime expiration)
		{
			HttpCookie cookie = new HttpCookie(name, cookieValue);
			cookie.Expires = expiration;
			cookie.Path = SafeAppPath();
			CreateCookie(cookie);
		}

		/// <summary>
		/// Creates the cookie.
		/// </summary>
		/// <param name="cookie">The cookie.</param>
		public abstract void CreateCookie(HttpCookie cookie);

		/// <summary>
		/// Removes the cookie.
		/// </summary>
		/// <param name="name">The name.</param>
		public virtual void RemoveCookie(string name)
		{
			HttpCookie cookie = new HttpCookie(name, "");

			cookie.Expires = DateTime.Now.AddYears(-10);
			cookie.Path = SafeAppPath();

			CreateCookie(cookie);
		}

		#region IMockResponse

		/// <summary>
		/// Determines whether a cookie is present on the response.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>
		/// 	<c>true</c> if the cookie exists in the response; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool IsCookiePresent(string name)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets a cookie added through one of 
		/// the <see cref="IResponse.CreateCookie(HttpCookie)"/> overloads.
		/// </summary>
		/// <param name="name">The cookie name.</param>
		/// <returns></returns>
		public virtual HttpCookie GetCookie(string name)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the urls the request was redirected to.
		/// </summary>
		/// <value>The redirected to.</value>
		public virtual string RedirectedTo
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets the output.
		/// </summary>
		/// <value>The output.</value>
		public virtual string OutputContent
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets or sets the URL info.
		/// </summary>
		/// <value>The URL info.</value>
		public UrlInfo UrlInfo
		{
			get { return urlInfo; }
			set { urlInfo = value; }
		}

		/// <summary>
		/// Gets or sets the URL builder.
		/// </summary>
		/// <value>The URL builder.</value>
		public IUrlBuilder UrlBuilder
		{
			get { return urlBuilder; }
			set { urlBuilder = value; }
		}

		#endregion

		private string SafeAppPath()
		{
			string appVirtualDir = String.IsNullOrEmpty(currentUrl.AppVirtualDir) ? "/" : currentUrl.AppVirtualDir;
			if (appVirtualDir[0] != '/')
			{
				return '/' + appVirtualDir;
			}
			return appVirtualDir;
		}
	}
}
