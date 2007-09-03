namespace Castle.NewGenerator.Core
{
	using System.Collections;

	public interface IGeneratorService
	{
		string CreateFolderOn(string baseFolder, string folderName);
		
		string CreateFolderOn(string baseFolder, string folderName, string description);

		void AddFromTemplate(string folder, string template, string targetFileName, IDictionary parameters);
		
		void AddStaticFile(string folder, string file);

		VSProject CreateVSProjectFromFromFolder(string name, string folder, ProjectType type);

		VSSolution CreateSolution(string name, string solutionFolder, params VSProject[] projects);
	}
}
