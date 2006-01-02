using ExtendedProperties = Commons.Collections.ExtendedProperties;
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

namespace NVelocity.Test
{
	using System;
	using System.IO;
	using NUnit.Framework;
	using NVelocity.App;
	using NVelocity.Runtime;


	[TestFixture]
	public class ComponentDirectiveTestCase : BaseTestCase
	{
		private VelocityEngine ve;
		private ExtendedProperties testProperties;

		[SetUp]
		protected void SetUp()
		{
			ve = new VelocityEngine();

			ExtendedProperties ep = new ExtendedProperties();
			
			ep.SetProperty(RuntimeConstants_Fields.FILE_RESOURCE_LOADER_PATH, 
				TemplateTestBase_Fields.FILE_RESOURCE_LOADER_PATH);

			ep.SetProperty(RuntimeConstants_Fields.RUNTIME_LOG_ERROR_STACKTRACE, "true");
			ep.SetProperty(RuntimeConstants_Fields.RUNTIME_LOG_WARN_STACKTRACE, "true");
			ep.SetProperty(RuntimeConstants_Fields.RUNTIME_LOG_INFO_STACKTRACE, "true");
			ep.SetProperty("userdirective", "NVelocity.Runtime.Directive.Component;NVelocity,NVelocity.Runtime.Directive.BlockComponent;NVelocity");

			ve.Init(ep);

			testProperties = new ExtendedProperties();
			testProperties.Load(new FileStream(TemplateTestBase_Fields.TEST_CASE_PROPERTIES, FileMode.Open, FileAccess.Read));
		}

		[Test]
		public void LineComponent1()
		{
			VelocityContext context = new VelocityContext();

			Template template = ve.GetTemplate(
				getFileName(null, "componentusage1", TemplateTestBase_Fields.TMPL_FILE_EXT));
			
			StringWriter writer = new StringWriter();

			template.Merge(context, writer);

			System.Console.WriteLine( writer.GetStringBuilder().ToString() );

			writer = new StringWriter();

			template.Merge(context, writer);

			System.Console.WriteLine( writer.GetStringBuilder().ToString() );

			writer = new StringWriter();

			template.Merge(context, writer);

			System.Console.WriteLine( writer.GetStringBuilder().ToString() );
		}

	}
}
