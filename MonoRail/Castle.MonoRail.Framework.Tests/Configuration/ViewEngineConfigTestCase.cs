// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Tests.Configuration
{
	using System;
	using System.IO;
	using System.Xml;

	using Castle.MonoRail.Framework.Configuration;

	using NUnit.Framework;

	[TestFixture]
	public class ViewEngineConfigTestCase
	{
		[Test]
		public void ShouldProcessAdditonalSourcesElement_IfConfiguringSingleViewEngine()
		{
			string configXml =
				@"
			<monorail>
    <controllers>
      <assembly>Castle.MonoRail.Framework.Tests</assembly>
    </controllers>

    <viewEngine viewPathRoot=""Views"">

      <additionalSources>
        <assembly name=""Castle.MonoRail.Framework.Tests"" namespace=""Castle.MonoRail.Framework.Tests.Content"" />
      </additionalSources>
    </viewEngine>
  </monorail>";

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(configXml);
			ViewEngineConfig config = new ViewEngineConfig();
			config.Deserialize(doc.DocumentElement);

			Assert.IsTrue(config.Sources.Length > 0, "additonal sources not loaded");
		}

		[Test]
		public void ShouldProcessAdditionalSourcesElement_IfConfiguringMultipleViewEngines()
		{
			string configXml = @"
			<monorail>
    <controllers>
      <assembly>Castle.MonoRail.Framework.Tests</assembly>
    </controllers>

    <viewEngines viewPathRoot=""Views"">
		<add type=""Castle.MonoRail.Framework.Tests.Configuration.TestViewEngine, Castle.MonoRail.Framework.Tests"" />
      <additionalSources>
        <assembly name=""Castle.MonoRail.Framework.Tests"" namespace=""Castle.MonoRail.Framework.Tests.Content"" />
      </additionalSources>
    </viewEngines>
  </monorail>";

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(configXml);
			ViewEngineConfig config = new ViewEngineConfig();
			config.Deserialize(doc.DocumentElement);

			Assert.IsTrue(config.Sources.Length > 0, "Additional sources not loaded");

		}

		[Test]
		public void ShouldUseDirectoryNamedViews_IfNoViewPathRootGiven()
		{
			// Multiple view engine config
			string configXml = @"
<monorail>
	<viewEngines>
		<add type=""Castle.MonoRail.Framework.Tests.Configuration.TestViewEngine, Castle.MonoRail.Framework.Tests"" />
	</viewEngines>
</monorail>";

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(configXml);
			ViewEngineConfig config = new ViewEngineConfig();
			config.Deserialize(doc.DocumentElement);

			Assert.AreEqual(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "views"), config.ViewPathRoot);

			// Single view engine config
			configXml = @"
<monorail>
	<viewEngine customEngine=""Castle.MonoRail.Framework.Tests.Configuration.TestViewEngine, Castle.MonoRail.Framework.Tests"" />
</monorail>";

			doc.LoadXml(configXml);

			config.Deserialize(doc.DocumentElement);

			Assert.AreEqual(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "views"), config.ViewPathRoot);
		}
	}

	public class TestViewEngine : ViewEngineBase
	{
		private bool _supportsJSGeneration;
		private string _viewFileExtension;
		private string _jsGeneratorFileExtension;

		public TestViewEngine()
		{
			_supportsJSGeneration = false;
			_viewFileExtension = "test";
			_jsGeneratorFileExtension = "testjs";
		}

		public override bool SupportsJSGeneration
		{
			get { return _supportsJSGeneration; }
		}

		public override string ViewFileExtension
		{
			get { return _viewFileExtension; }
		}

		public override string JSGeneratorFileExtension
		{
			get { return _jsGeneratorFileExtension; }
		}

		public override bool HasTemplate(string templateName)
		{
			throw new NotImplementedException();
		}

		public override void Process(IRailsEngineContext context, Controller controller, string templateName)
		{
			throw new NotImplementedException();
		}

		public override void Process(TextWriter output, IRailsEngineContext context, Controller controller,
									 string templateName)
		{
			throw new NotImplementedException();
		}

		public override void ProcessPartial(TextWriter output, IRailsEngineContext context, Controller controller,
											string partialName)
		{
			throw new NotImplementedException();
		}

		public override object CreateJSGenerator(IRailsEngineContext context)
		{
			throw new NotImplementedException();
		}

		public override void GenerateJS(TextWriter output, IRailsEngineContext context, Controller controller,
										string templateName)
		{
			throw new NotImplementedException();
		}

		public override void ProcessContents(IRailsEngineContext context, Controller controller, string contents)
		{
			throw new NotImplementedException();
		}
	}
}
