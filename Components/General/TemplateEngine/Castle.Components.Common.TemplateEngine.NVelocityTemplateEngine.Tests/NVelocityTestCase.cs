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

namespace Castle.Components.Common.TemplateEngine.NVelocityTemplateEngine.Tests
{
	using System;
	using System.IO;
	using System.Collections;
	using System.ComponentModel;

	using NUnit.Framework;


	[TestFixture]
	public class NVelocityTestCase
	{
		[Test]
		public void SimpleTemplateProcessing()
		{
			String path = Path.Combine(
				AppDomain.CurrentDomain.BaseDirectory, 
#if DOTNET2
			Path.Combine(System.Configuration.ConfigurationManager.AppSettings["tests.src"], "templates"));
#else
			Path.Combine(System.Configuration.ConfigurationSettings.AppSettings["tests.src"], "templates"));
#endif

			ITemplateEngine engine = new NVelocityTemplateEngine(path);
			(engine as ISupportInitialize).BeginInit();

			StringWriter writer = new StringWriter();

			Assert.IsTrue( engine.Process(new Hashtable(), "simple.vm", writer) );

			Assert.AreEqual("This is a simple template", writer.GetStringBuilder().ToString());
		}

		[Test]
		public void SimpleTemplateProcessingWithinResource()
		{
			NVelocityTemplateEngine engine = new NVelocityTemplateEngine();

			engine.AssemblyName = "Castle.Components.Common.TemplateEngine.NVelocityTemplateEngine.Tests";
			
			(engine as ISupportInitialize).BeginInit();

			StringWriter writer = new StringWriter();

			string templateFile = "Castle.Components.Common.TemplateEngine.NVelocityTemplateEngine.Tests/compiledres/simple.vm";
			Assert.IsTrue( engine.Process(
				new Hashtable(), 
				templateFile, 
				writer) );

			Assert.AreEqual("This is a simple template", writer.GetStringBuilder().ToString());
		}
	}
}
