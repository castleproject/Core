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

namespace Castle.MonoRail.Generator.Generators
{
	using System;
	using System.IO;
	using System.Collections;


	/// <summary>
	/// Generates a project skeleton.
	/// </summary>
	public class ProjectGenerator : AbstractGenerator, IGenerator
	{
		private DirectoryInfo rootDir;
		private DirectoryInfo frameworkDir;
		private DirectoryInfo projectDir;
		private DirectoryInfo projectTestDir;
		private DirectoryInfo controllersDir;
		private DirectoryInfo modelstDir;
		private DirectoryInfo viewsDir;
		private DirectoryInfo libDir;
		private bool useWindsorIntegration;
		private bool isNVelocity;

		public ProjectGenerator()
		{
		}

		#region IGenerator Members

		public bool Accept(String name, IDictionary options, TextWriter writer)
		{
			if (!"project".Equals(name))
			{
				return false;
			}
			else if (options.Count == 1)
			{
				writer.WriteLine("Creates a new VS.Net 2003 project structure");
				writer.WriteLine("");
				writer.WriteLine("name     : Project name");
				writer.WriteLine("outdir   : Target directory (must exists)");
				writer.WriteLine("windsor  : [Optional] Enable WindsorContainer Integration");
				writer.WriteLine("view     : [Optional] aspnet|nvelocity (defaults to nvelocity)");
				writer.WriteLine("lang     : [Optional] c#|vb.net (defaults to c#)");
				writer.WriteLine("");
				writer.WriteLine("Example:");
				writer.WriteLine("");
				writer.WriteLine(@"> generator project name:My.CoR.Project windsor outdir:c:\temp");
				writer.WriteLine("");

				return false;
			}
			else if (!options.Contains("outdir"))
			{
				writer.WriteLine("outdir must be specified");
				
				return false;
			}
			else if (!options.Contains("name"))
			{
				writer.WriteLine("name must be specified");
				
				return false;
			}
			else 
			{
				DirectoryInfo info = new DirectoryInfo(options["outdir"] as String);

				if (!info.Exists)
				{
					// info.Create(); // Is it safe to use it?
					writer.WriteLine("Error: The specified outdir does not exists.");

					return false;
				}
			}

			return true;
		}

