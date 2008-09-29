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
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Text;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Optional base class for helpers. 
	/// Extend from this class only if your helpers needs
	/// a reference to the controller which is using it or
	/// if you need to use one of the protected utility methods.
	/// </summary>
	public abstract class AbstractHelper : IContextAware, IControllerAware
	{
		private const string MonoRailVersion = "RC3_0006";

		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractHelper"/> class.
		/// </summary>
		public AbstractHelper()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractHelper"/> class, 
		/// setting the Controller, Context and ControllerContext.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		public AbstractHelper(IEngineContext engineContext)
		{
			SetController(engineContext.CurrentController, engineContext.CurrentControllerContext);
			SetContext(engineContext);
		}

		#region Context and Controller Reference

		private IController controller;
		private IControllerContext controllerContext;
		private IEngineContext context;
		private IServerUtility serverUtility;

		private UrlHelper urlHelper;

		/// <summary>
		/// Sets the context.
		/// </summary>
		/// <param name="context">The context.</param>
		public virtual void SetContext(IEngineContext context)
		{
			this.context = context;
			serverUtility = context.Server;
		}

		/// <summary>
		/// Sets the controller.
		/// </summary>
		/// <param name="controller">Current view's <see cref="Controller"/>.</param>
		/// <param name="controllerContext">The controller context.</param>
		public virtual void SetController(IController controller, IControllerContext controllerContext)
		{
			this.controller = controller;
			this.controllerContext = controllerContext;
		}

		/// <summary>
		/// Gets the controller.
		/// </summary>
		/// <value>The <see cref="Controller"/> used with the current view.</value>
		public IController Controller
		{
			get { return controller; }
		}

		/// <summary>
		/// Gets the controller context.
		/// </summary>
		/// <value>The controller context.</value>
		public IControllerContext ControllerContext
		{
			get { return controllerContext; }
		}

		/// <summary>
		/// Gets the engine context.
		/// </summary>
		/// <value>The context.</value>
		public IEngineContext Context
		{
			get { return context; }
		}

		#endregion

		/// <summary>
		/// Gets or sets the server utility.
		/// </summary>
		/// <value>The server utility.</value>
		public IServerUtility ServerUtility
		{
			get { return serverUtility; }
			set { serverUtility = value; }
		}

		/// <summary>
		/// Gets the URL helper instance.
		/// </summary>
		/// <value>The URL helper.</value>
		public UrlHelper UrlHelper
		{
			get { return urlHelper ?? (UrlHelper) controllerContext.Helpers["UrlHelper"]; }
			set { urlHelper = value; }
		}

		/// <summary>
		/// Merges <paramref name="userOptions"/> with <paramref name="defaultOptions"/> placing results in
		/// <paramref name="userOptions"/>.
		/// </summary>
		/// <param name="userOptions">The user options.</param>
		/// <param name="defaultOptions">The default options.</param>
		/// <remarks>
		/// All <see cref="IDictionary.Values"/> and <see cref="IDictionary.Keys"/> in <paramref name="defaultOptions"/>
		/// are copied to <paramref name="userOptions"/>. Entries with the same <see cref="DictionaryEntry.Key"/> in
		/// <paramref name="defaultOptions"/> and <paramref name="userOptions"/> are skipped.
		/// </remarks>
		protected void MergeOptions(IDictionary userOptions, IDictionary defaultOptions)
		{
			CommonUtils.MergeOptions(userOptions, defaultOptions);
		}

		/// <summary>
		/// Gets the current context.
		/// </summary>
		/// <value>The current context.</value>
		public IEngineContext CurrentContext
		{
			get { return context; }
			set { context = value; }
		}

		#region Helper methods

		/// <summary>
		/// Renders the a script block with a <c>src</c> attribute
		/// pointing to the url. The url must not have an extension. 
		/// <para>
		/// For example, suppose you invoke it like:
		/// <code>
		/// RenderScriptBlockToSource("/my/url/to/my/scripts");
		/// </code>
		/// </para>
		/// <para>
		/// That will render
		/// <code><![CDATA[
		/// <script type="text/javascript" src="/my/url/to/my/scripts.rails?VERSIONID"></script>
		/// ]]>
		/// </code>
		/// As you see the file extension will be inferred
		/// </para>
		/// </summary>
		/// <param name="url">The url for the scripts (should start with a '/')</param>
		/// <returns>An script block pointing to the given url.</returns>
		protected string RenderScriptBlockToSource(string url)
		{
			string extension = context.UrlInfo.Extension ?? string.Empty;
			return string.Format("<script type=\"text/javascript\" src=\"{0}{1}{2}?" + MonoRailVersion + "\"></script>",
				context.ApplicationPath + url, extension.Length == 0 ? string.Empty : ".", extension);
		}

		/// <summary>
		/// Renders the a script block with a <c>src</c> attribute
		/// pointing to the url sending the querystring as parameter. The url must not have an extension. 
		/// <para>
		/// For example, suppose you invoke it like:
		/// <code>
		/// RenderScriptBlockToSource("/my/url/to/my/scripts", "locale=pt-br");
		/// </code>
		/// </para>
		/// <para>
		/// That will render
		/// <code><![CDATA[
		/// <script type="text/javascript" src="/my/url/to/my/scripts.rails?VERSIONID&locale=pt-br"></script>
		/// ]]>
		/// </code>
		/// As you see the file extension will be inferred
		/// </para>
		/// </summary>
		/// <param name="url">The url for the scripts (should start with a '/')</param>
		/// <param name="queryString">The query string.</param>
		/// <returns>An script block pointing to the given url.</returns>
		protected string RenderScriptBlockToSource(string url, string queryString)
		{
			if (queryString != null && queryString != string.Empty)
			{
				queryString = "&" + queryString;
			}

			string extension = context.UrlInfo.Extension ?? string.Empty;
			return string.Format("<script type=\"text/javascript\" src=\"{0}{1}{2}?" + MonoRailVersion + "{3}\"></script>",
				context.ApplicationPath + url, extension.Length == 0 ? string.Empty : ".", extension, queryString);
		}

		/// <summary>
		/// Generates HTML element attributes string from <paramref name="attributes"/>.
		/// <code>key1="value1" key2</code>
		/// </summary>
		/// <param name="attributes">The attributes for the element.</param>
		/// <returns><see cref="String"/> to use inside HTML element's tag.</returns>
		/// <remarks>
		/// <see cref="string.Empty"/> is returned if <paramref name="attributes"/> is <c>null</c> or empty.
		/// <para>
		/// If for some <see cref="DictionaryEntry.Key"/> <see cref="DictionaryEntry.Value"/> is <c>null</c> or
		/// <see cref="string.Empty"/> only attribute name is appended to the string.
		/// </para>
		/// </remarks>
		protected static string GetAttributes(IDictionary attributes)
		{
			if (attributes == null || attributes.Count == 0) return string.Empty;

			StringBuilder contents = new StringBuilder();

			foreach(DictionaryEntry entry in attributes)
			{
				if (entry.Value == null || entry.Value.ToString() == string.Empty)
				{
					contents.Append(entry.Key);
				}
				else
				{
					contents.AppendFormat("{0}=\"{1}\"", entry.Key.ToString().ToLower(), entry.Value);
				}
				contents.Append(' ');
			}

			return contents.ToString();
		}

		/// <summary>
		/// Builds a query string encoded.
		/// </summary>
		/// <remarks>
		/// Supports multi-value query strings, using any
		/// <see cref="IEnumerable"/> as a value.
		/// <example>
		///	<code>
		/// IDictionary dict = new Hashtable();
		/// dict.Add("id", 5);
		/// dict.Add("selectedItem", new int[] { 2, 4, 99 });
		/// string querystring = BuildQueryString(dict);
		/// // should result in: "id=5&amp;selectedItem=2&amp;selectedItem=4&amp;selectedItem=99"
		/// </code>
		/// </example>
		/// </remarks>
		/// <param name="parameters">The parameters</param>
		public string BuildQueryString(IDictionary parameters)
		{
			return CommonUtils.BuildQueryString(serverUtility, parameters, true);
		}

		/// <summary>
		/// Builds a query string encoded.
		/// </summary>
		/// <remarks>
		/// Supports multi-value query strings, using any
		/// <see cref="IEnumerable"/> as a value.
		/// </remarks>
		/// <param name="parameters">The parameters</param>
		public string BuildQueryString(NameValueCollection parameters)
		{
			return CommonUtils.BuildQueryString(serverUtility, parameters, true);
		}

		/// <summary>
		/// Concat two string in a query string format (<c>key=value&amp;key2=value2</c>) 
		/// building a third string with the result
		/// </summary>
		/// <param name="leftParams">key values</param>
		/// <param name="rightParams">key values</param>
		/// <returns>The concatenation result</returns>
		protected string ConcatQueryString(string leftParams, string rightParams)
		{
			if (leftParams == null || leftParams.Length == 0)
			{
				return rightParams;
			}
			if (rightParams == null || rightParams.Length == 0)
			{
				return leftParams;
			}

			if (leftParams.EndsWith("&") || leftParams.EndsWith("&amp;"))
			{
				leftParams = leftParams.Substring( 0, leftParams.Length - 1 );
			}

			return string.Format("{0}&amp;{1}", leftParams, rightParams);
		}

		/// <summary>
		/// HTML encodes a string and returns the encoded string.  
		/// </summary>
		/// <param name="content">The text string to HTML encode.</param>
		/// <returns>The HTML encoded text.</returns>
		public virtual string HtmlEncode(string content)
		{
			return serverUtility.HtmlEncode(content);
		}

		/// <summary>
		/// Escapes a content replacing line breaks with html break lines.
		/// </summary>
		/// <param name="content">The text to escape.</param>
		/// <returns>The URL encoded and JavaScript escaped text.</returns>
		public String LineBreaksToHtml(String content)
		{
			if (content == null) return string.Empty;

			// TODO: Replace by a regular expression, which should be much more efficient

			return content.Replace("\r", "").Replace("\n", "<br/>");
		}

		/// <summary>
		/// URL encodes a string and returns the encoded string.  
		/// </summary>
		/// <param name="content">The text to URL encode.</param>
		/// <returns>The URL encoded text.</returns>
		public virtual string UrlEncode(string content)
		{
			return serverUtility.UrlEncode(content);
		}

		/// <summary>
		/// URL encodes the path portion of a URL string and returns the encoded string.  
		/// </summary>
		/// <param name="content">The text to URL encode.</param>
		/// <returns>The URL encoded text.</returns>
		public string UrlPathEncode(string content)
		{
			return serverUtility.UrlPathEncode(content);
		}

		/// <summary>
		/// Escapes JavaScript with Url encoding and returns the encoded string.  
		/// </summary>
		/// <remarks>
		/// Converts quotes, single quotes and CR/LFs to their representation as an escape character.
		/// </remarks>
		/// <param name="content">The text to URL encode and escape JavaScript within.</param>
		/// <returns>The URL encoded and JavaScript escaped text.</returns>
		public string JavaScriptEscape(string content)
		{
			if (string.IsNullOrEmpty(content)) return content;

			return serverUtility.JavaScriptEscape(content);
		}

		/// <summary>
		/// Builds a JS associative array based on the specified dictionary instance.
		/// <para>
		/// For example: <c>{name: value, other: 'another'}</c>
		/// </para>
		/// </summary>
		/// <param name="jsOptions">The js options.</param>
		/// <returns>An associative array in javascript</returns>
		public static string JavascriptOptions(IDictionary<string, string> jsOptions)
		{
			return JavascriptOptions(jsOptions as IDictionary);
		}

		/// <summary>
		/// Builds a JS associative array based on the specified dictionary instance.
		/// <para>
		/// For example: <c>{name: value, other: 'another'}</c>
		/// </para>
		/// </summary>
		/// <param name="jsOptions">The js options.</param>
		/// <returns>An associative array in javascript</returns>
		public static string JavascriptOptions(IDictionary jsOptions)
		{
			if (jsOptions == null || jsOptions.Count == 0)
			{
				return "{}";
			}

			StringBuilder sb = new StringBuilder(jsOptions.Count * 10);
			sb.Append("{");
			bool comma = false;

			foreach (DictionaryEntry entry in jsOptions)
			{
				if (!comma) comma = true; else sb.Append(", ");

				sb.Append(string.Format("{0}:{1}", entry.Key, entry.Value));
			}

			sb.Append("}");
			return sb.ToString();
		}

		/// <summary>
		/// Generates script block.
		/// <code>
		/// &lt;script type=\"text/javascript\"&gt;
		/// scriptContents
		/// &lt;/script&gt;
		/// </code>
		/// </summary>
		/// <param name="scriptContents">The script contents.</param>
		/// <returns><paramref name="scriptContents"/> placed inside <b>script</b> tags.</returns>
		public static string ScriptBlock(string scriptContents)
		{
			return "\r\n<script type=\"text/javascript\">\r\n" + scriptContents + "</script>\r\n";
		}

		/// <summary>
		/// Quotes the specified string with double quotes
		/// </summary>
		/// <param name="content">The content.</param>
		/// <returns>A quoted string</returns>
		public static string Quote(object content)
		{
			return "\"" + content + "\"";
		}

		/// <summary>
		/// Quotes the specified string with singdoublele quotes
		/// </summary>
		/// <param name="items">Items to quote</param>
		/// <returns>A quoted string</returns>
		public static string[] Quote(object[] items)
		{
			string[] quotedItems = new string[items.Length];

			int index = 0;

			foreach(string item in items)
			{
				quotedItems[index++] = Quote(item);
			}

			return quotedItems;
		}

		/// <summary>
		/// Quotes the specified string with double quotes
		/// </summary>
		/// <param name="content">The content.</param>
		/// <returns>A quoted string</returns>
		public static string SQuote(object content)
		{
			return "\'" + content + "\'";
		}

		#endregion 
	}
}
