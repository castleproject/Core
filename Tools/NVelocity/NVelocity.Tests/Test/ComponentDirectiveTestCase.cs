// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
	using System.IO;
	using App;
	using NUnit.Framework;
	using Runtime;
	using ExtendedProperties = global::Commons.Collections.ExtendedProperties;

	[TestFixture]
	public class ComponentDirectiveTestCase : BaseTestCase
	{
		private VelocityEngine velocityEngine;
		private ExtendedProperties testProperties;

		[SetUp]
		protected void SetUp()
		{
			velocityEngine = new VelocityEngine();

			ExtendedProperties extendedProperties = new ExtendedProperties();

			extendedProperties.SetProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH,
			                               TemplateTest.FILE_RESOURCE_LOADER_PATH);

			extendedProperties.SetProperty(RuntimeConstants.RUNTIME_LOG_ERROR_STACKTRACE, "true");
			extendedProperties.SetProperty(RuntimeConstants.RUNTIME_LOG_WARN_STACKTRACE, "true");
			extendedProperties.SetProperty(RuntimeConstants.RUNTIME_LOG_INFO_STACKTRACE, "true");
			extendedProperties.SetProperty("userdirective",
			                               "NVelocity.Runtime.Directive.Component;NVelocity,NVelocity.Runtime.Directive.BlockComponent;NVelocity");

			velocityEngine.Init(extendedProperties);

			testProperties = new ExtendedProperties();
			testProperties.Load(new FileStream(TemplateTest.TEST_CASE_PROPERTIES, FileMode.Open, FileAccess.Read));
		}

		[Test]
		public void LineComponent1()
		{
			VelocityContext context = new VelocityContext();

			Template template = velocityEngine.GetTemplate(
				GetFileName(null, "componentusage1", TemplateTest.TMPL_FILE_EXT));

			StringWriter writer = new StringWriter();

			template.Merge(context, writer);

			System.Console.WriteLine(writer.GetStringBuilder().ToString());

			writer = new StringWriter();

			template.Merge(context, writer);

			System.Console.WriteLine(writer.GetStringBuilder().ToString());

			writer = new StringWriter();

			template.Merge(context, writer);

			System.Console.WriteLine(writer.GetStringBuilder().ToString());
		}
	}
}