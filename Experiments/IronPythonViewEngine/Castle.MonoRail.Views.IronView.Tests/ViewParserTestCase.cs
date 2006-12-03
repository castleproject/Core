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

namespace Castle.MonoRail.Views.IronView.Tests
{
	using System;
	using System.IO;
	using NUnit.Framework;

	[TestFixture]
	public class ViewParserTestCase
	{
		[Test]
		public void CreateScripts()
		{
//			ViewParser parser = new ViewParser();
//			
//			String[] files = Directory.GetFiles("../../ViewsToTest", "*.py");
//			
//			foreach(String file in files)
//			{
//				String resultFileName =
//					Path.Combine(Path.GetDirectoryName(file), 
//					             String.Format("{0}-result.txt", Path.GetFileNameWithoutExtension(file)));
//
//				using(StreamReader reader = new StreamReader(file))
//				{
//					String result = parser.CreateScriptFromFile(reader, file);
//
//					// Debug.WriteLine(result);
//					Console.WriteLine(result);
//
//					AssertResultEqualsToResultFileContent(result, resultFileName);
//				}
//			}
		}

		private void AssertResultEqualsToResultFileContent(string script, string resultFileName)
		{
			using(StreamReader reader = new StreamReader(resultFileName))
			{
				String fileContent = reader.ReadToEnd();
				
				Assert.AreEqual(fileContent, script, "Resulting script should be equal");
			}
		}
	}
}
