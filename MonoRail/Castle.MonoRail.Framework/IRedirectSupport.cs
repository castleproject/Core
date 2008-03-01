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
	using System.Collections;
	using System.Collections.Specialized;

	/// <summary>
	/// 
	/// </summary>
	public interface IRedirectSupport
	{
		/// <summary>
		/// Redirects to url using referrer.
		/// </summary>
		void RedirectToReferrer();

		/// <summary> 
		/// Redirects to the site root directory (<c>Context.ApplicationPath + "/"</c>).
		/// </summary>
		void RedirectToSiteRoot();

		/// <summary>
		/// Redirects to the specified url
		/// </summary>
		/// <param name="url">An relative or absolute URL to redirect the client to</param>
		void RedirectToUrl(string url);
		/// <summary>
		/// Redirects to the specified url
		/// </summary>
		/// <param name="url">An relative or absolute URL to redirect the client to</param>
		/// <param name="endProcess">if set to <c>true</c>, sends the redirect and 
		/// kills the current request process.</param>
		void RedirectToUrl(string url, bool endProcess);
		/// <summary>
		/// Redirects to the specified url
		/// </summary>
		/// <param name="url">An relative or absolute URL to redirect the client to</param>
		/// <param name="queryStringParameters">The querystring entries</param>
		void RedirectToUrl(string url, IDictionary queryStringParameters);
		/// <summary>
		/// Redirects to the specified url
		/// </summary>
		/// <param name="url">An relative or absolute URL to redirect the client to</param>
		/// <param name="queryStringParameters">The querystring entries</param>
		void RedirectToUrl(string url, NameValueCollection queryStringParameters);
		/// <summary>
		/// Redirects to the specified url
		/// </summary>
		/// <param name="url">An relative or absolute URL to redirect the client to</param>
		/// <param name="queryStringParameters">The querystring entries</param>
		void RedirectToUrl(string url, params string[] queryStringParameters);
		/// <summary>
		/// Redirects to the specified url
		/// </summary>
		/// <param name="url">An relative or absolute URL to redirect the client to</param>
		/// <param name="queryStringAnonymousDictionary">The querystring entries as an anonymous dictionary</param>
		void RedirectToUrl(string url, object queryStringAnonymousDictionary);


		/// <summary>
		/// Redirects to another controller's action.
		/// </summary>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		void Redirect(string controller, string action);
		/// <summary>
		/// Redirects to another controller's action with the specified parameters.
		/// </summary>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="queryStringParameters">The querystring entries</param>
		void Redirect(string controller, string action, NameValueCollection queryStringParameters);
		/// <summary>
		/// Redirects to another controller's action with the specified parameters.
		/// </summary>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="queryStringParameters">The querystring entries</param>
		void Redirect(string controller, string action, IDictionary queryStringParameters);
		/// <summary>
		/// Redirects to another controller's action with the specified parameters.
		/// </summary>
		/// <param name="queryStringAnonymousDictionary">The querystring entries as an anonymous dictionary</param>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		void Redirect(string controller, string action, object queryStringAnonymousDictionary);
		/// <summary>
		/// Redirects to another controller's action in a different area.
		/// </summary>
		/// <param name="area">The area the target controller belongs to.</param>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		void Redirect(string area, string controller, string action);
		/// <summary>
		/// Redirects to another controller's action in a different area with the specified parameters.
		/// </summary>
		/// <param name="area">The area the target controller belongs to.</param>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="queryStringParameters">The querystring entries</param>
		void Redirect(string area, string controller, string action, IDictionary queryStringParameters);
		/// <summary>
		/// Redirects to another controller's action in a different area with the specified parameters.
		/// </summary>
		/// <param name="area">The area the target controller belongs to.</param>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="queryStringParameters">The querystring entries</param>
		void Redirect(string area, string controller, string action, NameValueCollection queryStringParameters);
		/// <summary>
		/// Redirects to another controller's action in a different area with the specified parameters.
		/// </summary>
		/// <param name="area">The area the target controller belongs to.</param>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="queryStringAnonymousDictionary">The querystring entries as an anonymous dictionary</param>
		void Redirect(string area, string controller, string action, object queryStringAnonymousDictionary);

		
		/// <summary>
		/// Redirects using a named route. 
		/// The name must exists otherwise a <see cref="MonoRailException"/> will be thrown. 
		/// </summary>
		/// <param name="routeName">Route name.</param>
		void RedirectUsingNamedRoute(string routeName);
		
		/// <summary>
		/// Redirects using a named route.
		/// The name must exists otherwise a <see cref="MonoRailException"/> will be thrown.
		/// </summary>
		/// <param name="routeName">Route name.</param>
		/// <param name="routeParameters">The route parameters.</param>
		void RedirectUsingNamedRoute(string routeName, object routeParameters);

		/// <summary>
		/// Redirects using a named route.
		/// The name must exists otherwise a <see cref="MonoRailException"/> will be thrown.
		/// </summary>
		/// <param name="routeName">Route name.</param>
		/// <param name="routeParameters">The route parameters.</param>
		void RedirectUsingNamedRoute(string routeName, IDictionary routeParameters);


		/// <summary>
		/// Tries to resolve the target redirect url by using the routing rules registered.
		/// </summary>
		/// <param name="useCurrentRouteParams">if set to <c>true</c> the current request matching route rules will be used.</param>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		void RedirectUsingRoute(string controller, string action, bool useCurrentRouteParams);
		/// <summary>
		/// Tries to resolve the target redirect url by using the routing rules registered.
		/// </summary>
		/// <param name="useCurrentRouteParams">if set to <c>true</c> the current request matching route rules will be used.</param>
		/// <param name="area">The area the target controller belongs to.</param>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		void RedirectUsingRoute(string area, string controller, string action, bool useCurrentRouteParams);
		/// <summary>
		/// Tries to resolve the target redirect url by using the routing rules registered.
		/// </summary>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="routeParameters">The routing rule parameters.</param>
		void RedirectUsingRoute(string controller, string action, IDictionary routeParameters);
		/// <summary>
		/// Tries to resolve the target redirect url by using the routing rules registered.
		/// </summary>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="routeParameters">The routing rule parameters.</param>
		void RedirectUsingRoute(string controller, string action, object routeParameters);
		/// <summary>
		/// Tries to resolve the target redirect url by using the routing rules registered.
		/// </summary>
		/// <param name="area">The area the target controller belongs to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="routeParameters">The routing rule parameters.</param>
		void RedirectUsingRoute(string area, string controller, string action, IDictionary routeParameters);
		/// <summary>
		/// Tries to resolve the target redirect url by using the routing rules registered.
		/// </summary>
		/// <param name="area">The area the target controller belongs to.</param>
		/// <param name="action">The desired action on the target controller.</param>
		/// <param name="controller">The controller name to be redirected to.</param>
		/// <param name="routeParameters">The routing rule parameters.</param>
		void RedirectUsingRoute(string area, string controller, string action, object routeParameters);
	}
}
