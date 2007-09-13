namespace Castle.NewGenerator.Core.MR
{
	using System.Collections.Specialized;
	using System.IO;
	using System.Reflection;
	using Mono.GetOptions;

	[Generator("mrproject", "New MonoRail Project")]
	[GeneratorOptions(typeof(NewProject.CLIOptions))]
	public class NewProject : BaseGenerator
	{
		private string name, solutionName, viewEngine, fileExtension, solutionFolder;
		private bool enableWindsor;

		public NewProject()
		{
			viewEngine = "nvelocity";
		}

		[Param(Required = true)]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[Param]
		public string SolutionName
		{
			get { return solutionName; }
			set { solutionName = value; }
		}

		[Param]
		public bool EnableWindsor
		{
			get { return enableWindsor; }
			set { enableWindsor = value; }
		}

		[Param]
		public string FileExtension
		{
			get { return fileExtension; }
			set { fileExtension = value; }
		}

		[Param]
		public string ViewEngine
		{
			get { return viewEngine; }
			set { viewEngine = value.ToLower(); }
		}

		[Param]
		public string SolutionFolder
		{
			get { return solutionFolder; }
			set { solutionFolder = value; }
		}

		//TODO: Expose the created solution and remove this property
		public string SolutionFilePath
		{
			get { return Path.Combine(solutionFolder, solutionName + ".sln"); }
		}

		/// <summary>
		/// Creates the following structure:
		/// 
		/// <![CDATA[
		/// <pre>
		/// -- \Contents
		/// --    \css
		/// --    \images
		/// --    \javascripts
		/// -- \Filters
		/// -- \Controllers
		/// --    BaseController.cs
		/// -- \Helpers
		/// -- \ViewComponents
		/// -- \Models
		/// -- \Views
		/// --    \components
		/// --    \layouts
		/// --    \rescues
		/// -- default.aspx
		/// -- web.config
		/// </pre>
		/// ]]>
		/// </summary>
		public override void Generate(GeneratorContext context, IGeneratorService generator)
		{
			string basePath = TargetPath;
			string testProjectName = Name + ".Tests";

			if (string.IsNullOrEmpty(fileExtension))
			{
				fileExtension = "castle";
			}

			if (string.IsNullOrEmpty(solutionName))
			{
				solutionName = Name;
				Name = Name + ".Web";
			}

			// Create folders

			if (string.IsNullOrEmpty(solutionFolder))
			{
				solutionFolder = generator.CreateFolderOn(basePath, solutionName, "Creating solution folder");
			}

			string libFolder = generator.CreateFolderOn(solutionFolder, "lib");
			string hintPath = @"..\lib\";

			basePath = solutionFolder;

			string projectFolder = generator.CreateFolderOn(basePath, name, "Creating project folder");

			string controllersFolder = generator.CreateFolderOn(projectFolder, "Controllers");
			
			string viewsFolder = generator.CreateFolderOn(projectFolder, "Views");
			string componentsFolder = generator.CreateFolderOn(viewsFolder, "components");
			string layoutsFolder = generator.CreateFolderOn(viewsFolder, "layouts");
			string rescuesFolder = generator.CreateFolderOn(viewsFolder, "rescues");

			string contentFolder = generator.CreateFolderOn(projectFolder, "Content");
			string cssFolder = generator.CreateFolderOn(contentFolder, "css");
			string imagesFolder = generator.CreateFolderOn(contentFolder, "images");
			string jsFolder = generator.CreateFolderOn(contentFolder, "javascripts");

			generator.CreateFolderOn(projectFolder, "Filters");
			generator.CreateFolderOn(projectFolder, "Helpers");
			generator.CreateFolderOn(projectFolder, "ViewComponents");
			generator.CreateFolderOn(projectFolder, "Models");

			// Add files

			HybridDictionary parameters = new HybridDictionary(true);
			parameters["projectName"] = Name;
			parameters["useWindsor"] = enableWindsor;
			parameters["fileExtension"] = fileExtension;

			generator.AddFromTemplate(controllersFolder, "MR/Controllers/BaseController.vm", "BaseController.cs", parameters);
			generator.AddFromTemplate(layoutsFolder, "MR/Layouts/default.vm", "default.vm", parameters);
			generator.AddFromTemplate(rescuesFolder, "MR/Rescues/generalerror.vm", "generalerror.vm", parameters);

			generator.AddStaticFile(jsFolder, "MR/js/prototype/prototype.js");
			generator.AddStaticFile(jsFolder, "MR/js/prototype/behaviour.js");
			generator.AddStaticFile(jsFolder, "MR/js/prototype/scriptaculous.js");
			generator.AddStaticFile(jsFolder, "MR/js/prototype/formhelper.js");

			// Create folders for test project

			string testProjectFolder = generator.CreateFolderOn(basePath, testProjectName, "Creating test project folder");
			generator.CreateFolderOn(testProjectFolder, "Controllers");
			generator.CreateFolderOn(testProjectFolder, "Filters");
			generator.CreateFolderOn(testProjectFolder, "Helpers");
			generator.CreateFolderOn(testProjectFolder, "ViewComponents");

			// Create configuration files (and others)

			generator.AddFromTemplate(projectFolder, "MR/webconfig.vm", "web.config", parameters);
			generator.AddFromTemplate(projectFolder, "MR/GlobalApplication.vm", "GlobalApplication.cs", parameters);
			generator.AddFromTemplate(projectFolder, "MR/globalasax.vm", "global.asax", parameters);

			// Create vs solution and projects

			VSProject webProject = generator.CreateVSProjectFromFromFolder(name, projectFolder, ProjectType.Web);
			webProject.AddAssemblyReference("Castle.Components.Binder.dll", hintPath);
			webProject.AddAssemblyReference("Castle.Components.Common.EmailSender.dll", hintPath);
			webProject.AddAssemblyReference("Castle.Components.Validator.dll", hintPath);
			webProject.AddAssemblyReference("Castle.Core.dll", hintPath);
			webProject.AddAssemblyReference("Castle.MonoRail.Framework.dll", hintPath);
			webProject.AddAssemblyReference("Castle.MonoRail.Framework.Views.NVelocity.dll", hintPath);
			webProject.AddAssemblyReference("NVelocity.dll", hintPath);
			webProject.AddAssemblyReferenceShared("System");
			webProject.AddAssemblyReferenceShared("System.Data");
			webProject.AddAssemblyReferenceShared("System.Web");
			webProject.AddAssemblyReferenceShared("System.Xml");
			webProject.AddAssemblyReferenceShared("System.Configuration");
			webProject.Save();

			VSProject testProject = generator.CreateVSProjectFromFromFolder(testProjectName, testProjectFolder, ProjectType.Test);
			testProject.AddAssemblyReference("Castle.Core.dll", hintPath);
			testProject.AddAssemblyReference("Castle.MonoRail.Framework.dll", hintPath);
			testProject.AddAssemblyReference("Castle.MonoRail.TestSupport.dll", hintPath);
			testProject.AddAssemblyReference("nunit.framework.dll", hintPath);
			testProject.AddAssemblyReference("WatiN.Core.dll", hintPath);
			testProject.AddAssemblyReference("Rhino.Mocks.dll", hintPath);
			testProject.AddProjectReference(webProject);
			testProject.Save();

			VSSolution solution = generator.CreateSolution(solutionName, solutionFolder, webProject, testProject);
			solution.Save();

			// Create nant script



			// Copy required assemblies to lib folder

			CopyReferencedAssembliesToLibFolder(libFolder, webProject, testProject);
		}

		private static void CopyReferencedAssembliesToLibFolder(string libFolder, params VSProject[] projects)
		{
			foreach(VSProject project in projects)
			{
				foreach(Assembly assm in project.AssemblyReferences)
				{
					string dest = Path.Combine(libFolder, Path.GetFileName(assm.Location));

					if (!File.Exists(dest))
					{
						File.Copy(assm.Location, dest);
					}
				}
			}
		}

		public class CLIOptions : GeneratorConfigurer<NewProject>
		{
			[Option("name", 'n')]
			public string name;

			[Option("solutionname", 's')]
			public string solutionname;

			[Option("viewengine")]
			public string viewengine;

			[Option("handlerextension")]
			public string handlerextension;

			[Option("windsor", 'w')]
			public bool enableWindsor;

			public override void Configure(NewProject generator, string[] args)
			{
				ProcessArgs(args);

				generator.Name = name;
				generator.SolutionName = solutionname;
				generator.FileExtension = handlerextension;
				generator.EnableWindsor = enableWindsor;

				if (viewengine != null)
				{
					generator.ViewEngine = viewengine;
				}
			
				base.Configure(generator, args);
			}
		}
	}
}
