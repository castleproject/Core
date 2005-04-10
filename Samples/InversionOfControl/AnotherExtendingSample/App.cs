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

namespace Extending2
{
	using System;

	using Extending2.Components;
	using Extending2.Dao;

	using Castle.Windsor;
	using Castle.Windsor.Configuration.Interpreters;

	public class App
	{
		[MTAThread]
		public static void Main()
		{
			IWindsorContainer container = new WindsorContainer(new XmlInterpreter("../AppConfig.xml"));

			// Facilities

			container.AddFacility( "transaction", new TransactionFacility() );
			container.AddFacility( "startable", new StartableFacility() );

			// Forms
			container.AddComponent( "form", typeof(MainForm) );
			container.AddComponent( "newblogform", typeof(NewBlog) );

			// Infrastructure
			container.AddComponent( "appRunner", typeof(WinformApplicationRunner) );
			container.AddComponent( "connectionFactory", 
				typeof(IConnectionFactory), typeof(TransactionalConnectionFactory) );

			// Daos
			container.AddComponent( "blogDao", typeof(BlogDao) );
			container.AddComponent( "postDao", typeof(PostDao) );
		}
	}
}
