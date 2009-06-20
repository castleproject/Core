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

namespace Castle.MonoRail.Views.AspView.Tests.Engine.Config
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using System.Web;
	using AspView.Compiler;
	using Configuration;
	using Framework;
	using NUnit.Framework;

	[TestFixture]
	public class ProgrammaticConfigTestFixture
	{
		AspViewEngine engine;
		IAspViewEngineTestAccess engineWithTestAccess;
		private Func<HttpApplication> oldApplicationInstanceGetter;

		[SetUp]
		public void SetUp()
		{
			var testInstance = new MyApplication();
			oldApplicationInstanceGetter = AspViewEngine.GetApplicationInstance;
			AspViewEngine.GetApplicationInstance = () => testInstance;
			var basePath = Environment.CurrentDirectory;
			engine = new AspViewEngine();
			engineWithTestAccess = engine;			

			engineWithTestAccess.SetViewSourceLoader(new StubViewSourceLoader());
			engineWithTestAccess.SetCompilationContext(
				new List<ICompilationContext>
					{
						new CompilationContext(
							new DirectoryInfo(basePath),
							new DirectoryInfo(basePath),
							new DirectoryInfo(basePath),
							new DirectoryInfo(basePath))
					});
		}

		[TearDown]
		public void TearDown()
		{
			AspViewEngine.GetApplicationInstance = oldApplicationInstanceGetter;
		}

		[Test]
		public void ProgrammaticConfigIsWorking()
		{
			engine.Initialize();

			var field = typeof(AspViewEngine).GetField("options", BindingFlags.Static | BindingFlags.NonPublic);
			var options = (AspViewEngineOptions)field.GetValue(engine);
			Assert.IsTrue(options.CompilerOptions.AllowPartiallyTrustedCallers);
			Assert.IsTrue(options.CompilerOptions.AutoRecompilation);
			Assert.IsTrue(options.CompilerOptions.Debug);
			Assert.IsTrue(options.CompilerOptions.KeepTemporarySourceFiles);
			Assert.AreEqual(options.CompilerOptions.TemporarySourceFilesDirectory, "DIR");

			
		}
		private class MyApplication : HttpApplication, IAspViewConfigurationEvents
		{
			public void Configure(CompilerOptionsBuilder compilerOptions)
			{
				compilerOptions
					.CompileForDebugging()
					.AllowPartiallyTrustedCallers()
					.AutoRecompilation()
					.KeepTemporarySourceFiles()
					.UsingTemporarySourceFilesDirectory("DIR");
			}
		}
	}

	internal class StubViewSourceLoader : IViewSourceLoader
	{
		void Dummy()
		{
			ViewChanged(null, new FileSystemEventArgs(WatcherChangeTypes.All, "", ""));
		}

		public bool HasSource(string sourceName)
		{
			throw new NotImplementedException();
		}

		public IViewSource GetViewSource(string templateName)
		{
			throw new NotImplementedException();
		}

		public string[] ListViews(string dirName, params string[] fileExtensionsToInclude)
		{
			throw new NotImplementedException();
		}

		public string VirtualViewDir
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public string ViewRootDir
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public bool EnableCache
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public IList AssemblySources
		{
			get { throw new NotImplementedException(); }
		}

		public IList PathSources
		{
			get { throw new NotImplementedException(); }
		}

		public void AddAssemblySource(AssemblySourceInfo assemblySourceInfo)
		{
			throw new NotImplementedException();
		}

		public void AddPathSource(string pathSource)
		{
			throw new NotImplementedException();
		}

		public event FileSystemEventHandler ViewChanged;
	}
}