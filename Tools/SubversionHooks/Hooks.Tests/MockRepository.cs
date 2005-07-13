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

namespace Castle.SvnHooks.Tests
{
	using System;
	using System.Collections;
	using System.Text;
	
	using Castle.SvnHooks;

	/// <summary>
	/// Summary description for MockRepository.
	/// </summary>
	public class MockRepository : IRepository
	{
		public MockRepository()
		{
		}

		public void AddFile(String path, String author, IDictionary properties, String contents)
		{
			byte[] byteContents = null;
			if (contents != null)
			{
				byteContents = Encoding.ASCII.GetBytes(contents);
			}
				
			Files.Add(path, new File(path, author, properties, byteContents));
		}

		#region IRepository Members

		public string[] GetProperty(string path, string property)
		{
			File file = Files[path] as File;
			if (file == null || file.Properties == null)
				return null;

			return (String[])file.Properties[property];
		}

		public System.IO.Stream GetFileContents(string path)
		{
			File file = Files[path] as File;
			if (file == null || file.Contents == null)
				return null;
            
			return new System.IO.MemoryStream(file.Contents, false);
		}


		#endregion

		private readonly IDictionary Files = new Hashtable();
		
		private class File
		{
			public File(String path, String author, IDictionary properties, byte[] contents)
			{
				this.properties = properties;
				this.contents = contents;
			}

			public IDictionary Properties
			{
				get { return properties; }
			}
			public byte[] Contents
			{
				get { return contents; }
			}

			private IDictionary properties;
			private byte[] contents;
		}
	}
}
