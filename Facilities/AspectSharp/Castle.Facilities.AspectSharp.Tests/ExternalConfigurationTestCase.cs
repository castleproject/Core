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

namespace Castle.Facilities.AspectSharp.Tests
{
	using System;

	using NUnit.Framework;

	using Castle.Model.Configuration;

	using Castle.Windsor;
	using Castle.Windsor.Configuration.Xml;

	using Castle.MicroKernel.SubSystems.Configuration;

	using Castle.Facilities.AspectSharp.Tests.Components;
	using Castle.Facilities.AspectSharp.Tests.Interceptors;

	/// <summary>
	/// Summary description for ExternalConfigurationTestCase.
	/// </summary>
	[TestFixture]
	public class ExternalConfigurationTestCase
	{
		[Test]
		public void ExternalConfigurationUsage()
		{
			XmlConfigurationStore store = new XmlConfigurationStore("../aop_castle_config.xml");

			WindsorContainer container = new WindsorContainer( store );

			container.AddFacility( "aspectsharp", new AspectSharpFacility() );
			
			// Logger implementation
			container.AddComponent( "logger", typeof(ILogger), typeof(MemoryLogger) );

			// AopAlliance interceptors
			container.AddComponent( "log4netinterceptor", typeof(LoggerTraceInterceptor) );
			
			// Protocol handlers
			container.AddComponent( "protocolhandler.miranda", 
				typeof(IProtocolHandler), typeof(MirandaProtocolHandler) );
			container.AddComponent( "protocolhandler.messenger", 
				typeof(IProtocolHandler), typeof(MessengerProtocolHandler) );
			
			// using...

			ILogger logger = (ILogger) container[ typeof(ILogger) ];
			Assert.AreEqual( 0, logger.Contents.Length );

			IProtocolHandler handler = (IProtocolHandler) 
				container[ "protocolhandler.miranda" ];
			handler.Handle( "contents" );

			handler = (IProtocolHandler) container[ "protocolhandler.messenger" ];
			handler.Handle( "contents" );

			Assert.AreEqual( "Entering Handle Leaving Handle Entering Handle Leaving Handle ", 
				logger.Contents );
		}
	}
}
