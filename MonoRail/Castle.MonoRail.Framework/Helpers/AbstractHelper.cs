// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
	using System.Text;

	/// <summary>
	/// Optional base class for helpers. 
	/// Extend from this class only if your helpers needs
	/// a reference to the controller which is using it or
	/// if you need to use one of the protected methods.
	/// </summary>
	public abstract class AbstractHelper : IControllerAware
	{
		#region Controller
		/// <summary>
		/// Store's <see cref="Controller"/> for the current view.
		/// </summary>
		private Controller controller;

		/// <summary>
		/// Sets the controller.
		/// </summary>
		/// <param name="controller">Current view's <see cref="Controller"/>.</param>
		public void SetController(Controller controller)
		{
			this.controller = controller;
		}

		/// <summary>
		/// Gets the controller.
		/// </summary>
		/// <value>The <see cref="Controller"/> used with the current view.</value>
		public Controller Controller
		{
			get { return controller; }
		}
		#endregion 

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
			foreach(DictionaryEntry entry in defaultOptions)
			{
				if (!userOptions.Contains(entry.Key))
				{
					userOptions[entry.Key] = entry.Value;
				}
			}
		}

		#region HTML generation methods

		/// <summary>
		/// Generates HTML element attributes string from <paramref name="attributes"/>.
		/// <code>key1="value1" key2</code>
		/// </summary>
		/// <param name="attributes">The attributes for the element.</param>
		/// <returns><see cref="String"/> to use inside HTML element's tag.</returns>
		/// <remarks>
		/// <see cref="String.Empty"/> is returned if <paramref name="attributes"/> is <c>null</c> or empty.
		/// <para>
		/// If for some <see cref="DictionaryEntry.Key"/> <see cref="DictionaryEntry.Value"/> is <c>null</c> or
		/// <see cref="String.Empty"/> only attribute name is appended to the string.
		/// </para>
		/// </remarks>
		protected String GetAttributes(IDictionary attributes)
		{
			if (attributes == null) return String.Empty;

			String contents = String.Empty;

			foreach (DictionaryEntry entry in attributes)
			{
				if (entry.Value == null || entry.Value.ToString() == String.Empty)
				{
					contents += String.Format("{0} ", entry.Key);
				}
				else
				{
					contents += String.Format("{0}=\"{1}\" ", entry.Key, entry.Value);
				}
			}

			return contents;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="paramMap"></param>
		/// <returns></returns>
		protected String BuildQueryString(IDictionary paramMap)
		{
			if (paramMap == null) return String.Empty;

			StringBuilder sb = new StringBuilder();

			foreach(DictionaryEntry entry in paramMap)
			{
				if (entry.Value == null) continue;

				sb.AppendFormat( "{0}={1}&", 
					UrlEncode(entry.Key.ToString()), UrlEncode(entry.Value.ToString()) );
			}

			return sb.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="leftParams"></param>
		/// <param name="rightParams"></param>
		/// <returns></returns>
		protected String ConcatQueryString(String leftParams, String rightParams)
		{
			// x=y    w=10
			// x=y&w=10

			if (leftParams == null || leftParams.Length == 0)
			{
				return rightParams;
			}
			if (rightParams == null || rightParams.Length == 0)
			{
				return leftParams;
			}

			if (leftParams.EndsWith("&"))
			{
				leftParams = leftParams.Substring( 0, leftParams.Length - 1 );
			}

			return String.Format("{0}&{1}", leftParams, rightParams);
		}

		protected String HtmlEncode(String content)
		{
			return controller.Context.Server.HtmlEncode(content);
		}

		protected String UrlEncode(String content)
		{
			return controller.Context.Server.UrlEncode(content);
		}

		protected String UrlPathEncode(String content)
		{
			return controller.Context.Server.UrlPathEncode(content);
		}

		/// <summary>
		/// Generates script block.
		/// <code>
		/// &lt;script&gt;
		/// scriptContents
		/// &lt;/script&gt;
		/// </code>
		/// </summary>
		/// <param name="scriptContents">The script contents.</param>
		/// <returns><paramref name="scriptContents"/> placed inside <b>script</b> tags.</returns>
		protected String ScriptBlock( String scriptContents )
		{
			return String.Format( "\r\n<script>\r\n{0}</script>\r\n", scriptContents );
		}

		#endregion 
	}
}
