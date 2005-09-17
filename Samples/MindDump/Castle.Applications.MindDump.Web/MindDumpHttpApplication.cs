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

namespace Castle.Applications.MindDump.Web
{
	using System;
	using System.Web;

	using Castle.Windsor;
	using Castle.Facilities.NHibernateExtension;

	using NHibernate;


	public class MindDumpHttpApplication : HttpApplication, IContainerAccessor
	{
		private static readonly String DefaultSessionFactory = "nhibernate.sessfactory.default";

		private static WindsorContainer container;

		public MindDumpHttpApplication()
		{
			this.BeginRequest += new EventHandler(OnBeginRequest);
			this.EndRequest += new EventHandler(OnEndRequest);
		}

		public void OnBeginRequest(object sender, EventArgs e)
		{
			ISessionFactory sessFac = (ISessionFactory) container[ typeof(ISessionFactory) ];

			ISession session = sessFac.OpenSession();

			SessionManager.Push(session, DefaultSessionFactory);

			HttpContext.Current.Items.Add( "nh.session", session );
		}

		public void OnEndRequest(object sender, EventArgs e)
		{
			ISession session = (ISession) HttpContext.Current.Items["nh.session"];

			if (session == null) return;

			try
			{
				SessionManager.Pop(DefaultSessionFactory);

				session.Dispose();
			}
			catch(Exception ex)
			{
				HttpContext.Current.Trace.Warn( "Error", "EndRequest: " + ex.Message, ex );
			}
		}

		public void Application_OnStart() 
		{
			container = new MindDumpContainer();
		}

		public void Application_OnEnd() 
		{
			container.Dispose();
		}

		#region IContainerAccessor implementation

		public IWindsorContainer Container
		{
			get { return container; }
		}

		#endregion
	}
}
