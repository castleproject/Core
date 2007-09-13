namespace Castle.NewGenerator.Core
{
	using System.Collections;
	using System.IO;
	using System.Text;
	using System.Xml;
	using Castle.Components.Common.TemplateEngine;

	public class DefaultGeneratorService : IGeneratorService
	{
		protected GeneratorContext context;
		protected ITemplateEngine templateEngine;

		public DefaultGeneratorService(GeneratorContext context, ITemplateEngine templateEngine)
		{
			this.context = context;
			this.templateEngine = templateEngine;
		}

		public string CreateFolderOn(string baseFolder, string folderName)
		{
			return CreateFolderOn(baseFolder, folderName, null);
		}

		public string CreateFolderOn(string baseFolder, string folderName, string description)
		{
			EnsureExists(baseFolder);
			string targetPath = Path.Combine(baseFolder, folderName);
			
			if (EnsureExists(targetPath))
			{
				if (description != null)
				{
					context.Info(description);
				}
			}

			return targetPath;
		}

		public void AddFromTemplate(string folder, string template, string targetFileName, IDictionary parameters)
		{
			string targetFile = Path.Combine(folder, targetFileName);

			if (File.Exists(targetFile))
			{
				context.Info("\tSkipping " + targetFileName);
				return;
			}

			context.Info("\tProcessing " + targetFileName);

			using(TextWriter writer = new StreamWriter(new FileStream(targetFile, FileMode.CreateNew, FileAccess.Write), Encoding.UTF8))
			{
				templateEngine.Process(parameters, template, writer);
				writer.Flush();
			}
		}

		public void AddStaticFile(string folder, string file)
		{
			string source = Path.Combine(context.TemplateFolder.FullName, file);
			string dest = Path.Combine(folder, Path.GetFileName(file));

			if (File.Exists(dest))
			{
				context.Info("\tSkipping " + Path.GetFileName(dest));
				return;
			}

			context.Info("\tCopying " + Path.GetFileName(file));

			File.Copy(source, dest);
		}

		public VSProject CreateVSProjectFromFromFolder(string name, string folder, ProjectType type)
		{
			XmlDocument doc = new XmlDocument(new NameTable());

			XmlNamespaceManager nsManager = new XmlNamespaceManager(doc.NameTable);
			nsManager.AddNamespace("", "http://schemas.microsoft.com/developer/msbuild/2003");
			nsManager.PushScope();

			doc.Load(Path.Combine(context.TemplateFolder.FullName, "VS/CSProject_template.xml"));

			return VSProject.Create(name, folder, type, doc);
		}

		public VSSolution CreateSolution(string name, string solutionFolder, params VSProject[] projects)
		{
			VSSolution solution = new VSSolution(name, solutionFolder, Path.Combine(solutionFolder, name + ".sln"));

			foreach(VSProject project in projects)
			{
				solution.Add(project);
			}

			return solution;
		}

		private static bool EnsureExists(string folder)
		{
			DirectoryInfo dir = new DirectoryInfo(folder);
			
			if (!dir.Exists)
			{
				dir.Create();
				return true;
			}

			return false;
		}
	}
}
