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
	using Manoli.Utils.CSharpFormat;
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
		private String baseGenFolder;

		private VelocityEngine velocity;
		private Template template;
		private Folder root;
		private XmlDocument siteMapDoc;
		private XmlDocument projectDom;
		
		// Formatters
		
		private CSharpFormat csharpFormatter = new CSharpFormat();
		private HtmlFormat htmlFormatter = new HtmlFormat();
		private JavaScriptFormat jsFormatter = new JavaScriptFormat();
		private TsqlFormat tsqlFormatter = new TsqlFormat();
		private VisualBasicFormat vbFormatter = new VisualBasicFormat();
		private int orderCount = int.MinValue;

		public AnakiaTask()
		{
		}

		#region Properties

		[TaskAttribute("basefolder")]
		public string BaseGenFolder
		{
			get { return baseGenFolder; }
			set { baseGenFolder = value; }
		}

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

				foreach (String fullFileName in sourceFileSet.FileNames)
				{
					string lastProcessedFile = null;

					try
					{
						lastProcessedFile = fullFileName;

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
					catch (Exception ex)
					{
						Console.WriteLine("File: {0} \r\n", lastProcessedFile);
						Console.WriteLine(ex);
						Console.WriteLine("\r\n --------------------------------------------------------");
					}
				}

				siteMapDoc = CreateSiteMap();

				siteMapDoc.Save(Path.Combine(TargetDir.FullName, "generatedsitemap.xml"));

				ITreeWalker walker = new BreadthFirstWalker();

				walker.Walk(root, new Act(AssignNavigationDocToFolders));
				walker.Walk(root, new Act(CreateBreadCrumb));
				walker.Walk(root, new Act(FixRelativePaths));
				walker.Walk(root, new Act(CreateHtml));

				foreach (FileToCopy file2Copy in staticFilesToCopy)
				{
					String dir = Path.GetDirectoryName(file2Copy.TargetFile);

					EnsureDirExists(dir);

					String targetFile = targetDir.FullName + "/" + file2Copy.TargetFile;

					if (File.Exists(targetFile))
					{
						if (File.GetLastWriteTime(targetFile) >= File.GetLastWriteTime(file2Copy.SourceFile))
						{
							continue;
						}

						File.Delete(targetFile);
					}

					File.Copy(file2Copy.SourceFile, targetFile);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				Console.Read();
			}
		}

		#endregion
		
		private void AssignNavigationDocToFolders(DocumentNode node)
		{
			if (node.NodeType != NodeType.Navigation)
			{
				return;
			}
			
			node.ParentFolder.NavigationNode = node;
		}

		private bool IsStaticFile(string fileName)
		{
			String ext = Path.GetExtension(fileName);

			return ext != ".xml";
		}

		private DocumentMeta CreateMeta(XmlDocument xmlDocument)
		{
			DocumentMeta meta = new DocumentMeta();

			XmlNode order = xmlDocument.SelectSingleNode("document/@order");
			
			if (order != null)
			{
				meta.Order = Convert.ToInt32(order.Value);
			}
			else
			{
				meta.Order = orderCount++;
			}
			
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
			try
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
			catch(Exception ex)
			{
				throw new Exception("Error generating html for " + node.TargetFilename + 
					" at " + node.ParentFolder.Path, ex);
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

			context.Put("cs", csharpFormatter);
			context.Put("html", htmlFormatter);
			context.Put("js", jsFormatter);
			context.Put("tsql", tsqlFormatter);
			context.Put("vb", vbFormatter);
			
			context.Put("basefolder", BaseGenFolder);
			context.Put("breadcrumbs", node.BreadCrumbs);
			context.Put("meta", node.Meta);
			context.Put("doc", node.XmlDoc);
			context.Put("root", node.XmlDoc.DocumentElement);
			context.Put("node", node);
			context.Put("sitemap", siteMapDoc.DocumentElement);
			context.Put("project", projectDom.DocumentElement);
			context.Put("folder", node.ParentFolder);
			context.Put("converter", new SimpleConverter());
			context.Put("helper", new SimpleHelper());
			
			if (node.ParentFolder.NavigationNode != null)
			{
				context.Put("navigation", node.ParentFolder.NavigationNode.XmlDoc);
			}
			
			context.Put("navlevel", node.ParentFolder.NavigationLevel);

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
		
		#region BreadCrumb related operations

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
			
			String title = folder.Name;
			
			if (folder.Documents.IndexNode != null)
			{
				title = folder.Documents.IndexNode.Meta.Title;
			}

			folder.BreadCrumbs.Add(new BreadCrumb(folder.Path + "/index.html", title));
		}
		
		#endregion

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
			
			int level = node.ParentFolder.Level;

			if (node.NodeType == NodeType.Ordinary)
			{
				level++;
				
				parent = node.XmlDoc.CreateElement(Path.GetFileNameWithoutExtension(node.Filename).Replace(' ', '_'));

				((XmlElement) parent).SetAttribute("level", node.ParentFolder.Level.ToString());
				((XmlElement) parent).SetAttribute("page", node.TargetFilename);
				((XmlElement) parent).SetAttribute("title", node.Meta.Title);
				((XmlElement) parent).SetAttribute("issubpage", "true");
				((XmlElement) parent).SetAttribute("path", node.ParentFolder.Path);

				fragment.AppendChild(parent);
			}

			foreach(XmlElement section in node.XmlDoc.SelectNodes("document/body/section"))
			{
				XmlElement newSection = CreateSectionXmlElement(level, node, section);
				
				parent.AppendChild(newSection);
				
				foreach(XmlElement secSectionLevel in section.SelectNodes("section"))
				{
					XmlElement newSectionSecLevel = CreateSectionXmlElement(level + 1, node, secSectionLevel);
				
					newSection.AppendChild(newSectionSecLevel);
					
					foreach(XmlElement thirdSectionLevel in secSectionLevel.SelectNodes("section"))
					{
						XmlElement newSectionThrdLevel = CreateSectionXmlElement(level + 2, node, thirdSectionLevel);
				
						newSectionSecLevel.AppendChild(newSectionThrdLevel);
					}
				}
			}

			node.ParentFolder.SectionFragments.Add(fragment);
		}

		private static XmlElement CreateSectionXmlElement(int level, DocumentNode node, XmlElement section)
		{
			String sectionId = section.GetAttribute("id");
			XmlNode titleElem = section.SelectSingleNode("title");
			String title = titleElem.FirstChild.Value;

			XmlElement newSection = node.XmlDoc.CreateElement("section");

			if (sectionId != String.Empty)
			{
				newSection.SetAttribute("id", sectionId);
			}
			if (title != null)
			{
				newSection.SetAttribute("title", title);
			}

			newSection.SetAttribute("page", node.TargetFilename);
			newSection.SetAttribute("level",level.ToString());

			return newSection;
		}

		private void FixRelativePaths(DocumentNode node)
		{
			if (node.XmlDoc == null) return;
			
			int level = node.ParentFolder.Level;
			
			XmlNodeList nodes = node.XmlDoc.SelectNodes("//@relative");
			
			foreach(XmlAttribute xmlNode in nodes)
			{
				XmlElement elem = xmlNode.OwnerElement;
				
				String relative = elem.GetAttribute("relative");
				
				if (relative.StartsWith("!"))
				{
					relative = relative.Substring(1);
					elem.SetAttribute("relative", relative);
				}
				else
				{
					elem.RemoveAttribute("relative");
				}
				
				elem.SetAttribute("src", Relativize(level, relative));
			}
		}

		private string Relativize(int level, string relativePath)
		{
			String newPath = "./";
			
			for(int i=1; i < level; i++)
			{
				newPath += "../";
			}
			
			if (relativePath[0] == '/')
			{
				return newPath + relativePath.Substring(1);
			}
			
			return newPath + relativePath;
		}

//		private void PrintStructure(Folder folder, int ident)
//		{
//			for(int i = 0; i < ident; i++)
//			{
//				Console.Write(' ');
//			}
//
//			Console.Write(folder.Name);
//			Console.WriteLine();
//
//			foreach(Folder f in folder.Folders)
//			{
//				PrintStructure(f, ident + 2);
//			}
//		}
//
//		private void PrintStructure2(Folder folder, int ident)
//		{
//			for(int i = 0; i < ident; i++)
//			{
//				Console.Write(' ');
//			}
//
//			Console.Write(folder.Name);
//			Console.WriteLine();
//
//			foreach(DocumentNode node in folder.Documents)
//			{
//				for(int i = 0; i < ident + 2; i++)
//				{
//					Console.Write(' ');
//				}
//
//				Console.Write("- ");
//				Console.Write(node.Filename);
//				Console.WriteLine();
//			}
//
//			foreach(Folder f in folder.Folders)
//			{
//				PrintStructure2(f, ident + 2);
//			}
//		}

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
