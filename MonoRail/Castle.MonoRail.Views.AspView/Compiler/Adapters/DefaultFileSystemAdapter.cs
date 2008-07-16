// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Views.AspView.Compiler.Adapters
{
	using System.IO;
	using System.Text;

	public class DefaultFileSystemAdapter : IFileSystemAdapter
	{
		public void Delete(string fileName)
		{
			if (File.Exists(fileName))
                File.Delete(fileName);
		}

		public void Create(DirectoryInfo directory)
		{
			directory.Create();
		}

		public bool Exists(DirectoryInfo directory)
		{
			return directory.Exists;
		}

		public void ClearSourceFilesFrom(DirectoryInfo directory)
		{
			foreach (FileInfo file in directory.GetFiles("*.cs"))
			{
				file.Delete();
			}
		}

		public void Save(string fileName, string content, DirectoryInfo directory)
		{
			File.WriteAllText(Path.Combine(directory.FullName, fileName), content, Encoding.UTF8);
		}

		public string[] GetSourceFilesFrom(DirectoryInfo directory)
		{
			return Directory.GetFiles(directory.FullName, "*.cs", SearchOption.TopDirectoryOnly);
		}
	}
}
