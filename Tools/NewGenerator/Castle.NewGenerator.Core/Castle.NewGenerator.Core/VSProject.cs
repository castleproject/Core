namespace Castle.NewGenerator.Core
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using System.Xml;

	public enum ProjectType
	{
		Web,
		Test
	}

	public class VSProject
	{
		const string ns = "http://schemas.microsoft.com/developer/msbuild/2003";

		private readonly Guid id;
		private readonly string name;
		private readonly string targetFileName;
		private readonly XmlDocument document;
		private XmlElement referencesGroup, contentGroup, compileGroup, projectReferencesGroup, foldersGroup;
		private List<Assembly> assemblyList = new List<Assembly>();

		public enum FileType
		{
			Undefined,
			StaticContent,
			SourceCode,
			Folder,
			None
		}

		private VSProject(Guid id, string name, string targetFileName, XmlDocument document)
		{
			this.id = id;
			this.name = name;
			this.targetFileName = targetFileName;
			this.document = document;
		}

		public string Name
		{
			get { return name; }
		}

		public string FileName
		{
			get { return targetFileName; }
		}

		public Guid Id
		{
			get { return id; }
		}

		public IList<Assembly> AssemblyReferences
		{
			get { return assemblyList; }
		}

		public void AddAssemblyReference(string fileName, string hintPath)
		{
			Assembly assm = Assembly.Load(fileName);

			assemblyList.Add(assm);

			XmlElement refElem = (XmlElement) referencesGroup.AppendChild(document.CreateElement("Reference", ns));
			refElem.SetAttribute("Include", assm.FullName);

			refElem.AppendChild(document.CreateElement("SpecificVersion", ns)).AppendChild(document.CreateTextNode("False"));
			refElem.AppendChild(document.CreateElement("HintPath", ns)).AppendChild(document.CreateTextNode(hintPath));
		}

		public void AddAssemblyReferenceShared(string assemblyName)
		{
			XmlElement refElem = (XmlElement)referencesGroup.AppendChild(document.CreateElement("Reference", ns));
			refElem.SetAttribute("Include", assemblyName);
		}

		public void AddProjectReference(VSProject project)
		{
			XmlElement refElem = (XmlElement) projectReferencesGroup.AppendChild(document.CreateElement("ProjectReference", ns));
			refElem.SetAttribute("Include", project.FileName);

			refElem.AppendChild(document.CreateElement("Project", ns)).AppendChild(document.CreateTextNode(project.Id.ToString("P")));
			refElem.AppendChild(document.CreateElement("Name", ns)).AppendChild(document.CreateTextNode(project.Name));
		}

		public void Save()
		{
			File.Delete(targetFileName);

			document.Save(targetFileName);
		}

		public static VSProject Create(string name, string folder, ProjectType type, XmlDocument templateFile)
		{
			Guid id = Guid.NewGuid();

			VSProject project = new VSProject(id, name, Path.Combine(folder, name + ".csproj"), templateFile);

			ImportExistingFilesFromFolder(project, folder, templateFile);

			SetUpProject(id, name, type, templateFile);

			return project;
		}

		private static void SetUpProject(Guid id, string name, ProjectType type, XmlDocument doc)
		{
			XmlElement projectNode = doc.DocumentElement;
			XmlElement pGroup = projectNode["PropertyGroup"];
			XmlElement projectGuid = pGroup["ProjectGuid"];
			XmlElement rootNamespace = pGroup["RootNamespace"];
			XmlElement assemblyName = pGroup["AssemblyName"];
			XmlElement typeGuids = pGroup["ProjectTypeGuids"];

			projectGuid.FirstChild.Value = id.ToString("P");
			rootNamespace.FirstChild.Value = name;
			assemblyName.FirstChild.Value = name;

			if (type == ProjectType.Web)
			{
				typeGuids.FirstChild.Value = "{349c5851-65df-11da-9384-00065b846f21};{fae04ec0-301f-11d3-bf4b-00c04f79efbc}";

				XmlElement importElement = doc.CreateElement("Import", ns);
				importElement.SetAttribute("Project", @"$(MSBuildExtensionsPath)\Microsoft\VisualStudio\v8.0\WebApplications\Microsoft.WebApplication.targets");
				doc.DocumentElement.AppendChild(importElement);


				XmlElement propElement = (XmlElement) doc.DocumentElement.AppendChild(doc.CreateElement("ProjectExtensions", ns)).
					AppendChild(doc.CreateElement("VisualStudio", ns)).
					AppendChild(doc.CreateElement("FlavorProperties", ns));

				propElement.SetAttribute("GUID", "{349c5851-65df-11da-9384-00065b846f21}");

				XmlElement webProjectPropElem = (XmlElement) propElement.AppendChild(doc.CreateElement("WebProjectProperties", ns));

				webProjectPropElem.AppendChild(doc.CreateElement("UseIIS", ns)).AppendChild(doc.CreateTextNode("False"));
				webProjectPropElem.AppendChild(doc.CreateElement("AutoAssignPort", ns)).AppendChild(doc.CreateTextNode("True"));
				webProjectPropElem.AppendChild(doc.CreateElement("DevelopmentServerVPath", ns)).AppendChild(doc.CreateTextNode("/"));
				webProjectPropElem.AppendChild(doc.CreateElement("IISUrl", ns));
				webProjectPropElem.AppendChild(doc.CreateElement("NTLMAuthentication", ns)).AppendChild(doc.CreateTextNode("False"));

//				XmlDocumentFragment fragment = doc.CreateDocumentFragment();
//				fragment.InnerXml = "\r\n" +
//									"  <ProjectExtensions>\r\n" + 
//									"    <VisualStudio>\r\n" + 
//									"      <FlavorProperties GUID=\"{349c5851-65df-11da-9384-00065b846f21}\">\r\n" + 
//									"        <WebProjectProperties>\r\n" + 
//									"          <UseIIS>False</UseIIS>\r\n" + 
//									"          <AutoAssignPort>True</AutoAssignPort>\r\n" + 
//									"          <DevelopmentServerVPath>/</DevelopmentServerVPath>\r\n" + 
//									"          <IISUrl>\r\n" + 
//									"          </IISUrl>\r\n" + 
//									"          <NTLMAuthentication>False</NTLMAuthentication>\r\n" + 
//									"        </WebProjectProperties>\r\n" + 
//									"      </FlavorProperties>\r\n" + 
//									"    </VisualStudio>\r\n" +
//									"  </ProjectExtensions>\r\n";
//				
//				doc.DocumentElement.AppendChild(fragment);
			}
			else
			{
				typeGuids.ParentNode.RemoveChild(typeGuids);
			}
		}

		private static void ImportExistingFilesFromFolder(VSProject project, string folder, XmlDocument templateFile)
		{
			project.referencesGroup = templateFile.CreateElement("ItemGroup", ns);
			project.contentGroup = templateFile.CreateElement("ItemGroup", ns);
			project.compileGroup = templateFile.CreateElement("ItemGroup", ns);
			project.projectReferencesGroup = templateFile.CreateElement("ItemGroup", ns);
			project.foldersGroup = templateFile.CreateElement("ItemGroup", ns);

			templateFile.DocumentElement.AppendChild(project.referencesGroup);
			templateFile.DocumentElement.AppendChild(project.contentGroup);
			templateFile.DocumentElement.AppendChild(project.compileGroup);
			templateFile.DocumentElement.AppendChild(project.projectReferencesGroup);
			templateFile.DocumentElement.AppendChild(project.foldersGroup);

			List<string> entries = new List<string>();
			entries.AddRange(Directory.GetDirectories(folder, "*", SearchOption.AllDirectories));
			entries.AddRange(Directory.GetFiles(folder, "*", SearchOption.AllDirectories));

			foreach(string entry in entries)
			{
				string nodeName;
				XmlElement targetNode;
				string relativeFilePath = entry.Substring(folder.Length + 1);
				FileType fileType = ResolveFileType(entry);

				switch(fileType)
				{
					case FileType.StaticContent:
						nodeName = "Content";
						targetNode = project.contentGroup;
						break;
					case FileType.SourceCode:
						nodeName = "Compile";
						targetNode = project.compileGroup;
						break;
					case FileType.Folder:
						nodeName = "Folder";
						targetNode = project.foldersGroup;
						break;
					case FileType.None:
						nodeName = "None";
						targetNode = project.contentGroup;
						break;
					default:
						continue;
				}

				XmlElement item = templateFile.CreateElement(nodeName, ns);
				item.SetAttribute("Include", relativeFilePath);
				targetNode.AppendChild(item);
			}
		}

		private static FileType ResolveFileType(string entry)
		{
			string ext = Path.GetExtension(entry);
			
			if (ext == "")
			{
				if (new DirectoryInfo(entry).Exists)
				{
					return FileType.Folder;
				}
			}

			switch(ext)
			{
				case ".aspx":
				case ".config":
				case ".asax":
				case ".css":
				case ".htm":
				case ".html":
				case ".js":
				case ".txt":
					return FileType.StaticContent;
				case ".cs":
					return FileType.SourceCode;
				case ".vm":
				case ".njs":
				case ".brail":
					return FileType.None;
				default:
					return FileType.Undefined;
			}
		}
	}
}
