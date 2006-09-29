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
	using System.Xml;

	public enum NodeType
	{
		Index,
		Navigation,
		Ordinary
	}
	
	public class DocumentNode
	{
		private NodeType nodeType;
		private String fileName;
		private String path;
		private DocumentMeta meta;
		private XmlDocument xmldoc;
		private Folder parentFolder;
		private ArrayList breadCrumbs = new ArrayList();

		public DocumentNode(String path, String fileName, XmlDocument doc, DocumentMeta meta)
		{
			this.path = path;
			this.fileName = fileName;
			this.meta = meta;
			
			xmldoc = doc;
			
			if (fileName.ToLower() == "navigation.xml")
			{
				nodeType = NodeType.Navigation;
			}
			else if (fileName.ToLower() == "index.xml")
			{
				nodeType = NodeType.Index;
			}
			else
			{
				nodeType = NodeType.Ordinary;
			}
		}

		public Folder ParentFolder
		{
			get { return parentFolder; }
			set { parentFolder = value; }
		}

		public NodeType NodeType
		{
			get { return nodeType; }
		}

		public string Filename
		{
			get { return fileName; }
		}
		
		public string TargetFilename
		{
			get { return System.IO.Path.GetFileNameWithoutExtension(fileName) + ".html"; }
		}

		public string Path
		{
			get { return path; }
		}

		public XmlDocument XmlDoc
		{
			get { return xmldoc; }
		}

		public ArrayList BreadCrumbs
		{
			get { return breadCrumbs; }
		}

		public DocumentMeta Meta
		{
			get { return meta; }
			set { meta = value; }
		}
	}
}
