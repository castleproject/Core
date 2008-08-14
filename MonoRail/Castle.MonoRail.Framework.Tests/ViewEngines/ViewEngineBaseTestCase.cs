// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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


#pragma warning disable 67

namespace Castle.MonoRail.Framework.Tests
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using NUnit.Framework;

	[TestFixture]
	public class ViewEngineBaseTestCase
	{
		[Test]
		public void IsTemplateForJSGeneration_ShouldReturnFalseForTemplateWithAnotherExtension()
		{
			const string templateName = "view.test";
			TestableViewEngineBase engine = new TestableViewEngineBase();
			engine.Service(new TestServiceProvider(new TestViewSourceLoader(templateName)));
			Assert.IsFalse(engine.IsTemplateForJSGeneration(templateName),
			               "This is a template typed by extension as not for JSGen and should not have been accepted");
		}

		[Test]
		public void IsTemplateForJSGeneration_ShouldReturnFalseForNonExistentTemplateWithNoExtension()
		{
			TestableViewEngineBase engine = new TestableViewEngineBase();
			engine.Service(new TestServiceProvider(new TestViewSourceLoader()));
			Assert.IsFalse(engine.IsTemplateForJSGeneration("view.testjs"),
			               "This template does not 'exist' so it should have failed");
		}

		[Test]
		public void IsTemplateForJSGeneration_ShouldReturnTrueForExistingTemplateWithCorrectExtension()
		{
			const string templateName = "fakeview.testjs";
			TestableViewEngineBase engine = new TestableViewEngineBase();
			engine.Service(new TestServiceProvider(new TestViewSourceLoader(templateName)));
			Assert.IsTrue(engine.IsTemplateForJSGeneration(templateName),
			              "Should have been accepted and found with correct extension");
		}

		[Test]
		public void IsTemplateForJSGeneration_ShouldReturnTrueForExistingTemplateWithoutExtension()
		{
			TestableViewEngineBase engine = new TestableViewEngineBase();
			engine.Service(new TestServiceProvider(new TestViewSourceLoader("fakeview.testjs")));
			Assert.IsTrue(engine.IsTemplateForJSGeneration("fakeview"),
			              "Should have been accepted and found without extension");
		}
	}

	public class TestViewSourceLoader : IViewSourceLoader
	{
		private readonly string[] views;

		public TestViewSourceLoader(params string[] views)
		{
			this.views = views;
		}

		#region IViewSourceLoader Members

		/// <summary>
		/// Evaluates whether the specified template exists.
		/// </summary>
		/// <param name="templateName">The template name</param>
		/// <returns><c>true</c> if it exists</returns>
		public bool HasSource(string templateName)
		{
			foreach(string view in views)
			{
				if (view.Equals(templateName, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Builds and returns a representation of a view template
		/// </summary>
		/// <param name="templateName">The template name</param>
		/// <returns></returns>
		public IViewSource GetViewSource(string templateName)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Gets a list of views on the specified directory
        /// </summary>
        /// <param name="dirName">Directory name</param>
        /// <param name="fileExtensionsToInclude">Optional fileExtensions to include in listing.</param>
        /// <returns></returns>
        public string[] ListViews(string dirName, params string[] fileExtensionsToInclude)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets/sets the root directory of views, obtained from the configuration.
		/// </summary>
		/// <value></value>
		public string VirtualViewDir
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets/sets the root directory of views, obtained from the configuration.
		/// </summary>
		public string ViewRootDir
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets or sets whether the instance should use cache
		/// </summary>
		public bool EnableCache
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets a list of assembly sources
		/// </summary>
		public IList AssemblySources
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets a list of assembly sources
		/// </summary>
		public IList PathSources
		{
			get { throw new System.NotImplementedException(); }
		}

		/// <summary>
		/// Adds the assembly source.
		/// </summary>
		/// <param name="assemblySourceInfo">The assembly source info.</param>
		public void AddAssemblySource(AssemblySourceInfo assemblySourceInfo)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Adds the path source.
		/// </summary>
		/// <param name="pathSource">The path source.</param>
		public void AddPathSource(string pathSource)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Raised when the view is changed.
		/// </summary>
		public event FileSystemEventHandler ViewChanged;

		#endregion
	}

	public class TestServiceProvider : IServiceProvider
	{
		private readonly IViewSourceLoader viewsourceloader;

		public TestServiceProvider(IViewSourceLoader viewsourceloader)
		{
			this.viewsourceloader = viewsourceloader;
		}

		#region IServiceProvider Members

		///<summary>
		///Gets the service object of the specified type.
		///</summary>
		///
		///<returns>
		///A service object of type serviceType.-or- null if there is no service object of type serviceType.
		///</returns>
		///
		///<param name="serviceType">An object that specifies the type of service object to get. </param><filterpriority>2</filterpriority>
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(IViewSourceLoader))
				return viewsourceloader;
			return null;
		}

		#endregion
	}

	public class TestableViewEngineBase : ViewEngineBase
	{
		private bool _supportsJSGeneration;
		private string _viewFileExtension;
		private string _jsGeneratorFileExtension;

		public TestableViewEngineBase()
		{
			_supportsJSGeneration = false;
			_viewFileExtension = ".test";
			_jsGeneratorFileExtension = ".testjs";
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

		/// <summary>
		/// Processes the view - using the templateName 
		/// to obtain the correct template,
		/// and using the context to output the result.
		/// </summary>
		public override void Process(String templateName, TextWriter output, IEngineContext context, IController controller,
		                             IControllerContext controllerContext)
		{
			throw new NotImplementedException();
		}

		public override void Process(string templateName, string layoutName, TextWriter output,
		                             IDictionary<string, object> parameters)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Should process the specified partial. The partial name must contains
		/// the path relative to the views folder.
		/// </summary>
		public override void ProcessPartial(String partialName, TextWriter output, IEngineContext context,
		                                    IController controller, IControllerContext controllerContext)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Implementors should return a generator instance if
		/// the view engine supports JS generation.
		/// </summary>
		/// <returns>A JS generator instance</returns>
		public override object CreateJSGenerator(JSCodeGeneratorInfo generatorInfo,
		                                         IEngineContext context, IController controller,
		                                         IControllerContext controllerContext)
		{
			throw new NotImplementedException();
		}

		public override void GenerateJS(string templateName, TextWriter output, JSCodeGeneratorInfo generatorInfo,
		                                IEngineContext context, IController controller, IControllerContext controllerContext)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Wraps the specified content in the layout using the 
		/// context to output the result.
		/// </summary>
		public override void RenderStaticWithinLayout(String contents, IEngineContext context, IController controller,
		                                              IControllerContext controllerContext)
		{
			throw new NotImplementedException();
		}
	}
}
