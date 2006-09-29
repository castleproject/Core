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

	public class Folder
	{
		private String name;
		private Folder parent;
		private DocumentNodeCollection documents;
		private FolderCollection folders;
		private ArrayList fragments = new ArrayList();
		private ArrayList breadCrumbs = new ArrayList();

		public Folder(String name)
		{
			this.name = name;
			documents = new DocumentNodeCollection(this);
			folders = new FolderCollection(this);
		}

		public ArrayList BreadCrumbs
		{
			get { return breadCrumbs; }
		}

		public string Path
		{
			get
			{
				if (parent != null)
				{
					return parent.Path + "/" + name;
				}
				else
				{
					return name;
				}
			}
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public Folder Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		public ArrayList SectionFragments
		{
			get { return fragments; }
		}

		public DocumentNodeCollection Documents
		{
			get { return documents; }
		}

		public FolderCollection Folders
		{
			get { return folders; }
		}
		
		public int Level
		{
			get { return 1 + (Parent != null ? Parent.Level : 0); }
		}
	}
}
