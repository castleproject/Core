// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Framework
{
	using System;
	using System.Web;
	using Castle.ActiveRecord.Framework.Scopes;

	/// <summary>
	/// HttpModule to set up a session for the request lifetime.
	/// <seealso cref="SessionScope"/>
	/// </summary>
	/// <remarks>
	/// To install the module, you must:
	/// <para>
	///    <list type="number">
	///      <item>
	///        <description>
	///        Add the module to the <c>httpModules</c> configuration section within <c>system.web</c>
	///        </description>
	///      </item>
	///    </list>
	/// </para>
	/// </remarks>
	public class SessionScopeWebModule : IHttpModule
	{
		/// <summary>
		/// The key used to store the session in the context items
		/// </summary>
		protected static readonly String SessionKey = "SessionScopeWebModule.session";
		
		/// <summary>
		/// Used to check whether the ThreadScopeInfo being used is suitable for a web environment
		/// </summary>
		private static bool isWebConfigured;

		/// <summary>
		/// Initialize the module.
		/// </summary>
		/// <param name="app">The app.</param>
		public void Init(HttpApplication app)
		{
			app.BeginRequest += new EventHandler(OnBeginRequest);
			app.EndRequest += new EventHandler(OnEndRequest);

			isWebConfigured = (ActiveRecordBase.holder.ThreadScopeInfo is WebThreadScopeInfo);
		}

		/// <summary>
		/// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"></see>.
		/// </summary>
		public void Dispose()
		{
			
		}

		/// <summary>
		/// Called when request is started, create a session for the request
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnBeginRequest(object sender, EventArgs e)
		{
			if (isWebConfigured) 
			{
				HttpContext.Current.Items.Add(SessionKey, new SessionScope());
			} 
			else 
			{
				throw new ActiveRecordException("Seems that the framework isn't configured properly. (isWeb != true and SessionScopeWebModule is in use) Check the documentation for further information");
			}
		}

		/// <summary>
		/// Called when the request ends, dipose of the scope
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnEndRequest(object sender, EventArgs e)
		{
			SessionScope session = (SessionScope) HttpContext.Current.Items[SessionKey];

			if (session != null)
			{
				session.Dispose();
			}
		}
	}
}
