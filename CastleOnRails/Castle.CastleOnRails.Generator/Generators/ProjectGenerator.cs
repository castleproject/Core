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

namespace Castle.CastleOnRails.Generator.Generators
{
	using System;
	using System.IO;
	using System.Text;
	using System.Collections;
	using System.ComponentModel;

	using Castle.Components.Common.TemplateEngine;
	using Castle.Components.Common.TemplateEngine.NVelocityTemplateEngine;


	/// <summary>
	/// 
	/// </summary>
	public class ProjectGenerator : IGenerator
	{
		private DirectoryInfo projectDir;
		private DirectoryInfo projectTestDir;
		private DirectoryInfo controllersDir;
		private DirectoryInfo modelstDir;
		private DirectoryInfo viewsDir;
		private DirectoryInfo libDir;
		private bool useWindsorIntegration;
		private ITemplateEngine engine;
		private bool isNVelocity;


		public ProjectGenerator()
		{
			// TODO: Move templates to bin directory

			String templatePath = Path.Combine( 
				AppDomain.CurrentDomain.BaseDirectory, 
				@"../Castle.CastleOnRails.Generator/templates" );

			engine = new NVelocityTemplateEngine(templatePath);
			(engine as ISupportInitialize).BeginInit();
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
			}
			else if (!options.Contains("name"))
			{
				writer.WriteLine("name must be specified");
			}
			else 
			{
				DirectoryInfo info = new DirectoryInfo(options["outdir"] as String);

				if (!info.Exists)
				{
					// info.Create(); // Is it safe to use it?
					writer.WriteLine("Error: The specified outdir does not exists.");
				}
			}

			return true;
		}

		public void Execute(IDictionary options, TextWriter writer)
		{
			useWindsorIntegration = options.Contains("windsor");
			isNVelocity = !(options.Contains("view") && options["view"].Equals("aspnet"));
			String name = options["name"] as String;

			// Steps to create the project:

			// 1. Create a controllers and views Directory

			CreateDirectories(name, options, writer);
			CopyFiles(name, options, writer);

			// 2. Create a proper web.config

			CreateWebConfig(options, writer);

			// 3. Create a build file?

			CreateNAntBuildFile(options, writer);

			// 4. Create the sln and the proper csproj (references to assemblies)

			CreateSolution(name, options, writer);
		}

		#endregion

		private void CreateDirectories(string name, IDictionary options, TextWriter writer)
		{
			DirectoryInfo info = new DirectoryInfo(options["outdir"] as String);
			
			try
			{
				projectDir = info.CreateSubdirectory(name);
				projectTestDir = info.CreateSubdirectory(name + ".Tests");
				libDir = info.CreateSubdirectory("lib");

				controllersDir = projectDir.CreateSubdirectory("Controllers");
				modelstDir = projectDir.CreateSubdirectory("Models");
				viewsDir = projectDir.CreateSubdirectory("Views");
			}
			catch(Exception ex)
			{
				writer.WriteLine(ex.Message);
				throw ex;
			}
		}

		private void CreateWebConfig(IDictionary options, TextWriter writer)
		{
			String templateName = (useWindsorIntegration) ? "webconfigwindsor.vm" : "webconfig.vm";

			Hashtable ctx = new Hashtable();

			ctx["viewpath"] = viewsDir.FullName;

			if (isNVelocity)
			{
				ctx["viewenginetypename"] = 
					"Castle.CastleOnRails.Framework.Views.NVelocity.NVelocityViewEngine, Castle.CastleOnRails.Framework.Views.NVelocity";
			}
			else
			{
				ctx["viewenginetypename"] = 
					"Castle.CastleOnRails.Framework.Views.Aspx.AspNetViewEngine, Castle.CastleOnRails.Framework";
			}

			using (StreamWriter swriter = 
					   new StreamWriter(Path.Combine(projectDir.FullName, "web.config"), false, Encoding.Default))
			{
				engine.Process(ctx, templateName, swriter);
			}
		}

		private void CreateNAntBuildFile(IDictionary options, TextWriter writer)
		{
		}

		private void CreateSolution(string name, IDictionary options, TextWriter writer)
		{
			String templateName = "solution.vm";

			Hashtable ctx = new Hashtable();
			ctx.Add("id", Guid.NewGuid().ToString());
			ctx.Add("basename", name);
			ctx.Add("basenameproj", name + ".csproj");
			ctx.Add("basenameprojtests", name + ".Tests.csproj");
			ctx.Add("projectTestDir", projectTestDir.FullName);

			using (StreamWriter swriter = 
					   new StreamWriter(Path.Combine(projectDir.FullName, name + ".sln"), false, Encoding.Default))
			{
				engine.Process(ctx, templateName, swriter);
			}

			templateName = "csproj.vm";

			using (StreamWriter swriter = 
					   new StreamWriter(Path.Combine(projectDir.FullName, name + ".csproj"), false, Encoding.Default))
			{
				engine.Process(ctx, templateName, swriter);
			}

			templateName = "csprojtest.vm";

			using (StreamWriter swriter = 
					   new StreamWriter(Path.Combine(projectTestDir.FullName, name + ".Tests.csproj"), false, Encoding.Default))
			{
				engine.Process(ctx, templateName, swriter);
			}
		}

		private void CopyFiles(string name, IDictionary options, TextWriter writer)
		{
			String sourcedir = AppDomain.CurrentDomain.BaseDirectory;

			CopyFileToLib(sourcedir, "Castle.CastleOnRails.Engine.dll");
			CopyFileToLib(sourcedir, "Castle.CastleOnRails.Framework.dll");
			
			if (isNVelocity)
			{
				CopyFileToLib(sourcedir, "Castle.CastleOnRails.Framework.Views.NVelocity.dll");
			}

			if (useWindsorIntegration)
			{
				CopyFileToLib(sourcedir, "Castle.CastleOnRails.WindsorExtension.dll");
				CopyFileToLib(sourcedir, "Castle.DynamicProxy.dll");
				CopyFileToLib(sourcedir, "Castle.MicroKernel.dll");
				CopyFileToLib(sourcedir, "Castle.Model.dll");
				CopyFileToLib(sourcedir, "Castle.Windsor.dll");
				CopyFileToLib(sourcedir, "Commons.dll");
				CopyFileToLib(sourcedir, "log4net.dll");
				CopyFileToLib(sourcedir, "NVelocity.dll");
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
