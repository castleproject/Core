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

namespace Castle.MonoRail.Framework.Helpers
{
	using System.Collections;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Services;

	/// <summary>
	/// Helper that allows the creation of urls using a dictionary.
	/// 
	/// <para>
	/// For more information see <see cref="DefaultUrlBuilder.BuildUrl(UrlInfo,IDictionary)"/>
	/// </para>
	/// 
	/// </summary>
	/// 
	/// <remarks> 
	/// By default the urlhelper sets the encode to <c>true</c>, so the html generated is valid xhtml.
	/// </remarks> 
	/// 
	/// <seealso cref="DefaultUrlBuilder.BuildUrl(UrlInfo,IDictionary)"/>
	public class UrlHelper : AbstractHelper
	{
		private IUrlBuilder urlBuilder;
		private UrlInfo	currentUrl;

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="UrlHelper"/> class.
		/// </summary>
		public UrlHelper() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="UrlHelper"/> class.
		/// setting the Controller, Context and ControllerContext.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		public UrlHelper(IEngineContext engineContext) : base(engineContext) { }
		#endregion

		/// <summary>
		/// Gets or sets the URL builder.
		/// </summary>
		/// <value>The URL builder.</value>
		public IUrlBuilder UrlBuilder
		{
			get { return urlBuilder; }
			set { urlBuilder = value; }
		}

		/// <summary>
		/// Gets or sets the current URL.
		/// </summary>
		/// <value>The current URL.</value>
		public UrlInfo CurrentUrl
		{
			get { return currentUrl; }
			set { currentUrl = value; }
		}

		/// <summary>
		/// Sets the context.
		/// </summary>
		/// <param name="context">The context.</param>
		public override void SetContext(IEngineContext context)
		{
			base.SetContext(context);

			urlBuilder = (IUrlBuilder) context.GetService(typeof(IUrlBuilder));
			currentUrl = context.UrlInfo;
		}

		/// <summary>
		/// Outputs a path constructed using the specified parameters.
		/// </summary>
		/// 
		/// <seealso cref="DefaultUrlBuilder.BuildUrl(UrlInfo,IDictionary)"/>
		/// 
		/// <example>
		/// The following code uses nvelocity syntax:
		/// 
		/// <code>
		///  $url.for("%{action='Save'}")
		/// </code>
		/// 
		/// <para>outputs</para>
		/// 
		/// <code>/ControllerNameFromContext/Save.extension_configured</code>
		/// 
		/// <code>
		///  $url.for("%{action='Edit',querystring='id=1'}")
		/// </code>
		/// 
		/// <para>outputs</para>
		/// 
		/// <code>/ControllerNameFromContext/Edit.extension_configured?id=1</code>
		/// </example>
		/// 
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		public string For(IDictionary parameters)
		{
			SetEncodeDefault(parameters);
			return urlBuilder.BuildUrl(currentUrl, parameters);
		}

		/// <summary>
		/// Outputs an anchor element constructed using the specified parameters.
		/// </summary>
		/// 
		/// <seealso cref="DefaultUrlBuilder.BuildUrl(UrlInfo,IDictionary)"/>
		/// 
		/// <example>
		/// The following code uses nvelocity syntax:
		/// 
		/// <code>
		///  $url.link('my link', "%{action='Save'}")
		/// </code>
		/// 
		/// <para>outputs</para>
		/// 
		/// <code><![CDATA[ <a href="/ControllerNameFromContext/Save.extension_configured">my link</a> ]]> </code>
		/// 
		/// <code>
		///  $url.link('my link', "%{action='Edit',querystring='id=1'}")
		/// </code>
		/// 
		/// <para>outputs</para>
		/// 
		/// <code><![CDATA[ <a href="/ControllerNameFromContext/Edit.extension_configured?id=1">my link</a> ]]> </code>
		/// </example>
		/// 
		/// <param name="innerContent">The anchor text.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		public string Link(string innerContent, IDictionary parameters)
		{
			SetEncodeDefault(parameters);
			return "<a href=\"" + For(parameters) + "\">" + innerContent + "</a>";
		}

		/// <summary>
		/// Outputs an anchor element constructed using the specified parameters.
		/// </summary>
		/// 
		/// <seealso cref="DefaultUrlBuilder.BuildUrl(UrlInfo,IDictionary)"/>
		/// 
		/// <example>
		/// The following code uses nvelocity syntax:
		/// 
		/// <code>
		///  $url.link('my link', "%{action='Save'}", "%{class='buttonlink'}")
		/// </code>
		/// 
		/// <para>outputs</para>
		/// 
		/// <code><![CDATA[ <a href="/ControllerNameFromContext/Save.extension_configured" class="buttonlink">my link</a> ]]> </code>
		/// 
		/// </example>
		/// 
		/// <param name="innerContent">The anchor text.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="anchorAttributes">The anchor element attributes.</param>
		/// <returns></returns>
		public string Link(string innerContent, IDictionary parameters, IDictionary anchorAttributes)
		{
			SetEncodeDefault(parameters);
			return "<a " + GetAttributes(anchorAttributes) + " href=\"" + For(parameters) + "\">" + innerContent + "</a>";
		}

		/// <summary>
		/// Outputs a button element constructed using the specified parameters.
		/// </summary>
		/// 
		/// <seealso cref="DefaultUrlBuilder.BuildUrl(UrlInfo,IDictionary)"/>
		/// 
		/// <example>
		/// The following code uses nvelocity syntax:
		/// 
		/// <code>
		///  $url.ButtonLink('my link', "%{action='Save'}")
		/// </code>
		/// 
		/// <para>outputs</para>
		/// 
		/// <code><![CDATA[ <button type="button" onclick="javascript:window.location.href = '/ControllerNameFromContext/Save.extension_configured'">my link</a> ]]> </code>
		/// 
		/// </example>
		/// 
		/// <param name="innerContent">The button text.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		public string ButtonLink(string innerContent, IDictionary parameters)
		{
			SetEncodeDefault(parameters);
			return "<button type=\"button\" onclick=\"javascript:window.location.href = '" + For(parameters) + "'\">" + innerContent + "</button>";
		}

		/// <summary>
		/// Outputs a button element constructed using the specified parameters.
		/// </summary>
		/// 
		/// <seealso cref="DefaultUrlBuilder.BuildUrl(UrlInfo,IDictionary)"/>
		/// 
		/// <example>
		/// The following code uses nvelocity syntax:
		/// 
		/// <code>
		///  $url.ButtonLink('my link', "%{action='Save'}", "%{class='buttonlink'}")
		/// </code>
		/// 
		/// <para>outputs</para>
		/// 
		/// <code><![CDATA[ <button type="button" onclick="javascript:window.location.href = '/ControllerNameFromContext/Save.extension_configured'" class="buttonlink">my link</a> ]]> </code>
		/// 
		/// </example>
		/// 
		/// <param name="innerContent">The button text.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="buttonAttributes">The button element attributes.</param>
		/// <returns></returns>
		public string ButtonLink(string innerContent, IDictionary parameters, IDictionary buttonAttributes)
		{
			SetEncodeDefault(parameters);
			return "<button type=\"button\"" + GetAttributes(buttonAttributes) + " onclick=\"javascript:window.location.href = '" + For(parameters) + "'\">" + innerContent + "</button>";
		}

		private static void SetEncodeDefault(IDictionary parameters)
		{
			if (!parameters.Contains("encode"))
			{
				parameters["encode"] = "true";
			}
		}
	}
}
