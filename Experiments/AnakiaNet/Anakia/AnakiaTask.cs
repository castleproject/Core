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

namespace Anakia
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Xml;
	using Commons.Collections;
	using NAnt.Core;
	using NAnt.Core.Attributes;
	using NAnt.Core.Types;
	using NVelocity;
	using NVelocity.App;
	using NVelocity.Context;

	[TaskName("anakia")]
	public class AnakiaTask : Task
	{
		private DirectoryInfo targetDir;
		private FileSet sourceFileSet;
		private String navigationFile;
		private String projectFile;
		private String templateFile;

		private VelocityEngine velocity;
		private Template template;
		private Folder root;
		private XmlDocument siteMapDoc;
		private XmlDocument projectDom;

		public AnakiaTask()
		{
		}

		#region Properties

		[TaskAttribute("navigationfile")]
		public String NavigationFile
		{
			get { return navigationFile; }
			set { navigationFile = value.ToLower(); }
		}

		[TaskAttribute("templatefile")]
		public String TemplateFile
		{
			get { return templateFile; }
			set { templateFile = value; }
		}

		[TaskAttribute("projectfile")]
		public String ProjectFile
		{
			get { return projectFile; }
			set { projectFile = value; }
		}

		[TaskAttribute("targetdir")]
		public DirectoryInfo TargetDir
		{
			get { return targetDir; }
			set { targetDir = value; }
		}

		[BuildElement("source")]
		public FileSet SourceFileSet
		{
			get { return sourceFileSet; }
			set { sourceFileSet = value; }
		}

		#endregion

		#region overrides

		protected override void InitializeTask(XmlNode taskNode)
		{
			base.InitializeTask(taskNode);

			// Initializes NVelocity

			velocity = new VelocityEngine();

			ExtendedProperties props = new ExtendedProperties();
			velocity.Init(props);

			template = velocity.GetTemplate(templateFile);

			// TODO: validate all arguments are present
		}

		protected override void ExecuteTask()
		{
			projectDom = new XmlDocument();
			projectDom.Load(projectFile);

			DirectoryInfo basedir = sourceFileSet.BaseDirectory;

			root = new Folder("castle");

			try
			{
				ArrayList staticFilesToCopy = new ArrayList();

				foreach(String fullFileName in sourceFileSet.FileNames)
				{
					String dir = Path.GetDirectoryName(fullFileName);
					String fileName = Path.GetFileName(fullFileName);
					String nodeName = String.Empty;

					Folder folder;
					String[] folders;

					if (basedir.FullName.ToLower() != dir.ToLower())
					{
						nodeName = dir.Substring(basedir.FullName.Length + 1);
					}

					if (IsStaticFile(fullFileName))
					{
						if (nodeName != String.Empty)
						{
							staticFilesToCopy.Add(new FileToCopy(
							                      	fullFileName, root.Name + "/" + nodeName + "/" + fileName));
						}
						else
						{
							staticFilesToCopy.Add(new FileToCopy(
							                      	fullFileName, root.Name + "/" + fileName));
						}

						continue;
					}

					if (nodeName != String.Empty)
					{
						folders = nodeName.Split('\\');
						folder = GetFolderInstance(folders);
					}
					else
					{
						folder = root;
					}

					XmlDocument doc = new XmlDocument();

					doc.Load(fullFileName);

					DocumentNode node = new DocumentNode(nodeName, fileName, doc, CreateMeta(doc));
					folder.Documents.Add(node);
				}

				siteMapDoc = CreateSiteMap();

				siteMapDoc.Save(@"E:\dev\castleall\trunk\Experiments\Documentation\docs\sitemap.xml");

				ITreeWalker walker = new BreadthFirstWalker();

				walker.Walk(root, new Act(CreateBreadCrumb));
				walker.Walk(root, new Act(CreateHtml));

				foreach(FileToCopy file2Copy in staticFilesToCopy)
				{
					String dir = Path.GetDirectoryName(file2Copy.TargetFile);

					EnsureDirExists(dir);

					String targetFile = targetDir.FullName + "/" + file2Copy.TargetFile;

					if (File.Exists(targetFile))
					{
						continue;
					}

					File.Copy(file2Copy.SourceFile, targetFile);
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex);
				Console.Read();
			}
		}

		#endregion

		private bool IsStaticFile(string fileName)
		{
			String ext = Path.GetExtension(fileName);

			return ext != ".xml";
		}

		private DocumentMeta CreateMeta(XmlDocument xmlDocument)
		{
			DocumentMeta meta = new DocumentMeta();

			XmlNode properties = xmlDocument.SelectSingleNode("document/properties");

			if (properties != null)
			{
				foreach(XmlNode child in properties.ChildNodes)
				{
					if (child.NodeType != XmlNodeType.Element) continue;

					if (child.Name == "title")
					{
						meta.Title = child.ChildNodes[0].Value;
					}
					else if (child.Name == "categories")
					{
						ArrayList categories = new ArrayList();

						foreach(XmlNode cat in child.SelectNodes("category"))
						{
							categories.Add(cat.ChildNodes[0].Value);
						}

						meta.Categories = (String[]) categories.ToArray(typeof(String));
					}
					else if (child.Name == "audience")
					{
						meta.Audience = child.ChildNodes[0].Value;
					}
				}
			}

			return meta;
		}

		private void CreateHtml(DocumentNode node)
		{
			EnsureDirExists(node.ParentFolder.Path);

			if (node.NodeType == NodeType.Navigation) return;

			DirectoryInfo targetFolder = new DirectoryInfo(
				targetDir.FullName + "/" + node.ParentFolder.Path);

			FileInfo htmlFileInTarget = new FileInfo(
				Path.Combine(targetFolder.FullName, node.TargetFilename));

			if (htmlFileInTarget.Exists)
			{
				htmlFileInTarget.Delete();
			}

			IContext ctx = CreateContext(node);

			using(StreamWriter writer = new StreamWriter(htmlFileInTarget.FullName, false))
			{
				template.Merge(ctx, writer);
			}
		}

		private void EnsureDirExists(String path)
		{
			DirectoryInfo finalDir = new DirectoryInfo(targetDir.FullName + "/" + path);

			if (!finalDir.Exists)
			{
				finalDir.Create();
			}
		}

		private IContext CreateContext(DocumentNode node)
		{
			VelocityContext context = new VelocityContext();

			context.Put("breadcrumbs", node.BreadCrumbs);
			context.Put("meta", node.Meta);
			context.Put("doc", node.XmlDoc);
			context.Put("root", node.XmlDoc.DocumentElement);
			context.Put("node", node);
			context.Put("sitemap", siteMapDoc.DocumentElement);
			context.Put("project", projectDom.DocumentElement);
			context.Put("folder", node.ParentFolder);
			context.Put("converter", new SimpleConverter());

			String relativePath = String.Empty;

			if (node.ParentFolder.Level == 1)
			{
				relativePath = ".";
			}
			else
			{
				for(int i = 0; i < node.ParentFolder.Level - 1; i++)
				{
					if (i == 0)
					{
						relativePath += "..";
					}
					else
					{
						relativePath += "/..";
					}
				}
			}

			context.Put("relativePath", relativePath);

			return context;
		}

		private void CreateBreadCrumb(DocumentNode node)
		{
			if (node.NodeType == NodeType.Navigation) return;

			EnsureFolderHasBreadCrumb(node.ParentFolder);

			node.BreadCrumbs.AddRange(node.ParentFolder.BreadCrumbs);

			if (node.NodeType != NodeType.Index)
			{
				node.BreadCrumbs.Add(new BreadCrumb("", node.Meta.Title));
			}
		}

		private void EnsureFolderHasBreadCrumb(Folder folder)
		{
			if (folder.BreadCrumbs.Count != 0) return;

			if (folder.Parent != null)
			{
				folder.BreadCrumbs.AddRange(folder.Parent.BreadCrumbs);
			}

			folder.BreadCrumbs.Add(new BreadCrumb(folder.Path + "/index.html", folder.Name));
		}

		private XmlDocument CreateSiteMap()
		{
			ITreeWalker walker = new DepthFirstWalker();
			walker.Walk(root, new Act(CreateNodeSiteMapFragments));

			XmlDocument siteMapDoc = new XmlDocument();
			XmlElement sitemapElem = siteMapDoc.CreateElement("sitemap");
			siteMapDoc.AppendChild(sitemapElem);

			AppendFragments(root, sitemapElem);

			return siteMapDoc;
		}

		private void AppendFragments(Folder folder, XmlElement elem)
		{
			XmlElement el = elem.OwnerDocument.CreateElement(folder.Name);

			elem.AppendChild(el);

			el.SetAttribute("level", folder.Level.ToString());

			if (folder.Documents.IndexNode != null)
			{
				el.SetAttribute("page", folder.Documents.IndexNode.TargetFilename);
				el.SetAttribute("title", folder.Documents.IndexNode.Meta.Title);
			}
			else
			{
				Console.WriteLine("No index.xml found for folder " + folder.Path);
			}

			el.SetAttribute("path", folder.Path);

			foreach(XmlDocumentFragment fragment in folder.SectionFragments)
			{
				foreach(XmlNode node in fragment.ChildNodes)
				{
					el.AppendChild(el.OwnerDocument.ImportNode(node, true));
				}
			}

			foreach(Folder f in folder.Folders)
			{
				AppendFragments(f, el);
			}
		}

		private void CreateNodeSiteMapFragments(DocumentNode node)
		{
			if (node.NodeType == NodeType.Navigation) return;

			XmlDocumentFragment fragment = node.XmlDoc.CreateDocumentFragment();

			XmlNode parent = fragment;

			if (node.NodeType == NodeType.Ordinary)
			{
				parent = node.XmlDoc.CreateElement(Path.GetFileNameWithoutExtension(node.Filename).Replace(' ', '_'));

				((XmlElement) parent).SetAttribute("level", node.ParentFolder.Level.ToString());
				((XmlElement) parent).SetAttribute("page", node.TargetFilename);
				((XmlElement) parent).SetAttribute("title", node.Meta.Title);
				((XmlElement) parent).SetAttribute("issubpage", "true");

				fragment.AppendChild(parent);
			}

			foreach(XmlElement section in node.XmlDoc.SelectNodes("document/body/section"))
			{
				String sectionId = section.GetAttribute("id");
				XmlNode titleElem = section.SelectSingleNode("title");
				String title = titleElem.FirstChild.Value;

				XmlElement newSection = node.XmlDoc.CreateElement("section");

				if (sectionId != null)
				{
					newSection.SetAttribute("id", sectionId);
				}
				if (title != null)
				{
					newSection.SetAttribute("title", title);
				}

				newSection.SetAttribute("page", node.TargetFilename);
				newSection.SetAttribute("level", node.ParentFolder.Level.ToString());

				parent.AppendChild(newSection);
			}

			node.ParentFolder.SectionFragments.Add(fragment);
		}

		private void PrintStructure(Folder folder, int ident)
		{
			for(int i = 0; i < ident; i++)
			{
				Console.Write(' ');
			}

			Console.Write(folder.Name);
			Console.WriteLine();

			foreach(Folder f in folder.Folders)
			{
				PrintStructure(f, ident + 2);
			}
		}

		private void PrintStructure2(Folder folder, int ident)
		{
			for(int i = 0; i < ident; i++)
			{
				Console.Write(' ');
			}

			Console.Write(folder.Name);
			Console.WriteLine();

			foreach(DocumentNode node in folder.Documents)
			{
				for(int i = 0; i < ident + 2; i++)
				{
					Console.Write(' ');
				}

				Console.Write("- ");
				Console.Write(node.Filename);
				Console.WriteLine();
			}

			foreach(Folder f in folder.Folders)
			{
				PrintStructure2(f, ident + 2);
			}
		}

		private Folder GetFolderInstance(string[] folders)
		{
			Folder current = root;

			foreach(String folderName in folders)
			{
				Folder folder = current.Folders[folderName];

				if (folder == null)
				{
					folder = new Folder(folderName);

					current.Folders[folderName] = folder;
				}

				current = folder;
			}

			return current;
		}
	}
}