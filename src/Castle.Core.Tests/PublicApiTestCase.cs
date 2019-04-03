// Copyright 2004-2019 Castle Project - http://www.castleproject.org/
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

#if DOTNET40 // PublicApiGenerator requires .NET Standard 2.0, and we only need to run it once

namespace Castle
{
	using System;
	using System.IO;
	using System.Reflection;

	using NUnit.Framework;

	using PublicApiGenerator;

	/// <summary>
	/// Development workflow:
	/// - As a developer you run the unit tests and this test will overwrite all the .cs files in the /ref directory for you
	/// - The build server (CI=true env var) will compare the public API of the built assemblies to the /ref files on disk,
	///   the assertion will obviously fail if someone changes the public API without also including the change to the ref/*.cs files
	/// </summary>
	[TestFixture]
	public class PublicApiTestCase
	{
		private static readonly string[] assemblies =
		{
			"Castle.Core",
			"Castle.Services.Logging.log4netIntegration",
			"Castle.Services.Logging.NLogIntegration",
			"Castle.Services.Logging.SerilogIntegration",
		};

		[Test]
		[ExcludeOnFramework(Framework.Mono, "On Mono, the FrameworkDisplayNameAttribute isn't populated")]
		public void PublicApi()
		{
			// Determine if we are in write (developer) or compare (CI server) mode
			string ci = Environment.GetEnvironmentVariable("CI");
			bool compare = string.Equals(ci, "true", StringComparison.OrdinalIgnoreCase);

			// Determine assembly locations
			string testCodeBase = typeof(PublicApiTestCase).Assembly.CodeBase;
			UriBuilder testUri = new UriBuilder(new Uri(testCodeBase));
			string testAssemblyPath = Uri.UnescapeDataString(testUri.Path);
			string testContainingDirectory = Path.GetDirectoryName(testAssemblyPath);

			string configuration = new DirectoryInfo(testContainingDirectory).Parent.Name;
			string rootDir = Path.GetFullPath(Path.Combine(testContainingDirectory, "../../../../.."));

			// Ensure reference source directory exists and is empty
			string refDir = Path.Combine(rootDir, "ref");
			if (!compare)
			{
				Directory.CreateDirectory(refDir);
				foreach (string file in Directory.GetFiles(refDir, "*.cs"))
				{
					File.Delete(file);
				}
			}

			// Process each assembly
			foreach (string assemblyName in assemblies)
			{
				string configurationDir = $"{rootDir}/src/{assemblyName}/bin/{configuration}";
				foreach (string frameworkDir in Directory.GetDirectories(configurationDir))
				{
					string framework = Path.GetFileName(frameworkDir);

					string assemblyBinPath = $"{configurationDir}/{framework}/{assemblyName}.dll";
					string assemblyRefPath = $"{refDir}/{assemblyName}-{framework}.cs";

					var assembly = Assembly.LoadFile(Path.GetFullPath(assemblyBinPath));
					string publicApi = ApiGenerator.GeneratePublicApi(assembly);

					if (compare)
					{
						Assert.IsTrue(File.Exists(assemblyRefPath), $"ref/{assemblyName}-{framework}.cs does not exist");

						string expectedPublicApi = string.Join(Environment.NewLine, File.ReadAllLines(assemblyRefPath));
						Assert.AreEqual(expectedPublicApi, publicApi, $"ref/{assemblyName}-{framework}.cs does not match {assemblyName}.dll");
					}
					else
					{
						File.WriteAllText(assemblyRefPath, publicApi);
					}
				}
			}
		}
	}
}

#endif
