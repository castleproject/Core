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

namespace Castle.MonoRail.WindsorExtension.Tests
{
	using System;
	using System.Configuration;
	using System.IO;
	using System.Xml;

	using Castle.MonoRail.Framework.Configuration;
	
	using NUnit.Framework;

	[TestFixture]
	public class WindsorMonoRailSectionHandlerTestCase
	{
#if DOTNET2
		String dir = Path.Combine(ConfigurationManager.AppSettings["tests.src"], "ConfigFiles/");
#else
		String dir = Path.Combine(ConfigurationSettings.AppSettings["tests.src"], "ConfigFiles/");
#endif

		[Test]
		public void SimpleTest()
		{
			MonoRailConfiguration config = GetConfig("SimpleTest.xml");

			AssertCommon(config);
		}

		[Test, Explicit]
		public void ComplexTest()
		{
			MonoRailConfiguration config = GetConfig("ComplexTest.xml");

			AssertCommon(config);
			Assert.AreEqual( "PhotosDemo", config.ControllersConfig.Assemblies[1] );
		}

		[Test, Explicit]
		public void PiComplexTest()
		{
			MonoRailConfiguration config = GetConfig("Pi-ComplexTest.xml");

			AssertCommon(config);
			Assert.AreEqual( "PhotosDemo", config.ControllersConfig.Assemblies[1] );
		}

		private static void AssertCommon( MonoRailConfiguration config )
		{
			Assert.AreEqual( "castleproject.org", config.SmtpConfig.Host);
			Assert.AreEqual( "secret", config.SmtpConfig.Password);
			Assert.AreEqual( "JoeDoe", config.SmtpConfig.Username);
			Assert.AreEqual( "Castle.MonoRail.Framework.Views.NVelocity.NVelocityViewEngine", 
			                 config.ViewEngineConfig.CustomEngine.FullName );			
			Assert.AreEqual( "MoviesDemo", config.ControllersConfig.Assemblies[0] );
	
			Assert.IsTrue( config.ViewEngineConfig.ViewPathRoot.EndsWith("views") );
		}

		public MonoRailConfiguration GetConfig(string fileName)
		{
			WindsorMonoRailSectionHandler configHandler = new WindsorMonoRailSectionHandler();

			XmlDocument configFile = GetXmlDocument(fileName);

			return configHandler.Create(null, null, configFile) as MonoRailConfiguration;
		}

		public XmlDocument GetXmlDocument(string fileName)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(GetFullPath(fileName));

			return doc;
		}

		private string GetFullPath(String fileName)
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir + fileName);
		}
	}
}
