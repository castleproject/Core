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

			Path.Combine(System.Configuration.ConfigurationManager.AppSettings["tests.src"], "templates"));

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

			engine.AddResourceAssembly("Castle.Components.Common.TemplateEngine.NVelocityTemplateEngine.Tests");
			
			(engine as ISupportInitialize).BeginInit();

			StringWriter writer = new StringWriter();

			string templateFile = "Castle.Components.Common.TemplateEngine.NVelocityTemplateEngine.Tests/compiledres/simple.vm";
			Assert.IsTrue( engine.Process(
				new Hashtable(), 
				templateFile, 
				writer) );

			Assert.AreEqual("This is a simple template", writer.GetStringBuilder().ToString());
		}


		[Test]
		public void SimpleTemplateProcessingWithinTwoResources()
		{
			NVelocityTemplateEngine engine = new NVelocityTemplateEngine();

			engine.AddResourceAssembly("Castle.Components.Common.TemplateEngine.NVelocityTemplateEngine.Tests");
			engine.AddResourceAssembly("Castle.Components.Common.TemplateEngine.NVelocityTemplateEngine.Tests.SR");

			(engine as ISupportInitialize).BeginInit();

			StringWriter writer = new StringWriter();

			string templateFile = "Castle.Components.Common.TemplateEngine.NVelocityTemplateEngine.Tests/compiledres/simple.vm";
			Assert.IsTrue(engine.Process(
				new Hashtable(),
				templateFile,
				writer));

			Assert.AreEqual("This is a simple template", writer.GetStringBuilder().ToString());

			// clear the writer for the second run
			writer = new StringWriter();
			
			string secondTemplateFile = "Castle.Components.Common.TemplateEngine.NVelocityTemplateEngine.Tests.SR/compiledres/simple.vm"; 
			Assert.IsTrue(engine.Process(
				new Hashtable(),
				secondTemplateFile,
				writer));
			
			Assert.AreEqual("This is the second simple template", writer.GetStringBuilder().ToString());
			
		}


		[Test]
		public void ShouldProcessAStringTemplate()
		{
			NVelocityTemplateEngine engine = new NVelocityTemplateEngine();

			(engine as ISupportInitialize).BeginInit();

			StringWriter writer = new StringWriter();
			StringWriter contextWriter = new StringWriter();

			string template = "This is a simple template";
			string contextTemplate = "This is a simple $templateType template";
			IDictionary context = new Hashtable();
			context.Add("templateType", typeof(NVelocityTemplateEngine).Name);

			Assert.IsTrue(engine.Process(new Hashtable(), "ShouldProcessAStringTemplate", writer, template));

			Assert.AreEqual("This is a simple template", writer.GetStringBuilder().ToString());

			Assert.IsTrue(engine.Process(context, "ShouldProcessAStringTemplate", contextWriter, contextTemplate));

			Assert.AreEqual("This is a simple NVelocityTemplateEngine template", contextWriter.GetStringBuilder().ToString());
		}

		[Test]
		public void ShouldProcessATextReaderTemplate()
		{
			NVelocityTemplateEngine engine = new NVelocityTemplateEngine();

			(engine as ISupportInitialize).BeginInit();

			StringWriter writer = new StringWriter();
			StringWriter contextWriter = new StringWriter();

			string template = "This is a simple template";
			string contextTemplate = "This is a simple $templateType template";
			IDictionary context = new Hashtable();
			context.Add("templateType", typeof(NVelocityTemplateEngine).Name);

			Assert.IsTrue(engine.Process(new Hashtable(), "ShouldProcessAStringTemplate", writer, new StringReader(template)));

			Assert.AreEqual("This is a simple template", writer.GetStringBuilder().ToString());

			Assert.IsTrue(engine.Process(context, "ShouldProcessAStringTemplate", contextWriter, new StringReader(contextTemplate)));

			Assert.AreEqual("This is a simple NVelocityTemplateEngine template", contextWriter.GetStringBuilder().ToString());
		}
	}
}