		public void Execute(IDictionary options, TextWriter writer)
		{
			writer.WriteLine("Generating Project...");

			// Start up

			useWindsorIntegration = options.Contains("windsor");
			isNVelocity = !(options.Contains("view") && options["view"].Equals("aspnet"));
			String name = options["name"] as String;

			DirectoryInfo sysDir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.System));
			frameworkDir = new DirectoryInfo( Path.Combine(sysDir.Parent.FullName, @"Microsoft.NET\Framework\v1.1.4322\") );

			// Steps to create the project:

			// 1. Create a controllers and views Directory

			writer.WriteLine("Creating directories...");
			CreateDirectories(name, options, writer);

			writer.WriteLine("Copying files...");
			CopyFiles(name, options, writer);

			// 2. Create a proper web.config

			writer.WriteLine("Creating web.config...");
			CreateWebConfig(options, writer);

			// 3. Create a build file?

			CreateNAntBuildFile(options, writer);

			// 4. Create the sln and the proper csproj (references to assemblies)

			writer.WriteLine("Creating solution...");
			CreateSolution(name, options, writer);


			writer.WriteLine("Done!");
		}

		#endregion

		private void CreateDirectories(string name, IDictionary options, TextWriter writer)
		{
			DirectoryInfo outdir = new DirectoryInfo(options["outdir"] as String);
			
			try
			{
				rootDir = outdir.CreateSubdirectory(name);
				projectDir = rootDir.CreateSubdirectory(name);
				projectTestDir = rootDir.CreateSubdirectory(name + ".Tests");
				libDir = rootDir.CreateSubdirectory("lib");

				controllersDir = projectDir.CreateSubdirectory("Controllers");
				modelstDir = projectDir.CreateSubdirectory("Models");
				viewsDir = projectDir.CreateSubdirectory("Views");
			}
			catch(Exception ex)
			{
				writer.WriteLine(ex.Message);
				throw;
			}
		}

		private void CreateWebConfig(IDictionary options, TextWriter writer)
		{
			String templateName = "webconfig.vm";

			Hashtable ctx = new Hashtable();
			ctx.Add("name", options["name"]);
			ctx.Add("viewpath", viewsDir.FullName);
			ctx.Add("useWindsorIntegration", useWindsorIntegration);
			ctx.Add("webapppath", projectDir.FullName);

			if (isNVelocity)
			{
				ctx["viewenginetypename"] = 
					"Castle.MonoRail.Framework.Views.NVelocity.NVelocityViewEngine, Castle.MonoRail.Framework.Views.NVelocity";
			}
			else
			{
				ctx["viewenginetypename"] = 
					"Castle.MonoRail.Framework.Views.Aspx.AspNetViewEngine, Castle.MonoRail.Framework";
			}

			WriteTemplateFile(Path.Combine(projectDir.FullName, "web.config"), ctx, templateName);

			templateName = "testconfig.vm";

			WriteTemplateFile(Path.Combine(projectTestDir.FullName, "App.config"), ctx, templateName);
		}

		private void CreateNAntBuildFile(IDictionary options, TextWriter writer)
		{
			// TODO: ?? Is it worthwhile?
		}

		private void CreateSolution(string name, IDictionary options, TextWriter writer)
		{
			Hashtable ctx = new Hashtable();

			ctx.Add("projid", Guid.NewGuid().ToString().ToUpper());
			ctx.Add("projtestid", Guid.NewGuid().ToString().ToUpper());
			ctx.Add("basename", name);
			ctx.Add("basenameproj", name + ".csproj");
			ctx.Add("basenameprojtests", name + ".Tests.csproj");
			ctx.Add("projectDir", projectDir.FullName);
			ctx.Add("projectTestDir", projectTestDir.FullName);
			ctx.Add("frameworkpath", frameworkDir.FullName);
			ctx.Add("isNVelocity", isNVelocity);
			ctx.Add("useWindsorIntegration", useWindsorIntegration);

			WriteTemplateFile(Path.Combine(rootDir.FullName, name + ".sln"), ctx, "solution.vm");
			WriteTemplateFile(Path.Combine(projectDir.FullName, name + ".csproj"), ctx, "csproj.vm");
			WriteTemplateFile(Path.Combine(projectTestDir.FullName, name + ".Tests.csproj"),  ctx, "csprojtest.vm");

			if (useWindsorIntegration)
			{
				WriteTemplateFile(Path.Combine(projectDir.FullName, "global.asax"), ctx, "global.vm");
				WriteTemplateFile(Path.Combine(projectDir.FullName, "MyHttpApplication.cs"), ctx, "httpapp.vm");
				WriteTemplateFile(Path.Combine(projectDir.FullName, "MyContainer.cs"), ctx, "container.vm");
			}
		}

		private void CopyFiles(string name, IDictionary options, TextWriter writer)
		{
			String sourcedir = AppDomain.CurrentDomain.BaseDirectory;

			CopyFileToLib(sourcedir, "Castle.MonoRail.Framework.dll");
			CopyFileToLib(sourcedir, "Castle.MonoRail.TestSupport.dll");
			
			if (isNVelocity)
			{
				CopyFileToLib(sourcedir, "Castle.MonoRail.Framework.Views.NVelocity.dll");
				CopyFileToLib(sourcedir, "NVelocity.dll");
			}

			if (useWindsorIntegration)
			{
				CopyFileToLib(sourcedir, "Castle.MonoRail.WindsorExtension.dll");
				CopyFileToLib(sourcedir, "Castle.DynamicProxy.dll");
				CopyFileToLib(sourcedir, "Castle.MicroKernel.dll");
				CopyFileToLib(sourcedir, "Castle.Model.dll");
				CopyFileToLib(sourcedir, "Castle.Windsor.dll");
			}
		}

		private void CopyFileToLib(string sourcedir, String filename)
		{
			File.Copy( 
				Path.Combine(sourcedir, filename), 
				Path.Combine(libDir.FullName, filename), 
				true );
		}
	}
}
