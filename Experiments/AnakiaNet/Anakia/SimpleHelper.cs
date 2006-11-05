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

	public class SimpleHelper
	{
		public String FileSizeInKBytes(String basePath, String file)
		{
			FileInfo info = new FileInfo(Path.Combine(basePath, file));
			
			if (info.Exists)
			{
				return String.Format("{0:#.##}", info.Length / 1024f).ToString();
			}
			
			throw new Exception("File " + info.FullName + " was not found");
		}
		
		public String RemoveOffset(String offset, String path)
		{
			return path.Substring(offset.Length);
		}
		
		public String Relativize(String offset, String path, String page)
		{
			try
			{
				if (offset.Length >= path.Length)
				{
					return String.Format("./{0}", page);
				}
				else
				{
					String newPath = path.Substring(offset.Length);
					return String.Format(".{0}/{1}", newPath, page);
				}
			}
			catch(Exception)
			{
				throw;
			}
		}
		
		public String GetShortTypeName(String typeName)
		{
			String[] parts = typeName.Split('.');
			
			return parts[parts.Length - 1];
		}
		
		public int GetLevels(String path)
		{
			return path.Split('/').Length - 1;
		}
		
		public Stack CreateStack()
		{
			return new Stack();
		}
		
		public int Push(Stack stack)
		{
			stack.Push(String.Empty);
			return stack.Count;
		}
		
		public int Pop(Stack stack)
		{
			stack.Pop();
			return stack.Count;
		}
	}
}
