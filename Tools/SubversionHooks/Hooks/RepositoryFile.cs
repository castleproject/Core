// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.SvnHooks
{
	using System;
	using System.IO;

	/// <summary>
	/// Summary description for IRepositoryFile.
	/// </summary>
	public class RepositoryFile : IDisposable
	{
		public RepositoryFile(IRepository repository, String path, RepositoryStatus contentsStatus, RepositoryStatus propertiesStatus)
		{
			if (path == null)
				throw new ArgumentNullException("path");
			if (path.Trim().Length == 0)
				throw new ArgumentException("Path must be set to a valid path", "path");
			if (path[path.Length-1] == '/')
				throw new ArgumentException("Path must be set to a file, not a directory", "path");
			if (propertiesStatus == RepositoryStatus.Added ||
				propertiesStatus == RepositoryStatus.Deleted)
			{
				throw new ArgumentException("Properties status cannot be set to Added or Deleted, use Updated", "propertiesStatus");
			}
			
			this.contentsStatus = contentsStatus;
			this.propertiesStatus = propertiesStatus;
			this.repository = repository;
			SetPathRelatedFields(path);

			if (fileName.EndsWith(" "))
				throw new ArgumentException("Filename cannot end with trailing spaces", "path");

			if (fileName.StartsWith(" "))
				throw new ArgumentException("Filename cannot begin with leading spaces", "path");

		}

		private void SetPathRelatedFields(String path)
		{
			// Set the path
			this.path = path;

			// Extract the file name from the path
			if (path.IndexOf('/') == -1)
			{
				fileName = path;
			}
			else
			{
				fileName = path.Substring(path.LastIndexOf('/')+1);
			}
			
			// Extract the extension from the file name
			if (fileName.IndexOf('.') == -1)
			{
				extension = String.Empty;
			}
			else
			{
				extension = fileName.Substring(fileName.LastIndexOf('.') + 1);
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			if(contents != null)
			{
				contents.Close();
				contents = null;
			}
		}


		#endregion

		public override string ToString()
		{
			return Path;	
		}

		public Stream GetContents()
		{
			if (!IsText)
				throw new InvalidOperationException("Cannot get the contents of a binary file");

			if (contents == null)
			{
				this.contents = repository.GetFileContents(Path);
			}

			contents.Seek(0, SeekOrigin.Begin);

			return new ReadOnlyStreamWrapper(contents);
		}

		public String[] GetProperty(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			return repository.GetProperty(Path, name);
		}


		public RepositoryStatus ContentsStatus
		{
			get { return contentsStatus; }
		}

		public RepositoryStatus PropertiesStatus
		{
			get { return propertiesStatus; }
		}

        public String Extension
		{
			get { return extension; }
		}
		public String FileName
		{
			get { return fileName; }
		}
		public String Path
		{
			get { return path; }
		}
		/// <value>
		/// The MIME type of the file it must always 
		/// return a valid string, if the MIME type
		/// is not known by Subversion it should 
		/// default to "text/plain"
		/// </value>
		public String MimeType
		{
			get 
			{
				if (mimeType == null)
				{
					String[] props = GetProperty("svn:mime-type");
				
					if (props == null)
					{
						mimeType = "text/plain";
					}
					else
					{
						mimeType = props[0];
					}
				}

				return mimeType;
			}
		}

		public bool IsText
		{
			get 
			{ 
				return MimeType.StartsWith("text/");
			}
		}


		private IRepository repository;
		private RepositoryStatus contentsStatus;
		private RepositoryStatus propertiesStatus;
		private String path;		
		private String fileName;
		private String extension;
		private String mimeType = null;
		private Stream contents = null;

	}
}
