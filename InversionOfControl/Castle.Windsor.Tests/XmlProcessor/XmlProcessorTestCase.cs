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

namespace Castle.Windsor.Tests.XmlProcessor
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Text.RegularExpressions;
	using System.Xml;
	
	using NUnit.Framework;

	using Castle.Windsor.Configuration.Interpreters.XmlProcessor;

	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	[TestFixture]
	public class XmlProcessorTestCase
	{
		String dir = "../Castle.Windsor.Tests/XmlProcessor/TestFiles/";

		[Test]
		public void InvalidFiles()
		{
			String dirFullPath = GetFullPath();

			foreach(String fileName in Directory.GetFiles(dirFullPath, "Invalid*.xml"))
			{
				try
				{
					XmlDocument doc = GetXmlDocument(fileName);
					XmlProcessor processor = new XmlProcessor();

					XmlNode result = processor.Process(doc.DocumentElement);

					Assert.Fail(fileName + " should throw an exception");
				}
				catch(XmlProcessorException e)
				{
					Debug.WriteLine("Expected exception:" + e.Message);
				}
			}
		}

		/// <summary>
		/// Runs the tests.
		/// </summary>
		[Test]
		public void RunTests()
		{
			String dirFullPath = GetFullPath();

			foreach(String fileName in Directory.GetFiles(dirFullPath, "*Test.xml"))
			{
				// Debug.WriteLine("Running " + fileName.Substring( fileName.LastIndexOf("/") + 1 ));

				XmlDocument doc = GetXmlDocument(fileName);

				String resultFileName = fileName.Substring(0, fileName.Length - 4) + "Result.xml";

				XmlDocument resultDoc = GetXmlDocument(resultFileName);

				XmlProcessor processor = new XmlProcessor();

				try
				{
					XmlNode result = processor.Process(doc.DocumentElement);

					String resultDocStr = StripSpaces(resultDoc.OuterXml);
					String resultStr = StripSpaces(result.OuterXml);

					// Debug.WriteLine(resultDocStr);
					// Debug.WriteLine(resultStr);

					Assert.AreEqual(resultDocStr, resultStr);
				}
				catch(Exception e)
				{
					throw new Exception("Error processing " + fileName, e);
				}
			}
		}

		#region Helpers

		public XmlDocument GetXmlDocument(string fileName)
		{
			XmlDocument doc = new XmlDocument();

			doc.Load(fileName);

			return doc;
		}

		private string StripSpaces(String xml)
		{
			return Regex.Replace(xml, "\\s+", "", RegexOptions.Compiled);
		}

		private string GetFullPath()
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir);
		}

		#endregion
	}
}