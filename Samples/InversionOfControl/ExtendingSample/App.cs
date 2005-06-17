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

namespace ExtendingSample
{
	using System;

	using ExtendingSample.Components;
	using ExtendingSample.Dao;

	using Castle.Windsor;
	using Castle.Windsor.Configuration.Interpreters;

	public class App
	{
		[STAThread]
		public static void Main()
		{
			IWindsorContainer container = new WindsorContainer( new XmlInterpreter("../AppConfig.config") );

			container.AddFacility( "startable", new StartableFacility() );

			container.AddComponent( "appRunner", 
				typeof(WinformApplicationRunner) );
			container.AddComponent( "newsletter", 
				typeof(INewsletterService), typeof(SimpleNewsletterService) );
			container.AddComponent( "smtpemailsender", 
				typeof(IEmailSender), typeof(SmtpEmailSender) );
			container.AddComponent( "templateengine", 
				typeof(ITemplateEngine), typeof(NVelocityTemplateEngine) );
			container.AddComponent( "connectionFactory", 
				typeof(IConnectionFactory), typeof(DefaultConnectionFactory) );
			container.AddComponent( "friendsDao", 
				typeof(FriendsDao) );
			container.AddComponent( "form", 
				typeof(MainForm) );
		}
	}
}
