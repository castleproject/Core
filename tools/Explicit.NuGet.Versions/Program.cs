// Copyright 2004-2018 Castle Project - http://www.castleproject.org/
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Ionic.Zip;

namespace Explicit.NuGet.Versions
{
	class Program
	{
		static void Main(string[] args)
		{
			var packageDiscoveryDirectory = Path.Combine(Environment.CurrentDirectory, args[0]);
			var packageDiscoverDirectoryInfo = new DirectoryInfo(packageDiscoveryDirectory);
			var packageMetaData = ReadNuspecFromPackages(packageDiscoverDirectoryInfo);
			UpdateNuspecManifestContent(packageMetaData, args[1]);
			WriteNuspecToPackages(packageMetaData);
		}

		private static void WriteNuspecToPackages(Dictionary<string, NuspecContentEntry> packageMetaData)
		{
			foreach (var packageFile in packageMetaData.ToList())
			{
				using (var zipFile = ZipFile.Read(packageFile.Key))
				{
					zipFile.UpdateEntry(packageFile.Value.EntryName, packageFile.Value.Contents);
					zipFile.Save();
				}
			}
		}

		private static void UpdateNuspecManifestContent(Dictionary<string, NuspecContentEntry> packageMetaData, string dependencyNugetId)
		{
			foreach (var packageFile in packageMetaData.ToList())
			{
				var nuspecXmlDocument = new XmlDocument();
				nuspecXmlDocument.LoadXml(packageFile.Value.Contents);

				SetPackageDepencyVersionsToBeExplicitForXmlDocument(nuspecXmlDocument, dependencyNugetId);

				string updatedNuspecXml;
				using (var writer = new StringWriterWithEncoding(Encoding.UTF8))
				using (var xmlWriter = new XmlTextWriter(writer) { Formatting = Formatting.Indented })
				{
					nuspecXmlDocument.Save(xmlWriter);
					updatedNuspecXml = writer.ToString();
				}

				packageMetaData[packageFile.Key].Contents = updatedNuspecXml;
			}
		}

		private static void SetPackageDepencyVersionsToBeExplicitForXmlDocument(XmlDocument nuspecXmlDocument, string nugetIdFilter)
		{
			WalkDocumentNodes(nuspecXmlDocument.ChildNodes, node => {
				if (node.Name.ToLowerInvariant() == "dependency" && !string.IsNullOrEmpty(node.Attributes["id"].Value) && node.Attributes["id"].Value.ToLowerInvariant().StartsWith(nugetIdFilter))
				{
					var currentVersion = node.Attributes["version"].Value;
					if (!node.Attributes["version"].Value.StartsWith("[") && !node.Attributes["version"].Value.EndsWith("]"))
					{
						node.Attributes["version"].Value = $"[{currentVersion}]";
					}
				}
			});
		}

		internal class NuspecContentEntry
		{
			public string EntryName { get; set; }
			public string Contents { get; set; }
		}

		private static Dictionary<string, NuspecContentEntry> ReadNuspecFromPackages(DirectoryInfo packageDiscoverDirectoryInfo)
		{
			var packageNuspecDictionary = new Dictionary<string, NuspecContentEntry>();
			foreach (var packageFilePath in packageDiscoverDirectoryInfo.GetFiles("*.nupkg", SearchOption.AllDirectories))
			{
				using (var zipFile = ZipFile.Read(packageFilePath.FullName))
				{
					foreach (var zipEntry in zipFile.Entries)
					{
						if (zipEntry.FileName.ToLowerInvariant().EndsWith(".nuspec"))
						{
							using (var zipEntryReader = new StreamReader(zipEntry.OpenReader()))
							{
								var nuspecXml = zipEntryReader.ReadToEnd();
								packageNuspecDictionary[packageFilePath.FullName] = new NuspecContentEntry {
									Contents = nuspecXml,
									EntryName = zipEntry.FileName
								};
								break;
							}
						}
					}
				}
			}

			return packageNuspecDictionary;
		}

		private static void WalkDocumentNodes(XmlNodeList nodes, Action<XmlNode> callback)
		{
			foreach (XmlNode node in nodes)
			{
				callback(node);
				WalkDocumentNodes(node.ChildNodes, callback);
			}
		}
	}

	public sealed class StringWriterWithEncoding : StringWriter
	{
		private readonly Encoding encoding;

		public StringWriterWithEncoding(Encoding encoding)
		{
			this.encoding = encoding;
		}

		public override Encoding Encoding
		{
			get { return encoding; }
		}
	}
}