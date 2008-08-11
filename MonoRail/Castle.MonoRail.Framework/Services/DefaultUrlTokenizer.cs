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
	using System.IO;
	using System.Text;
	using Configuration;
	using Core;

	/// <summary>
	/// Breaks the url into smaller pieces to find out
	/// the requested controller, action and optionally the area.
	/// <para>
	/// It alsos checks for default urls which map a single resource to an area/controller/action
	/// </para>
	/// </summary>
	public class DefaultUrlTokenizer : IUrlTokenizer, IServiceEnabledComponent
	{
		private readonly IDictionary defaultUrl2CustomUrlInfo = new HybridDictionary(true);

		/// <summary>
		/// Adds the default rule mapping.
		/// </summary>
		/// <remarks>
		/// A defautl rule can associate something like a 'default.castle' 
		/// to a controller/action like 'Home/index.castle'
		/// </remarks>
		/// <param name="url">The URL.</param>
		/// <param name="area">The area.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="action">The action.</param>
		public void AddDefaultRule(string url, string area, string controller, string action)
		{
			if (area == null)
			{
				area = string.Empty;
			}

			defaultUrl2CustomUrlInfo[url] = new UrlInfo(area, controller, action);
		}

		#region IServiceEnabledComponent

		/// <summary>
		/// Services the specified provider.
		/// </summary>
		/// <param name="provider">The provider.</param>
		public void Service(IServiceProvider provider)
		{
			IMonoRailConfiguration config = (IMonoRailConfiguration) provider.GetService(typeof(IMonoRailConfiguration));

			foreach(DefaultUrl url in config.DefaultUrls)
			{
				AddDefaultRule(url.Url, url.Area, url.Controller, url.Action);
			}
		}

		#endregion

		#region IUrlTokenizer

		/// <summary>
		/// Tokenizes the URL.
		/// </summary>
		/// <param name="rawUrl">The raw URL.</param>
		/// <param name="pathInfo">The path info.</param>
		/// <param name="uri">The URI.</param>
		/// <param name="isLocal">if set to <c>true</c> request is local.</param>
		/// <param name="appVirtualDir">Virtual directory</param>
		/// <returns></returns>
		public UrlInfo TokenizeUrl(string rawUrl, string pathInfo, Uri uri, bool isLocal, string appVirtualDir)
		{
			if (string.IsNullOrEmpty(rawUrl))
			{
				throw new ArgumentNullException("rawUrl", "rawUrl cannot be null or empty");
			}

			string domain = uri.Host;
			string subdomain = GetDomainToken(domain, 0);

			if (subdomain != null && (subdomain.Length+1) < domain.Length)
			{
				// Strip the subdomain from the main domain name
				domain = domain.Substring(subdomain.Length + 1);
			}

			if (rawUrl[0] == '/')
			{
				rawUrl = rawUrl.Substring(1);
			}

			// Strip the appVirtualDir from the Url
			if (appVirtualDir != null && appVirtualDir != "")
			{
				appVirtualDir = appVirtualDir.ToLower(System.Globalization.CultureInfo.InvariantCulture).Substring(1);

				if (!rawUrl.StartsWith(appVirtualDir, true, System.Globalization.CultureInfo.InvariantCulture))
				{
					// Sanity check
					throw new UrlTokenizerException("Url does not start with the virtual dir");
				}

				rawUrl = rawUrl.Substring(appVirtualDir.Length);
			}

			string area, controller, action;

			// Is the url a custom url?
			UrlInfo custom = (UrlInfo) defaultUrl2CustomUrlInfo[rawUrl];

			if (custom != null)
			{
				area = custom.Area;
				controller = custom.Controller;
				action = custom.Action;
			}
			else
			{
				ExtractAreaControllerAction(rawUrl, out area, out controller, out action);
			}

			string extension = GetExtension(rawUrl);

			return new UrlInfo(domain, subdomain, appVirtualDir, uri.Scheme, uri.Port, rawUrl, area, controller, action, extension, pathInfo);
		}

		#endregion

		/// <summary>
		/// Extracts the area controller action.
		/// </summary>
		/// <param name="rawUrl">The raw URL.</param>
		/// <param name="area">The area.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="action">The action.</param>
		private void ExtractAreaControllerAction(string rawUrl, out string area, out string controller, out string action)
		{
			string[] parts = rawUrl.Split('/');

			if (parts.Length < 2)
			{
				throw new UrlTokenizerException("Url smaller than 2 tokens");
			}

			action = parts[parts.Length - 1];

			int fileNameIndex = action.LastIndexOf('.');

			if (fileNameIndex != -1)
			{
				action = action.Substring(0, fileNameIndex);
			}

			controller = parts[parts.Length - 2];

			area = string.Empty;

			if (parts.Length - 3 == 0)
			{
				area = parts[parts.Length - 3];
			}
			else if (parts.Length - 3 > 0)
			{
				StringBuilder areaSB = new StringBuilder();

				for(int i = 0; i <= parts.Length - 3; i++)
				{
					if (parts[i] != null && parts[i].Length > 0)
					{
						areaSB.Append(parts[i]).Append('/');
					}
				}

				if (areaSB.Length > 0)
				{
					areaSB.Length -= 1;
				}

				area = areaSB.ToString();
			}
		}

		/// <summary>
		/// Gets the domain token.
		/// </summary>
		/// <param name="domain">The domain.</param>
		/// <param name="token">The token index.</param>
		/// <returns></returns>
		protected static string GetDomainToken(string domain, int token)
		{
			string[] parts = domain.Split('.');

			if (parts.Length == 1)
			{
				return null;
			}

			if (token < parts.Length)
			{
				return parts[token];
			}

			return string.Empty;
		}

		/// <summary>
		/// Gets the extension of the requested urls page without the preceding period.
		/// </summary>
		/// <param name="url">URL.</param>
		/// <returns>The resource file extension on the url (without the period).</returns>
		protected static string GetExtension(string url)
		{
			string ext = Path.GetExtension(url);

			if (ext.Length > 1)
			{
				return ext.Substring(1);
			}

			return ext;
		}
	}
}
