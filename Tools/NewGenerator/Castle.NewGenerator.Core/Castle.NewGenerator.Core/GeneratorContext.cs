namespace Castle.NewGenerator.Core
{
	using System;
	using System.IO;

	public class GeneratorContext
	{
		private readonly DirectoryInfo templateFolder;
		private readonly DirectoryInfo workingFolder;

		public GeneratorContext(string workingFolder, string templateFolder)
		{
			this.templateFolder = new DirectoryInfo(templateFolder);
			this.workingFolder = new DirectoryInfo(workingFolder);
		}

		public DirectoryInfo WorkingFolder
		{
			get { return workingFolder; }
		}

		public DirectoryInfo TemplateFolder
		{
			get { return templateFolder; }
		}

		public void Info(string message)
		{
			Console.WriteLine(message);
		}
	}
}
