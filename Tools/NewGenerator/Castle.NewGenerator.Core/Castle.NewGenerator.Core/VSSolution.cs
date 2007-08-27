namespace Castle.NewGenerator.Core
{
	using System.Collections.Generic;
	using System.IO;
	using System.Text;

	public class VSSolution
	{
		private readonly string name;
		private readonly string solutionFolder;
		private readonly string targetFileName;
		private readonly List<VSProject> projects = new List<VSProject>();

		public VSSolution(string name, string solutionFolder, string targetFileName)
		{
			this.name = name;
			this.solutionFolder = solutionFolder;
			this.targetFileName = targetFileName;
		}

		public void Add(VSProject project)
		{
			projects.Add(project);
		}

		public void Save()
		{
			File.Delete(targetFileName);

			using(TextWriter writer = new StreamWriter(new FileStream(targetFileName, FileMode.CreateNew, FileAccess.Write), Encoding.UTF8))
			{
				writer.WriteLine("");
				writer.WriteLine("Microsoft Visual Studio Solution File, Format Version 9.00");
				writer.WriteLine("# Visual Studio 2005");

				foreach(VSProject project in projects)
				{
					writer.WriteLine("Project(\"{0}\") = \"{2}\", \"{3}\", \"{1}\"", 
						"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", project.Id.ToString("P"), project.Name, MakeRelative(project.FileName));
					writer.WriteLine("	ProjectSection(WebsiteProperties) = preProject");
					writer.WriteLine("		Debug.AspNetCompiler.Debug = \"True\"");
					writer.WriteLine("		Release.AspNetCompiler.Debug = \"False\"");
					writer.WriteLine("	EndProjectSection");
					writer.WriteLine("EndProject");
				}

				writer.WriteLine("Global");
				writer.WriteLine("	GlobalSection(SolutionConfigurationPlatforms) = preSolution");
				writer.WriteLine("			Debug|Any CPU = Debug|Any CPU");
				writer.WriteLine("			Release|Any CPU = Release|Any CPU");
				writer.WriteLine("		EndGlobalSection");
				writer.WriteLine("		GlobalSection(ProjectConfigurationPlatforms) = postSolution");
				
				foreach(VSProject project in projects)
				{
					writer.WriteLine("			{0}.Debug|Any CPU.ActiveCfg = Debug|Any CPU", project.Id.ToString("P"));
					writer.WriteLine("			{0}.Debug|Any CPU.Build.0 = Debug|Any CPU", project.Id.ToString("P"));
					writer.WriteLine("			{0}.Release|Any CPU.ActiveCfg = Release|Any CPU", project.Id.ToString("P"));
					writer.WriteLine("			{0}.Release|Any CPU.Build.0 = Release|Any CPU", project.Id.ToString("P"));
				}

				writer.WriteLine("		EndGlobalSection");
				writer.WriteLine("	GlobalSection(SolutionProperties) = preSolution");
				writer.WriteLine("			HideSolutionNode = FALSE");
				writer.WriteLine("	EndGlobalSection");
				writer.WriteLine("EndGlobal");
			}
		}

		private string MakeRelative(string fileName)
		{
			return fileName.Substring(solutionFolder.Length + 1);
		}
	}
}
