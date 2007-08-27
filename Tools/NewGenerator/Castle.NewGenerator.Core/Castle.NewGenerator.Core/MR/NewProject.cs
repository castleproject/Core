namespace Castle.NewGenerator.Core.MR
{
	using System.Collections.Specialized;
	using Mono.GetOptions;

	[Generator("mrproject", "New MonoRail Project")]
	[GeneratorOptions(typeof(NewProject.CLIOptions))]
	public class NewProject : BaseGenerator
	{
		private string name, solutionName, viewEngine;
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
		public string ViewEngine
		{
			get { return viewEngine; }
			set { viewEngine = value.ToLower(); }
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
			string solutionFolder;
			string testProjectName = Name + ".Tests";

			if (solutionName == null)
			{
				solutionName = Name;
				Name = Name + ".Web";
			}

			// Create folders

			solutionFolder = generator.CreateFolderOn(basePath, solutionName, "Creating solution folder");
			generator.CreateFolderOn(solutionFolder, "lib");

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

			generator.AddFromTemplate(controllersFolder, "MR/Controllers/BaseController.vm", "BaseController.cs", parameters);
			generator.AddFromTemplate(layoutsFolder, "MR/Layouts/default.vm", "default.vm", parameters);
			generator.AddFromTemplate(rescuesFolder, "MR/Rescues/generalerror.vm", "generalerror.vm", parameters);

			generator.AddStaticFile(jsFolder, "MR/js/prototype/prototype.js");
			generator.AddStaticFile(jsFolder, "MR/js/prototype/behaviour.js");
			generator.AddStaticFile(jsFolder, "MR/js/prototype/scriptaculous.js");
			generator.AddStaticFile(jsFolder, "MR/js/prototype/formhelper.js");

			// Create folders for test project

			string testProjectFolder = generator.CreateFolderOn(basePath, testProjectName, "Creating test project folder");
			string testControllersFolder = generator.CreateFolderOn(testProjectFolder, "Controllers");
			string testFiltersFolder = generator.CreateFolderOn(testProjectFolder, "Filters");
			string testHelpersFolder = generator.CreateFolderOn(testProjectFolder, "Helpers");
			string testViewComponentsFolder = generator.CreateFolderOn(testProjectFolder, "ViewComponents");

			// Create configuration files

			// Create vs solution and projects

			VSProject webProject = generator.CreateVSProjectFromFromFolder(name, projectFolder, ProjectType.Web);
//			webProject.AddAssemblyReference("");
			webProject.Save();

			VSProject testProject = generator.CreateVSProjectFromFromFolder(testProjectName, testProjectFolder, ProjectType.Test);
//			testProject.AddAssemblyReference("");
			testProject.Save();

			VSSolution solution = generator.CreateSolution(name, solutionFolder, webProject, testProject);
			solution.Save();

			// Create nant script

			// Copy required assemblies to lib folder
		}

		public class CLIOptions : GeneratorConfigurer<NewProject>
		{
			[Option("name", 'n')]
			public string name;

			[Option("solutionname", 's')]
			public string solutionname;

			public override void Configure(NewProject generator, string[] args)
			{
				ProcessArgs(args);

				generator.Name = name;
				generator.SolutionName = solutionname;
			
				base.Configure(generator, args);
			}
		}
	}
}
