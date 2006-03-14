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
		protected static readonly String SessionKey = "SessionScopeWebModule.session";

		public void Init(HttpApplication app)
		{
			app.BeginRequest += new EventHandler(OnBeginRequest);
			app.EndRequest += new EventHandler(OnEndRequest);
		}

		public void Dispose()
		{
			
		}

		private void OnBeginRequest(object sender, EventArgs e)
		{
			HttpContext.Current.Items.Add(SessionKey, new SessionScope());
		}

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
